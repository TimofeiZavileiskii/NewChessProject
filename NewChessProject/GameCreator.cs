   using System;
using System.IO;
using System.Collections.Generic;
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

        Dictionary<string, CreatePlayer> CreatePlayerFunctions;

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
            
            game.GameEnded += player.GameEnded;
            game.RequestMade += player.RequestSend;

            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[0]).Click += player.RequestTakeback;
            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[1]).Click += player.RequestDraw;
            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[2]).Click += player.Resign;

            return player;
        }

        Player CreateAIPlayer(PlayerColour colour)
        {
            return new AIPlayer(colour, game);
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


        
        public GameCreator(Game game, GUIBoard guiBoard, StackPanel inGameInterface)
        {
            this.game = game;
            this.guiBoard = guiBoard; 
            this.inGameInterface = inGameInterface;

            board = new Board();
            board.SetDefaultBoardPosition();
            guiBoard.Update(new GameRepresentation(board.OutputPieces()));
            ReadGameSettings();

            CreatePlayerFunctions = new Dictionary<string, CreatePlayer>();
            CreatePlayerFunctions.Add("GUI Player", CreateGUIPlayer);
            CreatePlayerFunctions.Add("AI Player", CreateAIPlayer);
        }
        
        

        public void StartGame()
        {
            WriteGameSettings();


            game = new Game(board);

            playerWhite = CreatePlayerFunctions[playerWhiteType](PlayerColour.White);
            playerBlack = CreatePlayerFunctions[playerBlackType](PlayerColour.Black);

            game.MoveMade += playerWhite.OnMadeMove;
            game.MoveMade += playerBlack.OnMadeMove;
            game.GameEnded += guiBoard.EndGame;
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
