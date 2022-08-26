using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using CrystalCore;
using CrystalCore.Input;
using CrystalCore.Util;
using CrystalCore.Model;
using CrystalCore.View;
using CrystalCore.Model.Rulesets;
using CrystalCore.View.Subviews.Agents;
using CrystalCore.Model.Communication;
using CrystalCore.View.Configs;

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
        private const bool debugMode = false; // if true, crystalarium will not handle errors in a user-friendly way. this can be helpful for debugging.

        private const int BUILD = 665; // I like to increment this number every time I run the code after changing it. I don't always though.



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
            Textures.sampleAgent = Content.Load<Texture2D>("SampleAgent");
            Textures.emitter = Content.Load<Texture2D>("Agents/emitter");






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
                if (debugMode) { throw; }
                return;
            }

            // setup our interaction related code and register it with the engine.
            actions = new Actions(Engine.Controller, this);


            // create a test grid, and do some test things to it.
            Grid = Engine.addGrid(CurrentRuleset);
            //g.ExpandGrid(Direction.right);


            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            // create the render modes we are likely to use.

            /*ChunkViewConfig Standard = new ChunkViewConfig()
            {
                ChunkBackground = Textures.chunkGrid
               
            };*/


            // create a couple test viewports.
            view = Engine.addView(Grid, 0, 0, width, height, Configuration.DefaultSkin);

            // prevent the camera from leaving the world.
            view.bindCamera();




            // setup the minimap.
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


            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (errorSplash != null)
            {
                return;
            }

            // this is temporary code, meant to demonstrate a viewport's capabilities.

            view.Camera.VelZ += Engine.Controller.DeltaScroll / 150f;
            view.Camera.ZoomOrigin = view.LocalizeCoords(Mouse.GetState().Position);

            // minimap positions
            minimap.Camera.Position = view.Camera.Position;
            minimap.Camera.Zoom = view.Camera.Zoom / 12;

            // create ghosts.
            view.CreateGhost(actions.CurrentType, actions.GetMousePos(), actions.Rotation);

            try
            {
                Engine.Update(gameTime);
            }
            catch (Exception e)
            {
                errorSplash = new ErrorSplash("Crystalarium's engine unexpectedly crashed while updating the simulation." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                if (debugMode) { throw; }
                return;
            }

            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {


            // arguably temporary
            double frameRate = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 2);

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

            try
            {
                Engine.Draw(spriteBatch);
            }
            catch (Exception e)
            {
                errorSplash = new ErrorSplash("Crystalarium's engine unexpectedly crashed while rendering graphics." +
                    "\nIt would really be a help if you could report this problem, so it can get fixed." +
                    "\nA detailed description of the problem is below:\n\n" + e.ToString());
                if (debugMode) { throw; }
                spriteBatch.End();
                return;
            }


            string info = "Hovering over: " + actions.GetMousePos().X + ", " + actions.GetMousePos().Y;
            string rules = "Ruleset: " + CurrentRuleset.Name;

            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(Textures.testFont, "FPS/SPS " + frameRate + "/" + Engine.Sim.ActualStepsPS + " Chunks: " + Grid.gridSize.X * Grid.gridSize.Y, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(Textures.testFont, "Placing: " + actions.CurrentType.Name + " (facing " + actions.Rotation + ") \n" + info + "\n" + rules, new Vector2(10, 30), Color.White);


            spriteBatch.DrawString(Textures.testFont, "WASD or MMB to pan. Scroll to zoom. UHJK to grow the map. LMB to place agent. P to switch rulesets (resets grid).\nRMB to delete. R to rotate. Q and E to switch agent types. O to toggle port rendering.", new Vector2(10, height - 70), Color.White);

            EndDraw(height);



            base.Draw(gameTime);
        }

        private void EndDraw(int height)
        {
            spriteBatch.DrawString(Textures.testFont, "Milestone 4, Build " + BUILD, new Vector2(10, height - 25), Color.White);
            spriteBatch.End();

        }


    }
}
