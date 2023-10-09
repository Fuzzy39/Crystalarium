using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System;

namespace CrystalCore.Util.Graphics.Tests
{
    [TestClass()]
    public class RotatedRectTests
    {
        //[TestMethod()]
        //public void 



        [TestMethod()]
        public void NoRotationTest()
        {
            // Tests whether points on a given rectangle are as expected.


            // simple - no rotation, all values positive.
            RotatedRect simple = new RotatedRect(new(1), new(2), 0f, new(0));

            Assert.AreEqual(new Vector2(1, 1), simple.TopLeft);
            Assert.AreEqual(new Vector2(2, 1), simple.TopCenter);
            Assert.AreEqual(new Vector2(3, 1), simple.TopRight);

            Assert.AreEqual(new Vector2(1, 2), simple.CenterLeft);
            Assert.AreEqual(new Vector2(2, 2), simple.Center);
            Assert.AreEqual(new Vector2(3, 2), simple.CenterRight);

            Assert.AreEqual(new Vector2(1, 3), simple.BottomLeft);
            Assert.AreEqual(new Vector2(2, 3), simple.BottomCenter);
            Assert.AreEqual(new Vector2(3, 3), simple.BottomRight);

        }

        [TestMethod()]
        public void RightTest()
        {
            // should be right.

            RotatedRect rotated = new RotatedRect(new(0), new(2), MathF.PI / 2f, new(0));
            //Assert.
            AssertClose(new Vector2(0, 0), rotated.TopLeft);
            AssertClose(new Vector2(0, 1), rotated.TopCenter);
            AssertClose(new Vector2(0, 2), rotated.TopRight);

            AssertClose(new Vector2(-1, 0), rotated.CenterLeft);
            AssertClose(new Vector2(-1, 1), rotated.Center);
            AssertClose(new Vector2(-1, 2), rotated.CenterRight);

            AssertClose(new Vector2(-2, 0), rotated.BottomLeft);
            AssertClose(new Vector2(-2, 1), rotated.BottomCenter);
            AssertClose(new Vector2(-2, 2), rotated.BottomRight);



        }


        [TestMethod()]
        public void RectTest()
        {
            // should be right.

            RotatedRect rotated = new RotatedRect(new(0), new(1, 2), MathF.PI / 2f, new(0));
            //rotated = new RotatedRect(new Rectangle(0, 0, 1, 2), Direction.right);
            //Assert.
            AssertClose(new Vector2(0, 0), rotated.TopLeft);
            AssertClose(new Vector2(0, .5f), rotated.TopCenter);
            AssertClose(new Vector2(0, 1), rotated.TopRight);

            AssertClose(new Vector2(-1, 0), rotated.CenterLeft);
            AssertClose(new Vector2(-1, .5f), rotated.Center);
            AssertClose(new Vector2(-1, 1), rotated.CenterRight);

            AssertClose(new Vector2(-2, 0), rotated.BottomLeft);
            AssertClose(new Vector2(-2, .5f), rotated.BottomCenter);
            AssertClose(new Vector2(-2, 1), rotated.BottomRight);



        }

        [TestMethod()]
        public void RectOriginTest()
        {
            // simple test
            RotatedRect fromCenter = new RotatedRect(new Rectangle(25, 25, 50, 50), 0f, new(.5f, .5f));
            AssertClose(new Vector2(0, 0), fromCenter.TopLeft);
            AssertClose(new Vector2(50, 50), fromCenter.BottomRight);

            // x only
            fromCenter = new(new Rectangle(25, 0, 50, 50), 0f, new(.5f, 0));
            AssertClose(new Vector2(0, 0), fromCenter.TopLeft);
            AssertClose(new Vector2(50, 50), fromCenter.BottomRight);

            // y only
            fromCenter = new(new Rectangle(0, 25, 50, 50), 0f, new(0, .5f));
            AssertClose(new Vector2(0, 0), fromCenter.TopLeft);
            AssertClose(new Vector2(50, 50), fromCenter.BottomRight);


            // not centered
            fromCenter = new(new Rectangle(40, 25, 50, 50), 0f, new(.8f, .5f));
            AssertClose(new Vector2(0, 0), fromCenter.TopLeft);
            AssertClose(new Vector2(50, 50), fromCenter.BottomRight);

            // rotated basic
            fromCenter = new(new Rectangle(0, 0, 50, 50), MathF.PI / 4f, new(.5f, .5f));
            AssertClose(new Vector2(0, -25 * MathF.Sqrt(2)), fromCenter.TopLeft);
            AssertClose(new Vector2(0, 25 * MathF.Sqrt(2)), fromCenter.BottomRight);
            AssertClose(new Vector2(25 * MathF.Sqrt(2), 0), fromCenter.TopRight);

            // rotated basic
            fromCenter = new(new Rectangle(0, 0, 50, 50), MathF.PI / 4f, new(.5f, .5f));
            AssertClose(new Vector2(0, -25 * MathF.Sqrt(2)), fromCenter.TopLeft);
            AssertClose(new Vector2(0, 25 * MathF.Sqrt(2)), fromCenter.BottomRight);
            AssertClose(new Vector2(25 * MathF.Sqrt(2), 0), fromCenter.TopRight);

            // all together
            fromCenter = new(new Rectangle(25, 40, 50, 50), MathF.PI / 2f, new(.8f, .5f));
            AssertClose(new Vector2(50, 0), fromCenter.TopLeft);
            AssertClose(new Vector2(0, 50), fromCenter.BottomRight);

        }



        private void AssertClose(Vector2 a, Vector2 b)
        {
            Assert.AreEqual(a.X, b.X, .01);
            Assert.AreEqual(a.Y, b.Y, .01);
        }



    }
}