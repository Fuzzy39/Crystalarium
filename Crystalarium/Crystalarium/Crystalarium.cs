using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Crystalarium
{
    public class Crystalarium : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private SimulationManager simulationManager;

        private const int BUILD = 13;

        // Content (should maybe move this eventually?)
        private SpriteFont testFont;



        public Crystalarium()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

       


        protected override void Initialize()
        {
            
            // create the simulation manager.
            simulationManager = new SimulationManager(this.TargetElapsedTime.TotalSeconds);
            simulationManager.TargetStepsPS = 240; // completely arbitrary.

           

            base.Initialize();

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // initialize fonts
            testFont = Content.Load<SpriteFont>("testFont");

        }

        protected override void Update(GameTime gameTime)
        {

            // provided by monogame. Escape closes the program. I suppose it can stay for now.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            simulationManager.Update(gameTime);



            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {

            // arguably temporary
            double frameRate = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds,2);

            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            spriteBatch.Begin();

            // make everything a standard monogame blue.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // some debug text. We'll clear this out sooner or later...

            spriteBatch.DrawString(testFont, "Milestone 0, Build " + BUILD, new Vector2(10, height - 25), Color.White); ; ;
            spriteBatch.DrawString(testFont, "FPS/SPS " + frameRate + "/" + simulationManager.ActualStepsPS, new Vector2(10, 10), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
