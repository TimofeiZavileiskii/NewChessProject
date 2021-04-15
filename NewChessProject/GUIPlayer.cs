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

    enum AssistanceLevel
    {
        NoAssistance,
        HilightMoves,
        HilightThreats,
        HilightDefences
    }

    enum State
    {
        SelectMove,
        SelectPiece,
        SlectPawnTransformation,
        WaitForRequestAnswer,
        WaitForMove
    }

    enum HilightType
    {
        SafeMove,
        DefendedMove,
        ThreatenedMove,
        Check,
        SelectedPiece
    }


    class GUIPlayer : HumanPlayer
    {
        GUIBoard guiBoard;

        delegate void StateTransition(Vector position);
        (StateTransition, State)[,] stateMachine;

        Vector selectedPiece;
        List<Vector> allowedPositions;
        State state;
        Vector check;
        AssistanceLevel assistance;

        bool flipBoard;
        bool oneHumanPlayer;


        public GUIPlayer(PlayerColour colour, Game game, GUIBoard guiBoard, HumanSettings humanSettings) : base(colour, game)
        {
            allowedPositions = new List<Vector>();
            this.guiBoard = guiBoard;
            this.flipBoard = humanSettings.FlipBoard;
            assistance = humanSettings.Assistance;
            check = Vector.NullVector;
            selectedPiece = Vector.NullVector;
            CreateStateMachine();
            if (colour == PlayerColour.White)
            {
                state = State.SelectPiece;
                SetMySelfAsGUIUser();
            }
            else
            {
                state = State.WaitForMove;
            }
        }

        public void StartGame(object sender, GameStartEventArgs e)
        {
            oneHumanPlayer = e.OneHumanPlayer;
            if (e.OneHumanPlayer && colour == PlayerColour.Black)
            {
                GameRepresentationUpdated();
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

            stateMachine[(int)State.WaitForRequestAnswer, (int)Input.ClickNothing] = (null, State.WaitForRequestAnswer);
            stateMachine[(int)State.WaitForRequestAnswer, (int)Input.ClickYourPiece] = (null, State.WaitForRequestAnswer);
            stateMachine[(int)State.WaitForRequestAnswer, (int)Input.ClickValidMove] = (null, State.WaitForRequestAnswer);

            stateMachine[(int)State.WaitForMove, (int)Input.ClickNothing] = (null, State.WaitForMove);
            stateMachine[(int)State.WaitForMove, (int)Input.ClickYourPiece] = (null, State.WaitForMove);
            stateMachine[(int)State.WaitForMove, (int)Input.ClickValidMove] = (ThrowException, State.WaitForMove);
        }

        private void DiselectPiece(Vector vec)
        {
            selectedPiece = Vector.NullVector;
            allowedPositions.Clear();
            GameRepresentationUpdated();
        }

        private void ResetMove()
        {
            allowedPositions.Clear();
            selectedPiece = Vector.NullVector;
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
        }

        private void SetMySelfAsGUIUser()
        {
            guiBoard.SetNewUser(OnBoardClicked, OnWindowClicked, PieceSelected, Resign, RequestTakeback, RequestDraw);
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
            if(assistance != AssistanceLevel.NoAssistance)
            {
                foreach (Vector vec in allowedPositions)
                {
                    HilightType tileType = HilightType.SafeMove; //All moves have green colour
                    if (assistance != AssistanceLevel.HilightMoves && game.CheckForAttackFrom(Board.ReverseColour(Colour), selectedPiece, vec))
                    {
                        tileType = HilightType.ThreatenedMove; //Moves under attack get red colour
                        if (assistance != AssistanceLevel.HilightThreats && game.CheckForDefence(Colour, selectedPiece, vec))
                            tileType = HilightType.DefendedMove; //Moves under attack and under defence get yellow colour
                    }
                    output.Add(new BoardIndicator(vec, tileType));
                }
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
            check = Vector.NullVector;
            PlayerColour checkColour = colour;

            if (state == State.WaitForMove)
            {
                state = State.SelectPiece;
                SetMySelfAsGUIUser();
            }
            else
            {
                state = State.WaitForMove;
                checkColour = Board.ReverseColour(checkColour);
            }

            if (e.Result == MoveResult.Check)
            {
                check = game.GetKingsPosition(checkColour);
            }

            if (oneHumanPlayer || state != State.WaitForMove)
            {
                ResetMove();
                GameRepresentationUpdated();
            }
        }

        public void RequestDraw(object sender, EventArgs e)
        {
            game.OfferDraw(Colour);
        }

        public override void RequestSend(object sender, RequestMadeEventArgs e)
        {
            if(e.Request.ToWhichPlayer == Colour)
                guiBoard.MakeRequest(e.Request);
        }

        public void RequestTakeback(object sender, EventArgs e)
        {
            bool result = game.TakeBack(Colour);
            GameRepresentationUpdated();
            if (result)
                state = State.SelectPiece;

        }

        override public void GameEnded(object sender, GameEndedEventArgs e)
        {
            if(state != State.WaitForMove)
            {
                state = State.WaitForMove;
            }
            ResetMove();
            GameRepresentationUpdated();
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
            game.Resign(Colour);
        }

        private void GameRepresentationUpdated()
        {
            GameRepresentation gr = new GameRepresentation(game.GetPieceRepresentations(), GenerateMoveTiles(), GenerateBoardHilights());

            guiBoard.FlipBoard = (flipBoard && colour == PlayerColour.Black);
            guiBoard.Update(gr);
        }

        private List<BoardIndicator> GenerateBoardHilights()
        {
            List<BoardIndicator> output = new List<BoardIndicator>();

            if (selectedPiece != Vector.NullVector)
                output.Add(new BoardIndicator(selectedPiece, HilightType.SelectedPiece));
            if (check != Vector.NullVector)
                output.Add(new BoardIndicator(check, HilightType.Check));

            return output;
        }
        public void OnWindowClicked(object sender, EventArgs e)
        {
            InputAction(Vector.NullVector, Input.ClickNothing);
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
