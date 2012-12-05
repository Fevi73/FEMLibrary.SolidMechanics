using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.Geometry
{
    [Serializable]
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point()
        {
            X = 0;
            Y = 0;
        }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static double Length(Point a, Point b)
        {
            return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }
        
        public static Point Parse(string nodeStr)
        {
            string[] mas = nodeStr.Split(';');
            return new Point(double.Parse(mas[0].Trim()), double.Parse(mas[1].Trim()));
        }

        public override bool Equals(object obj)
        {
            bool res = false;
            Point point = obj as Point;
            if (point != null)
            {
                res = (this.X == point.X) && (this.Y == point.Y);
            }
            return res;
        }

        public override string ToString()
        {
            return X.ToString() + " ; " + Y.ToString();
        }
    }
}
