using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrystalCore.Util
{
    public enum Direction
    {
        up,
        down,
        left,
        right
    }

    static class DirectionUtil
    {
        public static bool IsVertical(this Direction d)
        {
            if(d == Direction.up || d== Direction.down)
            {
                return true;
            }

            return false;
        }

        public static bool IsHorizontal(this Direction d)
        {
            return !IsVertical(d);

        }

        public static Direction Opposite(this Direction d)
        {

            // I didn't know this was a thing in C#. Neat!
            return d switch
            {
                Direction.up => Direction.down,

                Direction.down => Direction.up,

                Direction.left => Direction.right,

                Direction.right => Direction.left,

                // default
                _ => Direction.up,// shouldn't ever happen.
            };
        }

        public static Point ToPoint(this Direction d)
        {
            Point p = new Point(0);
            switch (d)
            {

                case Direction.up:
                    p.Y = -1;
                    break;
                case Direction.down:
                    p.Y = 1;
                    break;

                case Direction.left:
                    p.X = -1;
                    break;
                case Direction.right:
                    p.X = 1;
                    break;
            }

       
            return p;
        }
    }

}
