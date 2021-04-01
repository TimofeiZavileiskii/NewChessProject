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

        public AIPlayer(PlayerColour colour, ChessEngine engine, Game game, int difficulty) : base(colour, game)
        {
            this.engine = engine;
            this.difficulty = difficulty;
        }

        private void ListList(List<AiMove> moves)
        {
            foreach(AiMove move in moves)
            {
                Console.WriteLine(move.Quality);
            }
        }
        
        AiMove ChooseMove(List<AiMove> moves)
        {
            AiMove output = new AiMove();

            int factor = -1;
            if (colour == PlayerColour.Black)
                factor = 1;


            moves = moves.OrderBy(x => (x.Quality * factor)).ToList();
            double random = moves.Count() / difficulty;


            return output;
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
                        AiMove mov = engine.EvaluateMove(piece.Position, move);
                        output.Add(mov);
                    }
                }
            }
            

            return output;
        }

        public void GameStarted(object sender, EventArgs eventArgs)
        {
            if(colour == PlayerColour.White)
                MakeMove();
        }

        private void MakeMove()
        {
            engine.UploadPosition(game.GenerateFENPosition().FENString);
            AiMove chosenMove;
            
            if (difficulty == 1)
            {
                chosenMove = engine.GetBestMove();
            }
            else
            {
                chosenMove = ChooseMove(GenerateMoves());

            }
            game.EnterMove(colour, chosenMove.Move.Item1, chosenMove.Move.Item2);
        }

        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            if(e.PlayerToMove == colour)
                MakeMove();
        }

    }
}
