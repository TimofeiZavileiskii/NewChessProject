using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class Rook : ManyStepPiece
    {
        public Rook(PlayerColour color, bool hasMoved = false) : base(color, hasMoved)
        {
            Type = PieceType.Rook;
            directions.Add(new Vector(0, 1));
            directions.Add(new Vector(1, 0));
            directions.Add(new Vector(-1, 0));
            directions.Add(new Vector(0, -1));
        }
    }
}
