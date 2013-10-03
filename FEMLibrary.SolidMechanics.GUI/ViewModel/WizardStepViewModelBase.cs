using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FEMLibrary.SolidMechanics.GUI.Models;
using GalaSoft.MvvmLight;
using FEMLibrary.SolidMechanics.Geometry;
using System.Collections.ObjectModel;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel
{
    public abstract class WizardStepViewModelBase: ViewModelBase
    {
        protected SolidMechanicsModel solidMechanicsModel;

        private string displayName;
        
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }

        public ObservableCollection<Shape> Figures
        {
            get;
            protected set;
        }

        /// <summary>
        /// The <see cref="IsCurrent" /> property's name.
        /// </summary>
        public const string IsCurrentPropertyName = "IsCurrent";

        private bool isCurrent = false;

        /// <summary>
        /// Gets the IsCurrent property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                return isCurrent;
            }

            set
            {
                if (isCurrent == value)
                {
                    return;
                }

                isCurrent = value;

                
                // Update bindings, no broadcast
                RaisePropertyChanged(IsCurrentPropertyName);
            }
        }

        protected WizardStepViewModelBase(string displayName, SolidMechanicsModel model)
        {
            this.displayName = displayName;
            solidMechanicsModel = model;
            Figures = new ObservableCollection<Shape>();
        }

        public abstract void RefreshProperties(SolidMechanicsModel model);
    }
}
