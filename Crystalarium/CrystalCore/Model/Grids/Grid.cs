using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using CrystalCore.Util;
using System.Diagnostics;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Communication;
using CrystalCore.Model.Rulesets;

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

            
            


        }


        public void Destroy()
        {
            sim.removeGrid(this);
        }

        public override void Reset()
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

        public void Remove(GridObject o)
        {

            // Remove a grid object from it's appropriate containers

            if (o is Chunk) // chunks can't be removed once added.
            {
                o = null; // Doesn't change the size of the grid. This should be used sparingly.
                return;
            }

            if (o is Agent)
            {
                _agents.Remove((Agent)o);
                UpdateSignals(((Agent)o).ChunksWithin);
                o = null;
                return;
            }

            if (o is Signal)
            {
                _signals.Remove((Signal)o);

                o = null;

                return;
            }

            throw new ArgumentException("Unknown or Invalid type of GridObject to remove from this grid.");

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
                    if (s.Bounds.Size == new Point(0))
                    {
                        continue; // this signal has been destroyed while other signals are updating.
                                  // this can be caused by conflicts with half port communication.
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
            Console.WriteLine("Step!");

            // have each agent determine the state they will be in next step based on the state of the grid last step.
            foreach (Agent a in _agents)
            {
                a.UpdateState();
            }


            // have each agent perform it's next step, no longer needing to look at the state of the grid.
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
        }

    }
}
