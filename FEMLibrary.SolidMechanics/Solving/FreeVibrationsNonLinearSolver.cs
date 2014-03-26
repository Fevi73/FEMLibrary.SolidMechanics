#define ALL_LAMBDAS
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Physics;
using FEMLibrary.SolidMechanics.Results;
using MatrixLibrary;
using System.Collections.Generic;

namespace FEMLibrary.SolidMechanics.Solving
{
    public class FreeVibrationsNonLinearSolver:MechanicalPlate2DSolver
    {
        public FreeVibrationsNonLinearSolver(Model model, Mesh mesh, double error, double amplitude, int maxIterations)
            : base(model, mesh, error, amplitude)
        {
            _maxIterations = maxIterations;
        }
        
        private int _maxIterations;

        public override IEnumerable<INumericalResult> Solve(int resultsCount)
        {
            IEnumerable<INumericalResult> results = new List<INumericalResult>();
            
            if (_mesh.IsMeshGenerated)
            {
                GetConstantMatrix();
                
                indeciesToDelete = getIndeciesWithStaticBoundaryConditions();

                Matrix stiffnessMatrix = GetStiffnessMatrix();
                stiffnessMatrix = applyStaticBoundaryConditions(stiffnessMatrix, indeciesToDelete);

                Matrix massMatrix = GetMassMatrix();
                massMatrix = applyStaticBoundaryConditions(massMatrix, indeciesToDelete);

                
#if (ALL_LAMBDAS)

                Vector firstEigenVector = null;
                double[] lambdas = null;
                Vector[] eigenVectors = null;
                int iterations = 0;
                do
                {
                    if (eigenVectors != null)
                    {
                        firstEigenVector = eigenVectors[0];
                    }
                    else 
                    {
                        firstEigenVector = new Vector(stiffnessMatrix.CountRows);
                    }

                    firstEigenVector = applyAmplitudeToVector(firstEigenVector);

                    Matrix nonlinearMatrix = GetNonlinearMatrix(firstEigenVector);
                    nonlinearMatrix = applyStaticBoundaryConditions(nonlinearMatrix, indeciesToDelete);

                    Matrix k = stiffnessMatrix + nonlinearMatrix;
                    lambdas = k.GetEigenvalueSPAlgorithm(massMatrix, out eigenVectors, _error, resultsCount);
                    iterations++;
                }
                while ((Vector.Norm(eigenVectors[0] - firstEigenVector) > _error) &&(iterations < _maxIterations));
                results = generateVibrationResults(lambdas, eigenVectors);

#else
                Vector eigenVector;
                double lambda = StiffnessMatrix.GetMaxEigenvalueSPAlgorithm(out eigenVector, _error);
                addStaticPoints(eigenVector);

                EigenValuesNumericalResult result = new EigenValuesNumericalResult(_mesh.Elements, eigenVector, Math.Sqrt(lambda / _model.Material.Rho));
                results.Add(result);
#endif
            }
            return results;
        }

        

        

    }
}
