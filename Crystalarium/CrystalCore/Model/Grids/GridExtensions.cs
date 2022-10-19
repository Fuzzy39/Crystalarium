using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.Model.Grids
{
    public static class GridExtensions
    {

        /// <summary>
        /// returns the Position in chunkCoords of a particular chunk
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Point GetChunkPos(this ChunkGrid cg, Chunk ch)
        {
            // get the chunk

            for (int x = 0; x < cg.Chunks.Count; x++)
            {
                List<Chunk> list = cg.Chunks[x];
                for (int y = 0; y < list.Count; y++)
                {
                    if (list[y] == ch)
                    {
                        // we found the chunk!
                        return new Point(x, y);
                    }
                }
            }

            throw new ArgumentException("Chunk '" + ch + "' is not part of Grid '" + cg + "'.");
        }


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

        public static Agent AgentAt(this Grid g, Point p)
        {
            Chunk c = g.getChunkAtCoords(p);

            foreach (ChunkMember cm in c.MembersWithin)
            {
                if (!(cm is Agent))
                {
                    continue;
                }

                if(cm.Bounds.Contains(p))
                {
                    return (Agent)cm;
                }
            }

            return null;
        }



        // returns the agents within these bounds
        public static List<Entity> EntitiesWithin(this Grid g, Rectangle bounds)
        {
            List<Entity> toReturn = new List<Entity>();

            foreach (Chunk ch in g.ChunksInBounds(bounds))
            {
                foreach (ChunkMember chm in ch.MembersWithin)
                {

                    if (!(chm is Entity)) // only agents take up space.
                    {
                        continue;
                    }

                    Entity e = (Entity)chm;

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


        public static List<Agent> AgentsWithin(this Grid g, Rectangle bounds)
        {
            List<Entity> possibleList = EntitiesWithin(g, bounds);

            List<Agent> toReturn = new List<Agent>();

            foreach(Entity e in possibleList)
            {
                if (e is Agent)
                {
                    toReturn.Add((Agent)e);
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
            if (!g.Bounds.Contains(rect))
            {
                throw new ArgumentException("Grid " + g + " does not contain bounds: " + rect);
            }


            List<Chunk> toReturn = new List<Chunk>();

            Chunk minimum = g.getChunkAtCoords(rect.Location);

            // the bottom right Chunk within rect's borders
            Point extremePoint = rect.Location + rect.Size - new Point(1);
            Chunk extreme = g.getChunkAtCoords(extremePoint);

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

                    toReturn.Add(g.Chunks[i.X][i.Y]);

                }
            }

            return toReturn;
        }

        public static void ExpandToFit(this Grid g, Rectangle rect)
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
