using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.FiniteElements
{
    public interface IFiniteElement
    {
        int Count { get; }
        FiniteElementNode this[int index] { get; }
    }
}
