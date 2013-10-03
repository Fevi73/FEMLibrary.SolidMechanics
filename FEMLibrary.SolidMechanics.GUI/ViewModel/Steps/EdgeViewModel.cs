using FEMLibrary.SolidMechanics.Geometry;
using FEMLibrary.SolidMechanics.Physics;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
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
            Array typesArray = Enum.GetValues(typeof(BoundaryConditionsType));
            foreach (var type in typesArray)
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
