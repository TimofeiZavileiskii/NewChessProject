using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewChessProject
{

    class GUIBoard
    {
        const double proportionOfMoveIndicator = 0.5;


        Canvas canvas;
        WrapPanel pieceSelection;

        BitmapImage[,] pieceTiles;
        GameRepresentation gr;
        VisualSettings visualSettings;

        public event EventHandler<BoardClickedEventArgs> OnBoardClicked;
        public event EventHandler OnWindowClicked;
        public event EventHandler<PieceSelectedEventArgs> PieceSelected;

        public event EventHandler Resign;
        public event EventHandler TakeBack;
        public event EventHandler RequestDraw;

        double squareWidth;
        double squareHeight;
        bool flipBoard;

        public bool FlipBoard
        {
            set
            {
                flipBoard = value;
            }
        }

        public GameRepresentation GameData
        {
            get
            {
                return gr;
            }
        }

        public void SetNewUser(EventHandler<BoardClickedEventArgs> boardClicked, EventHandler windowClicked, EventHandler<PieceSelectedEventArgs> pieceSelected,
            EventHandler resign, EventHandler takeBack, EventHandler requestDraw)
        {
            OnBoardClicked = null;
            OnWindowClicked = null;
            PieceSelected = null;
            Resign = null;
            TakeBack = null;
            RequestDraw = null;

            OnBoardClicked += boardClicked;
            OnWindowClicked += windowClicked;
            PieceSelected += pieceSelected;

            Resign += resign;
            TakeBack += takeBack;
            RequestDraw += requestDraw;
        }

        public GUIBoard(Canvas canvas, WrapPanel pieceSelection, MainWindow window)
        {
            this.canvas = canvas;
            this.pieceSelection = pieceSelection;
            window.MouseLeftButtonDown += WindowClicked;
            canvas.SizeChanged += ChangeDimensions;
            canvas.MouseLeftButtonDown += Clicked;
            flipBoard = false;

            GetPieceTiles();
            Resize();
            AddFunctionToButtonEvents();
        }

        private void AddFunctionToButtonEvents()
        {
            foreach (object obj in pieceSelection.Children)
            {
                ((Button)obj).Click += InputPawnSelection;
            }
        }

        private void GetPieceTiles()
        {
            pieceTiles = new BitmapImage[Enum.GetValues(typeof(PlayerColour)).Length, Enum.GetValues(typeof(PieceType)).Length];

            foreach (int colour in Enum.GetValues(typeof(PlayerColour)))
            {
                foreach (int type in Enum.GetValues(typeof(PieceType)))
                {
                    string path = @"\Resources\" + Enum.GetName(typeof(PlayerColour), colour).Substring(0, 1)
                         + Enum.GetName(typeof(PieceType), type) + ".png";
                    BitmapImage image = new BitmapImage(
                        new Uri($@"{ Environment.CurrentDirectory }{ path }"));
                    Console.WriteLine(path);
                    pieceTiles[colour, type] = image;
                }
            }
        }

        private void Resize()
        {
            squareWidth = canvas.ActualWidth / Board.boardWidth;
            squareHeight = canvas.ActualHeight / Board.boardHeight;

            if (squareHeight < squareWidth)
                squareWidth = squareHeight;
            else
                squareHeight = squareWidth;
        }

        private Color GetHilightColor(HilightType type)
        {
            Color output = Color.FromRgb(0, 0, 0);
            switch(type)
            {
                case HilightType.Check:
                    output = visualSettings.CheckHilight;
                    break;
                case HilightType.SelectedPiece:
                    output = visualSettings.SelectedPieceHilight;
                    break;
                case HilightType.SafeMove:
                    output = visualSettings.MoveHilight;
                    break;
                case HilightType.DefendedMove:
                    output = visualSettings.DefendedHilight;
                    break;
                case HilightType.ThreatenedMove:
                    output = visualSettings.AttackedHilight;
                    break;
                  
            }

            return output;

        }

        private void DrawGame()
        {
            canvas.Children.Clear();
            DrawBoard();
            foreach (BoardIndicator highlight in gr.PieceHilights)
            {
                DrawSquare(highlight.Position.X, GetYCoordinate(highlight.Position.Y), GetHilightColor(highlight.Type));
            }
            foreach (PieceRepresentation piece in gr.Pieces)
            {
                DrawPiece(piece);
            }
            foreach (BoardIndicator move in gr.Moves)
            {
                DrawPosition(move.Position.X, GetYCoordinate(move.Position.Y), GetHilightColor(move.Type));
            }
        }

        private void DrawBoard()
        {
            for (int i = 0; i < Board.boardWidth; i++)
            {
                for (int ii = 0; ii < Board.boardHeight; ii++)
                {
                    Color colour = Color.FromRgb(90, 100, 90);
                    if (((i + Board.boardHeight * ii + ii) % 2) != 0)
                    {
                        colour = Color.FromRgb(180, 200, 180);
                    }
                    DrawSquare(i, GetYCoordinate(ii), colour);
                }
            }
        }

        private int GetYCoordinate(int y)
        {
            int output;
            if (!flipBoard)
            {
                output = Board.boardHeight - y - 1;
            }
            else
            {
                output = y;
            }
            return output;
        }

        private void DrawPosition(int x, int y, Color colour)
        {
            Ellipse positionImage = new Ellipse
            {
                Width = squareWidth * proportionOfMoveIndicator,
                Height = squareHeight * proportionOfMoveIndicator,
                Fill = new SolidColorBrush(colour),
            };
            Canvas.SetLeft(positionImage, x * squareWidth + 0.25 * squareWidth);
            Canvas.SetTop(positionImage, y * squareHeight + 0.25 * squareHeight);

            canvas.Children.Add(positionImage);
        }

        private void DrawPiece(PieceRepresentation piece)
        {
            Image pieceImage = new Image
            {
                Source = pieceTiles[(int)piece.Colour, (int)piece.Type],
                Width = squareWidth,
                Height = squareHeight,
                IsHitTestVisible = false,
            };
            Canvas.SetLeft(pieceImage, piece.Position.X * squareWidth);
            Canvas.SetTop(pieceImage, GetYCoordinate(piece.Position.Y) * squareHeight);

            canvas.Children.Add(pieceImage);
        }

        public void RecieveDrawRequest(object sender, RoutedEventArgs e)
        {
            RequestDraw?.Invoke(this, EventArgs.Empty);
        }
        public void RecieveSurrenderRequest(object sender, RoutedEventArgs e)
        {
            Resign?.Invoke(this, EventArgs.Empty);
        }
        public void RecieveTakeBackRequest(object sender, RoutedEventArgs e)
        {
            TakeBack?.Invoke(this, EventArgs.Empty);
        }

        private void DrawSquare(double x, double y, Color colour)
        {
            Rectangle rectangle = new Rectangle
            {
                Height = squareHeight,
                Width = squareWidth,
                Fill = new SolidColorBrush(colour),
                IsHitTestVisible = false,
            };
            Canvas.SetLeft(rectangle, x * squareWidth);
            Canvas.SetTop(rectangle, y * squareHeight);

            canvas.Children.Add(rectangle);
        }

        public void ChangeDimensions(object sender, SizeChangedEventArgs e)
        {
            Resize();
            DrawGame();
        }

        protected virtual void BoardClicked(Vector position)
        {
            if (OnBoardClicked != null)
                OnBoardClicked(this, new BoardClickedEventArgs(position));
        }

        public void Update(GameRepresentation gr)
        {
            this.gr = gr;
            DrawGame();
        }

        public void Update()
        {
            DrawGame();
        }


        public void Clicked(object sender, MouseButtonEventArgs e)
        {
            int x = (int)Math.Floor((double)e.GetPosition(canvas).X / squareWidth);
            int y = (int)Math.Floor((double)e.GetPosition(canvas).Y / squareHeight);

            if (x > 7)
                x = 7;
            else if (x < 0)
                x = 0;

            if (y > 7)
                y = 7;
            else if (y < 0)
                y = 0;

            BoardClicked(new Vector(x, GetYCoordinate(y)));
        }

        public void WindowClicked(object sender, EventArgs e)
        {
            if (OnWindowClicked != null)
            {
                //  OnWindowClicked(this, EventArgs.Empty);
            }
        }

        public void InputPawnSelection(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            PieceType selectedPice;
            switch (button.Name)
            {
                case "QueenChoice":
                    selectedPice = PieceType.Queen;
                    break;
                case "RookChoice":
                    selectedPice = PieceType.Rook;
                    break;
                case "BishopChoice":
                    selectedPice = PieceType.Bishop;
                    break;
                case "KnightChoice":
                    selectedPice = PieceType.Knight;
                    break;
                default:
                    throw new ArgumentException("Wrong piece type chosen");
            }

            HidePieceSelection();

            PieceSelected?.Invoke(this, new PieceSelectedEventArgs(selectedPice));
        }

        private void HidePieceSelection()
        {
            pieceSelection.Visibility = Visibility.Hidden;
        }

        public void ShowPieceSelection(PlayerColour colour)
        {
            pieceSelection.Visibility = Visibility.Visible;


            PlacePictureInButton((Button)pieceSelection.Children[0], pieceTiles[(int)colour, (int)PieceType.Queen]);
            PlacePictureInButton((Button)pieceSelection.Children[1], pieceTiles[(int)colour, (int)PieceType.Rook]);
            PlacePictureInButton((Button)pieceSelection.Children[2], pieceTiles[(int)colour, (int)PieceType.Bishop]);
            PlacePictureInButton((Button)pieceSelection.Children[3], pieceTiles[(int)colour, (int)PieceType.Knight]);
        }

        private void PlacePictureInButton(Button button, BitmapImage image)
        {
            StackPanel stackPnl = new StackPanel();
            stackPnl.Orientation = Orientation.Horizontal;
            stackPnl.Margin = new Thickness(1);
            Image img = new Image();
            img.Source = image;
            stackPnl.Children.Add(img);
            button.Content = stackPnl;
        }

        public void EndGame(object sender, GameEndedEventArgs e)
        {
            try
            {
                string text = "";
                string drawMessage = "The game ended in a draw:";
                string victoryMessage = "The game ended in a " + e.Winner.ToString() + " player's victory";
                switch (e.Reason)
                {
                    case MoveResult.Stalemate:
                        text = drawMessage + " stalemate";
                        break;
                    case MoveResult.Mate:
                        text = victoryMessage + " by checkmate";
                        break;
                    case MoveResult.Resignation:
                        text = victoryMessage + ", as other player resigned";
                        break;
                    case MoveResult.MoveRepetition:
                        text = drawMessage + " 3 position repetition";
                        break;
                    case MoveResult.Draw:
                        text = drawMessage + " agreed draw";
                        break;
                    case MoveResult.TimeOut:
                        text = victoryMessage + ", as other player run out of time";
                        break;
                    case MoveResult.move50Rule:
                        text = drawMessage + " 50 moves were made without pawn advancing or piece taken";
                        break;
                    case MoveResult.InsufficientMaterial:
                        text = drawMessage + " insufficient material for a checkmate";
                        break;
                }

                ShowMessage(text);
            }
            catch (System.InvalidCastException)
            {

            }
        }

        public void MakeRequest(Request request)
        {
            request.Agreed = MessageBox.Show(request.Text, request.Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private void ShowMessage(string text)
        {
            MessageBox.Show(text);
        }

    }
}
