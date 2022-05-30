using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Sim
{
    public class Chunk : GridObject
    {
        /*
         * A chunk is a unit of several tiles. together, they make up the grid. 
         */

        public const int SIZE = 16;
        private Point _coords; // the coordinates, in chunks, of this chunk.
                               // (equal to top left position divided by size)

        public Point Coords
        {
            get => _coords;
        }


        public Chunk(Grid g, Point pos) : base(g, pos*new Point(SIZE), new Point(SIZE))
        {
           // check that this chunk does not exist over another chunk.
           foreach(List<Chunk> chunks in Parent.Chunks)
           {
                foreach (Chunk ch in chunks)
                {
                    if(ch == null)
                    {
                        continue;
                    }


                    if (!ch.Bounds.Intersects(this.Bounds))
                    {
                        continue;
                    }

                    if(ch==this)
                    {
                        continue;
                    }

                    // uh oh, this chunk intersects another chunk! bail!
                    Console.WriteLine("Chunk intersected another chunk at " + Bounds);
                    this.Destroy();
                }
           }

            _coords = pos;
        }

        public override string ToString()
        {
            return "Chunk " + Coords;
        }

    }
}
