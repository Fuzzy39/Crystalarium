using CrystalCore.Model.Grids;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{
    /// <summary>
    /// An Entity is a ChunkMember that faces a directaion.
    /// An Entity's bounds may not coincide with those of another entity. They are 'solid objects' on the grid.
    /// </summary>
    public abstract class Entity : ChunkMember
    {

        private Direction _facing; // the direction this entity is facing.

      
        // properties
        public Direction Facing
        {
            get => _facing;
            protected set
            {
                if (value == _facing) { return; }

                if (IsRectangle())
                {
                    if (_facing != value.Opposite())
                    {
                        // reasoning: there would have to be a check for whether anything was in the way, and the entity's bounds would have to change
                        throw new ArgumentException("Rectangular Agents may not be rotated freely. " +
                            "\nThey are limited to the direction they were placed with and their opposite.");
                    }

                }

                _facing = value;
            }
        }

        public Entity(Grid g, Rectangle bounds, Direction facing) : base(g, bounds)
        {
            _facing = facing;

            if (g.EntitiesWithin(bounds).Count > 1) // it will always be at least 1, because we are in our bounds.
            {
                throw new InvalidOperationException("Entity: " + this + " cannot be created. It overlaps another prexisting entity.");
            }
        }



        private bool IsRectangle()
        {
            return Bounds.Width != Bounds.Height;
        }

        public virtual void Rotate(RotationalDirection d)
        {
            
            if (IsRectangle())
            {
                Facing = Facing.Opposite();
           
                return;
            }

            Facing = Facing.Rotate(d);
          
        }

    }
}
