using System;
using System.Collections.Generic;
using Crystalarium.Sim;
using Crystalarium.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Crystalarium.Render.ChunkRender;


namespace Crystalarium.Render
{
    public class Viewbox
    {
        /*
         * A viewport renders a grid.
         * TODO expose border and bg color
         */

        private List<Viewbox> container; // the list of all existing viewports.
        private Rectangle _pixelBounds; // the bounds, in pixels, of the viewport on the game window.
        private Grid _grid; // the grid that this viewport is rendering.
        private Border _border;
        private List<Renderer> _renderers;

        // Graphical Features
        private Texture2D _background;
        private Color backgroundColor;

        // 'Camera' controls
        private double _scale; // the number of pixels that currently represent one tile in gridspace
        private Vector2 _position; // the position of the top left corner of the viewport, in tiles, in grid space

        private int _minScale; // the minumum and maximum amount of pixels that can represent one tile.
        private int _maxScale;


        private ChunkRender.Type _rendererType;
       

        // Properties

     

        public Border Border
        {
            get => _border;
        }

        public Rectangle PixelBounds
        {
            get => _pixelBounds;
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

        // when setting position with the position property, position is the location, in tile space, of the center of the viewport.
        public Vector2 Position
        {
            //get => _position; // too lazy to implement this properly. If we need it, I'll add it later.
            set
            {
               
                float x = (float)(-1f * ( (TileBounds().Size.X) / 2f))+ value.X+.5f;
                float y = (float)(-1f * ( (TileBounds().Size.Y)/2f)) + value.Y+.5f;
                _position = new Vector2(x, y);
                

            }
        }

        // this is pretty simple, honestly.
        public Texture2D Background
        {
            get => _background;
            set => _background = value;
        }

        // makes enough sense.
        public ChunkRender.Type RendererType
        {
            get => _rendererType;
            set
            { 
                _rendererType = value;
                _renderers.Clear();
            }
        }





        // create the viewport
        public Viewbox(List<Viewbox> viewports, Grid g, Point pos, Point dimensions)
        {
            // initialize from parameters
            _grid = g;
            container = viewports;
            container.Add(this);
            _pixelBounds = new Rectangle(pos, dimensions);
            _renderers = new List<Renderer>();

            //background
            _background = Textures.viewboxBG;

            // default scale values
            _minScale = 10;
            _maxScale = 50;

            // set the 'camera' to reasonable values
            _scale = (_minScale + _maxScale) / 2.0;
            Position = new Vector2(0, 0);

            // set default colors.
            backgroundColor = Color.White;

            // border
            _border = new Border(this);

            // renderer type
            _rendererType = ChunkRender.Type.Standard;


        }

        // an alternate viewport constructor, without points.
        public Viewbox(List<Viewbox> viewports, Grid g, int x, int y, int width, int height)
            : this(viewports, g, new Point(x, y), new Point(width, height)) { }






        public void Destroy()
        {
            container.Remove(this);
        }

        public void AddRenderer( Renderer renderer) 
        {
           

            _renderers.Add(renderer);
        }

        public void RemoveRenderer(Renderer renderer)
        {
            _renderers.Remove(renderer);
        }

        // sets all textures that are part of a viewport itself.
        public void SetTextures(Texture2D background, Texture2D sides, Texture2D corners)
        {

            Background = background;
            _border.SetTextures(sides, corners);
           
        }

        // only sets the textures for the viewport's borders.
        public void SetTextures(Texture2D sides, Texture2D corners)
        {
            SetTextures(Background, sides, corners);
        }

        // adds chunks to be rendered, if needbe.
        private void AddChunks()
        {
            foreach(List<Chunk> list in _grid.Chunks)
            {
                foreach (Chunk ch in list)
                {
                    if (TileBounds().Intersects(ch.Bounds))
                    {

                        Renderer.Create(_rendererType, this, ch, _renderers);
                    }
                }
            }
        }




        // Drawing code:

        public void Draw(SpriteBatch sb)
        {

            // draw the background.
            DrawBackground(sb);

            // Render Textures in the viewport.

            // update chunks to render.
            AddChunks();

            // render them
            for (int i = 0; i < _renderers.Count; i++)
            {
                Renderer r = _renderers[i];
                r.Draw(sb);
            }

            // finnally, draw the border.
            _border.Draw(sb);
        }


        private void DrawBackground(SpriteBatch sb)
        {
            // do not draw the background if no background is set.
            if (Background == null)
                return;

            sb.Draw(Background, _pixelBounds, backgroundColor);
        }

        // returns the bounds in tilespace of the viewport
        public RectangleF TileBounds()
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
        public bool RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle bounds)
        {
            // check if the texture needs to be rendered by this viewport
            if(!TileBounds().Intersects(bounds))
            {
                return false;
            }


            //it does! collect some basic information.
            // we add a couple pixels to the size of things
            Point pixelCoords = TiletoPixelCoords(bounds.Location.ToVector2())+new Point(1);
            Point pixelSize = new Point((int)(bounds.Size.X * _scale), (int)(bounds.Size.Y * _scale))+new Point(1);

          
            // partial rendering...
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

            topLeft = topLeft + this._pixelBounds.Location;

            // figure out the size of the rectangle we need to draw.
            int rightSide = this._pixelBounds.X + this._pixelBounds.Width;
            size.X = GetRenderSize(rightSide, texturePixelBounds.Size.X, leftCut, topLeft.X, out rightCut);

            int bottomSide = this._pixelBounds.Y + this._pixelBounds.Height;
            size.Y = GetRenderSize(bottomSide, texturePixelBounds.Size.Y, topCut, topLeft.Y, out bottomCut);


            Rectangle sourceRect = GetTextureSourceBounds(topCut, bottomCut, leftCut, rightCut, texturePixelBounds, texture);


            sb.Draw(
                       texture,
                       new Rectangle(topLeft, size),
                       sourceRect,
                       Color.White


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
            float textureWidth = (float)(texturePixelBounds.Width - leftCut - rightCut); 

            // get the width of the source rectangle, as a ratio of total width of the texuture
            float textureWidthRatio = textureWidth / (float)texturePixelBounds.Width;

            // get the width of the source rectangle in pixels
            int sourceWidth = (int)( textureWidthRatio * texture.Width );


            // get the height, in pixels, of the destination.
            float textureHeight = (float)(texturePixelBounds.Height - topCut - bottomCut);

            // get the height of the source rectangle, as a ratio of total width of the texuture
            float textureHeightRatio = textureHeight / (float)texturePixelBounds.Height;

            // get the height of the source rectangle in pixels
            int sourceHeight = (int)(textureHeightRatio * texture.Height);


            return new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);

        }
    }
}
