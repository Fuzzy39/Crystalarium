using CrystalCore.Model.CoreContract;
using Microsoft.Xna.Framework;



namespace CrystalCore.Model.ObjectContract
{
    public interface ComponentFactory
    {

        public Chunk CreateChunk(Point chunkCoords);

        public MapObject CreateObject(Point position, Entity entity);

        public bool IsValidPosition(Point position, Entity entity);

        public bool IsValidPosition(Rectangle bounds, bool hasCollision);

       

    }
}
