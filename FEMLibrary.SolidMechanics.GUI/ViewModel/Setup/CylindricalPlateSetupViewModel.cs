using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using FEMLibrary.SolidMechanics.GUI.Models;
using FEMLibrary.SolidMechanics.GUI.ViewModel.Steps;
using FEMLibrary.SolidMechanics.Geometry;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel
{
    public class CylindricalPlateSetupViewModel : SetupViewModel
    {
        public static readonly string CylindricalPlateSetupTitle = "Cylindrical plate solver";

        public CylindricalPlateSetupViewModel()
            : base(CylindricalPlateSetupTitle)
        { }

        protected override SolidMechanicsModel createModel()
        {
            return new SolidMechanicsModel(new CylindricalPlate());
        }

        protected override ObservableCollection<WizardStepViewModelBase> getSteps(SolidMechanicsModel m)
        {
            ObservableCollection<WizardStepViewModelBase> steps = new ObservableCollection<WizardStepViewModelBase>();

            steps.Add(new CylindricalShapeStepViewModel(m));
            steps.Add(new MaterialStepViewModel(m));
            steps.Add(new PointSettingsStepViewModel(m));
            steps.Add(new BoundarySettingsStepViewModel(m));
            steps.Add(new InitialSettingsStepViewModel(m));
            steps.Add(new MeshSettingsStepViewModel(m));
            steps.Add(new SolveStepViewModel(m));

            return steps;
        }

        protected override string getFileExtensionForModel()
        {
            return ".cpm";
        }

        protected override string getFileFilterForModel()
        {
            return "Cylindrical plate models (.cpm)|*.cpm";
        }
    }
}