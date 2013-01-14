using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Solving;
using FEMLibrary.SolidMechanics.Results;

namespace FEMLibrary.SolidMechanics.Utils
{
    public static class ResultHelper
    {
        public static bool IsResultsEqual(IEnumerable<Point> points, IResult result1, IResult result2, double t, double error)
        {
            double norm = 0;
            foreach (Point point in points)
            {
                norm += Vector.Norm(result1.GetResultAtPoint(point, t) - result2.GetResultAtPoint(point, t));
            }

            return (norm < error);
        }

        public static Point TransformCoordinates(Point pointForm, IFiniteElement elementFrom, IFiniteElement elementTo)
        {
            Point res = new Point();
            res.X = elementTo[0].Point.X +
                    ((pointForm.X - elementFrom[0].Point.X) * (elementTo[2].Point.X - elementTo[0].Point.X)) / (elementFrom[2].Point.X - elementFrom[0].Point.X);
            res.Y = elementTo[0].Point.Y +
                    ((pointForm.Y - elementFrom[0].Point.Y) * (elementTo[2].Point.Y - elementTo[0].Point.Y)) / (elementFrom[2].Point.Y - elementFrom[0].Point.Y);

            return res;
        }

        public static bool IsPointInRectangle(Point point, Point pointLeftBottom, Point pointRightTop)
        {
            bool isBetweenTopAndBottom = (point.X >= pointLeftBottom.X) && (point.X <= pointRightTop.X);
            bool isBetweenLeftAndRight = (point.Y >= pointLeftBottom.Y) && (point.Y <= pointRightTop.Y);
            return isBetweenLeftAndRight && isBetweenTopAndBottom;

        }
    }
}
