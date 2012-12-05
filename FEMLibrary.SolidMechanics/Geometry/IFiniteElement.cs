using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.Geometry
{
    public interface IFiniteElement
    {
        int Count { get; }
        FiniteElementNode this[int index] { get; }
    }
}
