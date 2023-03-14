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

        public void Draw(Texture2D texture, RectangleF destination, Color color, float rotation)
        {

            // we assume spritebatch has been begun if we are being called.
            // note:
            // rotation: radians clockeise
            // destination: position is the highest and lefrmost part of the texture (may go outside of texture) after rotation
            //      size is before rotation.



            Point size = destination.Size.ToPoint();
            Vector2 origin = new Vector2(texture.Width/ 2f, texture.Height / 2f);
            Vector2 pos = destination.TopLeft;

            float multiplier = (MathF.Sqrt(2) / 2 - .5f);

            Vector2 mults = multiplier * destination.Size;

            Vector2 newPos = new(pos.X+ size.X/2f + MathF.Abs(MathF.Sin(rotation*2f)*mults.X),
                            pos.Y+size.Y/2f + MathF.Abs(MathF.Sin(rotation * 2f) * mults.Y) );

          

            spriteBatch.Draw(
                texture, 
                new Rectangle(newPos.ToPoint(), size), 
                null, color, rotation, 
                origin,
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
