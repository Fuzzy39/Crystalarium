using System;
using System.Collections.Generic;
using Crystalarium.Sim;
using Crystalarium.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Crystalarium.Render.ChunkRender;


namespace Crystalarium.Render
{
    public class Camera
    {
        /*
         * A viewport renders a grid.
         * 
         */


        private GridView parent;

        // 'Camera' controls
        private double _scale; // the number of pixels that currently represent one tile in gridspace
        private Vector2 _position; // the position of the top left corner of the viewport, in tiles, in grid space
        private Vector3 _velocity; // the velocity of the camera in x, y, and z dimensions. (in pixels/frame)
        private const float FRICTION = .3f; // the rate which camera velocity is reduced, in pixels/frame.
        private const float MAX_SPEED = 10f; // the maximum velocity (in x and y dimensions) of the camera in pixels/frame.

        private int _minScale; // the minumum and maximum amount of pixels that can represent one tile.
        private int _maxScale;

        private Point _zoomOrigin; // the point, in pixels relative to the top left corner of our gridview,
                                   // that serves as the origin for dilation translations/zooming.

        private Rectangle _bounds; // the tilespace to where the center coordinate of the camera is confined.
        private bool _isBound; // whether the camera is bound, or whether it is free.



        // Properties

        // scale, but linear, and scaled from 0 to 100, where 100 is maxScale, and 0 is minScale.
        public float Zoom
        {
            get 
            {

                // scale where 0 is min and 1 is max. .5 is average of min and max.
                float linearScale = ((float)_scale - _minScale) / ((float)_maxScale - _minScale);

                return (MathF.Pow(2, linearScale)-1)* 100f;
            }
            set
            {
                if (value < 0) { value = 0; VelZ = 0; }
                if (value > 100) { value = 100; VelZ = 0; }

                Scale = (MathF.Log((value/ 100f)+1, 2) * (_maxScale -_minScale)) + _minScale;

            }
        }   


        public bool IsBound
        {
            get => _isBound;
            set => _isBound = value;
        }

        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public Vector3 Velocity
        {
            get => _velocity;

            set
            {
                VelX = value.X;
                VelY = value.Y;
                VelZ = value.Z;
            }
        

        }

        public float VelX
        {
            get => _velocity.X;
            set 
            {
                _velocity.X = value;
                // limit velocity.
                if (_velocity.X > MAX_SPEED) _velocity.X = MAX_SPEED;
                if (_velocity.X < -MAX_SPEED) _velocity.X = -MAX_SPEED;
            }
        }

        public float VelY
        {
            get => _velocity.Y;
            set
            {
                _velocity.Y = value;

                // limit velocity.
                if (_velocity.Y > MAX_SPEED) _velocity.Y = MAX_SPEED;
                if (_velocity.Y < -MAX_SPEED) _velocity.Y = -MAX_SPEED;
            }
        }

        public float VelZ
        {
            get => _velocity.Z;
            // we won't limit z, for now.
            set => _velocity.Z = value;
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

        private double Scale
        {
            get => _scale;
            set
            {
                // insure the scale is not set outside of bounds.
                if (value > _maxScale)
                {
                    value = _maxScale;
              
                }
                if (value < _minScale)
                {
                    value = _minScale;
              
                }


                _scale = value;
            }
        }

        // when setting position with the position property, position is the location, in tile space, of the center of the viewport.
        // this causes... headaches.
        public Vector2 Position
        {
            get
            {
                Vector2 toReturn = new Vector2();
                toReturn.X = _position.X + (TileBounds().Size.X / 2.0f);
                toReturn.Y = _position.Y + (TileBounds().Size.Y / 2.0f);
                return toReturn;
            }// too lazy to implement this properly. If we need it, I'll add it later.
            set
            {
                float x = (float)(-1f * ((TileBounds().Size.X) / 2f)) + value.X;
                float y = (float)(-1f * ((TileBounds().Size.Y) / 2f)) + value.Y;
                setPosition(new Vector2(x, y));
                _velocity = new Vector3(0);

            }
        }


        public Point ZoomOrigin
        {
            get => _zoomOrigin;
            set => _zoomOrigin = value; 
        
        }


        // Constructors

        public Camera(GridView parent)
        {
            this.parent = parent;

            // default scale values
            _minScale = 10;
            _maxScale = 50;

            // set the 'camera' to reasonable values
            _scale = (_minScale + _maxScale) / 2.0;
            _position = new Vector2(0, 0);


            _zoomOrigin = new Point(0);
            _bounds = new Rectangle(0, 0, 1, 1);
            _isBound = false;
        }



        // Drawing code:



        // returns the bounds in tilespace of the viewport
        public RectangleF TileBounds()
        {
            return new RectangleF(_position.X, _position.Y,
                (float)(parent.PixelBounds.Width / Scale),
                (float)(parent.PixelBounds.Height / Scale));
        }

        // returns  pixel coords relative to start of viewport.
        // this also works outside of the viewport.
        private Point TileToPixelCoords(Vector2 tilePos)
        {

            // tile to pixel:
            // first, tile to pixel relative to base coords
            // how do we do that?
            // -(camera tile pos - tile pos) tile pos relative to base

            int x = (int)(Scale * -1.0 * (_position.X - (float)tilePos.X));
            int y = (int)(Scale * -1.0 * (_position.Y - (float)tilePos.Y));
            return new Point(x, y);

        }

        // assumes pixelPos is localized.
        private Vector2 PixelToTileCoords(Point pixelPos)
        {
            float x = (float)(_position.X + (pixelPos.X / _scale));
            float y = (float)(_position.Y + (pixelPos.Y / _scale));
            return new Vector2(x, y);
        }


        public bool RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle bounds)
        {
            return RenderTexture(sb, texture, new RectangleF(bounds), Color.White);
        }

        public bool RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle bounds, Color c)
        {
            return RenderTexture(sb, texture, new RectangleF(bounds), c);
        }

        public bool RenderTexture(SpriteBatch sb, Texture2D texture, RectangleF bounds)
        {
            return RenderTexture(sb, texture, bounds, Color.White);
        }

        // bounds of object to render in tilespace
        public bool RenderTexture(SpriteBatch sb, Texture2D texture, RectangleF bounds, Color c)
        {
            // check if the texture needs to be rendered by this viewport
            if (!TileBounds().Intersects(bounds))
            {
                return false;
            }


            //it does! collect some basic information.
            // we add a couple pixels to the size of things

            Point pixelCoords = TileToPixelCoords(bounds.Location)-new Point(1);
            Point pixelSize = new Point((int)(bounds.Size.X * _scale), (int)(bounds.Size.Y * _scale))+new Point(1,1);

            // partial renderings
            // render it!
            Rectangle texturePixelBounds = new Rectangle(pixelCoords, pixelSize);

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

            topLeft = topLeft + parent.PixelBounds.Location;

            // figure out the size of the rectangle we need to draw.
            int rightSide = parent.PixelBounds.X + this.parent.PixelBounds.Width;
            size.X = GetRenderSize(rightSide, texturePixelBounds.Size.X, leftCut, topLeft.X, out rightCut);

            int bottomSide = parent.PixelBounds.Y + parent.PixelBounds.Height;
            size.Y = GetRenderSize(bottomSide, texturePixelBounds.Size.Y, topCut, topLeft.Y, out bottomCut);


            Rectangle sourceRect = GetTextureSourceBounds(topCut, bottomCut, leftCut, rightCut, texturePixelBounds, texture);


            sb.Draw(
                       texture,
                       new Rectangle(topLeft, size),
                       sourceRect,
                       c // the color of the texture


                   );
            return true;
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

        // sets the position of the camera if that position would be in bounds.
        // pos represents the location of the top left corner of the screen.
     
        private bool setPosition(Vector2 pos)
        {
            float centerX = (float)(((TileBounds().Size.X) / 2f)) + pos.X;
            float centerY = (float)(((TileBounds().Size.Y) / 2f)) + pos.Y;
            Vector2 nextCenter = new Vector2(centerX, centerY);

            if ((!_isBound) || new RectangleF(_bounds).Contains(nextCenter))
            {
               
                _position = pos;
                return true;
            }
           



            return false;


        }

     
        public void Update( Rectangle bounds)
        {

            _bounds = bounds;
              
         
            Vector2 nextPos = _position + new Vector2(Velocity.X / (float)Scale, Velocity.Y / (float)Scale);
            

            // prevent camera movement outside the bounds.
            if (!setPosition(new Vector2(nextPos.X, _position.Y)))
            {
                // add a bit of bounce to the camera.
                VelX = (float)(-VelX/2);
             

            }

            if (!setPosition(new Vector2(_position.X, nextPos.Y)))
            {
                // add a bit of bounce to the camera.
                VelY = (float)(-VelY / 2);


            }

           

            UpdateZoom(Zoom + Velocity.Z);

          


            // check that the camera is in bounds. If bounded, the camera should NEVER be out of bounds.
            if (_isBound & !new RectangleF(_bounds).Contains(Position))
            {
                throw new InvalidOperationException(Position + " is out of bounds " + _bounds + " for this Camera.");
            }


            _velocity.X = Reduce(Velocity.X, FRICTION);
            _velocity.Y = Reduce(Velocity.Y, FRICTION);
            _velocity.Z = Reduce(Velocity.Z, FRICTION);

            



        }



        public void UpdateZoom(float newScale)
        {
            // functionally a dilation.

            // we need to change the scale to newscale while ensuring that the center remains the same.
            // first, get the tilespace location of the center currently.

            // this must remain in the same location as the origin before and after the transformation
            Vector2 originLocation = PixelToTileCoords(_zoomOrigin);


            // if all went well, actually change the zoom.
            Zoom = newScale;

            // ensure we don't bust anything.
            if (_scale > MaxScale) { _scale = MaxScale; VelZ = 0; }
            if (_scale < MinScale) { _scale = MinScale; VelZ = 0; }

            // okay, we need to correct the camera position now.


            // get the changes we need to make
            Vector2 originError = originLocation- PixelToTileCoords(_zoomOrigin);
            Vector2 pos = _position + originError;

            // pos could be invalid. we must forcibly validate it.
            if (_isBound)
            {
                // first, get the center.
                float centerX = (float)(((TileBounds().Size.X) / 2f)) + pos.X;
                float centerY = (float)(((TileBounds().Size.Y) / 2f)) + pos.Y;
                Vector2 center = new Vector2(centerX, centerY);

                // rectify positions.
                // the .1fs are for some ridiclous floating point nonsense
                if (center.X > _bounds.Right) { pos.X -= center.X - _bounds.Right; }
                if (center.X < _bounds.Left) { pos.X -=  center.X - _bounds.Left - .1f; }
                if (center.Y > _bounds.Bottom) { pos.Y -= center.Y -_bounds.Bottom; }
                if (center.Y < _bounds.Top) { pos.Y -= center.Y - _bounds.Top -.1f; }

            }
            
            
            setPosition(pos);
            

        }

        public void AddVelocity(float vel, Direction d)
        {
            // get the velocity we need to add.
            Vector2 toAdd = d.ToPoint().ToVector2()*vel;
            VelX += toAdd.X;
            VelY += toAdd.Y;

        }

        // pull 'a' closer to zero by 'b' amount. don't let 'a' overshoot zero.
        private float Reduce(float a, float b)
        {
            // a should remain at zero if it is already there.
            if(a==0)
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
            if(a>0)
            {
                a = 0;
            }

            return a;
        }

    }


   
}
