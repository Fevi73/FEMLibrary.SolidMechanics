using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Utils;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.NumericalUtils;

namespace FEMLibrary.SolidMechanics.Results
{
    public class SemidiscreteVibrationsNumericalResult : INumericalResult
    {
        public IEnumerable<IFiniteElement> Elements { get; private set; }

        public List<Vector> U { get; set; }

        private double _deltaTime;
        private double _maxTime;

        public SemidiscreteVibrationsNumericalResult(List<IFiniteElement> elements, double deltaTime, double maxTime)
        {
            Elements = elements;
            U = new List<Vector>();
            _deltaTime = deltaTime;
            _maxTime = maxTime;
        }

        public SemidiscreteVibrationsNumericalResult()
        {
            Elements = new List<IFiniteElement>();
            U = new List<Vector>();
            _deltaTime = 0;
            _maxTime = 0;
        }

        public void AddResult(Vector result)
        {
            U.Add(result);
        }

        private double cutTime(double t)
        {
            while (t >= _maxTime)
            {
                t -= _maxTime;
            }

            return t;
        }

        public Vector GetResultAtPoint(Point point, double t)
        {
            IFiniteElement element = getElementForPoint(point);
            Vector result = null;
            if (element != null)
            {
                int time = (int)(cutTime(t) / _deltaTime);

                FiniteElementRectangle elementKsiTeta = new FiniteElementRectangle()
                    {
                        Node1 = new FiniteElementNode(-1, -1),
                        Node3 = new FiniteElementNode(1, 1)
                    };
                Point pointKsiTeta = ResultHelper.TransformCoordinates(point, element, elementKsiTeta);
                Matrix finiteElementApproximationMatrix = getFiniteElementApproximationMatrix(pointKsiTeta);
                result = GetUByElement(element, U[time]) * finiteElementApproximationMatrix;
            }
            return result;
        }

        private Vector GetUByElement(IFiniteElement element, Vector result)
        {
            Vector res = new Vector(8);
            if (U != null)
            {
                for (int i = 0; i < element.Count; i++)
                {
                    res[2 * i] = result[2 * element[i].Index];
                    res[2 * i + 1] = result[2 * element[i].Index + 1];
                }
            }
            return res;
        }

        private Matrix getFiniteElementApproximationMatrix(Point pointKsiTeta)
        {
            Matrix result = new Matrix(8, 3);
            result[0, 0] = result[1, 2] = 0.25*(1 - pointKsiTeta.X)*(1 - pointKsiTeta.Y);
            result[2, 0] = result[3, 2] = 0.25*(1 + pointKsiTeta.X)*(1 - pointKsiTeta.Y);
            result[4, 0] = result[5, 2] = 0.25*(1 + pointKsiTeta.X)*(1 + pointKsiTeta.Y);
            result[6, 0] = result[7, 2] = 0.25*(1 - pointKsiTeta.X)*(1 + pointKsiTeta.Y);
            return result;
        }

        private IFiniteElement getElementForPoint(Point point)
        {
            foreach(IFiniteElement element in Elements)
            {
                if (ResultHelper.IsPointInRectangle(point, element[0].Point, element[2].Point))
                {
                    return element;
                }
            }
            return null;

        }


        public string Name
        {
            get { return "Semidiscrete Result"; }
        }
    }
}
