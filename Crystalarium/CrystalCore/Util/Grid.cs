using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CrystalCore.Util
{
    internal class Grid<T>
    {
        private List<List<T>> _elements; // a 2d array where the outer array represents rows and the inner array represents columns. [x][y]

        
        private Point _size; // the size, in chunks, of the grid.
        private Point _origin; // the chunk coords where the chunk array, chunks, starts.


      


        internal List<List<T>> Elements
        {
            get => _elements;
        }

        internal List<T> ElementList
        {
            get
            {
                List<T> ToReturn = new List<T>();
                foreach (List<T> elements in _elements)
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

       

        internal Grid(T firstElement)
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

        public void Clear()
        {
            _elements.Clear();
        }

        public void AddElements(T[] elements, Direction d)
        {
            if(d.IsHorizontal())
            {
                if(elements.Length != _elements[0].Count)
                {
                    throw new ArgumentException("The number of elements added must be equal to the height of the array when adding on the "+d+" side.");
                }
                // do the thing
                AddHorizontal(elements, d);
                return; 
            }

            if (elements.Length != _elements.Count)
            {
                throw new ArgumentException("The number of elements added must be equal to the height of the array when adding on the " + d + " side.");
            }

            // do the signifigantly more annoying thing
            AddVertical(elements, d);
        }

        private void AddHorizontal(T[] elements, Direction d)
        {
            // we are adding a new list<Chunk> to _chunks.
            List<T> newList = new List<T>(elements);
            _size.X++;

            if (d == Direction.left)
            {
                _elements.Insert(0, newList);
                _origin.X--;
            }
            else
            {
                _elements.Add(newList);
            }

        }


        private void AddVertical(T[] elements, Direction d)
        {
            // we are adding a new Chunk to every list<Chunk> in _chunk.
            _size.Y++;


            if (d == Direction.up)
                _origin.Y--;

            // create the new chunks.
            for (int x = 0; x < _elements.Count; x++)
            {


                if (d == Direction.up)
                {

                    _elements[x].Insert(0, elements[x]);
                }
                else
                {

                    _elements[x].Add(elements[x]);

                }

            }
        }
    }
}
