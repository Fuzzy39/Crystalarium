using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Util.Timekeeping
{
    /// <summary>
    /// The Timekeeper class is for diagnostics
    /// I don't have a plan for how it should work...
    /// </summary>
    public class Timekeeper
    {
        // The timekeeper class keeps track of all durations, and averages them for a number of frames.
        private List<Duration> durations;
        private List<Duration> children;

        private Stopwatch timer;
        private const int AVG_SIZE = 30; // The amount of frames to average over for calculations.
        private TimeSpan targetElapsed; // the time between frames, nominally.
        private Task total;

        public double FrameRate
        {
            get
            {
                if(FreeTime.Ticks>=0) // we are meeting our target!
                {
                    return 1 / targetElapsed.TotalSeconds;
                }

                return 1 / total.AverageLength.TotalSeconds; // not so much.
            }
        }

        public TimeSpan FreeTime
        {
            get 
            {
                return targetElapsed - total.AverageLength; // can be negative, if we're lagging.
            }
        }

        public TimeSpan TotalTime
        {
            get 
            {  

                if(total.AverageLength>targetElapsed)
                {
                    return total.AverageLength;
                }

                TimeSpan toReturn = new TimeSpan(); 

                foreach(Duration d in children)
                {
                    toReturn += d.AverageLength;
                }
                return toReturn;
            }
        }

        internal Timekeeper(TimeSpan targetElapsed)
        {
            durations = new List<Duration>();
            children = new List<Duration>();
         
            this.targetElapsed = targetElapsed;

            total = new Task("!", AVG_SIZE);
            total.Start(new TimeSpan());
            timer = new Stopwatch();
            timer.Start();
        }

        public void NextFrame()
        {
            total.Stop(timer.Elapsed);
            total.Reset();

            foreach(Duration d in children)
            {
                d.Reset();
            }

            timer.Reset();
            total.Start(new TimeSpan());
            timer.Start();
 

        }


        private Duration GetDuration(string name)
        {
            foreach (Duration duration in durations)
            {
                if(duration.Name == name)
                {
                    return duration;
                }
            }

           return null;
        }

        public Task CreateTask(string parent, string name)
        {
            return CreateTask( parent == "" ? null: GetWorkload(parent), name);
        }


        public Task CreateTask(Workload parent, string name)
        {
            if(GetDuration(name) != null)
            {
                throw new ArgumentException("Cannot create Task '" + name + "'. Another duration of the same name already exists.");
            }


            Task toReturn = new Task(name, AVG_SIZE);

            if (parent != null)
            {
                parent.AddChildren(toReturn);
            }
            else
            {
                children.Add(toReturn);
            }

            durations.Add(toReturn);
            return toReturn;

        }

        public Workload CreateWorkload(string parent, string name)
        {

            return CreateWorkload( parent == ""? null : GetWorkload(parent), name);
        }

        public Workload CreateWorkload(Workload parent, string name)
        {
            if (GetDuration(name) != null)
            {
                throw new ArgumentException("Cannot create Workload '" + name + "'. Another duration of the same name already exists.");
            }


            Workload toReturn = new Workload(name, AVG_SIZE);

            if (parent != null)
            {
                parent.AddChildren(toReturn);
            }
            else
            {
                children.Add(toReturn);
            }


            durations.Add(toReturn);
            return toReturn;
        }


        public void StartTask(string name)
        {
            StartTask(GetTask(name));
        }

        public void StartTask(Task t)
        {
            t.Start(timer.Elapsed);
        }

        public void StopTask(string name)
        {
            StopTask(GetTask(name));
        }

        public void StopTask(Task t)
        {
            t.Stop(timer.Elapsed);
        }



        public Task GetTask(string name)
        {
            Duration d = GetDuration(name);

            if (!(d is Task) || d == null)
            {
                throw new InvalidOperationException("No task of name '" + name + "'.");
            }

            return (Task)d;
        }

        public Workload GetWorkload(string name)
        {
            Duration d = GetDuration(name);

            if (!(d is Workload) || d == null)
            {
                throw new InvalidOperationException("No workload of name '" + name + "'.");
            }

            return (Workload)d;
        }

        public string CreateReport()
        {
            string toReturn = "--------TIMING REPORT----------\n" +
                "Average of the past " + AVG_SIZE + " frames: \n" +
                "FPS: " + Math.Round(FrameRate, 2) + "\n\n" +
                "Used Time: " + Util.FormatTime(TotalTime)+" ("+Math.Round((TotalTime/targetElapsed)*100, 1)+"%):";
              
            String s = "";
            foreach (Duration child in children)
            {
                s+=child.CreateReport(total.AverageLength)+"\n";
            }
            toReturn += Util.Indent(s);
            return toReturn+ "\rFree Time: "+Util.FormatTime(FreeTime)+ "\n-------------------------------";
        }


    }
}
