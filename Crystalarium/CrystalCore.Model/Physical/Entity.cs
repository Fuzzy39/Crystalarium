using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Physical
{

    /// <summary>
    /// An Entity is responsible for the simulation behavior of a MapOpject.
    /// </summary>
    public interface Entity
    {
        public event EventHandler OnReady;

        public MapObject Physical { get; }

        public bool HasCollision { get; }

        public Point Size { get; }

        public bool Destroyed { get; }

        public void Destroy();

    }
}
