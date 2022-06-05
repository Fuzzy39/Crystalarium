using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Sim.Base
{
    // does this need to be public? not sure.
    public abstract class ChunkMember : GridObject
    {
        /*
         * A Chunk Member is a GridObject that is not a chunk, and exists on top of a chunk.
         * Depending on its size, it can be part of multiple chunks. Though only one chunk is its parent. 
         * This is so that this member is only updated/rendered once.
         * 
         */
        private Chunk _parentChunk; // the chunk where our position (top left corner) is.
        private List<Chunk> _chunksWithin; // the list of chunks that intersect our boundries. this includes our parent.

        public Chunk Parent
        {
            get => _parentChunk;

        }

        public List<Chunk> ChunksWithin
        {
            get => _chunksWithin;
        }

        public ChunkMember(Grid g, Rectangle bounds) : base(g, bounds)
        {
            // since this ChunkMember exists purely on the chunk grid, it cannot exist outside of it.
            if(!g.Bounds.Contains(bounds))
            {
                Console.WriteLine("I'm fucked");
                throw new ArgumentException("Invalid Bounds for ChunkMember '"+bounds+"'. Bounds were outside the currently existing chunk grid, with bounds "+g.Bounds);
            }

            // who is our parent?
            _parentChunk = _grid.getChunkAtCoords(bounds.Location);
            _parentChunk.Children.Add(this);

            //initialize the other owner array

            _chunksWithin = SetChunksWithin();

            foreach( Chunk ch in _chunksWithin)
            {
                ch.MembersWithin.Add(this);
            }


        }

        private List<Chunk> SetChunksWithin()
        {
            List<Chunk> toReturn = new List<Chunk>();

            Chunk minimum = _parentChunk;

            // the bottom right Chunk within our borders
            Chunk extreme = _grid.getChunkAtCoords(Bounds.Location + Bounds.Size - new Point(1));

            // iterate through all chunks between (and including) the minimum and extreme, and add them.

            // how much to iterate?
            Point initial = _grid.getChunkPos(minimum);
            Point sizeInChunks = _grid.getChunkPos(extreme) - initial + new Point(1);
           
            // this should get all of the chunks.
            for(int x = 0; x < sizeInChunks.X; x++ )
            {
                for(int y = 0; y < sizeInChunks.Y; y++)
                {
                    Point i = new Point(x, y) + initial;
                    Console.WriteLine(i);
                    toReturn.Add( _grid.Chunks[i.X][i.Y]);

                }
            }

            return toReturn;
            
        }
            
        new public void Destroy()
        {
            // remove all external references to ourselves.
            _parentChunk.Children.Remove(this);

            
            foreach(Chunk ch in _chunksWithin)
            {
                ch.MembersWithin.Remove(this);
            }


            base.Destroy();
        }


        public override string ToString()
        {
            string within = "";
            foreach (Chunk ch in _chunksWithin)
            {
                within += ch + ", ";
            }
            // prune off the last comma, if required.
            within = within.Length>1 ? within.Substring(0, within.Length - 2): "";

            return "ChunkMember: { Bounds: " + _bounds + ", Parent: "+_parentChunk+", Intersecting Chunks: {"+within+"} }";
        }
    }
}
