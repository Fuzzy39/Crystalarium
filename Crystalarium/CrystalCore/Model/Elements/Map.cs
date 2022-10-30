using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using CrystalCore.Util.Timekeeping;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CrystalCore.Model.Elements
{
    /// <summary>
    /// the grid class represents a grid, a 2D plane where devices are built using various rulesets.
    /// The Grid class manages all gridobjects on the grid.
    /// </summary>
    public class Map
    {


        private SimulationManager sim;


        private List<Agent> _agents; // t  he amount of agents in this grid.
        private int _signals;
        private int _chunks;

        internal Grid<Chunk> grid;



        private Ruleset _ruleset; // the ruleset this grid is following.


        public event EventHandler OnReset;

        public int AgentCount { get => _agents.Count; }

        public int SignalCount { get => _signals; }

        public int ChunkCount { get => _chunks; }



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

     



        public Rectangle Bounds
        {
            get
            {
                return new Rectangle
                  ( grid.Origin.X * Chunk.SIZE,
                    grid.Origin.Y * Chunk.SIZE,
                    grid.Size.X * Chunk.SIZE,
                    grid.Size.Y * Chunk.SIZE);
            }

        }

        public Vector2 Center
        {
            get
            {
                // the center tile coords of this grid
                return grid.Size.ToVector2() * Chunk.SIZE / 2f;

            }


        }

        internal Map(SimulationManager sim, Ruleset r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("Null Ruleset not viable.");
            }
            _ruleset = r;
            this.sim = sim;
            sim.addGrid(this);

            _agents = new List<Agent>();

            Reset();


        }


        public void Destroy()
        {
            sim.removeGrid(this);
        }

        public void Reset()
        {

            // reseting our grid should remove all references to any remaining mapObjects
            if (grid != null)
            {
                foreach (Chunk ch in grid.ElementList)
                {
                    ch.Destroy();
                }

            }
            grid = new Grid<Chunk>(new Chunk(this, new Point(0, 0) ));



            // could be redundant?
            _agents.Clear();
            _signals = 0;
            _chunks = 1;

        }

        public void ExpandGrid(Direction d)
        {
            Chunk[] toAdd;
            Point start;

            // figure out a sensible starting point in grid space.
            // if we switched on direction and made each one make sense, then 
            if (d == Direction.up || d == Direction.left)
            {
                start = grid.Elements[0][0].Coords + d.ToPoint();
            }
            else
            {
                start = (grid.Origin + (grid.Size-new Point(1))) + d.ToPoint();
            }

            // get the size of the new entries.
            if (d.IsVertical())
            {
                // create an array!
                toAdd = new Chunk[grid.Size.X];
            }
            else
            {
                toAdd = new Chunk[grid.Size.Y];

            }

            // calculate the direction we need to move from our starting point to generate the required chunks
            Direction counting = d.IsVertical() ? d.Rotate(RotationalDirection.cw) : d.Rotate(RotationalDirection.ccw);
            Point traverse = counting.ToPoint();

            // make all the chunks
            for (int i = 0; i < toAdd.Length; i++)
            {
                
                // calculate our distance from our start
                Point add = new Point(i * traverse.X, i * traverse.Y);
                    Point coord = start + add;
                // create the chunk, add it to the list.
                toAdd[i] = new Chunk(this, coord);
            }

            if(d == Direction.right || d == Direction.down)
            {
               Array.Reverse(toAdd);
            }

            grid.AddElements(toAdd, d);

            UpdateSignals(grid.ElementList);
        }

        internal void OnObjectDestroyed(object o, EventArgs e)
        {

            // Remove a grid object from it's appropriate containers.

            if (o is Agent)
            {
                _agents.Remove((Agent)o);
                return;
            }

            if (o is Signal)
            {
                _signals--;
                return;
            }

            if (o is Chunk)
            {
                _chunks--;
                return;
            }
        }

        internal void OnObjectCreated(object o, EventArgs e)
        {
            if (o is Agent)
            {
                _agents.Add((Agent)o);
                return;
            }

            if (o is Signal)
            {
                _signals++;
                return;
            }

            if (o is Chunk)
            {
                _chunks++;
                return;
            }
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
                    if (s.Destroyed)
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
