using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrixLibrary;

namespace FEMLibrary.SolidMechanics.NumericalUtils
{
    public class GaussianQuadrature
    {
        public static readonly double[] NODES = { -Math.Sqrt(3.0 / 5.0), 0, Math.Sqrt(3.0 / 5.0) };
        public static readonly double[] WEIGHT = { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
        public const int ORDER = 3;
    }

    public class Integration
    {
        public static double GaussianIntegration(Func<double, double, double> function)
        {
            Matrix w = new Matrix(GaussianQuadrature.ORDER, 1);

            for (int i = 0; i < GaussianQuadrature.ORDER; i++)
            {
                w[i,0] = GaussianQuadrature.WEIGHT[i];
            }

            Matrix F = new Matrix(GaussianQuadrature.ORDER, GaussianQuadrature.ORDER);
            for (int i = 0; i < GaussianQuadrature.ORDER; i++)
            {
                for (int j = 0; j < GaussianQuadrature.ORDER; j++)
                {
                    F[i, j] = function(GaussianQuadrature.NODES[i], GaussianQuadrature.NODES[j]);
                }
            }

            Matrix resMatrix = Matrix.Transpose(w) * F * w;

            return resMatrix[0,0];
     
        }

        public static Matrix GaussianIntegrationMatrix(Func<double, double, Matrix> function)
        {
            Matrix res =new Matrix(function(0, 0).CountRows, function(0, 0).CountColumns);

            for (int i = 0; i < GaussianQuadrature.ORDER; i++)
            {
                for (int j = 0; j < GaussianQuadrature.ORDER; j++)
                {
                    res += GaussianQuadrature.WEIGHT[i] * GaussianQuadrature.WEIGHT[j] * function(GaussianQuadrature.NODES[i], GaussianQuadrature.NODES[j]);
                }
            }

            return res;
        }
    }
}
