using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.Results;
using FEMLibrary.SolidMechanics.Solving;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

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

        private double error = 0.0001;

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
                return error;
            }

            set
            {
                if (error == value)
                {
                    return;
                }

                error = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ErrorPropertyName);

            }
        }

        /// <summary>
        /// The <see cref="MaxIterations" /> property's name.
        /// </summary>
        public const string MaxIterationsPropertyName = "MaxIterations";

        private int maxIterations = 30;

        /// <summary>
        /// Gets the MaxIterations property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int MaxIterations
        {
            get
            {
                return maxIterations;
            }

            set
            {
                if (maxIterations == value)
                {
                    return;
                }

                maxIterations = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MaxIterationsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MaxResults" /> property's name.
        /// </summary>
        public const string MaxResultsPropertyName = "MaxResults";

        private int maxResults = 1;

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
                return maxResults;
            }

            set
            {
                if (maxResults == value)
                {
                    return;
                }

                maxResults = value;

                RaisePropertyChanged(MaxResultsPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="TimeElapsed" /> property's name.
        /// </summary>
        public const string TimeElapsedPropertyName = "TimeElapsed";

        private TimeSpan timeElapsed;

        /// <summary>
        /// Gets the TimeElapsed property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public TimeSpan TimeElapsed
        {
            get
            {
                return timeElapsed;
            }

            set
            {
                if (timeElapsed == value)
                {
                    return;
                }

                timeElapsed = value;

                RaisePropertyChanged(TimeElapsedPropertyName);
            }
        }
        /// <summary>
        /// The <see cref="CurrentResult" /> property's name.
        /// </summary>
        public const string CurrentResultPropertyName = "CurrentResult";

        private INumericalResult currentResultProperty = null;

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
                return currentResultProperty;
            }

            set
            {
                if (currentResultProperty == value)
                {
                    return;
                }
                currentResultProperty = value;
                ShowResult(currentResultProperty, pointsForGrid);
                RaisePropertyChanged(CurrentResultPropertyName);
            }
        }

        public ObservableCollection<INumericalResult> Results { get; private set; }

        public ObservableCollection<ResultItem> GridResults { get; private set; }

        #endregion

        private IEnumerable<FEMLibrary.SolidMechanics.Geometry.Point> pointsForGrid;

        private Thread drawingThread;

        public void Solve()
        {
            CylindricalPlate plate = solidMechanicsModel.Model.Shape as CylindricalPlate;
            if (plate != null)
            {
                LinearMesh mesh = new LinearMesh(plate, solidMechanicsModel.HorizontalElements);
                Stopwatch sw = new Stopwatch();
                sw.Start();


                Solver solver = new CylindricalPlate1DSolver(solidMechanicsModel.Model, mesh, error, solidMechanicsModel.MaxAmplitude);

                IEnumerable<INumericalResult> results = solver.Solve(maxResults);
                
                sw.Stop();
                TimeElapsed = sw.Elapsed;
                pointsForGrid = mesh.GetPointsForResult();
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

                /*drawingThread = new Thread(delegate()
                {
                    DrawResult(result, elements);
                });
                drawingThread.Start();
                 */
            }
        }

        private void DrawResult(IResult numericalResult, IEnumerable<IFiniteElement> elements)
        {
            double time = 0;
            FillResultPicture(numericalResult, elements, time);
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
            foreach (FiniteElementRectangle fe in elements) 
            {
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
                }));
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
