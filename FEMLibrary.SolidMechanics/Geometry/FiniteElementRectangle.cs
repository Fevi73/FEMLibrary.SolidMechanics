using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.Geometry
{
    public class FiniteElementRectangle:IFiniteElement
    {
        public FiniteElementNode Node1 { get; set; }
        public FiniteElementNode Node2 { get; set; }
        public FiniteElementNode Node3 { get; set; }
        public FiniteElementNode Node4 { get; set; }

        public int Count { get { return 4; } }

        public FiniteElementNode this[int index]
        {
            get 
            {
                switch (index)
                {
                    case 0: return Node1;
                    case 1: return Node2;
                    case 2: return Node3;
                    case 3: return Node4;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public override string ToString()
        {
            return "[ " + Node1.ToString() + "\t"+
                Node2.ToString() + "\t"+
                Node3.ToString() + "\t"+
                Node4.ToString() + " ]";
        }


    }
}
