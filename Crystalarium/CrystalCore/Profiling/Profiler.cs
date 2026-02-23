using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Profiling
{
    /// <summary>
    /// The profiler is for keeping track of the amount of time various tasks take up, to help find performance issues.
    /// It is used by creating and disposing Profiling.Task objects, and its static methods
    /// 
    /// It is structured in a way that is not useful for multithreaded code. Some Revision will be needed in the future, possibly.
    /// </summary>
    public class Profiler
    {
        private static Profiler _instance;
        private static string _report;

        private Stack<Task> _currentTasks;
        private List<ProfilerRecord> _records;
        private ProfilerRecord? _head;
        private Stopwatch _stopwatch;
        

        public static int SampleSize
        {
            get; set;
        }

        private Profiler() 
        {
            SampleSize = 30;
            _report = string.Empty;
            _currentTasks = new Stack<Task>();
            _records = new(); // arguably we should store a list of children so it could be more flexible...
            _head = null;
            _instance = this;
        }

        internal static Profiler GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Profiler();
            }
            return _instance;
        }



        internal void StartTask(Task task)
        {
            // try to get the corresponding record.
            ProfilerRecord? record;
            if (_head == null)
            {
                record = _records.FirstOrDefault(i => i.Name.Equals(task.Name));
                _stopwatch = Stopwatch.StartNew();
                if (record == null)
                {
                    record = new ProfilerRecord(task.Name, null);
                    _records.Add(record);
                }
            }
            else
            {
                record = _head.GetChild(task.Name);
                if(record == null) record = _head.AddChild(task.Name);
            }

            
            _head = record;

            // finally update the task
            task.TimeStarted = _stopwatch.Elapsed;
            _currentTasks.Push(task);
        }


        internal void FinishTask(Task task)
        {
       
            if(_currentTasks.Peek()!= task)
            {
                throw new InvalidOperationException("Can only finish the most recently started active task. Need to finish '"
                    +_currentTasks.Peek().Name+"' before '"+task.Name+"'.");
            }

            // record the time for this task.
            _head.Update(_stopwatch.Elapsed-task.TimeStarted, SampleSize);
            
            // return the current task to its parent.
            _currentTasks.Pop();

            if(_head.Parent == null)
            {
                _report = _head.CreateReport(null);
                _stopwatch.Stop();
            }
            _head = _head.Parent;

        }



        /// <summary>
        /// Gets the report of the most recent root task to have been completed
        /// </summary>
        /// <returns></returns>
        public static string GetReport()
        {
            return _report;
        }
        

    }
}
