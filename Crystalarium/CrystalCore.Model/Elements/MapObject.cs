using Microsoft.Xna.Framework;
using System;

namespace CrystalCore.Model.Elements
{
    abstract public class MapObject : IDestroyable
    {

        /*
         * A GridObject represents an object on the grid. This could include many things, like chunks and agents.
         */

        private Rectangle _bounds;// the position and size in tile space where this GridObject is located.
        protected Map _map; // the grid that this object belongs to.
        private bool _destroyed;

        public event EventHandler OnDestroy;
        public event EventHandler OnCreate;

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

      

        public Map Map
        {
            get
            {
                if (_map == null)
                {
                    throw new InvalidOperationException("the grid of a gridobject was null? werid...");
                }

                return _map;
            }
        }

        public bool Destroyed
        {
            get { return _destroyed; }

        }


        // constructors
        public MapObject(Map m, Rectangle rect)
        {
            if (m == null)
            {
                throw new ArgumentException("grid cannot be null for gridobject");
            }

            if (rect.Size.X < 1 || rect.Size.Y < 1)
            {
                throw new ArgumentException("GridObjects must have size.");
            }

            _bounds = rect;
            _map = m;
            _destroyed = false;

            // our grid should be notified when we are destroyed.
            OnCreate += m.OnObjectCreated;
            OnCreate(this, new EventArgs());

            OnDestroy += m.OnObjectDestroyed;

        }

        public MapObject(Map m, Point pos, Point size)
          : this(m, new Rectangle(pos, size)) { }


        public MapObject(Map m, int x, int y, int width, int height)
            : this(m, new Rectangle(x, y, width, height)) { }


        public virtual void Destroy()
        {
            // remove references to this object.

            if (OnDestroy != null)
            {
                OnDestroy(this, new EventArgs());
            }
            _bounds = new Rectangle(0, 0, 0, 0);
            _map = null;
            _destroyed = true;

        }


        public override string ToString()
        {
            return "GridObject { Bounds: " + _bounds + "}";
        }


    }
}
