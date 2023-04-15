using CrystalCore;
using CrystalCore.Input;
using CrystalCore.Model.Elements;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

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

        internal const int BUILD = 996; // I like to increment this number every time I run the code after changing it. I don't always though.
       
        private bool minimapEnabled = true; // setting false is useful for testing graphics stuff.



        // Engine facing objects
        internal Engine Engine { get; private set; } // the 'engine'

        internal GridView view { get; private set; } // the primary view
        private GridView minimap; // the minimap
        internal Map Map { get; private set; } // the world seen by the view and minimap



        // Objects to Setup controls, rulesets, and the (horrible) UI, respectively.
        internal Actions Controls { get; private set; }
        internal Configuration Configuration{get; private set;}
        internal CrudeUI UI { get; private set; }


        // Engine external game state
        private ErrorSplash errorSplash = null;
        internal Ruleset CurrentRuleset { get; set; }


        public CrystalGame()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; // I guess there are reasons this might be false, but it used to be false by default, which was confusing.

        }


        protected override void Initialize()
        {
            //let's make the window a tiny bit bigger, for testing
            _graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

         
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);


            // create the folder for saves, if it does not exist.
            if (!Directory.Exists("Saves"))
            {
                Directory.CreateDirectory("Saves");
            }


            base.Initialize();

        }


        public void OnResize(object sender, EventArgs e)
        {
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            _graphics.ApplyChanges();
        }




        protected override void LoadContent()
        {


            Textures.LoadContent(Content);



            // create the engine
            Engine = new Engine(TargetElapsedTime, GraphicsDevice);

           


            try
            {
                // setup the engine's configuration.
                Configuration = new Configuration(this);

            }
            catch (InitializationFailedException e)
            {
                errorSplash = new ErrorSplash(e.Message, new SpriteBatch(GraphicsDevice));
                Engine = null;
                return;
            }
            catch (Exception e)
            {
                SetErrorSplash("Crystalarium's engine unexpectedly crashed during initialization." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                //Engine = null;
                return;
            }


            Engine.Sim.TargetStepsPS = 10;

            // setup our interaction related code and register it with the engine.
            Controls = new Actions(Engine.Controller, this);

            // Make the UI
            UI = new CrudeUI(this);

            // create a test grid, and do some test things to it.
            Map = Engine.addGrid(CurrentRuleset);

            Map.OnReset += Controls.OnMapReset;


            IBatchRenderer r = (IBatchRenderer)Engine.Renderer; 

            // create a couple test viewports.
            view = Engine.addView(Map, 0, 0, (int)r.Width, (int)r.Height, Configuration.DefaultSkin);
            view.Camera.MinScale = 12;
            //prevent the camera from leaving the world.
            view.SetCameraBound(true);


            // setup the minimap.
            if (minimapEnabled)
            {
                SetupMinimap((int)r.Width);
            }
            
        }

        private void SetupMinimap(int width)
        {
            minimap = Engine.addView( Map, width - 250, 0, 250, 250, Configuration.MiniMapSkin);

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


        // mostly ugly hacks
        protected override void Update(GameTime gameTime)
        {



            if (errorSplash != null)
            {
                return;
            }

            // this is temporary code, meant to demonstrate a viewport's capabilities.

            if (Engine.Controller.Context == "play")
            {
                view.Camera.VelZ += Engine.Controller.DeltaScroll / 150f;
                view.Camera.ZoomOrigin = view.LocalizeCoords(Mouse.GetState().Position);

                    // create ghosts.
                view.CreateGhost(Controls.CurrentType, Controls.GetMousePos(), Controls.Rotation);
            }
            else
            {
                // stop the camera
                view.Camera.Velocity = new Vector3(0);
            }

            // minimap positions
            if (minimapEnabled)
            {
                minimap.Camera.Position = view.Camera.Position;
                minimap.Camera.Zoom = view.Camera.Zoom;
            }



            try
            {
                Engine.Update(gameTime);
            }
            catch (Exception e)
            {
                SetErrorSplash("Crystalarium's engine unexpectedly crashed while updating the simulation." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                //Engine = null;
                return;
            }

            base.Update(gameTime);

          
        }


        protected override void Draw(GameTime gameTime)
        {




            if (errorSplash != null)
            {
                
                errorSplash.Draw(GraphicsDevice);
                //EndDraw(height);
               
                return;
            }


            // make everything a flat color.
            GraphicsDevice.Clear(new Color(70, 70, 70));


            // try to draw the game
            try
            {
                Engine.StartDraw();
            }
            catch (Exception e)
            { 
                
                SetErrorSplash("Crystalarium's engine unexpectedly crashed while rendering graphics." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                Engine.EndDraw();
                //Engine = null;
                return;
            }

            UI.Draw(Engine.Renderer, gameTime);


            Engine.EndDraw();
            base.Draw(gameTime);
            
        

        }




        internal void SetErrorSplash(string s)
        {
            errorSplash = new ErrorSplash(s, new SpriteBatch(GraphicsDevice));
        }

       


    }


   
}
 