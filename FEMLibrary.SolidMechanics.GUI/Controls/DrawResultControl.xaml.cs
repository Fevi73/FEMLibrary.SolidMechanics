using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FEMLibrary.SolidMechanics.GUI.Controls
{
    /// <summary>
    /// Interaction logic for DrawResultControl.xaml
    /// </summary>
    public partial class DrawResultControl : UserControl
    {
        public DrawResultControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string LinearData
        {
            get { return (string) GetValue(LinearDataProperty); }
            set { SetValue(LinearDataProperty, value); }
        }

        public static readonly DependencyProperty LinearDataProperty = DependencyProperty.Register("LinearData",
                                                                                                   typeof (string),
                                                                                                   typeof (DrawResultControl),
                                                                                                   new PropertyMetadata(string.Empty));

        public string NonLinearData
        {
            get { return (string)GetValue(NonLinearDataProperty); }
            set { SetValue(NonLinearDataProperty, value); }
        }

        public static readonly DependencyProperty NonLinearDataProperty = DependencyProperty.Register("NonLinearData",
                                                                                                   typeof(string),
                                                                                                   typeof(DrawResultControl),
                                                                                                   new PropertyMetadata(string.Empty));

        public string AxesData
        {
            get { return (string)GetValue(AxesDataProperty); }
            set { SetValue(AxesDataProperty, value); }
        }

        public static readonly DependencyProperty AxesDataProperty = DependencyProperty.Register("AxesData",
                                                                                                   typeof(string),
                                                                                                   typeof(DrawResultControl),
                                                                                                   new PropertyMetadata(string.Empty));


        public static void ShowDrawResult(string linear, string nonlinear, string axes)
        {
            DrawResultControl drawResultControl = new DrawResultControl();
            drawResultControl.LinearData = linear;
            drawResultControl.NonLinearData = nonlinear;//"M0,0 L100,0 L100,10 L0,10 Z";
            drawResultControl.AxesData = axes;

            Window window = new Window();
            window.Content = drawResultControl;
            window.Show();
        }
    }
}
