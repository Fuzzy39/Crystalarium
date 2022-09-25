using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Model.Grids;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Objects
{
    abstract public class GridObject
    {

        /*
         * A GridObject represents an object on the grid. This could include many things, like chunks and agents.
         */

        private Rectangle _bounds;// the position and size in tile space where this GridObject is located.
        protected ChunkGrid _grid; // the grid that this object belongs to.
        private bool _destroyed;

        public event EventHandler OnDestroy;

        public virtual Rectangle Bounds
        {
            get => _bounds;
            protected set
            {
                if (value.Width * value.Height == 0)
                {
                    throw new ArgumentException("GridObjects must have size.");
                }
                _bounds = value;
            }
        }

        public ChunkGrid ChunkGrid
        {
            get => _grid;
        }

        public Grid Grid
        {
            get
            {
                if(_grid is ChunkGrid)
                {
                    return (Grid)_grid;
                }

                throw new InvalidOperationException("I feel like grids should be the only type of grid... If that no longer makes sense, remove this.");
            }
        }

        public bool Destroyed
        {
            get { return _destroyed; }
        }


        // constructors
        public GridObject(ChunkGrid g, Rectangle rect)
        {
            if(g == null)
            {
                throw new ArgumentException("grid cannot be null for gridobject");
            }

            if(rect.Size.X<1 || rect.Size.Y<1 )
            {
                throw new ArgumentException("GridObjects must have size.");
            }

            _bounds = rect;
            _grid = g;
            _destroyed = false;

            // our grid should be notified when we are destroyed.
            OnDestroy += g.OnObjectDestroyed;

        }

        public GridObject(ChunkGrid g, Point pos, Point size)
          : this(g, new Rectangle(pos, size)) { }


        public GridObject(ChunkGrid g, int x, int y, int width, int height)
            : this(g, new Rectangle(x, y, width, height)) { }


        public virtual void Destroy()
        {
            // remove references to this object.

            if (OnDestroy != null)
            {
                OnDestroy(this, new EventArgs());
            }
            _bounds = new Rectangle(0,0,0,0);
            _grid = null;
            _destroyed= true;
            
        }


        public override string ToString()
        {
            return "GridObject { Bounds: " + _bounds+"}";
        }


    }
}
