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
    public class RectangleSetupViewModel : SetupViewModel
    {
        protected abstract SolidMechanicsModel createModel();

        protected override ObservableCollection<WizardStepViewModelBase> getSteps(SolidMechanicsModel m)
        {
            ObservableCollection<WizardStepViewModelBase> steps = new ObservableCollection<WizardStepViewModelBase>();

            //steps.Add(new ShapeStepViewModel(model));
            steps.Add(new CylindricalShapeStepViewModel(m));
            steps.Add(new MaterialStepViewModel(m));
            steps.Add(new PointSettingsStepViewModel(m));
            steps.Add(new BoundarySettingsStepViewModel(m));
            steps.Add(new InitialSettingsStepViewModel(m));
            //steps.Add(new RectangleMeshSettingsStepViewModel(model));
            steps.Add(new MeshSettingsStepViewModel(m));
            steps.Add(new SolveStepViewModel(m));

            return steps;
        }

        private ObservableCollection<WizardStepViewModelBase> getSteps(SolidMechanicsModel m)
        {
            ObservableCollection<WizardStepViewModelBase> steps = new ObservableCollection<WizardStepViewModelBase>();

            steps.Add(new ShapeStepViewModel(m));
            steps.Add(new MaterialStepViewModel(m));
            steps.Add(new PointSettingsStepViewModel(m));
            steps.Add(new BoundarySettingsStepViewModel(m));
            steps.Add(new InitialSettingsStepViewModel(m));
            steps.Add(new RectangleMeshSettingsStepViewModel(m));
            steps.Add(new Solve2DStepViewModel(m));

            return steps;
        }

    }
}