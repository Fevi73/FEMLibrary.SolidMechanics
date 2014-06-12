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
    public abstract class SetupViewModel : ViewModelBase
    {
        private const string FILENAME = "tempmodel.dat";
        protected SolidMechanicsModel model;

        public ObservableCollection<WizardStepViewModelBase> Steps { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SetupViewModel(string title)
        {
            MoveNextCommand = new RelayCommand(MoveNext, CanMoveNext);
            MovePreviousCommand = new RelayCommand(MovePrevious, CanMovePrevious);
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand(Load);
            Title = title;

            InitializeSteps();
        }

        public void InitializeSteps() 
        {
            model = createModel();
            Steps = getSteps(model);
            SelectStep(Steps[0]);
            foreach (WizardStepViewModelBase step in Steps) {
                step.PropertyChanged += step_PropertyChanged;
            }
        }

        void step_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == WizardStepViewModelBase.IsCurrentPropertyName) 
            {
                ActiveStep.IsCurrent = false;
                ActiveStep = (WizardStepViewModelBase)sender;
            }
        }

        protected abstract SolidMechanicsModel createModel();
        protected abstract ObservableCollection<WizardStepViewModelBase> getSteps(SolidMechanicsModel m);

        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string title = null;


        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                if (title == value)
                {
                    return;
                }

                title = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(TitlePropertyName);
            }
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
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = "model";
            saveDialog.DefaultExt = getFileExtensionForModel();
            saveDialog.Filter = getFileFilterForModel();
            if (saveDialog.ShowDialog() == true)
            {
                model.Save(saveDialog.FileName);
            }
        }

        public RelayCommand LoadCommand { get; private set; }

        public void Load()
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.DefaultExt = getFileExtensionForModel();
            openDialog.Filter = getFileFilterForModel();
            if (openDialog.ShowDialog() == true)
            {
                SolidMechanicsModel model = SolidMechanicsModel.Load(openDialog.FileName);
                foreach (WizardStepViewModelBase step in Steps)
                {
                    step.RefreshProperties(model);
                }
            }
        }

        protected abstract string getFileExtensionForModel();
        protected abstract string getFileFilterForModel();

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}