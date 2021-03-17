using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewChessProject
{
    class Timer
    {
        PlayerColour owningPlayer;
        double timeLeft;
        double timeSpanLength;
        double lastTime;
        double passedTime;
        bool working;
        Thread timingThread;

        //Event will be called when time specified in constructor will pass
        public event EventHandler TimePassed;

        public double TimeLeft
        {
            get
            {
                return timeLeft / TimeSpan.TicksPerSecond;
            }
        }

        public PlayerColour Owner
        {
            get
            {
                return owningPlayer;
            }
        }

        public Timer(double time, double totalTime, PlayerColour colour)
        {
            timeSpanLength = time;
            working = false;
            lastTime = DateTime.Now.Ticks;
            timeLeft = totalTime * TimeSpan.TicksPerMinute;
            owningPlayer = colour;

            timingThread = new Thread(TrackTime);
            timingThread.Start();
        }

        ~Timer()
        {
            timingThread.Abort();
        }

        public void Add(double addedTime)
        {
            timeLeft += addedTime;
        }

        public void Start()
        {
            lastTime = DateTime.Now.Ticks;
            working = true;
        }

        public void Stop()
        {
            working = false;
        }

        private void TrackTime()
        {
            while (true)
            {
                if (working)
                {
                    passedTime += (DateTime.Now.Ticks - lastTime);
                    timeLeft -= (DateTime.Now.Ticks - lastTime);
                    lastTime = DateTime.Now.Ticks;

                    if (passedTime > timeSpanLength * TimeSpan.TicksPerSecond)
                    {
                        passedTime = 0;
                        TimePassed(this, EventArgs.Empty);
                    }
                }
            }
        }



    }
}
