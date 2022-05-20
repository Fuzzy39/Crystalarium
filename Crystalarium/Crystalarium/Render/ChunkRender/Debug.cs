using Crystalarium.Sim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Render.ChunkRender
{
    public class Debug: Renderer
    {

        // it is assumed that chunks are on the same grid.
        private GridView _debugTarget; // this is where we get the debug info from.

        public GridView Target
        {
            get => _debugTarget;
            set
            {
                _debugTarget = value;
            }
        }

        public Debug(GridView v, Chunk ch, List<Renderer> others) : base(v, ch, others) { }

        protected override void Render(SpriteBatch sb)
        {
            

            renderTarget.Camera.RenderTexture(sb, Textures.pixel, renderData.Bounds, determineColor());

          
            
           

        }

        private Color determineColor()
        {
            // determine the color:

            if (_debugTarget == null || renderData.Parent != _debugTarget.Grid)
            {
                return Color.Black;
            }
            int r;
            int g;
            int b;

            if(renderData.Coords.Equals(new Point(0)))
            {

                r = 150;
                g = 50;
                b = 50;
            }
            else
            {

                r = 50;
                g = 50;
                b = 150;

                Point pos =renderData.Parent.getChunkPos(renderData);
                if ((pos.X+pos.Y)%2==0)
                {
                    r += 30;
                    g += 30;
                    b += 30;
                }
            }



            // check if the target is rendered
            if (isRenderedByTarget(renderData))
            {
                r += 70;
                g += 70;
                b += 70;
            }

            return new Color(r, g, b);
           
            

          
            
            
        }

        private bool isRenderedByTarget(Chunk ch)
        {
            foreach(Renderer r in _debugTarget.Renderers)
            {
               if(r.Chunk == ch)
               {
                    return true; 
               }
            }

            return false;
        }

    }
}
