using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Crystalarium.Sim
{
    class Grid
    {
        /* the grid class represents a grid.
        * In Crystalarium, a grid is a 2d plane where devices can be built using a number of systems,
        * with the primary system called Crysm.
        */


        SimulationManager sim;

        private List<List<Chunk>> chunks; // a 2d array where the outer array represents rows and the inner array represents collumns.

        public Grid(SimulationManager sim)
        {
            this.sim = sim;
            sim.addGrid(this);

            // initialize the chunk array.
            chunks = new List<List<Chunk>>();
            //chunks.Add(new List<Chunk>());
            // create initial chunk.
            chunks[0][0] = new Chunk( this, new Point(0,0) );

            // temporary code. Add some chunks, for testing!

        }

        public void Destroy()
        {
            sim.removeGrid(this);
        }

        public void Add( GridObject o)
        {
            // what we do with the gridobject depends on what kind of object it is.
            if(o is Chunk)
            {
                Chunk ch = (Chunk)o;

                //  this will not work.
                chunks[ch.Bounds.X][ch.Bounds.Y] = ch;

                return;
            }

            // This girdObject is not of any known instance, so we throw an expection; we aren't prepared to 
            // handle it
            throw new ArgumentException("Unknown or Invalid type of GridObject to Add to this grid.");
        }

        public void Remove(GridObject o)
        {

            // Remove a grid object from it's appropriate containers
            if( o is Chunk)
            {
                chunks.Remove((Chunk)o);
                return;
            }

            throw new ArgumentException("Unknown or Invalid type of GridObject to remove from this grid.");

        }

    }
}
