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
        protected SolidMechanicsModel _solidMechanicsModel;

        private string _displayName;
        
        public string DisplayName
        {
            get
            {
                return _displayName;
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

        private bool _isCurrent = false;

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
                return _isCurrent;
            }

            set
            {
                if (_isCurrent == value)
                {
                    return;
                }

                _isCurrent = value;

                
                // Update bindings, no broadcast
                RaisePropertyChanged(IsCurrentPropertyName);
            }
        }

        protected WizardStepViewModelBase(string displayName, SolidMechanicsModel model)
        {
            _displayName = displayName;
            _solidMechanicsModel = model;
            Figures = new ObservableCollection<Shape>();
        }

        public abstract void RefreshProperties(SolidMechanicsModel model);
    }
}
