using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public abstract class Mesh
    {
        public List<FiniteElementNode> Nodes { get; protected set; }
        public List<IFiniteElement> Elements { get; protected set; }

        public bool IsMeshGenerated { get; protected set; }

        protected Mesh()
        {
            initMeshElements();
        }


        protected virtual void initMeshElements()
        {
            Nodes = new List<FiniteElementNode>();
            Elements = new List<IFiniteElement>();
        }

        public FiniteElementNode GetNodeOnPoint(Point point)
        {
            FiniteElementNode node = null;
            if (Nodes.Count > 0)
            {
                node = Nodes[0];
                double minDistance = Point.Length(point, node.Point);
                foreach (FiniteElementNode finiteElementNode in Nodes)
                {
                    double distance = Point.Length(point, finiteElementNode.Point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        node = finiteElementNode;
                    }
                }
            }
            return node;
        }



        public abstract void Generate();
        
        public abstract IEnumerable<Point> GetPointsForResult();

    }
}
