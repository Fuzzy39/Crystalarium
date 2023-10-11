using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public const int SIZE = 16;


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
