using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CrystalCore.View.Rendering
{
    /// <summary>
    /// A Bounded renderer renders things within a boundry.
    /// Images which are partly outside of this boundry are cropped to fit. 
    /// This class is not an *actual* renderer, and is going to be destroyed shortly, probably.
    /// </summary>
    internal class OldCameraHelper
    {
        /// <summary>
        ///   The pixel coordinates of the window that this BoundedRenderer will render within.
        /// </summary>
        private Rectangle _pixelBoundry;

        private IRenderer _rend;
        
        /// <summary>
        ///   The pixel coordinates of the window that this BoundedRenderer will render within.
        /// </summary>
        public Rectangle PixelBoundry
        {
            get => _pixelBoundry;
        }

        /// <summary>
        ///  Create a new BoundedRender with the specified pixel bounds.
        /// </summary>
        /// <param name="pixelBoundry"> The pixel coordinates of the window that this BoundedRenderer will render within. </param>
        public OldCameraHelper(Rectangle pixelBoundry, IRenderer rend)
        {
            _pixelBoundry = pixelBoundry;
            _rend = rend;
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
        public void RenderTexture(Texture2D texture, Rectangle pixelBounds, Color c, Direction d)
        {

            Rectangle bounds = new(pixelBounds.Location + _pixelBoundry.Location, pixelBounds.Size);
            _rend.Draw(texture, RotatedRect.FromFootprint(bounds, d), c);
            return;

           
                

        }

    }

}
