﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.MathF;

namespace CrystalCore.Util.Graphics
{
    public class RotatedRect
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
        public float Rotation { get; private set; }

        /// <summary>
        /// The size of the rectangle, width and height swapping when it is rotated sideways.
        /// </summary>
        public Vector2 AdjustedSize
        {
            get
            {
                
                if(DirectionUtil.FromRadians(Rotation).IsHorizontal())
                {
                    return new(Height, Width);
                }
                else
                {
                    return new(Width, Height);
                }
            }
        }


        public RectangleF AsRectangleF
        {
            get
            {
                float w = Width;
                float h = Height;
                return new RectangleF(X, Y, w, h);
            }
        }

        public Rectangle AsRectangle
        {
            get
            {
                return AsRectangleF.toRectangle();
            }
        }

        public Vector2 TopLeft
        {
            get
            {
                return PositionOfSizeRelativePoint(0, 0);
            }
        }

        public Vector2 TopCenter
        {

            get
            {
                return PositionOfSizeRelativePoint(.5f, 0f);
            }
        }

        public Vector2 TopRight
        {
            get
            {
                return PositionOfSizeRelativePoint(1f, 0f);
            }
        }



        public Vector2 CenterLeft
        {
            get
            {
                return PositionOfSizeRelativePoint(0, .5f);
            }
        }

        public Vector2 Center
        {
            get
            {
                return PositionOfSizeRelativePoint(.5f, .5f);
            }
        }

        public Vector2 CenterRight
        {
            get
            {
                return PositionOfSizeRelativePoint(1, .5f);
            }
        }



        public Vector2 BottomLeft
        {
            get
            {
                return PositionOfSizeRelativePoint(0, 1);
            }
        }

        public Vector2 BottomCenter
        {
            get
            {
                return PositionOfSizeRelativePoint(.5f, 1f);
            }
        }



        public Vector2 BottomRight
        {
            get
            {
                return PositionOfSizeRelativePoint(1f, 1f);
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


        private Vector2 PositionOfSizeRelativePoint(float x, float y)
        {
            float OwnX = x * Width;
            float OwnY = y * Height;

            // do a bit of trig
            float realX = Cos(-Rotation) * OwnX + Sin(-Rotation) * OwnY;
            float realY = Sin(Rotation) * OwnX + Cos(Rotation) * OwnY;

            return new Vector2(realX+X,realY+Y);

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
        /// <param name="location"> the physiscal location of the location origin of this rectangle.</param>
        /// <param name="size"> the width and height of this rectangle </param>
        /// <param name="locationOrigin">the point on the rectangle where location is defined, from 0,0 (top left) to 1,1 (bottom right)</param>
        /// <param name="rotation">the angle, in radians, that this rectangle is rotated around from rotation origin. </param>
        /// <param name="rotationOrigin">The point on the rectangle that is rotated about, from 0,0 (top left) to 1,1 (bottom right) </param>
        public RotatedRect( Vector2 location, Vector2 size, float rotation, Vector2 rotationOrigin)
        {

            Rotation = MathHelper.WrapAngle(rotation);

            Width = size.X;

            Height = size.Y;

            // we need to get the location, now.

            // step 1: convert location origin into real units in a rectangle oriented reference frame

            // what have I gotten myself into?

            rotationOrigin *= size;
            rotationOrigin += location;

         

            // step 2: rotate location origin about rotation origin
            location -= rotationOrigin;
            Vector2 oldLoc = location;
            location.X = oldLoc.X * MathF.Cos(Rotation) - oldLoc.Y * MathF.Sin(Rotation);
            location.Y = oldLoc.Y *MathF.Cos(Rotation) + oldLoc.X *MathF.Sin(Rotation);
            location += rotationOrigin;

            // step 3: translate origin to position
            
            X = location.X;
            Y = location.Y;
            
            
        }

        public RotatedRect(RectangleF rect, float rotation, Vector2 origin):this(rect.Location, rect.Size, rotation, origin)
        {
        }

        public RotatedRect(Rectangle rect, float rotation, Vector2 origin) : this(new RectangleF(rect), rotation, origin)
        {
        }

        public RotatedRect(RectangleF rect, Direction d): this(rect, d.ToRadians(), new(.5f))
        {

        }

        public RotatedRect(Rectangle rect, Direction d) : this(rect, d.ToRadians(), new(.5f))
        {

        }



        // valid rotation values are between 0 and pi/2. MAKE IT SO.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BoundingLocation"></param>
        /// <param name="size">Size of interior rectangle</param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static RotatedRect FromBoundingLocation(Vector2 BoundingLocation, Vector2 size, float rotation)
        {
            rotation = MathHelper.WrapAngle(rotation);

            float w = size.X;
            float h = size.Y;


            Vector2 loc = rotation switch
            {

                // How does this math work?
                // I made a desmos graph for it
                // did a bunch of geometry on a sheet of paper
                // just trust it
                // unless it doesn't work
                // then idk, good luck I guess

                // down
                < -PI / 2f => new(BoundingLocation.X + (w*Cos(rotation+PI)), 
                                        BoundingLocation.Y + w*Sin(rotation+PI) + h*Cos(rotation +PI)),

                // left
                < 0 => new(BoundingLocation.X, 
                           BoundingLocation.Y + w*Sin(-rotation)),

                // up
                < PI / 2f => new(BoundingLocation.X + h * Sin(rotation), 
                                       BoundingLocation.Y ),

                // right
                _ => new(BoundingLocation.X + w * Cos(PI-rotation) + h * Sin(PI-rotation), 
                         BoundingLocation.Y + h * Cos(PI-rotation)),
            };

            RotatedRect toReturn = new RotatedRect(loc, size, 0f, new(0));
            toReturn.Rotation = rotation;
            return toReturn;
        }

        public static RotatedRect FromBoundingLocation(Point BoundingLocation, Point size, float rotation)
        {
            return FromBoundingLocation(BoundingLocation.ToVector2(), size.ToVector2(), rotation);
        }


        /// <summary>
        /// Create a (gird aligned) rotated rectangle knowing the footprint it takes up.
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static RotatedRect FromFootprint(RectangleF loc, Direction d)
        {
            if(d.IsHorizontal())
            {
                loc = new(loc.X,loc.Y, loc.Height, loc.Width);
            }

            return FromBoundingLocation(loc.Location, loc.Size, d.ToRadians());
        }

        public static RotatedRect FromFootprint(Rectangle loc, Direction d)
        {
            return FromFootprint(new RectangleF(loc), d);
        }




    }
}
