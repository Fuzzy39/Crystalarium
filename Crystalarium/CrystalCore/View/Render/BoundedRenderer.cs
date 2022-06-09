using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Render
{
    internal class BoundedRenderer
    {
        /*
         *  A Bounded renderer renders things, within a boundry.
         *  
         * 
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
        public virtual bool RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle pixelBounds, Color c, Direction d)
        {

            // pray that you never have to edit this again.
            // if you do, please refactor this mess.

            float rotation = d.ToRadians();

            // check if the texture needs to be rendered by this viewport
            Rectangle absoluteBounds = new Rectangle(pixelBounds.Location + PixelBoundry.Location, pixelBounds.Size);
            if (!absoluteBounds.Intersects(this.PixelBoundry))
            {
                return false;
            }


            //it does! collect some basic information.
            // we add a couple pixels to the size of things

         

            // partial renderings
            // render it!
            Rectangle texturePixelBounds = pixelBounds;

            // figure out the rectangle we need to draw.

            // some flags (this is getting messy)
            // whether this side had bits cut off from it.
            int topCut = 0;
            int bottomCut = 0;
            int rightCut = 0;
            int leftCut = 0;

            // get the top left point of the drawing area
            Point topLeft = texturePixelBounds.Location;
            Point size = texturePixelBounds.Size;


            if (topLeft.X < 0) 
            {
                // adjust the size to match what is visible
                size.X += topLeft.X;

                // keep track of what was removed
                leftCut = -topLeft.X;

                // set position to what was removed.
                topLeft.X = 0;
            }

            if (topLeft.Y < 0)
            {
                // adjust the size to match what is visible
                size.Y += topLeft.Y;

                // keep track of what was removed
                topCut = -topLeft.Y;

                // set position to inside of the viewport
                topLeft.Y = 0;

            }

            topLeft = topLeft + _pixelBoundry.Location;

            topCut = (int)MathF.Abs(MathF.Cos(rotation) * topCut + MathF.Sin(rotation) * leftCut);
        
            leftCut = (int)MathF.Abs(MathF.Cos(rotation) * leftCut + MathF.Sin(rotation) * topCut);
            // figure out the size of the rectangle we need to draw.
            int rightSide = _pixelBoundry.X + _pixelBoundry.Width;
            size.X = GetRenderSize(rightSide, texturePixelBounds.Size.X, leftCut, topLeft.X, out rightCut);

            int bottomSide = _pixelBoundry.Y + _pixelBoundry.Height;
            size.Y = GetRenderSize(bottomSide, texturePixelBounds.Size.Y, topCut, topLeft.Y, out bottomCut);

            bottomCut = (int)MathF.Abs(MathF.Cos(rotation) * bottomCut + MathF.Sin(rotation) * bottomCut);

            rightCut = (int)MathF.Abs(MathF.Cos(rotation) * rightCut + MathF.Sin(rotation) * rightCut);


            Rectangle sourceRect = GetTextureSourceBounds(topCut, bottomCut, leftCut, rightCut, texturePixelBounds, texture);

       
            Rectangle destRect = new Rectangle(topLeft, size);
        

           Vector2 rotationOrigin = new Vector2(sourceRect.Width / 2f, sourceRect.Height / 2f);
         
            destRect.Size = AbsRotate(destRect.Size, rotation);

            Rectangle acutalSourceRect = new Rectangle(AbsRotate(sourceRect.Location, rotation), AbsRotate(sourceRect.Size, rotation));

         

            sb.Draw(
                       texture,
                       new Rectangle(
                           destRect.Location + new Point(destRect.Width/2, destRect.Height/2), destRect.Size),
                       acutalSourceRect,
                       c, // the color of the texture
                       rotation,
                       rotationOrigin,
                       /*sourceRect.Center.ToVector2(),*/ // the center of rotation
                       SpriteEffects.None,
                       0f

                   );

          
            return true;
        }

        // rot is in radians
        private Point rotate(Point p, float rot)
        {
            return new Point(
              (int)(MathF.Cos(rot) * p.X + MathF.Sin(rot) * p.Y),
              (int)(MathF.Cos(rot) * p.Y + MathF.Sin(rot) * p.X));
        }


        private Point AbsRotate(Point p, float rot)
        {
            Point toReturn = rotate(p, rot);
            return new Point(Math.Abs(toReturn.X), Math.Abs(toReturn.Y));
        }

        private int GetRenderSize(int viewportFarPos, int size, int nearCut, int position, out int farCut)
        {
            int currentSize = size - nearCut;

            if (!(currentSize + position > viewportFarPos))
            {
                farCut = 0;
                return currentSize;
            }

            currentSize = viewportFarPos - position;
            farCut = size - currentSize;


            return currentSize;
        }

        private Rectangle GetTextureSourceBounds(int topCut, int bottomCut, int leftCut, int rightCut, Rectangle texturePixelBounds, Texture2D texture)
        {
            // now figure out the source rectangle. what part of the image do we need to draw?

            // get the ratio of the destinations's position, multiply it by the source.
            int sourceX = (int)((float)leftCut / (float)texturePixelBounds.Width * texture.Width);
            int sourceY = (int)((float)topCut / (float)texturePixelBounds.Height * texture.Height);


            // figure out the size of the source rectangle:

            // get the width, in pixels, of the destination.
            float textureWidth = (float)(texturePixelBounds.Width - ((leftCut > rightCut) ? leftCut : rightCut));

            // get the width of the source rectangle, as a ratio of total width of the texuture
            float textureWidthRatio = textureWidth / (float)texturePixelBounds.Width;

            // get the width of the source rectangle in pixels
            int sourceWidth = (int)(textureWidthRatio * texture.Width);


            // get the height, in pixels, of the destination.
            float textureHeight = (float)(texturePixelBounds.Height - ((bottomCut < topCut) ? topCut : bottomCut));

            // get the height of the source rectangle, as a ratio of total width of the texuture
            float textureHeightRatio = textureHeight / (float)texturePixelBounds.Height;

            // get the height of the source rectangle in pixels
            int sourceHeight = (int)(textureHeightRatio * texture.Height);


            return new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);

        }



    }

}
