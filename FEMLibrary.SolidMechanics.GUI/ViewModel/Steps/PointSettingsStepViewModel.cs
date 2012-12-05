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
            SetPointConditions(_solidMechanicsModel);
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
            CurrentPoint = Points[0];
        }

        public ObservableCollection<PointViewModel> Points { get; private set; }

        /// <summary>
        /// The <see cref="CurrentEdge" /> property's name.
        /// </summary>
        public const string CurrentPointPropertyName = "CurrentPoint";

        private PointViewModel _currentPoint;

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
                return _currentPoint;
            }

            set
            {
                if (_currentPoint == value)
                {
                    return;
                }

                _currentPoint = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(CurrentPointPropertyName);
            }
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            _solidMechanicsModel.Model.Shape.Points.Clear();
            _solidMechanicsModel.Model.PointConditions.Clear();
            foreach (Point point in model.Model.Shape.Points) {
                _solidMechanicsModel.Model.Shape.Points.Add(point);
                _solidMechanicsModel.Model.PointConditions.Add(point, model.Model.PointConditions[point]);
            }

            SetPointConditions(_solidMechanicsModel);
        }
    }

    public class PointViewModel : ViewModelBase
    {
        public int Index { get; private set; }
        public Point Point { get; private set; }
        public BoundaryCondition Condition { get; private set; }

        public PointViewModel(int index, Point point, BoundaryCondition condition)
        {
            Index = index;
            Point = point;
            Condition = condition;
            Types = new ObservableCollection<BoundaryConditionsType>(getTypesOfBoundaryConditions());
        }

        public ObservableCollection<BoundaryConditionsType> Types { get; private set; }

        private IEnumerable<BoundaryConditionsType> getTypesOfBoundaryConditions()
        {
            List<BoundaryConditionsType> typesEnumarable = new List<BoundaryConditionsType>();
            Array typesArray = Enum.GetValues(typeof (BoundaryConditionsType));
            foreach(var type in typesArray)
            {
                typesEnumarable.Add((BoundaryConditionsType)type);
            }
            return typesEnumarable;
        }

        /// <summary>
        /// The <see cref="BoundaryConditionType" /> property's name.
        /// </summary>
        public const string BoundaryConditionTypePropertyName = "BoundaryConditionType";

        /// <summary>
        /// Gets the BoundaryConditionType property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public BoundaryConditionsType BoundaryConditionType
        {
            get
            {
                return Condition.Type;
            }

            set
            {
                if (Condition.Type == value)
                {
                    return;
                }

                Condition.Type = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(BoundaryConditionTypePropertyName);

            }
        }

        /// <summary>
        /// The <see cref="LoadUp" /> property's name.
        /// </summary>
        public const string LoadUpPropertyName = "LoadUp";

        /// <summary>
        /// Gets the LoadDown property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double LoadUp
        {
            get
            {
                return Condition.Value[2];
            }

            set
            {
                if (Condition.Value[2] == value)
                {
                    return;
                }

                Condition.Value[2] = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(LoadUpPropertyName);

            }
        }

        /// <summary>
        /// The <see cref="LoadRight" /> property's name.
        /// </summary>
        public const string LoadRightPropertyName = "LoadRight";


        /// <summary>
        /// Gets the LoadRight property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double LoadRight
        {
            get
            {
                return Condition.Value[0];
            }

            set
            {
                if (Condition.Value[0] == value)
                {
                    return;
                }

                Condition.Value[0] = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(LoadRightPropertyName);
            }
        }

    }
}
