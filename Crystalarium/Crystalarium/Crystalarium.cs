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
using CrystalCore.View.ChunkRender;
using CrystalCore.Rulesets;
using CrystalCore.View.AgentRender;
using CrystalCore.Model.Communication;

namespace Crystalarium
{
    public class Crystalarium : Game
    {

        /*
         * Welcome to Crystalarium! This is the primary file of the game.
         * At the moment, much of it is 'test code' and liable to some big changes, and maybe a complete rewrite or two down the road.
         * Don't expect this code to be super pretty just yet. The bulk of the systems that are more built up are in CrystalCore, the 'engine' of the game.
         * 
         */

        private GraphicsDeviceManager _graphics; 
        private SpriteBatch spriteBatch;

        private CrystalCore.Engine engine; // the 'engine'

        private const int BUILD = 623; // I like to increment this number every time I run the code after changing it. I don't always though.


        // Temporary variables for testing purposes:



        private SpriteFont testFont; // arial I think
        

        // Other
        internal GridView view { get; private set; } // the primary view
        private GridView minimap; // the minimap

        internal Grid Grid { get; private set; } // the world seen by the view and minimap

        private Actions actions; // this sets up our user interaction.


        public Crystalarium()
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
            engine = new Engine(TargetElapsedTime);

          

            base.Initialize();

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // initialize fonts
            testFont = Content.Load<SpriteFont>("testFont");

            // textures.
            Textures.pixel = Content.Load<Texture2D>("pixel");
            Textures.tile = Content.Load<Texture2D>("tile");
            Textures.testSquare = Content.Load<Texture2D>("testSquare");
            Textures.viewboxBG = Content.Load<Texture2D>("ViewportBG");
            Textures.chunkGrid = Content.Load<Texture2D>("chunkGrid");
            Textures.sampleAgent = Content.Load<Texture2D>("SampleAgent");


            // setup our interaction related code and register it with the engine.
            actions = new Actions(engine.Controller, this);


            // create a test grid, and do some test things to it.
            Grid = engine.addGrid(actions.CurrentRuleset);
            //g.ExpandGrid(Direction.right);
           

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            // create the render modes we are likely to use.

            ChunkViewTemplate Standard = new ChunkViewTemplate()
            {
                ChunkBackground = Textures.chunkGrid
               
            };


            // create a couple test viewports.
            view = engine.addView(Grid, 0, 0, width, height, Standard);

            // background
            view.Background = Textures.viewboxBG;

            // prevent the camera from leaving the world.
            view.bindCamera();
            //view.DoDebugPortRendering = true;
            //view.Camera.Position = g.center;


            // for the minimap.
            ChunkViewTemplate Debug = new ChunkViewTemplate()
            {

                ChunkBackground = Textures.pixel,
                DoCheckerBoardColoring = true,
                BackgroundColor = new Color(50, 50, 150),
                OriginChunkColor = new Color(150, 50, 50),

                // to make it a minimap!
                ViewCastOverlay = Textures.pixel,
                ViewCastTarget = view // note that this must be done after view has been initialized.
            
            };

            // setup the minimap.
            minimap = engine.addView(Grid, width - 250, 0, 250, 250, Debug);

            // background
            minimap.Background = Textures.viewboxBG;

            // setup borders
            minimap.Border.SetTextures(Textures.pixel, Textures.pixel);
            minimap.Border.Width = 2;


            // Set the camera of the minimap.
            minimap.Camera.MinScale = 1;

            //minimap.DoAgentRendering = false;

        }

        // mostly ugly hacks
        protected override void Update(GameTime gameTime)
        {


            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            

            // this is temporary code, meant to demonstrate a viewport's capabilities.

            view.Camera.VelZ += engine.Controller.DeltaScroll / 150f;
            view.Camera.ZoomOrigin = view.LocalizeCoords(Mouse.GetState().Position);

            // minimap positions
            minimap.Camera.Position = view.Camera.Position;
            minimap.Camera.Zoom = view.Camera.Zoom / 12;

            // create ghosts.
            actions.CurrentType.createGhost(view, actions.GetMousePos(), actions.Rotation);


            engine.Update(gameTime);
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {


            // arguably temporary
            double frameRate = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 2);

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            spriteBatch.Begin();

            // make everything a flat color.
            GraphicsDevice.Clear(new Color(70, 70, 70));

            engine.Draw(spriteBatch);

        
            string info = "Hovering over: " + actions.GetMousePos().X + ", " + actions.GetMousePos().Y;
            string rules = "Ruleset: " + actions.CurrentRuleset.Name;

            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(testFont, "FPS/SPS " + frameRate + "/" + engine.Sim.ActualStepsPS + " Chunks: " + Grid.gridSize.X * Grid.gridSize.Y, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(testFont, "Placing: "+actions.CurrentType.Name+" (facing " + actions.Rotation + ") \n" + info+"\n"+rules, new Vector2(10, 30), Color.White);

            spriteBatch.DrawString(testFont, "Milestone 4, Build " + BUILD, new Vector2(10, height - 25), Color.White);
            spriteBatch.DrawString(testFont, "WASD or MMB to pan. Scroll to zoom. UHJK to grow the map. LMB to place agent. P to switch rulesets (resets grid).\nRMB to delete. R to rotate. Q and E to switch agent types. O to toggle port rendering.", new Vector2(10, height - 70), Color.White);


            spriteBatch.End();


            base.Draw(gameTime);
        }

       
    }
}
