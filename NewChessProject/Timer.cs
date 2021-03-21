using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewChessProject
{
    class Timer
    {
        double timeStarted;
        double timeOfLastReport;
        double totalTime;
        double reportPeriod;

        PlayerColour owningPlayer;
        bool working;
        Thread timingThread;

        //Event will be called when time specified in constructor will pass
        public event EventHandler TimePassed;

        public double TimeLeft
        {
            get
            {
                double output = totalTime;
                if (working)
                    output -= PassedTime();
                return output / TimeSpan.TicksPerSecond;
            }
        }

        private double PassedTime()
        {
            return DateTime.Now.Ticks - timeStarted;
        }

        public PlayerColour Owner
        {
            get
            {
                return owningPlayer;
            }
        }

        public Timer(double reportPeriod, double totalTime, PlayerColour colour)
        {
            working = false;
            this.totalTime = totalTime * TimeSpan.TicksPerSecond;
            owningPlayer = colour;
            this.reportPeriod = reportPeriod;
            timeOfLastReport = DateTime.Now.Ticks;

            timingThread = new Thread(TrackTime);
            timingThread.Start();
        }

        ~Timer()
        {
            timingThread.Abort();
        }

        public void Add(double addedTime)
        {
            totalTime += addedTime * TimeSpan.TicksPerSecond;
        }

        public void Start()
        {
            timeStarted = DateTime.Now.Ticks;
            working = true;
        }

        public void Stop()
        {
            working = false;
            totalTime = totalTime - PassedTime();
        }

        private void TrackTime()
        {
            while(true)
            {
                if (working)
                {
                    if(DateTime.Now.Ticks - timeOfLastReport > reportPeriod)
                    {
                        timeOfLastReport = DateTime.Now.Ticks;
                        TimePassed(this, EventArgs.Empty);
                    }
                }
            }
        }



    }
}
