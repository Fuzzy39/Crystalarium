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
    public class BasicRenderer: IRenderer
    {

        private SpriteBatch spriteBatch;

        public BasicRenderer(SpriteBatch sb)
        {
            spriteBatch = sb;
        }

        public void Draw(Texture2D texture, RotatedRect destination, Color color)
        {

           


            spriteBatch.Draw(
                texture, 
                destination.AsRectangle, 
                null, color, destination.Rotation, 
                new(0),
                SpriteEffects.None, 0f
            );

        }

        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {  
            font.Draw(spriteBatch, text, height, position, color);
        }

        public void DrawString(FontFamily font, string text, Point position, float height, Color color)
        {
            this.DrawString(font, text, position.ToVector2(), height, color);
        }
    }
}
