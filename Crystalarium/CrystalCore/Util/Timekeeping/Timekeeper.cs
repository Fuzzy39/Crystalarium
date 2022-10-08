using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Util.Timekeeping
{
    /// <summary>
    /// The Timekeeper class is for diagnostics
    /// I don't have a plan for how it should work...
    /// </summary>
    internal class Timekeeper
    {

        //private Stopwatch timer;

        // How should the timekeeping system work?
        // it should keep tabs on a single frame at a time (could do averaging of the last few frames)?
        // it shoud be possible to create things called workloads and tasks - tasks are individualy measured,
        // and workloads contain tasks or other workoads. when the timekeeper generates a timing report, the sum of times a task under
        // a workload. 
    }
}
