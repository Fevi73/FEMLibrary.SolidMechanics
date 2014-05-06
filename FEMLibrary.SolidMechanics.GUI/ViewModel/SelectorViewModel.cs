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
        private void showMainWindow(SetupViewModel viewModel)

        public RelayCommand ShowCylindricalPlateSolverCommand { get; private set; }

        public void ShowCylindricalPlateSolver()
        {
            MainWindow window = new MainWindow();
            window.DataContext = new CylindricalPlateSetupViewModel();
            window.Show();
        }

        public RelayCommand ShowRectanglePlateSolverCommand { get; private set; }

        public void ShowRectanglePlateSolver()
        {
            MainWindow window = new MainWindow();
            window.DataContext = new RectangleSetupViewModel();
            window.Show();
        }
    }
}
