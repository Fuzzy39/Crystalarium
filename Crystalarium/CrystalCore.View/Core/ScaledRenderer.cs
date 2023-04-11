using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.MathF;

namespace CrystalCore.View.Core
{
    public class ScaledRenderer : BasicRenderer, IBatchRenderer
    {

        private static Vector2 internalResolution = new Vector2(1600,900);
        private Vector2 windowResolution;
        private GraphicsDevice graphicsDevice;

        public static float Width
        {
            get
            {
                return internalResolution.X;
            }
        }


        public static float Height
        {
            get
            {
                return internalResolution.Y;
            }
        }

        public ScaledRenderer(GraphicsDevice gd): base(new SpriteBatch(gd))
        {

            windowResolution = gd.Viewport.Bounds.Size.ToVector2();
            graphicsDevice = gd;

        }

        public override void End()
        {
            base.End();
            windowResolution = graphicsDevice.Viewport.Bounds.Size.ToVector2();
        }

        public override void Draw(Texture2D texture, RotatedRect position, Rectangle sourceRect, Color color)
        {
            // I somehow have a hunch that this math won't work right the first time...


            Vector2 size;
            float rot = position.Rotation;
            if((rot < -PI/2f) || (rot>0 && rot<=PI/2f))
            {
                // when a rectangle is rotated a certain ammount, height and width should be scaled as opposities
                // this code ought to be a seperate method but whatever.    
                size  = new(position.Height, position.Width);
                size = ToRealResolution(size);
                float x, y;
                size.Deconstruct(out y, out x);
                size = new(x, y);

            }
            else
            {
                size = new(position.Width, position.Height);
                size = ToRealResolution(size);
            }

                

            RotatedRect real = RotatedRect.FromBoundingLocation
                (ToRealResolution(position.BoundingBox.Location), size, position.Rotation);

            base.Draw(texture, real, sourceRect, color);

        }


        public override void DrawString(FontFamily font, string text, Vector2 position, float height, Color color)
        {
            // won't work super well at different aspect ratios but I don't think there's a ton I can do about it.
            height = ScaleY(height);
            position = ToRealResolution(position);
            base.DrawString(font, text, position, height, color);
        }


        public Vector2 ToVirtualResolution(Vector2 realRes)
        {

            realRes.X *= internalResolution.X;
            realRes.X /= windowResolution.X;

            realRes.Y *= internalResolution.Y;
            realRes.Y /= windowResolution.Y;

            return realRes;

        }

        private Vector2 ToRealResolution(Vector2 virtRes)
        {
            return new(ScaleX(virtRes.X), ScaleY(virtRes.Y));
        }

        private float ScaleX(float x)
        {
            x/=internalResolution.X;
            x*=windowResolution.X;
            return x;
        }

        private float ScaleY(float y)
        {
            y /= internalResolution.Y;
            y *= windowResolution.Y;
            return y;
        }

      

       
    }
}
