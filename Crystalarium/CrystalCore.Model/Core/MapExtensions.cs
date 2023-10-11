using CrystalCore.Model.DefaultObjects;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace CrystalCore.Model.Core
{
    public static class MapExtensions
    {

        /// <summary>
        /// returns the Position in chunkCoords of a particular chunk
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Point GetChunkPos(this DefaultMap cg, OldChunk ch)
        {
            // get the chunk

            for (int x = 0; x < cg.grid.ElementList.Count; x++)
            {
                List<OldChunk> list = cg.grid.Elements[x];
                for (int y = 0; y < list.Count; y++)
                {
                    if (list[y] == ch)
                    {
                        // we found the chunk!
                        return new Point(x, y);
                    }
                }
            }

            throw new ArgumentException("Chunk '" + (ch != null ? ch : "null") + "' is not part of Grid '" + cg + "'.");
        }


        public static Agent getAgentAtPos(this DefaultMap g, Point coords)
        {
            OldChunk ch = g.getChunkAtCoords(coords);

            if (ch == null)
            {
                return null;
            }

            List<ChunkMember> agents = ch.MembersWithin;
            foreach (ChunkMember cm in agents)
            {
                if (!(cm is Agent))
                {
                    continue;
                }

                Agent a = (Agent)cm;

                if (a.Bounds.Contains(coords))
                {
                    return a;
                }

            }

            return null;
        }




        // returns the agents within these bounds
        public static List<OldEntity> EntitiesWithin(this DefaultMap g, Rectangle bounds)
        {
            List<OldEntity> toReturn = new List<OldEntity>();

            foreach (OldChunk ch in g.ChunksInBounds(bounds))
            {
                foreach (ChunkMember chm in ch.MembersWithin)
                {

                    if (!(chm is OldEntity)) // only agents take up space.
                    {
                        continue;
                    }

                    OldEntity e = (OldEntity)chm;

                    if (e.Bounds.Intersects(bounds))
                    {
                        // only add a if it isn't already in the list
                        if (toReturn.Exists((obj) => { return obj == e; })) // I'm pretty sure this line of code is witchcraft, but whatever.
                        {
                            continue;
                        }

                        toReturn.Add(e);
                    }


                }
            }

            return toReturn;
        }


        public static List<Agent> AgentsWithin(this DefaultMap g, Rectangle bounds)
        {
            List<OldEntity> possibleList = g.EntitiesWithin(bounds);

            List<Agent> toReturn = new List<Agent>();

            foreach (OldEntity e in possibleList)
            {
                if (e is Agent)
                {
                    toReturn.Add((Agent)e);
                }
            }

            return toReturn;

        }


        public static OldChunk getChunkAtCoords(this DefaultMap g, Point Coords)
        {
            // we could iterate through every chunk, but we could also do math.
            // math is probably better

            if (!g.Bounds.Contains(Coords))
            {
                // no chunk there.
                return null;
            }

            // should be the coord in the grid's array where chunks are stored.
            Point chunkCoord = (Coords - g.Bounds.Location) / new Point(OldChunk.SIZE);

            // get and return that chunk.
            OldChunk toReturn = g.grid.Elements[chunkCoord.X][chunkCoord.Y];


            // it's possible this doesn't work. If that's true, I'd like to know.
            Debug.Assert(toReturn.Bounds.Contains(Coords) || toReturn.Destroyed);

            return toReturn;


        }



        public static List<OldChunk> ChunksInBounds(this DefaultMap g, Rectangle rect)
        {
            rect = Rectangle.Intersect(g.Bounds, rect);


            List<OldChunk> toReturn = new List<OldChunk>();

            OldChunk minimum = g.getChunkAtCoords(rect.Location);

            // the bottom right Chunk within rect's borders
            Point extremePoint = rect.Location + rect.Size - new Point(1);
            OldChunk extreme = g.getChunkAtCoords(extremePoint);

            // iterate through all chunks between (and including) the minimum and extreme, and add them.

            // how much to iterate?
            Point initial = g.GetChunkPos(minimum);
            Point sizeInChunks = g.GetChunkPos(extreme) - initial + new Point(1);

            // this should get all of the chunks.
            for (int x = 0; x < sizeInChunks.X; x++)
            {
                for (int y = 0; y < sizeInChunks.Y; y++)
                {
                    Point i = new Point(x, y) + initial;

                    toReturn.Add(g.grid.Elements[i.X][i.Y]);

                }
            }

            return toReturn;
        }

        public static void ExpandToFit(this DefaultMap g, Rectangle rect)
        {
            // First: which way to expand?
            while (rect.Y < g.Bounds.Y)
            {
                g.ExpandGrid(Direction.up);
            }

            while (rect.X < g.Bounds.X)
            {
                g.ExpandGrid(Direction.left);
            }

            while (rect.Right > g.Bounds.Right)
            {
                g.ExpandGrid(Direction.right);
            }

            while (rect.Bottom > g.Bounds.Bottom)
            {
                g.ExpandGrid(Direction.down);
            }

        }


    }

}
