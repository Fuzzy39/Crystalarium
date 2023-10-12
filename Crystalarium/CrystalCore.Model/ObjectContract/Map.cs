using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.ObjectContract
{
    public interface Map
    {
        Rectangle Bounds { get; }
        Vector2 Center { get; }
        Ruleset Ruleset { get; set; }

        event EventHandler OnMapObjectDestroyed;
        event EventHandler OnMapObjectReady;
        event EventHandler OnReset;
        event EventHandler OnResize;

        void Expand(Direction d);
        void ExpandToFit(Rectangle rect);
        void Reset();
        void Reset(Rectangle minimumBounds);
    }
}