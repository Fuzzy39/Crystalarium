﻿using Microsoft.Xna.Framework;

namespace CrystalCore.Model.ObjectContract
{
    /// <summary>
    /// A MapObject represents a physical region on the map. It cooresponds to an entity which has behavior in the simulation.
    /// </summary>
    public interface MapObject : MapComponent
    {

        public event EventHandler OnReady;

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

        /// <summary>
        /// Called when it's Entity is initialized. Probably.
        /// </summary>
        protected void Ready();

    }
}
