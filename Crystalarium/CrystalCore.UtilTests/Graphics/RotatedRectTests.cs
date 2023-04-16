using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalCore.Util.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
            RotatedRect simple = new RotatedRect(new(1),new(2), 0f, new(0));

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

            RotatedRect rotated = new RotatedRect(new(0), new(2), MathF.PI/2f, new(0));
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

            RotatedRect rotated = new RotatedRect(new(0), new(1,2), MathF.PI / 2f, new(0));
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
        public void FootprintTest()
        {
            RotatedRect r = RotatedRect.FromFootprint(new Rectangle(0, 0, 2, 1), Direction.left);

            AssertClose(new Vector2(0, 1), r.TopLeft);
            AssertClose(new Vector2(0, 0), r.TopRight);

            AssertClose(new Vector2(2, 1), r.BottomLeft);
            AssertClose(new Vector2(2, 0), r.BottomRight);
        }



        private void AssertClose( Vector2 a, Vector2 b)
        {
            Assert.AreEqual(a.X, b.X, .01);
            Assert.AreEqual(a.Y, b.Y, .01);
        }


      
    }
}