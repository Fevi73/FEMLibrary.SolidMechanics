using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Physics;
using System.Collections.Generic;

namespace FEMLibrary.SolidMechanics.Solving
{
    public abstract class Solver
    {
        protected Model _model;
        protected BoundaryMeshSettings _meshSettings;
        protected Mesh _mesh;

        protected Solver(Model model, Mesh mesh)
        {
            _model = model;
            _mesh = mesh;
            _mesh.Generate();
        }

        public abstract IEnumerable<INumericalResult> Solve(int resultsCount);
    }
}
