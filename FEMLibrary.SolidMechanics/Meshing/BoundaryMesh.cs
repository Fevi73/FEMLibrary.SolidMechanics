using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public class BoundaryMesh
    {
        public Edge Edge { get; private set; }
        public int NumberOfElements { get; private set; }

        public BoundaryMesh(Edge edge, int count)
        {
            Edge = edge;
            NumberOfElements = count;
        }
    }
}
