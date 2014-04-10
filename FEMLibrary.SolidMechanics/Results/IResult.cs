using FEMLibrary.SolidMechanics.FiniteElements;
using FEMLibrary.SolidMechanics.Geometry;
using MatrixLibrary;
using System.Collections.Generic;

namespace FEMLibrary.SolidMechanics.Results
{
    public interface IResult<T>
    {
        string Name { get; }
        T GetResultAtPoint(Point point, double t);
    }

    public interface IResult : IResult<Vector>
    {
    }

    public interface INumericalResult :IResult
    {
        IEnumerable<IFiniteElement> Elements { get; }
    }

    
}
