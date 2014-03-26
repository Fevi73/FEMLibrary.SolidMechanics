using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.Meshing
{
    public abstract class Mesh2D:Mesh
    {
        public List<FiniteElementRectangleEdge> Segments { get; protected set; }
        
        protected List<KeyValuePair<FiniteElementRectangleEdge, Edge>> boundarySegments;

        private bool isSettingsSetted;
        public BoundaryMeshSettings BoundaryMeshSettings { get; protected set; }

        protected Mesh2D(BoundaryMeshSettings boundaryMeshSettings)
        {
            initMeshElements();
            BoundaryMeshSettings = boundaryMeshSettings;
            isSettingsSetted = true;
        }

        protected Mesh2D(Shape shape)
        {
            initMeshElements();
            BoundaryMeshSettings = new BoundaryMeshSettings(shape);
            isSettingsSetted = false;
        }


        protected override void initMeshElements()
        {
            base.initMeshElements();
            Segments = new List<FiniteElementRectangleEdge>();
            boundaryNodes = new List<KeyValuePair<FiniteElementNode, Edge>>();
            boundarySegments = new List<KeyValuePair<FiniteElementRectangleEdge, Edge>>();
        }

        

        public List<FiniteElementRectangleEdge> GetSegmentsOnEdge(Edge edge)
        {
            List<FiniteElementRectangleEdge> segments = new List<FiniteElementRectangleEdge>();
            foreach (KeyValuePair<FiniteElementRectangleEdge, Edge> pair in boundarySegments)
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
        protected void AddBoundarySegment(FiniteElementRectangleEdge FiniteElementRectangleEdge, Edge edge)
        {
            boundarySegments.Add(new KeyValuePair<FiniteElementRectangleEdge, Edge>(FiniteElementRectangleEdge, edge));
        }

        public override void Generate()
        {
            if (!isSettingsSetted) getDefaultBoundaryMeshSettings(BoundaryMeshSettings.Shape);
            IsMeshGenerated = Generate(BoundaryMeshSettings);
        }

        protected abstract bool Generate(BoundaryMeshSettings boundaryMeshSettings);

        protected abstract BoundaryMeshSettings getDefaultBoundaryMeshSettings(Shape shape);

    }
}
