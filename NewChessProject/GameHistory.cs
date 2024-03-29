﻿using System;
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
            return p1.Position == p2.Position && p1.Castling == p2.Castling && p1.EnPassante == p2.EnPassante && p1.CurrentPlayer == p2.CurrentPlayer;
        }

        public void ListGameHistory()
        {
            for(int i = 0; i < previousPositions.Count; i++)
            {
                FENPosition pos = previousPositions[i];
                Console.WriteLine(pos.Position + " " + pos.CurrentPlayer + " " + pos.Castling + " " + pos.EnPassante);
            }
        }

        public bool CheckPositionRepetition(int maxPositions)
        {
            int repetitions = 0;
            FENPosition currentPosition = previousPositions.Peek();
            for(int i = 0; i < previousPositions.Count; i++) 
            {
                if (CompareFENPositions(previousPositions[i], currentPosition))
                    repetitions++;
            }
            if(!(repetitions < maxPositions))
            {
                Console.WriteLine("Break");
            }

            return !(repetitions < maxPositions);
        }

        public FENPosition ReverseMove(int movesToGoBack)
        {
            if (previousPositions.Count > movesToGoBack)
            {
                for (int i = 0; i < movesToGoBack; i++)
                {
                    previousPositions.Pop();
                }

                return previousPositions.Peek();
            }
            else
            {
                return null;
            }
        }
    }
}
