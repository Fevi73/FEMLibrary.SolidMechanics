using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Physics;
using MatrixLibrary;

namespace FEMLibrary.SolidMechanics.Solving
{
    public class AnaliticalResultRectangleWithTwoFixedSides : IResult
    {
        private double lambda1_2;
        private double lambda1;
        private double k_2;
        private double lambda_2;

        private double N0;

        private double p;
        private double Lambda;
        private double D;
        private double B;

        private double alfa1;
        private double h;
        private double l;
        private double E;
        private double v;

        private double A;
        private double ch;
        private double sh;
        private double _b;

        private Model _model;
        private Rectangle _rectangle;

        public AnaliticalResultRectangleWithTwoFixedSides(Model model, double sigma)
        {
            alfa1 = model.Material.GetAlfa1();
            _rectangle = _model.Shape as Rectangle;
            double h = 0;
            double l = 0;
            if (_rectangle != null)
            {
                h = _rectangle.Height;
                l = _rectangle.Width;
            }

            E = model.Material.E[0];
            v = model.Material.v[0, 0];

            D = E*h*h*h*(1 + alfa1)/(12*(1 - v*v));
            Lambda = 14*E*h/(30*(1 + v));
            B = E*h*(1 + alfa1)/(1 - v*v);
            N0 = sigma*h;

            lambda_2 = N0/D;
            k_2 = Lambda/(Lambda + N0);
            lambda1_2 = lambda_2*k_2;

            lambda1 = Math.Sqrt(lambda1_2);


            ch = Math.Cosh(lambda1*l/2);
            sh = Math.Sinh(lambda1*l/2);

            _b = l*(1/(Lambda + N0) - 1/(D*lambda_2))/(2*lambda1*sh);

            p = (-1)*CountLoad();


        }

        public double CountLoad()
        {
            double ldiv2 = l/2.0;
            double delta = Lambda*Lambda*(sh*ch - lambda1*ldiv2)/
                           (2*(Lambda + N0)*(Lambda + N0)*ldiv2*lambda1*sh*sh)
                           - 2*Lambda*(ldiv2*lambda1*ch - sh)/(lambda1_2*ldiv2*ldiv2*(Lambda + N0)*sh)
                           + 1.0/3.0;

            return Math.Sqrt((2*N0)/(B*delta))*(N0/ldiv2);
        }

        public Vector GetResultAtPoint(Point point, double t)
        {
            Vector res = new Vector(3);

            double L = l/2;

            double x = point.X - L;
            double z = point.Y;


            double w = _b*p*(Math.Cosh(lambda1*x) - ch) + p*(x*x - l*l/4)/(2*D*lambda_2);
            double u = 0;
            double y = 0;

            res[0] = u + z*y;
            res[1] = 0;
            res[2] = w;

            return res;
        }

        public string Name
        {
            get { return "AnaliticalResultRectangleWithTwoFixedSides"; }
        }
    }
}
