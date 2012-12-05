using MatrixLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FEMLibrary.SolidMechanics.Testing
{
    
    
    /// <summary>
    ///This is a test class for VectorTest and is intended
    ///to contain all VectorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VectorTest
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
        ///A test for Insert
        ///</summary>
        [TestMethod()]
        public void InsertTest()
        {
            int startLength = 5;
            Vector vector = new Vector(startLength);// TODO: Initialize to an appropriate value
            vector[0] = 1;
            vector[1] = 2;
            vector[2] = 3;
            vector[3] = 4;
            vector[4] = 5;
            double value = 10; // TODO: Initialize to an appropriate value
            int index = 5; // TODO: Initialize to an appropriate value
            vector.Insert(value, index);
            Assert.AreEqual(startLength+1, vector.Length);
        }
    }
}
