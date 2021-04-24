using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace NewChessProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameCreator gc;
        GUIBoard guiBoard;
        Game game;
        
        VisualSettingsWindow settingsWindow;
        VisualSettings visualSettings;

        public MainWindow()
        {
            InitializeComponent();

            InGameInterface.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            ImportButton.Visibility = Visibility.Visible;


            AdditionalSettings additionalSettings = new AdditionalSettings(AdditionalSettings);
            visualSettings = new VisualSettings();
            settingsWindow = new VisualSettingsWindow(visualSettings);

            guiBoard = new GUIBoard(Screen, PieceSelection, this, visualSettings);
            visualSettings.SettingsUpdated += guiBoard.RedrawBoard;

            gc = new GameCreator(guiBoard, this, additionalSettings, InGameInterface);
            game = gc.GetGame();
            DataContext = gc;
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            gc.StartGame();
            SetGameScreen();
        }

        public void SetGameScreen()
        {
            InGameInterface.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Hidden;
            ImportButton.Visibility = Visibility.Hidden;
            ImportTextBox.Visibility = Visibility.Hidden;
            ImportLabel.Visibility = Visibility.Hidden;
        }

        public void SetSettingsMenu()
        {
            InGameInterface.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            ImportButton.Visibility = Visibility.Visible;
            ImportTextBox.Visibility = Visibility.Visible;
            ImportLabel.Visibility = Visibility.Visible;
        }

        private void ImportFen(object sender, RoutedEventArgs e)
        {
            gc.ImportFENString();
        }

        private void Click_Settings(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            gc.CloseGame();
            settingsWindow.Close();
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            if (!settingsWindow.Activate())
            {
                settingsWindow = new VisualSettingsWindow(visualSettings);
                settingsWindow.Show();
            }
        }
    }
}
