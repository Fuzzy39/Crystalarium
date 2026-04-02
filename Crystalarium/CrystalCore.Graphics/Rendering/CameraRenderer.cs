using CrystalCore.Graphics.Core;
using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.Graphics.Rendering
{
    public class CameraRenderer : IRenderer
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
            Point pixelCoords = camera.TileToPixelCoords(rect.BoundingBox.Location).ToPoint() - new Point(1) +
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
            Vector2 pixelCoords = camera.TileToPixelCoords(position.Location) +
                                pixelBounds.Location.ToVector2();
            Vector2 pixelSize = new ((float)(size.X * camera.Scale), (float)(size.Y * camera.Scale));

            // round everything!
            Rectangle rect = new Rectangle(
                (int)MathF.Round(pixelCoords.X), (int)MathF.Round(pixelCoords.Y), 
                (int)MathF.Round(pixelSize.X), (int)MathF.Round(pixelSize.Y));

            // avoiding rounding, I guess? Everything still looks sorta funky.
            //if(rect.Width*rect.Height>100)  rect.Inflate(1, 1);
            baseRenderer.Draw(texture, rect, d, color);
        }

        public void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            throw new NotImplementedException();
        }


    }
}
