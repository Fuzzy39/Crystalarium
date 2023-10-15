using CrystalCore.Model.ObjectContract;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Core
{
    /// <summary>
    /// DefaultMap represents a map made of 
    /// </summary>
    public class DefaultMap : Map
    {



        private Grid _grid;



        private Ruleset _ruleset; // the ruleset this grid is following.



        public event EventHandler OnReset;
        public event EventHandler OnResize;
        public event EventHandler OnMapObjectReady;
        public event EventHandler OnMapObjectDestroyed;


        public Ruleset Ruleset
        {
            get => _ruleset;
            set
            {
                _ruleset = value;

                Reset();

            }
        }


        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(_grid.Origin, _grid.Size);
            }

        }

        public Vector2 Center
        {
            get
            {
                // the center tile coords of this grid
                return _grid.Size.ToVector2() / 2f + Bounds.Location.ToVector2();

            }


        }

        public DefaultMap(Ruleset r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("Null Ruleset not viable.");
            }
            _ruleset = r;
            Reset();




        }


        public void Reset()
        {
            Reset(new Rectangle(0, 0, 0, 0));
        }

        public void Reset(Rectangle minimumBounds)
        {




            // reseting our grid should remove all references to any remaining mapObjects
            _grid.Destroy();


            _grid = new Grid(_ruleset.ComponentFactory);
            this.ExpandToFit(minimumBounds);



            if (OnReset != null)
            {
                OnReset(this, new EventArgs());

            }
        }

        /// <summary>
        /// Grow the map in a particular direction.
        /// </summary>
        /// <param name="d"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Expand(Direction d)
        {
            _grid.Expand(d);

            if (OnResize != null)
            {
                OnResize(this, new EventArgs());
            }
        }

        public void ExpandToFit(Rectangle rect)
        {
            // First: which way to expand?
            while (rect.Y < Bounds.Y)
            {
                _grid.Expand(Direction.up);
            }

            while (rect.X < Bounds.X)
            {
                _grid.Expand(Direction.left);
            }

            while (rect.Right > Bounds.Right)
            {
                _grid.Expand(Direction.right);
            }

            while (rect.Bottom > Bounds.Bottom)
            {
                _grid.Expand(Direction.down);
            }

            if (OnResize != null)
            {
                OnResize(this, new EventArgs());
            }
        }

        internal void OnObjectDestroyed(object o, EventArgs e)
        {

            // Remove a grid object from it's appropriate containers.
            OnMapObjectDestroyed?.Invoke(o, new());

        }

        internal void OnObjectReady(object o, EventArgs e)
        {
            OnMapObjectReady?.Invoke(o, new());
        }





    }
}
