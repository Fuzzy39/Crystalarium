using CrystalCore.Model.Core;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.OldObjects
{
    // does this need to be public? not sure.
    public abstract class ChunkMember : OldMapObject
    {
        /*
         * A Chunk Member is a GridObject that is not a chunk, and exists on top of a chunk.
         * Depending on its size, it can be part of multiple chunks. Though only one chunk is its parent. 
         * This is so that this member is only updated/rendered once.
         * 
         */
        private OldChunk _parentChunk; // the chunk where our position (top left corner) is.
        private List<OldChunk> _chunksWithin; // the list of chunks that intersect our boundries. this includes our parent.

        public OldChunk Parent
        {
            get => _parentChunk;

        }

        public List<OldChunk> ChunksWithin
        {
            get => _chunksWithin;
        }


        public override Rectangle Bounds
        {
            get => base.Bounds;
            protected set
            {


                if (!Map.Bounds.Contains(value))
                {
                    throw new ArgumentException("Bounds: " + value + " Are outside of Grid with bounds: " + Map.Bounds);
                }
                base.Bounds = value;
                ResetChunksWithin();
            }
        }


        public ChunkMember(DefaultMap g, Rectangle bounds) : base(g, bounds)
        {
            // since this ChunkMember exists purely on the chunk grid, it cannot exist outside of it.
            if (!g.Bounds.Contains(bounds))
            {

                throw new ArgumentException("Invalid Bounds for ChunkMember '" + bounds + "'. Bounds were outside the currently existing chunk grid, with bounds " + g.Bounds);
            }

            // who is our parent?
            _parentChunk = Map.getChunkAtCoords(bounds.Location);
            _parentChunk.Children.Add(this);

            //initialize the other owner array

            _chunksWithin = SetChunksWithin();

            foreach (OldChunk ch in _chunksWithin)
            {
                ch.MembersWithin.Add(this);
            }


        }

        private List<OldChunk> SetChunksWithin()
        {
            return Map.ChunksInBounds(Bounds);
        }

        public override void Destroy()
        {
            // remove all external references to ourselves.
            _parentChunk.Children.Remove(this);


            foreach (OldChunk ch in _chunksWithin)
            {
                ch.MembersWithin.Remove(this);
            }


            base.Destroy();
        }

        private void ResetChunksWithin()
        {
            // this method rectifies chunk memberships.
            // good if a chunkmember gets resized or moved.
            // kinda hacky.

            //_parentChunk.Children.Remove(this);
            //_parentChunk = _grid.getChunkAtCoords(Bounds.Location);
            //_parentChunk.Children.Add(this);

            foreach (OldChunk ch in _chunksWithin)
            {
                ch.MembersWithin.Remove(this);
            }

            _chunksWithin.Clear();
            _chunksWithin = SetChunksWithin();

            foreach (OldChunk ch in _chunksWithin)
            {
                ch.MembersWithin.Add(this);
            }
        }


        public override string ToString()
        {
            string within = "";
            foreach (OldChunk ch in _chunksWithin)
            {
                within += ch + ", ";
            }
            // prune off the last comma, if required.
            within = within.Length > 1 ? within.Substring(0, within.Length - 2) : "";

            return "ChunkMember: { Bounds: " + Bounds + ", Parent: " + _parentChunk + ", Intersecting Chunks: {" + within + "} }";
        }
    }
}
