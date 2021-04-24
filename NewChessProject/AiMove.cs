using System;
using System.Collections.Generic;

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
