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

        private bool ComparePositions(List<PieceRepresentation> list1, List<PieceRepresentation> list2)
        {
            bool output = true;

            foreach (PieceRepresentation pr in list1)
            {
                if (!list2.Contains(pr))
                {
                    output = false;
                    break;
                }
            }
            
            return (list1.Count == list2.Count) && output;
        }

        public bool CheckPositionRepetition(int maxPositions)
        {
            int repetitions = 0;
            List<PieceRepresentation> currentPosition = previousPositions.Peek();
            foreach(List<PieceRepresentation> previousPosition in previousPositions)
            {
                if (ComparePositions(previousPosition, currentPosition))
                    repetitions++;
            }
            Console.WriteLine(repetitions);
            return !(repetitions < maxPositions);
        }

        public List<PieceRepresentation> ReverseMove()
        {
            previousPositions.Pop();
            return previousPositions.Peek();
        }
    }
}
