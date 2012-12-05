using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Physics;
using GalaSoft.MvvmLight;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class BoundarySettingsStepViewModel:WizardStepViewModelBase
    {
        public BoundarySettingsStepViewModel(SolidMechanicsModel model):base("Boundary Settings", model)
        {
            SetBoundaryConditions(_solidMechanicsModel);
        }

        public void SetBoundaryConditions(SolidMechanicsModel model) 
        {
            Edges = new ObservableCollection<EdgeViewModel>();
            int i = 1;
            foreach (Edge edge in model.Model.Shape.Edges)
            {
                Edges.Add(new EdgeViewModel(i, edge, model.Model.BoundaryConditions[edge]));
                i++;
            }
            CurrentEdge = Edges[0];
        }

        public ObservableCollection<EdgeViewModel> Edges { get; private set; }

        /// <summary>
        /// The <see cref="CurrentEdge" /> property's name.
        /// </summary>
        public const string CurrentEdgePropertyName = "CurrentEdge";

        private EdgeViewModel _currentEdge;

        /// <summary>
        /// Gets the CurrentEdge property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public EdgeViewModel CurrentEdge
        {
            get
            {
                return _currentEdge;
            }

            set
            {
                if (_currentEdge == value)
                {
                    return;
                }

                _currentEdge = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(CurrentEdgePropertyName);
            }
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            _solidMechanicsModel.Model.Shape.Edges.Clear();
            _solidMechanicsModel.Model.BoundaryConditions.Clear();
            foreach (Edge edge in model.Model.Shape.Edges)
            {
                _solidMechanicsModel.Model.Shape.Edges.Add(edge);
                _solidMechanicsModel.Model.BoundaryConditions.Add(edge, model.Model.BoundaryConditions[edge]);
            }
            SetBoundaryConditions(model);
        }
    }

    public class EdgeViewModel : ViewModelBase
    {
        public int Index { get; private set; }
        public Edge Edge { get; private set; }
        public BoundaryCondition Condition { get; private set; }

        public EdgeViewModel(int index, Edge edge, BoundaryCondition condition)
        {
            Index = index;
            Edge = edge;
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
