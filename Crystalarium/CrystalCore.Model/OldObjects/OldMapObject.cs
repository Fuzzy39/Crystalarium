using CrystalCore.Model.Core;
using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.OldObjects
{
    abstract public class OldMapObject : MapComponent
    {

        /*
         * A GridObject represents an object on the grid. This could include many things, like chunks and agents.
         */

        private Rectangle _bounds;// the position and size in tile space where this GridObject is located.
        protected Map _map; // the grid that this object belongs to.
        private bool _destroyed;

        public event EventHandler OnDestroy;
        public event EventHandler OnCreate;
        public event EventHandler OnReady;

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
        public OldMapObject(Map m, Rectangle rect)
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
            OnCreate?.Invoke(this, new EventArgs());

            OnReady += m.OnObjectReady;
            OnDestroy += m.OnObjectDestroyed;

        }

        public OldMapObject(Map m, Point pos, Point size)
          : this(m, new Rectangle(pos, size)) { }


        public OldMapObject(Map m, int x, int y, int width, int height)
            : this(m, new Rectangle(x, y, width, height)) { }


        protected void Ready()
        {
            OnReady?.Invoke(this, new());
        }

        public virtual void Destroy()
        {
            // remove references to this object.


            OnDestroy?.Invoke(this, new EventArgs());

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
