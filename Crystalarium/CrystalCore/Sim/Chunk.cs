using CrystalCore.Sim.Base;
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
         * As it stands now, chunks are not exactly meant to ever be destroyed (I think)
         * That's just how the grid handles them though
         */

        public const int SIZE = 16;
        private Point _coords; // the coordinates, in chunks, of this chunk.
                               // (equal to top left position divided by size)

        private List<ChunkMember> _children; // all chunkmembers whose bound's locations are within this chunk.
        private List<ChunkMember> _membersWithin; // all chunkMembers whose bounds intersect ours.



        public Point Coords
        {
            get => _coords;
        }


        public List<ChunkMember> Children
        {
            get =>  _children;
       
        }

        public List<ChunkMember> MembersWithin
        {
            get => _membersWithin;

        }


        internal Chunk(Grid g, Point pos) : base(g, pos*new Point(SIZE), new Point(SIZE))
        {
           // check that this chunk does not exist over another chunk.
           foreach(List<Chunk> chunks in Grid.Chunks)
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

            _children = new List<ChunkMember>();
            _membersWithin = new List<ChunkMember>();
        }

        public override string ToString()
        {
            return "Chunk " + Coords;
        }

    }
}
