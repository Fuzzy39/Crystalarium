using System;
using System.Collections.Generic;
using Crystalarium.Sim;
using Crystalarium.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Crystalarium.Render.ChunkRender;
using Crystalarium.Input;

namespace Crystalarium.Render
{
    public class GridView
    {
        /* A GridView represents an area that renders a grid.   
         * 
         * it has a camera, which controls where elements of the grid appear in the gridview, and if they appear at all.
         * it also has a border, which can be optionally rendered to show where this gridview exists.
         * The gridview itself is responsible for parsing the grid and deciding which elements to render.
         * 
         * If anything seems strange, note that Border, Camera, and Gridview were oringially a single class, ViewPort (or ViewBox, later)
         * 
         */


        // fields


        // basic
        private List<GridView> container; // the list of all existing viewports.
        private Rectangle _pixelBounds; // the bounds, in pixels, of the viewport on the game window.

        // elements
        private Grid _grid; // the grid that this GridView is rendering.
        private Border _border; // the border of this Gridview (which exists, whether it is being rendered or not)
        private Camera _camera; // the camera of the gridview. Responsible for zooming and Panning and actual image rendering


        // Graphical Features
        private Texture2D _background;
        private Color backgroundColor;

        // Chunk Renderer related
        private List<Renderer> _renderers; // list of chunk renderers currently in existence
        private ChunkRender.Type _rendererType; // how are we rendering chunks?

        private GridView debugRenderTarget; // if using debug renderers, this is the viewbox those renderers target.
                                            // I'll admit, this is hacky.
        private Controller _controller; // The controller controlling this gridview. Null if the controller is controlling another grid.


        // Properties
        public Rectangle PixelBounds
        {
            get => _pixelBounds;
        }

        public Grid Grid
        {
            get => _grid;
        }

        public Border Border
        {
            get => _border;
        }

        public Camera Camera
        {
            get => _camera;
        }


        public Texture2D Background
        {
            get => _background;
            set => _background = value;
        }

  
        // Chunk Renderer related
        public List<Renderer> Renderers
        {
            get => _renderers;
        }


        public ChunkRender.Type RendererType
        {
            get => _rendererType;
            set
            {
                _rendererType = value;
                _renderers.Clear();
            }
        }

        public Controller Controller
        {
            get => _controller;
            set => _controller = value;
        }

        // Contstructors


        // create the viewport
        public GridView(List<GridView> container, Grid g, Point pos, Point dimensions)
        {
            // initialize from parameters
            _grid = g;
            this.container = container;
            this.container.Add(this);
            _pixelBounds = new Rectangle(pos, dimensions);
            _renderers = new List<Renderer>();
            _camera = new Camera(this);

            //background
            _background = Textures.viewboxBG;
            backgroundColor = Color.White;

            // border
            _border = new Border(this);

            // renderer type
            _rendererType = ChunkRender.Type.Standard;


        }

        // an alternate viewport constructor, without points.
        public GridView(List<GridView> container, Grid g, int x, int y, int width, int height)
            : this(container, g, new Point(x, y), new Point(width, height)) { }


        public void Destroy()
        {
            container.Remove(this);
        }




        // Methods

        public  Point LocalizeCoords(Point p)
        {
            return p - _pixelBounds.Location;
        }


        public void Draw(SpriteBatch sb)
        {

            // draw the background.
            DrawBackground(sb);

            // Render Textures in the viewport.

            // update chunks to render.
            AddChunks();

            // render them
            for (int i = 0; i < _renderers.Count;)
            {

                Renderer r = _renderers[i];


                // repeat the previous index if this renderer was destroyed.
                if (r.Draw(sb))
                    i++;
            }

            // draw the viewport if in debug mode.
            DrawOtherGridView(sb);

            // finally, draw the border.
            _border.Draw(sb);
        }


        private void DrawBackground(SpriteBatch sb)
        {
            // do not draw the background if no background is set.
            if (Background == null)
                return;

            sb.Draw(Background, _pixelBounds, backgroundColor);
        }

        private void DrawOtherGridView(SpriteBatch sb)
        {
            if (debugRenderTarget != null)
            {
                _camera.RenderTexture(sb, Textures.pixel,
                    debugRenderTarget.Camera.TileBounds(),
                    new Color(0, 0, 0, .3f));
            }
        }



        // only sets the textures for the viewport's borders.
        public void SetTextures(Texture2D sides, Texture2D corners)
        {
            SetTextures(Background, sides, corners);
        }

        // adds chunks to be rendered, if needbe.
        private void AddChunks()
        {
            foreach (List<Chunk> list in _grid.Chunks)
            {
                foreach (Chunk ch in list)
                {
                    if (_camera.TileBounds().Intersects(ch.Bounds))
                    {

                        Renderer.Create(_rendererType, this, ch, _renderers);


                    }
                }
            }

            // for debug renderers, we need to update (or set) their target.
            if (RendererType == ChunkRender.Type.Debug)
            {
                SetDebugRenderTarget(debugRenderTarget);
            }


        }


        public void SetDebugRenderTarget(GridView v)
        {
            debugRenderTarget = v;
            foreach (Renderer r in _renderers)
            {
                ((ChunkRender.Debug)r).Target = v;
            }
        }


        public void AddRenderer(Renderer renderer)
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

        public void Update()
        {
            _camera.Update(_grid.Bounds);
        }
    }
}
