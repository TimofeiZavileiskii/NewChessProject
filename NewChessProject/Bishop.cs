using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewChessProject
{
    class Bishop : ManyStepPiece
    {
        public Bishop(PlayerColour color, bool hasMoved = false) : base(color, hasMoved)
        {
            Type = PieceType.Bishop;
            directions.Add(new Vector(1, 1));
            directions.Add(new Vector(1, -1));
            directions.Add(new Vector(-1, 1));
            directions.Add(new Vector(-1, -1));
        }
    }
}
