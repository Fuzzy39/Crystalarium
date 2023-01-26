using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Rendering
{



    internal interface IRenderer
    {

        /// <summary>
        ///   The pixel coordinates of the window that this BoundedRenderer will render within.
        /// </summary>
        public Rectangle PixelBoundry
        {
            get;
        }

       

        /// <summary>
        ///  Render a texture within the bounds of this renderer.
        ///  Any images rendered partially outside of this renderer's bounds will be cropped to fit.
        /// </summary>
        /// <param name="sb">The SpriteBatch object used to render the image.</param>
        /// <param name="texture">The texture we are to render.</param>
        /// <param name="pixelBounds">The area in pixels that the image is to be rendered, relative to the location of this renderer's bounds.</param>
        /// <param name="c">The color the texture will be rendered with.</param>
        /// <param name="d">The direction this texture will be facing, with up being the default orientation.</param>
        public void RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle pixelBounds, Color c, Direction d);

    }
}
