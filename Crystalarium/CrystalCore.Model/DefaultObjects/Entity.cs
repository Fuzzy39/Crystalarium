using CrystalCore.Model.Core;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.DefaultObjects
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

        // the disgusting line there properly rotates the entitiy.
        public Entity(Map g, Point location, Point upwardsSize, Direction facing) : base(g, CalculateBounds(location, upwardsSize, facing))
        {
            _facing = facing;

            if (g.EntitiesWithin(Bounds).Count > 1) // it will always be at least 1, because we are in our bounds.
            {
                throw new InvalidOperationException("Entity with bounds " + Bounds + " cannot be created. It overlaps another prexisting entity.");
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


        public static bool IsValidLocation(Map g, Rectangle bounds)
        {

            if (g.Bounds.Contains(bounds))
            {
                if (g.EntitiesWithin(bounds).Count == 0)
                {
                    return true;
                }

            }

            return false;
        }


        public static bool IsValidLocation(Map g, Point location, Point upwardsSize, Direction facing)
        {
            return IsValidLocation(g, new(location, AdjustSize(upwardsSize, facing)));
        }



        private static Point AdjustSize(Point size, Direction facing)
        {
            if (facing.IsVertical())
            {
                return size;
            }

            return new(size.Y, size.X);
        }


        public static Rectangle CalculateBounds(Point location, Point upwardsSize, Direction facing)
        {
            return new(location, AdjustSize(upwardsSize, facing));
        }


    }
}
