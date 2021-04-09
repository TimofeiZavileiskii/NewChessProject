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

        private void DrawGame(Game game)
        {
            GameRepresentation gr = new GameRepresentation(game.GetPieceRepresentations());

            guiBoard.Update(gr);
        }

        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            DrawGame((Game)sender);
        }

        public void OnGameEnded(object sender, EventArgs e)
        {
            DrawGame((Game)sender);
        }

    }
}
