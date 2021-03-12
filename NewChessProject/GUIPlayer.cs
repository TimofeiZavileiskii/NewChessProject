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

    class GUIPlayer : Player
    {
        Vector selectedPiece;
        List<Vector> allowedPositions;
        State state;
        Vector check;
        GUIBoard guiBoard;

        delegate void StateTransition(Vector position);
        (StateTransition, State)[,] stateMachine;

        public GUIPlayer(PlayerColour colour, Game game, GUIBoard guiBoard) : base(colour, game)
        {
            allowedPositions = new List<Vector>();
            this.guiBoard = guiBoard;
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
                guiBoard.ShowPieceSelection(Colour);
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
            if (state == State.WaitForMove)
            {
                state = State.SelectPiece;
                GameRepresentationUpdated();

                if (e.Result == MoveResult.Check)
                    check = game.GetKingsPosition(Colour);
            }
            else
            {
                state = State.WaitForMove;
            }
        }

        public void RequestDraw(object sender, EventArgs e)
        {
            if (state != State.WaitForMove)
            {
                game.Resign(Colour);
            }
        }

        public void TakeBackProposed(object sender, EventArgs e)
        {

        }

        public void RequestTakeback(object sender, EventArgs e)
        {
            if (state != State.WaitForMove)
            {
                game.Resign(Colour);
            }
        }

        public void GameEnded(object sender, GameEndedEventArgs e)
        {
            if (state == State.WaitForMove)
            {
                ResetMove();
                GameRepresentationUpdated();

                guiBoard.EndGame(this, e);
            }
            else
            {
                state = State.WaitForMove;
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

        public void Resign(object sender, EventArgs e)
        {
            if(state != State.WaitForMove)
            {
                game.Resign(Colour);
            }
        }

        private void GameRepresentationUpdated()
        {
            GameRepresentation gr = new GameRepresentation(game.GetPieceRepresentations(), GenerateMoveTiles(), GenerateBoardHilights());

            guiBoard.Update(gr);
        }

        private List<BoardIndicator> GenerateBoardHilights()
        {
            List<BoardIndicator> output = new List<BoardIndicator>();

            if (selectedPiece != new Vector(-1, -1))
                output.Add(new BoardIndicator(selectedPiece, Color.FromRgb(50, 230, 50)));

            return output;
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
            }
        }

    }
}
