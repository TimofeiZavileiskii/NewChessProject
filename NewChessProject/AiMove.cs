using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    struct AiMove
    {
        private PieceType? chosenPawnTransformation;
        private (Vector, Vector) move;

        public (Vector, Vector) Move
        {
            get
            {
                return move;
            }
        }

        public PieceType? ChosenPawnTransformation
        {
            get
            {
                return chosenPawnTransformation;
            }
        }

        public AiMove((Vector, Vector) move, PieceType? choosePieceType = null)
        {
            chosenPawnTransformation = choosePieceType;
            this.move = move;
        }

    }
}
