using Microsoft.Xna.Framework;

namespace CrystalCore.Util
{
    public static class MiscUtil
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


        public static float PickGreatest(params float[] nums)
        {

            float toReturn = nums[0];
            foreach (float num in nums)
            {
                if (num > toReturn)
                {
                    toReturn = num;
                }
            }

            return toReturn;
        }

        public static float PickLeast(params float[] nums)
        {

            float toReturn = nums[0];
            foreach (float num in nums)
            {
                if (num < toReturn)
                {
                    toReturn = num;
                }
            }

            return toReturn;
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

        public static string Indent(string s)
        {
            string toReturn = "";
            string[] lines = s.Split('\n');

            foreach (string sub in lines)
            {

                toReturn += "\n    " + sub;
            }
            return toReturn;
        }


        public static string FormatTime(TimeSpan time)
        {
            if (time.TotalDays >= 1)
            {
                return time.Days + "d " + time.Hours + "h";
            }
            if (time.TotalHours >= 1)
            {
                return time.Hours + "h " + time.Minutes + "m";
            }
            if (time.TotalMinutes >= 1)
            {
                return time.Minutes + "m " + time.Seconds + "s";
            }
            if (time.TotalSeconds >= 1)
            {
                return time.Seconds + "s " + time.Milliseconds + "ms";
            }

            if (time.TotalMilliseconds >= 1)
            {
                return time.Milliseconds + "ms " + ((time.Ticks - (time.Milliseconds * 10 * 1000)) / 10) + "us";
            }

            if (time.Ticks != 0)
            {
                return Math.Round(time.Ticks / 1000.0, 1) + "us";
            }

            return "0us";
        }

    }
}
