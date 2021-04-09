using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


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
        public MainWindow()
        {
            InitializeComponent();

            InGameInterface.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            ImportButton.Visibility = Visibility.Visible;

            guiBoard = new GUIBoard(Screen, PieceSelection, this);
            AdditionalSettings additionalSettings = new AdditionalSettings(AdditionalSettings);

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
            game.EndImmediatly();
        }
    }
}
