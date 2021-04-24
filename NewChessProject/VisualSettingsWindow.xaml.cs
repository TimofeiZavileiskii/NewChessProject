using System;
using System.Collections.Generic;
using System.Windows;


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

        private void WindowClosed(object sender, EventArgs e)
        {
            visualSettings.WriteVisualSettings();
        }
    }
}
