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
        private Viewbox _debugTarget; // this is where we get the debug info from.

        public Viewbox Target
        {
            get => _debugTarget;
            set
            {
                _debugTarget = value;
            }
        }

        public Debug(Viewbox v, Chunk ch, List<Renderer> others) : base(v, ch, others) { }

        protected override void Render(SpriteBatch sb)
        {
            // determine the color:
            Color color=Color.Black;
            if(_debugTarget != null & renderData.Parent == _debugTarget.Grid )
            {
               // check if the target is rendered
               if(isRenderedByTarget(renderData))
               {
                    color = Color.White;
               }
               else
               {
                    color = Color.Blue;
               }
            }

            renderTarget.RenderTexture(sb, Textures.pixel, renderData.Bounds, color);

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
