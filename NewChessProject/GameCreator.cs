   using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameCreator
    {
        Board board;
        Player playerWhite;
        Player playerBlack;
        GUIBoard guiBoard;
        Game game;

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
            GUIPlayer player = new GUIPlayer(colour, game);
            guiBoard.OnBoardClicked += player.OnBoardClicked;
            guiBoard.OnWindowClicked += player.OnWindowClicked;
            guiBoard.PieceSelected += player.PieceSelected;
            player.OnGameEnded += guiBoard.EndGame;
            player.OnPawnNeedsTransforemation += guiBoard.PreparePieceSelection;
            player.OnGameRepresentationUpdated += guiBoard.Update;

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

        public GameCreator(Player white, Player black, Game game, GUIBoard guiBoard)
        {
            this.game = game;
            this.guiBoard = guiBoard;
            board = new Board();
            SetDefaultBoardPosition();

            guiBoard.Update(null, new GUIBoardUpdateEventArgs(new GameRepresentation(board.OutputPieces())));
            playerWhite = white;
            playerBlack = black;
            ReadGameSettings();

            CreatePlayerFunctions = new Dictionary<string, CreatePlayer>();
            CreatePlayerFunctions.Add("GUI Player", CreateGUIPlayer);
            CreatePlayerFunctions.Add("AI Player", CreateAIPlayer);
        }
        
        private void SetDefaultBoardPosition()
        {
            board.AddPiece(new Bishop(PlayerColour.White), new Vector(2, 0));
            board.AddPiece(new Bishop(PlayerColour.White), new Vector (5, 0));

            board.AddPiece(new Bishop(PlayerColour.Black), new Vector(2, Board.boardHeight - 1));
            board.AddPiece(new Bishop(PlayerColour.Black), new Vector(5, Board.boardHeight - 1));

            board.AddPiece(new Knight(PlayerColour.White), new Vector(1, 0));
            board.AddPiece(new Knight(PlayerColour.White), new Vector(6, 0));

            board.AddPiece(new Knight(PlayerColour.Black), new Vector(1, Board.boardHeight - 1));
            board.AddPiece(new Knight(PlayerColour.Black), new Vector(6, Board.boardHeight - 1));

            board.AddPiece(new Rook(PlayerColour.White), new Vector(0, 0));
            board.AddPiece(new Rook(PlayerColour.White), new Vector(7, 0));

            board.AddPiece(new Rook(PlayerColour.Black), new Vector(0, Board.boardHeight - 1));
            board.AddPiece(new Rook(PlayerColour.Black), new Vector(7, Board.boardHeight - 1));

            board.AddPiece(new Queen(PlayerColour.White), new Vector(3, 0));
            board.AddPiece(new Queen(PlayerColour.Black), new Vector(3, Board.boardHeight - 1));

            board.AddPiece(new King(PlayerColour.White), new Vector(4, 0));
            board.AddPiece(new King(PlayerColour.Black), new Vector(4, Board.boardHeight - 1));

            for (int i = 0; i < Board.boardWidth; i++)
            {
                board.AddPiece(new Pawn(PlayerColour.White), new Vector(i, 1));
            }

            for (int i = 0; i < Board.boardWidth; i++)
            {
                board.AddPiece(new Pawn(PlayerColour.Black), new Vector(i, Board.boardWidth - 2));
            }
        }

        public void StartGame()
        {
            WriteGameSettings();


            game = new Game(board);

            playerWhite = CreatePlayerFunctions[playerWhiteType](PlayerColour.White);
            playerBlack = CreatePlayerFunctions[playerBlackType](PlayerColour.Black);

            game.MoveMade += playerWhite.OnMadeMove;
            game.MoveMade += playerBlack.OnMadeMove;
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
