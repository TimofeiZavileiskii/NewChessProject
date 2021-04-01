using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    struct AiMove
    {
        private int quality;
        private (Vector, Vector) move;

        public int Quality
        {
            get
            {
                return quality;
            }
        }

        public (Vector, Vector) Move
        {
            get
            {
                return move;
            }
        }

        public AiMove(int quality, (Vector, Vector) move)
        {
            this.quality = quality;
            this.move = move;
        }

    }
}
