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
    public class SetupViewModel : ViewModelBase
    {
        private const string FILENAME = "tempmodel.dat";
        private SolidMechanicsModel model;

        private bool isRect = false;

        public ObservableCollection<WizardStepViewModelBase> Steps { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SetupViewModel()
        {

            if (isRect)
            {
                SolidMechanicsModel2D m = new SolidMechanicsModel2D(new Rectangle(0, 0));
                Steps = getSteps2D(m);
                model = m;
            }
            else
            {
                model = new SolidMechanicsModel(new CylindricalPlate(0, 0));
                Steps = getSteps(model);
            }
            
            activeStep = Steps[0];
            activeStep.IsCurrent = true;

            MoveNextCommand = new RelayCommand(MoveNext, CanMoveNext);
            MovePreviousCommand = new RelayCommand(MovePrevious, CanMovePrevious);
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand(Load);
        }

        private ObservableCollection<WizardStepViewModelBase> getSteps(SolidMechanicsModel m)
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

        private ObservableCollection<WizardStepViewModelBase> getSteps2D(SolidMechanicsModel2D m)
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

        /// <summary>
        /// The <see cref="ActiveStep" /> property's name.
        /// </summary>
        public const string ActiveStepPropertyName = "ActiveStep";

        private WizardStepViewModelBase activeStep = null;

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public WizardStepViewModelBase ActiveStep
        {
            get
            {
                return activeStep;
            }

            set
            {
                if (activeStep == value)
                {
                    return;
                }

                activeStep = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ActiveStepPropertyName);
            }
        }

        public void SelectStep(WizardStepViewModelBase step)
        {
            step.IsCurrent = true;
            ActiveStep = step;
        }


        public RelayCommand MoveNextCommand { get; private set; }

        public void MoveNext()
        {
            int indexNextActiveStep = Steps.IndexOf(ActiveStep) + 1;
            ActiveStep.IsCurrent = false;
            SelectStep(Steps[indexNextActiveStep]);
        }

        public bool CanMoveNext()
        {
            int indexNextActiveStep = Steps.IndexOf(ActiveStep) + 1;
            return indexNextActiveStep < Steps.Count;
        }

        public RelayCommand MovePreviousCommand { get; private set; }

        public void MovePrevious()
        {
            int indexPreviousActiveStep = Steps.IndexOf(ActiveStep) - 1;
            ActiveStep.IsCurrent = false;
            SelectStep(Steps[indexPreviousActiveStep]);
        }

        public bool CanMovePrevious()
        {
            int indexPreviousActiveStep = Steps.IndexOf(ActiveStep) - 1;
            return indexPreviousActiveStep > -1;
        }

        public RelayCommand SaveCommand { get; private set; }

        public void Save()
        {
            model.Save(FILENAME);
        }

        public RelayCommand LoadCommand { get; private set; }

        public void Load()
        {
            SolidMechanicsModel model = SolidMechanicsModel.Load(FILENAME);
            foreach (WizardStepViewModelBase step in Steps) 
            {
                step.RefreshProperties(model);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}