using System;
using System.Collections.Generic;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrystalCore.Input;
using CrystalCore.View.Rendering;

using AgentRenderer = CrystalCore.View.Subviews.Agents.AgentView;
using ChunkRenderer = CrystalCore.View.Subviews.ChunkView;
using CrystalCore.View.Configs;
using CrystalCore.View.Subviews.Agents;
using CrystalCore.Model.Rulesets;
using CrystalCore.Model.Grids;

namespace CrystalCore.View
{
    public class GridView
    {
        /* A GridView represents an area that renders a grid.   
         * 
         * it has a camera, which controls where elements of the grid appear in the gridview, and if they appear at all.
         * it also has a border, which can be optionally rendered to show where this gridview   exists.
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
        private SkinSet _skinSet; // Our Current Skinset, which defines any graphical settings for anything we could possibly render.

        private GridView _viewCastTarget; // if not null, chunks viewed by this gridview (assuming it has the same grid) will be brightened.

        private bool _doAgentRendering; // whether a gridview with this skin renders agents and signals.
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

        public SkinSet SkinSet
        {
            get => _skinSet;
            set { _skinSet = value; Reset(); }
        }

        public Skin CurrentSkin
        {
            get
            {
                return _skinSet.GetSkin(Grid.Ruleset);
            }
        }

        public bool DoAgentRendering // whether agents are rendered in this gridview
        {
            get => _doAgentRendering;
            set
            {
                    _doAgentRendering = value;
            }

        }


        public GridView ViewCastTarget
        {
            get => _viewCastTarget;
            set
            {
               
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
        internal GridView(List<GridView> container, Grid g, Point pos, Point dimensions, SkinSet skinSet)
        {
            // initialize from parameters
            _grid = g;
            g.OnReset += OnGridReset;

            this.container = container;
            this.container.Add(this);
            _pixelBounds = new Rectangle(pos, dimensions);

            _camera = new PhysicsCamera(PixelBounds);

            _subviewManager = new SubviewManager(this);

            _skinSet = skinSet;

          

            // border
            _border = new Border(this);

            // Rendering options.
            DoAgentRendering = true;
            AllowMultipleGhosts = false;
            DoDebugPortRendering = false;

            _viewCastTarget = null;
            


        }

        // an alternate viewport constructor, without points.
        internal GridView(List<GridView> container, Grid g, int x, int y, int width, int height, SkinSet skinSet)
            : this(container, g, new Point(x, y), new Point(width, height), skinSet) { }


        public void Destroy()
        {
            container.Remove(this);
        }


        public Point LocalizeCoords(Point p)
        {
            return p - _pixelBounds.Location;
        }

        public void CreateGhost( AgentType t, Point loc, Direction facing)
        {
            AgentViewConfig conf =CurrentSkin.GetAgentViewConfig(t);
            Manager.AddGhost(new AgentGhost(this, conf, loc, facing));
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
            if (CurrentSkin.GridViewBG == null)
                return;

            sb.Draw(CurrentSkin.GridViewBG, _pixelBounds, CurrentSkin.GridViewBGColor);
        }

        private void DrawOtherGridView(SpriteBatch sb)
        {
            if (ViewCastTarget == null)
            {
                return;
            }

            _camera.RenderTexture(sb, SkinSet.ViewCastOverlay,
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

            Camera.Position = _grid.Center; // we want to prevent a crash, if the camera is in an invalid position when it is bound.
            Camera.IsBound = true;

        }

        public void unbindCamera()
        {
            Camera.IsBound = false;
        }
        
        public void OnGridReset(Object sender, EventArgs e)
        {
            Reset();
        }

        // this should be called whenever our skin changes.
        public void Reset()
        {
            Manager.Reset();
        }
    }
}
