using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Util.Timekeeping
{
    /// <summary>
    /// A task is a duration used to measure a single event
    /// </summary>
    public class Task : Duration
    {

        
        private TimeSpan startTime;
        private bool running;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="averageSpan">The amount of frames to average to get a timing.</param>
        internal Task(string name, int averageSpan) : base(name, averageSpan)
        {
            startTime= new TimeSpan();
            running = false;
        }


        internal void Start(TimeSpan time)
        {
            if(running)
            {
                throw new InvalidOperationException("A Task cannot be started when it is already running");
            }

            startTime = time;
            running = true;

        }

        internal void Stop(TimeSpan time)
        {
            if(!running)
            {
                throw new InvalidOperationException("A Task cannot be stoppen when it is not running.");
            }

            lengthThisFrame += time - startTime;
            running = false;
        }

        internal override void Reset()
        {
            if (running)
            {
                throw new InvalidOperationException("Task '" + Name + "' was not stopped before the end of the frame.");
            }

            base.Reset();
        }
    }
}
