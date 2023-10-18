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

        public event EventHandler? OnMapObjectDestroyed;
        public event EventHandler? OnMapObjectReady;
        public event EventHandler? OnReset;
       

        public void Reset();
        public void Reset(Rectangle minimumBounds);
    }
}