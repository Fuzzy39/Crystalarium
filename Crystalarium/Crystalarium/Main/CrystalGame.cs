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

        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
            
        internal Engine Engine { get; private set; } // the 'engine'

        private const int BUILD = 954; // I like to increment this number every time I run the code after changing it. I don't always though.

        private double frameRate = 60;

        internal Ruleset CurrentRuleset { get; set; }

        internal GridView view { get; private set; } // the primary view
        private GridView minimap; // the minimap

        internal Map Map { get; private set; } // the world seen by the view and minimap

        private Actions actions; // this sets up our user interaction.
        internal Configuration Configuration{get; private set;}

        internal ErrorSplash errorSplash = null;



        internal Menu currentMenu = null;

        internal Menu RulesetMenu { get; private set; }
        internal Menu SaveMenu { get; private set; }
        internal Menu LoadMenu { get; private set; }

        internal Menu InstructionsMenu { get; private set; }

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



            // create the basics.
            Engine = new Engine(TargetElapsedTime);

            // create the folder for saves, if it does not exist.
            if(!Directory.Exists("Saves"))
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
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            if(errorSplash != null)
            {
                return;
            }

            view.Destroy();
            view = Engine.addView(spriteBatch.GraphicsDevice, Map, 0, 0, width, height, Configuration.DefaultSkin);

            // prevent the camera from leaving the world.
            view.SetCameraBound(true);


            minimap.Destroy();
            SetupMinimap(width);

           

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // initialize fonts
            Textures.testFont = Content.Load<SpriteFont>("Consolas12");
            Textures.Consolas = new FontFamily(
                new SpriteFont[]{
                //Content.Load<SpriteFont>("Consolas6"),
               // Content.Load<SpriteFont>("Consolas12"),
                // Content.Load<SpriteFont>("Consolas18"),
               // Content.Load<SpriteFont>("Consolas24"),
                Content.Load<SpriteFont>("Consolas48"),
                //Content.Load<SpriteFont>("Consolas100"),
                Content.Load<SpriteFont>("Consolas200")
                });

            // textures.
            Textures.pixel = Content.Load<Texture2D>("pixel");
            Textures.tile = Content.Load<Texture2D>("tile");
            Textures.testSquare = Content.Load<Texture2D>("testSquare");
            Textures.viewboxBG = Content.Load<Texture2D>("ViewportBG");
            Textures.chunkGrid = Content.Load<Texture2D>("chunkGrid");
            Textures.altChunkGrid = Content.Load<Texture2D>("AltChunkGrid");
            Textures.sampleAgent = Content.Load<Texture2D>("SampleAgent");

            // agent textures
            Textures.emitter = Content.Load<Texture2D>("Agents/emitter");
            Textures.channel = Content.Load<Texture2D>("Agents/channel");
            Textures.luminalGate = Content.Load<Texture2D>("Agents/luminalGate");
            Textures.mirror = Content.Load<Texture2D>("Agents/mirror");
            Textures.notGate = Content.Load<Texture2D>("Agents/notgate");
            Textures.prism = Content.Load<Texture2D>("Agents/prism");
            Textures.stopper = Content.Load<Texture2D>("Agents/stopper");



            try
            {
                // setup the engine's configuration.
                Configuration = new Configuration(this);
                
            }
            catch (InitializationFailedException e)
            {
                errorSplash = new ErrorSplash(e.Message);
                Engine = null;
                return;
            }
            catch(Exception e)
            {
                errorSplash = new ErrorSplash("Crystalarium's engine unexpectedly crashed during initialization." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                Engine = null;
                return;
            }
       

            Engine.Sim.TargetStepsPS = 10; 

            // setup our interaction related code and register it with the engine.
            actions = new Actions(Engine.Controller, this);

            // Create menus ( this feels like a very ugly hack)
            // well, what is ui structure but a bunch of data definitions and hooks into actual code?
            // this will be expanded on in the future, I bet.

            // all of this is actually hideous
            // horrible
            // I despise looking at this


            RulesetMenu = new Menu("Switch Ruleset?",
                  "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",

                (int i) => { return "Press " + i + " to " + "switch to ruleset '" + Engine.Rulesets[i - 1].Name + "'."; },
                (int i) => { return false; },
                (int i) => { return Engine.Rulesets.Count < i; });

            SaveMenu = new Menu("Save Map",
                 "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",
                (int i) =>
                {
                    string path = Path.Combine("Saves", i + ".xml");
                    return "Press " + i + " to " + "save in slot " + i + " (" + (File.Exists(path) ? (new FileInfo(path).Length / 1024 + " KB).") : "Empty).");

                },
                (int i) => { return false; },
                 (int i) => { return false; }
                );


            LoadMenu = new Menu("Load Map",
                 "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",
                (int i) =>
                {
                    string path = Path.Combine("Saves", i + ".xml");
                    return "Press " + i + " to " + "load from slot " + i + " (" + (File.Exists(path) ? (new FileInfo(path).Length / 1024 + " KB).") : "Empty).");

                },
                (int i) => { return false; },
                (int i) => { return !File.Exists(Path.Combine("Saves", i + ".xml")); }
                );

            InstructionsMenu = new Menu("Controls",
                "Press " + Engine.Controller.GetAction("OpenRulesetMenu").FirstKeybindAsString() +
                " or " + Engine.Controller.GetAction("Close").FirstKeybindAsString() + " to return to game.",
             (int i) => // mostly this part, ew
             {
                 Controller c = Engine.Controller;
                 return "These are Crystalarium's Controls. They can be edited in Settings/Controls.xml." +

                 "\n\nCamera: Move: " + c.GetAction("CamUp").FirstKeybindAsString() + c.GetAction("CamLeft").FirstKeybindAsString()
                 + c.GetAction("CamDown").FirstKeybindAsString() + c.GetAction("CamRight").FirstKeybindAsString()
                 + ". Zoom: Scrollwheel. Pan: " + c.GetAction("Pan").FirstKeybindAsString()
                 + ". Toggle Debug View: " + c.GetAction("ToggleDebugView").FirstKeybindAsString() +

                 ".\nInteract: Place: " + c.GetAction("PlaceAgent").FirstKeybindAsString() + ". Remove: "
                 + c.GetAction("RemoveAgent").FirstKeybindAsString() + ". Rotate: " + c.GetAction("RotateAgent").FirstKeybindAsString() +
                 ".\nSelect Agents: Previous/Next Agent: " + c.GetAction("PrevAgent").FirstKeybindAsString() + ", " + c.GetAction("NextAgent").FirstKeybindAsString() +
                 ". Select: Number keys. Pipette: " + c.GetAction("Pipette").FirstKeybindAsString() + "." +

                 "\nSimulation: Pause/Unpause: " + c.GetAction("ToggleSim").FirstKeybindAsString() +
                 ". Single Step: " + c.GetAction("SimStep").FirstKeybindAsString() + ". Decrease/Increase Speed: " +
                 c.GetAction("DecreaseSimSpeed").FirstKeybindAsString() + ", " + c.GetAction("IncreaseSimSpeed").FirstKeybindAsString() +

                 ".\nOther: Switch Ruleset: " + c.GetAction("OpenRulesetMenu").FirstKeybindAsString() + ". Save: " + c.GetAction("Save").FirstKeybindAsString() +
                 ". Load: " + c.GetAction("Load").FirstKeybindAsString() + ".";


             },
             (int i) => { return i > 1; },
             (int i) => { return false; }
             );

            // create a test grid, and do some test things to it.
            Map = Engine.addGrid(CurrentRuleset);

            Map.OnReset += actions.OnMapReset;
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;



            // create a couple test viewports.
            view = Engine.addView(spriteBatch.GraphicsDevice, Map, 0, 0, width, height, Configuration.DefaultSkin);
            //view.Camera.MinScale = 1;
            // prevent the camera from leaving the world.
            view.SetCameraBound(true);


            // setup the minimap.
            SetupMinimap(width);
            

        }

        private void SetupMinimap(int width)
        {
            minimap = Engine.addView(spriteBatch.GraphicsDevice, Map, width - 250, 0, 250, 250, Configuration.MiniMapSkin);

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
                view.CreateGhost(actions.CurrentType, actions.GetMousePos(), actions.Rotation);
            }
            else
            {
                // stop the camera
                view.Camera.Velocity = new Vector3(0);
            }

            // minimap positions
                minimap.Camera.Position = view.Camera.Position;
            minimap.Camera.Zoom = view.Camera.Zoom;



            try
            {
                Engine.Update(gameTime);
            }
            catch (Exception e)
            {
                errorSplash = new ErrorSplash("Crystalarium's engine unexpectedly crashed while updating the simulation." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                Engine = null;
                return;
            }

            base.Update(gameTime);

          
        }


        protected override void Draw(GameTime gameTime)
        {

           
            frameRate += (((1 / gameTime.ElapsedGameTime.TotalSeconds) - frameRate) * 0.1);

            // setup
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            spriteBatch.Begin();

           
          

            if (errorSplash != null)
            {
                errorSplash.Draw(spriteBatch, GraphicsDevice);
                EndDraw(height);
                return;
            }

            // make everything a flat color.
            GraphicsDevice.Clear(new     Color(70, 70, 70));


            // tru to draw the game
            try
            {
                Engine.Draw(spriteBatch);
            }
            catch (Exception e)
            {
                errorSplash = new ErrorSplash("Crystalarium's engine unexpectedly crashed while rendering graphics." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                Engine = null;
                spriteBatch.End();
                return;
            }

            // Draw text on top of the game.

            DrawText(width, height, Math.Round(frameRate,1));

            if (Engine.Controller.Context == "menu")
            {
                DrawMenu(width, height);    
            }



            EndDraw(height);
            

           
            base.Draw(gameTime);

        

        }

        // draw info on top of the game.
        private void DrawText(int width, int height, double frameRate)
        {


            string info = "Hovering over: " + actions.GetMousePos().X + ", " + actions.GetMousePos().Y;
            if (Engine.Controller.Context == "menu")
            {
                info = "Hovering over: N/A, N/A";
            }

            string rules = "Ruleset: " + CurrentRuleset.Name;

            // some debug text. We'll clear this out sooner or later...
          
            Textures.Consolas.Draw(spriteBatch, "FPS: "+frameRate+" Sim Speed: "+Engine.Sim.ActualStepsPS + " Steps/Second Chunks: " 
                + Map.ChunkCount +" Agents: "+Map.AgentCount+" Connections: "+Map.ConnectionCount, 22,
                new Vector2(10, 10), Color.White);
            Textures.Consolas.Draw(spriteBatch, "Placing: " + actions.CurrentType.Name + " (facing " + actions.Rotation + ") \n" + info + "\n" + rules, 22, new Vector2(10, 30), Color.White);

       


            Textures.Consolas.Draw(spriteBatch,
                "Press "+Engine.Controller.GetAction("Instructions").FirstKeybindAsString()+" For instructions.", 
                22,
                new Vector2(10, height - 50), Color.White);

        }

        //draw the crude menu for switching rulesets.
        private void DrawMenu(int width, int height)
        {
            spriteBatch.Draw(Textures.pixel, new Rectangle(0, 0, width, height), new Color(0, 0, 0, 180));
            currentMenu.Draw(spriteBatch);

        }



      

        // draw the build number, the most important thing!
        private void EndDraw(int height)
        {
          
            Textures.Consolas.Draw(spriteBatch, "Milestone 7, Build " + BUILD, 22, new Vector2(10, height - 25), Color.White);
            spriteBatch.End();
        }


    }


   
}
 