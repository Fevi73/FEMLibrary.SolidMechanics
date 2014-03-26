﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.Geometry
{
    public class FiniteElementNode
    {
        public Point Point { get; set; }
        public int Index { get; set; }

        public FiniteElementNode()
        {
            Point = new Point();
            Index = 0;
        }
        public FiniteElementNode(double x, double y)
        {
            Point = new Point(x, y);
            Index = 0;
        }

        public FiniteElementNode(double x, double y, int index)
        {
            Point = new Point(x, y);
            Index = index;
        }

        public static double Length(FiniteElementNode a, FiniteElementNode b)
        {
            return Point.Length(a.Point, b.Point);
        }

        public override string ToString()
        {
            return Point.ToString();//X + " ; " + Y;
        }
    }
}
