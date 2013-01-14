using System.Collections.Generic;
using System.Linq;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Physics;
using FEMLibrary.SolidMechanics.Solving;
using MatrixLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FEMLibrary.SolidMechanics.Utils;
using FEMLibrary.SolidMechanics.Results;

namespace FEMLibrary.SolidMechanics.Testing
{
    
    
    /// <summary>
    ///This is a test class for StationaryNonlinear2DSolverTest and is intended
    ///to contain all StationaryNonlinear2DSolverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StationaryNonlinear2DSolverTest
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
            Rectangle body = new Rectangle(1, 1);

            Material material = Material.Aluminium();
            Model model = new Model(material, body);

            model.BoundaryConditions[body.Edges[0]].Type = BoundaryConditionsType.Static;
            
            double load = 1000;
            model.BoundaryConditions[body.Edges[1]].Type = BoundaryConditionsType.Kinematic;
            model.BoundaryConditions[body.Edges[1]].Value[2] = load;
            
            Mesh mesh = new RectangularMesh(body, 2, 2); 
            
            double error = 0.001;
            int maxIterations = 20;
            Solver solver = new StationaryNonlinear2DSolver(model, mesh, error, maxIterations);

            IResult actual = solver.Solve(1).FirstOrDefault();
            IResult expected = new AnaliticalResultRectangleWithOneSideFixed(model);
            
            IEnumerable<Point> points = mesh.GetPointsForResult();

            Assert.IsTrue(ResultHelper.IsResultsEqual(points, expected, actual, 0, error));
        }
    }
}
