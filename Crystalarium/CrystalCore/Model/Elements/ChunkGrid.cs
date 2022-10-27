using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CrystalCore.Model.Elements
{
    /// <summary>
    /// A Chunk Grid is a rectangular 2D plane of evenly spaced chunks. it can be expanded in any direction.
    /// </summary>
    public abstract class ChunkGrid
    {
        private List<List<Chunk>> _chunks; // a 2d array where the outer array represents rows and the inner array represents columns. [x][y]

        private Point chunksOrigin; // the chunk coords where the chunk array, chunks, starts.
        private Point chunksSize; // the size, in chunks, of the grid.


        public event EventHandler OnReset;


        internal List<List<Chunk>> Chunks
        {
            get => _chunks;
        }

        internal List<Chunk> ChunkList
        {
            get
            {
                List<Chunk> ToReturn = new List<Chunk>();
                foreach (List<Chunk> chunks in _chunks)
                {
                    ToReturn.AddRange(chunks);

                }
                return ToReturn;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle
                  (chunksOrigin.X * Chunk.SIZE,
                    chunksOrigin.Y * Chunk.SIZE,
                    chunksSize.X * Chunk.SIZE,
                    chunksSize.Y * Chunk.SIZE);
            }

        }

        public Point gridSize
        {
            get => chunksSize;
        }


        public Vector2 Center
        {
            get
            {
                // the center tile coords of this grid
                return chunksSize.ToVector2() * Chunk.SIZE / 2f;

            }


        }

        internal ChunkGrid()
        {

        }


        /// <summary>
        /// should be called in child's constructor.
        /// </summary>
        public void Reset()
        {
            // remove any existing chunks.
            if (_chunks != null)
            {
                foreach (List<Chunk> list in _chunks)
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
            _chunks = new List<List<Chunk>>();
            _chunks.Add(new List<Chunk>());

            // create initial chunk.
            _chunks[0].Add(new Chunk(this, new Point(0, 0)));

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
            List<Chunk> newList = new List<Chunk>();
            chunksSize.X++;
            int x = IndexOfAddition(d);


            if (d == Direction.left)
            {
                _chunks.Insert(x, new List<Chunk>());
                chunksOrigin.X--;
            }
            else
            {
                _chunks.Add(newList);
            }


            // generate the new chunks
            for (int y = 0; y < chunksSize.Y; y++)
            {
                Point gridLoc = new Point(x, y) + chunksOrigin;
                Chunk ch = new Chunk(this, gridLoc);
                _chunks[x].Add(ch);

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
            for (int x = 0; x < _chunks.Count; x++)
            {


                if (d == Direction.up)
                {

                    _chunks[x].Insert(0, null);
                }
                else
                {

                    _chunks[x].Add(null);

                }

                // including chunk origins is important to get the correct coords for this chunk.
                Chunk ch = new Chunk(this, new Point(x + chunksOrigin.X, y + chunksOrigin.Y));

                _chunks[x][y] = ch;

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
                return _chunks.Count;
            }

            return _chunks[0].Count;
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
