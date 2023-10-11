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

        public OldGrid(Chunk initial)
        {
            // initialize the chunk array.
            _elements = new List<List<T>>();
            _elements.Add(new List<T>());

            // create initial chunk.
            _elements[0].Add(firstElement);

            // set the chunk origin.
            _origin = new Point(0, 0);
            _size = new Point(1, 1);
        }
    }
}
