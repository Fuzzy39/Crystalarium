using System;
using System.Collections.Generic;
using System.Text;
using Crystalarium.Sim;
using Microsoft.Xna.Framework.Graphics;

namespace Crystalarium.Render.ChunkRender
{
    public abstract class Renderer
    {
        // Chunk.Renderer renders a chunk.
        // that's the idea, at least.

        protected Viewbox renderTarget;
        protected Chunk renderData;

        public Renderer( Viewbox v, Chunk ch)
        {
            // add ourselves to the list of renderers.
            v.AddRenderer(this);

            renderTarget = v;
            renderData = ch;
        }

        // remove external refrences to this object.
        public void Destroy()
        {
            renderTarget.RemoveRenderer(this);
        }


        public void Draw(SpriteBatch sb)
        {
            // probably don't kill anybody.
            // we might have to kill ourselves, if we aren't rendering anything.

            // check that we are visible on screen.
            if (renderTarget.TileBounds().Intersects(renderData.Bounds))
            {
                Render(sb);
            }
            else
            {
                this.Destroy();
            }


        }

        protected abstract void Render(SpriteBatch sb);


    }
}
