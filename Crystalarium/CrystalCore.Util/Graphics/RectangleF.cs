using Microsoft.Xna.Framework;

namespace CrystalCore.Util.Graphics
{
    public struct RectangleF
    {
        // defines a rectangle, but made with floats.
        // this, shockingly, does not exist in monogame.
        // It made making viewport a little annoying, as you could imagnine.
        // nothing was where it was supposed to be...

        public RectangleF(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public RectangleF(Vector2 location, Vector2 size) : this(location.X, location.Y, size.X, size.Y)
        {

        }

        public RectangleF(Rectangle r) : this(r.X, r.Y, r.Width, r.Height)
        {

        }

        public Rectangle toRectangle()
        {
            return new Rectangle(
                (int)MathF.Floor(X),
                (int)MathF.Floor(Y),
                (int)MathF.Ceiling(Width),
                (int)MathF.Ceiling(Height));
        }

        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }


        // the top left corner of the rectangle
        public Vector2 Location
        {
            get => new Vector2(X, Y);
        }

        // the bottom right corner of the rectangle
        public Vector2 BottomRight
        {
            get { return new Vector2(X + Width, Y + Height); }
        }


        public Vector2 TopRight
        {
            get { return new Vector2(X + Width, Y); }
        }

        public Vector2 BottomLeft
        {
            get { return new Vector2(X, Y + Height); }
        }

        public Vector2 TopLeft
        {
            get { return new Vector2(X, Y); }
        }


        public Vector2 Center
        {
            get { return new(X + Width / 2, Y + Height / 2); }
        }

        public Vector2 RelativeCenter
        {
            get { return Center - TopLeft; }
        }

        // the size of the rectangle
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
        }

        public float Area
        {
            get { return Width * Height; }
        }

        public float Top
        {
            get => Y;
        }

        public float Left
        {
            get => X;
        }

        public float Right
        {
            get => X + Width;
        }

        public float Bottom
        {
            get => Y + Height;
        }


        // returns whether the specified rectangle is entirely within this rectangle.
        public bool Contains(RectangleF rect)
        {
            return Contains(rect.TopLeft)
                & Contains(rect.BottomRight);

        }


        public bool Contains(Rectangle rect)
        {
            return Contains(new RectangleF(rect));
        }


        // returns whether the point is inside of this rectangle.
        public bool Contains(Vector2 point)
        {
            if (X <= point.X & Y <= point.Y)
            {

                if (BottomRight.X >= point.X & BottomRight.Y >= point.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(Point point)
        {
            return Contains(point.ToVector2());
        }



        // returns whether any part of this rectangle is within the specified rectangle.
        public bool Intersects(RectangleF rect)
        {

            // this hurts my brain, despite not being very complicated.
            return Right >= rect.Left
                 & Left <= rect.Right
                 & Top <= rect.Bottom
                 & Bottom >= rect.Top;

        }


        public bool Intersects(Rectangle rect)
        {
            return Intersects(new RectangleF(rect));
        }

        public RectangleF Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;

            Width += 2 * horizontalAmount;
            Height += 2 * verticalAmount;

            return this;
        }

        public override string ToString() => "{ Location: { " + X + ", " + Y + " }, Size: { " + Width + ", " + Height + " } }";
    }
}
