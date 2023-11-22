using CrystalCore.Model.CoreContract;
using CrystalCore.Model.ObjectContract;
//using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Core
{
    /// <summary>
    /// DefaultMap represents a map made of 
    /// </summary>
    public class DefaultMap : Map
    {



        private DefaultGrid _grid;



        //private Ruleset _ruleset; // the ruleset this grid is following.



        public event EventHandler? OnReset;
        public event MapObjectEvent? OnMapObjectReady;
        public event ComponentEvent? OnMapComponentDestroyed;


        public Grid Grid 
        {
            get => _grid;
        }

        /*public Ruleset Ruleset
        {
            get => _ruleset;
            set
            {
                _ruleset = value;

                Reset();

            }
        }*/



        public DefaultMap(/*Ruleset r*/)
        {
            /*if (r == null)
            {
                throw new ArgumentNullException("Null Ruleset not viable.");
            }
            _ruleset = r;*/
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
            //_grid = new DefaultGrid(_ruleset.ComponentFactory);
            _grid.ExpandToFit(minimumBounds);

             OnReset?.Invoke(this, new EventArgs());

            
        }

       // These handle the events of MapComponents.

      

        void Map.OnComponentDestroyed(MapComponent component, EventArgs e)
        {
            component.OnDestroy -= ((Map)this).OnComponentDestroyed;
            OnMapComponentDestroyed?.Invoke(component, e);
        }

        void Map.OnObjectReady(MapObject mapObj, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
