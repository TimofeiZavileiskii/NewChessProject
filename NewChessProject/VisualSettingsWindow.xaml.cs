using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewChessProject
{
    /// <summary>
    /// Interaction logic for VisualSettingsWindow.xaml
    /// </summary>
    partial class VisualSettingsWindow : Window
    {
        VisualSettings visualSettings;

        public VisualSettingsWindow(VisualSettings vs)
        {
            InitializeComponent();

            visualSettings = vs;
            DataContext = vs;
        }
    }
}
