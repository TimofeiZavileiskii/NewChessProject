using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;




namespace NewChessProject
{
    enum PlayerColour
    {
        Black,
        White
    }

    enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    enum GameState
    {
        BlackMove,
        BlackPawnSelection,
        WhiteMove,
        WhitePawnSelection,
        EndGame
    }

    enum EnterResult
    {
        Done,
        NotPlayersMove,
        WrongSquareSelected,
        WaitForPawnSlection,
        NoMoveExist
    }

    enum MoveResult
    {
        Continue,
        Check,
        Mate,
        Stalemate,
        MoveRepetition,
        Resignation,
        TimeOut,
        Draw,
        move50Rule,
        InsufficientMaterial
    }

    class Game : INotifyPropertyChanged
    {

        const int maxPositionRepetition = 3;
        const double minuteLength = 60;
        const int max50MoveRule = 50;

        public event EventHandler<MadeMoveEventArgs> MoveMade;
        public event EventHandler<GameStartEventArgs> GameStarted;
        public event EventHandler<GameEndedEventArgs> OnGameEnded;
        public event EventHandler<RequestMadeEventArgs> RequestMade;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ResetGame;

        GameState gameState;
        FENPosition currentFENPosition;
        Board board;
        List<Piece>[] takenPieces;
        Timer[] timers;
        double[] timesPerPlayer;

        double addedTimePerTurn;
        GameHistory gameHistory;
        int fullMovesMade;

        public string BlackTime
        {
            get
            {
                return TurnTimeToMinutes(timers[(int)PlayerColour.Black].TimeLeft);
            }
        }

        public string WhiteTime
        {
            get
            {
                return TurnTimeToMinutes(timers[(int)PlayerColour.White].TimeLeft);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PieceType GetPieceType(Vector location)
        {
            return board[location].Type;
        }

        public Vector GetKingsPosition(PlayerColour colour)
        {
            return board.GetKingPosition(colour);
        }

        public List<PieceRepresentation> GetPieceRepresentations()
        {
            return board.OutputPieces();
        }

        public List<Vector> GetAllowedPositions(PlayerColour colour, Vector movedPiece)
        {
            List<Vector> output = new List<Vector>();
            if (board[movedPiece] != null)
                if (board[movedPiece].Colour == colour)
                    output = board[movedPiece].AvailableMoves;
            return output;
        }

        public Game()
        {
            board = new Board();
            board.SetDefaultBoardPosition();

            gameState = GameState.WhiteMove;
            gameHistory = new GameHistory();
        }


        //Presents the time value in timer as a string suitable for the timer label
        private string TurnTimeToMinutes(double totalTime)
        {
            const double decimalThreshold = 20;


            double seconds;
            int minutes = (int)Math.Floor(totalTime / minuteLength);
            if (totalTime < decimalThreshold)
            {
                seconds = Math.Round(totalTime - minuteLength * minutes, 1);
            }
            else
            {
                seconds = Math.Round(totalTime - minuteLength * minutes, 0);
            }
            string output = seconds.ToString();

            if (output.Length == 1)
                output = "0" + output;
                

            if (minutes != 0)
                output = minutes.ToString() + ":" + output;

            return output;
        }

        //The event called by timers to update the values, used in properties
        private void UpdateTime(object sender, EventArgs e)
        {
            timesPerPlayer[(int)((Timer)sender).Owner] = ((Timer)sender).TimeLeft;
            NotifyPropertyChanged("WhiteTime");
            NotifyPropertyChanged("BlackTime");

            if (((Timer)sender).TimeLeft < 0.05)
            {
                PlayerColour playerRunOutOfTime = IdentifyPlayersColour(gameState);

                if (board.NotEnoughMaterialForMate(Board.ReverseColour(playerRunOutOfTime)))
                {
                    GameEnded(MoveResult.InsufficientMaterial, null);
                }
                else
                {
                    GameEnded(MoveResult.TimeOut, playerRunOutOfTime);
                }
            }
        }

        public void StartGame(double timePerPlayer, double timePerTurn, bool oneHumanPlayer)
        {
            const double reportTime = 0.1;

            addedTimePerTurn = timePerTurn;

            timers = new Timer[Enum.GetValues(typeof(PlayerColour)).Length];
            takenPieces = new List<Piece>[Enum.GetValues(typeof(PlayerColour)).Length];
            timesPerPlayer = new double[Enum.GetValues(typeof(PlayerColour)).Length];

            foreach (PlayerColour colour in Enum.GetValues(typeof(PlayerColour)))
            {
                takenPieces[(int)colour] = new List<Piece>();
            }

            foreach (PlayerColour colour in Enum.GetValues(typeof(PlayerColour)))
            {
                timers[(int)colour] = new Timer(reportTime, timePerPlayer * minuteLength, colour);
                timers[(int)colour].OnTimePassed += UpdateTime;
            }
            timers[(int)PlayerColour.White].Start();

            GenerateMoves();

            currentFENPosition = GenerateFENPosition();

            gameHistory.Add(GetFENPosition());

            GameStarted?.Invoke(this, new GameStartEventArgs(oneHumanPlayer, IdentifyPlayersColour(gameState)));
        }

        //Event fireds if move was finished
        private void OnMadeMove(PlayerColour playerMadeMove)
        {
            if (IdentifyPlayersColour(gameState) == PlayerColour.Black)
                fullMovesMade++;
            SwitchPlayers();

            currentFENPosition = GenerateFENPosition();

            gameHistory.Add(GetFENPosition());
            GenerateMoves();

            MoveResult result = DetermineMoveResult();

            if (result == MoveResult.Check || result == MoveResult.Continue)
            {
                if (board.Rule50Counter >= max50MoveRule)
                    GameEnded(MoveResult.move50Rule, null);
                else
                    MoveMade?.Invoke(this, new MadeMoveEventArgs(result, IdentifyPlayersColour(gameState)));
            }
            else
            {
                GameEnded(result, playerMadeMove);
            }
        }

        private void GameEnded(MoveResult endReason, PlayerColour? winner)
        {
            TerminateTimers();

            OnGameEnded?.Invoke(this, new GameEndedEventArgs(endReason, winner));
            ResetGame?.Invoke(this, EventArgs.Empty);
        }

        public void EndImmediatly()
        {
            TerminateTimers();
            gameState = GameState.EndGame;
        }

        private void TerminateTimers()
        {
            if (timers != null)
                foreach (Timer timer in timers)
                    timer.Terminate();
        }

        private PlayerColour FENPlayerToMove(FENPosition fenPos)
        {
            PlayerColour output = PlayerColour.White;
            if (fenPos.CurrentPlayer == "b")
                output = PlayerColour.Black;
            return PlayerColour.White;
        }

        private void GenerateMoves()
        {
            board.GenerateMoves(Board.ReverseColour(IdentifyPlayersColour(gameState)));
            board.GenerateMoves(IdentifyPlayersColour(gameState));
            board.FilterMovesByCheck(IdentifyPlayersColour(gameState));
            board.RemoveEnPassante();
        }

        public EnterResult EnterMove(PlayerColour sendersColour, Vector movedPiece, Vector targetPosition)
        {
            if(IdentifyPlayersColour(gameState) != sendersColour)
            {
                return EnterResult.NotPlayersMove;
            }
            if(board[movedPiece] == null || board[movedPiece].Colour != sendersColour)
            {
                return EnterResult.WrongSquareSelected;
            }
            if (!board[movedPiece].MoveExists(targetPosition))
            {
                return EnterResult.NoMoveExist;
            }

            board.MovePiece(movedPiece, targetPosition);


            if(board.IsPawnTransformationNeeded(sendersColour))
            {
                gameState = TurnSelectPawnState(gameState);
                return EnterResult.WaitForPawnSlection;
            }
            else
            {
                OnMadeMove(sendersColour);

                return EnterResult.Done;
            }
        }

        //Outputs the game state, which corresponds to the pawn by the player of colour, whose turn was represented by the inputed game state
        private GameState TurnSelectPawnState(GameState gm)
        {
            GameState result = GameState.BlackPawnSelection;
            if(gm == GameState.WhiteMove)
            {
                result = GameState.WhitePawnSelection;
            }
            return result;
        }

        //Outputs colour of a player, whose turn is represented by the inputed state
        private PlayerColour IdentifyPlayersColour(GameState state)
        {
            PlayerColour output = PlayerColour.White;
            if(state == GameState.BlackMove || state == GameState.BlackPawnSelection)
            {
                output = PlayerColour.Black;
            }
            return output;
        }

        private void SwitchPlayers()
        {
            if(gameState == GameState.BlackMove || gameState == GameState.BlackPawnSelection)
            {
                gameState = GameState.WhiteMove;
            }
            else
            {
                gameState = GameState.BlackMove;
            }
            timers[(int)Board.ReverseColour(IdentifyPlayersColour(gameState))].Stop();
            timers[(int)IdentifyPlayersColour(gameState)].Start();
            timers[(int)IdentifyPlayersColour(gameState)].Add(addedTimePerTurn);
        }

        //Outputs wheather or not a piece is present on the given square
        public bool PiecePresent(PlayerColour colour, Vector vector)
        {
            if (board[vector] != null)
                return board[vector].Colour == colour;
            return false;
        }

        public void ChoosePawnTransformation(PlayerColour sendersColour, PieceType type)
        {
            if(IdentifyPlayersColour(gameState) == sendersColour)
                board.TransformPawn(sendersColour, type);

            OnMadeMove(sendersColour);
        }
        
        private MoveResult DetermineMoveResult()
        {
            MoveResult result = MoveResult.Continue;

            bool areAvailkableMoves = board.IsTherePossibleMoves(IdentifyPlayersColour(gameState));
            bool kingUnderThreat = board.IsChecked(IdentifyPlayersColour(gameState));

            if(board.NotEnoughMaterialForMate(IdentifyPlayersColour(gameState)) && board.NotEnoughMaterialForMate(Board.ReverseColour(IdentifyPlayersColour(gameState))))
            {
                result = MoveResult.InsufficientMaterial;
                gameState = GameState.EndGame;
            }
            if (!areAvailkableMoves && !kingUnderThreat)
            {
                result = MoveResult.Stalemate;
                gameState = GameState.EndGame;
            }
            else if (!areAvailkableMoves && kingUnderThreat)
            {
                result = MoveResult.Mate;
                gameState = GameState.EndGame;
            }
            else if (gameHistory.CheckPositionRepetition(maxPositionRepetition))
            {
                gameHistory.ListGameHistory();
                result = MoveResult.MoveRepetition;
                gameState = GameState.EndGame;
            }
            else if (areAvailkableMoves && kingUnderThreat)
            {
                result = MoveResult.Check;
            }

            return result;
        }

        //Method is used to set an unverified FEN position. It will not input anything and return false, if FEN string is not valid
        public bool SetFENString(string FenString)
        {
            if (!VerifyFENString(FenString))
            {
                return false;
            }
            FENPosition fenPos = new FENPosition(FenString);
            ImportGame(fenPos);

            return true;
        }

        private bool VerifyFENString(string fenString)
        {
            if (fenString == "")
                return false;

            bool correctFormat = Regex.IsMatch(fenString, @"^(([rnbqkpRNBQKP1-8]{1,8}\/){7}([rnbqkpRNBQKP1-8]{1,8}) (w|b) (-|(K?Q?k?q?)) (-|([a-h][1-8])) (([0-9])|([1-9][0-9]*)) [1-9][0-9]*)$");

            string[] splitString = fenString.Split(' ');
            bool uniqueKings = Regex.Matches(splitString[0], @"k").Count == 1 && Regex.Matches(splitString[0], @"K").Count == 1;

            bool correctLength = true;

            //Checks that length of each row is correct
            foreach(string row in splitString[0].Split('/'))
            {
                int numbersSum = 0;
                foreach (Match number in Regex.Matches(row, @"\d"))
                {
                    numbersSum += Convert.ToInt32(number.ToString());
                }

                correctLength = Regex.Matches(row, @"[rnbqkpRNBQKP]").Count + numbersSum == Board.boardWidth;

                if (!correctLength)
                    break;
            }

            //Checks that no pawn is standing on the square, where it has to promote
            bool noPawnsNeedToPromote = !splitString[0].Split('/')[0].Contains("P") && !splitString[0].Split('/')[Board.boardHeight - 1].Contains("p");

            //Checks that king of the player, who cannot move right now, is not in check
            //It requires to generatePositions
            Board testBoard = new Board();
            FENPosition testedFENPosition = new FENPosition(fenString);
            testBoard.UploadFENPosition(testedFENPosition);

            PlayerColour currentTurn = FENPlayerToMove(testedFENPosition);
            testBoard.GenerateMoves(currentTurn);

            bool kingIsSafe = !testBoard.IsChecked(Board.ReverseColour(currentTurn));

            return correctFormat && uniqueKings && correctLength && noPawnsNeedToPromote && noPawnsNeedToPromote && kingIsSafe;
        }

        //Sets the current game to the state specified by the passed FEN string
        private void ImportGame(FENPosition fenPosition)
        {
            currentFENPosition = fenPosition;
            fullMovesMade = Convert.ToInt32(fenPosition.TotalMoves);

            if (fenPosition.CurrentPlayer == "b")
                gameState = GameState.BlackMove;
            else
                gameState = GameState.WhiteMove;

            board.UploadFENPosition(fenPosition);
        }

        public void Resign(PlayerColour colour)
        {
            GameEnded(MoveResult.Resignation, Board.ReverseColour(colour));
        }

        private void MakeRequest(Request request)
        {
            RequestMade?.Invoke(this, new RequestMadeEventArgs(request));
        }

        public void OfferDraw(PlayerColour colour)
        {
            Request request = new Request(RequestType.OfferDraw, "Agree to the draw?", Board.ReverseColour(colour));
            MakeRequest(request);
            if (request.Agreed)
                GameEnded(MoveResult.Draw, null);
        }

        public bool TakeBack(PlayerColour colour)
        {
            Request request = new Request(RequestType.ProposeTakeback, "Accept takeback from " + colour.ToString() + " player", Board.ReverseColour(colour));
            MakeRequest(request);
            if (request.Agreed)
            {
                int reverseMoves = 2;
                if(IdentifyPlayersColour(gameState) != colour)
                {
                    reverseMoves = 1;
                }

                FENPosition lastPosition = gameHistory.ReverseMove(reverseMoves);

                if (lastPosition != null)
                {
                    ImportGame(lastPosition);
                    GenerateMoves();
                    return true;
                }
                else
                {
                    MessageBox.Show("There is no previous move to return to.");
                }
            }
            return false;
        }

        public FENPosition GetFENPosition()
        {
            return currentFENPosition;
        }

        //Outputs the FEN string corresponding to the current game state
        private FENPosition GenerateFENPosition()
        {
            FENPosition output = board.GetFENInformation();
            output.CurrentPlayer = IdentifyPlayersColour(gameState).ToString().ToLower()[0].ToString();
            output.TotalMoves = fullMovesMade.ToString();

            Console.WriteLine(output.FENString);
            return output;
        }

        //Makes the move specified by the vectors, and outputs the board wich will appear as a result of the move
        private Board TestMove(Board board, PlayerColour nextPlayerColour, Vector movedPiece, Vector move)
        {
            Board testBoard = board.Copy();
            testBoard.MovePiece(movedPiece, board[movedPiece].AvailableMoves.Find(x => x == move));
            testBoard.GenerateMoves(nextPlayerColour);
            testBoard.GenerateMoves(Board.ReverseColour(nextPlayerColour));
            testBoard.FilterMovesByCheck(nextPlayerColour);

            return testBoard;
        }

        public bool CheckForAttackFrom(PlayerColour colour, Vector movedPiece, Vector move)
        {
            return TestMove(board, colour, movedPiece, move).IsThereThreat(move, Board.ReverseColour(colour));
        }

        public bool CheckForDefence(PlayerColour colour, Vector movedPiece, Vector move)
        {
            bool output = true;

            Board testBoard = TestMove(board, Board.ReverseColour(colour), movedPiece, move);
            List<Vector> attackingPieces = testBoard.GetAttackingPieces(Board.ReverseColour(colour), move);
            
            foreach(Vector attackingPiece in attackingPieces)
            {
                Board newTestBoard = TestMove(testBoard, colour, attackingPiece, move);
                output = newTestBoard.IsThereThreat(move, Board.ReverseColour(colour));
                if (!output)
                    break;
            }

            return output;
        }
    }
}
