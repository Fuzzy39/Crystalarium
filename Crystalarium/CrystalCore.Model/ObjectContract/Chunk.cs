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
        public int Size { get; }


        public Point ChunkCoords 
        { 
            get
            {
                return new Point(Bounds.X/Size, Bounds.Y/Size);
            }        
        }

        public Rectangle Bounds { get; }

        public List<MapObject> ObjectsWithin { get; }




    }
}
