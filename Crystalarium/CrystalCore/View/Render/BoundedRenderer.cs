using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.View.Render
{
    internal class BoundedRenderer
    {
        /*
         *  A Bounded renderer renders things, within a boundry.
         *  
         *  No relation to any other renderers, which are broadly higher level constructs.
         */


        private Rectangle _pixelBoundry;

        public Rectangle PixelBoundry
        {
            get => _pixelBoundry;
        }

        public BoundedRenderer(Rectangle pixelBoundry)
        {
            _pixelBoundry = pixelBoundry;
        }



        // bounds in pixels relative to the renderer's location.
        public void RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle pixelBounds, Color c, Direction d)
        {

            // if the image is outside of our bounds, don't even bother.
            if (!ToAbsCoords(pixelBounds).Intersects(_pixelBoundry))
            {
                return;
            }

            // crop the pixel bounds if needbe.
            Rectangle finalDestBounds = GetFinalDestBounds(ToAbsCoords(pixelBounds));

            // figure out the source bounds.
            Rectangle sourceBounds = GetSourceBounds(texture, ToAbsCoords(pixelBounds), finalDestBounds, d);

            // get the correct dest bounds, compensating for rotation
            Rectangle actualDestBounds = AdjustDestBounds(finalDestBounds, d);

            // draw the actual thing
            sb.Draw(
                    texture,
                    actualDestBounds,
                    sourceBounds,
                    c,
                    d.ToRadians(),
                    new Vector2(0),
                    SpriteEffects.None,
                    0f
            );

        }

        // here, pixelBounds is absolute.
        private Rectangle GetFinalDestBounds(Rectangle pixelBounds)
        {
            // if these bounds are fully within our borders, we don't need to do anything to it.
            if (this.PixelBoundry.Contains(pixelBounds))
            {
                return pixelBounds;
            }

            // if not, get what's left.
            return Rectangle.Intersect(this.PixelBoundry, pixelBounds);
        }

        private Rectangle AdjustDestBounds(Rectangle destBounds, Direction facing)
        {
            Point loc = destBounds.Location;
            Point size = destBounds.Size;

            switch (facing)
            {

                case Direction.left:
                    size = new Point(size.Y, size.X);
                    loc.Y += size.X;
                    break;
                case Direction.right:
                    size = new Point(size.Y, size.X);
                    loc.X += size.Y;
                    break;
                case Direction.down:
                    loc = loc + size;
                    break;
            }

            return new Rectangle(loc, size);
        }

        private Rectangle ToAbsCoords(Rectangle rect)
        {

            rect.Location = ToAbsCoords(rect.Location);
            return rect;
        }

        private Point ToAbsCoords(Point p)
        {
            // translates pixel coords relative to our borders to coords relative to the window. 'absolute' coords.
            return p + _pixelBoundry.Location;
        }

        // get the bounds of the source rectangle.
        private Rectangle GetSourceBounds(Texture2D texture, Rectangle original, Rectangle modified, Direction facing)
        {
            Cut[] destCuts = GetDestCuts(original, modified);

            // take the dest cuts and get source cuts.
            Cut[] cuts = TransferCuts(destCuts, texture.Bounds);

            // rotate the cuts based on which way this sprite should be facing.
            cuts = RotateCuts(cuts, facing);

            // finally, pop out our source bounds.
            Rectangle toReturn = texture.Bounds;
            
            foreach (Cut c in cuts)
            {
              
                toReturn = Rectangle.Intersect(toReturn, c.slice);
            }
           
            return toReturn;
        }

        private Cut[] GetDestCuts(Rectangle original, Rectangle modified)
        {
            // get the cuts used on the final dest rectangle.
            Cut[] toReturn = new Cut[4];

            toReturn[0] = new Cut(original, Direction.up, modified.Y - original.Y);
            toReturn[1] = new Cut(original, Direction.left, modified.X - original.X);
            toReturn[2] = new Cut(original, Direction.down,  original.Bottom - modified.Bottom );
            toReturn[3] = new Cut(original, Direction.right, original.Right - modified.Right );

            // sanity check.
            foreach (Cut c in toReturn)
            {
                Debug.Assert(c.ratio >= 0);
            }

            return toReturn;
        }

        // transfer cuts to another rectangle.
        private Cut[] TransferCuts(Cut[] toTransform, Rectangle applyTo)
        {
            Cut[] toReturn = new Cut[toTransform.Length];

            for (int i = 0; i < toTransform.Length; i++)
            {
                float ratio = toTransform[i].ratio;
                toReturn[i] = new Cut(applyTo, toTransform[i].dir, ratio);
            }

            return toReturn;
        }


        private Cut[] RotateCuts(Cut[] cuts, Direction facing)
        {
            
            for (int i = 0; i < cuts.Length; i++)
            {
               
                for (Direction j = facing; j != Direction.up; j=j.Rotate(RotationalDirection.counterclockwise))
                {
                    // I think counterclockwise is right, but I could be wrong.
                    cuts[i].dir = cuts[i].dir.Rotate(RotationalDirection.counterclockwise);
                }
            }

            return cuts;
        }
    }

}
