using CrystalCore.Sim;
using CrystalCore.Sim.Base;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Util
{
    public static class GridExtensions
    {

        public static Agent getAgentAtPos(this Grid g, Point coords)
        {
            Chunk ch = g.getChunkAtCoords(coords);

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
        public static List<Agent> AgentsWithin(this Grid g, Rectangle bounds)
        {
            List<Agent> toReturn = new List<Agent>();

            foreach (Chunk ch in g.ChunksInBounds(bounds))
            {
                foreach (ChunkMember chm in ch.MembersWithin)
                {

                    if (!(chm is Agent)) // only agents take up space.
                    {
                        continue;
                    }

                    Agent a = (Agent)chm;

                    if (a.Bounds.Intersects(bounds))
                    {
                        // only add a if it isn't already in the list
                        if (toReturn.Exists((Agent obj) => { return obj == a; }))
                        {
                            continue;
                        }

                        toReturn.Add(a);
                    }


                }
            }

            return toReturn;
        }


        public static Chunk getChunkAtCoords(this Grid g, Point Coords)
        {
            // we could iterate through every chunk, but we could also do math.
            // math is probably better

            if (!g.Bounds.Contains(Coords))
            {
                // no chunk there.
                return null;
            }
           
            // should be the coord in the grid's array where chunks are stored.
            Point chunkCoord = (Coords - g.Bounds.Location) / new Point(Chunk.SIZE);

            // get and return that chunk.
            Chunk toReturn = g.Chunks[chunkCoord.X][chunkCoord.Y];

            // it's possible this doesn't work. If that's true, I'd like to know.
            Debug.Assert(toReturn.Bounds.Contains(Coords));

            return toReturn;


        }



        public static List<Chunk> ChunksInBounds(this Grid g, Rectangle rect)
        {
            List<Chunk> toReturn = new List<Chunk>();

            Chunk minimum = g.getChunkAtCoords(rect.Location);

            // the bottom right Chunk within rect's borders
            Point extremePoint = rect.Location + rect.Size - new Point(1);
            Chunk extreme = g.getChunkAtCoords(extremePoint);

            // iterate through all chunks between (and including) the minimum and extreme, and add them.

            // how much to iterate?
            Point initial = g.getChunkPos(minimum);
            Point sizeInChunks = g.getChunkPos(extreme) - initial + new Point(1);

            // this should get all of the chunks.
            for (int x = 0; x < sizeInChunks.X; x++)
            {
                for (int y = 0; y < sizeInChunks.Y; y++)
                {
                    Point i = new Point(x, y) + initial;

                    toReturn.Add(g.Chunks[i.X][i.Y]);

                }
            }

            return toReturn;
        }


    }

}
