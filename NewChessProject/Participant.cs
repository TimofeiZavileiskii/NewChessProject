using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    abstract class Participant
    {
        public Participant()
        {

        }

        virtual public void OnMadeMove(object sender, MadeMoveEventArgs e)
        {

        }
    }
}
