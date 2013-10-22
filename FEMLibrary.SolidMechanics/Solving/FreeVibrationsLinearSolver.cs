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

namespace FEMLibrary.SolidMechanics.Solving
{
    public class FreeVibrationsLinearSolver:MechanicalPlate2DSolver
    {
        public FreeVibrationsLinearSolver(Model model, Mesh mesh, double error, double amplitude)
            : base(model, mesh, error, amplitude)
        { }

        public override IEnumerable<INumericalResult> Solve(int resultsCount)
        {
            ICollection<INumericalResult> results = new List<INumericalResult>();
            
            if (_mesh.IsMeshGenerated)
            {
                GetConstantMatrix();

                indeciesToDelete = getIndeciesWithStaticBoundaryConditions();

                Matrix StiffnessMatrix = GetStiffnessMatrix();
                StiffnessMatrix = applyStaticBoundaryConditions(StiffnessMatrix, indeciesToDelete);

                Matrix MassMatrix = GetMassMatrix();
                MassMatrix = applyStaticBoundaryConditions(MassMatrix, indeciesToDelete);

#if (ALL_LAMBDAS)
                Vector[] eigenVectors;
                double[] lambdas = StiffnessMatrix.GetEigenvalueSPAlgorithm(MassMatrix, out eigenVectors, _error, resultsCount);

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
