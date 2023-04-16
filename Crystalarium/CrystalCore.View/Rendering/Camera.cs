
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Rendering
{
    /// <summary>
    /// The Camera Class renders images in a coordinate space.
    /// the camera's position in this coordinate space as well as the scale the space is rendered at can be freely changed.
    /// </summary>
    public class Camera : IRenderer
    {



        /// <summary>
        /// The  <see cref="BoundedRenderer">BoundedRenderer</see> this Camera uses to render textures. It also contains this Camera's pixel bounds.
        /// </summary>
        private BoundedRenderer renderer;

        /// <summary>
        /// the number of pixels that currently represent one tile in gridspace.
        /// </summary>
        protected double _scale;

        /// <summary>
        /// the position of the top left corner of the Camera, in tiles, in grid space.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        /// The minumum amount of pixels that can represent one tile.
        /// </summary>
        private int _minScale;

        /// <summary>
        /// The maximum amount of pixels that can represent one tile.
        /// </summary>
        private int _maxScale;

        /// <summary>
        /// The tilespace to where the center of the view is confined, if <see cref="_isBound">_isBound</see> is true.
        /// </summary>
        protected Rectangle _bounds;

        /// <summary>
        /// Whether the position of this camera is bound to <see cref="_bounds">_bounds</see> or whether it is free.
        /// </summary>
        protected bool _isBound; 

        /// <summary>
        ///  Whether this Camera is bound to <see cref="Bounds">Bounds</see> or whether it is free.
        /// </summary>
        public bool IsBound
        {
            get => _isBound;
            internal set
            {
               
                _isBound = value;
            }
            
        }

        /// <summary>
        /// The Rectangle in tile cooridinates where the center of this Camera's view is bound, if <see cref="IsBound">IsBound</see> is true.
        /// </summary>
        public Rectangle Bounds
        {
            get => _bounds;
            internal set => _bounds = value;
        }

        /// <summary>
        /// The minumum amount of pixels that can represent one tile.
        /// </summary>
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

        /// <summary>
        /// The maximum amount of pixels that can represent one tile.
        /// </summary>
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

        /// <summary>
        /// the number of pixels that currently represent one tile in gridspace.
        /// </summary>
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
        /// <summary>
        /// The position of the center of this Camera's view, in tile coordinates.
        /// </summary>
        public virtual Vector2 Position
        {
            get
            {
                Vector2 toReturn = new Vector2();
                toReturn.X = _position.X + (TileBounds().Size.X / 2.0f);
                toReturn.Y = _position.Y + (TileBounds().Size.Y / 2.0f);
                return toReturn;
            }
            set
            {
                float x = (float)(-1f * ((TileBounds().Size.X) / 2f)) + value.X;
                float y = (float)(-1f * ((TileBounds().Size.Y) / 2f)) + value.Y;
                SetPosition(new Vector2(x, y));
               

            }
        }

        
        /// <summary>
        /// The Position of the top left corner of the Camera's view, in tile coordinates.
        /// </summary>
        public virtual Vector2 OriginPosition
        {
            get => _position;
            set => value = _position;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelBoundry">The Bounds in pixels relative to the top left corner of the game window.</param>
        internal Camera( Rectangle pixelBoundry, IRenderer rend)
        {

            renderer = new BoundedRenderer( pixelBoundry, rend);

            // default scale values
            _minScale = 10;
            _maxScale = 100;

            // set the 'camera' to reasonable values
            _scale = (_minScale + _maxScale) / 2.0;
            _position = new Vector2(0, 0);


           
            _bounds = new Rectangle(0, 0, 1, 1);
            _isBound = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="bounds">The new value of <see cref="Bounds">Bounds</see>. </param>
        /// <exception cref="InvalidOperationException">Thrown if the Camera is bound and is outside of it's bounds.</exception>
        internal virtual void Update(Rectangle bounds)
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
        /// <summary>
        /// Sets the position of this camera, if it is inside of bounds.
        /// If not, this method will attempt to set the X and the Y independently.
        /// </summary>
        /// <param name="pos">The Position that the camera's top left corner will be set to.</param>
        /// <returns>whether the requested position was set.</returns>
        protected bool SetPosition(Vector2 pos)
        {
            float centerX = (float)(((TileBounds().Size.X) / 2f)) + pos.X;
            float centerY = (float)(((TileBounds().Size.Y) / 2f)) + pos.Y;
            Vector2 nextCenter = new Vector2(centerX, centerY);

            if ((!_isBound) || new RectangleF(_bounds).Contains(nextCenter))
            {

                _position = pos;
                return true;
            }

            // granularly set X and y, if applicable.
            if(_bounds.Left<=nextCenter.X &  _bounds.Right>=nextCenter.X)
            {
                _position.X = pos.X;
                return false;
            }

            if (_bounds.Top <= nextCenter.Y & _bounds.Bottom >= nextCenter.Y)
            {
                _position.Y = pos.Y;
                return false;
            }


            return false;


        }


        // returns  pixel coords relative to start of viewport.
        // this also works outside of the viewport.
        /// <summary>
        ///  Translates Tile coordinates to pixel coordinates, regardless to whether the camera is viewing this coordinate.
        /// </summary>
        /// <param name="tilePos"></param>
        /// <returns>the pixel coords where tilePos is located, relative to the top left corner of this camera's bounds.</returns>
        public Point TileToPixelCoords(Vector2 tilePos)
        {

            // tile to pixel:
            // first, tile to pixel relative to base coords
            // how do we do that?
            // -(camera tile pos - tile pos) tile pos relative to base

            int x = (int)(Scale * -1.0 * (_position.X - (float)tilePos.X));
            int y = (int)(Scale * -1.0 * (_position.Y - (float)tilePos.Y));
            return new Point(x, y);

        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelPos">Relative to the top left corner of this camera's pixel bounds.</param>
        /// <returns></returns>
        public Vector2 PixelToTileCoords(Point pixelPos)
        {
            float x = (float)(_position.X + (pixelPos.X / _scale));
            float y = (float)(_position.Y + (pixelPos.Y / _scale));
            return new Vector2(x, y);
        }

        

        // returns the bounds in tilespace of the viewport
        /// <summary>
        /// 
        /// </summary>
        /// <returns>The tile coordinates this camera can currently see.</returns>
        public RectangleF TileBounds()
        {
            return new RectangleF(_position.X, _position.Y,
                (float)(renderer.PixelBoundry.Width / Scale),
                (float)(renderer.PixelBoundry.Height / Scale));
        }


        public void Draw(Texture2D texture, RotatedRect rect, Rectangle source, Color c)
        {

            if(texture.Width!= source.Width || texture.Height!= source.Height)
            {
                throw new NotImplementedException("Wait for render target!");
            }

            RectangleF bounds = rect.AsRectangleF;
            if (bounds.Area < 0)
            {
                throw new ArgumentException("A Camera was asked to render a texture with bounds " + bounds + ". Negative size is not acceptable.");
            }

            if (bounds.Area == 0)
            {
                return;
            }

            //Console.WriteLine(rect.BoundingBox);

            Vector2 size = rect.AdjustedSize;
            Point pixelCoords = TileToPixelCoords(rect.BoundingBox.Location) - new Point(1);
            Point pixelSize = new Point((int)(size.X * _scale), (int)(size.Y * _scale)) + new Point(1, 1);

         


            renderer.RenderTexture( texture, new Rectangle(pixelCoords, pixelSize), c, DirectionUtil.FromRadians(rect.Rotation));

        }

     

        // whatever, I'll fix this in a bit, once we have rendertargets
        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            throw new NotImplementedException();
        }

       
    }
}
