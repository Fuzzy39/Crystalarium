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
        cw,
        ccw
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

        public static Direction FromRadians(float rad)
        {
            rad = MathHelper.WrapAngle(rad);

            if (rad < -Math.PI * .75f)
            {
                return Direction.left;
            }

            if (rad < -Math.PI * .25f)
            {
                return Direction.up;
            }

            if (rad < Math.PI * .25f)
            {
                return Direction.right;
            }

            if (rad < Math.PI * .75f)
            {
                return Direction.down;
            }

            return Direction.left;
        }




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

        /// <summary>
        /// Whether this direction points in the positive direction of the X or Y axis.
        /// returns true for right and down.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsPositive(this Direction d)
        {
            return d == Direction.right || d == Direction.down;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <returns>returns true if cp has no negative component</returns>
        public static bool IsPositive(this CompassPoint cp)
        {
            Point p = cp.ToPoint();
            return p.X > 1 && p.Y > 1;
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
        // the code contains more trig than you might expect.
        // not like an enormous amount, but I bet you'd expect none.
        public static float ToRadians(this Direction d)
        {

            return d.ToCompassPoint().ToRadians();

        }

        public static Direction Rotate(this Direction d, RotationalDirection r)
        {
            // it's big, 'cause I'm lazy
            if (r == RotationalDirection.cw)
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
            if (r == RotationalDirection.cw)
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
                toReturn = Enum.GetNames(typeof(CompassPoint)).Length - 1;
            }

            return (CompassPoint)toReturn;

        }

        public static CompassPoint Opposite(this CompassPoint cp)
        {
            for (int i = 0; i < 4; i++)
            {
                cp = cp.Rotate(RotationalDirection.cw);
            }
            return cp;
        }

        public static CompassPoint ToCompassPoint(this Direction d)
        {
            return d switch
            {
                Direction.up => CompassPoint.north,

                Direction.down => CompassPoint.south,

                Direction.left => CompassPoint.west,

                Direction.right => CompassPoint.east,

                // default
                _ => CompassPoint.north,


            };
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


        public static Point ToPoint(this CompassPoint d)
        {
            Point p = new Point(0);
            switch (d)
            {

                case CompassPoint.north:
                    p.Y = -1;
                    break;
                case CompassPoint.northeast:
                    p.Y = -1;
                    p.X = 1;
                    break;
                case CompassPoint.northwest:
                    p.Y = -1;
                    p.X = -1;
                    break;
                case CompassPoint.south:
                    p.Y = 1;
                    break;
                case CompassPoint.southeast:
                    p.Y = 1;
                    p.X = 1;
                    break;
                case CompassPoint.southwest:
                    p.Y = 1;
                    p.X = -1;
                    break;
                case CompassPoint.west:
                    p.X = -1;
                    break;
                case CompassPoint.east:
                    p.X = 1;
                    break;
            }


            return p;
        }

        public static bool IsDiagonal(this CompassPoint cp)
        {
            // by golly, this is wizardry, ain't it?
            // even CompassPoints are Orthagonal, odds are diagonal.
            return ((int)cp) % 2 != 0;

        }

        /// <summary>
        /// Returns the angle a compsspoint represents in radians, where North is zero and clockwise is positive.
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static float ToRadians(this CompassPoint cp)
        {
            float toReturn = -MathF.PI / 2f;

            foreach (int i in Enum.GetValues<CompassPoint>())
            {
                CompassPoint point = (CompassPoint)i;

                if (point == cp)
                {
                    return MathHelper.WrapAngle(toReturn);
                }

                toReturn += MathF.PI / 4f;
            }

            throw new InvalidOperationException("This should neer happen - this method done borked");
        }
    }



}
