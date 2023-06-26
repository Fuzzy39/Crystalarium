using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CrystalCrash.Main
{
    public class CrashHandler : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static SpriteFont Consolas { get; private set; }

        private ErrorSplash error;
        private string errorMessage;
       

        public CrashHandler(string errorMessage)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.errorMessage = errorMessage;
        }
            
        protected override void Initialize()
        {
            
            _graphics.PreferredBackBufferWidth = 1280;  
            _graphics.PreferredBackBufferHeight = 720;   
            _graphics.ApplyChanges();

            base.Initialize();

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            
        }

        public void OnResize(object sender, EventArgs e)
        {
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            _graphics.ApplyChanges();
        }



        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Consolas = Content.Load<SpriteFont>("Consolas");

            error = new ErrorSplash(errorMessage, _spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
          

            // TODO: Add your drawing code here
            error.Draw(_graphics.GraphicsDevice);
            base.Draw(gameTime);
        }
    }
}