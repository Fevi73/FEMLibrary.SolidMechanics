using System;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Physics;
using MatrixLibrary;

namespace FEMLibrary.SolidMechanics.Results
{
    public class AnaliticalResultRectangleWithOneSideFixed:IResult
    {
        private Model _model;
        private Rectangle _rectangle;

        public AnaliticalResultRectangleWithOneSideFixed(Model model) {
            _model = model;

            double alfa1 = model.Material.GetAlfa1();
            _rectangle = _model.Shape as Rectangle;
            double h = 0;
            double l = 0;
            if (_rectangle != null)
            {
                h = _rectangle.Height;
                l = _rectangle.Width;
            }
            double E = model.Material.E[0];
            double v = model.Material.v[0, 0];

            D = E * h * h * h * (1 + alfa1) / (12 * (1 - v * v));
            Lambda = 14 * E * h / (30 * (1 + v));
            p = -model.BoundaryConditions[_rectangle.Edges[1]].Value[2];
            l2 = l * l;
            l3 = l * l * l;
            l4 = l * l * l * l;
            l5 = l * l * l * l * l;
            l6 = l * l * l * l * l * l;
            l7 = l * l * l * l * l * l * l;
            

            wK1 = p / (2 * Lambda);
            wK2 = p / (6 * D);
        }

        double l2; 
        double l3;
        double l4;
        double l5;
        double l6;
        double l7;

        double wK1;
        double wK2;

        double p;
        double Lambda;
        double D;

        public Vector GetResultAtPoint(Point point, double t)
        {
            Vector res = new Vector(3);

            double L = _rectangle.Width / 2;

            double x = point.X - L;
            double z = point.Y;//- model.Shape.H / 2;


            double w = wK1 * ((x - L) * (x - L) - l2)
                - wK2 / 4 * ((x - L) * (x - L) * (x - L) * (x - L) - l4)
                - wK2 * (l3 * x + l3 * L);

            double u = p * p * (1 / (6 * Lambda * D)) * ((x - L) * (x - L) * (x - L) * (x - L) * (x - L) / 5 + l3 * (x - L) * (x - L) * 0.5 - 0.3 * l5) -
                        p * p / (2 * Lambda * Lambda) * ((x - L) * (x - L) * (x - L) / 3 + l3 / 3) -
                        p * p / (72 * D * D) * ((x - L) * (x - L) * (x - L) * (x - L) * (x - L) * (x - L) * (x - L) / 7 + (x - L) * (x - L) * (x - L) * (x - L) * l3 / 2 + l6 * x + l7 / 7);
            //- p * p / (28 * D * D) * l7;


            double y = wK2 * ((x - L) * (x - L) * (x - L) + l3);

            res[0] = u + z * y;
            res[1] = 0;
            res[2] = w;

            return res;
        }

        public string Name
        {
            get { return "AnaliticalResultRectangleWithOneSideFixed"; }
        }
    }
}
