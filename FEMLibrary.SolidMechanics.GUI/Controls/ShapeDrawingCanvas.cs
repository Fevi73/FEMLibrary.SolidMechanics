using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FEMLibrary.SolidMechanics.Geometry;
using System.Collections.Specialized;
using FEMLibrary.SolidMechanics.Utils;

namespace FEMLibrary.SolidMechanics.GUI.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FEMLibrary.SolidMechanics.GUI.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FEMLibrary.SolidMechanics.GUI.Controls;assembly=FEMLibrary.SolidMechanics.GUI.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ShapeDrawingCanvas/>
    ///
    /// </summary>
    public class ShapeDrawingCanvas : Canvas
    {
        private System.Windows.Point coordinateOrigin;

        public ShapeDrawingCanvas() 
        {
            coordinateOrigin = new System.Windows.Point(50, 400);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(ShapeDrawingCanvas_MouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(ShapeDrawingCanvas_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(ShapeDrawingCanvas_MouseLeftButtonUp);
        }


        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(ShapeDrawingCanvas), new FrameworkPropertyMetadata(ZoomPropertyChangedCallback));
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        private static void ZoomPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeDrawingCanvas canvas = d as ShapeDrawingCanvas;
            if (canvas != null)
            {
                canvas.Refresh(canvas.Shapes);
            }
        }

        public static readonly DependencyProperty ShapesProperty = 
            DependencyProperty.Register("Shapes", typeof(IEnumerable<Shape>), typeof(ShapeDrawingCanvas), new FrameworkPropertyMetadata(shapePropertyChangedCallback));
        public IEnumerable<Shape> Shapes 
        {
            get { return (IEnumerable<Shape>)GetValue(ShapesProperty); }
            set { SetValue(ShapesProperty, value); }
        }

        private static void shapePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeDrawingCanvas canvas = d as ShapeDrawingCanvas;
            if (canvas != null) {
                    canvas.OnShapesChanged(e.NewValue, e.OldValue);
            }
        }

        private void OnShapesChanged(object newShapes, object oldShapes)
        {
            IEnumerable<Shape> shapes = newShapes as IEnumerable<Shape>;
            Refresh(shapes);

            INotifyCollectionChanged collection = newShapes as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += new NotifyCollectionChangedEventHandler(collectionCollectionChanged);
            }

            INotifyCollectionChanged oldCollection = oldShapes as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(collectionCollectionChanged);
            }
        }

        private void Refresh(IEnumerable<Shape> shapes)
        {
            if (shapes != null)
            {
                this.DeleteAllShapes();
                this.DrawShapes(shapes);
            }
        }

        private void collectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (Shape shape in e.NewItems)
                        {
                            DrawShape(shape);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (Shape shape in e.OldItems)
                        {
                            RemoveShape(shape);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        DeleteAllShapes();
                        break;
                    }
            }
        }

        private Dictionary<Shape, Visual> visuals = new Dictionary<Shape, Visual>();
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            return visuals.Values.ToArray()[index];
        }
        public void AddVisual(Visual visual, Shape shape)
        {
            visuals.Add(shape, visual);
            AddVisualChild(visual);
            AddLogicalChild(visual);
        }
        public void DeleteVisual(Visual visual)
        {
            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        public void DeleteAllShapes() 
        {
            while (visuals.Count > 0) {
                Shape shape = visuals.Keys.Last();
                RemoveShape(shape);
            }
        }

        public void RemoveShape(Shape shape)
        {
            Visual visual = visuals[shape];
            visuals.Remove(shape);
            DeleteVisual(visual);
        }

        public void DrawShape(Shape shape)
        {
            DrawingVisual visual = new DrawingVisual();
            drawShape(visual, shape);
            this.AddVisual(visual, shape);
        }

        public void DrawShapes(IEnumerable<Shape> shapes) 
        {
            foreach (Shape shape in shapes)
            {
                this.DrawShape(shape);
            }
        }

        private Pen drawingPen = new Pen(Brushes.Red, 1);
        private void drawShape(DrawingVisual visual, Shape shape)
        {
            using (DrawingContext dc = visual.RenderOpen())
            {
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    int prevIndex = i - 1;
                    if (prevIndex < 0) 
                    { 
                        prevIndex = shape.Points.Count - 1; 
                    }

                    System.Windows.Point firstPoint = ConvertPoint(shape.Points[prevIndex]);
                    System.Windows.Point secondPoint = ConvertPoint(shape.Points[i]);

                    dc.DrawLine(drawingPen, firstPoint, secondPoint);
                }
            }
        }

        private new System.Windows.Point ConvertPoint(Geometry.Point point)
        {
            System.Windows.Point innerPoint = new System.Windows.Point(ConvertXCoordite(point.X), ConvertYCoordite(point.Y));
            return innerPoint;
        }

        private double ConvertXCoordite(double x)
        {
            return (Zoom * x) + coordinateOrigin.X;
        }

        private double ConvertYCoordite(double y)
        {
            return ((-1) * Zoom * y) + coordinateOrigin.Y;
        }

        private bool isDragging = false;
        private System.Windows.Point startPoint;

        private void ShapeDrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            startPoint = e.GetPosition(this);
        }
        
        private void ShapeDrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                System.Windows.Point currentPoint = e.GetPosition(this);
                coordinateOrigin.X += currentPoint.X - startPoint.X;
                coordinateOrigin.Y += currentPoint.Y - startPoint.Y;
                this.Refresh(this.Shapes);
                startPoint = currentPoint;
                /*
                System.Diagnostics.Trace.WriteLine("---------------------");
                System.Diagnostics.Trace.WriteLine(coordinateOrigin);
                */
            }
        }
        private void ShapeDrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        } 

    }
}
