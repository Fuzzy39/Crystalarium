using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

using CrystalCore;
using CrystalCore.Input;
using CrystalCore.Util;
using CrystalCore.Sim;
using CrystalCore.View;
using CrystalCore.View.ChunkRender;
using CrystalCore.Rulesets;

namespace Crystalarium
{
    public class Crystalarium : Game
    {

        // Much of the code here is temporary, meant to demonstrate and test the systems being worked on.

        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        private CrystalCore.CrystalCore engine; // the 'engine'

        private const int BUILD = 461;


        // Temporary variables for testing purposes:

        // Temporary Content 
        private Ruleset testRules;
        private SpriteFont testFont;
        AgentType currentType;

        // Other
        GridView view; // the primary view
        GridView minimap; // the minimap

        Grid g; // the world seen by the view and minimap

        string info; // used when clicking for info

        Direction rotation = Direction.up;

        // used when panning
        Point panOrigin = new Point();
        Vector2 panPos = new Vector2();


        public Crystalarium()
        {

            _graphics = new GraphicsDeviceManager(this);



            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }




        protected override void Initialize()
        {
            //let's make the window a tiny bit bigger, for testing
            _graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            _graphics.ApplyChanges();


            // create a ruleset
            testRules = new Ruleset("test");


            // create the basics.
            engine = new CrystalCore.CrystalCore(TargetElapsedTime, testRules);

            // make a local reference to the input controller, since we use it a lot
            Controller c = engine.Controller;





            // make an action

            // test code.
            float camSpeed = 1.2f;

            // camera up
            c.addAction("up", () => view.Camera.AddVelocity(camSpeed, Direction.up));
            new Keybind(c, Keystate.Down, "up", Button.W);
            new Keybind(c, Keystate.Down, "up", Button.Up);


            // camera down
            c.addAction("down", () => view.Camera.AddVelocity(camSpeed, Direction.down));
            new Keybind(c, Keystate.Down, "down", Button.S);
            new Keybind(c, Keystate.Down, "down", Button.Down);

            // camera left
            c.addAction("left", () => view.Camera.AddVelocity(camSpeed, Direction.left));
            new Keybind(c, Keystate.Down, "left", Button.A);
            new Keybind(c, Keystate.Down, "left", Button.Left);

            // camera right
            c.addAction("right", () => view.Camera.AddVelocity(camSpeed, Direction.right));

            new Keybind(c, Keystate.Down, "right", Button.D);
            new Keybind(c, Keystate.Down, "right", Button.Right);


            // grow the grid!
            c.addAction("grow up", () => g.ExpandGrid(Direction.up));
            new Keybind(c, Keystate.OnPress, "grow up", Button.U);
            c.addAction("grow down", () => g.ExpandGrid(Direction.down));
            new Keybind(c, Keystate.OnPress, "grow down", Button.J);
            c.addAction("grow left", () => g.ExpandGrid(Direction.left));
            new Keybind(c, Keystate.OnPress, "grow left", Button.H);
            c.addAction("grow right", () => g.ExpandGrid(Direction.right));
            new Keybind(c, Keystate.OnPress, "grow right", Button.K);



            c.addAction("place agent", () =>
            {
                Point clickCoords = getMousePos();

                if (currentType.isValidLocation(g, clickCoords, rotation))
                {
                    currentType.createAgent(g, clickCoords, rotation);
                }


            });
            new Keybind(c, Keystate.Down, "place agent", Button.MouseLeft);


            c.addAction("remove agent", () =>
            {
                Point clickCoords = getMousePos();
                Agent toRemove = null;

                // remove all agents on this tile (there should only be one once things are working properly)
                while (true)
                {

                    toRemove = g.getAgentAtPos(clickCoords);
                    if (toRemove == null)
                    {
                        break;
                    }

                    toRemove.Destroy();
                }




            });
            new Keybind(c, Keystate.Down, "remove agent", Button.MouseRight);


            c.addAction("rotate", () =>
            {
                Point clickCoords = getMousePos();

                Agent a = g.getAgentAtPos(clickCoords);
                if (a == null)
                {
                    rotation = rotation.Rotate(RotationalDirection.clockwise);
                    return;
                }


                a.Rotate(RotationalDirection.clockwise);





            });
            new Keybind(c, Keystate.OnPress, "rotate", Button.R);



            c.addAction("start pan", () =>
            {
                Point pixelCoords = view.LocalizeCoords(Mouse.GetState().Position);


                panOrigin = pixelCoords;
                panPos = view.Camera.Position;
        

            });
            new Keybind(c, Keystate.OnPress, "start pan", Button.MouseMiddle) { DisableOnSuperset = false };


            c.addAction("pan", () =>
            {


                Point pixelCoords = view.LocalizeCoords(Mouse.GetState().Position);
                Vector2 mousePos = view.Camera.PixelToTileCoords(pixelCoords);
                Vector2 originPos = view.Camera.PixelToTileCoords(panOrigin);

                view.Camera.Position = panPos + (originPos - mousePos);
           



            });
            new Keybind(c, Keystate.Down, "pan", Button.MouseMiddle) { DisableOnSuperset = false };


            c.addAction("next agent", () =>
            {
                List<AgentType> types = testRules.AgentTypes;

                int i = types.IndexOf(currentType);

                i++;
                if (i >= types.Count)
                {
                    i = 0;
                }
                
                currentType = types[i];

            });
            new Keybind(c, Keystate.OnPress, "next agent", Button.E);

            c.addAction("prev agent", () =>
            {
                List<AgentType> types = testRules.AgentTypes;

                int i = types.IndexOf(currentType);

                i--;
                if (i < 0)
                {
                    i = types.Count-1;
                }

                currentType = types[i];

            });
            new Keybind(c, Keystate.OnPress, "prev agent", Button.Q);


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


            // define ruleset.


            AgentType t;

            t=testRules.CreateType("big", new Point(2, 2));
            t.RenderConfig.AgentBackground = Textures.sampleAgent;
            t.RenderConfig.BackgroundColor = Color.DodgerBlue;

            t = testRules.CreateType("small", new Point(1, 1));
            t.RenderConfig.AgentBackground = Textures.sampleAgent;
            t.RenderConfig.BackgroundColor = Color.Crimson;

            currentType = t;

            t = testRules.CreateType("flat", new Point(2, 1));
            t.RenderConfig.AgentBackground = Textures.sampleAgent;
            t.RenderConfig.BackgroundColor = Color.Gold;

            t = testRules.CreateType("tall", new Point(1, 2));
            t.RenderConfig.AgentBackground = Textures.sampleAgent;
            t.RenderConfig.BackgroundColor = Color.LimeGreen;



            // create a test grid, and do some test things to it.
            g = engine.addGrid();
            //g.ExpandGrid(Direction.right);

            // testing purposes.
            //box.createAgent(g, new Point(7, 7), Direction.up);




            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            // create the render modes we are likely to use.

            ChunkViewTemplate Standard = new ChunkViewTemplate()
            {
                ChunkBackground = Textures.chunkGrid,

            };


            // create a couple test viewports.
            view = engine.addView(g, 0, 0, width, height, Standard);

            // background
            view.Background = Textures.viewboxBG;

            // prevent the camera from leaving the world.
            view.bindCamera();
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
            minimap = engine.addView(g, width - 250, 0, 250, 250, Debug);

            // background
            minimap.Background = Textures.viewboxBG;

            // setup borders
            minimap.Border.SetTextures(Textures.pixel, Textures.pixel);
            minimap.Border.Width = 2;


            // Set the camera of the minimap.
            minimap.Camera.MinScale = 1;

            minimap.DoAgentRendering = false;





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

        
            info = "Hovering over: " + getMousePos().X + ", " + getMousePos().Y;

            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(testFont, "FPS/SPS " + frameRate + "/" + engine.Sim.ActualStepsPS + " Chunks: " + g.gridSize.X * g.gridSize.Y, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(testFont, "Placing: "+currentType.Name+" (facing " + rotation + ") \n" + info, new Vector2(10, 30), Color.White);

            spriteBatch.DrawString(testFont, "Milestone 3, Build " + BUILD, new Vector2(10, height - 25), Color.White);
            spriteBatch.DrawString(testFont, "WASD or MMB to pan. Scroll to zoom. UHJK to grow the map. LMB to place agent.\nRMB to delete. R to rotate. Q and E to switch agent types.", new Vector2(10, height - 70), Color.White);


            spriteBatch.End();


            base.Draw(gameTime);
        }

        private Point getMousePos()
        {
            Point pixelCoords = view.LocalizeCoords(Mouse.GetState().Position);

            Vector2 clickCoords = view.Camera.PixelToTileCoords(pixelCoords);

            clickCoords.Floor();
            return clickCoords.ToPoint();

        }
    }
}
