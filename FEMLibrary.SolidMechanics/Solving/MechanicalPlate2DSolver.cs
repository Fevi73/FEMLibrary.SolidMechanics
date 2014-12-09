using FEMLibrary.SolidMechanics.FiniteElements;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.NumericalUtils;
using FEMLibrary.SolidMechanics.Physics;
using FEMLibrary.SolidMechanics.Results;
using MatrixLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.Solving
{
    public abstract class MechanicalPlate2DSolver:Solver
    {


        public MechanicalPlate2DSolver(Model model, Mesh mesh, double error, double amplitude)
            : base(model, mesh)
        {
            _error = error;
            _amplitude = amplitude;
            indeciesToDelete = new List<int>();
        }


        protected double _error;
        protected double _amplitude;

        protected Matrix ConstMatrix;

        protected double M1;
        protected double M2;
        protected double M3;
        protected double G13;

        protected IFiniteElement elementCurrent;
        protected Vector previousU;
        protected ICollection<int> indeciesToDelete;

        public Matrix GetConstantMatrix()
        {
            ConstMatrix = new Matrix(4, 4);
            M1 = ConstMatrix[0, 0] = _model.Material.GetE1Modif() * (1 + _model.Material.GetAlfa1());
            M2 = ConstMatrix[1, 0] = ConstMatrix[0, 1] = _model.Material.GetLambda1() * _model.Material.GetE0();
            M3 = ConstMatrix[1, 1] = _model.Material.GetE0();
            G13 = ConstMatrix[2, 2] = ConstMatrix[2, 3] = ConstMatrix[3, 2] = ConstMatrix[3, 3] = _model.Material.GetG13();
            return ConstMatrix;
        }

        #region MassMatrix

        public Matrix GetMassMatrix()
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

        protected Matrix GetLocalMassMatrixMatrix(IFiniteElement element)
        {
            elementCurrent = element;

            Matrix localMassMatrix = Integration.GaussianIntegrationMatrix(LocalMassMatrixFunction);

            return localMassMatrix;
        }

        protected Matrix LocalMassMatrixFunction(double ksi, double eta)
        {
            Matrix baseFunctionsMatrix = GetLocalBaseFunctionsMatrix(ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return baseFunctionsMatrix * Matrix.Transpose(baseFunctionsMatrix) * J.GetJacobianDeterminant(ksi, eta);
        }

        protected Matrix GetLocalBaseFunctionsMatrix(double ksi, double eta)
        {
            Matrix matrix = new Matrix(8, 2);
            matrix[0, 0] = matrix[1, 1] = 0.25 * (1 - ksi) * (1 - eta);
            matrix[2, 0] = matrix[3, 1] = 0.25 * (1 + ksi) * (1 - eta);
            matrix[4, 0] = matrix[5, 1] = 0.25 * (1 + ksi) * (1 + eta);
            matrix[6, 0] = matrix[7, 1] = 0.25 * (1 - ksi) * (1 + eta);
            return matrix;
        }


        #endregion

        #region StiffnessMatrix

        public Matrix GetStiffnessMatrix()
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

        protected Matrix GetLocalStiffnessMatrix(IFiniteElement element)
        {
            elementCurrent = element;

            Matrix localStiffnessMatrix = Integration.GaussianIntegrationMatrix(LocalStiffnessMatrixFunction);

            return localStiffnessMatrix;
        }

        protected Matrix LocalStiffnessMatrixFunction(double ksi, double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return derivativeMatrix * ConstMatrix * Matrix.Transpose(derivativeMatrix) * J.GetJacobianDeterminant(ksi, eta);
        }

        protected Matrix GetLocalDerivativeMatrix(IFiniteElement element, double ksi, double eta)
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

        #region Nonliear Stiffness Matrix
        public Matrix GetNonlinearMatrix(Vector u)
        {
            Vector uWithStaticPoints = addStaticPoints(u, indeciesToDelete);
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

        protected Matrix GetLocalNonlinearMatrix(IFiniteElement element, Vector u)
        {
            elementCurrent = element;
            previousU = u;

            Matrix localMassMatrix = Integration.GaussianIntegrationMatrix(LocalNonlinearMatrixFunction);

            return localMassMatrix;
        }

        protected Matrix LocalNonlinearMatrixFunction(double ksi, double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, ksi, eta);

            Matrix derivativeUMatrix = GetLocalUDerivativeMatrix(previousU, elementCurrent, ksi, eta);

            JacobianRectangular J = new JacobianRectangular();
            J.Element = elementCurrent;

            return derivativeMatrix * derivativeUMatrix * Matrix.Transpose(derivativeMatrix) * J.GetJacobianDeterminant(ksi, eta);
        }



        protected Matrix GetLocalUDerivativeMatrix(Vector previousU, IFiniteElement elementCurrent, double ksi, double eta)
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
            double d1u1 = uDerivative[0];
            double d3u3 = uDerivative[1];
            Matrix derivativeUMatrix = new Matrix(4, 4);
            /*derivativeUMatrix[0, 0] = derivativeUMatrix[2, 2] = uDerivative[0];
            derivativeUMatrix[1, 1] = derivativeUMatrix[3, 3] = uDerivative[1];
            derivativeUMatrix[3, 0] = derivativeUMatrix[1, 2] = uDerivative[2];
            derivativeUMatrix[2, 1] = derivativeUMatrix[0, 3] = uDerivative[3];*/
            derivativeUMatrix[0, 0] = (M1 * d1u1 + M2 * d3u3) * 0.5;
            derivativeUMatrix[1, 1] = (M2 * d1u1 + M3 * d3u3) * 0.5;
            derivativeUMatrix[2, 2] = (M2 * d1u1 + M3 * d3u3) * 0.5 + G13 * d1u1;
            derivativeUMatrix[3, 3] = (M1 * d1u1 + M2 * d3u3) * 0.5 + G13 * d3u3;
            derivativeUMatrix[2, 3] = G13 * d3u3;
            derivativeUMatrix[3, 2] = G13 * d1u1;
            return derivativeUMatrix;

        }


        #endregion

        #region Boundary conditions

        protected Vector applyStaticBoundaryConditionsToVector(Vector result, ICollection<int> indeciesToDelete)
        {
            indeciesToDelete = indeciesToDelete.OrderByDescending(i => i).ToList();

            foreach (int index in indeciesToDelete)
            {
                result = Vector.Cut(result, index);
            }
            return result;
        }


        protected Matrix applyStaticBoundaryConditions(Matrix stiffnessMatrix, ICollection<int> indeciesToDelete)
        {
            indeciesToDelete = indeciesToDelete.OrderByDescending(i => i).ToList();

            foreach (int index in indeciesToDelete)
            {
                stiffnessMatrix = Matrix.Cut(stiffnessMatrix, index, index);
            }
            return stiffnessMatrix;
        }

        //static == fixed
        protected ICollection<int> getIndeciesWithStaticBoundaryConditions()
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

        protected Vector addStaticPoints(Vector resultVector, ICollection<int> indeciesToDelete)
        {
            Vector res = new Vector(resultVector);
            indeciesToDelete = indeciesToDelete.OrderBy(i => i).ToList();
            foreach (int index in indeciesToDelete)
            {
                res.Insert(0, index);
            }
            return res;
        }
        #endregion

        #region Initial Value

        protected Vector applyAmplitudeToVector(Vector vector)
        {
            double max = vector.MaxOdd();
            if (max > 0)
            {
                double amplitudeCoef = _amplitude / max;
                for (int i = 1; i < vector.Length; i++)
                {
                    if (i % 2 == 1)
                    {
                        vector[i] *= amplitudeCoef;
                    }

                }
            }
            return vector;
        }

        #endregion

        protected ICollection<INumericalResult> generateVibrationResults(double[] lambdas, Vector[] eigenVectors)
        {
            List<INumericalResult> results = new List<INumericalResult>();

            for (int i = 0; i < lambdas.Length; i++)
            {
                Vector eigenVector = eigenVectors[i];
                //eigenVector = addStaticPoints(eigenVector, indeciesToDelete);
                //eigenVector = applyAmplitudeToVector(eigenVector);
                EigenValuesNumericalResult result = new EigenValuesNumericalResult(_mesh.Elements, eigenVector, Math.Sqrt(lambdas[i] / _model.Material.Rho));
                results.Add(result);
            }

            return results;
        }
    }
}
