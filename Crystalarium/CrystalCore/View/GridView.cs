using System;
using System.Collections.Generic;
using CrystalCore.Model;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrystalCore.Input;
using CrystalCore.View.Rendering;

using AgentRenderer = CrystalCore.View.Subviews.Agents.AgentView;
using ChunkRenderer = CrystalCore.View.Subviews.ChunkView;
using CrystalCore.View.Configs;

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

        private GridView _viewCastTarget; // if not null, chunks viewed by this gridview (assuming it has the same grid) will be brightened.


        public bool AllowMultipleGhosts { get; set; } // can this gridview contain multiple ghosts, or just one?
        public bool DoDebugPortRendering { get; set; } // should agents render their debug ports?

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


     



        public GridView ViewCastTarget
        {
            get => _viewCastTarget;
            set
            {
                if(skinset.ViewCastOverlay == null)
                {
                    throw new InvalidOperationException("the current skinset does not support grid view casting.");
                }
                if (value == null)
                {
                    _viewCastTarget = null;
                    return;
                }

                if (value == this)
                {

                    throw new InvalidOperationException("A Gridview cannot viewcast itself.");
                }


                if (value.Grid != this.Grid)
                {
                    // hopefully these error messages make sense.
                    throw new InvalidOperationException("Viewcasting requires GridViews that view the same grid.");
                }

                _viewCastTarget = value;


            }
        }


      


        // Contstructors


        // create the viewport
        internal GridView(List<GridView> container, Grid g, Point pos, Point dimensions, ChunkViewConfig renderConfig)
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

            _viewCastTarget = null;
            _viewCastOverlay = null;


        }

        // an alternate viewport constructor, without points.
        internal GridView(List<GridView> container, Grid g, int x, int y, int width, int height, ChunkViewConfig renderConfig)
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
            if (ViewCastTarget == null)
            {
                return;
            }

            if (ViewCastOverlay == null)
            {
                return;
            }

            _camera.RenderTexture(sb, ViewCastOverlay,
                ViewCastTarget.Camera.TileBounds(),
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
