using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class GameViewer : Participant
    {
        GUIBoard guiBoard;
        public GameViewer(GUIBoard guiBoard)
        {
            this.guiBoard = guiBoard;
        }

        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            GameRepresentation gr = new GameRepresentation(((Game)sender).GetPieceRepresentations());

            guiBoard.Update(gr);
        }

    }
}
