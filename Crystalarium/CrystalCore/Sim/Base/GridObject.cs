using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrystalCore.Sim.Base
{
    abstract public class GridObject
    {

        /*
         * A GridObject represents an object on the grid. This could include many things, like chunks and agents.
         */

        protected Rectangle _bounds;// the position and size in tile space where this GridObject is located.
        protected Grid _grid; // the grid that this object belongs to.


        public Rectangle Bounds
        {
            get => _bounds;
            // I'm not sure if setting this is a good idea, so, I won't allow it for now.
        }

        public Grid Grid
        {
            get => _grid;
        }


        // constructors
        public GridObject(Grid g, Rectangle rect)
        {
            if(rect.Size.X<1 || rect.Size.Y<1 )
            {
                throw new ArgumentException("GridObjects must have size.");
            }

            _bounds = rect;
            _grid = g;

            _grid.Add(this);

        }

        public GridObject(Grid g, Point pos, Point size)
          : this(g, new Rectangle(pos, size)) { }


        public GridObject(Grid g, int x, int y, int width, int height)
            : this(g, new Rectangle(x, y, width, height)) { }


        public void Destroy()
        {
            // remove references to this object.
            _grid.Remove(this);
            _bounds = new Rectangle(0,0,0,0);
            _grid = null;
            
        }


        public override string ToString()
        {
            return "GridObject { Bounds: " + _bounds+"}";
        }


    }
}
