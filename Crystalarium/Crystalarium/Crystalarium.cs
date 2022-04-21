using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Crystalarium
{
    public class Crystalarium : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        int elapsed = 0;
        int updateCycles = 0;
        int drawCycles = 0;
        public Crystalarium()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // System.Console.WriteLine(gameTime.TotalGameTime.TotalSeconds);
            // TODO: Add your update logic here
            updateCycles++;
            if((int)gameTime.TotalGameTime.TotalSeconds>elapsed)
            {
                elapsed++;
                System.Console.WriteLine(updateCycles+" update cycles and "+drawCycles+"draw cycles");
                updateCycles = 0;
                drawCycles = 0;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            drawCycles++;
            base.Draw(gameTime);
        }
    }
}
