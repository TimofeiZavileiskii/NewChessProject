using System;
using System.Collections.Generic;

namespace NewChessProject
{
    struct BoardIndicator
    {
        Vector vector;
        HilightType type;

        public BoardIndicator(Vector vector, HilightType type)
        {
            this.vector = vector;
            this.type = type;
        }

        public Vector Position
        {
            get
            {
                return vector;
            }
        }

        public HilightType Type
        {
            get
            {
                return type;
            }
        }


    }
}
