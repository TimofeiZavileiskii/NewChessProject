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
        public bool Agreed { get; set; }

        public string Text{ 
            get { return text; } 
        }

        public string Title
        {
            get { return title; }
        }

        public Request(RequestType type, string text, string title = "")
        {
            this.text = text;
            this.title = title;
            this.type = type;
        }

    }
}
