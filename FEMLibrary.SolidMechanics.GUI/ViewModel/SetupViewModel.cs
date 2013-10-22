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

        public ObservableCollection<WizardStepViewModelBase> Steps { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SetupViewModel()
        {
            model = new SolidMechanicsModel(new Rectangle(0, 0));
            Steps = getSteps();
            activeStep = Steps[0];
            activeStep.IsCurrent = true;

            MoveNextCommand = new RelayCommand(MoveNext, CanMoveNext);
            MovePreviousCommand = new RelayCommand(MovePrevious, CanMovePrevious);
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand(Load);
        }

        private ObservableCollection<WizardStepViewModelBase> getSteps()
        {
            ObservableCollection<WizardStepViewModelBase> steps = new ObservableCollection<WizardStepViewModelBase>();

            steps.Add(new ShapeStepViewModel(model));
            steps.Add(new MaterialStepViewModel(model));
            steps.Add(new PointSettingsStepViewModel(model));
            steps.Add(new BoundarySettingsStepViewModel(model));
            steps.Add(new InitialSettingsStepViewModel(model));
            steps.Add(new RectangleMeshSettingsStepViewModel(model));
            steps.Add(new SolveStepViewModel(model));

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