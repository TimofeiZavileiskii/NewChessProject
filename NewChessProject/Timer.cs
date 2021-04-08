using System;
using System.Threading;
using System.Windows.Threading;
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
        Dispatcher reportCaller;

        //Event will be called when time specified in constructor will pass
        public event EventHandler OnTimePassed;

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
        
        private void TimePassed()
        {
            if(OnTimePassed != null)
                OnTimePassed(this, EventArgs.Empty);
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
            reportCaller = Dispatcher.CurrentDispatcher;
            working = false;
            this.totalTime = totalTime * TimeSpan.TicksPerSecond;
            owningPlayer = colour;
            this.reportPeriod = reportPeriod;
            timeOfLastReport = DateTime.Now.Ticks;

            timingThread = new Thread(TrackTime);
            timingThread.Start();
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

        public void Terminate()
        {
            timingThread.Abort();
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
                        reportCaller.Invoke(() =>
                        {
                            TimePassed();
                        });               
                    }
                }
            }
        }



    }
}
