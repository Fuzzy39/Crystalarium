using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Physical
{
    /// <summary>
    /// A MapObject represents a physical region on the map. It cooresponds to an entity which has behavior in the simulation.
    /// </summary>
    public interface MapObject : MapComponent
    {



        public Rectangle Bounds { get; }

        public Entity Entity { get; }


        // the chunk that the top left corner of this MapObject is in.
        public Chunk Parent { get; }


        public bool IsRepresentationOf(Entity entity)
        {
            return entity == Entity;
        }

        public string ToString()
        {
            return "[ MapObject @ " + Bounds + " ]";
        }



    }
}
