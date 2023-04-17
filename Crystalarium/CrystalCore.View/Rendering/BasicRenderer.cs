using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }

        public virtual void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            font.Draw(spriteBatch, text, height, position, color);
        }

        void IBatchRenderer.Begin()
        {
            spriteBatch.Begin();
        }

        public virtual void End()
        {
            spriteBatch.End();
        }
    }
}
