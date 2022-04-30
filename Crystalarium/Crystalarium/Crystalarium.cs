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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private SimulationManager sim;
        private List<Viewbox> viewports;
        

        private const int BUILD = 126;

        // Content (should maybe move this eventually?)
        private SpriteFont testFont;

        // TEST
        Viewbox v;
        Viewbox w;
        Grid g;
        double i= 0;
        double j = 0;

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
            viewports = new List<Viewbox>();

          
           

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


            // create a test grid, and do some test things to it.
            g = new Grid(sim);


            g.ExpandGrid(Direction.left);
           // g.ExpandGrid(Direction.down);
            g.ExpandGrid(Direction.up);
            g.ExpandGrid(Direction.right);
            g.ExpandGrid(Direction.left);
           
           
            g.DebugReport();

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            // create a couple test viewports.
            v = new Viewbox(viewports, g, 0, 0, width, height);
           
          

            w = new Viewbox(viewports, g, width-300, 0, 300, 900/4);
            w.SetTextures(Textures.pixel, Textures.pixel);
            w.Border.Width = 2;
            w.MinScale = 1;
            w.Scale = 4; 



        }

        protected override void Update(GameTime gameTime)
        {

            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            sim.Update(gameTime);

            // this is temporary code, meant to demonstrate a viewport's capabilities.
            

            i += .025; // i is a counter for the viewport's position.
            j += .005; // j is a counter for the viewport's zoom.
            float loopSize = 5f; // the size, in tiles, of the loop the viewport will travel.

            // position goes around in a circle while the viewport is slowly zoomed in and out.

            Vector2 pos = new Vector2();
            pos.X = (float)(Math.Sin(i) * loopSize );
            pos.Y = (float)(Math.Cos(i)*loopSize);

            // set viewport values.
            v.Scale = v.MinScale + (Math.Sin(j)+1) * ((v.MaxScale - v.MinScale))/2;
            v.Position = pos;

            /*pos = new Vector2();
            pos.X = (float)(Math.Sin(i) * loopSize);
            pos.Y = (float)(Math.Cos(i) * loopSize);*/

            // set W viewport values
            
            w.Position = pos;


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
            foreach(Viewbox v in viewports)
            {
                v.Draw(spriteBatch);
            }



            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(testFont, "Milestone 0, Build " + BUILD, new Vector2(10, height - 25), Color.White); 
            spriteBatch.DrawString(testFont, "FPS/SPS " + frameRate + "/" + sim.ActualStepsPS, new Vector2(10, 10), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
