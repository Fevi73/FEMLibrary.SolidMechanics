using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public abstract class Mesh
    {
        public List<FiniteElementNode> Nodes { get; protected set; }
        public List<IFiniteElement> Elements { get; protected set; }
        public List<Segment> Segments { get; protected set; }
        protected List<KeyValuePair<FiniteElementNode, Edge>> boundaryNodes;
        protected List<KeyValuePair<Segment, Edge>> boundarySegments;

        public bool IsMeshGenerated { get; private set; }
        private bool isSettingsSetted;
        
        public BoundaryMeshSettings BoundaryMeshSettings { get; protected set; }

        protected Mesh(BoundaryMeshSettings boundaryMeshSettings)
        {
            initMeshElements();
            BoundaryMeshSettings = boundaryMeshSettings;
            isSettingsSetted = true;
        }

        protected Mesh(Shape shape)
        {
            initMeshElements();
            BoundaryMeshSettings = new BoundaryMeshSettings(shape);
            isSettingsSetted = false;
        }


        protected void initMeshElements()
        {
            Nodes = new List<FiniteElementNode>();
            Elements = new List<IFiniteElement>();
            Segments = new List<Segment>();
            boundaryNodes = new List<KeyValuePair<FiniteElementNode, Edge>>();
            boundarySegments = new List<KeyValuePair<Segment, Edge>>();
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

        public List<FiniteElementNode> GetNodesOnEdge(Edge edge)
        {
            List<FiniteElementNode> nodes = new List<FiniteElementNode>();
            foreach(KeyValuePair<FiniteElementNode, Edge> pair in boundaryNodes)
            {
                if (pair.Value == edge)
                {
                    nodes.Add(pair.Key);
                }
            }
            return nodes;
        }

        public List<Segment> GetSegmentsOnEdge(Edge edge)
        {
            List<Segment> segments = new List<Segment>();
            foreach (KeyValuePair<Segment, Edge> pair in boundarySegments)
            {
                if (pair.Value == edge)
                {
                    segments.Add(pair.Key);
                }
            }
            return segments;
        }

        protected void AddBoundaryNode(FiniteElementNode node, Edge edge) {
            boundaryNodes.Add(new KeyValuePair<FiniteElementNode, Edge>(node, edge));
        }
        protected void AddBoundarySegment(Segment segment, Edge edge)
        {
            boundarySegments.Add(new KeyValuePair<Segment, Edge>(segment, edge));
        }

        public void Generate()
        {
            if (!isSettingsSetted) getDefaultBoundaryMeshSettings(BoundaryMeshSettings.Shape);
            IsMeshGenerated = Generate(BoundaryMeshSettings);
        }

        protected abstract bool Generate(BoundaryMeshSettings boundaryMeshSettings);

        protected abstract BoundaryMeshSettings getDefaultBoundaryMeshSettings(Shape shape);

        public abstract IEnumerable<Point> GetPointsForResult();

    }
}
