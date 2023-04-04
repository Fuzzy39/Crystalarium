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
    internal class ScaledRenderer : IRenderer
    {
        public void Draw(Texture2D texture, RotatedRect position, Rectangle sourceRect, Color color)
        {
            throw new NotImplementedException();
        }

        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            throw new NotImplementedException();
        }
    }
}
