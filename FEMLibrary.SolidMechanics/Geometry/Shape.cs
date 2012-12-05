using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FEMLibrary.SolidMechanics.Geometry
{
    [Serializable]
    public abstract class Shape
    {
        public List<Edge> Edges { get; protected set; }
        public List<Point> Points { get; protected set; }

        protected Shape() 
        {
            Edges = new List<Edge>();
            Points = new List<Point>();
        }

        public void Copy(Shape shape) {
            Points.Clear();
            foreach (Point point in shape.Points)
            {
                Points.Add(point);
            }
            
            Edges.Clear();
            foreach (Edge edge in shape.Edges) 
            {
                Edges.Add(edge);
            }
        }

    }
}
