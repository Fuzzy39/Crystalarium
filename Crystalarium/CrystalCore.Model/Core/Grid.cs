using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CrystalCore.Model.Core
{
    internal class Grid
    {

        private List<List<Chunk>> _chunks;

        private Point _size; // the size, in chunks, of the grid.
        private Point _origin;

       
        public List<List<Chunk>> Chunks
        {
            get => _chunks;
        }

        public List<Chunk> ChunkList
        {
            get
            {
                List<Chunk> ToReturn = new List<Chunk>();
                foreach (List<Chunk> elements in _chunks)
                {
                    ToReturn.AddRange(elements);

                }
                return ToReturn;
            }
        }


        public Point Size
        {
            get => _size;
        }

        public Point Origin
        {
            get => _origin;
        }

        public Grid(DefaultMap map)
        {
            

            // set the chunk origin.
            _origin = new Point(0, 0);
            _size = new Point(1, 1);


            // initialize the chunk array.
            _chunks = new List<List<Chunk>> { new() };


            // create initial chunk.
            _chunks[0].Add(map.Factory.CreateChunk(map, new(0,0)));

           
        }
    }
}
