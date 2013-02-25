#define ALL_LAMBDAS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FEMLibrary.SolidMechanics.Physics;
using FEMLibrary.SolidMechanics.Meshing;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Utils;
using FEMLibrary.SolidMechanics.NumericalUtils;
using System.Diagnostics;
using FEMLibrary.SolidMechanics.Results;
using FEMLibrary.SolidMechanics.ODE;
using System.IO;

namespace FEMLibrary.SolidMechanics.Solving
{
    public class NewmarkVibrationNonLinearSolver:Solver
    {
        public NewmarkVibrationNonLinearSolver(Model model, Mesh mesh, double error, Vector init, double maxTime, int intervalsTime)
            : base(model, mesh)
        {
            _error = error;
            _init = init;
            _maxTime = maxTime;
            _intervalsTime = intervalsTime;
        }


        private double _maxTime;
        private int _intervalsTime;

        private Vector _init;

        private double _error;
        private Matrix ConstMatrix;

        private double M1;
        private double M2;
        private double M3;
        private double G13;

        IFiniteElement elementCurrent;
        Vector previousU;

        List<int> _indeciesToDelete;

        public override IEnumerable<INumericalResult> Solve(int resultsCount)
        {
            List<INumericalResult> results = new List<INumericalResult>();
            
            if (_mesh.IsMeshGenerated)
            {
                GetConstantMatrix();
                _indeciesToDelete = getIndeciesWithStaticBoundaryConditions();
                Matrix StiffnessMatrix = AsumeStaticBoundaryConditions(GetStiffnessMatrix(), _indeciesToDelete);
                Matrix MassMatrix = AsumeStaticBoundaryConditions(GetMassMatrix(), _indeciesToDelete);

                _init = AsumeStaticBoundaryConditionsToVector(_init, _indeciesToDelete);

                CauchyProblemResult newmarkResult = NewmarkSolver(MassMatrix, StiffnessMatrix, _init, 0.5, 0.5, _maxTime, _intervalsTime);
                
                SemidiscreteVibrationsNumericalResult result = new SemidiscreteVibrationsNumericalResult(_mesh.Elements, newmarkResult.DeltaTime, _maxTime);
                foreach (Vector v in newmarkResult.Results) {
                    Vector resultWithStaticPoints = addStaticPoints(v, _indeciesToDelete);
                    result.AddResult(resultWithStaticPoints);
                }

                results.Add(result);
            }
            return results;
        }

        private CauchyProblemResult NewmarkSolver(Matrix massMatrix, Matrix stiffnessMatrix, Vector initValue, double gamma, double alfa, double maxTime, int intervals)
        {
            double deltaTime = maxTime / intervals;
            CauchyProblemResult result = new CauchyProblemResult(deltaTime);

            double a1 = (2 / (deltaTime * deltaTime * gamma));
            double a2 = (2 / (deltaTime * gamma));
            double a3 = ((1 / gamma) - 1);

            double a4 = deltaTime * (1-alfa); 
            double a5 = deltaTime * alfa;

            Vector u = initValue;
            Vector du = new Vector(initValue.Length);
            Vector ddu = massMatrix.LUalgorithm((-1) * stiffnessMatrix * initValue);

            result.AddResult(u);

            Matrix K = massMatrix * a1 + stiffnessMatrix;
            for (double t = deltaTime; t <= maxTime; t += deltaTime)
            {
                Vector uprev = u;
                Vector duprev = du;
                Vector dduprev = ddu;

                Vector F = massMatrix * (a1 * uprev + a2 * duprev + a3 * dduprev);
                u = K.LUalgorithm(F);

                ddu = a1 * (u - uprev) - a2 * duprev - a3 * dduprev;
                du = duprev + a4 * dduprev + a5 * ddu;

                ApplyNonlinearity(massMatrix, stiffnessMatrix, ref u, ref du, ref ddu, a1, a2, a3, a4, a5);

                result.AddResult(u);
            }

            return result;
        }

        private void ApplyNonlinearity(Matrix massMatrix, Matrix stiffnessMatrix, ref Vector u, ref Vector du, ref Vector ddu, double a1, double a2, double a3, double a4, double a5)
        {
            Matrix nonlinearMatrix;
            Vector uprev;
            Vector duprev;
            Vector dduprev;

            do
            {
                uprev = u;
                duprev = du;
                dduprev = ddu;
                nonlinearMatrix = AsumeStaticBoundaryConditions(GetNonlinearMatrix(uprev), _indeciesToDelete);
                Matrix K_hat = stiffnessMatrix + nonlinearMatrix;
                Vector F_hat = (-1) * massMatrix * dduprev;
                u = K_hat.LUalgorithm(F_hat);
                ddu = a1 * (u - uprev) - a2 * duprev - a3 * dduprev;
                du = duprev + a4 * dduprev + a5 * ddu;
            }
            while (Vector.Norm(u - uprev) < _error);
        }

        #region Nonliear Stiffness Matrix
        private Matrix GetNonlinearMatrix(Vector u)
        {
            Vector uWithStaticPoints = addStaticPoints(u, _indeciesToDelete);
            Matrix NonlinearMatrix = new Matrix(_mesh.Nodes.Count * 2, _mesh.Nodes.Count * 2);
            foreach (IFiniteElement element in _mesh.Elements)
            {
                Matrix localMassMatrix = GetLocalNonlinearMatrix(element, uWithStaticPoints);

                for (int i = 0; i < element.Count; i++)
                {
                    for (int j = 0; j < element.Count; j++)
                    {
                        NonlinearMatrix[2 * element[i].Index, 2 * element[j].Index] += localMassMatrix[2 * i, 2 * j];
                        NonlinearMatrix[2 * element[i].Index + 1, 2 * element[j].Index] += localMassMatrix[2 * i + 1, 2 * j];
                        NonlinearMatrix[2 * element[i].Index, 2 * element[j].Index + 1] += localMassMatrix[2 * i, 2 * j + 1];
                        NonlinearMatrix[2 * element[i].Index + 1, 2 * element[j].Index + 1] += localMassMatrix[2 * i + 1, 2 * j + 1];
                    }
                }
            }

            return NonlinearMatrix;
        }

        private Matrix GetLocalNonlinearMatrix(IFiniteElement element, Vector u)
        {
            elementCurrent = element;
            previousU = u;

            Matrix localMassMatrix =  Integration.GaussianIntegrationMatrix(LocalNonlinearMatrixFunction);

            return localMassMatrix;
        }

        private Matrix LocalNonlinearMatrixFunction(double ksi, double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, ksi, eta);

            Matrix derivativeUMatrix = GetLocalUDerivativeMatrix(previousU, elementCurrent, ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return derivativeMatrix * ConstMatrix * Matrix.Transpose(derivativeUMatrix) * Matrix.Transpose(derivativeMatrix) * J.GetJacobianDeterminant(ksi, eta);
        }

        

        private Matrix GetLocalUDerivativeMatrix(Vector previousU, IFiniteElement elementCurrent, double ksi, double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, ksi, eta);
            Vector uOnElement = new Vector(8);
            if (previousU != null)
            {
                for (int i = 0; i < elementCurrent.Count; i++)
                {
                    uOnElement[2 * i] = previousU[2 * elementCurrent[i].Index];
                    uOnElement[2 * i + 1] = previousU[2 * elementCurrent[i].Index + 1];
                }
            }

            Vector uDerivative = Matrix.Transpose(derivativeMatrix) * uOnElement;
            Matrix derivativeUMatrix = new Matrix(4, 4);
            derivativeUMatrix[0, 0] = derivativeUMatrix[2, 2] = uDerivative[0];
            derivativeUMatrix[1, 1] = derivativeUMatrix[3, 3] = uDerivative[1];
            derivativeUMatrix[3, 0] = derivativeUMatrix[1, 2] = uDerivative[2];
            derivativeUMatrix[2, 1] = derivativeUMatrix[0, 3] = uDerivative[3];
            return derivativeUMatrix;

        }


        #endregion

        #region MassMatrix

        private Matrix GetMassMatrix()
        {
            Matrix MassMatrix = new Matrix(_mesh.Nodes.Count * 2, _mesh.Nodes.Count * 2);
            foreach (IFiniteElement element in _mesh.Elements)
            {
                Matrix localMassMatrix = GetLocalMassMatrixMatrix(element);

                for (int i = 0; i < element.Count; i++)
                {
                    for (int j = 0; j < element.Count; j++)
                    {
                        MassMatrix[2 * element[i].Index, 2 * element[j].Index] += localMassMatrix[2 * i, 2 * j];
                        MassMatrix[2 * element[i].Index + 1, 2 * element[j].Index] += localMassMatrix[2 * i + 1, 2 * j];
                        MassMatrix[2 * element[i].Index, 2 * element[j].Index + 1] += localMassMatrix[2 * i, 2 * j + 1];
                        MassMatrix[2 * element[i].Index + 1, 2 * element[j].Index + 1] += localMassMatrix[2 * i + 1, 2 * j + 1];
                    }
                }
            }

            return MassMatrix;
        }

        private Matrix GetLocalMassMatrixMatrix(IFiniteElement element)
        {
            elementCurrent = element;

            Matrix localMassMatrix = _model.Material.Rho * Integration.GaussianIntegrationMatrix(LocalMassMatrixFunction);

            return localMassMatrix;
        }

        private Matrix LocalMassMatrixFunction(double ksi, double eta)
        {
            Matrix baseFunctionsMatrix = GetLocalBaseFunctionsMatrix(ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return baseFunctionsMatrix * Matrix.Transpose(baseFunctionsMatrix) * J.GetJacobianDeterminant(ksi, eta);
        }

        private Matrix GetLocalBaseFunctionsMatrix(double ksi, double eta)
        {
            Matrix matrix = new Matrix(8, 2);
            matrix[0, 0] = matrix[1, 1] = 0.25 * (1 - ksi) * (1 - eta);
            matrix[2, 0] = matrix[3, 1] = 0.25 * (1 + ksi) * (1 - eta);
            matrix[4, 0] = matrix[5, 1] = 0.25 * (1 + ksi) * (1 + eta);
            matrix[6, 0] = matrix[7, 1] = 0.25 * (1 - ksi) * (1 + eta);
            return matrix;
        }


        #endregion

        #region Const Matrix
        
        private Matrix GetConstantMatrix()
        {
            ConstMatrix = new Matrix(4, 4);
            M1 = ConstMatrix[0, 0] = _model.Material.GetE1Modif() * (1 + _model.Material.GetAlfa1());
            M2 = ConstMatrix[1, 0] = ConstMatrix[0, 1] = _model.Material.GetLambda1() * _model.Material.GetE0();
            M3 = ConstMatrix[1, 1] = _model.Material.GetE0();
            G13 = ConstMatrix[2, 2] = ConstMatrix[2, 3] = ConstMatrix[3, 2] = ConstMatrix[3, 3] = _model.Material.GetG13();
            return ConstMatrix;
        }

        #endregion

        #region Boundary conditions

        private Vector AsumeStaticBoundaryConditionsToVector(Vector result, List<int> indeciesToDelete)
        {
            indeciesToDelete.Sort();
            indeciesToDelete.Reverse();

            foreach (int index in indeciesToDelete)
            {
                result = Vector.Cut(result, index);
            }
            return result;
        }

        //static == fixed
        private Matrix AsumeStaticBoundaryConditions(Matrix stiffnessMatrix, List<int> indeciesToDelete)
        {
            indeciesToDelete.Sort();
            indeciesToDelete.Reverse();

            foreach (int index in indeciesToDelete) {
                stiffnessMatrix = Matrix.Cut(stiffnessMatrix, index, index);
            }
            return stiffnessMatrix;
        }

        private List<int> getIndeciesWithStaticBoundaryConditions()
        {
            List<int> indecies = new List<int>();

            foreach (Edge edge in _model.Shape.Edges)
            {
                BoundaryCondition boundaryCondition = _model.BoundaryConditions[edge];
                if (boundaryCondition.Type == BoundaryConditionsType.Static)
                {
                    IEnumerable<FiniteElementNode> nodes = _mesh.GetNodesOnEdge(edge);
                    foreach (FiniteElementNode node in nodes)
                    {
                        if (!indecies.Contains(2 * node.Index))
                        {
                            indecies.Add(2 * node.Index);
                        }
                        if (!indecies.Contains(2 * node.Index + 1))
                        {
                            indecies.Add(2 * node.Index + 1);
                        }
                    }
                }
            }


            foreach (Point point in _model.Shape.Points)
            {
                BoundaryCondition pointCondition = _model.PointConditions[point];
                if (pointCondition.Type == BoundaryConditionsType.Static)
                {
                    FiniteElementNode node = _mesh.GetNodeOnPoint(point);
                    if (node != null)
                    {
                        if (!indecies.Contains(2 * node.Index))
                        {
                            indecies.Add(2 * node.Index);
                        }
                        if (!indecies.Contains(2 * node.Index + 1))
                        {
                            indecies.Add(2 * node.Index + 1);
                        }
                    }
                }
            }

            return indecies;
        }

        private Vector addStaticPoints(Vector resultVector, List<int> indeciesToDelete)
        {
            Vector res = new Vector(resultVector);
            indeciesToDelete.Sort();
            foreach (int index in indeciesToDelete)
            {
                res.Insert(0, index);
            }
            return res;
        }
        #endregion

        #region StiffnessMatrix

        private Matrix GetStiffnessMatrix()
        {
            Matrix StiffnessMatrix = new Matrix(_mesh.Nodes.Count * 2, _mesh.Nodes.Count * 2);
            foreach (IFiniteElement element in _mesh.Elements)
            {
                Matrix localStiffnessMatrix = GetLocalStiffnessMatrix(element);

                for (int i = 0; i < element.Count; i++)
                {
                    for (int j = 0; j < element.Count; j++)
                    {
                        StiffnessMatrix[2 * element[i].Index, 2 * element[j].Index] += localStiffnessMatrix[2 * i, 2 * j];
                        StiffnessMatrix[2 * element[i].Index + 1, 2 * element[j].Index] += localStiffnessMatrix[2 * i + 1, 2 * j];
                        StiffnessMatrix[2 * element[i].Index, 2 * element[j].Index + 1] += localStiffnessMatrix[2 * i, 2 * j + 1];
                        StiffnessMatrix[2 * element[i].Index + 1, 2 * element[j].Index + 1] += localStiffnessMatrix[2 * i + 1, 2 * j + 1];
                    }
                }
            }

            return StiffnessMatrix;
        }



        #region Local Matrix

        private Matrix GetLocalStiffnessMatrix(IFiniteElement element)
        {
            elementCurrent = element;

            Matrix localStiffnessMatrix = Integration.GaussianIntegrationMatrix(LocalStiffnessMatrixFunction);

            return localStiffnessMatrix;
        }

        private Matrix LocalStiffnessMatrixFunction(double ksi, double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return derivativeMatrix * ConstMatrix * Matrix.Transpose(derivativeMatrix) * J.GetJacobianDeterminant(ksi, eta);
        }

        private Matrix GetLocalDerivativeMatrix(IFiniteElement element, double ksi, double eta)
        {
            Matrix LocalDerivativeMatrix = new Matrix(8, 4);

            Matrix gradNksieta = new Matrix(2, 4);

            gradNksieta[0, 0] = (eta - 1) * 0.25;
            gradNksieta[1, 0] = (ksi - 1) * 0.25;
            gradNksieta[0, 1] = (1 - eta) * 0.25;
            gradNksieta[1, 1] = (-ksi - 1) * 0.25;
            gradNksieta[0, 2] = (eta + 1) * 0.25;
            gradNksieta[1, 2] = (ksi + 1) * 0.25;
            gradNksieta[0, 3] = (-eta - 1) * 0.25;
            gradNksieta[1, 3] = (1 - ksi) * 0.25;

            JacobianRectangular J = new JacobianRectangular();
            J.Element = element;

            Matrix gradN = J.GetInverseJacobian(ksi, eta) * gradNksieta;

            LocalDerivativeMatrix[0, 0] = LocalDerivativeMatrix[1, 3] = gradN[0, 0];
            LocalDerivativeMatrix[2, 0] = LocalDerivativeMatrix[3, 3] = gradN[0, 1];
            LocalDerivativeMatrix[4, 0] = LocalDerivativeMatrix[5, 3] = gradN[0, 2];
            LocalDerivativeMatrix[6, 0] = LocalDerivativeMatrix[7, 3] = gradN[0, 3];

            LocalDerivativeMatrix[1, 1] = LocalDerivativeMatrix[0, 2] = gradN[1, 0];
            LocalDerivativeMatrix[3, 1] = LocalDerivativeMatrix[2, 2] = gradN[1, 1];
            LocalDerivativeMatrix[5, 1] = LocalDerivativeMatrix[4, 2] = gradN[1, 2];
            LocalDerivativeMatrix[7, 1] = LocalDerivativeMatrix[6, 2] = gradN[1, 3];

            return LocalDerivativeMatrix;
        }
        #endregion

        #endregion

    }
}
