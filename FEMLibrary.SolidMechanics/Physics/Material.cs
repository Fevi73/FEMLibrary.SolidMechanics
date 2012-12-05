using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrixLibrary;

namespace FEMLibrary.SolidMechanics.Physics
{
    [Serializable]
    public class Material
    {
        public const int DIMENSIONS = 3;

        public Vector E { get; set; }
        public Matrix v { get; set; }
        public Matrix G { get; set; }
        public double Rho { get; set; }

        public Material()
        {
            E = new Vector(DIMENSIONS);
            v = new Matrix(DIMENSIONS, DIMENSIONS);
            G = new Matrix(DIMENSIONS, DIMENSIONS);
            Rho = 0;
        }

        public static Material CreateHomogeniousMaterial(double E, double v, double rho = 0)
        {
            Material material = new Material();
            material.SetHomogeniusValues(E, v, rho);
            return material;
        }

        public void SetHomogeniusValues(double E, double v, double rho = 0) {
            this.AutoFillV(v);
            this.AutoiFillE(E);
            this.AutoFillG();
            this.Rho = rho;
        }

        public void Copy(Material material) 
        {
            this.SetHomogeniusValues(material.E[0], material.v[0,0], material.Rho);
        }

        public static Material Cooper() {
            return CreateHomogeniousMaterial(120000, 0.34, 8940);
        }

        public static Material Steel()
        {
            return CreateHomogeniousMaterial(210000, 0.28, 7860);
        }

        public static Material Aluminium()
        {
            return CreateHomogeniousMaterial(70000, 0.34, 2700);
        }

        public double GetAlfa1()
        {
            double D = 1 - v[0, 1] * v[1, 0] - v[0, 2] * v[2, 0] - v[2, 1] * v[1, 2] - v[0, 2] * v[2, 1] * v[1, 0] - v[1, 2] * v[2, 0] * v[0, 1];
            return (v[2, 0] + v[1, 0] * v[2, 1]) * (v[2, 0] + v[1, 0] * v[2, 1]) * E[0] / (D * E[2]);
        }

        public double GetE1Modif()
        {
            double delta = 1 - v[0, 1] * v[1, 0];
            return E[0] / (delta * delta);
        }

        public double GetE0()
        {
            double D = 1 - v[0, 1] * v[1, 0] - v[0, 2] * v[2, 0] - v[2, 1] * v[1, 2] - v[0, 2] * v[2, 1] * v[1, 0] - v[1, 2] * v[2, 0] * v[0, 1];
            double delta = 1 - v[0, 1] * v[1, 0];
            return E[2] * delta * delta / D;
        }

        public double GetLambda1()
        {
            double delta = 1 - v[0, 1] * v[1, 0];
            return ((v[2, 1] + v[1, 0] * v[2, 1]) * E[0]) / (E[2] * delta * delta);
        }

        public double GetG13() 
        {
            return G[0, 2];
        }

        public void AutoFillG()
        {
            for (int i = 0; i < DIMENSIONS; i++)
            {
                for (int j = 0; j < DIMENSIONS; j++)
                {
                    G[i, j] = E[i] / (2 * (1 + v[i, j]));
                }
            }
        }

        public void AutoFillV(double vCoef)
        {
            for (int i = 0; i < DIMENSIONS; i++)
            {
                for (int j = 0; j < DIMENSIONS; j++)
                {
                    v[i, j] = vCoef;
                }
            }
        }

        public void AutoiFillE(double eCoef)
        {
            for (int i = 0; i < DIMENSIONS; i++)
            {
                E[i] = eCoef;
            }
        }
    }
}
