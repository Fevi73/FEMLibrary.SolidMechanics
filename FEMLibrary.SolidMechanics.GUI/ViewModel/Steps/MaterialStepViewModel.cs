using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FEMLibrary.SolidMechanics.GUI.Models;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel.Steps
{
    public class MaterialStepViewModel:WizardStepViewModelBase
    {
        public MaterialStepViewModel(SolidMechanicsModel model):base("Material", model)
        {
        }

        /// <summary>
        /// The <see cref="Rho" /> property's name.
        /// </summary>
        public const string RhoPropertyName = "Rho";

        /// <summary>
        /// Gets the Rho property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double Rho
        {
            get
            {
                return solidMechanicsModel.Model.Material.Rho;
            }

            set
            {
                if (solidMechanicsModel.Model.Material.Rho == value)
                {
                    return;
                }

                solidMechanicsModel.Model.Material.Rho = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(RhoPropertyName);

            }
        }

        /// <summary>
        /// The <see cref="PoissonCoef" /> property's name.
        /// </summary>
        public const string PoissonCoefPropertyName = "PoissonCoef";

        /// <summary>
        /// Gets the PoissonCoef property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double PoissonCoef
        {
            get
            {
                return solidMechanicsModel.Model.Material.v[0,0];
            }

            set
            {
                if (solidMechanicsModel.Model.Material.v[0, 0] == value)
                {
                    return;
                }

                solidMechanicsModel.Model.Material.AutoFillV(value);
                solidMechanicsModel.Model.Material.AutoFillG();

                // Update bindings, no broadcast
                RaisePropertyChanged(PoissonCoefPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="YoungModulus" /> property's name.
        /// </summary>
        public const string YoungModulusPropertyName = "YoungModulus";

        /// <summary>
        /// Gets the YoungModulus property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public double YoungModulus
        {
            get 
            { 
                return solidMechanicsModel.Model.Material.E[0]; 
            }

            set
            {
                if (solidMechanicsModel.Model.Material.E[0] == value)
                {
                    return;
                }

                solidMechanicsModel.Model.Material.AutoiFillE(value);
                solidMechanicsModel.Model.Material.AutoFillG();

                // Update bindings, no broadcast
                RaisePropertyChanged(YoungModulusPropertyName);

            }
        }

        public override void RefreshProperties(SolidMechanicsModel model)
        {
            Rho = model.Model.Material.Rho;
            PoissonCoef = model.Model.Material.v[0, 0];
            YoungModulus = model.Model.Material.E[0];
        }
    }
}
