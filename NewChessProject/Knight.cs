using System;
using System.Collections.Generic;

namespace NewChessProject
{
    class Knight : OneStepPiece
    {
        public Knight(PlayerColour color, bool hasMoved = false) : base(color, hasMoved)
        {
            Type = PieceType.Knight;
            directions.Add(new Vector(2, 1));
            directions.Add(new Vector(2, -1));
            directions.Add(new Vector(-2, 1));
            directions.Add(new Vector(-2, -1));
            directions.Add(new Vector(1, 2));
            directions.Add(new Vector(1, -2));
            directions.Add(new Vector(-1, -2));
            directions.Add(new Vector(-1, 2));
        }
    }
}
