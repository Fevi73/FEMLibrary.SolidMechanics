using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary.SolidMechanics.GUI.ViewModel
{
    public class SelectorViewModel : ViewModelBase
    {
        public SelectorViewModel() 
        {
            ShowCylindricalPlateSolverCommand = new RelayCommand(ShowCylindricalPlateSolver);
            ShowRectanglePlateSolverCommand = new RelayCommand(ShowRectanglePlateSolver);
        }
        private void showMainWindow(SetupViewModel viewModel) 
        {
            MainWindow window = new MainWindow();
            window.DataContext = viewModel;
            window.Show();
        }

        public RelayCommand ShowCylindricalPlateSolverCommand { get; private set; }

        public void ShowCylindricalPlateSolver()
        {
            showMainWindow(new CylindricalPlateSetupViewModel());
        }

        public RelayCommand ShowRectanglePlateSolverCommand { get; private set; }

        public void ShowRectanglePlateSolver()
        {
            showMainWindow(new RectangleSetupViewModel());
        }
    }
}
