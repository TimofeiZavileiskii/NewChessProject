using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class AIPlayer : Player
    {
        ChessEngine engine;
        int difficulty; // Should range from 1 to 10 - it will define the accuracy with which the moves will be chosen by the player

        public AIPlayer(PlayerColour colour, ChessEngine engine, Game game, int difficulty) : base(colour, game)
        {
            this.engine = engine;
            this.difficulty = difficulty;
        }

        
        AiMove ChooseMove(List<AiMove> moves)
        {
            moves.OrderBy(x => x.Quality);
            double random = moves.Count() / difficulty;

            return new AiMove();
        }

        List<AiMove> GenerateMoves()
        {
            List<AiMove> output = new List<AiMove>();

            List<PieceRepresentation> pieces = game.GetPieceRepresentations();

            foreach (PieceRepresentation piece in pieces)
            {
                if (piece.Colour == colour)
                {
                    List<Vector> moves = game.GetAllowedPositions(Colour, piece.Position);
                    foreach (Vector move in moves)
                    {
                        output.Add(engine.EvaluateMove(piece.Position, move));
                    }
                }
            }
            return output;
        }

        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            AiMove chosenMove = ChooseMove(GenerateMoves());

            game.EnterMove(colour, chosenMove.Move.Item1, chosenMove.Move.Item2);

        }

    }
}
