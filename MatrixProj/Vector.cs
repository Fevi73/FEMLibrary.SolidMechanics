using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixLibrary
{
    [Serializable]
    public class Vector
    {
        private double[] vector;
        private int n; // kilkist elementiv

        public Vector(int length)
        {
            vector = new double[length];
            n = length;
        }
        public Vector(Vector v)
        {
            this.n = v.n;
            vector = new double[this.n];
            for (int i = 0; i < this.n; i++)
            {
                vector[i] = v[i];
            }
        }

        #region - Properties -
        public int Length
        {
            get { return n; }
        }

        public double this[int i]
        {
            get
            {
                if ((0 <= i) && (i <= n))
                    return vector[i];
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if ((0 <= i) && (i <= n))
                    vector[i] = value;
                else throw new IndexOutOfRangeException();
            }
        }

        #endregion


        public override string ToString()
        {
            string str_vector = "{";
            for (int i = 0; i < n; i++)
            {
                str_vector += vector[i].ToString("00.000") + "   ";
            }
            str_vector += "}";
            return str_vector;
        }

        #region - Reload operation -
        public static Vector operator +(Vector v1, Vector v2)
        {
            if (v1.Length == v2.Length)
            {
                Vector res = new Vector(v1.Length);
                for (int i = 0; i < v1.Length; i++)
                {
                    res[i] = v1[i] + v2[i];
                }
                return res;
            }
            else
            {
                throw new Exception("Sum of vector cannot be count!!!");
            }
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            if (v1.Length == v2.Length)
            {
                Vector res = new Vector(v1.Length);
                for (int i = 0; i < v1.Length; i++)
                {
                    res[i] = v1[i] - v2[i];
                }
                return res;
            }
            else
            {
                throw new Exception("Sum of vector cannot be count!!!");
            }
        }

        public static Vector operator *(double c, Vector v)
        {
            Vector res = new Vector(v.Length);
            for (int i = 0; i < v.Length; i++)
            {
                res[i] = c * v[i];
            }
            return res;
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            if (v.Length == m.CountColumns)
            {
                Vector res = new Vector(m.CountRows);
                int N = m.CountColumns;
                for (int i = 0; i < m.CountRows; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < N; j++)
                    {
                        sum += m[i, j] * v[j];
                    }
                    res[i] = sum;
                }
                return res;
            }
            else
            {
                throw new CountException();
            }
        }

        public static Vector operator *(Vector v, Matrix m)
        {
            if (v.Length == m.CountRows)
            {
                Vector res = new Vector(m.CountColumns);
                int N = m.CountRows;
                for (int i = 0; i < m.CountColumns; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < N; j++)
                    {
                        sum += m[j, i] * v[j];
                    }
                    res[i] = sum;
                }
                return res;
            }
            else
            {
                throw new CountException();
            }
        }

        public static Vector operator /(Vector v, double number)
        {
            if (number != 0)
            {
                Vector res = new Vector(v.Length);
                for (int i = 0; i < v.Length; i++)
                {
                    res[i] = v[i] / number;
                }
                return res;
            }
            else
            {
                throw new CountException();
            }
        }

        public static double operator *(Vector v1, Vector v2)
        {
            if (v1.Length == v2.Length)
            {
                int N = v1.Length;
                double sum = 0;
                for (int i = 0; i < N; i++)
                {

                    sum += v1[i] * v2[i];
                }
                return sum;
            }
            else
            {
                throw new CountException();
            }
        }
        #endregion

        public void Copy(Vector v) 
        {
            int minLength = this.Length > v.Length ? v.Length : this.Length;
            for (int i = 0; i < minLength; i++) {
                this[i] = v[i];
            }
        }

        public static double Norm(Vector v)
        {
            double sum = 0;
            for (int i = 0; i < v.Length; i++)
            {
                sum += v[i] * v[i];
            }
            return Math.Sqrt(sum);
        }

        public void Insert(double value, int index) {
            n++;
            double[] newVector = new double[n];
            for (int i = 0; i < index; i++) {
                newVector[i] = vector[i];
            }
            newVector[index] = value;
            for (int i = index+1; i < n; i++)
            {
                newVector[i] = vector[i-1];
            }
            vector = newVector;
        }

        public static Vector Cut(Vector vector, int index)
        {
            Vector cuttedVector = new Vector(vector.Length - 1);
            int displacement = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                if (i != index)
                {
                    cuttedVector[i + displacement] = vector[i];
                }
                else
                {
                    displacement = -1;
                }
            }
            return cuttedVector;
        }
    }
}
