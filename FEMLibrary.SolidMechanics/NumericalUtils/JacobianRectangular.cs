using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.FiniteElements;

namespace FEMLibrary.SolidMechanics.NumericalUtils
{
    public class JacobianRectangular
    {
        public IFiniteElement Element { get; set; }
        public Matrix GetJacobian(double ksi, double eta)
        {
            Matrix res = new Matrix(2, 2);
            double x1ksi = 0.25 * ((1 - eta) * (Element[1].Point.X - Element[0].Point.X) + (1 + eta) * (Element[2].Point.X - Element[3].Point.X));
            double x3ksi = 0.25 * ((1 - eta) * (Element[1].Point.Y - Element[0].Point.Y) + (1 + eta) * (Element[2].Point.Y - Element[3].Point.Y));
            double x1eta = 0.25 * ((1 - ksi) * (Element[3].Point.X - Element[0].Point.X) + (1 + ksi) * (Element[2].Point.X - Element[1].Point.X));
            double x3eta = 0.25 * ((1 - ksi) * (Element[3].Point.Y - Element[0].Point.Y) + (1 + ksi) * (Element[2].Point.Y - Element[1].Point.Y));
            res[0, 0] = x1ksi;
            res[1, 0] = x1eta;
            res[0, 1] = x3ksi;
            res[1, 1] = x3eta;
            return res;
        }

        public double GetJacobianDeterminant(double ksi, double eta)
        {
            return GetJacobian(ksi, eta).Determinant();
        }

        
        public Matrix GetInverseJacobian(double ksi, double eta)
        {
            Matrix J = GetJacobian(ksi, eta);
            Matrix res = new Matrix(2, 2);
            double det = J.Determinant();
            res[0, 0] = J[1, 1] / det;
            res[1, 1] = J[0, 0] / det;
            res[0, 1] = -J[0, 1] / det;
            res[1, 0] = -J[1, 0] / det;
            return res;
        }
    }
}
