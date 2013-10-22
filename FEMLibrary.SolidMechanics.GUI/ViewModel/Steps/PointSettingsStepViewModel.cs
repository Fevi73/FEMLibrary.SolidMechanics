using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Physics;
using GalaSoft.MvvmLight;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class PointSettingsStepViewModel:WizardStepViewModelBase
    {
        public PointSettingsStepViewModel(SolidMechanicsModel model)
            : base("Point Settings", model)
        {
            SetPointConditions(solidMechanicsModel);
            currentPoint = Points[0];
        }

        private void SetPointConditions(SolidMechanicsModel model)
        {
            Points = new ObservableCollection<PointViewModel>();
            int i = 1;
            foreach (Point point in model.Model.Shape.Points)
            {
                Points.Add(new PointViewModel(i, point, model.Model.PointConditions[point]));
                i++;
            }
            
        }

        public ObservableCollection<PointViewModel> Points { get; private set; }

        /// <summary>
        /// The <see cref="CurrentEdge" /> property's name.
        /// </summary>
        public const string CurrentPointPropertyName = "CurrentPoint";

        private PointViewModel currentPoint;

        /// <summary>
        /// Gets the CurrentEdge property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public PointViewModel CurrentPoint
        {
            get
            {
                return currentPoint;
            }

            set
            {
                if (currentPoint == value)
                {
                    return;
                }

                currentPoint = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(CurrentPointPropertyName);
            }
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            solidMechanicsModel.Model.Shape.Points.Clear();
            solidMechanicsModel.Model.PointConditions.Clear();
            foreach (Point point in model.Model.Shape.Points) 
            {
                solidMechanicsModel.Model.Shape.Points.Add(point);
                solidMechanicsModel.Model.PointConditions.Add(point, model.Model.PointConditions[point]);
            }

            SetPointConditions(solidMechanicsModel);
            CurrentPoint = Points[0];
        }
    }

    
}
