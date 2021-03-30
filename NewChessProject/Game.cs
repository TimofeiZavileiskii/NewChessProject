using System;
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
        move50Rule
    }

    class Game : INotifyPropertyChanged
    {
        const int maxPositionRepetition = 3;
        const double minuteLength = 60;
        const int max50MoveRule = 50;

        public event EventHandler<MadeMoveEventArgs> MoveMade;
        public event EventHandler<GameEndedEventArgs> OnGameEnded;
        public event EventHandler<RequestMadeEventArgs> RequestMade;
        public event PropertyChangedEventHandler PropertyChanged;

        GameState gameState;
        Board board;
        List<Piece>[] takenPieces;
        Timer[] timers;
        double[] timesPerPlayer;
        double timePerTurn;
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

        public Game(Board board, double timePerPlayer, double timePerTurn)
        {
            const double reportTime = 0.1;
            //fullMovesMade = 1;
            gameState = GameState.WhiteMove;
            gameHistory = new GameHistory();
            this.timePerTurn = timePerTurn;

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

            this.board = board;

            ImportGame(new FENPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));



            gameHistory.Add(GenerateFENPosition());


            GenerateMoves();
        }

        private void GameEnded(MoveResult endReason, PlayerColour? winner)
        {
            foreach (Timer timer in timers)
                timer.Stop();


            OnGameEnded?.Invoke(this, new GameEndedEventArgs(endReason, winner));
        }

        private void UpdateTime(object sender, EventArgs e)
        {
            timesPerPlayer[(int)((Timer)sender).Owner] = ((Timer)sender).TimeLeft;
            NotifyPropertyChanged("WhiteTime");
            NotifyPropertyChanged("BlackTime");

            if (((Timer)sender).TimeLeft < 0.05)
            {
                GameEnded(MoveResult.TimeOut, board.ReverseColour(IdentifyPlayersColour(gameState)));
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GenerateMoves()
        {
            board.GenerateMoves(board.ReverseColour(IdentifyPlayersColour(gameState)));
            board.GenerateMoves(IdentifyPlayersColour(gameState));
            board.FilterMovesByCheck(IdentifyPlayersColour(gameState));
            board.RemoveEnPassante();
        }

        public EnterResult EnterMove(PlayerColour colour, Vector movedPiece, Vector targetPosition)
        {
            if(IdentifyPlayersColour(gameState) != colour)
            {
                return EnterResult.NotPlayersMove;
            }
            if(board[movedPiece] == null || board[movedPiece].Colour != colour)
            {
                return EnterResult.WrongSquareSelected;
            }
            if (!board[movedPiece].MoveExists(targetPosition))
            {
                return EnterResult.NoMoveExist;
            }

            board.MovePiece(movedPiece, targetPosition);


            if(board.IsPawnTransformationNeeded(colour))
            {
                gameState = TurnSelectPieceState(gameState);
                return EnterResult.WaitForPawnSlection;
            }
            else
            {
                OnMadeMove();

                return EnterResult.Done;
            }
        }

        private GameState TurnSelectPieceState(GameState gm)
        {
            GameState result = GameState.BlackPawnSelection;
            if(gm == GameState.WhiteMove)
            {
                result = GameState.WhitePawnSelection;
            }
            return result;
        }

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
            timers[(int)board.ReverseColour(IdentifyPlayersColour(gameState))].Stop();
            timers[(int)IdentifyPlayersColour(gameState)].Start();
            timers[(int)IdentifyPlayersColour(gameState)].Add(timePerTurn);
        }

        public bool PiecePresent(PlayerColour colour, Vector vector)
        {
            if (board[vector] != null)
                return board[vector].Colour == colour;
            return false;
        }

        public void ChoosePawnTransformation(PlayerColour colour, PieceType type)
        {
            board.TransformPawn(colour, type);

            OnMadeMove();
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
          
        private MoveResult DetermineMoveResult()
        {
            MoveResult result = MoveResult.Continue;

            bool areAvailkableMoves = board.IsTherePossibleMoves(IdentifyPlayersColour(gameState));
            bool kingUnderThreat = board.IsChecked(IdentifyPlayersColour(gameState));

            
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
                result = MoveResult.MoveRepetition;
                gameState = GameState.EndGame;
            }
            else if (areAvailkableMoves && kingUnderThreat)
            {
                result = MoveResult.Check;
            }

            return result;
        }

        public void ImportGame(FENPosition fenPosition)
        {
            fullMovesMade = Convert.ToInt32(fenPosition.TotalMoves);

            if (fenPosition.CurrentPlayer == "b")
                gameState = GameState.BlackMove;
            else
                gameState = GameState.WhiteMove;

            board.UploadFENPosition(fenPosition);
        }

        public Vector GetKingsPosition(PlayerColour colour)
        {
            return board.GetKingPosition(colour);
        }

        public void Resign(PlayerColour colour)
        {
            GameEnded(MoveResult.Resignation, board.ReverseColour(IdentifyPlayersColour(gameState)));
        }

        private void MakeRequest(Request request)
        {
            RequestMade?.Invoke(this, new RequestMadeEventArgs(request));
        }

        public void OfferDraw(PlayerColour colour)
        {
            Request request = new Request(RequestType.OfferDraw, "Agree to the draw?");
            MakeRequest(request);
            if (request.Agreed)
                GameEnded(MoveResult.Draw, null);
        }

        public void TakeBack(PlayerColour colour)
        {
            Request request = new Request(RequestType.ProposeTakeback, "Accept takeback from " + colour.ToString() + " player");
            MakeRequest(request);
            if (request.Agreed)
            {
                ImportGame(gameHistory.ReverseMove());
                GenerateMoves();
            }
        }

        private FENPosition GenerateFENPosition()
        {
            FENPosition output = board.GetFENInformation();
            output.CurrentPlayer = IdentifyPlayersColour(gameState).ToString().ToLower()[0].ToString();
            output.TotalMoves = fullMovesMade.ToString();

            return output;
        }

        protected virtual void OnMadeMove()
        {
            if (IdentifyPlayersColour(gameState) == PlayerColour.Black)
                fullMovesMade++;
            SwitchPlayers();

            gameHistory.Add(GenerateFENPosition());
            GenerateMoves();

            MoveResult result = DetermineMoveResult();

            if (result == MoveResult.Check || result == MoveResult.Continue)
            {
                if (board.Rule50Counter >= max50MoveRule)
                    GameEnded(MoveResult.move50Rule, null);
                else
                    MoveMade?.Invoke(this, new MadeMoveEventArgs(result));
            }
            else
            {
                GameEnded(result, board.ReverseColour(IdentifyPlayersColour(gameState)));
            }
        }
    }
}
