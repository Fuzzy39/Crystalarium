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

    public enum RotationalDirection
    {
        clockwise,
        counterclockwise
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

        // What? Radians? really?
        // Yes, really. We do need this.
        public static float ToRadians(this Direction d)
        {
            return d switch
            {
                Direction.up => 0,

                Direction.right => MathF.PI/2f,

                Direction.down => MathF.PI,

                Direction.left => MathF.PI*3f/2f,

                // default
                _ => 0,

            };
        }

        public static Direction Rotate(this Direction d, RotationalDirection r)
        {
            // it's big, 'cause I'm lazy
            if(r == RotationalDirection.clockwise)
            {
                return d switch
                {
                    Direction.up => Direction.right,

                    Direction.down => Direction.left,

                    Direction.left => Direction.up,

                    Direction.right => Direction.right,

                    // default
                    _ => Direction.up,

                };
            }
            else
            {
                return d switch
                {
                    Direction.up => Direction.left,

                    Direction.down => Direction.right,

                    Direction.left => Direction.down,

                    Direction.right => Direction.up,

                    // default
                    _ => Direction.up,


                };
            }

            
        }
    }

}
