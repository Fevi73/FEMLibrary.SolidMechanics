using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.Results
{
    public class CauchyProblemResult : IResult
    {
        public CauchyProblemResult(double deltaTime) {
            _deltaTime = deltaTime;
            results = new List<Vector>();
        }

        private List<Vector> results;
        private double _deltaTime;

        public List<Vector> Results
        {
            get { return results; }
        }
        public double DeltaTime
        {
            get { return _deltaTime; }
        }

        public virtual string Name
        {
            get { return "Cauchy Problem Result"; }
        }

        public void AddResult(Vector result)
        {
            results.Add(result);
        }

        public Vector GetResultAtPoint(Point point, double t)
        {
            int time = (int)(t / _deltaTime);
            return results[time];
        }
    }
}
