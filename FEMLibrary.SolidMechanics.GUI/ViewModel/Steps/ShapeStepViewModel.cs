using System;
using System.Collections.Generic;
using System.Linq;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class ShapeStepViewModel:WizardStepViewModelBase
    {
        private Rectangle rectangle;

        public ShapeStepViewModel(SolidMechanicsModel model):base("Shape", model)
        {
            rectangle = solidMechanicsModel.Model.Shape as Rectangle;
            IsValid = rectangle != null;
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            Rectangle rectangle = model.Model.Shape as Rectangle;
            if (rectangle != null)
            {
                Height = rectangle.Height;
                Width = rectangle.Width;
            }
        }

        /// <summary>
        /// The <see cref="IsValid" /> property's name.
        /// </summary>
        public const string IsValidPropertyName = "IsValid";

        private bool isValid = false;

        /// <summary>
        /// Gets the IsValid property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
            }

            set
            {
                if (isValid == value)
                {
                    return;
                }

                isValid = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsValidPropertyName);

            }
        }

        /// <summary>
        /// The <see cref="Height" /> property's name.
        /// </summary>
        public const string HeightPropertyName = "Height";

        /// <summary>
        /// Gets the Height property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double Height
        {
            get
            {
                if (isValid)
                {
                    return rectangle.Height;
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (isValid)
                {
                    if (rectangle.Height == value)
                    {
                        return;
                    }

                    rectangle.Height = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(HeightPropertyName);
                }
            }
        }

        /// <summary>
        /// The <see cref="Width" /> property's name.
        /// </summary>
        public const string WidthPropertyName = "Width";


        /// <summary>
        /// Gets the Width property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double Width
        {
            get
            {
                if (isValid)
                {
                    return rectangle.Width;
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (isValid)
                {
                    if (rectangle.Width == value)
                    {
                        return;
                    }

                    rectangle.Width = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(WidthPropertyName);
                }

            }
        }
    }
}
