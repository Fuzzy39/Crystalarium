using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Sim;
using CrystalCore.Sim.Base;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View
{
    internal abstract class RendererBase
    {
        // this class provides for the basic features a chunk render needs to have.
        // It does not sepcify how chunks are rendered.

        protected GridView renderTarget;
        protected GridObject _renderData;
        protected List<RendererBase> _others;

        internal GridObject RenderData
        {
            get => _renderData;
        }

        protected RendererBase( GridView v, GridObject o, List<RendererBase> others)
        {
            // check that we don't already exist
            foreach (RendererBase r in others)
            {
                if (r.RenderData == o)
                    throw new InvalidOperationException("Attempted to create an already existing Renderer.");
                    
            }
            _others = others;


               

            // add ourselves to the list of renderers.
            v.AddRenderer(this);

            renderTarget = v;
            _renderData = o;
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
            if(_renderData == null)
            {
                this.Destroy();
            }

            // check that we are visible on screen.
            if (renderTarget.Camera.TileBounds().Intersects(_renderData.Bounds))
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
            if(!(obj is RendererBase))
                return false;
            RendererBase rend = (RendererBase)obj;
            if(rend._renderData==_renderData & rend.renderTarget== renderTarget )
            {
                return true;
            }

            return false;
           
        }

      

    }
}
