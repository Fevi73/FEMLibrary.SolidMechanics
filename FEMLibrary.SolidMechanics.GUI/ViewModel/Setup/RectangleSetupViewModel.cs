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
        public static readonly string RectangleSetupTitle = "Rectangle plate solver";

        public RectangleSetupViewModel() :base(RectangleSetupTitle)
        { }

        protected override SolidMechanicsModel createModel()
        {
            return new SolidMechanicsModel2D();
        }

        protected override ObservableCollection<WizardStepViewModelBase> getSteps(SolidMechanicsModel m)
        {
            ObservableCollection<WizardStepViewModelBase> steps = new ObservableCollection<WizardStepViewModelBase>();

            steps.Add(new ShapeStepViewModel(model));
            steps.Add(new MaterialStepViewModel(m));
            steps.Add(new PointSettingsStepViewModel(m));
            steps.Add(new BoundarySettingsStepViewModel(m));
            steps.Add(new InitialSettingsStepViewModel(m));
            steps.Add(new RectangleMeshSettingsStepViewModel(model));
            steps.Add(new Solve2DStepViewModel(m));

            return steps;
        }

        protected override string getFileExtensionForModel()
        {
            return ".rpm";
        }

        protected override string getFileFilterForModel()
        {
            return "Rectangular plate models (.rpm)|*.rpm";
        }
    }
}