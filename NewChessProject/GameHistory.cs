using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameHistory
    {
        Stack<string> previousPositions;
        public GameHistory()
        {
            previousPositions = new Stack<string>();
        }

        public void Add(String addedPosition)
        {
            previousPositions.Push(addedPosition);
        }

        public bool CheckPositionRepetition(int maxPositions)
        {
            int repetitions = 0;
            string currentPosition = previousPositions.Peek();
            foreach(string previousPosition in previousPositions)
            {
                if (previousPosition == currentPosition)
                    repetitions++;
            }
            Console.WriteLine(repetitions);
            return !(repetitions < maxPositions);
        }

        public string ReverseMove()
        {
            previousPositions.Pop();
            previousPositions.Pop();
            return previousPositions.Peek();
        }
    }
}
