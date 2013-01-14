using FEMLibrary.SolidMechanics.Geometry;
using MatrixLibrary;
using System.Collections.Generic;

namespace FEMLibrary.SolidMechanics.Results
{
    public interface IResult
    {
        string Name { get; }
        Vector GetResultAtPoint(Point point, double t);
    }

    public interface INumericalResult :IResult
    {
        IEnumerable<IFiniteElement> Elements { get; }
    }

    
}
