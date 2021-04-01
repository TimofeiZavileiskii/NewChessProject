using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Input;

namespace NewChessProject
{
    class GameCreator
    {
        Board board;
        Player playerWhite;
        Player playerBlack;
        GUIBoard guiBoard;
        Game game;
        StackPanel inGameInterface;
        ComboBox[] playerTypeSelection;

        Dictionary<string, CreatePlayer> CreatePlayerFunctions;

        const string stockFishAddress = @"stockfish_13_win_x64_bmi2\stockfish_13_win_x64_bmi2";

        string playerWhiteType;
        string playerBlackType;
        double initialTime;
        double timePerTurn;
        bool touchRule;
        bool carryPieces;

        delegate Player CreatePlayer(PlayerColour colour);

        Player CreateGUIPlayer(PlayerColour colour)
        {
            GUIPlayer player = new GUIPlayer(colour, game, guiBoard);

            guiBoard.OnBoardClicked += player.OnBoardClicked;
            guiBoard.OnWindowClicked += player.OnWindowClicked;
            guiBoard.PieceSelected += player.PieceSelected;


            game.OnGameEnded += player.GameEnded;
            game.RequestMade += player.RequestSend;

            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[0]).Click += player.RequestTakeback;
            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[1]).Click += player.RequestDraw;
            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[2]).Click += player.Resign;

            return player;
        }

        Player CreateAIPlayer(PlayerColour colour)
        {
            AIPlayer player = new AIPlayer(colour, new ChessEngine(stockFishAddress, "StockFish"), game, 1);
            game.GameStarted += player.GameStarted;
            return player;
        }

        public double InitialTime
        {
            get
            {
                return initialTime;
            }
            set
            {
                initialTime = Convert.ToDouble(value);
            }
        }
        public double TimePerTurn
        {
            get
            {
                return timePerTurn;
            }
            set
            {
                timePerTurn = Convert.ToDouble(value);
            }
        }
        public bool TouchRule
        {
            get
            {
                return touchRule;
            }
            set
            {
                touchRule = value;
            }
        }
        public bool CarryPieces
        {
            get
            {
                return carryPieces;
            }
            set
            {
                carryPieces = value;
            }
        }
        public string PlayerWhiteType
        {
            get
            {
                return playerWhiteType;
            }
            set
            {
                playerWhiteType = value;
            }
        }
        public string PlayerBlackType
        {
            get
            {
                return playerBlackType;
            }
            set
            {
                playerBlackType = value;
            }
        }

        public GameCreator(Game game, GUIBoard guiBoard, StackPanel inGameInterface, ComboBox whitePl, ComboBox blackPl)
        {
            Board.FillFENRepresentations();
            this.game = game;
            this.guiBoard = guiBoard; 
            this.inGameInterface = inGameInterface;

            playerTypeSelection = new ComboBox[Enum.GetValues(typeof(PlayerColour)).Length];
            playerTypeSelection[(int)PlayerColour.White] = whitePl;
            playerTypeSelection[(int)PlayerColour.Black] = blackPl;



            board = new Board();
            board.SetDefaultBoardPosition();
            guiBoard.Update(new GameRepresentation(board.OutputPieces()));
            ReadGameSettings();

            CreatePlayerFunctions = new Dictionary<string, CreatePlayer>();
            CreatePlayerFunctions.Add("This computer", CreateGUIPlayer);
            CreatePlayerFunctions.Add("AI", CreateAIPlayer); 
        }
        
        

        public void StartGame()
        {
            WriteGameSettings();

            playerWhiteType = playerTypeSelection[(int)PlayerColour.White].Text;
            playerBlackType = playerTypeSelection[(int)PlayerColour.Black].Text;

            game = new Game(board, initialTime, timePerTurn);
            BindTimersWithInterface();

            Console.WriteLine(playerWhiteType);
            Console.WriteLine(playerBlackType);

            playerWhite = CreatePlayerFunctions[playerWhiteType](PlayerColour.White);
            playerBlack = CreatePlayerFunctions[playerBlackType](PlayerColour.Black);

            game.MoveMade += playerWhite.OnMadeMove;
            game.MoveMade += playerBlack.OnMadeMove;
            game.OnGameEnded += guiBoard.EndGame;

            game.StartGame();
        }

        private void BindTimersWithInterface()
        {
            inGameInterface.DataContext = game;
            Label blackTime = (Label)((WrapPanel)inGameInterface.Children[0]).Children[1];
            Label whiteTime = (Label)((WrapPanel)inGameInterface.Children[3]).Children[1];

            Binding whiteBinding = new Binding("WhiteTime");
            whiteBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            whiteTime.SetBinding(ContentControl.ContentProperty, whiteBinding);

            Binding blackBinding = new Binding("BlackTime");
            blackBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            blackTime.SetBinding(ContentControl.ContentProperty, blackBinding);
        }

        private void ReadGameSettings()
        {
            if (File.Exists(@"GameSettings.txt"))
            {
                StreamReader file = new StreamReader(@"GameSettings.txt");
                playerWhiteType = file.ReadLine();
                playerBlackType = file.ReadLine();
                initialTime = Convert.ToDouble(file.ReadLine());
                timePerTurn = Convert.ToDouble(file.ReadLine());
                touchRule = Convert.ToBoolean(file.ReadLine());
                carryPieces = Convert.ToBoolean(file.ReadLine());

                file.Close();
            }
            else
            {
                playerWhiteType = "GUI Player";
                playerBlackType = "GUI Player";
                initialTime = 30;
                timePerTurn = 0;
                touchRule = false;
                carryPieces = false;
            }
            playerTypeSelection[(int)PlayerColour.White].SelectedIndex = 0;
            playerTypeSelection[(int)PlayerColour.Black].SelectedIndex = 0;
        }

        private void WriteGameSettings()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"GameSettings.txt");
            file.WriteLine(playerWhiteType);
            file.WriteLine(playerBlackType);
            file.WriteLine(initialTime);
            file.WriteLine(timePerTurn);
            file.WriteLine(touchRule);
            file.WriteLine(carryPieces);
            file.Close();
        }

    }
}
