﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.Geometry
{
    public class FiniteElementRectangleEdge
    {
        #region - Properties -

        public FiniteElementNode A
        {
            get;
            set;
        }

        public FiniteElementNode B
        {
            get;
            set;
        }

        public int Count
        {
            get { return 2; }
        }

        public FiniteElementNode this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return A;
                    case 1: return B;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion

        public FiniteElementRectangleEdge()
        {
            A = new FiniteElementNode();
            B = new FiniteElementNode();
        }

        public FiniteElementRectangleEdge(FiniteElementNode a, FiniteElementNode b)
        {
            this.A = a;
            this.B = b;
        }

        public double Length()
        {
            return FiniteElementNode.Length(A, B);
        }

    }

}
