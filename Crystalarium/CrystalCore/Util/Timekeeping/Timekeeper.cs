using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Util.Timekeeping
{
    /// <summary>
    /// The Timekeeper class is for diagnostics...
    /// THERE IS ONLY ONE. It is foolish to meddle with Chronos, keeper of time.
    /// </summary>
    // Ahem. Excuse the outburst.
    public class Timekeeper
    {
        // there is only one keeper of time.
        private static Timekeeper timekeeper = null;

        // The timekeeper class keeps track of all durations, and averages them for a number of frames.
        private List<Duration> durations;
        private List<Duration> children;

        private Stopwatch timer;
        private const int AVG_SIZE = 30; // The amount of frames to average over for calculations.
        private TimeSpan targetElapsed; // the time between frames, nominally.
        private Task total; // the total time between now and the last frame.

        public static Timekeeper Instance
        {
            get
            {
                if (timekeeper == null)
                {
                    timekeeper = new Timekeeper();
                }
                return timekeeper;
            }
        }


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
                return targetElapsed - UsedTime; // can be negative, if we're lagging.
            }
        }

        public TimeSpan UsedTime
        {
            get 
            {  

                if(total.AverageLength>(targetElapsed + TimeSpan.FromMilliseconds(.5)))
                {
                    return total.AverageLength;
                }

                return AccountedTime;

            }
        }

        private TimeSpan AccountedTime // the time acounted for by durations
        {
            get
            {

                TimeSpan toReturn = new TimeSpan();

                foreach (Duration d in children)
                {
                    toReturn += d.AverageLength;
                }
                return toReturn;
            }
        }

        private Timekeeper()
        {
            durations = new List<Duration>();
            children = new List<Duration>();
         
            this.targetElapsed = TimeSpan.FromSeconds(1/60.0);

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

                if (GetDuration(name) is Task)
                {
                    return (Task)GetDuration(name);
                }

                throw new ArgumentException("Cannot create Task '" + name + "'. A Workload of the same name already exists.");
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
                if(GetDuration(name) is Workload)
                {
                    return (Workload)GetDuration(name);
                }

                throw new ArgumentException("Cannot create Workload '" + name + "'. A task of the same name already exists.");
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
                "Used Time: " + Util.FormatTime(UsedTime)+" ("+Math.Round((UsedTime/targetElapsed)*100, 1)+"%):";
              
            String s = "";
            foreach (Duration child in children)
            {
                s+=child.CreateReport(UsedTime)+"\n";
            }
            toReturn += Util.Indent(s);
            // this line is hideous
            return toReturn+ "Other: "+((UsedTime>AccountedTime)?Util.FormatTime(UsedTime-AccountedTime)+" ("+
                Math.Round(((UsedTime-AccountedTime) / UsedTime) * 100, 1) +"%)" :Util.FormatTime(new TimeSpan())+" (0%)")
                +"\nFree Time: "+Util.FormatTime(FreeTime)+" ("+Math.Round(FreeTime / total.AverageLength * 100, 1) + "%)\n-------------------------------";
        }


    }
}
