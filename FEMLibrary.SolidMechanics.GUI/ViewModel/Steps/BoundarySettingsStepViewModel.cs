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
            SetBoundaryConditions(solidMechanicsModel);
            currentEdge = Edges[0];
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
            
        }

        public ObservableCollection<EdgeViewModel> Edges { get; private set; }

        /// <summary>
        /// The <see cref="CurrentEdge" /> property's name.
        /// </summary>
        public const string CurrentEdgePropertyName = "CurrentEdge";

        private EdgeViewModel currentEdge;

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
                return currentEdge;
            }

            set
            {
                if (currentEdge == value)
                {
                    return;
                }

                currentEdge = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(CurrentEdgePropertyName);
            }
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            solidMechanicsModel.Model.Shape.Edges.Clear();
            solidMechanicsModel.Model.BoundaryConditions.Clear();
            foreach (Edge edge in model.Model.Shape.Edges)
            {
                solidMechanicsModel.Model.Shape.Edges.Add(edge);
                solidMechanicsModel.Model.BoundaryConditions.Add(edge, model.Model.BoundaryConditions[edge]);
            }
            SetBoundaryConditions(model);
            CurrentEdge = Edges[0];
        }
    }

    
}
