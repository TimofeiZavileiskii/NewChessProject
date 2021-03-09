using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameHistory
    {
        Stack<List<PieceRepresentation>> previousPositions;
        public GameHistory()
        {
            previousPositions = new Stack<List<PieceRepresentation>>();
        }

        public void Add(List<PieceRepresentation> addedMove)
        {
            previousPositions.Push(addedMove);
        }

        private bool ComparePositions()
        {

        }

        public bool CheckPositionRepetition(int maxPositions)
        {
            int repetitions = 0;
            List<PieceRepresentation> currentPosition = previousPositions.Peek();
            foreach(List<PieceRepresentation> previousPosition in previousPositions)
            {
                if (previousPosition == currentPosition)
                    repetitions++;
            }
            Console.WriteLine(repetitions);
            return !(repetitions < maxPositions + 1);
        }

        public List<PieceRepresentation> ReverseMove()
        {
            previousPositions.Pop();
            return previousPositions.Peek();
        }
    }
}
