using System;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Utils;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.NumericalUtils;
using FEMLibrary.SolidMechanics.FiniteElements;

namespace FEMLibrary.SolidMechanics.Results
{
    public class EigenValuesNumericalResult : INumericalResult
    {
        public IEnumerable<IFiniteElement> Elements { get; private set; }

        public Vector U { get; set; }

        private double _frequency;

        public EigenValuesNumericalResult(List<IFiniteElement> elements, Vector result, double frequency)
        {
            Elements = elements;
            U = result;
            _frequency = frequency;
        }

        public EigenValuesNumericalResult()
        {
            Elements = new List<IFiniteElement>();
            U = new Vector(0);
            _frequency = 0;
        }

        public Vector DU(double ksi, double eta, IFiniteElement element)
        {
            Vector uElement = GetUByElement(element);
            return uElement * GetLocalDerivativeMatrix(element, ksi, eta);
        }

        public Vector GetUByIndex(int index)
        {
            Vector res = new Vector(2);
            if (U != null)
            {
                res[0] = U[2 * index];
                res[1] = U[2 * index + 1];
            }
            return res;
        }

        public static Vector GetUByIndex(int index, Vector U)
        {
            Vector res = new Vector(2);
            if (U != null)
            {
                res[0] = U[2 * index];
                res[1] = U[2 * index + 1];
            }
            return res;
        }

        private Vector GetUByElement(IFiniteElement element)
        {
            Vector res = new Vector(8);
            if (U != null)
            {
                for (int i = 0; i < element.Count; i++)
                {
                    res[2 * i] = U[2 * element[i].Index];
                    res[2 * i + 1] = U[2 * element[i].Index + 1];
                }
            }
            return res;
        }

        private Matrix GetLocalDerivativeMatrix(IFiniteElement element, double ksi, double eta)
        {
            Matrix LocalDerivativeMatrix = new Matrix(8, 4);

            Matrix gradNksieta = new Matrix(2, 4);

            gradNksieta[0, 0] = (eta - 1) * 0.25;
            gradNksieta[1, 0] = (ksi - 1) * 0.25;
            gradNksieta[0, 1] = (1 - eta) * 0.25;
            gradNksieta[1, 1] = (-ksi - 1) * 0.25;
            gradNksieta[0, 2] = (eta + 1) * 0.25;
            gradNksieta[1, 2] = (ksi + 1) * 0.25;
            gradNksieta[0, 3] = (-eta - 1) * 0.25;
            gradNksieta[1, 3] = (1 - ksi) * 0.25;

            JacobianRectangular J = new JacobianRectangular();
            J.Element = element;

            Matrix gradN = J.GetInverseJacobian(ksi, eta) * gradNksieta;

            LocalDerivativeMatrix[0, 0] = LocalDerivativeMatrix[1, 3] = gradN[0, 0];
            LocalDerivativeMatrix[2, 0] = LocalDerivativeMatrix[3, 3] = gradN[0, 1];
            LocalDerivativeMatrix[4, 0] = LocalDerivativeMatrix[5, 3] = gradN[0, 2];
            LocalDerivativeMatrix[6, 0] = LocalDerivativeMatrix[7, 3] = gradN[0, 3];

            LocalDerivativeMatrix[1, 1] = LocalDerivativeMatrix[0, 2] = gradN[1, 0];
            LocalDerivativeMatrix[3, 1] = LocalDerivativeMatrix[2, 2] = gradN[1, 1];
            LocalDerivativeMatrix[5, 1] = LocalDerivativeMatrix[4, 2] = gradN[1, 2];
            LocalDerivativeMatrix[7, 1] = LocalDerivativeMatrix[6, 2] = gradN[1, 3];

            return LocalDerivativeMatrix;
        }

        

        public Vector GetResultAtPoint(Point point, double t)
        {
            IFiniteElement element = getElementForPoint(point);
            Vector result = null;
            if (element != null)
            {
                FiniteElementRectangle elementKsiTeta = new FiniteElementRectangle()
                    {
                        Node1 = new FiniteElementNode(-1, -1),
                        Node3 = new FiniteElementNode(1, 1)
                    };
                Point pointKsiTeta = ResultHelper.TransformCoordinates(point, element, elementKsiTeta);
                Matrix finiteElementApproximationMatrix = getFiniteElementApproximationMatrix(pointKsiTeta);
                result = GetUByElement(element) * (finiteElementApproximationMatrix * Math.Cos(_frequency * t));
            }
            return result;
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
            get {
                return _frequency.ToString("0.######");
            }
        }
    }
}
