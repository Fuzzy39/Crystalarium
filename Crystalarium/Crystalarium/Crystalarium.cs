using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Crystalarium.Sim;
using Crystalarium.Render;
using System.Collections.Generic;

namespace Crystalarium
{
    public class Crystalarium : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private SimulationManager sim;
        private List<Render.Viewport> viewports;
        

        private const int BUILD = 73;

        // Content (should maybe move this eventually?)
        private SpriteFont testFont;
        private Texture2D pixel;
        private Texture2D tile;
        private Texture2D viewportBG;

        // TEST
        Sim.Viewport v;
        Sim.Viewport w;
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
            viewports = new List<Sim.Viewport>();

          
           

            base.Initialize();

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // initialize fonts
            testFont = Content.Load<SpriteFont>("testFont");
            pixel = Content.Load<Texture2D>("pixel");
            tile = Content.Load<Texture2D>("tile");
            viewportBG = Content.Load<Texture2D>("ViewportBG");

            // create a couple test viewports.
            v = new Sim.Viewport(viewports, new Grid(sim), 80, 100, 300, 300);
            v.setTextures(viewportBG, pixel, pixel);
            v.BorderWidth = 2;

            w = new Sim.Viewport(viewports, new Grid(sim), 420, 100, 300, 300);
            w.setTextures(viewportBG, pixel, pixel);
            w.BorderWidth = 2;


            // this line sets up a demo later. This field of Viewport is temporary, and for testing only.
            v.testTexture = tile;
            w.testTexture = tile;

        }

        protected override void Update(GameTime gameTime)
        {

            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            sim.Update(gameTime);

            // this is temporary code, meant to demonstrate a viewport's capabilities.
            

            i += .025; // i is a counter for the viewport's position.
            j += .01; // j is a counter for the viewport's zoom.
            float loopSize = 3f; // the size, in tiles, of the loop the viewport will travel.

            // position goes around in a circle while the viewport is slowly zoomed in and out.

            Vector2 pos = new Vector2();
            pos.X = (float)(Math.Sin(-i) * loopSize );
            pos.Y = (float)(Math.Cos(-i)*loopSize);

            // set viewport values.
            v.Scale = v.MinScale + (Math.Sin(-j)+1) * ((v.MaxScale - v.MinScale))/2;
            v.Position = pos;

            pos = new Vector2();
            pos.X = (float)(Math.Sin(i) * loopSize);
            pos.Y = (float)(Math.Cos(i) * loopSize);

            // set W viewport values
            w.Scale = w.MinScale + (Math.Sin(j) + 1) * ((w.MaxScale - w.MinScale)) / 2;
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
            foreach(Sim.Viewport v in viewports)
            {
                v.draw(spriteBatch);
            }



            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(testFont, "Milestone 0, Build " + BUILD, new Vector2(10, height - 25), Color.White); 
            spriteBatch.DrawString(testFont, "FPS/SPS " + frameRate + "/" + sim.ActualStepsPS, new Vector2(10, 10), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
