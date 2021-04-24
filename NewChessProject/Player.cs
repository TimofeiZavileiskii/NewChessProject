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
        public PlayerColour Colour
        {
            get
            {
                return colour;
            }
        }

        public Player(PlayerColour colour, Game game)
        {
            this.colour = colour;
            this.game = game;
        }

        public virtual void StartGame(object sender, GameStartEventArgs e)
        {

        }

        //All players must be able to respond to requests
        public abstract void RequestSend(object sender, RequestMadeEventArgs e);

        public virtual void GameEnded(object sender, GameEndedEventArgs e)
        {

        }
    }
}
