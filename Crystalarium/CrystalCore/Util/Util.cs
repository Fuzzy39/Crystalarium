using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Util
{
    static class Util
    {

        // pull 'a' closer to zero by 'b' amount. don't let 'a' overshoot zero.

        public static float Reduce(float a, float b)
        {
            // a should remain at zero if it is already there.
            if (a == 0)
            {
                return a;
            }

            b = MathF.Abs(b);
            if (a > 0)
            {
                a -= b;
                if (a < 0)
                {
                    a = 0;
                }

                return a;

            }

            a += b;
            if (a > 0)
            {
                a = 0;
            }

            return a;
        }


        // create a rectangle with points a and b as its corners.
        public static Rectangle RectFromPoints(Point a, Point b)
        {

            // sort everything
            // mildly cursed.
            int lowX = a.X > b.X ? b.X : a.X;
            int highX = a.X < b.X ? b.X : a.X;

            int lowY = a.Y > b.Y ? b.Y : a.Y;
            int highY = a.Y < b.Y ? b.Y : a.Y;

            return new Rectangle(lowX, lowY, highX - lowX, highY - lowY);

        }
    }
}
