using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameHistory
    {
        Stack<FENPosition> previousPositions;
        public GameHistory()
        {
            previousPositions = new Stack<FENPosition>();
        }

        public void Add(FENPosition addedPosition)
        {
            previousPositions.Push(addedPosition);
        }

        private bool CompareFENPositions(FENPosition p1, FENPosition p2)
        {
            if (p1.Position == p2.Position && p1.Castling == p2.Castling && p1.EnPassante == p2.EnPassante)
                return true;
            return false;
        }

        public bool CheckPositionRepetition(int maxPositions)
        {
            int repetitions = 0;
            FENPosition currentPosition = previousPositions.Peek();
            foreach(FENPosition previousPosition in previousPositions)
            {
                if (CompareFENPositions(previousPosition, currentPosition))
                    repetitions++;
            }
            Console.WriteLine(repetitions);
            return !(repetitions < maxPositions);
        }

        public FENPosition ReverseMove()
        {
            previousPositions.Pop();
            previousPositions.Pop();
            return previousPositions.Peek();
        }
    }
}
