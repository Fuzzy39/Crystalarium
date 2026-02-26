using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Util.Profiling
{
    /// <summary>
    /// This class represents a profiling task that is currently in progress. It is complete when it is disposed.
    /// </summary>
    public class ProfilingTask : IDisposable
    {


        string _name;

        public bool Finished { get; private set; }

        public string Name { get { return _name; } }

        internal TimeSpan TimeStarted { get; set; }


        public ProfilingTask(string name)
        {
            _name = name;
            Finished = false;
            Profiler.GetInstance().StartTask(this);
        }

        public void Dispose()
        {
            Finished = true;
            Profiler.GetInstance().FinishTask(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
