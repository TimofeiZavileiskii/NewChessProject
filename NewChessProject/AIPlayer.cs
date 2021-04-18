using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChessProject
{
    class AIPlayer : Player
    {
        const string stockFishAddress = @"stockfish_13_win_x64_bmi2\stockfish_13_win_x64_bmi2";

        ChessEngine engine;
        Thread readerThread;

        Dispatcher mainThreadDispatcher;

        public AIPlayer(PlayerColour colour, Game game, int difficulty, int timePerTurn) : base(colour, game)
        {
            engine = new ChessEngine(stockFishAddress, "Stockfish", difficulty, timePerTurn);
            mainThreadDispatcher = Dispatcher.CurrentDispatcher;
        }
        
        public void GameStarted(object sender, EventArgs eventArgs)
        {
            if(colour == PlayerColour.White)
                MakeMove();
        }

        private void MakeMove()
        {
            engine.UploadPosition(game.GetFENPosition().FENString);

            readerThread = new Thread(() =>
            {
                AiMove chosenMove = engine.GetBestMove();

                mainThreadDispatcher.Invoke(() =>
                {
                    EnterResult result = game.EnterMove(colour, chosenMove.Move.Item1, game.GetAllowedPositions(colour, chosenMove.Move.Item1).Find(x => x == chosenMove.Move.Item2));
                    if (result == EnterResult.WaitForPawnSlection)
                    {
                        game.ChoosePawnTransformation(colour, (PieceType)chosenMove.ChosenPawnTransformation);
                    }
                });
            });
            readerThread.Start();
        }

        public override void GameEnded(object sender, GameEndedEventArgs e)
        {
            if (readerThread != null)
            {
                readerThread.Abort();
            }
        }

        public override void OnMadeMove(object sender, MadeMoveEventArgs e)
        {
            if(e.PlayerToMove == colour)
                MakeMove();
        }

        public override void RequestSend(object sender, RequestMadeEventArgs e)
        {
            if (e.Request.ToWhichPlayer == Colour)
            {
                switch (e.Request.Type)
                {
                    case (RequestType.ProposeTakeback):
                        e.Request.Agreed = true;
                        break;
                    case (RequestType.OfferDraw):
                        e.Request.Agreed = EvaluateDraw();
                        break;
                }
            }
        }

        ~AIPlayer()
        {
            readerThread.Abort();
        }

        private bool EvaluateDraw()
        {
            bool output = false;
            engine.UploadPosition(game.GetFENPosition().FENString);
            int assesment = engine.EvaluatePosition();

            int factor = 1;
            if (colour == PlayerColour.Black)
                factor = -1;

            if (assesment * factor < -2)
            {
                output = true;
            }
            return output;
        }

    }
}
