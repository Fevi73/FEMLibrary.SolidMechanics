using System.Linq;
using FEMLibrary.SolidMechanics.Solving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FEMLibrary.SolidMechanics.Physics;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Utils;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Results;

namespace FEMLibrary.SolidMechanics.Testing
{
    
    
    /// <summary>
    ///This is a test class for FreeVibrationsLinearSolverTest and is intended
    ///to contain all FreeVibrationsLinearSolverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FreeVibrationsLinearSolverTest
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
        ///A test for Solve
        ///</summary>
        [TestMethod()]
        public void SolveTest()
        {
            double error = 0.001;
            Rectangle rectangle = new Rectangle(0.1, 1);
            Model model = new Model(Material.Aluminium(), rectangle); // TODO: Initialize to an appropriate value
            model.PointConditions[rectangle.Points[2]].Type = BoundaryConditionsType.Static;
            model.PointConditions[rectangle.Points[3]].Type = BoundaryConditionsType.Static;
            Mesh mesh = new RectangularMesh(rectangle, 10, 4); // TODO: Initialize to an appropriate value
            FreeVibrationsLinearSolver solver = new FreeVibrationsLinearSolver(model, mesh, error); // TODO: Initialize to an appropriate value
            IResult expected = null; // TODO: Initialize to an appropriate value
            IResult actual = null;
            actual = solver.Solve(1).FirstOrDefault();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Solve
        ///</summary>
        [TestMethod()]
        public void SolveSimpleTest()
        {
            double error = 0.001;
            Rectangle rectangle = new Rectangle(1, 1);
            Model model = new Model(Material.Aluminium(), rectangle);
            model.PointConditions[rectangle.Points[2]].Type = BoundaryConditionsType.Static;
            model.PointConditions[rectangle.Points[3]].Type = BoundaryConditionsType.Static;
            //model.BoundaryConditions[rectangle.Edges[0]].Type = BoundaryConditionsType.Static;
            //model.BoundaryConditions[rectangle.Edges[2]].Type = BoundaryConditionsType.Static;
            Mesh mesh = new RectangularMesh(rectangle, 2, 2); 
            FreeVibrationsLinearSolver solver = new FreeVibrationsLinearSolver(model, mesh, error);
            IResult expected = null; // TODO: Initialize to an appropriate value
            IResult actual = solver.Solve(1).FirstOrDefault();

            IEnumerable<Point> points = mesh.GetPointsForResult();

            ResultHelper.IsResultsEqual(points, expected, actual, 0, error);

            Assert.AreEqual(true, false);
        }
    }
}
