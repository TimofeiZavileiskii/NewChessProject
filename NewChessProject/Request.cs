using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    enum RequestType
    {
        OfferDraw,
        ProposeTakeback
    }

    class Request
    {
        string text;
        string title;
        RequestType type;
        PlayerColour toWhichPlayer;
        public bool Agreed { get; set; }

        public PlayerColour ToWhichPlayer
        {
            get { return toWhichPlayer; }
        }

        public string Text{ 
            get { return text; } 
        }

        public string Title
        {
            get { return title; }
        }

        public RequestType Type
        {
            get { return type; }
        }

        public Request(RequestType type, string text, PlayerColour toWhichPlayer, string title = "")
        {
            this.toWhichPlayer = toWhichPlayer;
            this.text = text;
            this.title = title;
            this.type = type;
        }

    }
}
