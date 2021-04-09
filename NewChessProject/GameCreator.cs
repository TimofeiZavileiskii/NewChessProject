﻿using System;
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
        static private class PlayerFactory
        {
            static readonly Dictionary<PlayerType, CreatePlayerDel> CreatePlayerFunctions;
            const string stockFishAddress = @"stockfish_13_win_x64_bmi2\stockfish_13_win_x64_bmi2";

            delegate Player CreatePlayerDel(PlayerColour colour, GameCreator gc);

            static Player CreateGUIPlayer(PlayerColour colour, GameCreator gc)
            {
                GUIPlayer player = new GUIPlayer(colour, gc.game, gc.guiBoard);

                gc.guiBoard.OnBoardClicked += player.OnBoardClicked;
                gc.guiBoard.OnWindowClicked += player.OnWindowClicked;
                gc.guiBoard.PieceSelected += player.PieceSelected;


                gc.game.OnGameEnded += player.GameEnded;
                gc.game.RequestMade += player.RequestSend;

                ((Button)((WrapPanel)gc.inGameInterface.Children[2]).Children[0]).Click += player.RequestTakeback;
                ((Button)((WrapPanel)gc.inGameInterface.Children[2]).Children[1]).Click += player.RequestDraw;
                ((Button)((WrapPanel)gc.inGameInterface.Children[2]).Children[2]).Click += player.Resign;

                return player;
            }

            static Player CreateAIPlayer(PlayerColour colour, GameCreator gc)
            {
                AiSettings settings = ((AiSettings)gc.additionalSettings.PlayerSettings[(int)colour]);
                int timePerTurn = settings.MaxTimePerTurn;
                if(!(timePerTurn > 0))
                {
                    timePerTurn = -1;
                }

                AIPlayer player = new AIPlayer(colour, new ChessEngine(stockFishAddress, "StockFish", settings.Difficulty, timePerTurn), gc.game);

                gc.game.GameStarted += player.GameStarted;
                gc.game.RequestMade += player.RequestSend;
                return player;
            }

            public static Player CreatePlayer(PlayerType type, PlayerColour colour, GameCreator gc)
            {
                return CreatePlayerFunctions[type](colour, gc);
            }

            static PlayerFactory()
            {
                CreatePlayerFunctions = new Dictionary<PlayerType, CreatePlayerDel>();
                CreatePlayerFunctions.Add(PlayerType.GUIPlayer, CreateGUIPlayer);
                CreatePlayerFunctions.Add(PlayerType.AIPlayer, CreateAIPlayer);
            }

        }

        Player playerWhite;
        Player playerBlack;
        GUIBoard guiBoard;
        Game game;
        StackPanel inGameInterface;
        MainWindow gameWindow;
        AdditionalSettings additionalSettings;

        string importFENString;
        double initialTime;
        double timePerTurn;
        bool touchRule;
        bool carryPieces;

        List<PlayerType> possiblePlayerTypes;
        PlayerType whitePlayerType;
        PlayerType blackPlayerType;

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
        public string ImportedFENString
        {
            get
            {
                return importFENString;
            }
            set
            {
                importFENString = value;
            }
        }
        public PlayerType WhitePlayerType
        {
            get { return whitePlayerType; }
            set 
            {
                whitePlayerType = value;
                additionalSettings.WhitePlayerType = value;
            }
        }
        public PlayerType BlackPlayerType
        {
            get { return blackPlayerType; }
            set 
            {
                blackPlayerType = value;
                additionalSettings.BlackPlayerType = value;
            }
        }

        public void ImportFENString()
        {
            bool output = game.SetFENString(ImportedFENString);
            if (!output)
            {
                ImportedFENString = "";
                MessageBox.Show("FEN input was invalid");
            }
            

            guiBoard.Update(new GameRepresentation(game.GetPieceRepresentations()));
        }

        public Game GetGame()
        {
            return game;
        }

        public List<PlayerType> PossiblePlayerTypes
        {
            get { return possiblePlayerTypes; }
        }

        private void PrepareForGameRestart(object sender, EventArgs e)
        {
            game = new Game(new Board());
            game.ResetGame += PrepareForGameRestart;
            guiBoard.Update(new GameRepresentation(game.GetPieceRepresentations()));
            gameWindow.SetSettingsMenu();

            ReadGameSettings();
        }


        public GameCreator(GUIBoard guiBoard, MainWindow gameWindow, AdditionalSettings adSett, StackPanel inGameInterface)
        {
            this.guiBoard = guiBoard; 
            this.inGameInterface = inGameInterface;
            this.gameWindow = gameWindow;

            gameWindow.SetSettingsMenu();

            additionalSettings = adSett;

            possiblePlayerTypes = ((PlayerType[])Enum.GetValues(typeof(PlayerType))).ToList();
            game = new Game(new Board());
            game.ResetGame += PrepareForGameRestart;
            guiBoard.Update(new GameRepresentation(game.GetPieceRepresentations()));
            importFENString = "";

            ReadGameSettings();
        }

        

        public void StartGame()
        {
            WriteGameSettings();

            playerWhite = PlayerFactory.CreatePlayer(whitePlayerType, PlayerColour.White, this);
            playerBlack = PlayerFactory.CreatePlayer(BlackPlayerType, PlayerColour.Black, this);

            if (blackPlayerType != PlayerType.GUIPlayer && whitePlayerType != PlayerType.GUIPlayer)
                CreateGameViewer();

            game.MoveMade += playerWhite.OnMadeMove;
            game.MoveMade += playerBlack.OnMadeMove;
            game.OnGameEnded += guiBoard.EndGame;

            game.StartGame(initialTime, timePerTurn);
            BindTimersWithInterface();
        }

        private void CreateGameViewer()
        {
            GameViewer gv = new GameViewer(guiBoard);

            game.MoveMade += gv.OnMadeMove;
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
                WhitePlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), file.ReadLine());
                BlackPlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), file.ReadLine());
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
