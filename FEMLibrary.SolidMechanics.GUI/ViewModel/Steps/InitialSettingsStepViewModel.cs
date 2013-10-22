using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Physics;
using GalaSoft.MvvmLight;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class InitialSettingsStepViewModel:WizardStepViewModelBase
    {
        public InitialSettingsStepViewModel(SolidMechanicsModel model)
            : base("Initial Settings", model)
        {
        }


        /// <summary>
        /// The <see cref="CurrentEdge" /> property's name.
        /// </summary>
        public const string MaxAmplitudePropertyName = "MaxAmplitude";

        /// <summary>
        /// Gets the CurrentEdge property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double MaxAmplitude
        {
            get
            {
                return solidMechanicsModel.MaxAmplitude;
            }

            set
            {
                if (solidMechanicsModel.MaxAmplitude == value)
                {
                    return;
                }

                solidMechanicsModel.MaxAmplitude = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(MaxAmplitudePropertyName);
            }
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            MaxAmplitude = model.MaxAmplitude;
        }
    }

    
}
