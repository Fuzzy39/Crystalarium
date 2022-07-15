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

    public enum CompassPoint
    {
        north,
        northeast,
        east,
        southeast,
        south,
        southwest,
        west,
        northwest
    }

    public static class DirectionUtil
    {
        public static bool IsVertical(this Direction d)
        {
            if (d == Direction.up || d == Direction.down)
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

                Direction.right => MathF.PI / 2f,

                Direction.down => MathF.PI,

                Direction.left => MathF.PI * 3f / 2f,

                // default
                _ => 0,

            };
        }

        public static Direction Rotate(this Direction d, RotationalDirection r)
        {
            // it's big, 'cause I'm lazy
            if (r == RotationalDirection.clockwise)
            {
                return d switch
                {
                    Direction.up => Direction.right,

                    Direction.down => Direction.left,

                    Direction.left => Direction.up,

                    Direction.right => Direction.down,

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

        public static CompassPoint Rotate(this CompassPoint cp, RotationalDirection r)
        {
            int toReturn = (int)cp;
            if(r == RotationalDirection.clockwise)
            {
                toReturn++;
                if (toReturn >= Enum.GetNames(typeof(CompassPoint)).Length)
                { 
                    toReturn = 0; 
                }

                return (CompassPoint)toReturn;

               
            }

            toReturn--;
            if (toReturn < 0)
            {
                toReturn = Enum.GetNames(typeof(CompassPoint)).Length-1;
            }

            return (CompassPoint)toReturn;

        }


        public static Direction? ToDirection(this CompassPoint p)
        {

            return p switch
            {
                CompassPoint.west => Direction.left,

                CompassPoint.east => Direction.right,

                CompassPoint.south => Direction.down,

                CompassPoint.north => Direction.up,

                // default
                _ => null


            };
        }
    }

}
