using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    enum Input
    {
        ClickYourPiece,
        ClickValidMove,
        ClickNothing
    }

    enum State
    {
        SelectMove,
        SelectPiece,
        SlectPawnTransformation,
        WaitForMove
    }

    class PieceSelectedEventArgs : EventArgs
    {
        public PieceSelectedEventArgs(PieceType type)
        {
            SelectedPieceType = type;
        }

        public PieceType SelectedPieceType { get; set; }
    }

    class GameEndedEventArgs : EventArgs
    {
        public GameEndedEventArgs(MoveResult result)
        {
            GameResult = result;
        }

        public MoveResult GameResult { get; set; }
    }

    class GUIPlayer : Player
    {
        Vector selectedPiece;
        List<Vector> allowedPositions;
        State state;
        Vector check;


        delegate void StateTransition(Vector position);
        (StateTransition, State)[,] stateMachine;

        public event EventHandler<GUIBoardUpdateEventArgs> OnGameRepresentationUpdated;
        public event EventHandler OnPawnNeedsTransforemation;
        public event EventHandler<GameEndedEventArgs> OnGameEnded;

        public GUIPlayer(PlayerColour colour, Game game) : base(colour, game)
        {
            allowedPositions = new List<Vector>();
            CreateStateMachine();
            if(colour == PlayerColour.White)
            { 
                state = State.SelectPiece;
            }
            else
            {
                state = State.WaitForMove;
            }
        }
        
        private void CreateStateMachine()
        {
            stateMachine = new (StateTransition, State)[Enum.GetValues(typeof(State)).Length, Enum.GetValues(typeof(Input)).Length];

            stateMachine[(int)State.SelectPiece, (int)Input.ClickNothing] = (null, State.SelectPiece);
            stateMachine[(int)State.SelectPiece, (int)Input.ClickYourPiece] = (SelectPiece, State.SelectMove);
            stateMachine[(int)State.SelectPiece, (int)Input.ClickValidMove] = (ThrowException, State.SelectMove);

            stateMachine[(int)State.SelectMove, (int)Input.ClickNothing] = (DiselectPiece, State.SelectPiece);
            stateMachine[(int)State.SelectMove, (int)Input.ClickYourPiece] = (SelectPiece, State.SelectMove);
            stateMachine[(int)State.SelectMove, (int)Input.ClickValidMove] = (MakeMove, State.SelectPiece);

            stateMachine[(int)State.SlectPawnTransformation, (int)Input.ClickNothing] = (null, State.SlectPawnTransformation);
            stateMachine[(int)State.SlectPawnTransformation, (int)Input.ClickYourPiece] = (null, State.SlectPawnTransformation);
            stateMachine[(int)State.SlectPawnTransformation, (int)Input.ClickValidMove] = (null, State.SlectPawnTransformation);

            stateMachine[(int)State.WaitForMove, (int)Input.ClickNothing] = (null, State.WaitForMove);
            stateMachine[(int)State.WaitForMove, (int)Input.ClickYourPiece] = (null, State.WaitForMove);
            stateMachine[(int)State.WaitForMove, (int)Input.ClickValidMove] = (ThrowException, State.WaitForMove);
        }

        private void DiselectPiece(Vector vec)
        {
            selectedPiece = new Vector(-1, -1);
            allowedPositions.Clear();
            GameRepresentationUpdated();
        }

        private void ResetMove()
        {
            allowedPositions.Clear();
            selectedPiece = new Vector(-1, -1);
        }

        private void MakeMove(Vector vec)
        {
            EnterResult result = game.EnterMove(colour, selectedPiece, allowedPositions.Find(x => x == vec));
            if (result == EnterResult.WaitForPawnSlection)
            {
                state = State.SlectPawnTransformation;
                PawnNeedsTransformation();
            }

            ResetMove();
            GameRepresentationUpdated();
        }

        private void ThrowException(Vector vec)
        {
            throw new NotImplementedException("At this state no move should be selected");
        }

        private void SelectPiece(Vector vec)
        {
            selectedPiece = vec;
            allowedPositions = game.GetAllowedPositions(colour, vec);
            GameRepresentationUpdated();
        }

        private List<BoardIndicator> GenerateMoveTiles()
        {
            List<BoardIndicator> output = new List<BoardIndicator>();
            foreach(Vector vec in allowedPositions)
            {
                output.Add(new BoardIndicator(vec, Color.FromRgb(30, 220, 50))); //All moves have green colour
            }
            return output;
        }

        private void InputAction(Vector vector, Input input)
        {
            State originalState = state;

            state = stateMachine[(int)state, (int)input].Item2;
            if (stateMachine[(int)originalState, (int)input].Item1 != null)
            {
                stateMachine[(int)originalState, (int)input].Item1(vector);
            }
        }

        override public void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            if((e.Result == MoveResult.Continue || e.Result == MoveResult.Check))
            {
                if (state == State.WaitForMove)
                {
                    state = State.SelectPiece;
                    GameRepresentationUpdated();

                    if (e.Result == MoveResult.Check)
                        check = game.GetKingsPosition(Colour);
                }
                else if(state == State.SelectPiece)
                {
                    state = State.WaitForMove;
                }
            }
            else if(state == State.SelectPiece && (e.Result == MoveResult.Stalemate || 
                e.Result == MoveResult.Mate || e.Result == MoveResult.MoveRepetition))
            {
                ResetMove();
                GameRepresentationUpdated();
                state = State.WaitForMove;
                GameEnded(e.Result);
            }
        }

        public void OnBoardClicked(object sender, BoardClickedEventArgs e)
        {
            InputAction(e.Position, DetermineInput(e.Position));
        }

        private Input DetermineInput(Vector vector)
        {
            Input output = Input.ClickNothing;
            if (allowedPositions.Contains(vector))
            {
                output = Input.ClickValidMove;
            }
            if (game.PiecePresent(colour, vector))
            {
                output = Input.ClickYourPiece;
            }
            return output;
        }

        private void GameRepresentationUpdated()
        {
            GameRepresentation gr = new GameRepresentation(game.GetPieceRepresentations(), GenerateMoveTiles(), GenerateBoardHilights());

            if (OnGameRepresentationUpdated != null)
                OnGameRepresentationUpdated(this, new GUIBoardUpdateEventArgs(gr));
        }

        private List<BoardIndicator> GenerateBoardHilights()
        {
            List<BoardIndicator> output = new List<BoardIndicator>();

            if (selectedPiece != new Vector(-1, -1))
                output.Add(new BoardIndicator(selectedPiece, Color.FromRgb(50, 230, 50)));

            return output;
        }

        private void GameEnded(MoveResult result)
        {
            if(OnGameEnded != null)
                OnGameEnded(this, new GameEndedEventArgs(result));
        }

        private void PawnNeedsTransformation()
        {
            if (OnPawnNeedsTransforemation != null)
                OnPawnNeedsTransforemation(this, EventArgs.Empty);
        }

        public void OnWindowClicked(object sender, EventArgs e)
        {
            InputAction(new Vector(0,0), Input.ClickNothing);
        }

        public void PieceSelected(object sender, PieceSelectedEventArgs e)
        {
            if (state == State.SlectPawnTransformation)
            {
                game.ChoosePawnTransformation(colour, e.SelectedPieceType);
                state = State.SelectMove;
            }
        }

    }
}
