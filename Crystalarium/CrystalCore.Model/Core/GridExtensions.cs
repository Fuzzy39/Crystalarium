using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Core
{
    internal static class GridExtensions
    {
        public static Chunk ChunkAtCoords(this Grid g, Point tileCoords)
        {
            Point chunkCoord = g.TileToChunkCoords(tileCoords);

            Point chunkIndex = chunkCoord - g.ChunkOrigin;

            return g.Chunks[chunkIndex.X][chunkIndex.Y];

        }

        public static List<MapObject> ObjectsIntersecting(this Grid g, Rectangle bounds)
        {
            List<Chunk> chunks = g.ChunksIntersecting(bounds);
            List<MapObject> toReturn = new();

            foreach (Chunk chunk in chunks)
            {
                foreach (MapObject obj in chunk.ObjectsIntersecting)
                {

                    if (obj.Bounds.Intersects(bounds))
                    {
                        // only add a if it isn't already in the list
                        // explaination of line: if element in toReturn exists such that element is obj ...
                        // simpler explanation: if obj is already in the list...
                        if (toReturn.Exists((test) => { return test == obj; }))
                        {
                            continue;
                        }

                        toReturn.Add(obj);
                    }


                }
            }
            return toReturn;

        }


        public static MapObject FindClosestObjectInDirection(this Grid g, ref Point location, CompassPoint direction, out int length)
        {
            length = 0;
            while (true)
            {
                location += direction.ToPoint();
                length++;
                if (!g.Bounds.Contains(location))
                {
                    return null;
                }

                List<MapObject> objs = g.ObjectsIntersecting(new(location, new(1)));
                MapObject obj = objs.Where((search) => search.Entity.HasCollision).First();
                if (obj != null)
                {
                    return obj;
                }


            }



        }


    }
}
