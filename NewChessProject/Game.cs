using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Draw
    }

    class Game : INotifyPropertyChanged
    {
        const int maxPositionRepetition = 3;


        public event EventHandler<MadeMoveEventArgs> MoveMade;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        public event EventHandler<RequestMadeEventArgs> RequestMade;
        public event PropertyChangedEventHandler PropertyChanged;

        GameState gameState;
        Board board;
        List<Piece>[] takenPieces;
        Timer[] timers;
        double[] timesPerPlayer;
        double timePerTurn;
        GameHistory gameHistory;

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
            const double minuteLength = 60;

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
            if (minutes != 0)
                output = minutes.ToString() + ":" + output;

            return output;
        }

        public Game(Board board, double timePerPlayer, double timePerTurn)
        {
            const double reportTime = 0.1;
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
                timers[(int)colour] = new Timer(reportTime, timePerPlayer, colour);
                timers[(int)colour].TimePassed += UpdateTime;
            }
            timers[(int)PlayerColour.White].Start();

            this.board = board;

            gameHistory.Add(board.Copy());

            GenerateMoves();
        }

        private void UpdateTime(object sender, EventArgs e)
        {
            timesPerPlayer[(int)((Timer)sender).Owner] = ((Timer)sender).TimeLeft;
            NotifyPropertyChanged("WhiteTime");
            NotifyPropertyChanged("BlackTime");
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

        public void ImportGame()
        {

        }

        public Vector GetKingsPosition(PlayerColour colour)
        {
            return board.GetKingPosition(colour);
        }

        public void Resign(PlayerColour colour)
        {
            GameEnded?.Invoke(this, new GameEndedEventArgs(MoveResult.Resignation, board.ReverseColour(IdentifyPlayersColour(gameState))));
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
                GameEnded?.Invoke(this, new GameEndedEventArgs(MoveResult.Draw, colour));
        }


        public void TakeBack(PlayerColour colour)
        {
            Request request = new Request(RequestType.ProposeTakeback, "Accept takeback from " + colour.ToString() + " player");
            MakeRequest(request);
            if (request.Agreed)
            {
                board = gameHistory.ReverseMove();
                GenerateMoves();
                
            }
        }

        protected virtual void OnMadeMove()
        {
            gameHistory.Add(board.Copy());

            SwitchPlayers();
            GenerateMoves();

            MoveResult result = DetermineMoveResult();

            if (result == MoveResult.Check || result == MoveResult.Continue)
            {
                MoveMade?.Invoke(this, new MadeMoveEventArgs(result));
            }
            else
            {
                GameEnded?.Invoke(this, new GameEndedEventArgs(result, board.ReverseColour(IdentifyPlayersColour(gameState))));
            }
        }
    }
}
