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

        public void Draw(Texture2D texture, RotatedRect position, Color color);



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
