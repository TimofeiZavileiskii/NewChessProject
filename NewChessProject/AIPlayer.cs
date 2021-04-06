using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class AIPlayer : Player
    {
        ChessEngine engine;

        Dispatcher mainThreadDispatcher;
        bool analysing;

        public AIPlayer(PlayerColour colour, ChessEngine engine, Game game) : base(colour, game)
        {
            this.engine = engine;
            mainThreadDispatcher = Dispatcher.CurrentDispatcher;
            analysing = false;
        }
        

        public void GameStarted(object sender, EventArgs eventArgs)
        {
            if(colour == PlayerColour.White)
                MakeMove();
        }

        private void MakeMove()
        {
            engine.UploadPosition(game.GenerateFENPosition().FENString);

            Thread thread = new Thread(() =>
            {
                AiMove chosenMove = engine.GetBestMove();

                mainThreadDispatcher.Invoke(() =>
                {
                    EnterResult result = game.EnterMove(colour, chosenMove.Move.Item1, game.GetAllowedPositions(colour, chosenMove.Move.Item1).Find(x => x == chosenMove.Move.Item2));
                    if (result == EnterResult.WaitForPawnSlection)
                    {
                        game.ChoosePawnTransformation(colour, (PieceType)chosenMove.ChosenPawnTransformation);
                    }
                });
            });
            thread.Start();
        }



        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            if(e.PlayerToMove == colour)
                MakeMove();
        }

        public override void RequestSend(object sender, RequestMadeEventArgs e)
        {
            switch (e.Request.Type)
            {
                case(RequestType.ProposeTakeback):
                    e.Request.Agreed = true;
                    break;
                case (RequestType.OfferDraw):
                    e.Request.Agreed = EvaluateDraw();
                    break;
            }
        }

        private bool EvaluateDraw()
        {
            bool output = false;
            engine.UploadPosition(game.GenerateFENPosition().FENString);
            int assesment = engine.EvaluatePosition();

            int factor = 1;
            if (colour == PlayerColour.Black)
                factor = -1;

            if (assesment * factor < -2)
            {
                output = true;
            }
            return output;
        }

        private Vector IdentifySpecialMove(Vector piece, Vector target)
        {
            PieceType type = game.GetPieceType(piece);
            SpecialMove specailMove = SpecialMove.Nothing;

            switch (type)
            {
                case PieceType.Pawn:
                    if(Math.Abs(piece.Y - target.Y) == 2)
                        specailMove = SpecialMove.DoubleFoward;
                    if(Math.Abs(piece.X - target.X) == 1 && !game.PiecePresent(colour, target))
                        specailMove = SpecialMove.EnPassante;

                    break;
                case PieceType.King:
                    if (piece.X - target.X == 2)
                        specailMove = SpecialMove.CastleLeft;
                    if(piece.X - target.X == -2)
                        specailMove = SpecialMove.CastleRight;

                    break;
            }

            return new Vector(target.X, target.Y, specailMove);
        }

    }
}
