using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Sim;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.ChunkRender
{
    internal abstract class RendererBase
    {
        // this class provides for the basic features a chunk render needs to have.
        // It does not sepcify how chunks are rendered.

        protected GridView renderTarget;
        protected Chunk renderData;

        internal Chunk Chunk
        {
            get => renderData;
        }

        protected RendererBase( GridView v, Chunk ch, List<RendererBase> others)
        {
            // check that we don't already exist
            foreach (Renderer r in others)
            {
                if (r.Chunk == ch)
                    throw new InvalidOperationException("Attempted to create an already existing Chunk Renderer.");
                    
            }

               

            // add ourselves to the list of renderers.
            v.AddRenderer(this);

            renderTarget = v;
            renderData = ch;
        }

        // remove external refrences to this object.
        internal void Destroy()
        {
            renderTarget.RemoveRenderer(this);
        }


        // returns whether drawing was successful.
        internal bool Draw(SpriteBatch sb)
        {
            // probably don't kill anybody.
            // we might have to kill ourselves, if we aren't rendering anything.

            // check that we are visible on screen.
            if (renderTarget.Camera.TileBounds().Intersects(renderData.Bounds))
            {
              
                Render(sb);
                return true;
            }
            else
            {

                
                
                this.Destroy();
                return false;
                
            }


        }

        protected abstract void Render(SpriteBatch sb);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if(!(obj is Renderer))
                return false;
            Renderer rend = (Renderer)obj;
            if(rend.renderData==renderData & rend.renderTarget== renderTarget )
            {
                return true;
            }

            return false;
           
        }

      

    }
}
