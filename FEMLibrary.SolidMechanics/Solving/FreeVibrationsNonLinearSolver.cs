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

namespace FEMLibrary.SolidMechanics.Solving
{
    public class FreeVibrationsNonLinearSolver:Solver
    {
        public FreeVibrationsNonLinearSolver(Model model, Mesh mesh, double error, Vector init, double maxTime, int intervalsTime)
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
        

        public override IEnumerable<INumericalResult> Solve(int resultsCount)
        {
            List<INumericalResult> results = new List<INumericalResult>();
            
            if (_mesh.IsMeshGenerated)
            {
                GetConstantMatrix();

                Matrix StiffnessMatrix = GetStiffnessMatrix();
                Matrix MassMatrix = GetMassMatrix();
                Matrix GeneralMatrix = Matrix.Transpose((-1) * StiffnessMatrix * MassMatrix.Inverse());
                List<int> indeciesToDelete;
                GeneralMatrix = AsumeStaticBoundaryConditions(GeneralMatrix, out indeciesToDelete);

                Vector init = InitValue();

                //CauchyProblemResult cauchyProblemResult = CauchyProblemSolver.HeunMethodSolve((t,v)=>GetF(v,GeneralMatrix), init, _maxTime, _intervalsTime);
                //CauchyProblemResult cauchyProblemResult = CauchyProblemSolver.AdamsBashforthMethodsSolve((t, v) => GetF(v, GeneralMatrix), init, _maxTime, _intervalsTime);
                CauchyProblemResult cauchyProblemResult = CauchyProblemSolver.GirMethodsSolve((t, v) => GetF(v, GeneralMatrix), init, _maxTime, _intervalsTime);
                
                SemidiscreteVibrationsNumericalResult result = new SemidiscreteVibrationsNumericalResult(_mesh.Elements, cauchyProblemResult.DeltaTime);
                foreach (Vector v in cauchyProblemResult.Results) {
                    AddToResult(result, v, indeciesToDelete);
                }

                results.Add(result);
            }
            return results;
        }

        private Vector GetF(Vector v, Matrix GeneralMatrix)
        {
            Vector u = GetU(v);
            Vector u1 = GetU1(v);

            Vector u_derivative = u1;
            Vector u1_derivative = GeneralMatrix * u;

            return GetV(u_derivative, u1_derivative);
        }

        private Vector InitValue()
        {
            Vector u = AsumeStaticBoundaryConditionsToVector(_init);
            Vector u1 = new Vector(u.Length);
            return GetV(u, u1);
        }

        private void AddToResult(SemidiscreteVibrationsNumericalResult result, Vector V, List<int> indeciesToDelete)
        {
            Vector u = GetU(V);
            addStaticPoints(u, indeciesToDelete);
            result.AddResult(u);
        }

        private Vector GetU(Vector v)
        {
            Vector u = new Vector(v.Length / 2);
            for (int i = 0; i < u.Length; i++) {
                u[i] = v[i];
            }
            return u;
        }

        private Vector GetU1(Vector v)
        {
            Vector u1 = new Vector(v.Length / 2);
            int shift = v.Length - u1.Length;
            for (int i = 0; i < u1.Length; i++)
            {
                u1[i] = v[i + shift];
            }
            return u1;
        }

        private Vector GetV(Vector u, Vector u1)
        {
            Vector v = new Vector(u.Length + u1.Length);
            for (int i = 0; i < u.Length; i++)
            {
                v[i] = u[i];
            }

            for (int i = 0; i < u1.Length; i++)
            {
                v[u.Length + i] = u1[i];
            }
            return v;
        }

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
            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return Matrix.IndentityMatrix(8) * J.GetJacobianDeterminant(ksi, eta);
        }

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

        private Vector AsumeStaticBoundaryConditionsToVector(Vector result)
        {
            List<int> indecies = getIndeciesWithStaticBoundaryConditions();

            indecies.Sort();
            indecies.Reverse();

            foreach (int index in indecies)
            {
                result = Vector.Cut(result, index);
            }
            return result;
        }

        //static == fixed
        private Matrix AsumeStaticBoundaryConditions(Matrix stiffnessMatrix, out List<int> indeciesToDelete)
        {
            indeciesToDelete = getIndeciesWithStaticBoundaryConditions();
            
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

        private void addStaticPoints(Vector resultVector, List<int> indeciesToDelete)
        {
            indeciesToDelete.Sort();
            foreach (int index in indeciesToDelete)
            {
                resultVector.Insert(0, index);
            }
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
