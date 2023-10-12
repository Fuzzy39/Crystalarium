
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.ObjectContract
{

    /// <summary>
    /// A chunk represents a square 'chunk' of the map. It is used when searching for MapObjects. Chunks are responsible for the MapObjects in their borders.
    /// </summary>
    internal interface Chunk:MapComponent
    {

        /// <summary>
        /// The Size of a chunk, in tiles.
        /// </summary>
        public const int SIZE = 16; // essentially arbitrary, other than the textures I made for chunks assume this
            

        public Point ChunkCoords 
        { 
            get
            {
                return new Point(Bounds.X/SIZE, Bounds.Y/SIZE);
            }        
        }

        public Rectangle Bounds { get; }

        public List<MapObject> ObjectsWithin { get; }




    }
}
