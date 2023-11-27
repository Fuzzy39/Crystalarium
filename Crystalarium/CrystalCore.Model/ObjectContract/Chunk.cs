
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.ObjectContract
{

    /// <summary>
    /// A chunk represents a square 'chunk' of the map. It is used when searching for MapObjects. Chunks are responsible for the MapObjects in their borders.
    /// </summary>
    public interface Chunk:MapComponent
    {

        /// <summary>
        /// The Size of a chunk, in tiles.
        /// </summary>
        public const int SIZE = 16; // essentially arbitrary, other than the textures I made for chunks assume this
            

        public Point ChunkCoords 
        {
            get;     
        }

        public Rectangle Bounds 
        {
            
            get
            {
                return new(ChunkCoords.X * SIZE, ChunkCoords.Y * SIZE, SIZE, SIZE);
            }
        }

        public List<MapObject> ObjectsIntersecting { get; }

     

        /// <summary>
        /// Register a Map object with a particular chunk. The map object should intersect with the chunk's boundries.
        /// This should be done before the MapObject is ready.
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterObject(MapObject obj);   


    }
}
