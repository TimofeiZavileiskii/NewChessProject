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
        public MainWindow()
        {
            InitializeComponent();

            InGameInterface.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            ImportButton.Visibility = Visibility.Visible;

            guiBoard = new GUIBoard(Screen, PieceSelection, this);
            gc = new GameCreator(guiBoard, InGameInterface);
            DataContext = gc;
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            gc.StartGame();
            InGameInterface.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Hidden;
            ImportButton.Visibility = Visibility.Hidden;
        }

        private void ImportFen(object sender, RoutedEventArgs e)
        {
            gc.ImportFENString();
        }

        private void Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void Screen_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Click_Settings(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        
        }

        private void Board_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void WindowClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
