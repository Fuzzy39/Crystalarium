using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Util.Timekeeping
{
    /// <summary>
    /// A duration represents an event that can be timed.
    /// </summary>
    public abstract class Duration
    {
        private string _name;

        Queue<TimeSpan> previousDurations;
        private int averageSpan;
        protected TimeSpan lengthThisFrame;

        internal TimeSpan AverageLength
        {
            get
            {
                TimeSpan toReturn = new TimeSpan();
                foreach (TimeSpan t in previousDurations)
                {
                    toReturn += t;
                }

                return toReturn / previousDurations.Count;

            }
        }

        /// <summary>
        /// The Average length, formatted in a human readable form.
        /// </summary>
        internal string FormattedLength
        {
            get
            {
                return Util.FormatTime(AverageLength);
            }
        }
        
        /// <summary>
        /// The length of this duration for the current frame, currently.
        /// </summary>
        internal TimeSpan LengthThisFrame
        {
            get { return lengthThisFrame; }
        }

        public string Name
        {
            get { return _name; }
        }

        internal Duration(string name, int averageSpan)
        {
            _name = name;
            previousDurations = new Queue<TimeSpan>();
            this.averageSpan = averageSpan;
            lengthThisFrame = new TimeSpan();
        }

        /// <summary>
        /// Saves the time taken for the current frame.
        /// </summary>
        internal virtual void Reset()
        {
            if (previousDurations.Count == averageSpan)
            {
                previousDurations.Dequeue();
            }

            previousDurations.Enqueue(lengthThisFrame);
            lengthThisFrame = new TimeSpan();


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Total">The total time used on average during the averaging period.</param>
        /// <returns></returns>
        internal virtual string CreateReport(TimeSpan Total)
        {
            return Name + ": " + FormattedLength + " (" + Math.Round((AverageLength / Total)*100, 1) + "%)";
        }

    }
}
