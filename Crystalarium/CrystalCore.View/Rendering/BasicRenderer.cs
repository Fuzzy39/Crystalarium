using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Rendering
{
    /// <summary>
    /// A Basic Renderer accomplishes the tasks of a renderer with a spritebatch.
    /// No bells and whistles. Just adds a layer of abstraction above the spritebatch.
    /// </summary>
    public class BasicRenderer : IBatchRenderer
    {

        private GraphicsDevice gd;
        private SpriteBatch spriteBatch;


        private bool hasDrawntoPrimary = false;
        protected bool hasTarget = false;


        public Vector2 Size
        {
            get
            {
                return gd.Viewport.Bounds.Size.ToVector2();
            }
        }

        public BasicRenderer(GraphicsDevice gd)
        {
            this.gd = gd;
            spriteBatch = new SpriteBatch(gd);



        }

        public virtual void Draw(Texture2D texture, RotatedRect destination, Rectangle source, Color color)
        {




            spriteBatch.Draw(
                texture,
                destination.AsRectangle,
                source,
                color,
                destination.Rotation,
                new(0),
                SpriteEffects.None, 0f
            );

            if (!hasTarget)
            {
                hasDrawntoPrimary = true;
            }

        }

        public virtual void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            font.Draw(spriteBatch, text, height, position, color);
        }

        void IBatchRenderer.Begin()
        {
            if (hasTarget)
            {
                throw new InvalidOperationException("Has a render target.");
            }
            spriteBatch.Begin();
        }

        public virtual void End()
        {
            if (hasTarget)
            {
                throw new InvalidOperationException("Has a render target.");
            }
            spriteBatch.End();
            hasDrawntoPrimary = false;
        }


        public virtual RenderTarget2D CreateTarget(Point size)
        {
            return new RenderTarget2D(gd, size.X, size.Y);
        }

        public virtual void StartTarget(RenderTarget2D target)
        {

            if (hasTarget)
            {
                throw new InvalidOperationException("Already has a target");
            }

            if (hasDrawntoPrimary)
            {
                throw new InvalidOperationException("A render target cannot be created after graphics has been drawn to the window.");
            }


            gd.SetRenderTarget(target);
            spriteBatch.Begin();
            hasTarget = true;

        }


        public virtual void EndTarget()
        {
            if (!hasTarget)
            {
                throw new InvalidOperationException("A target has not begun.");
            }



            spriteBatch.End();
            gd.SetRenderTarget(null);
            hasTarget = false;



        }

    }
}
