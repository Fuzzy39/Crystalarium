using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Profiling
{
    /// <summary>
    /// This class represents a profiling task that is currently in progress. It is complete when it is disposed.
    /// </summary>
    public class Task : IDisposable
    {

  
        string _name;

        public string Name { get { return _name; } }

        internal TimeSpan TimeStarted { get; set; }


        public Task(string name) 
        {
            _name = name;
            Profiler.GetInstance().StartTask(this);
        }

        public void Dispose()
        {
            Profiler.GetInstance().FinishTask(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
