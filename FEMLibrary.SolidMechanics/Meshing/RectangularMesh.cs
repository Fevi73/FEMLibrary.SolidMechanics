using System;
using FEMLibrary.SolidMechanics.Geometry;
using System.Collections.Generic;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public class RectangularMesh:Mesh2D
    {
        private int _countWidthElements;
        private int _countHeightElements;
        private Rectangle _rectangle;

        public RectangularMesh(Rectangle rectangle, int countHeightElements, int countWidthElements)
            : base(rectangle)
        {
            _rectangle = rectangle;
            _countHeightElements = countHeightElements;
            _countWidthElements = countWidthElements;
            MiddleNodes = new List<FiniteElementNode>();
        }

        protected override bool Generate(BoundaryMeshSettings boundaryMeshSettings)
        {
            bool res = true;
            GenerateMeshNodes(_countWidthElements, _countHeightElements);
            GenerateFiniteElements(_countWidthElements, _countHeightElements);
            GenerateBoundarySegments(_countWidthElements, _countHeightElements);
            return res;

        }

        private void GenerateBoundarySegments(int LElements, int HElements)
        {
            boundarySegments.Clear();

            foreach(Edge edge in _rectangle.Edges)
            {
                List<FiniteElementNode> nodes = GetNodesOnEdge(edge);
                int segmentsCount = nodes.Count - 1;
                for (int i = 0; i < segmentsCount; i++)
                {
                    FiniteElementRectangleEdge segment = new FiniteElementRectangleEdge(nodes[i], nodes[i + 1]);
                    AddBoundarySegment(segment, edge);
                }
            }
        }

        private void GenerateFiniteElements(int LElements, int HElements)
        {
            Elements.Clear();

            int LNodes = LElements + 1;
            int finiteElementsCount = Nodes.Count - LNodes;

            for (int i = 0; i < finiteElementsCount; i++)
            {
                if ((i + 1) % LNodes != 0)
                {
                    FiniteElementRectangle element = new FiniteElementRectangle();
                    element.Node4 = Nodes[i];
                    element.Node3 = Nodes[i + 1];
                    element.Node1 = Nodes[i + LNodes];
                    element.Node2 = Nodes[i + LNodes + 1];

                    Elements.Add(element);
                }

            }

        }

        private void GenerateMeshNodes(int LElements, int HElements)
        {
            Nodes.Clear();
            boundaryNodes.Clear();

            int indexCur = 0;
            double xCur = _rectangle.Points[0].X;
            double yCur = _rectangle.Points[0].Y;

            if (HElements % 2 == 1) HElements++;

            int HNodes = HElements + 1;
            int LNodes = LElements + 1;

            double xStep = _rectangle.Width / Convert.ToDouble(LElements);
            double yStep = _rectangle.Height / Convert.ToDouble(HElements);

            int HNodesdiv2 = HNodes / 2;

            for (int i = 0; i < HNodesdiv2; i++)
            {
                for (int j = 0; j < LNodes; j++)
                {
                    FiniteElementNode node = new FiniteElementNode();
                    node.Index = indexCur;
                    node.Point.X = Math.Round(xCur, 4);
                    node.Point.Y = Math.Round(yCur, 4);
                    Nodes.Add(node);
                    if (j == 0)
                       AddBoundaryNode(node, _rectangle.Edges[0]);
                    if (j == (LNodes - 1))
                        AddBoundaryNode(node, _rectangle.Edges[2]);

                    indexCur++;
                    xCur += xStep;
                    if (i == 0)
                    {
                        AddBoundaryNode(node, _rectangle.Edges[1]);
                    }
                }
                yCur -= yStep;
                xCur = _rectangle.Points[0].X;
            }

            //xCur = _rectangle.Points[0].X;
            //yCur = (_rectangle.Points[0].Y + _rectangle.Points[1].Y) / 2;
            MiddleNodes.Clear();
            for (int j = 0; j < LNodes; j++)
            {
                FiniteElementNode node = new FiniteElementNode();
                node.Index = indexCur;
                node.Point.X = Math.Round(xCur, 4);
                node.Point.Y = Math.Round(yCur, 4);
                Nodes.Add(node);
                MiddleNodes.Add(node);
                if (j == 0)
                    AddBoundaryNode(node, _rectangle.Edges[0]);
                if (j == (LNodes - 1))
                    AddBoundaryNode(node, _rectangle.Edges[2]);
                indexCur++;
                xCur += xStep;
            }

            yCur -= yStep;
            xCur = _rectangle.Points[0].X;
            for (int i = 0; i < HNodesdiv2; i++)
            {
                for (int j = 0; j < LNodes; j++)
                {
                    FiniteElementNode node = new FiniteElementNode();
                    node.Index = indexCur;
                    node.Point.X = Math.Round(xCur, 4);
                    node.Point.Y = Math.Round(yCur, 4);
                    Nodes.Add(node);
                    if (j == 0)
                        AddBoundaryNode(node, _rectangle.Edges[0]);
                    if (j == (LNodes - 1))
                        AddBoundaryNode(node, _rectangle.Edges[2]);

                    if (i == (HNodesdiv2 - 1))
                    {
                        AddBoundaryNode(node, _rectangle.Edges[3]);
                    }
                    indexCur++;
                    xCur += xStep;
                }
                yCur -= yStep;
                xCur = _rectangle.Points[0].X;
            }
        }

        public List<FiniteElementNode> MiddleNodes { get; private set; }

        protected override BoundaryMeshSettings getDefaultBoundaryMeshSettings(Shape shape)
        {
            BoundaryMeshSettings settings = new BoundaryMeshSettings(shape);
            for(int i = 0; i < shape.Edges.Count;i++)
            {
                if (i % 2 == 0)
                {
                    settings.Settings.Add(new BoundaryMesh(shape.Edges[i], _countHeightElements));
                }
                else {
                    settings.Settings.Add(new BoundaryMesh(shape.Edges[i], _countWidthElements));
                }
            }
            return settings;
        }

        public override IEnumerable<Point> GetPointsForResult()
        {
            foreach(FiniteElementNode node in MiddleNodes)
            {
                yield return node.Point; 
            }
        }
    }
}
