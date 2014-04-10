using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.FiniteElements
{
    public class FiniteElementSegment:IFiniteElement
    {
        public FiniteElementNode Node1 { get; set; }
        public FiniteElementNode Node2 { get; set; }

        public int Count { get { return 2; } }

        public FiniteElementNode this[int index]
        {
            get 
            {
                switch (index)
                {
                    case 0: return Node1;
                    case 1: return Node2;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public override string ToString()
        {
            return "[ " + Node1.ToString() + "\t"+
                Node2.ToString() + " ]";
        }


    }
}
