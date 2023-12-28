using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Core
{
    /// <summary>
    /// The physical component of a map.
    /// </summary>
    public interface Grid
    {



        List<List<Chunk>> Chunks { get; }

        List<Chunk> ChunkList { get; }


        Point ChunkOrigin { get; }


        Point ChunkSize { get; }

        Rectangle Bounds { get; }

        ComponentFactory ComponentFactory { get; }

        event EventHandler? OnResize;


        void Destroy();

        void Expand(Direction d);

        void ExpandToFit(Rectangle rect);

        public Point TileToChunkCoords(Point tileCoords);

        public List<Chunk> ChunksIntersecting(Rectangle bounds);

        public Chunk ChunkAtCoords(Point tileCoords);

        public List<MapObject> ObjectsIntersecting(Rectangle bounds);

        public MapObject FindClosestObjectInDirection(ref Point location, CompassPoint direction);
    }
}