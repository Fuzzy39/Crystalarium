using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using CrystalCore.Util;
using System.Diagnostics;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Communication;
using CrystalCore.Model.Rulesets;

namespace CrystalCore.Model
{
    public class Grid
    {
        /* the grid class represents a grid.
        * In Crystalarium, a grid is a 2d plane where devices can be built using a number of systems,
        * with the primary system called Crysm.
        * 
        * A grid manages a number of chunks, and any chunkmembers of those chunks.
        * 
        */


        private SimulationManager sim;

        private List<List<Chunk>> _chunks; // a 2d array where the outer array represents rows and the inner array represents columns. [x][y]
        private List<Agent> _agents; // the amount of agents in this grid.
        private List<Signal> _signals;


        private Point chunksOrigin; // the chunk coords where the chunk array, chunks, starts.
        private Point chunksSize; // the size, in chunks, of the grid.

        private Ruleset _ruleset; // the ruleset this grid is following.

        public event EventHandler OnReset;
        public List<List<Chunk>> Chunks
        {
            get => _chunks;
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle
                  (chunksOrigin.X * Chunk.SIZE,
                    chunksOrigin.Y * Chunk.SIZE,
                    chunksSize.X * Chunk.SIZE,
                    chunksSize.Y * Chunk.SIZE);
            }

        }

        public Point gridSize
        {
            get => chunksSize;
        }

        public int AgentCount { get => _agents.Count; }

        public int SignalCount { get => _signals.Count; }

        public Vector2 center
        {
            get
            {
                // the center tile coords of this grid
                return chunksSize.ToVector2() * Chunk.SIZE / 2f;

            }


        }


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
            if(r == null)
            {
                throw new ArgumentNullException("Null Ruleset not viable.");
            }    
            _ruleset = r;
            this.sim = sim;
            sim.addGrid(this);
            _agents = new List<Agent>();
            _signals = new List<Signal>();

            // perform first time setup.
            Reset();


        }

        // Probably temporary.
        public void DebugReport()
        {
            Console.WriteLine("Size: " + chunksSize + "\nOrigin:" + chunksOrigin + "\n");

            foreach (List<Chunk> list in _chunks)
            {
                String s = "{";
                foreach (Chunk ch in list)
                {
                    // this is silly! why do I have to write all of this?
                    s += ((ch != null) ? ch.ToString() : "null") + ", ";
                }

                s = s.Substring(0, s.Length - 2) + "},";
                Console.WriteLine(s);
            }
        }

        public void Destroy()
        {
            sim.removeGrid(this);
        }

        public void Reset()
        {
        
            // Destroy all members of this grid, starting from the most dependent objects to the least.
            /*(if (_signals != null) // Agents manage signals, this shouldn't be a thing.
            {
                while (_signals.Count > 0)
                {
                    _signals[0].Destroy();
                }
            }*/

            if (_agents != null)
            {
                while(_agents.Count>0)
                {
                    _agents[0].Destroy();
                }
            }

            // remove any existing chunks.
            if (_chunks != null)
            {
                foreach (List<Chunk> list in _chunks)
                {
                    foreach (Chunk ch in list)
                    {
                        ch.Destroy();
                    }
                }

            }



            // initialize the chunk array.
            _chunks = new List<List<Chunk>>();
            _chunks.Add(new List<Chunk>());

            // create initial chunk.
            _chunks[0].Add(new Chunk(this, new Point(0, 0)));

            // set the chunk origin.
            chunksOrigin = new Point(0, 0);
            chunksSize = new Point(1, 1);

            // alert others that we have reset.
            if (OnReset != null)
            {
                OnReset(this, new EventArgs());
            }
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
                UpdateSignals( ((Agent)o).ChunksWithin);
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
            if(a.Grid!= this)
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

        public void ExpandGrid(Direction d)
        {
            if (d.IsHorizontal())
            {
                ExpandHorizontal(d);
            }
            else
            {
                ExpandVertical(d);
            }

            // update all chunks, 'cause I'm lazy.
            List<Chunk> chunks = new List<Chunk>();
            foreach( List<Chunk> list in _chunks)
            {
                chunks.AddRange(list);
            }
            UpdateSignals(chunks);

        }

        private void ExpandHorizontal(Direction d)
        {
            // we are adding a new list<Chunk> to _chunks.
            List<Chunk> newList = new List<Chunk>();
            chunksSize.X++;
            int x;


            if (d == Direction.left)
            {
                x = 0;
                _chunks.Insert(x, new List<Chunk>());
                chunksOrigin.X--;
            }
            else
            {
                x = _chunks.Count;
                _chunks.Add(newList);
            }


            // generate the new chunks
            for (int y = 0; y < chunksSize.Y; y++)
            {
                Point gridLoc = new Point(x, y) + chunksOrigin;
                Chunk ch = new Chunk(this, gridLoc);
                _chunks[x].Add(ch);

            }

        }

        private void ExpandVertical(Direction d)
        {
            // we are adding a new Chunk to every list<Chunk> in _chunk.
            chunksSize.Y++;


            if (d == Direction.up)
                chunksOrigin.Y--;

            // create the new chunks.

            for (int x = 0; x < _chunks.Count; x++)
            {

                int y;

                if (d == Direction.up)
                {
                    y = 0;
                    _chunks[x].Insert(0, null);
                }
                else
                {
                    y = _chunks[x].Count;
                    _chunks[x].Add(null);

                }

                // including chunk origins is important to get the correct coords for this chunk.
                Chunk ch = new Chunk(this, new Point(x + chunksOrigin.X, y + chunksOrigin.Y));

                _chunks[x][y] = ch;

            }
        }


        // returns the Position in chunkCoords of a particular chunk
        public Point getChunkPos(Chunk ch)
        {
            // get the chunk

            for (int x = 0; x < Chunks.Count; x++)
            {
                List<Chunk> list = Chunks[x];
                for (int y = 0; y < list.Count; y++)
                {
                    if (list[y] == ch)
                    {
                        // we found the chunk!
                        return new Point(x, y);
                    }
                }
            }

            throw new ArgumentException("Chunk '" + ch + "' is not part of Grid '" + this + "'.");
        }


        internal void UpdateSignals( List<Chunk> where)
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
