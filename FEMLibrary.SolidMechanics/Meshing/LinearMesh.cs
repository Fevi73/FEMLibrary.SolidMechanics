using System;
using FEMLibrary.SolidMechanics.Geometry;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.FiniteElements;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public class LinearMesh:Mesh1D
    {
        private int _countWidthElements;
        private CylindricalPlate _plate;

        public LinearMesh(CylindricalPlate plate, int countWidthElements)
            : base(plate)
        {
            _plate = plate;
            _countWidthElements = countWidthElements;
            MiddleNodes = new List<FiniteElementNode>();
        }

        public override void Generate()
        {
            GenerateMeshNodes(_countWidthElements);
            GenerateFiniteElements(_countWidthElements);
            IsMeshGenerated = true;
        }

        private void GenerateFiniteElements(int LElements)
        {
            Elements.Clear();

            int LNodes = LElements + 1;
            int finiteElementsCount = LElements;

            for (int i = 0; i < finiteElementsCount; i++)
            {
                FiniteElementSegment element = new FiniteElementSegment();
                element.Node1 = Nodes[i];
                element.Node2 = Nodes[i + 1];

                Elements.Add(element);

            }

        }

        private void GenerateMeshNodes(int LElements)
        {
            Nodes.Clear();
            boundaryNodes.Clear();
            MiddleNodes.Clear();

            int indexCur = 0;
            double xCur = _plate.Points[0].X;

            int LNodes = LElements + 1;

            double xStep = _plate.Width / Convert.ToDouble(LElements);

            for (int j = 0; j < LNodes; j++)
            {
                FiniteElementNode node = new FiniteElementNode();
                node.Index = indexCur;
                node.Point.X = Math.Round(xCur, 4);
                node.Point.Y = 0;
                Nodes.Add(node);
                MiddleNodes.Add(node);
                if (j == 0)
                    AddBoundaryNode(node, _plate.Edges[0]);
                if (j == (LNodes - 1))
                    AddBoundaryNode(node, _plate.Edges[2]);
                indexCur++;
                xCur += xStep;
            }
        }

        public List<FiniteElementNode> MiddleNodes { get; private set; }

        public override IEnumerable<Point> GetPointsForResult()
        {
            foreach(FiniteElementNode node in MiddleNodes)
            {
                yield return node.Point; 
            }
        }
    }
}
