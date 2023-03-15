using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Util.Graphics
{
    internal class RotatedRect
    {

        // A rotated rectangle is a rectangleF that can be rotated, I guess?



        public float X { get; private set; }
        public float Y { get; private set; }

        public float Width { get; set; }
        public float Height { get; set; }


        /// <summary>
        ///  The clockwise Rotation in radians, about the top left corner
        ///  Should be between -pi and pi.
        /// </summary>
        private float Rotation { get; set; }

        private Vector2 DistFromLoc(float dist, float rot)
        {
            return new(X + MathF.Cos(rot) * dist, Y + MathF.Sin(rot) * dist);
        }

        public Vector2 TopLeft
        {
            get
            {
                return new(X, Y);
            }
        }

        public Vector2 TopRight
        {
            get
            {
                return DistFromLoc(Width, Rotation);
            }
        }

        public Vector2 TopCenter
        {

            get
            {
                return DistFromLoc(Width / 2f, Rotation);
            }
        }


        public Vector2 BottomLeft
        {
            get
            {
                return DistFromLoc(Height, Rotation - (MathF.PI / 2f));
            }
        }

        public Vector2 CenterLeft
        {
            get
            {
                return DistFromLoc(Height, Rotation - (MathF.PI / 2f));
            }
        }

        public Vector2 BottomRight
        {
            get
            {
                float dist = MathF.Sqrt(Height*Height+Width*Width);

                float rot = Rotation-MathF.Atan(Height / Width);

                return DistFromLoc(dist, rot);
            }
        }



        public Vector2 BottomCenter
        {
            get
            {

                float Xdisplacement = Width / 2f;

                float dist = MathF.Sqrt(Height * Height + Xdisplacement * Xdisplacement);

                float rot = Rotation - MathF.Atan(Height / Xdisplacement);

                return DistFromLoc(dist, rot);
            }
        }

        public Vector2 Center
        {
            get
            {
                float dist = MathF.Sqrt(Height * Height / 4f + Width * Width / 4f);

                float rot = Rotation - MathF.Atan(Height / Width);

                return DistFromLoc(dist, rot);
            }
        }

        public RectangleF BoundingBox
        {
            get
            {
                float X1 = MiscUtil.PickLeast(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);
                float X2 = MiscUtil.PickGreatest(TopLeft.X, TopRight.X, BottomLeft.X, BottomRight.X);

                float Y1 = MiscUtil.PickLeast(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);
                float Y2 = MiscUtil.PickGreatest(TopLeft.Y, TopRight.Y, BottomLeft.Y, BottomRight.Y);

                return new RectangleF(X1,Y1, X2- X1, Y2 - Y1);

            }
        }



        public void SetBoundingBoxLocation(Vector2 newLoc)
        {
            Vector2 oldLoc = BoundingBox.Location;

            Vector2 diff = newLoc-oldLoc;

            X += diff.X;
            Y += diff.Y;

        }

        

        // And finally, the constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"> the location of the location origin of this rectangle.</param>
        /// <param name="size"> the width and height of this rectangle </param>
        /// <param name="locationOrigin">the point on the rectangle where location is defined, from 0,0 (top left) to 1,1 (bottom right)</param>
        /// <param name="rotation">the angle, in radians, that this rectangle is rotated around from rotation origin. </param>
        /// <param name="rotationOrigin">The point on the rectangle that is rotated about, from 0,0 (top left) to 1,1 (bottom right) </param>
        public RotatedRect( Vector2 location, Vector2 size, Vector2 locationOrigin, float rotation, Vector2 rotationOrigin)
        {

            Rotation = MathHelper.WrapAngle(rotation);

            Width = size.X;

            Height = size.Y;

            // we need to get the location, now.

            // step 1: convert location origin into real units relative to the rotation origin in a rectangle oriented reference frame

            // what have I gotten myself into?

            Vector2 locAway = rotationOrigin - locationOrigin;
            locAway *= size;


            // convert origins to real units
            // find the distance between locorg and rotorg
            //use the law of sines to find the distance 

            // we know the poi
            

        }


    }
}
