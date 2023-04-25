using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Rendering
{
    internal class CameraRenderer : IRenderer
    {


        private Rectangle pixelBounds;
        private IRenderer baseRenderer;
        private PhysicsCamera camera;

        public PhysicsCamera Camera
        {
            get
            {
                return camera;
            }
        }
      


        public CameraRenderer(Rectangle pixelBounds, IRenderer rend)
        {
            this.pixelBounds = pixelBounds;
            baseRenderer = rend;
            camera = new PhysicsCamera(pixelBounds.Size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds">The bounds in tilespace that the camera is restrained to.</param>
        public void Update(Rectangle bounds)
        {
            camera.Update(bounds);
        }


        public void Draw(Texture2D texture, RotatedRect rect, Rectangle source, Color c)
        {

            if (texture.Width != source.Width || texture.Height != source.Height)
            {
                throw new NotImplementedException("Wait for render target!");
            }

            RectangleF bounds = rect.AsRectangleF;
            if (bounds.Area < 0)
            {
                throw new ArgumentException("A Camera was asked to render a texture with bounds " + bounds + ". Negative size is not acceptable.");
            }

            if (bounds.Area == 0)
            {
                return;
            }

            //Console.WriteLine(rect.BoundingBox);

            Vector2 size = rect.AdjustedSize;
            Point pixelCoords = camera.TileToPixelCoords(rect.BoundingBox.Location) - new Point(1) + pixelBounds.Location;
            Point pixelSize = new Point((int)(size.X * camera.Scale), (int)(size.Y * camera.Scale)) + new Point(1, 1);


            Rectangle footprint = new Rectangle(pixelCoords, pixelSize);
            Direction facing = DirectionUtil.FromRadians(rect.Rotation);

            baseRenderer.Draw(texture, RotatedRect.FromFootprint(footprint, facing), c);

        }



        // whatever, I'll fix this in a bit, once we have rendertargets
        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            throw new NotImplementedException();
        }


    }
}
