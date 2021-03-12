using System;
using System.Collections.Generic;
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
        Resignation
    }






    class Game
    {
        const int maxPositionRepetition = 3;
        public event EventHandler<MadeMoveEventArgs> MoveMade;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        GameState gameState;
        Board board;
        List<Piece>[] takenPieces;
        GameHistory gameHistory;

        public Game(Board board)
        {
            gameState = GameState.WhiteMove;
            gameHistory = new GameHistory();

            takenPieces = new List<Piece>[Enum.GetValues(typeof(PlayerColour)).Length];
            foreach (PlayerColour colour in Enum.GetValues(typeof(PlayerColour)))
            {
                takenPieces[(int)colour] = new List<Piece>();
            }
            this.board = board;

            gameHistory.Add(board.OutputPieces());

            GenerateMoves();
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

        public void OfferDraw(PlayerColour colour)
        {

        }

        

        public void Surrender(PlayerColour colour)
        {
            MoveMade?.Invoke(this, new MadeMoveEventArgs(MoveResult.MoveRepetition));
        }

        protected virtual void OnMadeMove()
        {
            gameHistory.Add(board.OutputPieces());

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
