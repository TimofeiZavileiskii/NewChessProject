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
    enum PlayerType
    {
        GUIPlayer,
        AIPlayer
    }

    class GameCreator
    {
        Board board;
        Player playerWhite;
        Player playerBlack;
        GUIBoard guiBoard;
        Game game;
        StackPanel inGameInterface;

        Dictionary<PlayerType, CreatePlayer> CreatePlayerFunctions;

        const string stockFishAddress = @"stockfish_13_win_x64_bmi2\stockfish_13_win_x64_bmi2";

        double initialTime;
        double timePerTurn;
        bool touchRule;
        bool carryPieces;

        List<PlayerType> possiblePlayerTypes;
        PlayerType whitePlayerType;
        PlayerType blackPlayerType;

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
            AIPlayer player = new AIPlayer(colour, new ChessEngine(stockFishAddress, "StockFish", 2), game);
            game.GameStarted += player.GameStarted;
            game.RequestMade += player.RequestSend;
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

        public PlayerType WhitePlayerType
        {
            get { return whitePlayerType; }
            set { whitePlayerType = value; }
        }

        public PlayerType BlackPlayerType
        {
            get { return blackPlayerType; }
            set { blackPlayerType = value; }
        }
        public List<PlayerType> PossiblePlayerTypes
        {
            get { return possiblePlayerTypes; }
        }

        public GameCreator(Game game, GUIBoard guiBoard, StackPanel inGameInterface)
        {
            Board.FillFENRepresentations();
            this.game = game;
            this.guiBoard = guiBoard; 
            this.inGameInterface = inGameInterface;

            possiblePlayerTypes = ((PlayerType[])Enum.GetValues(typeof(PlayerType))).ToList();

            board = new Board();
            board.SetDefaultBoardPosition();
            guiBoard.Update(new GameRepresentation(board.OutputPieces()));
            ReadGameSettings();

            CreatePlayerFunctions = new Dictionary<PlayerType, CreatePlayer>();
            CreatePlayerFunctions.Add(PlayerType.GUIPlayer, CreateGUIPlayer);
            CreatePlayerFunctions.Add(PlayerType.AIPlayer, CreateAIPlayer); 
        }
        
        

        public void StartGame()
        {
            WriteGameSettings();

            game = new Game(board, initialTime, timePerTurn);
            BindTimersWithInterface();

            playerWhite = CreatePlayerFunctions[whitePlayerType](PlayerColour.White);
            playerBlack = CreatePlayerFunctions[blackPlayerType](PlayerColour.Black);

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
                whitePlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), file.ReadLine());
                blackPlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), file.ReadLine());
                initialTime = Convert.ToDouble(file.ReadLine());
                timePerTurn = Convert.ToDouble(file.ReadLine());
                touchRule = Convert.ToBoolean(file.ReadLine());
                carryPieces = Convert.ToBoolean(file.ReadLine());

                file.Close();
            }
            else
            {
                whitePlayerType = PlayerType.GUIPlayer;
                blackPlayerType = PlayerType.GUIPlayer;
                initialTime = 30;
                timePerTurn = 0;
                touchRule = false;
                carryPieces = false;
            }
        }

        private void WriteGameSettings()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"GameSettings.txt");
            file.WriteLine(whitePlayerType);
            file.WriteLine(blackPlayerType);
            file.WriteLine(initialTime);
            file.WriteLine(timePerTurn);
            file.WriteLine(touchRule);
            file.WriteLine(carryPieces);
            file.Close();
        }

    }
}
