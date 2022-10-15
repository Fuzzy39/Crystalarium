using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using CrystalCore.Util;
using System.Diagnostics;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Communication;
using CrystalCore.Model.Rulesets;
using CrystalCore.Util.Timekeeping;

namespace CrystalCore.Model.Grids
{
    /// <summary>
    /// the grid class represents a grid, a 2D plane where devices are built using various rulesets.
    /// The Grid class manages all gridobjects on the grid.
    /// </summary>
    public class Grid : ChunkGrid
    { 


        private SimulationManager sim;

       
        private List<Agent> _agents; // t  he amount of agents in this grid.
        private List<Signal> _signals;


        

        private Ruleset _ruleset; // the ruleset this grid is following.

        
      

        public int AgentCount { get => _agents.Count; }

        public int SignalCount { get => _signals.Count; }



        public Ruleset Ruleset
        {
            get => _ruleset;
            set
            {
                if (value == _ruleset)
                {
                    return;
                }

                Reset();


                _ruleset = value;
            }
        }


        internal Grid(SimulationManager sim, Ruleset r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("Null Ruleset not viable.");
            }
            _ruleset = r;
            this.sim = sim;
            sim.addGrid(this);
            _agents = new List<Agent>();
            _signals = new List<Signal>();


            Reset();


        }


        public void Destroy()
        {
            sim.removeGrid(this);
        }

        public new void Reset()
        {
                
            // Destroy all members of this grid, starting from the most dependent objects to the least. 

            if (_agents != null)
            {
                while (_agents.Count > 0)
                {
                    _agents[0].Destroy();
                }
            }

            base.Reset();
          
        }

        public override void ExpandGrid(Direction d)
        {
            base.ExpandGrid(d);
            UpdateSignals(ChunkList);
        }

        internal override void OnObjectDestroyed(Object o, EventArgs e)
        {

            // Remove a grid object from it's appropriate containers.

            if (o is Agent)
            {
                _agents.Remove((Agent)o);
                return;
            }

            if (o is Signal)
            {
                _signals.Remove((Signal)o);
                return;
            }

            base.OnObjectDestroyed(o, e);
        }

        public void AddAgent(Agent a)
        {
            if (a.Grid != this)
            {
                throw new ArgumentException("Agent " + a + "Does not belong to this grid.");
            }

            _agents.Add(a);
            UpdateSignals(a.ChunksWithin);
        }

        internal void AddSignal(Signal s)
        {
            if (s.Grid != this)
            {
                throw new ArgumentException("Signal " + s + "Does not belong to this grid.");
            }
            _signals.Add(s);
        }


        /// <summary>
        /// Asks each signal in where chunks to re establish their target.
        /// </summary>
        /// <param name="where"></param>
        internal void UpdateSignals(List<Chunk> where)
        {

            // this will update some signals multiple times, but eh...
            foreach (Chunk ch in where)
            {
                List<ChunkMember> toUpdate = new List<ChunkMember>(ch.MembersWithin);
                foreach (ChunkMember member in toUpdate)
                {
                    if (!(member is Signal))
                    {
                        continue;
                    }

                    Signal s = (Signal)member;

                    // signals can be destroyed while this loop runs. if they are, ignore them.
                    if(s.Destroyed)
                    {
                        continue;
                    }
                    s.Update();

                }

            }

        }

  

        /// <summary>
        /// Perform a simulation step for this grid.
        /// </summary>
        internal void Step()
        {


            // have each agent determine the state they will be in next step based on the state of the grid last step.
            Timekeeper.Instance.StartTask("Get AgentState");
            foreach (Agent a in _agents)
            {
                a.PreserveState();
            }
            Timekeeper.Instance.StopTask("Get AgentState");

            // have each agent perform it's next step, no longer needing to look at the state of the grid.
            Timekeeper.Instance.StartTask("Transform");
            for (int i = 0; i < _agents.Count; i++)
            {
                Agent a = _agents[i];

                a.Update();

                // transformations applied to agents can destroy them.
                if (a.Destroyed)
                {
                    i--;
                }


            }
            Timekeeper.Instance.StopTask("Transform");
        }

    }
}
