using CrystalCore;
using CrystalCore.Model.Grids;
using CrystalCore.Model.Rulesets;
using CrystalCore.Util.Timekeeping;
using CrystalCore.View;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        private const int BUILD = 783; // I like to increment this number every time I run the code after changing it. I don't always though.

        

        internal Ruleset CurrentRuleset { get; set; }




        internal GridView view { get; private set; } // the primary view
        private GridView minimap; // the minimap

        internal Grid Grid { get; private set; } // the world seen by the view and minimap

        private Actions actions; // this sets up our user interaction.
        internal Configuration Configuration{get; private set;}

        private ErrorSplash errorSplash = null;

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




            // create the basics.
            Engine = new Engine(TargetElapsedTime);



            base.Initialize();

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // initialize fonts
            Textures.testFont = Content.Load<SpriteFont>("testFont");

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
                errorSplash = new ErrorSplash(e.Message+"\n\nIf you are seing this in Milestone 5, it means something has gone deeply wrong.\nThis shouldn't be possible- If you can't even open the game, I've clearly made a mistake.");
                return;
            }
            catch(Exception e)
            {
                errorSplash = new ErrorSplash("Crystalarium's engine unexpectedly crashed during initialization." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                return;
            }

            Engine.Sim.TargetStepsPS = 10; 

            // setup our interaction related code and register it with the engine.
            actions = new Actions(Engine.Controller, this);


            // create a test grid, and do some test things to it.
            Grid = Engine.addGrid(CurrentRuleset);


            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;



            // create a couple test viewports.
            view = Engine.addView(Grid, 0, 0, width, height, Configuration.DefaultSkin);

            // prevent the camera from leaving the world.
            view.bindCamera();


            // setup the minimap.
            SetupMinimap(width);
            

        }

        private void SetupMinimap(int width)
        {
            minimap = Engine.addView(Grid, width - 250, 0, 250, 250, Configuration.MiniMapSkin);

            // setup borders
            minimap.Border.SetTextures(Textures.pixel, Textures.pixel);
            minimap.Border.Width = 2;

            // Set the camera of the minimap.
            minimap.Camera.MinScale = 1;

            // to make it a minimap!
            minimap.ViewCastTarget = view; // note that this must be done after view has been initialized.
            //minimap.DoAgentRendering = false;
        }


        // mostly ugly hacks
        protected override void Update(GameTime gameTime)
        {


            Timekeeper.Instance.StartTask("Other Update");

            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
            minimap.Camera.Zoom = view.Camera.Zoom / 12;


            Timekeeper.Instance.StopTask("Other Update");

            try
            {
                Engine.Update(gameTime);
            }
            catch (Exception e)
            {
                errorSplash = new ErrorSplash("Crystalarium's engine unexpectedly crashed while updating the simulation." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                return;
            }

            base.Update(gameTime);

          
        }

        protected override void Draw(GameTime gameTime)
        {

            Timekeeper.Instance.StartTask("Other Draw");
          

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
            GraphicsDevice.Clear(new Color(70, 70, 70));

            Timekeeper.Instance.StopTask("Other Draw");

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
                spriteBatch.End();
                return;
            }

            Timekeeper.Instance.StartTask("Other Draw");
            // Draw text on top of the game.
            DrawText(width, height);

            if (Engine.Controller.Context == "menu")
            {
                DrawMenu(width, height);    
            }

            EndDraw(height);


            Timekeeper.Instance.StopTask("Other Draw");
            base.Draw(gameTime);

        

        }

        // draw info on top of the game.
        private void DrawText(int width, int height)
        {


            string info = "Hovering over: " + actions.GetMousePos().X + ", " + actions.GetMousePos().Y;
            if (Engine.Controller.Context == "menu")
            {
                info = "Hovering over: N/A, N/A";
            }

            string rules = "Ruleset: " + CurrentRuleset.Name;

            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(Textures.testFont, "Sim Speed: "+Engine.Sim.ActualStepsPS + " Steps/Second Chunks: " 
                + Grid.gridSize.X * Grid.gridSize.Y+" Agents: "+Grid.AgentCount+" Signals: "+Grid.SignalCount,
                new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(Textures.testFont, "Placing: " + actions.CurrentType.Name + " (facing " + actions.Rotation + ") \n" + info + "\n" + rules, new Vector2(10, 30), Color.White);

            // diag info
            if (actions.TimingInfoEnabled)
            {
                spriteBatch.DrawString(Textures.testFont, Timekeeper.Instance.CreateReport(), new Vector2(10, 100), Color.White, 0f, new Vector2(), .8f, SpriteEffects.None, 0);
            }


            spriteBatch.DrawString(Textures.testFont, 
                "WASD or MMB to pan. Scroll to zoom. UHJK to grow the map. LMB to place agent. RMB to delete. R to rotate. Tab to Copy Agent." +
                "\nQ and E to switch agent types. P to switch rulesets (resets grid). O to toggle port rendering. I to toggle performance info." +
                "\nSpace to toggle simulation. Z for single sim step. Shift/Control to Raise/Lower sim speed.", 
                new Vector2(10, height - 95), Color.White);

        }

        //draw the crude menu for switching rulesets.
        private void DrawMenu(int width, int height)
        {
            spriteBatch.Draw(Textures.pixel, new Rectangle(0, 0, width, height), new Color(0, 0, 0, 180));
            spriteBatch.DrawString(Textures.testFont, "Switch Ruleset?", new Vector2(100, 100), Color.White, 0f, new Vector2(), 1.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Textures.testFont, "Press P to return to game.", new Vector2(120, 150), Color.White);

            for (int i = 1; i <= 9; i++)
            {
                if (Engine.Rulesets.Count < i)
                {
                    break;
                }
                spriteBatch.DrawString(Textures.testFont, "Press " + i + " to switch to ruleset '" + Engine.Rulesets[i - 1].Name + "'.", new Vector2(120, 150 + (25 * i)), Color.White);
            }
        }

        // draw the build number, the most important thing!
        private void EndDraw(int height)
        {
            spriteBatch.DrawString(Textures.testFont, "Milestone 5, Build " + BUILD, new Vector2(10, height - 25), Color.White);
            spriteBatch.End();

        }


    }
}
