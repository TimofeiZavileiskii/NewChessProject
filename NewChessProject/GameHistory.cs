using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameHistory
    {
        Stack<Board> previousPositions;
        public GameHistory()
        {
            previousPositions = new Stack<Board>();
        }

        public void Add(Board addedPosition)
        {
            previousPositions.Push(addedPosition);
        }

        public bool CheckPositionRepetition(int maxPositions)
        {
            int repetitions = 0;
            Board currentPosition = previousPositions.Peek();
            foreach(Board previousPosition in previousPositions)
            {
                if (previousPosition == currentPosition)
                    repetitions++;
            }
            Console.WriteLine(repetitions);
            return !(repetitions < maxPositions);
        }

        public Board ReverseMove()
        {
            previousPositions.Pop();
            previousPositions.Pop();
            return previousPositions.Peek();
        }
    }
}
