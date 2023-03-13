using CrystalCore.Util;
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
    /// An IRenderer represents an object that draws textures.
    /// </summary>
    internal interface IRenderer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="destinationRectangle"></param>
        /// <param name="color"></param>
        /// <param name="rotation">rotation around center of destination in radians</param>
        /// 

        public void Draw(Texture2D texture, RectangleF destination, Color color, float rotation);

        public void Draw(Texture2D texture, RectangleF destination, Color color, Direction d)
        {
            Draw(texture, destination, color, d.ToRadians());
        }

        public void Draw(Texture2D texture, RectangleF destination, Color color, CompassPoint cp)
        {
            Draw(texture, destination, color, cp.ToRadians());
        }

        public void Draw(Texture2D texture, RectangleF destination, Color color)
        {
            Draw(texture, destination, color, 0f);
        }


        public void Draw(Texture2D texture, Rectangle destination, Color color, float rotation)
        {
            Draw(texture, new RectangleF(destination), color, rotation);
        }

        public void Draw(Texture2D texture, Rectangle destination, Color color, Direction d)
        {
            Draw(texture, destination, color, d.ToRadians());
        }

        public void Draw(Texture2D texture, Rectangle destination, Color color, CompassPoint cp)
        {
            Draw(texture, destination, color, cp.ToRadians());
        }

        public void Draw(Texture2D texture, Rectangle destination, Color color)
        {
            Draw(texture, destination, color, 0f);
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
        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color );

        public void DrawString(FontFamily font, string text, Point position, float height, Color color);



    }
}
