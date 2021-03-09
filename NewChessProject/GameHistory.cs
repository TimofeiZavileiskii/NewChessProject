using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameHistory
    {
        Stack<List<PieceRepresentation>> previousMoves;
        public GameHistory()
        {
            previousMoves = new Stack<List<PieceRepresentation>>();
        }

        public void Add(List<PieceRepresentation> addedMove)
        {
            previousMoves.Push(addedMove);
        }

        public bool CheckPositionRepetition(int maxPositions)
        {
            int repetitions = 0;
            List<PieceRepresentation> currentPosition = previousMoves.Peek();
            for(int i = previousMoves.Count() - 2; i > 0 && repetitions < maxPositions; i--)
            {
                if(currentPosition == previousMoves)
                    repetitions++;

                checkedPosition = checkedPosition.PreviousNode;
            }
            Console.WriteLine(repetitions);
            return !(repetitions < maxPositions);
        }

        public List<PieceRepresentation> ReverseMove()
        {
            List<PieceRepresentation> output = currentMove.BoardPosition;
            currentMove = currentMove.PreviousNode;

            return output;
        }
    }
}
