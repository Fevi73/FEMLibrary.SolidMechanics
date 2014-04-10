#define ALL_LAMBDAS
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
    public class CylindricalPlate1DSolver:Solver
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

        #region Const Matrix
        public Matrix GetConstantMatrix()
        {
            CylindricalPlate plate = _model.Shape as CylindricalPlate;

            double hInv = 1 / plate.Height;

            double K = plate.Curvature;

            double E = _model.Material.E[0]; 
            double v = _model.Material.v[0,0];
            double h = plate.Height;

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

            Matrix D = matrixD(K, hInv);

            ConstMatrix = Matrix.Transpose(D) * A * D;
            return ConstMatrix;
        }

        private Matrix matrixD(double K, double hInv)
        {
            
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

        #endregion

        #region MassMatrix

        public Matrix GetMassMatrix()
        {
            int N = _mesh.Elements.Count;
            int count = 6 * (N + 1);

            Matrix MassMatrix = new Matrix(count, count);
            foreach (IFiniteElement element in _mesh.Elements)
            {
                Matrix localMassMatrix = GetLocalMassMatrix(element);

                for (int i = 0; i < localMassMatrix.CountRows; i++)
                {
                    for (int j = 0; j < localMassMatrix.CountColumns; j++)
                    {
                        MassMatrix[6 * element[0].Index + i, 6 * element[0].Index + j] += localMassMatrix[i, j];
                    }
                }
            }

            return MassMatrix;
        }

        protected Matrix GetLocalMassMatrix(IFiniteElement element)
        {
            elementCurrent = element;

            Matrix localMassMatrix = _model.Material.Rho * Integration.GaussianIntegrationMatrix(LocalMassMatrixFunction);

            return localMassMatrix;
        }

        protected Matrix LocalMassMatrixFunction(double eta)
        {
            Matrix baseFunctionsMatrix = GetLocalBaseFunctionsMatrix(eta);

            return baseFunctionsMatrix * Matrix.Transpose(baseFunctionsMatrix) * ((elementCurrent[1].Point.X - elementCurrent[0].Point.X) / 2);
        }

        protected Matrix GetLocalBaseFunctionsMatrix(double eta)
        {
            Matrix localDerivativeMatrix = new Matrix(6, 12);

            for (int j = 0; j < 6; j++)
            {
                localDerivativeMatrix[j, j] = (1 - eta) / 2;
                localDerivativeMatrix[j, j + 6] = (1 + eta) / 2;
            }

            return localDerivativeMatrix;
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


        protected Matrix applyStaticBoundaryConditionsToMatrix(Matrix stiffnessMatrix, ICollection<int> indeciesToDelete)
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
                        for (int i = 0; i < 6; i++)
                        {
                            if (!indecies.Contains(6 * node.Index + i))
                            {
                                indecies.Add(6 * node.Index + i);
                            }
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
                        for (int i = 0; i < 6; i++)
                        {
                            if (!indecies.Contains(6 * node.Index + i))
                            {
                                indecies.Add(6 * node.Index + i);
                            }
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

            CylindricalPlate plate = _model.Shape as CylindricalPlate;

            double h = plate.Height;

            for (int i = 0; i < lambdas.Length; i++)
            {
                Vector eigenVector = eigenVectors[i];
                eigenVector = addStaticPoints(eigenVector, indeciesToDelete);
                EigenValuesCylindricalPlateNumericalResult result = new EigenValuesCylindricalPlateNumericalResult(_mesh.Elements, eigenVector, Math.Sqrt(lambdas[i]  / _model.Material.Rho) , h);
                results.Add(result);
            }

            return results;
        }


        public override IEnumerable<INumericalResult> Solve(int resultsCount)
        {
            ICollection<INumericalResult> results = new List<INumericalResult>();

            if (_mesh.IsMeshGenerated)
            {
                GetConstantMatrix();

                indeciesToDelete = getIndeciesWithStaticBoundaryConditions();

                Matrix StiffnessMatrix = GetStiffnessMatrix();
                StiffnessMatrix = applyStaticBoundaryConditionsToMatrix(StiffnessMatrix, indeciesToDelete);

                Matrix MassMatrix = GetMassMatrix();
                MassMatrix = applyStaticBoundaryConditionsToMatrix(MassMatrix, indeciesToDelete);

#if (ALL_LAMBDAS)
                Vector[] eigenVectors;
                double[] lambdas = StiffnessMatrix.GetEigenvalueSPAlgorithm(MassMatrix, out eigenVectors, _error, resultsCount);

                results = generateVibrationResults(lambdas, eigenVectors);

#else
                Vector eigenVector;
                double lambda = StiffnessMatrix.GetMaxEigenvalueSPAlgorithm(out eigenVector, _error);
                addStaticPoints(eigenVector, indeciesToDelete);

                EigenValuesNumericalResult result = new EigenValuesNumericalResult(_mesh.Elements, eigenVector, Math.Sqrt(lambda / _model.Material.Rho));
                results.Add(result);
#endif
            }
            return results;
        }
    }
}
