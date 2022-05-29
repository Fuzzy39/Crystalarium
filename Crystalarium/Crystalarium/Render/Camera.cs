
using Crystalarium.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Render
{
    public class Camera 
    {

        /*
         * A Camera Uses a BoundedRenderer and maps pixels to a tile space that can be scrolled and scaled.
         * 
         */

        private BoundedRenderer renderer;


        public double _scale; // the number of pixels that currently represent one tile in gridspace
        private Vector2 _position; // the position of the top left corner of the viewport, in tiles, in grid space


        private int _minScale; // the minumum and maximum amount of pixels that can represent one tile.
        private int _maxScale;

        protected Rectangle _bounds; // the tilespace to where the center of the view is confined.
        protected bool _isBound; // whether the position of this renderer is bound or whether it is free.

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

        public int MinScale
        {
            get => _minScale;
            set
            {
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
                SetPosition(new Vector2(x, y));
               

            }
        }

        // Origin Position represents the location in tilespace in tiles of the top left corner of this renderer.
        public Vector2 OriginPosition
        {
            get => _position;
            set => value = _position;
        }



        public Camera(Rectangle pixelBoundry)
        {

            renderer = new BoundedRenderer(pixelBoundry);

            // default scale values
            _minScale = 10;
            _maxScale = 50;

            // set the 'camera' to reasonable values
            _scale = (_minScale + _maxScale) / 2.0;
            _position = new Vector2(0, 0);


           
            _bounds = new Rectangle(0, 0, 1, 1);
            _isBound = false;
        }

        public virtual void Update(Rectangle bounds)
        {
            _bounds = bounds;

            // check that the renderer's position is in bounds. If bounded, the camera should NEVER be out of bounds.
            if (_isBound & !new RectangleF(_bounds).Contains(Position))
            {
                throw new InvalidOperationException(Position + " is out of bounds " + _bounds + " for this Camera.");
            }

        }



        // sets the position of the camera if that position would be in bounds.
        // pos represents the location of the top left corner of the screen.
        public bool SetPosition(Vector2 pos)
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
        public Vector2 PixelToTileCoords(Point pixelPos)
        {
            float x = (float)(_position.X + (pixelPos.X / _scale));
            float y = (float)(_position.Y + (pixelPos.Y / _scale));
            return new Vector2(x, y);
        }



        // returns the bounds in tilespace of the viewport
        public RectangleF TileBounds()
        {
            return new RectangleF(_position.X, _position.Y,
                (float)(renderer.PixelBoundry.Width / Scale),
                (float)(renderer.PixelBoundry.Height / Scale));
        }



        //Rendering stuff

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
            // stuff

           

            Point pixelCoords = TileToPixelCoords(bounds.Location) - new Point(1);
            Point pixelSize = new Point((int)(bounds.Size.X * _scale), (int)(bounds.Size.Y * _scale)) + new Point(1, 1);
        

            return renderer.RenderTexture(sb, texture, new Rectangle(pixelCoords, pixelSize), c);

        }


    }
}
