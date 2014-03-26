using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.Geometry
{
    [Serializable]
    public class Edge
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public Edge(double startPointX, double startPointY, double endPointX, double endPointY) 
        {
            StartPoint = new Point(startPointX, startPointY);
            EndPoint = new Point(endPointX, endPointY);
        }

        public Edge(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Edge() 
        {
            StartPoint = new Point();
            EndPoint = new Point();
        }
    }
}
