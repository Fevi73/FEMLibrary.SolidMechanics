using MatrixLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FEMLibrary.SolidMechanics.Testing
{
    
    
    /// <summary>
    ///This is a test class for MatrixTest and is intended
    ///to contain all MatrixTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MatrixTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private Matrix Matrix1() {
            //{-04.000   02.000   }
            Matrix m = new Matrix(2, 2); // TODO: Initialize to an appropriate value
            m[0, 0] = -1;
            m[0, 1] = 3;
            m[1, 0] = 3;
            m[1, 1] = -1;
            return m;
        }

        private Matrix Matrix2()
        {
            //{06.236   01.764  }
            Matrix m = new Matrix(2, 2);
            m[0, 0] = 5;
            m[1, 1] = 3;
            m[0, 1] = 2;
            m[1, 0] = 2;
            return m;
        }

        private Matrix Matrix3()
        {
            //{03.414   00.586   02.000   }
            Matrix m = new Matrix(3, 3); 
            m[0, 0] = 2;
            m[1, 1] = 2;
            m[2, 2] = 2;
            m[0, 1] = -1;
            m[1, 0] = -1;
            m[1, 2] = -1;
            m[2, 1] = -1;
            return m;
        }

        private Matrix Matrix4()
        {
            Matrix m = new Matrix(3, 3); 
            m[0, 0] = 1;
            m[1, 1] = 1;
            m[2, 2] = 1;
            m[2, 0] = 1;
            m[0, 2] = 1;
            m[0, 1] = -1;
            m[1, 0] = -1;
            m[1, 2] = -1;
            m[2, 1] = -1;
            return m;
        }

        [TestMethod()]
        public void TestJacobiAlgorithm()
        {
            Matrix m = Matrix3();
            double eps = 0.001; 
            Vector[] eigenVectors;

            Vector lambdas = m.GetEigenvalues(out eigenVectors, eps);

            int eigenIndex = 2;

            Vector expected = lambdas[eigenIndex] * eigenVectors[eigenIndex];
            Vector actual = m * eigenVectors[eigenIndex];
            Assert.IsTrue(Vector.Norm(actual - expected) < eps, "Close enough");
        }

        [TestMethod()]
        public void TestMaxEigenvalue()
        {
            Matrix m = Matrix3();
            double eps = 0.001;
            Vector eigenVector;

            double lambda = m.GetMaxEigenvalueSPAlgorithm(out eigenVector, eps);

            Vector expected = lambda * eigenVector;
            Vector actual = m * eigenVector;
            
            Assert.IsTrue(Vector.Norm(actual - expected) < eps, "Close enough");

        }

        [TestMethod()]
        public void TestMinEigenvalue()
        {
            Matrix m = Matrix3();

            Vector eigenVector;
            double eps = 0.001;
            double lambda = m.GetMinEigenvalueSPAlgorithm(out eigenVector, eps);

            Vector expected = lambda * eigenVector;
            Vector actual = m * eigenVector;
            
            Assert.IsTrue(Vector.Norm(actual - expected) < eps, "Close enough");
        }

        [TestMethod()]
        public void TestSequenceEigenvalues()
        {
            Matrix m = Matrix3();
            double eps = 0.001;
            Vector[] eigenVectors;

            double[] lambdas = m.GetEigenvalueSPAlgorithm(out eigenVectors, eps, 3);

            Vector expected = lambdas[0] * eigenVectors[0];
            Vector actual = m * eigenVectors[0];
            Assert.IsTrue(Vector.Norm(actual - expected) < eps, "Close enough");
        }
    }
}
