using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Meshing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FEMLibrary.SolidMechanics.Testing
{
    
    
    /// <summary>
    ///This is a test class for MeshTest and is intended
    ///to contain all MeshTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MeshTest
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


        internal virtual Mesh CreateMesh()
        {
            // TODO: Instantiate an appropriate concrete class.
            Rectangle body = new Rectangle(1, 2, 4, 6);
            Mesh mesh = new RectangularMesh(body, 3, 3); 
            return mesh;
        }

        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateTest()
        {
            Mesh mesh = CreateMesh(); // TODO: Initialize to an appropriate value
            mesh.Generate();
            Assert.AreEqual(mesh.Elements.Count, 4);
        }
    }
}
