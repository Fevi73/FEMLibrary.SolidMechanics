using FEMLibrary.SolidMechanics.GUI.Models;
using System.Threading;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;
using System.Windows;
using System;
using FEMLibrary.SolidMechanics.Meshing;
using FEMLibrary.SolidMechanics.FiniteElements;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class RectangleMeshSettingsStepViewModel : WizardStepViewModelBase
    {
        public RectangleMeshSettingsStepViewModel(SolidMechanicsModel model)
            : base("Mesh Settings", model)
        { }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            SolidMechanicsModel2D m = model as SolidMechanicsModel2D;
            if (m != null)
            {
                VerticalElements = m.VerticalElements;
            }
            HorizontalElements = model.HorizontalElements;
        }

        /// <summary>
        /// The <see cref="VerticalElements" /> property's name.
        /// </summary>
        public const string VerticalElementsPropertyName = "VerticalElements";

        /// <summary>
        /// Gets the VerticalElements property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int VerticalElements
        {
            get
            {
                SolidMechanicsModel2D m = solidMechanicsModel as SolidMechanicsModel2D;
                if (m != null)
                {
                    return m.VerticalElements;
                }
                return 0;
            }

            set
            {
                SolidMechanicsModel2D m = solidMechanicsModel as SolidMechanicsModel2D;
                if (m != null)
                {
                    if (m.VerticalElements == value)
                    {
                        return;
                    }

                    m.VerticalElements = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(VerticalElementsPropertyName);
                    FillResultPicture();
                }

                
            }
        }

        /// <summary>
        /// The <see cref="HorizontalElements" /> property's name.
        /// </summary>
        public const string HorizontalElementsPropertyName = "HorizontalElements";

        /// <summary>
        /// Gets the HorizontalElements property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public int HorizontalElements
        {
            get
            {
                return solidMechanicsModel.HorizontalElements;
            }

            set
            {
                if (solidMechanicsModel.HorizontalElements == value)
                {
                    return;
                }

                solidMechanicsModel.HorizontalElements = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(HorizontalElementsPropertyName);
                FillResultPicture();
            }
        }

        private void FillResultPicture()
        {
            Rectangle rectangle = solidMechanicsModel.Model.Shape as Rectangle;
            if (rectangle != null)
            {
                RectangularMesh mesh = new RectangularMesh(rectangle, VerticalElements, HorizontalElements);
                List<Shape> shapes = new List<Shape>();
                foreach (FiniteElementRectangle fe in mesh.Elements)
                {
                    shapes.Add(ConvertFiniteElementToShape(fe));
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Figures.Clear();
                    foreach (Shape shape in shapes)
                    {
                        Figures.Add(shape);
                    }
                }));
            }
        }

        private Shape ConvertFiniteElementToShape(FiniteElementRectangle fe)
        {
            Rectangle rectangle = new Rectangle(fe.Node1.Point.X, fe.Node1.Point.Y, fe.Node3.Point.Y - fe.Node1.Point.Y, fe.Node3.Point.X - fe.Node1.Point.X);
            return rectangle;
        }
    }
}
