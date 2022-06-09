using System;
using System.Collections.Generic;
using CrystalCore.Sim;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrystalCore.View.ChunkRender;
using CrystalCore.Input;
using CrystalCore.View.Render;

namespace CrystalCore.View
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
        private PhysicsCamera _camera; // the camera of the gridview. Responsible for zooming and Panning and actual image rendering


        // Graphical Features
        private Texture2D _background;
        private Color backgroundColor;

        // Chunk Renderer related
        private List<RendererBase> _renderers; // list of chunk renderers currently in existence
        private RendererTemplate _renderConfig; // how are we rendering chunks?

     
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

        public PhysicsCamera Camera
        {
            get => _camera;
        }


        public Texture2D Background
        {
            get => _background;
            set => _background = value;
        }

  
        // Chunk Renderer related
        internal List<RendererBase> Renderers
        {
            get => _renderers;
        }


        public RendererTemplate RenderConfig
        {
            get => _renderConfig;
            set
            {
                _renderConfig = value;
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
        internal GridView(List<GridView> container, Grid g, Point pos, Point dimensions, RendererTemplate renderConfig)
        {
            // initialize from parameters
            _grid = g;
            this.container = container;
            this.container.Add(this);
            _pixelBounds = new Rectangle(pos, dimensions);
            _renderers = new List<RendererBase>();
            _camera = new PhysicsCamera(PixelBounds);

            //background
            _background = null;
            backgroundColor = Color.White;

            // border
            _border = new Border(this);

            // renderer type
            _renderConfig = renderConfig;


        }

        // an alternate viewport constructor, without points.
        internal GridView(List<GridView> container, Grid g, int x, int y, int width, int height, RendererTemplate renderConfig)
            : this(container, g, new Point(x, y), new Point(width, height), renderConfig) { }


        public void Destroy()
        {
            container.Remove(this);
        }




        // Methods

        public Point LocalizeCoords(Point p)
        {
            return p - _pixelBounds.Location;
        }

     


        internal void Draw(SpriteBatch sb)
        {

            // draw the background.
            DrawBackground(sb);

            // Render Textures in the viewport.

            // update chunks to render.
            AddChunks();

            // render them
            for (int i = 0; i < _renderers.Count;)
            {

                RendererBase r = _renderers[i];


                // repeat the previous index if this renderer was destroyed.
                //if (r.Draw(sb))
                    i++;
            }

            // draw the viewport if in debug mode.
            DrawOtherGridView(sb);

            // TEST IMAGE

            _camera.RenderTexture(sb, RenderConfig.ViewCastOverlay,
                new Rectangle(5, 5, 5, 5),
                Color.White, Direction.down);   

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
            if (RenderConfig.ViewCastTarget == null)
            {
                return;
            }

            if(RenderConfig.ViewCastOverlay == null)
            {
                return;
            }
                
            _camera.RenderTexture(sb, RenderConfig.ViewCastOverlay,
                RenderConfig.ViewCastTarget.Camera.TileBounds(),
                new Color(.2f, .2f, .2f, .001f));
            
        }


        // adds chunks to be rendered, if needbe.
        private void AddChunks()
        {
            foreach (List<Chunk> list in _grid.Chunks)
            {
                foreach (Chunk ch in list)
                {

                    if (!_camera.TileBounds().Intersects(ch.Bounds))
                    {
                        continue;
                    }

                    // ensure this chunk does not already exist
                    bool existing = false;
                    foreach (Renderer r in _renderers)
                    {
                        if (r.RenderData == ch)
                        {
                            existing = true;
                        }

                    }

                    // this is really poetic.
                    // use this if statement to guide you in life.
                    if (existing)
                    {
                        continue;
                    }

                    RenderConfig.CreateRenderer(this, ch, _renderers);


                    
                }
            }


        }


        internal void AddRenderer(RendererBase renderer)
        {


            _renderers.Add(renderer);
        }

        internal void RemoveRenderer(RendererBase renderer)
        {
            _renderers.Remove(renderer);
        }

   

        internal void Update()
        {
            _camera.Update(_grid.Bounds);
        }


        public void bindCamera()
        {

            Camera.Position = _grid.center; // we want to prevent a crash, if the camera is in an invalid position when it is bound.
            Camera.IsBound = true;

        }

        public void unbindCamera()
        {
            Camera.IsBound = false;
        }
    }
}
