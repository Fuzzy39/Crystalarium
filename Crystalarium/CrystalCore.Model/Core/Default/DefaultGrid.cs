using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Core.Default
{
    internal class DefaultGrid : Grid

    {

        private List<List<Chunk>> _chunks;

        private Point _size; // the size, in chunks, of the grid.
        private Point _origin; // the chunk index that has the chunk with coords (0,0)

        private ComponentFactory _factory; // A nice factory to bake more chunks.


        public event EventHandler? OnResize;

        public List<List<Chunk>> Chunks
        {
            get => _chunks;
        }

        public List<Chunk> ChunkList
        {
            get
            {
                List<Chunk> ToReturn = new();
                foreach (List<Chunk> elements in _chunks)
                {
                    ToReturn.AddRange(elements);

                }
                return ToReturn;
            }
        }


        /// <summary>
        /// The size of the grid, in chunks.
        /// </summary>
        public Point ChunkSize
        {
            get => _size;
        }


        /// <summary>
        /// The top left most coordinate in the grid, in chunks.
        /// </summary>
        public Point ChunkOrigin
        {
            get => _origin;
        }




        /// <summary>
        /// The bounds of this grid in tile coordinates.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(new(_origin.X * Chunk.SIZE,
                   _origin.Y * Chunk.SIZE), new(_size.X * Chunk.SIZE, _size.Y * Chunk.SIZE));
            }

        }

        public ComponentFactory ComponentFactory => _factory;


        public DefaultGrid(ComponentFactory factory)
        {

            _factory = factory;

            // set the chunk origin.
            _origin = new Point(0, 0);
            _size = new Point(1, 1);


            // initialize the chunk array.
            _chunks = new List<List<Chunk>> { new() };


            // create initial chunk.
            _chunks[0].Add(_factory.CreateChunk(new(0, 0)));

        }

        public void Destroy()
        {
            foreach (List<Chunk> elements in _chunks)
            {
                foreach (Chunk chunk in elements)
                {
                    chunk.Destroy();
                }
            }

            _chunks.Clear();
        }


        /// <summary>
        /// Grow the grid in a particular direction.
        /// </summary>
        /// <param name="d"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Expand(Direction d)
        {

            Point start = DetermineStartingCoord(d);

            Chunk[] toAdd = CreateChunks(d, start);


            if (d == Direction.right || d == Direction.down)
            {
                Array.Reverse(toAdd);
            }

            AddElements(toAdd, d);

            if (OnResize != null)
            {
                OnResize(this, new EventArgs());
            }

        }


        public void ExpandToFit(Rectangle rect)
        {
            bool expanded = false;
            // First: which way to expand?
            while (rect.Y < Bounds.Y)
            {
                expanded = true;
                Expand(Direction.up);
            }

            while (rect.X < Bounds.X)
            {
                expanded = true;
                Expand(Direction.left);
            }

            while (rect.Right > Bounds.Right)
            {
                expanded = true;
                Expand(Direction.right);
            }

            while (rect.Bottom > Bounds.Bottom)
            {
                expanded = true;
                Expand(Direction.down);
            }

            if (OnResize != null && expanded)
            {
                OnResize(this, new EventArgs());
            }
        }


        private Point DetermineStartingCoord(Direction d)
        {
            // figure out a sensible starting point in grid space.
            // if we switched on direction and made each one make sense, then 

            if (d == Direction.up || d == Direction.left)
            {
                return _chunks[0][0].ChunkCoords + d.ToPoint();
            }

            return _origin + (_size - new Point(1)) + d.ToPoint();

        }


        private Chunk[] CreateChunks(Direction d, Point start)
        {
            Chunk[] toAdd;

            // get the size of the new entries.
            if (d.IsVertical())
            {
                // create an array!
                toAdd = new Chunk[_size.X];
            }
            else
            {
                toAdd = new Chunk[_size.Y];

            }

            // calculate the direction we need to move from our starting point to generate the required chunks
            Direction counting = d.IsVertical() ? d.Rotate(RotationalDirection.cw) : d.Rotate(RotationalDirection.ccw);
            Point traverse = counting.ToPoint();

            // make all the chunks
            for (int i = 0; i < toAdd.Length; i++)
            {

                // calculate our distance from our start
                Point add = new(i * traverse.X, i * traverse.Y);
                Point coord = start + add;
                // create the chunk, add it to the list.
                toAdd[i] = _factory.CreateChunk(coord);
            }

            return toAdd;
        }

        private void AddElements(Chunk[] elements, Direction d)
        {
            if (d.IsHorizontal())
            {
                if (elements.Length != _chunks[0].Count)
                {
                    throw new ArgumentException("The number of elements added must be equal to the height of the array when adding on the " + d + " side.");
                }
                // do the thing
                AddHorizontal(elements, d);
                return;
            }

            if (elements.Length != _chunks.Count)
            {
                throw new ArgumentException("The number of elements added must be equal to the height of the array when adding on the " + d + " side.");
            }

            // do the signifigantly more annoying thing
            AddVertical(elements, d);
        }


        private void AddHorizontal(Chunk[] elements, Direction d)
        {
            // we are adding a new list<Chunk> to _chunks.
            List<Chunk> newList = new List<Chunk>(elements);
            _size.X++;

            if (d == Direction.left)
            {
                _chunks.Insert(0, newList);
                _origin.X--;
                return;
            }

            _chunks.Add(newList);

        }


        private void AddVertical(Chunk[] elements, Direction d)
        {
            // we are adding a new Chunk to every list<Chunk> in _chunk.
            _size.Y++;

            if (d == Direction.up)
                _origin.Y--;

            // create the new chunks.
            for (int x = 0; x < _chunks.Count; x++)
            {
                if (d == Direction.up)
                {

                    _chunks[x].Insert(0, elements[x]);
                    continue;
                }

                _chunks[x].Add(elements[x]);

            }

        }

        public Point TileToChunkCoords(Point tileCoords)
        {
            // I didn't feel like writing a whole other private function, okay?
            Func<int, int> roundDown = (a) => (int)MathF.Floor((float)a / Chunk.SIZE);

            return new(roundDown(tileCoords.X), roundDown(tileCoords.Y));
        }

        public List<Chunk> ChunksIntersecting(Rectangle bounds)
        {
            // the rectangle containing all coordinates contained in the grid,as opposed to a rectangle representing the bounds of the grid.

            
            Rectangle coordinateBounds = new(Bounds.Location, Bounds.Size);

            bounds = Rectangle.Intersect(coordinateBounds, bounds);
            bounds = new(bounds.Location, bounds.Size - new Point(1));

            Point least = TileToChunkCoords(bounds.Location) - ChunkOrigin;
            Point greatest = TileToChunkCoords(bounds.Location + bounds.Size) - ChunkOrigin;

            List<Chunk> toReturn = new List<Chunk>();

            for (int x = least.X; x <= greatest.X; x++)
            {
                for (int y = least.Y; y <= greatest.Y; y++)
                {
                    toReturn.Add(Chunks[x][y]);
                }
            }

            return toReturn;
        }

        public Chunk ChunkAtCoords(Point tileCoords)
        {
            Point chunkCoord = TileToChunkCoords(tileCoords);

            Point chunkIndex = chunkCoord - ChunkOrigin;

            return Chunks[chunkIndex.X][chunkIndex.Y];

        }

        public List<MapObject> ObjectsIntersecting(Rectangle bounds)
        {
            List<Chunk> chunks = ChunksIntersecting(bounds);
            List<MapObject> toReturn = new();

            foreach (Chunk chunk in chunks)
            {
                foreach (MapObject obj in chunk.ObjectsIntersecting)
                {

                    if (obj.Bounds.Intersects(bounds))
                    {
                        // only add a if it isn't already in the list
                        // explaination of line: if element in toReturn exists such that element is obj ...
                        // simpler explanation: if obj is already in the list...
                        if (toReturn.Exists((test) => { return test == obj; }))
                        {
                            continue;
                        }

                        toReturn.Add(obj);
                    }


                }
            }
            return toReturn;

        }


        public MapObject FindClosestObjectInDirection( ref Point location, CompassPoint direction)
        {
      
            while (true)
            {
                location += direction.ToPoint();
              
                if (!Bounds.Contains(location))
                {
                    location -= direction.ToPoint();
                    return null;
                }

                List<MapObject> objs = ObjectsIntersecting(new(location, new(1))).Where((search) => search.Entity.HasCollision).ToList();

                if (objs.Count > 0)
                {
                    return objs.First();
                }


            }



        }

        public override string ToString()
        {
            return "Grid [ size: " + _size + ", top left at position: " + _origin + " (in chunks) ]";
        }

    }
}
