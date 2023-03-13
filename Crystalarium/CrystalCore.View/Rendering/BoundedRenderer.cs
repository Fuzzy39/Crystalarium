using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.View.Rendering
{
    /// <summary>
    /// A Bounded renderer renders things within a boundry.
    /// Images which are partly outside of this boundry are cropped to fit. 
    /// </summary>
    internal class BoundedRenderer
    {
        /// <summary>
        ///   The pixel coordinates of the window that this BoundedRenderer will render within.
        /// </summary>
        private Rectangle _pixelBoundry;
        
        /// <summary>
        ///   The pixel coordinates of the window that this BoundedRenderer will render within.
        /// </summary>
        public Rectangle PixelBoundry
        {
            get => _pixelBoundry;
        }

        /// <summary>
        ///  Create a new BoundedRender with the specified pixel bounds.
        /// </summary>
        /// <param name="pixelBoundry"> The pixel coordinates of the window that this BoundedRenderer will render within. </param>
        public BoundedRenderer(Rectangle pixelBoundry)
        {
            _pixelBoundry = pixelBoundry;
        }

    
        /// <summary>
        ///  Render a texture within the bounds of this renderer.
        ///  Any images rendered partially outside of this renderer's bounds will be cropped to fit.
        /// </summary>
        /// <param name="sb">The SpriteBatch object used to render the image.</param>
        /// <param name="texture">The texture we are to render.</param>
        /// <param name="pixelBounds">The area in pixels that the image is to be rendered, relative to the location of this renderer's bounds.</param>
        /// <param name="c">The color the texture will be rendered with.</param>
        /// <param name="d">The direction this texture will be facing, with up being the default orientation.</param>
        public void RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle pixelBounds, Color c, Direction d)
        {

            // if the image is outside of our bounds, don't even bother.
            if (!ToAbsCoords(pixelBounds).Intersects(_pixelBoundry))
            {
                return;
            }

            // crop the pixel bounds if needbe. (Where do we expect this image to end up at?)
            Rectangle finalDestBounds = GetFinalDestBounds(ToAbsCoords(pixelBounds));

            // figure out the source bounds. ( What part of the texture are we using?)
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

        /// <summary>
        ///  Returns the Rectangle of pixels relative to the window that a rendered texture would have.
        /// </summary>
        /// <param name="pixelBounds">A Rectangle of pixels, in coordinates relative to the window.</param>
        /// <returns>The Rectangle of pixels, relative to the window, that fits inside our bounds</returns>
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

        /// <summary>
        ///  Takes the final location of the texture on the screen and adjusts it to the correct value to input into spritebatch.draw, based on which way this image will face.
        /// </summary>
        /// <param name="destBounds">The rectangle on the window in pixels where this texture will appear</param>
        /// <param name="facing">the direction this image</param>
        /// <returns>A rectangle that will correspond to these bounds when drawn with this facing.</returns>
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

        /// <summary>
        ///  Transforms a rectangle of pixel coords relative to our location into one relative to the window's.
        /// </summary>
        /// <param name="rect">A rectangle with coords relative to our location.</param>
        /// <returns>A rectangle with coords relative to the top left of the game window.</returns>
        private Rectangle ToAbsCoords(Rectangle rect)
        {

            rect.Location = ToAbsCoords(rect.Location);
            return rect;
        }

        /// <summary>
        ///  Transforms a point in pixel coords relative to our location into one relative to the window's.
        /// </summary>
        /// <param name="p">A point with coords relative to our location.</param>
        /// <returns>A point with coords relative to the top left corner of the game window.</returns>
        private Point ToAbsCoords(Point p)
        {
            // translates pixel coords relative to our borders to coords relative to the window. 'absolute' coords.
            return p + _pixelBoundry.Location;
        }

        // get the bounds of the source rectangle.
        /// <summary>
        ///  Gets the part of the texture that should be rendered, according to what parts of the destination are being cropped off.
        /// </summary>
        /// <param name="texture">The texture to be rendered.</param>
        /// <param name="original">The intended bounds of the location to render this texture, relative to the game window.</param>
        /// <param name="modified">The actual (cropped) bounds of the location to render this texture, relative to the game window.</param>
        /// <param name="facing">The direction this texture will be rendered facing, with up being default.</param>
        /// <returns>The bounds in pixels of the texture that will be rendered.</returns>
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

        /// <summary>
        ///  Returns the Cuts of a rectangle based on it's intial state and modified state.
        /// </summary>
        /// <param name="original">The rectangle before it was cut.</param>
        /// <param name="modified">The rectangle after it was cut.</param>
        /// <returns>The Cuts that occured to orginal to get modified.</returns>
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

        /// <summary>
        /// Take a series of cuts of one rectangle and apply the ratio of those cuts to another.
        /// </summary>
        /// <param name="toTransform">an array of cuts of one rectangle.</param>
        /// <param name="applyTo">The rectangle to scale these cuts to.</param>
        /// <returns>The Array of cuts that is the same as toTransform, but proportional to applyTo.</returns>
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


        /// <summary>
        /// Rotates every cut in an array of cuts.
        /// </summary>
        /// <param name="cuts">an array of cuts.</param>
        /// <param name="facing">The way cuts should be rotated, with the default being up.</param>
        /// <returns>an array of cuts, rotated.</returns>
        private Cut[] RotateCuts(Cut[] cuts, Direction facing)
        {
            
            for (int i = 0; i < cuts.Length; i++)
            {
               
                for (Direction j = facing; j != Direction.up; j=j.Rotate(RotationalDirection.ccw))
                {
                    // I think counterclockwise is right, but I could be wrong.
                    cuts[i].dir = cuts[i].dir.Rotate(RotationalDirection.ccw);
                }
            }

            return cuts;
        }
    }

}
