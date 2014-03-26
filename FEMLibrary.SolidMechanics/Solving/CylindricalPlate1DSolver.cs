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
    public abstract class CylindricalPlate1DSolver:Solver
    {
        public CylindricalPlate1DSolver(Model model, Mesh mesh, double error, double amplitude)
            : base(model, mesh)
        {
            _error = error;
            _amplitude = amplitude;
            indeciesToDelete = new List<int>();
        }

        protected double _error;
        protected double _amplitude;

        protected Matrix ConstMatrix;

        protected IFiniteElement elementCurrent;
        protected Vector previousU;
        protected ICollection<int> indeciesToDelete;

        public Matrix GetConstantMatrix(double E, double v, double h)
        {
            Matrix A = new Matrix(9, 9);

            A[0, 0] = A[0, 2] = A[1, 1] = A[1, 2] = A[2, 0] = A[2, 1] = A[3, 3] = A[3, 5] = A[4, 4] = A[4, 5] = A[5, 3] = A[5, 4] = (E * h * (1 - v)) / (3 * (1 + v) * (1 - 2 * v));
            A[0, 1] = A[1, 0] = A[3, 4] = A[4, 3] = (E * h * (1 - v)) / (6 * (1 + v) * (1 - 2 * v));
            A[2, 2] = A[5, 5] = (8 * E * h * (1 - v)) / (5 * (1 + v) * (1 - 2 * v));


            A[0, 3] = A[0, 5] = A[1, 4] = A[1, 5] = A[2, 3] = A[2, 4] = A[3, 0] = A[3, 2] = A[4, 1] = A[4, 2] = A[5, 0] = A[5, 1] = (E * h * v) / (3 * (1 + v) * (1 - 2 * v));
            A[0, 4] = A[1, 3] = A[3, 1] = A[4, 0] = (E * h * v) / (6 * (1 + v) * (1 - 2 * v));
            A[2, 5] = A[5, 2] = (8 * E * h * v) / (5 * (1 + v) * (1 - 2 * v));

            A[6, 6] = A[6, 8] = A[7, 7] = A[7, 8] = A[8, 6] = A[8, 7] = (E * h) / (6 * (1 + v));
            A[6, 7] = A[7, 6] = (E * h) / (12 * (1 + v));
            A[8, 8] = (4 * E * h) / (15 * (1 + v));

            Matrix D = matrixD();

            ConstMatrix = Matrix.Transpose(D) * A * D;
            return ConstMatrix;
        }

        private Matrix matrixD()
        {
            CylindricalPlate plate = _model.Shape as CylindricalPlate;

            double hInv = 1 / plate.Height;

            double K = plate.Curvature;
            Matrix D = new Matrix(9, 12);
            D[0, 1] = D[1, 3] = D[2, 5] = D[6, 7] = D[7, 9] = D[8, 11] = 1;
            D[0, 6] = D[1, 8] = D[2, 10] = D[8, 4] = K;
            D[5, 10] = 2 * K;

            D[3, 6] = -hInv + K / 2;
            D[3, 8] = D[6, 2] = D[7, 2] = hInv - K / 2;
            D[3, 10] = D[6, 4] = 4 * hInv - 2 * K;

            D[4, 6] = D[6, 0] = D[7, 0] = -hInv - K / 2;
            D[4, 8] = hInv + K / 2;
            D[4, 10] = D[7, 4] = -4 * hInv - 2 * K;
            return D;
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

            Matrix localMassMatrix = _model.Material.Rho * Integration.GaussianIntegrationMatrix(LocalMassMatrixFunction);

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
            int N = _mesh.Elements.Count;
            int count = 6 * (N + 1);

            Matrix StiffnessMatrix = new Matrix(count, count);
            foreach (IFiniteElement element in _mesh.Elements)
            {
                Matrix localStiffnessMatrix = GetLocalStiffnessMatrix(element);

                for (int i = 0; i < localStiffnessMatrix.CountRows; i++)
                {
                    for (int j = 0; j < localStiffnessMatrix.CountColumns; j++)
                    {
                        StiffnessMatrix[6 * element[0].Index + i, 6 * element[0].Index + j] += localStiffnessMatrix[i, j];
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

        protected Matrix LocalStiffnessMatrixFunction(double eta)
        {
            Matrix derivativeMatrix = GetLocalDerivativeMatrix(elementCurrent, eta);

            return derivativeMatrix * ConstMatrix * Matrix.Transpose(derivativeMatrix) * ((elementCurrent[1].Point.X - elementCurrent[0].Point.X) / 2);
        }

        protected Matrix GetLocalDerivativeMatrix(IFiniteElement element, double eta)
        {
            int n = 12;
            Matrix LocalDerivativeMatrix = new Matrix(n, n);

            int m = 0;
            for (int j = 0; j < n; j++)
            {
                if (j % 2 == 0)
                {
                    LocalDerivativeMatrix[j, m] = (1 - eta) / 2;
                    LocalDerivativeMatrix[j, m + 6] = (1 + eta) / 2;
                }
                else
                {
                    LocalDerivativeMatrix[j, m] = 1 / (element[0].Point.X - element[1].Point.X);
                    LocalDerivativeMatrix[j, m + 6] = 1 / (element[1].Point.X - element[0].Point.X);
                    m += 1;
                }
            }

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
                eigenVector = addStaticPoints(eigenVector, indeciesToDelete);
                eigenVector = applyAmplitudeToVector(eigenVector);
                EigenValuesNumericalResult result = new EigenValuesNumericalResult(_mesh.Elements, eigenVector, Math.Sqrt(lambdas[i]));///_model.Material.Rho));
                results.Add(result);
            }

            return results;
        }

    }
}
