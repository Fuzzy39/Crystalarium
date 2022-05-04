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

        protected GridView renderTarget;
        protected Chunk renderData;

        public Chunk Chunk
        {
            get => renderData;
        }


        protected Renderer( GridView v, Chunk ch, List<Renderer> others)
        {
            // check that we don't already exist
            foreach (Renderer r in others)
            {
                if (r.Chunk == ch)
                    return;
            }

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


        // returns whether drawing was successful.
        public bool Draw(SpriteBatch sb)
        {
            // probably don't kill anybody.
            // we might have to kill ourselves, if we aren't rendering anything.

            // check that we are visible on screen.
            if (renderData == null)
            {
                this.Destroy();
                return false;
            }

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

        // I guess that this implements the factory pattern? At least that was the attempt.
        public static Renderer Create( Type t, GridView v, Chunk ch, List<Renderer> others)
        {
            switch(t)
            {
                case Type.Default:
                    return new Default(v, ch, others);
                case Type.Standard:
                    return new Standard(v, ch, others);
                case Type.Debug:
                    return new Debug(v, ch, others);
                default:
                    throw new ArgumentException(t+" is not a valid type of Chunk Renderer.");
            }
        }

    }
}
