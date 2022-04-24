using System;
using System.Collections.Generic;
using System.Text;
using Crystalarium.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Crystalarium.Sim
{
    class Viewport
    {
        /*
         * A viewport renders a grid.
         */

        private List<Viewport> container; // the list of all existing viewports.
        private Rectangle _pixelBounds; // the bounds, in pixels, of the viewport on the game window.
        private Grid _grid; // the grid that this viewport is rendering.

        private Texture2D sideTexture; // the side border texture of this viewport
        private Texture2D cornerTexture; // the corner border texture of this viewport
        private int _borderWidth; // the width, in pixels, of the border of the viewport

        private double _scale; // the number of pixels that currently represent one tile in gridspace
        private Vector2 _position; // the position of the top left corner of the viewport, in tiles, in grid space

        private int _minScale; // the minumum and maximum amount of pixels that can represent one tile.
        private int _maxScale;


        // #### TEST CODE: REMOVE WHEN POSSIBLE ###
        // this is just to showcase the idea behind rendering things, more or less.
        public Texture2D testTexture;
        private Rectangle locationInGameSpace= new Rectangle(0,0,1,1);

        public int BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = (value < 0) ? 0 : value; }
        }

        public int MinScale
        {
            get => _minScale;
            set {
                // set this value responsibly.
                if (value < 1)
                    value = 1;
                if (value > _maxScale)
                    value = _maxScale;

                _minScale = value;

                if (_scale < _minScale)
                    _scale = MinScale;
            }
        }

        public int MaxScale
        {
            get => _maxScale;
            set
            {
                // set this value responsibly
                if (value < _minScale)
                    value = MinScale;

                _maxScale = value;

                if (_scale > _maxScale)
                    _scale = _maxScale;
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                // insure the scale is not set outside of bounds.
                if (value > _maxScale)
                    value = _maxScale;
                if (value < _minScale)
                    value = _minScale;

                    
                _scale = value;
            }
        }

        // when setting position with the position property, position is the location, in tile space, of the center of the screen.
        public Vector2 Position
        {
            //get => _position; // too lazy to implement this properly. If we need it, I'll add it later.
            set
            {
                float x = (float)(-1f * ( (TileBounds().Size.X) / 2f))+ value.X+.5f;
                float y = (float)(-1f * ( (TileBounds().Size.Y)/2f)) + value.Y+.5f;
                _position = new Vector2(x, y);
                //System.Console.WriteLine(TileBounds() + "\n pos "+_position+"\n scale "+_scale+"\n pixel coords "+TiletoPixelCoords(locationInGameSpace.Location.ToVector2()));

            }
        }



        // create the viewport
        public Viewport(List<Viewport> viewports, Grid g, Point pos, Point dimensions)
        {
            // initialize from parameters
            _grid = g;
            container = viewports;
            container.Add(this);
            _pixelBounds = new Rectangle(pos, dimensions);

            // border related variables
            _borderWidth = 0;
            sideTexture = null;
            cornerTexture = null;

            // default scale values
            _minScale = 10;
            _maxScale = 50;

            // set the 'camera' to reasonable values
            _scale = (_minScale + _maxScale) / 2.0;
            Position = new Vector2(0, 0);
           

        }

        // an alternate viewport constructor, without points.
        public Viewport(List<Viewport> viewports, Grid g, int x, int y, int width, int height)
            : this(viewports, g, new Point(x, y), new Point(width, height)) { }


        public void destroy()
        {
            container.Remove(this);
        }

        public void setTextures(Texture2D sides, Texture2D corners)
        {
            sideTexture = sides;
            cornerTexture = corners;

        }

        public void draw(SpriteBatch sb)
        {
            // we drawin' the thing!
            if(!RenderTexture(sb, testTexture, locationInGameSpace))
            {
               // System.Console.WriteLine("it borked!");
            }
            
           

            // the last thing we do is draw the border.
            drawBorders(sb);
            drawCorners(sb);
        }


        private void drawBorders(SpriteBatch sb)
        {
            if(sideTexture== null || BorderWidth <1)
            {
                return;
            }

            // draw the four sides of the container.
            // this is ugly, but what was I gonna do about it?

            // top side.
            sb.Draw(
                cornerTexture,
                new Rectangle(_pixelBounds.X, _pixelBounds.Y, _pixelBounds.Width, BorderWidth),
                null,
                Color.White,
                0, // no rotation
                new Vector2(0,0),
                new SpriteEffects(),
                1f
            );

            // bottom side.
            sb.Draw(
                cornerTexture,
                new Rectangle(_pixelBounds.X,  _pixelBounds.Y+_pixelBounds.Height - BorderWidth, _pixelBounds.Width,BorderWidth),
                null,
                Color.White,
                0, // no rotation
                new Vector2(0, 0),
                new SpriteEffects(),
                1f
            );

            // left side.
            sb.Draw(
                cornerTexture,
                new Rectangle(_pixelBounds.X+BorderWidth, _pixelBounds.Y, _pixelBounds.Height, BorderWidth),
                null,
                Color.White,
                MathF.PI / 2,
                new Vector2(0, 0),
                new SpriteEffects(),
                1f
            );

            //right side.
            sb.Draw(
                cornerTexture,
                new Rectangle(_pixelBounds.X+_pixelBounds.Width, _pixelBounds.Y, _pixelBounds.Height, BorderWidth),
                null,
                Color.White,
                MathF.PI / 2,
                new Vector2(0, 0),
                new SpriteEffects(),
                1f
            );


            // this is hideous.

        }

        private void drawCorners(SpriteBatch sb)
        {

            // do not draw corners if we don't have corners to draw.
            if(cornerTexture==null || BorderWidth<1)
            {
                return;
            }

            // draw all 4 corners

            // TODO: rotate sprites here so that corner designs are facing the correct way.

            // this might be stupid, but I'm not repeating code..
            int x = _pixelBounds.X;
            for (int xCounter = 0; xCounter < 2; xCounter++, x += _pixelBounds.Width - BorderWidth)
            {
                int y = _pixelBounds.Y;
                for (int yCounter = 0; yCounter < 2; yCounter++, y += _pixelBounds.Height - BorderWidth)
                {
                    
                    sb.Draw(
                        cornerTexture,
                        new Rectangle(x, y, BorderWidth, BorderWidth),
                        Color.White
                    );

                }
            }

            

        }

        // returns the bounds in tilespace of the viewport
        private RectangleF TileBounds()
        {
            return new RectangleF(_position.X, _position.Y, (float)(_pixelBounds.Width / Scale), (float)(_pixelBounds.Height / Scale));
        }


        // returns  pixel coords relative to start of viewport.
        // this also works outside of the viewport.
        private Point TiletoPixelCoords(Vector2 tilePos)
        {
            /*if (!TileBounds().Contains(tilePos))
            {
                return new Point(-1);
            }*/
            // tile to pixel:
            // first, tile to pixel relative to base coords
            // how do we do that?
            // -(camera tile pos - tile pos) tile pos relative to base

            int x = (int)(Scale * -1.0 * (_position.X - (float)tilePos.X));
            int y = (int)(Scale * -1.0 * (_position.Y - (float)tilePos.Y));
            return new Point(x, y);

        }



        // bounds of object to render in tilespace
        private bool RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle bounds)
        {
            // check if the texture needs to be rendered by this viewport
            if(!TileBounds().Intersects(bounds))
            {
                return false;
            }


            //it does! collect some basic information.
            Point pixelCoords = TiletoPixelCoords(bounds.Location.ToVector2());
            Point pixelSize = new Point((int)(bounds.Size.X * _scale), (int)(bounds.Size.Y * _scale));

            // if true, the entire image is inside of this
            if (TileBounds().Contains(bounds))
            {
                
               
                sb.Draw(
                       texture,
                       new Rectangle(pixelCoords+_pixelBounds.Location,pixelSize),
                       Color.White
                   );

                return true;
            }


            // partial rendering...
            // render it!
            renderPartialTexture(sb, texture, new Rectangle(pixelCoords, pixelSize));
            return true;
        }

        


        // pixelBounds: the bounds of the texture to be rendered, in pixels, relative to the viewframe.
        // this is disgusting.
        private void renderPartialTexture(SpriteBatch sb, Texture2D texture, Rectangle pixelBounds)
        {
            // oh jeez
            // oh no
            // oh god

            // figure out the rectangle we need to draw.

            // some flags (this is getting messy)
            // whether this side had bits cut off from it.
            int topCut = 0;
            int bottomCut = 0;
            int rightCut = 0;
            int leftCut = 0;

            // get the top left point of the drawing area
            Point topLeft = pixelBounds.Location;
            Point size = pixelBounds.Size;

            
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

            
            topLeft = topLeft + this._pixelBounds.Location;

           
            // figure out the size of the rectangle we need to draw.
            int rightSide = this._pixelBounds.X + this._pixelBounds.Width;

            if (size.X + topLeft.X > rightSide)
            {
                size.X = rightSide - topLeft.X;
                rightCut = pixelBounds.Size.X - size.X;
            }

            int bottomSide = this._pixelBounds.Y + this._pixelBounds.Height;

            if (size.Y + topLeft.Y > bottomSide)
            {
                size.Y = bottomSide - topLeft.Y;
                bottomCut = pixelBounds.Size.Y - size.Y;
            }

            // now figure out the source rectangle. what part of the image do we need to draw?
            int sourceX = (int)((float)leftCut / (float)pixelBounds.Width * texture.Width);
            int sourceY = (int)((float)topCut / (float)pixelBounds.Height * texture.Height);

            // yep, this is ugly.
            int sourceWidth = (int)((float)(pixelBounds.Width - leftCut - rightCut) / (float)pixelBounds.Width * texture.Width);
            int sourceHeight = (int)((float)(pixelBounds.Height - topCut - bottomCut) / (float)pixelBounds.Height * texture.Height);

            // okay, hopefully that works...
            Rectangle sourceRect = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);

            
            sb.Draw(
                       texture,
                       new Rectangle(topLeft, size),
                       sourceRect,
                       Color.White
                      

                   );

        }
      



    }
}
