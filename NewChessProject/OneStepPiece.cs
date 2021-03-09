using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class OneStepPiece : Piece
    {
        public OneStepPiece(PlayerColour colour, bool hasMoved = false) : base(colour)
        {

        }

        override public void GenerateMoves(Board board, Vector position)
        {
            availableMoves.Clear();

            foreach (Vector pos in directions)
            {
                Vector checkedPosition = position;
                checkedPosition += pos;

                if (CheckInRange(checkedPosition))
                {
                    if (board[checkedPosition] != null)
                    {
                        if (board[checkedPosition].Colour == Colour)
                        {
                            continue;
                        }
                    }
                    availableMoves.Add(checkedPosition);
                }
            }
        }
    }
}
