using System;
using System.Collections.Generic;
using CrystalCore.Model;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrystalCore.View.ChunkRender;
using CrystalCore.Input;
using CrystalCore.View.Rendering;

using AgentRenderer = CrystalCore.View.AgentRender.AgentView;
using ChunkRenderer = CrystalCore.View.ChunkRender.ChunkView;
using CrystalCore.View.AgentRender;

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
        private SubviewManager _subviewManager; // our subview manager, who kindly takes after our subviews.

        // Graphical Features
        private Texture2D _background;
        private Color backgroundColor;

        // Chunk Renderer related
        private ChunkViewTemplate _renderConfig; // how are we rendering chunks?
        private bool _doAgentRendering; // whether we render agents.




        public bool AllowMultipleGhosts { get; set; }
        public bool DoDebugPortRendering { get; set; }


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

        internal SubviewManager Manager
        {
            get => _subviewManager;
        }


        public Texture2D Background
        {
            get => _background;
            set => _background = value;
        }



        public bool DoAgentRendering // whether agents are rendered in this gridview
        {
            get => _doAgentRendering;
            set
            {
                _doAgentRendering = value;
                _subviewManager.AgentViews.Clear();
            }

        }

        public ChunkViewTemplate RenderConfig
        {
            get => _renderConfig;

        }


        // Contstructors


        // create the viewport
        internal GridView(List<GridView> container, Grid g, Point pos, Point dimensions, ChunkViewTemplate renderConfig)
        {
            // initialize from parameters
            _grid = g;
            this.container = container;
            this.container.Add(this);
            _pixelBounds = new Rectangle(pos, dimensions);

            _camera = new PhysicsCamera(PixelBounds);

            _subviewManager = new SubviewManager(this);

            //background
            _background = null;
            backgroundColor = Color.White;

            // border
            _border = new Border(this);

            // renderer type
            _renderConfig = renderConfig;
            DoAgentRendering = true;

            // ghost related


            AllowMultipleGhosts = false;
            DoDebugPortRendering = false;


        }

        // an alternate viewport constructor, without points.
        internal GridView(List<GridView> container, Grid g, int x, int y, int width, int height, ChunkViewTemplate renderConfig)
            : this(container, g, new Point(x, y), new Point(width, height), renderConfig) { }


        public void Destroy()
        {
            container.Remove(this);
        }


        public Point LocalizeCoords(Point p)
        {
            return p - _pixelBounds.Location;
        }




        internal void Draw(SpriteBatch sb)
        {

            // draw the background.
            DrawBackground(sb);


            // Update our subview manager and have it render its subviews.
            _subviewManager.Draw(sb);


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
            if (RenderConfig.ViewCastTarget == null)
            {
                return;
            }

            if (RenderConfig.ViewCastOverlay == null)
            {
                return;
            }

            _camera.RenderTexture(sb, RenderConfig.ViewCastOverlay,
                RenderConfig.ViewCastTarget.Camera.TileBounds(),
                new Color(.2f, .2f, .2f, .001f));

        }






        internal void Update()
        { 
            try
            {
                _camera.Update(_grid.Bounds);
            }
            catch
            {
                Console.WriteLine("Camera is out of bounds. Resetting position.");
                _camera.Position = new Vector2(Grid.Bounds.Width/2f, Grid.Bounds.Height/2f);
            }
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
