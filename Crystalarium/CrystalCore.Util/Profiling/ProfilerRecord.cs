using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Util.Profiling
{
    internal class ProfilerRecord
    {
        private string _name;
        private bool _updated; // whether this record has been updated since the last time the root record was updated.

        private ProfilerRecord _parent;
        private List<ProfilerRecord> _children;
        private List<TimeSpan> _recordQueue; // index 0 is the oldest record.


        public ProfilerRecord Parent { get { return _parent; } }
        public string Name { get { return _name; } }

        public TimeSpan AverageTime
        {
            get
            {
                TimeSpan toReturn = TimeSpan.Zero;
                foreach (TimeSpan timeSpan in _recordQueue)
                {
                    toReturn += timeSpan;
                }

                return toReturn / _recordQueue.Count;
            }
        }



        public ProfilerRecord(string name, ProfilerRecord parent)
        {
            _name = name;
            _updated = false;
            _recordQueue = new List<TimeSpan>();
            _parent = parent;

            _children = new List<ProfilerRecord>();
        }

        public ProfilerRecord? GetChild(string name)
        {
            return _children.FirstOrDefault(i => i._name.Equals(name));

        }

        public ProfilerRecord AddChild(string name)
        {
            ProfilerRecord toReturn = new ProfilerRecord(name, this);
            _children.Add(toReturn);
            return toReturn;
        }

        public void Update(TimeSpan time, int sampleSize)
        {

            // update our record
            if (_updated)
            {
                // add the new time to the old time... somehow
                _recordQueue[_recordQueue.Count - 1] += time;
            }
            else
            {
                _recordQueue.Add(time);
            }

            if (_recordQueue.Count > sampleSize) _recordQueue.RemoveAt(0);


            _updated = true;

            // update un-updated children
            foreach (ProfilerRecord child in _children)
            {
                if (!child._updated) child.Update(TimeSpan.Zero, sampleSize);
            }
        }

        public void Reset()
        {
            _updated = false;
            foreach (ProfilerRecord child in _children)
            {
                child.Reset();
            }
        }

        public string CreateReport(TimeSpan? parentTime)
        {

            // Report our own time spent
            string selfReport = _name + ": " + MiscUtil.FormatTime(AverageTime) +
                (parentTime != null ? " (" + Math.Round(AverageTime / (TimeSpan)parentTime * 100, 1) + "%)" : "");
            if (_children.Count == 0) return selfReport;

            // create the report for everything below us
            string subReport = "";
            TimeSpan childrenTime = TimeSpan.Zero;
            foreach (ProfilerRecord child in _children)
            {
                subReport += "\n" + child.CreateReport(AverageTime);
                childrenTime += child.AverageTime;
            }

            // including the time not spent on children
            TimeSpan selfTime = AverageTime - childrenTime;
            subReport += "\nself: " + MiscUtil.FormatTime(selfTime) + " (" + Math.Round(selfTime / AverageTime * 100, 1) + "%)";


            return selfReport + MiscUtil.Indent(subReport);
        }

        public override string ToString()
        {
            return Name;
        }



    }
}
