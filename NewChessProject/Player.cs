using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    abstract class Player : Participant
    {
        protected PlayerColour colour;
        protected Game game;

        public Player(PlayerColour colour, Game game)
        {
            this.colour = colour;
            this.game = game;
        }

        public PlayerColour Colour
        {
            get
            {
                return colour;
            }
        }
        public abstract void RequestSend(object sender, RequestMadeEventArgs e);

        public virtual void GameEnded(object sender, GameEndedEventArgs e)
        {

        }

    }
}
