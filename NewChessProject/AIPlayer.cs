using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class AIPlayer : Player
    {
        ChessEngine engine;
        int difficulty; // Should range from 1 to 10 - it will define the accuracy with which the moves will be chosen by the player

        public AIPlayer(PlayerColour colour, ChessEngine engine, Game game) : base(colour, game)
        {
            this.engine = engine;
        }
        

        public void GameStarted(object sender, EventArgs eventArgs)
        {
            if(colour == PlayerColour.White)
                MakeMove();
        }

        private void MakeMove()
        {
            engine.UploadPosition(game.GenerateFENPosition().FENString);
            AiMove chosenMove = engine.GetBestMove();

            game.EnterMove(colour, chosenMove.Move.Item1, IdentifySpecialMove(chosenMove.Move.Item1, chosenMove.Move.Item2));
        }

        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            if(e.PlayerToMove == colour)
                MakeMove();
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
