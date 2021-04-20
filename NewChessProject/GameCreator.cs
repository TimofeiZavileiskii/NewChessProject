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
        static private class PlayerFactory
        {
            static readonly Dictionary<PlayerType, CreatePlayerDel> CreatePlayerFunctions;

            delegate Player CreatePlayerDel(PlayerColour colour, GameCreator gc);
            
            private static void BindPlayer(Player player, Game game)
            {
                game.OnGameEnded += player.GameEnded;
                game.RequestMade += player.RequestSend;
                game.GameStarted += player.StartGame;

            }

            static Player CreateGUIPlayer(PlayerColour colour, GameCreator gc)
            {
                GUIPlayer player = new GUIPlayer(colour, gc.game, gc.guiBoard, gc.additionalSettings.GeneralSettings);

                BindPlayer(player, gc.game);

                return player;
            }

            static Player CreateAIPlayer(PlayerColour colour, GameCreator gc)
            {
                AiSettings settings = (AiSettings)gc.additionalSettings.PlayerSettings[(int)colour];
                int timePerTurn = settings.MaxTimePerTurn;
                if(!(timePerTurn > 0))
                {
                    timePerTurn = -1;
                }

                AIPlayer player = new AIPlayer(colour, gc.game, settings.Difficulty, timePerTurn);

                BindPlayer(player, gc.game);

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

        List<PlayerType> possiblePlayerTypes;
        PlayerType whitePlayerType;
        PlayerType blackPlayerType;

        const string settingsFileName = @"GameSettings.txt";

        public double InitialTime
        {
            get
            {
                return initialTime;
            }
            set
            {
                double inputedTime = Convert.ToDouble(value);
                if (inputedTime > 0)
                    initialTime = inputedTime;
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
                double inputedTime = Convert.ToDouble(value);
                if (inputedTime > 0)
                    timePerTurn = inputedTime;
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
            game = new Game();
            game.ResetGame += PrepareForGameRestart;
            guiBoard.FlipBoard = false;
            guiBoard.Update(new GameRepresentation(game.GetPieceRepresentations()));
            gameWindow.SetSettingsMenu();



            ReadGameSettings();
        }


        public GameCreator(GUIBoard guiBoard, MainWindow gameWindow, AdditionalSettings aditionalSettings, StackPanel inGameInterface)
        {
            this.guiBoard = guiBoard; 
            this.inGameInterface = inGameInterface;
            this.gameWindow = gameWindow;

            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[0]).Click += guiBoard.RecieveTakeBackRequest;
            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[1]).Click += guiBoard.RecieveDrawRequest;
            ((Button)((WrapPanel)inGameInterface.Children[2]).Children[2]).Click += guiBoard.RecieveSurrenderRequest;

            gameWindow.SetSettingsMenu();

            this.additionalSettings = aditionalSettings;

            possiblePlayerTypes = ((PlayerType[])Enum.GetValues(typeof(PlayerType))).ToList();
            game = new Game();
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

            bool oneHumanPlayer = (whitePlayerType == PlayerType.GUIPlayer ^ blackPlayerType == PlayerType.GUIPlayer);

            game.StartGame(initialTime, timePerTurn, oneHumanPlayer);
            BindTimersWithInterface();
        }

        private void CreateGameViewer()
        {
            GameViewer gv = new GameViewer(guiBoard);

            game.MoveMade += gv.OnMadeMove;
            game.OnGameEnded += gv.OnGameEnded;
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
            if (File.Exists(settingsFileName))
            {
                StreamReader file = new StreamReader(settingsFileName);
                WhitePlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), file.ReadLine());
                BlackPlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), file.ReadLine());
                initialTime = Convert.ToDouble(file.ReadLine());
                timePerTurn = Convert.ToDouble(file.ReadLine());

                file.Close();
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private void SetDefaultSettings()
        {
            whitePlayerType = PlayerType.GUIPlayer;
            blackPlayerType = PlayerType.GUIPlayer;
            initialTime = 30;
            timePerTurn = 0;
        }

        private void WriteGameSettings()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(settingsFileName);
            file.WriteLine(whitePlayerType);
            file.WriteLine(blackPlayerType);
            file.WriteLine(initialTime);
            file.WriteLine(timePerTurn);
            file.Close();
        }
    }
}
