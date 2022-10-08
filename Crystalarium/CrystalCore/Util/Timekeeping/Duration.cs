using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Util.Timekeeping
{
    /// <summary>
    /// A duration represents an event that can be timed.
    /// </summary>
    public class Duration
    {
        private string _name;
        private TimeSpan startTime;

        public string Name
        {
            get { return _name; }
        }

        public Duration(string name, TimeSpan time)
        {
            _name = name;
            Reset(time);
        }

        public void Reset(TimeSpan time)
        {
            startTime = time;
        }

        public TimeSpan GetDuration(TimeSpan time)
        {
            return time - startTime;
        }

        public string GetFormattedDuration(TimeSpan time)
        {
            TimeSpan duration = GetDuration(time);
            
            if(duration.TotalDays>=1)
            {
                return duration.Days + "d " + duration.Hours+"h";
            }
            if(duration.TotalHours>=1)
            {
                return duration.Hours+"h "+duration.Minutes+"m";
            }
            if(duration.TotalMinutes>=1)
            {
                return duration.Minutes + "m "+duration.Seconds+"s";
            }
            if(duration.TotalSeconds>=1)
            {
                return duration.Seconds + "s " + duration.Milliseconds + "ms";
            }

            if(duration.TotalMilliseconds>=1)
            {
                return duration.Milliseconds + "ms " + ((duration.Ticks - (duration.Milliseconds * 10 * 1000))/10) + "us";
            }

            if(duration.Ticks!=0)
            {
                return Math.Round(duration.Ticks / 1000.0, 1) + "us";
            }

            return "0us";
            
        }

    }
}
