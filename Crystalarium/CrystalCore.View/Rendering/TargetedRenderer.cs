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

    /// <summary>
    /// Renders within bounds by rendering to a texture, using a renderTarget.
    /// </summary>
    internal class TargetedRenderer : IRenderer
    {

        /// <summary>
        ///   The pixel coordinates of the window that this BoundedRenderer will render within.
        /// </summary>
        private Rectangle _pixelBoundry;

        
        private RenderTarget2D _renderTarget;

        public Rectangle PixelBoundry
        {
            get => _pixelBoundry;
        }


        public TargetedRenderer(GraphicsDevice gd, Rectangle pixelBoundry)
        {
            _pixelBoundry = pixelBoundry;
            _renderTarget = new RenderTarget2D
            (
                gd, 
                pixelBoundry.Width, 
                pixelBoundry.Height,
                false, 
                gd.PresentationParameters.BackBufferFormat, 
                DepthFormat.Depth24
            );

        }



        public void RenderTexture(SpriteBatch sb, Texture2D texture, Rectangle pixelBounds, Color c, Direction d)
        {
            sb.GraphicsDevice.SetRenderTarget(_renderTarget);

            sb.Draw(texture, pixelBounds, c);

            sb.GraphicsDevice.SetRenderTarget(null);
           
        }
    }
}
