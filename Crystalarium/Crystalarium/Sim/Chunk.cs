using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Sim
{
    class Chunk : GridObject
    {
        /*
         * A chunk is a unit of several tiles. together, they make up the grid. 
         */

        public const int SIZE = 16;

        public Chunk(Grid g, Point pos) : base(g, pos*new Point(SIZE), new Point(SIZE))
        {
           // check that this chunk does not exist over another chunk.
           foreach(Chunk ch in Parent.GetChunks())
           {
                if(!ch.Bounds.Intersects(this.Bounds))
                {
                    continue;   
                }

                // uh oh, this chunk intersects another chunk! bail!
                Console.WriteLine("Chunk intersected another chunk at " + Bounds);
                this.Destroy();
           }
        }
    }
}
