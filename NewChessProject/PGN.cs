using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    static class PGN
    {
        static public Game ImportGame(string pgn)
        {
            Board board = new Board();
            board.SetDefaultBoardPosition();
            Game game = new Game(board);
            


        }

    }
}
