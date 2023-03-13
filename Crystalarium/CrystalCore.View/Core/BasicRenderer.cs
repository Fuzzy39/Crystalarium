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
    internal class BasicRenderer: IRenderer
    {

        private SpriteBatch spriteBatch;

        internal BasicRenderer(SpriteBatch sb)
        {
            spriteBatch = sb;
        }

        public void Draw(Texture2D texture, RectangleF destination, Color color, float rotation)
        {
            throw new NotImplementedException();
        }

        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            throw new NotImplementedException();
        }

        public void DrawString(FontFamily font, string text, Point position, float height, Color color)
        {
            throw new NotImplementedException();
        }
    }
}
