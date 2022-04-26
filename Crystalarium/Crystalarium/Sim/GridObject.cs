using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Crystalarium.Sim
{
    abstract class GridObject
    {

        /*
         * A GridObject represents an object on the grid. This could include many things, like chunks and agents.
         */

        protected Rectangle _bounds;// the position and size in tile space where this GridObject is located.
        protected Grid _parent; // the grid that this object belongs to.


        public Rectangle Bounds
        {
            get => _bounds;
            // I'm not sure if setting this is a good idea, so, I won't allow it for now.
        }

        public Grid Parent
        {
            get => _parent;
        }


        public GridObject(Grid g, Rectangle rect)
        {
            _bounds = rect;
            _parent = g;

            _parent.Add(this);

        }
        
        public void Destroy()
        {
            // remove references to this object.
            _parent.Remove(this);
            _bounds = new Rectangle(0,0,0,0);
            _parent = null;
        }

        public GridObject(Grid g, Point pos, Point size)
            : this(g, new Rectangle(pos, size)) { }


        public GridObject(Grid g, int x, int y, int width, int height)
            : this(g, new Rectangle(x, y, width, height)) { }
        
    }
}
