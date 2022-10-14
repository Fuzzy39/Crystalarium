using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Util.Timekeeping
{

    /// <summary>
    /// A Workload is a duration consisting of many other durations
    /// </summary>
    public class Workload : Duration
    {
        List<Duration> children;


        internal override TimeSpan LengthThisFrame
        {
            get
            {
                TimeSpan toReturn = TimeSpan.Zero;
                foreach(Duration d in children)
                {
                    toReturn += d.LengthThisFrame;
                }
                return toReturn;
            }
        }

        internal Workload(string name, int averageSpan, params Duration[] toAdd) : base(name, averageSpan)
        {

            children = new List<Duration>();
            AddChildren(toAdd); 
          
        }

        /// <summary>
        /// Note that circular references will break the system. A workload should not be a child of itself.
        /// I am, however, to lazy to write any code to detect this state. 
        /// </summary>
        /// <param name="toAdd"></param>
        internal void AddChildren(params Duration[] toAdd)
        {
            foreach (Duration d in toAdd)
            {
                
                children.Add(d);
            }
        }

        internal Duration GetChild(string name)
        {
            foreach(Duration d in children)
            {
                if (d.Name == name)
                {
                    return d;
                }
            }
            return null;
        }

        internal override void Reset()
        {

            // important!
            // This must occur before our children get reset.
            lengthThisFrame = LengthThisFrame;

            // we ought to calculate our length before resetting.
            foreach (Duration d in children)
            {
              
                d.Reset();

            }

           
            base.Reset();
        }

   

        internal override string CreateReport(TimeSpan total)
        {
            String s = base.CreateReport(total)+":";
            foreach (Duration d in children)
            {
                s += Util.Indent(d.CreateReport(AverageLength));
            }
            return s;
        }
    }
}
