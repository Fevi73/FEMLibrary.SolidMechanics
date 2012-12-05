using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using FEMLibrary.SolidMechanics.Physics;

namespace FEMLibrary.SolidMechanics.GUI.Converters
{
    public class BoundaryConditionTypeToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility res = Visibility.Visible;
            if (value is BoundaryConditionsType)
            {
                if (((BoundaryConditionsType)value) == BoundaryConditionsType.Static)
                {
                    res = Visibility.Hidden;
                }
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
