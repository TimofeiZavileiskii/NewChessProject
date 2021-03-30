using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class AIPlayer : Player
    {
        ChessEngine engine;

        public AIPlayer(PlayerColour colour, ChessEngine engine, Game game) : base(colour, game)
        {
            this.engine = engine;
        }

    }
}
