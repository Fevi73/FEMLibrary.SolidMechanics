using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Solving;
using GalaSoft.MvvmLight.Command;
using MatrixLibrary;
using System.Timers;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using FEMLibrary.SolidMechanics.Results;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class SolveStepViewModel:WizardStepViewModelBase
    {

        public SolveStepViewModel(SolidMechanicsModel model):base("Solve", model)
        {
            SolveCommand = new RelayCommand(Solve);
            Results = new ObservableCollection<INumericalResult>();
            GridResults = new ObservableCollection<ResultItem>();
        }

        #region - Properties -

        /// <summary>
        /// The <see cref="Error" /> property's name.
        /// </summary>
        public const string ErrorPropertyName = "Error";

        private double _error = 0.0001;

        /// <summary>
        /// Gets the Error property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double Error
        {
            get
            {
                return _error;
            }

            set
            {
                if (_error == value)
                {
                    return;
                }

                _error = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ErrorPropertyName);

            }
        }

        /// <summary>
        /// The <see cref="MaxIterations" /> property's name.
        /// </summary>
        public const string MaxIterationsPropertyName = "MaxIterations";

        private double _maxIterations = 30;

        /// <summary>
        /// Gets the MaxIterations property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double MaxIterations
        {
            get
            {
                return _maxIterations;
            }

            set
            {
                if (_maxIterations == value)
                {
                    return;
                }

                _maxIterations = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MaxIterationsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MaxResults" /> property's name.
        /// </summary>
        public const string MaxResultsPropertyName = "MaxResults";

        private int _maxResults = 1;

        /// <summary>
        /// Gets the MaxResults property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int MaxResults
        {
            get
            {
                return _maxResults;
            }

            set
            {
                if (_maxResults == value)
                {
                    return;
                }

                _maxResults = value;

                RaisePropertyChanged(MaxResultsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrentResult" /> property's name.
        /// </summary>
        public const string CurrentResultPropertyName = "CurrentResult";

        private INumericalResult _currentResultProperty = null;

        /// <summary>
        /// Gets the CurrentResult property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public INumericalResult CurrentResult
        {
            get
            {
                return _currentResultProperty;
            }

            set
            {
                if (_currentResultProperty == value)
                {
                    return;
                }
                _currentResultProperty = value;
                ShowResult(_currentResultProperty, _pointsForGrid);
                RaisePropertyChanged(CurrentResultPropertyName);
            }
        }

        public ObservableCollection<INumericalResult> Results { get; private set; }

        public ObservableCollection<ResultItem> GridResults { get; private set; }

        #endregion

        private IEnumerable<FEMLibrary.SolidMechanics.Geometry.Point> _pointsForGrid;

        private Thread drawingThread;

        public void Solve()
        {
            Rectangle rectangle = _solidMechanicsModel.Model.Shape as Rectangle;
            if (rectangle != null)
            {
                RectangularMesh mesh = new RectangularMesh(rectangle, _solidMechanicsModel.VerticalElements, _solidMechanicsModel.HorizontalElements);

                Solver initSolver = new FreeVibrationsLinearSolver(_solidMechanicsModel.Model, mesh, _error);
                IEnumerable<INumericalResult> initResults = initSolver.Solve(1);
                EigenValuesNumericalResult res = initResults.First() as EigenValuesNumericalResult;


                //Solver solver = new NewmarkVibrationNonLinearSolver(_solidMechanicsModel.Model, mesh, _error, res.U, 5, 100);

                //Solver solver = new FreeVibrationsNonLinearSolver(_solidMechanicsModel.Model, mesh, _error, res.U, 2, 50);

                Solver solver = new FreeVibrationsLinearSolver(_solidMechanicsModel.Model, mesh, _error);

                //Solver solver = new StationaryNonlinear2DSolver(_solidMechanicsModel.Model, mesh, _error, 20);

                //IResult analiticalResult = new AnaliticalResultRectangleWithOneSideFixed(_solidMechanicsModel.Model);
                IEnumerable<INumericalResult> results = solver.Solve(_maxResults);
                
                _pointsForGrid = mesh.GetPointsForResult();
                Results.Clear();
                foreach (INumericalResult result in results)
                {
                    Results.Add(result);
                }
            }
        }

        private void ShowResult(INumericalResult result, IEnumerable<FEMLibrary.SolidMechanics.Geometry.Point> points)
        {
            if (drawingThread != null)
            {
                drawingThread.Abort();
            }

            if (result != null)
            {
                FillResultGrid(null, result, points);
                IEnumerable<IFiniteElement> elements = result.Elements;

                drawingThread = new Thread(delegate()
                {
                    DrawResult(result, elements);
                });
                drawingThread.Start();
            }
        }

        private void DrawResult(IResult numericalResult, IEnumerable<IFiniteElement> elements)
        {
            double time = 0;
            while (true)
            {
                FillResultPicture(numericalResult, elements, time);
                time+=0.01;
                Thread.Sleep(50);
            }
        }

        private void FillResultPicture(IResult numericalResult, IEnumerable<IFiniteElement> elements, double t)
        {
            List<Shape> shapes = new List<Shape>();
            foreach (FiniteElementRectangle fe in elements) {
                shapes.Add(ConvertFiniteElementToShape(fe, numericalResult, t));
            }
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Figures.Clear();
                    foreach (Shape shape in shapes)
                    {
                        Figures.Add(shape);
                    }
                }), null);
            }
            else 
            {
                Thread.CurrentThread.Abort();
            }
        }

        private Shape ConvertFiniteElementToShape(FiniteElementRectangle fe, IResult numericalResult, double t)
        {
            Rectangle rectangle = new Rectangle(fe.Node1.Point.X, fe.Node1.Point.Y, fe.Node3.Point.Y - fe.Node1.Point.Y, fe.Node3.Point.X - fe.Node1.Point.X);
            foreach (FEMLibrary.SolidMechanics.Geometry.Point point in rectangle.Points) 
            {
                MatrixLibrary.Vector numericalResultAtPoint = numericalResult.GetResultAtPoint(point, t);
                if (numericalResultAtPoint != null)
                {
                    point.X = point.X + numericalResultAtPoint[0];
                    point.Y = point.Y + numericalResultAtPoint[2];
                }
            }
            return rectangle;
        }

        

        private void FillResultGrid(IResult analiticalResult, IResult numericalResult, IEnumerable<FEMLibrary.SolidMechanics.Geometry.Point> points)
        {
            GridResults.Clear();
            foreach (FEMLibrary.SolidMechanics.Geometry.Point point in points)
            {
                ResultItem resultItem = new ResultItem();
                resultItem.Alfa1 = point.X;
                //Point point = new Point(node.Point.X, 0);//(-1)*(rectangle.Height/2));
                MatrixLibrary.Vector numericalResultAtPoint = numericalResult.GetResultAtPoint(point, 0);
                //MatrixLibrary.Vector analiticalResultAtPoint = analiticalResult.GetResultAtPoint(point, 0);

                if (numericalResultAtPoint != null)
                {
                    resultItem.U1Numeric = numericalResultAtPoint[0];
                    resultItem.U3Numeric = numericalResultAtPoint[2];
                }
                /*if (analiticalResultAtPoint != null)
                {
                    resultItem.U1Analitical = analiticalResultAtPoint[0];
                    resultItem.U3Analitical = analiticalResultAtPoint[2];
                }*/

                GridResults.Add(resultItem);
            }
        }

        public RelayCommand SolveCommand { get; private set; }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            
        }
    }
}
