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
            for(int i = 0; i > previousMoves.Count(); i++)
            {
                if(currentPosition == position.PreviousNode)
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
