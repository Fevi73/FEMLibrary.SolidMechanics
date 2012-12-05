using FEMLibrary.SolidMechanics.GUI.Models;
using System.Threading;
using System.Collections.Generic;
using FEMLibrary.SolidMechanics.Geometry;
using System.Windows;
using System;
using FEMLibrary.SolidMechanics.Meshing;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class RectangleMeshSettingsStepViewModel : WizardStepViewModelBase
    {
        public RectangleMeshSettingsStepViewModel(SolidMechanicsModel model)
            : base("Mesh Settings", model)
        {
            
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            VerticalElements = model.VerticalElements;
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
                return _solidMechanicsModel.VerticalElements;
            }

            set
            {
                if (_solidMechanicsModel.VerticalElements == value)
                {
                    return;
                }

                _solidMechanicsModel.VerticalElements = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(VerticalElementsPropertyName);
                FillResultPicture();
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
                return _solidMechanicsModel.HorizontalElements;
            }

            set
            {
                if (_solidMechanicsModel.HorizontalElements == value)
                {
                    return;
                }

                _solidMechanicsModel.HorizontalElements = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(HorizontalElementsPropertyName);
                FillResultPicture();
            }
        }

        private void FillResultPicture()
        {
            Rectangle rectangle = _solidMechanicsModel.Model.Shape as Rectangle;
            if (rectangle != null)
            {
                RectangularMesh mesh = new RectangularMesh(rectangle, _solidMechanicsModel.VerticalElements, _solidMechanicsModel.HorizontalElements);
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
