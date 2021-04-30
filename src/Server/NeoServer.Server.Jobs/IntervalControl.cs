using System;

namespace NeoServer.Server.Jobs
{
    public class IntervalControl
    {
        private int interval;
        private DateTime lastRun;

        public IntervalControl(int interval)
        {
            this.interval = interval;
        }

        public void MarkAsExecuted() => lastRun = DateTime.Now;
        public bool CanExecuteNow() => DateTime.Now >= lastRun.AddMilliseconds(interval);
    }
}
