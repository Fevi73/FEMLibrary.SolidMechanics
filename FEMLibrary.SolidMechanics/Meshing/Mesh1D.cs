using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public abstract class Mesh1D : Mesh
    {
        protected Mesh1D(Shape shape)
        {
            initMeshElements();
        }

        protected override void initMeshElements()
        {
            base.initMeshElements();
        }

        

        protected void AddBoundaryNode(FiniteElementNode node, Edge edge)
        {
            boundaryNodes.Add(new KeyValuePair<FiniteElementNode, Edge>(node, edge));
        }


    }
}
