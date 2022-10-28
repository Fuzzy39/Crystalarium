using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Util
{
    internal class Grid<T>
    {
        private List<List<T>> _elements; // a 2d array where the outer array represents rows and the inner array represents columns. [x][y]

        
        private Point _gridSize; // the size, in chunks, of the grid.
        private Point _gridOrigin; // the chunk coords where the chunk array, chunks, starts.


        public event EventHandler OnReset;


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

        

        public Point GridSize
        {
            get => _gridSize;
        }


        public Vector2 Center
        {
            get
            {
                // the center tile coords of this grid
                return chunksSize.ToVector2() * Chunk.SIZE / 2f;

            }


        }

        internal Grid()
        {
                
        }


        /// <summary>
        /// should be called in child's constructor.
        /// </summary>
        public void Reset()
        {
            // remove any existing chunks.
            if (_elements != null)
            {
                foreach (List<Chunk> list in _elements)
                {
                    foreach (Chunk ch in list)
                    {
                        ch.Destroy();
                    }
                }

            }

            // do (re)initialization
            Initialize();

            // alert others that we have reset.
            if (OnReset != null)
            {
                OnReset(this, new EventArgs());
            }
        }

        private void Initialize()
        {
            // initialize the chunk array.
            _elements = new List<List<Chunk>>();
            _elements.Add(new List<Chunk>());

            // create initial chunk.
            _elements[0].Add(new Chunk(this, new Point(0, 0)));

            // set the chunk origin.
            chunksOrigin = new Point(0, 0);
            chunksSize = new Point(1, 1);
        }


        public virtual void ExpandGrid(Direction d)
        {
            if (d.IsHorizontal())
            {
                ExpandHorizontal(d);
            }
            else
            {
                ExpandVertical(d);
            }

        }

        private void ExpandHorizontal(Direction d)
        {
            // we are adding a new list<Chunk> to _chunks.
            List<T> newList = new List<T>();
            _gridSize.X++;
            int x = IndexOfAddition(d);


            if (d == Direction.left)
            {
                _elements.Insert(x, new List<T>());
                _gridOrigin.X--;
            }
            else
            {
                _elements.Add(newList);
            }


            // generate the new chunks
            for (int y = 0; y < GridSize.Y; y++)
            {
                Point gridLoc = new Point(x, y) + _gridOrigin;
                Chunk ch = new Chunk(this, gridLoc);
                _elements[x].Add(ch);

            }

        }


        private void ExpandVertical(Direction d)
        {
            // we are adding a new Chunk to every list<Chunk> in _chunk.
            chunksSize.Y++;


            if (d == Direction.up)
                chunksOrigin.Y--;

            // create the new chunks.
            int y = IndexOfAddition(d);
            for (int x = 0; x < _elements.Count; x++)
            {


                if (d == Direction.up)
                {

                    _elements[x].Insert(0, null);
                }
                else
                {

                    _elements[x].Add(null);

                }

                // including chunk origins is important to get the correct coords for this chunk.
                Chunk ch = new Chunk(this, new Point(x + chunksOrigin.X, y + chunksOrigin.Y));

                _elements[x][y] = ch;

            }
        }

        private int IndexOfAddition(Direction d)
        {
            if (d == Direction.up || d == Direction.left)
            {
                return 0;
            }

            if (d == Direction.right)
            {
                return _elements.Count;
            }

            return _elements[0].Count;
        }


        internal virtual void OnObjectDestroyed(object sender, EventArgs e)
        {

            if (!(sender is GridObject))
            {
                throw new ArgumentException("sender must be GridObject.");
            }

            GridObject o = (GridObject)sender;

            if (o is Chunk)
            {
                return;
            }

            throw new ArgumentException("Unknown or Invalid type of GridObject to remove from this grid.");
        }
    }
}
