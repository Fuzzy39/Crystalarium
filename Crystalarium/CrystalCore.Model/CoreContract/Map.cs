using CrystalCore.Model.ObjectContract;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.CoreContract
{
    /// <summary>
    /// A Map represents, well, a map.
    /// It's grid represents the physical objects on the map.
    /// </summary>
    public interface Map
    {
     
        public Grid Grid { get; }
        public Ruleset Ruleset { get; set; }

        // these are events that can be subscribed to by, say, the gridview.
        public event ComponentEvent? OnMapComponentDestroyed;
        public event MapObjectEvent? OnMapObjectReady;
        public event EventHandler? OnReset;
       

        public void Reset();
        public void Reset(Rectangle minimumBounds);

        internal void OnComponentDestroyed(MapComponent component, EventArgs e);

        internal void OnObjectReady(MapObject mapObj, EventArgs e);
       
    }

    public delegate void ComponentEvent(MapComponent mc, EventArgs e);

    public delegate void MapObjectEvent(MapObject obj, EventArgs e);


}