using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Crystalarium.Util
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
            switch (d)
            {

                case Direction.up:
                    return Direction.down;

                case Direction.down:
                    return Direction.up;

                case Direction.left:
                    return Direction.right;

                case Direction.right:
                    return Direction.left;
            }

            // this should never happen.
            return Direction.up;

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
