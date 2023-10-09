using CrystalCore.Model.Core;

using System.Drawing;


namespace CrystalCore.Model.ObjectContract
{
    /// <summary>
    /// A MapObject represents a physical region on the map. It typically does not have behavior.
    /// </summary>
    internal interface MapObject : MapComponent
    {
        public Rectangle Bounds { get; }

        public Map Map { get; }

        public bool IsValidPosition(Rectangle Bounds);

        public string ToString()
        {
            return "[ MapObject @ " + Bounds + " ]";
        }

    }
}
