using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Core
{
    public interface IBatchRenderer : IRenderer
    {

        public Vector2 Size
        {
            get;
        }

        public float Width
        {
            get
            {
                return Size.X;
            }
        }

        public float Height
        {
            get
            {
                return Size.Y;
            }
        }




        public void Begin();
        public void End();


        public RenderTarget2D CreateTarget(Point size);
        public void StartTarget(RenderTarget2D target);

        public void EndTarget();

    }
}
