using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    abstract class ManyStepPiece : Piece
    {
        public ManyStepPiece(PlayerColour colour, bool hasMoved = false) : base(colour, hasMoved)
        {

        }

        override public void GenerateMoves(Board board, Vector position)
        {
            availableMoves.Clear();

            foreach (Vector pos in directions)
            {
                bool finish = false;
                Vector checkedPosition = position;
                while (!finish)
                {
                    checkedPosition = checkedPosition + pos;
                    if (CheckInRange(checkedPosition))
                    {
                        if (board[checkedPosition] != null)
                        {
                            finish = true;
                            if (board[checkedPosition].Colour != Colour)
                            {
                                availableMoves.Add(checkedPosition);
                            }
                        }
                        else
                        {
                            availableMoves.Add(checkedPosition);
                        }
                    }
                    else
                    {
                        finish = true;
                    }
                }
            }
        }
    }
}
