using CrystalCore.Model;
using CrystalCore.Model.DefaultObjects;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;

namespace CrystalCore.Model.Core
{
    /// <summary>
    /// the grid class represents a grid, a 2D plane where devices are built using various rulesets.
    /// The Grid class manages all gridobjects on the grid.
    /// </summary>
    public class Map
    {



        public OldGrid<Chunk> grid;



        private Ruleset _ruleset; // the ruleset this grid is following.



        public event EventHandler OnReset;
        public event EventHandler OnResize;
        public event EventHandler OnMapObjectReady;
        public event EventHandler OnMapObjectDestroyed;





        public Ruleset Ruleset
        {
            get => _ruleset;
            set
            {
                _ruleset = value;

                Reset();

            }
        }





        public Rectangle Bounds
        {
            get
            {
                return new Rectangle
                  (grid.Origin.X * Chunk.SIZE,
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
                return grid.Size.ToVector2() * Chunk.SIZE / 2f + Bounds.Location.ToVector2();

            }


        }

        public Map( Ruleset r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("Null Ruleset not viable.");
            }
            _ruleset = r;

            

            Reset();




        }


        public void Reset()
        {
            Reset(new Rectangle(0, 0, 0, 0));
        }

        public void Reset(Rectangle minimumBounds)
        {




            // reseting our grid should remove all references to any remaining mapObjects
            if (grid != null)
            {
                foreach (Chunk ch in grid.ElementList)
                {
                    ch.Destroy();
                }

            }


            grid = new OldGrid<Chunk>(new Chunk(this, new Point(0, 0)));
            this.ExpandToFit(minimumBounds);


           

            if (OnReset != null)
            {
                OnReset(this, new EventArgs());

            }
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
                start = grid.Origin + (grid.Size - new Point(1)) + d.ToPoint();
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

            if (d == Direction.right || d == Direction.down)
            {
                Array.Reverse(toAdd);
            }

            grid.AddElements(toAdd, d);

            if (OnResize != null)
            {
                OnResize(this, new EventArgs());
            }
        }

        internal void OnObjectDestroyed(object o, EventArgs e)
        {

            // Remove a grid object from it's appropriate containers.
            OnMapObjectDestroyed?.Invoke(o, new());
       
        }

     
        internal void OnObjectReady(object o, EventArgs e)
        {
            OnMapObjectReady?.Invoke(o, new());
        }



        /// <summary>
        /// Perform a simulation step for this grid.
        /// </summary>
        //internal void Step()
        //{


        //    // have each agent determine the state they will be in next step based on the state of the grid last step.
        //    foreach (Agent a in _agents)
        //    {
        //        a.CalculateNextStep();
        //    }

        //    // have each agent perform it's next step, no longer needing to look at the state of the grid.
        //    for (int i = 0; i < _agents.Count; i++)
        //    {
        //        Agent a = _agents[i];

        //        a.Update();

        //        // transformations applied to agents can destroy them.
        //        if (a.Destroyed)
        //        {
        //            i--;
        //        }


        //    }
        //}

    }
}
