using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FEMLibrary.SolidMechanics.Results;
using MatrixLibrary;

namespace FEMLibrary.SolidMechanics.ODE
{
    public class CauchyProblemSolver
    {
        public static CauchyProblemResult HeunMethodSolve(Func<double, Vector, Vector> f, Vector initValue, double maxTime, int intervals)
        {
            double deltaTime = maxTime / intervals;
            CauchyProblemResult result = new CauchyProblemResult(deltaTime);
            Vector v = initValue;
            result.AddResult(v);

            for (double t = deltaTime; t <= maxTime; t += deltaTime)
            {
                Vector prevV = result.Results.Last();
                Vector prevF = f(t, prevV);
                v = prevV + (deltaTime / 2) * (f(t, prevV + deltaTime * prevF) + prevF);
                result.AddResult(v);
            }

            return result;
        }

        public static CauchyProblemResult AdamsBashforthMethodsSolve(Func<double, Vector, Vector> f, Vector initValue, double maxTime, int intervals)
        {
            double deltaTime = maxTime / intervals;
            CauchyProblemResult result = new CauchyProblemResult(deltaTime);

            CauchyProblemResult firstResult = CauchyProblemSolver.HeunMethodSolve(f, initValue, deltaTime * 2, 2);

            foreach (Vector res in firstResult.Results)
            {
                result.AddResult(res);
            }

            for (int t = 3; t <= intervals; t++)
            {
                int lastIndex = result.Results.Count - 1;
                Vector u_n_2 = result.Results[lastIndex - 2];
                Vector u_n_1 = result.Results[lastIndex - 1];
                Vector u_n = result.Results[lastIndex];

                Vector u = u_n + (deltaTime / 12) * (23 * f(deltaTime * lastIndex, u_n) - 16 * f(deltaTime * (lastIndex - 1), u_n_1) + 5 * f(deltaTime * (lastIndex - 2), u_n_2));
                result.AddResult(u);
            }

            return result;
        }

        public static CauchyProblemResult GirMethodsSolve(Func<double, Vector, Vector> f, Vector initValue, double maxTime, int intervals)
        {
            double eps = 0.001;
            double deltaTime = maxTime / intervals;
            CauchyProblemResult result = new CauchyProblemResult(deltaTime);

            CauchyProblemResult firstResult = CauchyProblemSolver.HeunMethodSolve(f, initValue, deltaTime * 2, 2);

            foreach (Vector res in firstResult.Results)
            {
                result.AddResult(res);
            }

            for (int t = 3; t <= intervals; t++)
            {
                int lastIndex = result.Results.Count - 1;
                Vector u_n_2 = result.Results[lastIndex - 2];
                Vector u_n_1 = result.Results[lastIndex - 1];
                Vector u_n = result.Results[lastIndex];

                Vector u = u_n + (deltaTime / 12) * (23 * f(deltaTime * lastIndex, u_n) - 16 * f(deltaTime * (lastIndex - 1), u_n_1) + 5 * f(deltaTime * (lastIndex - 2), u_n_2));
                Vector prevU = new Vector(u.Length);

                Vector constU = (18 * u_n - 9 * u_n_1 + 2 * u_n_2) / 11;

                while (Vector.Norm(u - prevU) > eps)
                {
                    prevU = u;
                    u = ((6 * deltaTime) / 11) * f(deltaTime * lastIndex, prevU) + constU;
                }

                result.AddResult(u);
            }

            return result;
        }
    }
}
