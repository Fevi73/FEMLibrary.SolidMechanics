using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public class BoundaryMeshSettings {
        public Shape Shape { get; private set; }
        public List<BoundaryMesh> Settings { get; private set; }

        public BoundaryMeshSettings(Shape shape)
        {
            Shape = shape;
            Settings = new List<BoundaryMesh>();
        }
    }
}
