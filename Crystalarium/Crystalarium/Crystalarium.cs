using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Crystalarium.Sim;
using Crystalarium.Render;
using Crystalarium.Util;
using System.Collections.Generic;

namespace Crystalarium
{
    public class Crystalarium : Game
    {

        // Much of the code here is temporary, meant to demonstrate and test the systems being worked on.

        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private SimulationManager sim;
        private List<GridView> viewports;


        private const int BUILD = 200;

        // Content (should maybe move this eventually?)
        private SpriteFont testFont;

        // TEST
        GridView view;
        GridView minimap;

        Grid g;
        double i= 0;
        double j = 0;
        int mode;
        bool pressed;
        // temporary.
        public static int frames = 0;

        public Crystalarium()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

    


        }

       


        protected override void Initialize()
        {
            
            // create the simulation manager.
            sim = new SimulationManager(this.TargetElapsedTime.TotalSeconds);
            sim.TargetStepsPS = 240; // completely arbitrary.

            // and viewports
            viewports = new List<GridView>();

            //TargetElapsedTime = TimeSpan.FromSeconds(1 / 2.0f);


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
     
    
           
            g.ExpandGrid(Direction.right);
            g.ExpandGrid(Direction.left);
            g.ExpandGrid(Direction.left);

            g.ExpandGrid(Direction.up);
            g.ExpandGrid(Direction.up);
            g.ExpandGrid(Direction.down);
 


            g.DebugReport();

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            // create a couple test viewports.
            view = new GridView(viewports, g, 0, 0, width, height);
            //v.RendererType = Render.ChunkRender.Type.Default;
          

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
            minimap.Camera.Scale = 3;
            minimap.Camera.Position = new Vector2(0, 0);
       


        }

        // mostly ugly hacks
        protected override void Update(GameTime gameTime)
        {

            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                pressed = true;
            }

            if (pressed & !Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                pressed = false;
                mode++;
                if (mode > 1)
                    mode = 0;

                switch (mode)
                {
                    case 0:
                        g.Reset();
                        


                        g.ExpandGrid(Direction.right);
                        g.ExpandGrid(Direction.left);
                        g.ExpandGrid(Direction.left);

                        g.ExpandGrid(Direction.up);
                        g.ExpandGrid(Direction.up);
                        g.ExpandGrid(Direction.down);
                        break;
                    case 1:
                        g.Reset();
                        g.ExpandGrid(Direction.up);
                        g.ExpandGrid(Direction.left);

                        g.ExpandGrid(Direction.right);
                        g.ExpandGrid(Direction.right);
                        g.ExpandGrid(Direction.right);
                        g.ExpandGrid(Direction.right);
                        g.ExpandGrid(Direction.right);
                        g.ExpandGrid(Direction.right);

                        g.DebugReport();
                        break;
                }
            }

            

            sim.Update(gameTime);

            // this is temporary code, meant to demonstrate a viewport's capabilities.



            i += 0.008; // i is a counter for the viewport's position.

            j += .005; // j is a counter for the viewport's zoom.

            Vector2 pos = new Vector2();
            double scale = 0;

            switch (mode)
            {
                case 0:
                    float loopSize = 20f; // the size, in tiles, of the loop the viewport will travel.

                    // position goes around in a circle while the viewport is slowly zoomed in and out.

                   
                    pos.X = (float)(Math.Sin(i) * loopSize);
                    pos.Y = (float)(Math.Cos(i) * loopSize);

                    // set viewport values.
                    scale = view.Camera.MinScale + (Math.Sin(j) + 1) * ((view.Camera.MaxScale - view.Camera.MinScale)) * .5;
                  
                    break;
                case 1:
                    pos.X = (float)(Math.Sin(i) * 8f* 6f)+48;
                    pos.Y =0;

                    // doesn't make a ton of sense, but whatever
                    scale = view.Camera.MinScale + (2-(Math.Sin(2*i) +1)) * ((view.Camera.MaxScale - view.Camera.MinScale)) * .5;
                 
                    break;
            }

            // main camera
            view.Camera.Scale = scale;
            view.Camera.Position = pos;

            // minimap positions
            minimap.Camera.Position = pos;
            minimap.Camera.Scale = scale/12;
            
           


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
            spriteBatch.DrawString(testFont, "Demo #"+mode+". Press enter to switch demos.", new Vector2(10, height-45), Color.White);
            spriteBatch.DrawString(testFont, "FPS/SPS " + frameRate + "/" + sim.ActualStepsPS, new Vector2(10, 10), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
