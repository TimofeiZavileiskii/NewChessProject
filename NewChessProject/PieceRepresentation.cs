using System;
using System.Collections.Generic;

namespace NewChessProject
{
    struct PieceRepresentation
    {
        Vector vector;
        PieceType type;
        PlayerColour colour;

        public PieceRepresentation(Vector vector, PieceType type, PlayerColour colour)
        {
            this.vector = vector;
            this.type = type;
            this.colour = colour;
        }

        public Vector Position
        {
            get
            {
                return vector;
            }
        }

        public PieceType Type
        {
            get
            {
                return type;
            }
        }

        public PlayerColour Colour
        {
            get
            {
                return colour;
            }
        }
    }
}
