using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Core
{


    /// <summary>
    /// An IRenderer represents an object that draws textures.
    /// </summary>
    public interface IRenderer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="destinationRectangle"></param>
        /// <param name="color"></param>
        /// <param name="rotation">rotation around center of destination in radians</param>
        /// 

        public void Draw(Texture2D texture, RotatedRect position, Rectangle sourceRect, Color color);

        public void Draw(Texture2D texture, RotatedRect position, Color color)
        {
            Draw(texture, position, new(new(), new(texture.Width, texture.Height)), color);
        }


        public void Draw(Texture2D texture, RectangleF position, Color color)
        {
            Draw(texture, new RotatedRect(position.Location, position.Size, 0, new(0)), color);
        }

        public void Draw(Texture2D texture, Rectangle position, Color color)
        {
            Draw(texture, new RectangleF(position), color);
        }

        public void Draw(Texture2D texture, RectangleF position, Direction d, Color color)
        {
            Draw(texture, new RotatedRect(position.Location, position.Size, d.ToRadians(), new(.5f)), color);
        }


        // primary method



        /// <summary>
        /// 
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="height">The height of a line of text.</param>
        /// <param name="color"></param>
        /// 
        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color);

        public void DrawString(FontFamily font, string text, Point position, float height, Color color)
        {
            DrawString(font, text, position.ToVector2(), height, color);
        }



    }
}
