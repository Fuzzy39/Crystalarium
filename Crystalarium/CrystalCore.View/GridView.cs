using CrystalCore.Model.Elements;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using CrystalCore.View.Configs;
using CrystalCore.View.Core;
using CrystalCore.View.Rendering;
using CrystalCore.View.Subviews.Agents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CrystalCore.View
{
    public class GridView : IRenderable
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
        private Map _map; // the grid that this GridView is rendering.
        private Border _border; // the border of this Gridview (which exists, whether it is being rendered or not)
        private PhysicsCamera _camera; // the camera of the gridview. Responsible for zooming and Panning and actual image rendering
        //private RenderTarget2D _target; // the target this gridview is rendered to.
        private SubviewManager _subviewManager; // our subview manager, who kindly takes after our subviews.
        private SkinSet _skinSet; // Our Current Skinset, which defines any graphical settings for anything we could possibly render.

        private GridView _viewCastTarget; // if not null, chunks viewed by this gridview (assuming it has the same grid) will be brightened.

        private bool _doAgentRendering; // whether a gridview with this skin renders agents and signals.
        public bool AllowMultipleGhosts { get; set; } // can this gridview contain multiple ghosts, or just one?
        public bool DoDebugRendering { get; set; } // should agents render their debug ports and signals?

        // Properties
        public Rectangle PixelBounds
        {
            get => _pixelBounds;
        }

        public Map Map
        {
            get => _map;
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
                return _skinSet.GetSkin(Map.Ruleset);
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


                if (value.Map != this.Map)
                {
                    // hopefully these error messages make sense.
                    throw new InvalidOperationException("Viewcasting requires GridViews that view the same grid.");
                }

                _viewCastTarget = value;


            }
        }


      


        // Contstructors


        // create the viewport
        public GridView(List<GridView> container, Map g, Point pos, Point dimensions, SkinSet skinSet)
        {
            // initialize from parameters
            _map = g;
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
            DoDebugRendering = false;

            _viewCastTarget = null;



        }

        // an alternate viewport constructor, without points.
        internal GridView(List<GridView> container, Map g, int x, int y, int width, int height, SkinSet skinSet)
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



        public void Draw(IRenderer rend)
        {
            //sb.End();

            //sb.GraphicsDevice.SetRenderTarget(_target);
            //sb.GraphicsDevice.Clear(Color.Magenta);

            // no, I don't know what these settings do.
            // could look it up...
            //sb.Begin();


            // draw the background.
            DrawBackground(rend);

            // Update our subview manager and have it render its subviews.
            _subviewManager.Draw(rend);

            // draw the viewport if in debug mode.
            DrawOtherGridView(rend);
            //sb.End();


            //sb.GraphicsDevice.SetRenderTarget(null);

            //sb.Begin();

            //sb.Draw(_target, PixelBounds, Color.White);

            // finally, draw the border.
            _border.Draw(rend);   
        }


        private void DrawBackground(IRenderer rend)
        {
            // do not draw the background if no background is set.
            if (CurrentSkin.GridViewBG == null)
                return;

            rend.Draw(CurrentSkin.GridViewBG, _pixelBounds, CurrentSkin.GridViewBGColor);
           
        }

        private void DrawOtherGridView(IRenderer rend)
        {
            if (ViewCastTarget == null)
            {
                return;
            }

            _camera.RenderTexture(sb, SkinSet.ViewCastOverlay,
                ViewCastTarget.Camera.TileBounds(),
                new Color(.2f, .2f, .2f, .001f));

        }

        public void Update()
        { 
            try
            {
                _camera.Update(_map.Bounds);
            }
            catch
            {

                _camera.Position = Map.Center;
                Console.WriteLine("Camera is out of bounds. Resetting position to "+_camera.Position);
            }
        }


        public void SetCameraBound(bool bound)
        {

            if (bound)
            {
                Camera.Position = _map.Center; // we want to prevent a crash, if the camera is in an invalid position when it is bound.
            }
            Camera.IsBound = bound;

        }
        
        public void OnGridReset(Object sender, EventArgs e)
        {
            Reset();
        }

        // this should be called whenever our skin changes.
        public void Reset()
        {
            Manager.Reset();
            Camera.Position = _map.Center;
        }
    }
}
