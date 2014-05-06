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
        public RelayCommand ShowCylindricalPlateSolverCommand { get; private set; }

        public void ShowCylindricalPlateSolver()
        {
        }

        public RelayCommand ShowRectanglePlateSolverCommand { get; private set; }

        public void ShowRectanglePlateSolver()
        {
        }
    }
}
