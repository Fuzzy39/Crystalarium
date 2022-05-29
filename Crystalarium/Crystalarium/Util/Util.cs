using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Util
{
    struct Util
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
    }
}
