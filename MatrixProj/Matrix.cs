using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixLibrary
{
    [Serializable]
    public class Matrix
    {
        private double[,] matrix;
        private int n; // kilkist rjadky
        private int m; // kilkist stovpciv

        public Matrix(int n, int m)
        {
            matrix = new double[n, m];
            this.n = n;
            this.m = m;

        }
        public Matrix(Matrix m)
        {
            this.n = m.n;
            this.m = m.m;
            matrix = new double[this.n, this.m];
            for (int i = 0; i < this.n; i++)
            {
                for (int j = 0; j < this.m; j++)
                {
                    matrix[i, j] = m[i, j];
                }
            }
        }

        #region - Properties -
        public bool IsQuadratic
        {
            get { return (n == m); }
        }

        public int CountRows
        {
            get { return n; }
        }

        public int CountColumns
        {
            get { return m; }
        }

        public double this[int i, int j]
        {
            get
            {
                if (((0 <= i) && (i <= n)) && ((0 <= j) && (j <= m)))
                    return matrix[i, j];
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (((0 <= i) && (i <= n)) && ((0 <= j) && (j <= m)))
                    matrix[i, j] = value;
                else throw new IndexOutOfRangeException();
            }
        }

        #endregion

        #region - Algorithms -

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.CountColumns == m2.CountRows)
            {
                Matrix res = new Matrix(m1.CountRows, m2.CountColumns);
                int N = m1.CountColumns;
                for (int i = 0; i < m1.CountRows; i++)
                {
                    for (int j = 0; j < m2.CountColumns; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < N; k++)
                        {
                            sum += m1[i, k] * m2[k, j];
                        }
                        res[i, j] = sum;
                    }
                }
                return res;
            }
            else
            {
                throw new CountException();
            }
        }

        public static Matrix operator *(double d, Matrix m)
        {
            Matrix res = new Matrix(m);
            for (int i = 0; i < m.CountRows; i++)
            {
                for (int j = 0; j < m.CountColumns; j++)
                {
                    res[i, j] *= d;
                }
            }
            return res;
        }

        public static Matrix operator *(Matrix m, double d)
        {
            Matrix res = new Matrix(m);
            for (int i = 0; i < m.CountRows; i++)
            {
                for (int j = 0; j < m.CountColumns; j++)
                {
                    res[i, j] *= d;
                }
            }
            return res;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if ((m1.CountColumns == m2.CountColumns) && (m1.CountRows == m2.CountRows))
            {
                Matrix res = new Matrix(m1.CountRows, m1.CountColumns);
                for (int i = 0; i < m1.CountRows; i++)
                {
                    for (int j = 0; j < m2.CountColumns; j++)
                    {
                        res[i, j] = m1[i, j] + m2[i, j];
                    }
                }
                return res;
            }
            else
            {
                throw new CountException();
            }
        }

        public static Matrix Transpose(Matrix mat)
        {
            Matrix res = new Matrix(mat.CountColumns, mat.CountRows);
            for (int i = 0; i < mat.CountRows; i++)
            {
                for (int j = 0; j < mat.CountColumns; j++)
                {
                    res[j, i] = mat[i, j];
                }
            }
            return res;
        }

        public double GetMaxElement(out int columnIndex, out int rowIndex) {
            columnIndex = 1;
            rowIndex = 0;
            double maxElement = Math.Abs(this[columnIndex, rowIndex]);
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < m; j++)
                {
                    if (i != j)
                    {
                        if (maxElement < Math.Abs(this[i, j]))
                        {
                            maxElement = Math.Abs(this[i, j]);
                            columnIndex = i;
                            rowIndex = j;
                        }
                    }
                }
            }
            return maxElement;

        }

        public bool IsDiagonal(double eps) {
            bool isDiag = true;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (i != j)
                    {
                        if (Math.Abs(this[i, j]) > eps)
                        {
                            isDiag = false;
                        }
                    }
                }
            }
            return isDiag;
        }

        
        #region - Solving system equation -

        public double Determinant()
        {
            if (IsQuadratic)
            {
                int N = this.n;
                Matrix L = new Matrix(N, N);
                Matrix U = new Matrix(N, N);
                this.GetLUMatrixs(L, U);

                double res = 1;
                for (int i = 0; i < N; i++)
                {
                    res *= U[i, i];
                }
                return res;
            }
            else throw new CountException("Matrix is not Quadratic!!!");
        }

        public void GetLUMatrixs(Matrix L, Matrix U)
        {
            if (IsQuadratic && L.IsQuadratic && U.IsQuadratic)
            {
                int N = this.m;
                for (int i = 0; i < N; i++)
                {
                    for (int j = i; j < N; j++)
                    {
                        double sum1 = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum1 += L[i, k] * U[k, j];
                        }
                        U[i, j] = this[i, j] - sum1;

                    }
                    for (int j = i + 1; j < N; j++)
                    {
                        double sum2 = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum2 += L[i, k] * U[k, j];
                        }
                        L[j, i] = (this[j, i] - sum2) / U[i, i];

                    }
                }
                for (int i = 0; i < N; i++)
                {
                    L[i, i] = 1;
                }
            }
            else throw new CountException("Matrix is not Quadratic!!!");
        }

        public Vector LUalgorithm(Vector b)
        {
            if (this.IsQuadratic)
            {
                int N = this.n;
                Matrix U = new Matrix(N, N);
                Matrix L = new Matrix(N, N);

                GetLUMatrixs(L, U);

                double[] y = new double[N];
                for (int i = 0; i < N; i++)
                {
                    double sum1 = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum1 += L[i, k] * y[k];
                    }
                    y[i] = b[i] - sum1;
                }

                Vector x = new Vector(N);
                for (int i = N - 1; i >= 0; i--)
                {
                    double sum2 = 0;
                    for (int k = N - 1; k > i; k--)
                    {
                        sum2 += U[i, k] * x[k];
                    }
                    x[i] = (y[i] - sum2) / U[i, i];
                }
                return x;
            }
            else throw new CountException("Matrix is not Quadratic!!!");
        }
        
        #endregion

        #region - Eigenvalue problem -

        /// <summary>
        /// Get max eigenvalue for positive-definite, symmetric matrix http://en.wikipedia.org/wiki/Positive-definite_matrix
        /// </summary>
        /// <param name="eigenvector">Correspond eigenvector for maximum eigenvalue</param>
        /// <returns>Maximum eigenvalue</returns>
        public double GetMaxEigenvalueSPAlgorithm(out Vector eigenvector, double eps)
        {
            double lambda = 0;
            int N = this.CountRows;
            eigenvector = new Vector(N);
            if (this.IsQuadratic)
            {
                double lambdaPrev = lambda;
                double lambdaPrevPrev = lambdaPrev;
                Vector y = new Vector(N);
                for (int i = 0; i < N; i++)
                {
                    y[i] = 1;
                }
                double s = y * y;
                double yNorm = Math.Sqrt(s);
                Vector x = y / yNorm;

                //diff
                do
                {
                    y = this * x;
                    s = y * y;
                    yNorm = Math.Sqrt(s);
                    double t = y * x;
                    x = y / yNorm;
                    lambdaPrevPrev = lambdaPrev;
                    lambdaPrev = lambda;
                    lambda = s / t;
                } while (Math.Abs(lambda - lambdaPrev) > eps);//Math.Abs(lambdaPrev - lambdaPrevPrev));
                eigenvector = x;
            }
            return lambda;
        }

        public double GetMinEigenvalueSPAlgorithm(out Vector eigenvector, double eps)
        {
            double lambda = 0;
            int N = this.CountRows;
            eigenvector = new Vector(N);
            if (this.IsQuadratic)
            {
                double lambdaPrev = lambda;
                Vector y = new Vector(N);
                for (int i = 0; i < N; i++)
                {
                    y[i] = 1;
                }
                double s = y * y;
                double yNorm = Math.Sqrt(s);
                Vector x = y / yNorm;

                //diff
                do
                {
                    y = this.LUalgorithm(x);
                    s = y * y;
                    yNorm = Math.Sqrt(s);
                    double t = y * x;
                    x = y / yNorm;
                    lambdaPrev = lambda;
                    lambda = s / t;
                } while (Math.Abs(lambda - lambdaPrev) > eps);
                eigenvector = x;
            }
            return (1/lambda);
        }

        public double[] GetEigenvalueSPAlgorithm(out Vector[] eigenvectors, double eps, int resultsCount)
        {
            double[] eigenvalues;
            double[,] eigvectors;
            
            alglib.smatrixevd(this.matrix, this.CountRows, 1, true, out eigenvalues, out eigvectors);

            eigenvectors = new Vector[eigenvalues.Length];
            for (int i = 0; i < eigenvalues.Length; i++) {
                eigenvectors[i] = new Vector(this.CountRows);
                for (int j = 0; j < this.CountRows; j++) {
                    eigenvectors[i][j] = eigvectors[j, i];
                }
            }

            return eigenvalues;

            /*double[] eigenvalues = new double[resultsCount];
            eigenvectors = new Vector[resultsCount];

            if (this.IsQuadratic)
            {
                int N = this.CountRows;
                Vector z = new Vector(N);
                for (int i = 0; i < N; i++)
                {
                    z[i] = 1;
                }

                for (int i = 0; i < resultsCount; i++)
                {
                    double lambda = 0;
                    double lambdaPrev = lambda;

                    Vector y = z;
                    for (int j = 0; j < i; j++)
                    {
                        y -= (z * eigenvectors[j]) * eigenvectors[j];
                    }
                    
                    double s = y * y;
                    double yNorm = Math.Sqrt(s);
                    Vector x = y / yNorm;

                    do
                    {
                        y = this.LUalgorithm(x);
                        s = y * y;
                        yNorm = Math.Sqrt(s);
                        double t = y * x;
                        x = y / yNorm;
                        lambdaPrev = lambda;
                        lambda = s / t;
                    } while (Math.Abs(lambda - lambdaPrev) > eps);
                    
                    eigenvectors[i] = x;
                    eigenvalues[i] = (1 / lambda);
                }
            }

            return eigenvalues;*/
        }

        public Vector GetEigenvalues(out Vector[] eigenvectors, double eps)
        {
            int iterations;
            return GetEigenvalues(out eigenvectors, out iterations, eps);
        }

        public Vector GetEigenvalues(out Vector[] eigenvectors, out int iterations, double eps)
        {
            eigenvectors = new Vector[this.n];
            Vector eigenvalues = null;
            iterations = 0;
            if (this.IsQuadratic)
            {
                Matrix currentMatrix = new Matrix(this);
                Matrix eigenVectorsMatrix = Matrix.IndentityMatrix(this.n);

                while (!currentMatrix.IsDiagonal(eps))
                {
                    int i, j;
                    currentMatrix.GetMaxElement(out i, out j);
                    double p = 2 * currentMatrix[i, j];
                    double q = currentMatrix[i, i] - currentMatrix[j, j];
                    double d = Math.Sqrt(p * p + q * q);
                    double c = Math.Sqrt(2) / 2;
                    double s = Math.Sqrt(2) / 2;
                    if (Math.Abs(q) > eps)
                    {
                        double r = Math.Abs(q) / (2 * d);
                        c = Math.Sqrt(0.5 + r);
                        s = Math.Sqrt(0.5 - r) * Math.Sign(p * q);
                    }

                    currentMatrix = MultiplyJacobiLeftAndRightMatrices(currentMatrix, c, s, i, j);

                    eigenVectorsMatrix = MultiplyJacobiRightMatrix(eigenVectorsMatrix, c, s, i, j);
                    iterations++;
                }

                eigenvalues = new Vector(this.m);
                for (int i = 0; i < this.m; i++)
                {
                    eigenvalues[i] = currentMatrix[i, i];
                    eigenvectors[i] = new Vector(this.n);
                    for (int j = 0; j < this.n; j++)
                    {
                        eigenvectors[i][j] = eigenVectorsMatrix[j, i];
                    }
                }
            }

            return eigenvalues;
        }

        private Matrix MultiplyJacobiLeftAndRightMatrices(Matrix matrix, double c, double s, int i, int j)
        {
            Matrix result = new Matrix(matrix);

            result[i, i] = c * c * matrix[i, i] + s * s * matrix[j, j] + 2 * c * s * matrix[i, j];
            result[j, j] = s * s * matrix[i, i] + c * c * matrix[j, j] - 2 * c * s * matrix[i, j];

            result[i, j] = (c * c - s * s) * matrix[i, j] + c * s * (matrix[j, j] - matrix[i, i]);
            result[j, i] = (c * c - s * s) * matrix[i, j] + c * s * (matrix[j, j] - matrix[i, i]);

            for (int im = 0; im < matrix.n; im++)
            {
                if ((im != i) && (im != j))
                {
                    result[i, im] = c * matrix[im, i] + s * matrix[im, j];
                    result[im, i] = c * matrix[im, i] + s * matrix[im, j];

                    result[j, im] = -s * matrix[im, i] + c * matrix[im, j];
                    result[im, j] = -s * matrix[im, i] + c * matrix[im, j];
                }
            }

            return result;
        }

        private Matrix MultiplyJacobiRightMatrix(Matrix matrix, double c, double s, int i, int j)
        {
            Matrix result = new Matrix(matrix);

            for (int im = 0; im < matrix.n; im++)
            {
                result[im, i] = c * matrix[im, i] + s * matrix[im, j];

                result[im, j] = -s * matrix[im, i] + c * matrix[im, j];
            }

            return result;
        }

        public static Matrix CutRow(Matrix matrix, int rowIndex) 
        {
            Matrix cuttedMatrix = new Matrix(matrix.CountRows - 1, matrix.CountColumns);
            int displacement = 0;
            for (int i = 0; i < matrix.CountRows; i++) {
                if (i != rowIndex)
                {
                    for (int j = 0; j < matrix.CountColumns; j++)
                    {
                        cuttedMatrix[i + displacement, j] = matrix[i, j];
                    }
                }
                else {
                    displacement = -1;
                }
            }
            return cuttedMatrix;
        }
        public static Matrix CutColumn(Matrix matrix, int columnIndex)
        {
            Matrix cuttedMatrix = new Matrix(matrix.CountRows, matrix.CountColumns - 1);
            for (int i = 0; i < matrix.CountRows; i++)
            {
                int displacement = 0;
                for (int j = 0; j < matrix.CountColumns; j++)
                {
                    if (j != columnIndex)
                    {
                        cuttedMatrix[i, j + displacement] = matrix[i, j];
                    }
                    else
                    {
                        displacement = -1;
                    }
                }
            }
            return cuttedMatrix;
        }

        public static Matrix Cut(Matrix matrix, int rowIndex, int columnIndex) {
            Matrix result = CutColumn(matrix, columnIndex);
            result = CutRow(result, columnIndex);
            return result;
        }


        #endregion

        #endregion

        public static Matrix IndentityMatrix(int n) {
            return DiagonalMatrix(1, n);
        }

        public static Matrix DiagonalMatrix(double value, int n)
        {
            Matrix matrix = new Matrix(n, n);
            for (int i = 0; i < n; i++)
            {
                matrix[i, i] = value;
            }
            return matrix;
        }

        public override string ToString()
        {
            string str_matrix = "";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    str_matrix += matrix[i, j].ToString("00.000") + "   ";
                }
                str_matrix += "\r\n";
            }
            return str_matrix; 
        }
        /*
        public override bool Equals(object obj)
        {
            bool isEqual = false;
            Vector vector = obj as Vector;
            if (vector != null)
            {
                isEqual = true;
                
            }
            return isEqual;
        }*/


    }

}
