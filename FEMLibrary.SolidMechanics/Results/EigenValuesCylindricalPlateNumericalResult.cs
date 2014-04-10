using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Utils;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.NumericalUtils;
using FEMLibrary.SolidMechanics.FiniteElements;

namespace FEMLibrary.SolidMechanics.Results
{
    public class EigenValuesCylindricalPlateNumericalResult : INumericalResult
    {
        public IEnumerable<IFiniteElement> Elements { get; private set; }

        public Vector U { get; set; }

        private double _frequency;

        private double h;

        public EigenValuesCylindricalPlateNumericalResult(List<IFiniteElement> elements, Vector result, double frequency, double h)
        {
            Elements = elements;
            U = result;
            _frequency = frequency;
            this.h = h;
        
        }

        public EigenValuesCylindricalPlateNumericalResult()
        {
            Elements = new List<IFiniteElement>();
            U = new Vector(0);
            _frequency = 0;
        }


        IFiniteElement FindElement(double alfa1)
        {
            foreach (IFiniteElement segment in Elements)
            {
                if ((segment[0].Point.X <= alfa1) && (segment[1].Point.X >= alfa1))
                {
                    return segment;
                }
            }
            return null;
        }

        double U10(double alfa1)
        {
            return UIJ(alfa1, 0);

        }

        private double UIJ(double alfa1, int shift)
        {
            IFiniteElement e = FindElement(alfa1);
            return U[6 * e[0].Index + shift] * (e[1].Point.X - alfa1) / (e[1].Point.X - e[0].Point.X) + U[6 * e[1].Index+shift] * (alfa1 - e[0].Point.X) / (e[1].Point.X - e[0].Point.X);
        }

        double U11(double alfa1)
        {
            return UIJ(alfa1, 1);
        }
        double U12(double alfa1)
        {
            return UIJ(alfa1, 2);
        }
        double U30(double alfa1)
        {
            return UIJ(alfa1, 3);
        }
        double U31(double alfa1)
        {
            return UIJ(alfa1, 4);
        }
        double U32(double alfa1)
        {
            return UIJ(alfa1, 5);
        }

        public double p0(double alfa3)
        {
            return 0.5 - alfa3 / h;
        }
        public double p1(double alfa3)
        {
            return 0.5 + alfa3 / h;
        }
        public double p2(double alfa3)
        {
            return 1 - (4 * alfa3 * alfa3) / (h * h);
        }

        

        public string Name
        {
            get {
                return _frequency.ToString("0.######");
            }
        }


        public Vector GetResultAtPoint(Point point, double t)
        {
            Vector res = new Vector(3);
            res[0] = U10(point.X) * p0(point.Y) + U11(point.X) * p1(point.Y) + U12(point.X) * p2(point.Y);
            res[2] = U30(point.X) * p0(point.Y) + U31(point.X) * p1(point.Y) + U32(point.X) * p2(point.Y);
            res[1] = 0;
            return res;
        }
    }
}
