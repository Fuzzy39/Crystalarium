using CrystalCore;
using CrystalCore.Model.Core;
using CrystalCore.Model.Rules;
using CrystalCore.Util.Profiling;
using CrystalCore.View;
using CrystalCore.View.Core;
using CrystalCore.View.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;



namespace Crystalarium.Main
{
    public class CrystalGame : Game
    {

 

        /*
         * Welcome to Crystalarium! This is the primary file of the game.
         * At the moment, much of it is 'test code' and liable to some big changes, and maybe a complete rewrite or two down the road.
         * Don't expect this code to be super pretty just yet. The bulk of the systems that are more built up are in CrystalCore, the 'engine' of the game.
         * 
         */


        // Misc.
        private GraphicsDeviceManager _graphics;

        // version number.


        internal static string VersionString
        {
            get
            {

                Version version = new Version(8, 5, 1302);//System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

#if DEBUG
                string type = "(Experiemental)";
#else
                string type = "(Development)";
                if(version.Minor == 0)
                    type = "(Release)";
                
#endif
                return "Milestone " + (version.Major + 1) + " "+type+" v" + version.Major+"."+version.Minor+"."+version.Build;
            }
        }



        // Engine facing objects
        internal Engine Engine { get; private set; } // the 'engine'

        internal GridView view { get; private set; } // the primary view
        private GridView minimap; // the minimap

        private bool _minimapEnabled;
        internal bool MinimapEnabled 
        { 
            get
            {
                return _minimapEnabled;
            }

            set
            {
                _minimapEnabled = value;
                if (!value)
                {
                    Engine.removeView(minimap);
                    return;
                }

                Point size = new Point(350, 300);
                minimap = Engine.addView(Map, (int)Engine.Renderer.Width - size.X, 0, size.X, size.Y, Configuration.MiniMapSkin);

                // setup borders
                minimap.Border.SetTextures(Textures.pixel, Textures.pixel);
                minimap.Border.Width = 2;

                // Set the camera of the minimap.
                minimap.Camera.MaxScale = 15;
                minimap.Camera.MinScale = 1;

                // to make it a minimap!
                minimap.ViewCastTarget = view; // note that this must be done after view has been initialized.
                                               //minimap.DoAgentRendering = false;
            }

        }

        private Map _map;
        // the world seen by the view and minimap
        internal Map Map 
        {
            get { return _map; }
            set 
            { 
                if(_map!=null) Engine.removeMap(_map); 
                _map = value;

                Actions.onMapChange(); // this is silly.

                // setup views
                if (view!=null) view.Map = _map;
                if (_minimapEnabled) minimap.Map = _map;

            } 
        } 



        // Objects to Setup controls, rulesets, and the (horrible) UI, respectively.
        internal Actions Actions { get; private set; }
        internal Configuration Configuration { get; private set; }
        internal CrudeUI UI { get; private set; }


        // Engine external game state
        internal Ruleset CurrentRuleset { get => Map.Ruleset; }

        private ProfilingTask frameTask;


        public CrystalGame()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; // I guess there are reasons this might be false, but it used to be false by default, which was confusing.
            IsFixedTimeStep = false;
         

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;



            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);


        }


        protected override void Initialize()
        {


            // create the folder for saves, if it does not exist.
            if (!Directory.Exists("Saves"))
            {
                Directory.CreateDirectory("Saves");
            }

           

            base.Initialize();

        }


        public void OnResize(object sender, EventArgs e)
        {
            // uh, actively resizing seems to make things glitch out.
            // this is reasonable.
            // so I guess we don't. That works betterg
        }




        protected override void LoadContent()
        {

            Textures.LoadContent(Content);

            // create the engine
            Engine = new Engine(TargetElapsedTime, GraphicsDevice, Textures.Consolas);


            // setup the engine's configuration.
            Configuration = new Configuration(this);
            Engine.Sim.TargetStepsPS = 10;


            // setup our interaction related code and register it with the engine.
            Actions = new Actions(Engine.Controller, this);


            // let's get this show on the road!
            Engine.Initialize();
        

            // Make the UI
            UI = new CrudeUI(this);

            // create a test grid, and do some test things to it.
            Map = Engine.addMap(Engine.Rulesets[0]);




            IBatchRenderer r = Engine.Renderer;

            // create a couple test viewports.
            view = Engine.addView(Map, 0, 0, (int)r.Width, (int)r.Height, Configuration.DefaultSkin);
            view.Camera.MinScale = 12;
            //prevent the camera from leaving the world.
            view.SetCameraBound(true);


            // setup the minimap.
            MinimapEnabled = true;

        }

       

        // mostly ugly hacks
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // this should go first, apparently.

            // start profiling
            if (frameTask==null || frameTask.Finished) frameTask = new("Frame");
            using (new ProfilingTask("Update"))
            {
                // almost all of this code probably deserves to be moved.
                if (Engine.Controller.Context == "play" && IsActive)
                {
                    view.Camera.VelZ += Engine.Controller.DeltaScroll*60f / 150f;
                    // HACK
                    view.Camera.ZoomOrigin = view.LocalizeCoords(
                        //Mouse.GetState().Position); // For basic Renderer
                        ((ScaledRenderer)Engine.Renderer).ToVirtualResolution(Mouse.GetState().Position.ToVector2()).ToPoint());

                    // create ghosts.
                    view.CreateGhost(Actions.CurrentType, Actions.GetMousePos(), Actions.Rotation);
                }
                else
                {
                    // stop the camera
                    view.Camera.Velocity = new Vector3(0);
                }

                // minimap positions
                if (MinimapEnabled)
                {
                    minimap.Camera.Position = view.Camera.Position;
                    minimap.Camera.Zoom = view.Camera.Zoom;
                }


                Engine.Update(gameTime, IsActive);


                
            }

        }



        protected override void Draw(GameTime gameTime)
        {

            using (new ProfilingTask("Draw"))
            {
                // make everything a flat color.
                GraphicsDevice.Clear(new Color(70, 70, 70));
             
                // draw the game
                Engine.StartDraw();
                

                // for the time being, the game handles the 'UI' as the engine has no such systems. 
                UI.Draw(Engine.Renderer, gameTime);
                
                // wrap up.
                Engine.EndDraw();
                base.Draw(gameTime);
            }
            frameTask.Dispose();
            frameTask = null;
        }




    }



}
