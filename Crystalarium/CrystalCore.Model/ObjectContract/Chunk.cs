
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

        public List<MapObject> ObjectsWithin { get; }




    }
}
