using CrystalCore.Model.Core;
using Microsoft.Xna.Framework;



namespace CrystalCore.Model.Physical
{
    public interface ComponentFactory
    {


        internal Map Map { get; }
        public Chunk CreateChunk(Point chunkCoords);

        public MapObject CreateObject(Point position, Entity entity);

        public bool IsValidPosition(Point position, Entity entity);

        public bool IsValidPosition(Rectangle bounds, bool hasCollision);



    }
}
