using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Crystalarium.Sim;
using Crystalarium.Render;
using Crystalarium.Util;
using System.Collections.Generic;
using Crystalarium.Input;

namespace Crystalarium
{
    public class Crystalarium : Game
    {

        // Much of the code here is temporary, meant to demonstrate and test the systems being worked on.

        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private SimulationManager sim;
        private List<GridView> viewports;
        private Controller controller;



        private const int BUILD = 267;


        // Content (should maybe move this eventually?)
        private SpriteFont testFont;

        // TEST
        GridView view;
        GridView minimap;

        Grid g;    
       

        public Crystalarium()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

       
       

        protected override void Initialize()
        {
            
            // create the basics.
            sim = new SimulationManager(this.TargetElapsedTime.TotalSeconds);
            viewports = new List<GridView>();

            // test the controller.
            controller = new Controller();

            // make an action

            // shitty test code.
            float camSpeed = 1f;
            
            // camera up
            controller.addAction("up", ()=>view.Camera.AddVelocity(camSpeed, Direction.up));
            new Keybind(controller, Keystate.Down, "up", Button.W);
            new Keybind(controller, Keystate.Down, "up", Button.Up);


            // camera down
            controller.addAction("down", () => view.Camera.AddVelocity(camSpeed, Direction.down));
            new Keybind(controller, Keystate.Down, "down", Button.S);
            new Keybind(controller, Keystate.Down, "down", Button.Down);

            // camera left
            controller.addAction("left", () => view.Camera.AddVelocity(camSpeed, Direction.left));
            new Keybind(controller, Keystate.Down, "left", Button.A);
            new Keybind(controller, Keystate.Down, "left", Button.Left);

            // camera right
            controller.addAction("right", () => view.Camera.AddVelocity(camSpeed, Direction.right));

            new Keybind(controller, Keystate.Down, "right", Button.D);
            new Keybind(controller, Keystate.Down, "right", Button.Right);


            // grow the grid!
            controller.addAction("grow up", () => g.ExpandGrid(Direction.up));
            new Keybind(controller, Keystate.OnPress, "grow up", Button.U);
  


            controller.addAction("grow down", () => g.ExpandGrid(Direction.down));
            new Keybind(controller, Keystate.OnPress, "grow down", Button.J);


            // camera left
            controller.addAction("grow left", () => g.ExpandGrid(Direction.left));
            new Keybind(controller, Keystate.OnPress, "grow left", Button.H);
          

            // camera right
            controller.addAction("grow right", () => g.ExpandGrid(Direction.right));

            new Keybind(controller, Keystate.OnPress, "grow right", Button.K);
      



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


            // create a test grid, and do some test things to it.
            g = new Grid(sim);

            // make it a size or something.
           /* g.ExpandGrid(Direction.right);
            g.ExpandGrid(Direction.left);
            g.ExpandGrid(Direction.left);

            g.ExpandGrid(Direction.up);
            g.ExpandGrid(Direction.up);
            g.ExpandGrid(Direction.down);*/





            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            // create a couple test viewports.
            view = new GridView(viewports, g, 0, 0, width, height);
            //view.RendererType = Render.ChunkRender.Type.Default;
         


            // setup the minimap.

            minimap = new GridView(viewports, g, width-250, 0, 250, 250);

            // setup borders
            minimap.SetTextures(Textures.pixel, Textures.pixel);
            minimap.Border.Width = 2;

            // Set the render mode to debug.
            minimap.RendererType = Render.ChunkRender.Type.Debug;
            minimap.SetDebugRenderTarget(view);

            // Set the camera of the minimap.
            minimap.Camera.MinScale = 1;
            
           
       


        }

        // mostly ugly hacks
        protected override void Update(GameTime gameTime)
        {

            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


          
            sim.Update(gameTime);

            // update viewport.

            // update viewports.
            foreach (GridView v in viewports)
            {
                v.Update();
            }




            // this is temporary code, meant to demonstrate a viewport's capabilities.

            view.Camera.VelZ += controller.DeltaScroll/200f;
            view.Camera.ZoomOrigin = view.LocalizeCoords(Mouse.GetState().Position);

            // minimap positions
            minimap.Camera.Position = view.Camera.Position;
            minimap.Camera.Scale = view.Camera.Scale/12;


     
            controller.Update();
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {

          
            // arguably temporary
            double frameRate = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds,2);

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            spriteBatch.Begin();

            // make everything a flat color.
            GraphicsDevice.Clear(new Color(70,70,70));

            // draw viewports
            foreach(GridView v in viewports)
            {
                v.Draw(spriteBatch);
            }



            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(testFont, "Milestone 1, Build " + BUILD, new Vector2(10, height - 25), Color.White);
            spriteBatch.DrawString(testFont, "WASD to pan. Scroll to zoom. UHJK to E X P A N D", new Vector2(10, height-45), Color.White);
            spriteBatch.DrawString(testFont, "FPS/SPS " + frameRate + "/" + sim.ActualStepsPS +" Chunks: "+g.gridSize.X*g.gridSize.Y, new Vector2(10, 10), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
