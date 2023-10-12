using Microsoft.Xna.Framework;



namespace CrystalCore.Model.ObjectContract
{
    public interface ComponentFactory
    {

        public Chunk CreateChunk(Point chunkCoords);

        public MapObject CreateObject();

        public MapObject CreateObjectWithCollision();
       

    }
}
