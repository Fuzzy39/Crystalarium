using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using CrystalCore.Util.Profiling;
using CrystalCore.View.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public void Update(GameTime gametime, Rectangle bounds)
        {
            camera.Update(gametime, bounds);
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
                throw new ArgumentException("A Camera was asked to render a texture with bounds " + bounds +
                                            ". Negative size is not acceptable.");
            }

            if (bounds.Area == 0)
            {
                return;
            }

            //Console.WriteLine(rect.BoundingBox);

            Vector2 size = rect.Size;
            Point pixelCoords = camera.TileToPixelCoords(rect.BoundingBox.Location) - new Point(1) +
                                pixelBounds.Location;
            Point pixelSize = new Point((int)(size.X * camera.Scale), (int)(size.Y * camera.Scale)) +
                              new Point(1, 1);


            //Rectangle footprint = new Rectangle(pixelCoords, pixelSize);
            Direction facing = DirectionUtil.FromRadians(rect.Rotation);

            //  baseRenderer.Draw(texture, RotatedRect.FromFootprint(footprint, facing), c);
            baseRenderer.Draw(texture, RotatedRect.FromBoundingLocation(pixelCoords, pixelSize, rect.Rotation), c);
            
        }



       
        public void Draw(Texture2D texture, RectangleF position, Direction d, Color color)
        {
            Vector2 size = position.Size;
            Point pixelCoords = camera.TileToPixelCoords(position.Location) +
                                pixelBounds.Location;
            Point pixelSize = new Point((int)(size.X * camera.Scale), (int)(size.Y * camera.Scale));
         

            baseRenderer.Draw(texture,new Rectangle(pixelCoords, pixelSize), d, color);
        }

        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            throw new NotImplementedException();
        }


    }
}
