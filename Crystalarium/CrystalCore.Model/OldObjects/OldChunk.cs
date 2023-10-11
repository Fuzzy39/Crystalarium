using CrystalCore.Model.Core;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.DefaultObjects
{
    public class OldChunk : OldMapObject
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
            get => _children; //BIG CHONK

        }

        public List<ChunkMember> MembersWithin
        {
            get => _membersWithin;

        }


        internal OldChunk(DefaultMap m, Point pos) : base(m, pos * new Point(SIZE), new Point(SIZE))
        {
            // check that this chunk does not exist over another chunk.
            if (m.grid != null)
            {
                CheckOverlap();
            }

            _coords = pos;

            _children = new List<ChunkMember>();
            _membersWithin = new List<ChunkMember>();
            Ready();
        }

        private void CheckOverlap()
        {
            foreach (OldChunk ch in Map.grid.ElementList)
            {

                if (ch == null)
                {
                    continue;
                }


                if (!ch.Bounds.Intersects(Bounds))
                {
                    continue;
                }

                if (ch == this)
                {
                    continue;
                }

                // uh oh, this chunk intersects another chunk! bail!
                throw new InvalidOperationException("Chunk intersected another chunk at " + Bounds);


            }
        }

        public override string ToString()
        {
            return "Chunk " + Coords;
        }


        public override void Destroy()
        {

            // how was this not a thing until M6?
            while (_membersWithin.Count > 0)
            {
                _membersWithin[0].Destroy();
            }

            base.Destroy();

        }

    }
}
