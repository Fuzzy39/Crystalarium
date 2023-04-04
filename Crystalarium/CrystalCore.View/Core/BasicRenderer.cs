using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Core
{
    /// <summary>
    /// A Basic Renderer accomplishes the tasks of a renderer with a spritebatch.
    /// No bells and whistles. Just adds a layer of abstraction above the spritebatch.
    /// </summary>
    public class BasicRenderer: IBatchRenderer
    {

        private SpriteBatch spriteBatch;

        public BasicRenderer(SpriteBatch sb)
        {
            spriteBatch = sb;
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
