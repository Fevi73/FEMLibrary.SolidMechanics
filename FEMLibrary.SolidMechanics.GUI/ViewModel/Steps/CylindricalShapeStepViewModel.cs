using System;
using System.Collections.Generic;
using System.Linq;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.Geometry;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class CylindricalShapeStepViewModel:ShapeStepViewModel
    {
        public CylindricalShapeStepViewModel(SolidMechanicsModel model)
            : base(model)
        {
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            base.RefreshProperties(model);
            CylindricalPlate plate = model.Model.Shape as CylindricalPlate;
            if (plate != null)
            {
                Curvature = plate.Curvature;
            }
        }

        

        /// <summary>
        /// The <see cref="Height" /> property's name.
        /// </summary>
        public const string CurvaturePropertyName = "Curvature";

        /// <summary>
        /// Gets the Height property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double Curvature
        {
            get
            {
                if (isValid)
                {
                    CylindricalPlate plate = rectangle as CylindricalPlate;
                    if (plate != null)
                    {
                        return plate.Curvature;
                    }
                }

                return 0;
            }

            set
            {
                if (isValid)
                {
                    CylindricalPlate plate = rectangle as CylindricalPlate;
                    if (plate != null)
                    {
                        if (plate.Curvature == value)
                        {
                            return;
                        }

                        plate.Curvature = value;

                        // Update bindings, no broadcast
                        RaisePropertyChanged(HeightPropertyName);    
                    }
                    
                }
            }
        }

    }
}
