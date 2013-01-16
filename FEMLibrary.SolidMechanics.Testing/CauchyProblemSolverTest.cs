using FEMLibrary.SolidMechanics.ODE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MatrixLibrary;
using FEMLibrary.SolidMechanics.Results;

namespace FEMLibrary.SolidMechanics.Testing
{
    
    
    /// <summary>
    ///This is a test class for CauchyProblemSolverTest and is intended
    ///to contain all CauchyProblemSolverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CauchyProblemSolverTest
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


        /// <summary>
        ///A test for HeunMethodSolve
        ///</summary>
        [TestMethod()]
        public void HeunMethodSolveTest()
        {
            Func<double, Vector, Vector> f = testF;
            
            Vector initValue = new Vector(2);
            initValue[0] = 0;
            initValue[1] = -3;

            double maxTime = 20; 
            int intervals = 200; 
            CauchyProblemResult actual = CauchyProblemSolver.HeunMethodSolve(f, initValue, maxTime, intervals);
            Assert.AreEqual(actual, actual);
        }

        private Vector testF(double t, Vector v)
        {
            Vector res = new Vector(2);
            res[0] = v[1];
            res[1] = t - 2 - v[1];
            return res;
        }


    }
}
