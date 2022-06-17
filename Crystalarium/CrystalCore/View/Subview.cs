using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Sim;
using CrystalCore.Sim.Base;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View
{
    internal abstract class Subview
    {
        /*
         *  This class supplies the foundational code required to render a gridobject in a gridview.
         *  It is a subview because it renders a piece of the whole grid.
         * 
         */

        protected GridView renderTarget;
        protected GridObject _renderData;
        protected List<Subview> _others;

        internal GridObject RenderData
        {
            get => _renderData;
        }

        protected Subview( SubviewManager m, GridObject o, List<Subview> others)
        {
            // check that we don't already exist
            foreach (Subview r in others)
            {
                if (r.RenderData == o)
                    throw new InvalidOperationException("Attempted to create an already existing Renderer.");
                    
            }
            _others = others;


               

            // add ourselves to the list of renderers.
            m.AddRenderer(this);

      
            renderTarget = m.Parent;
            _renderData = o;
        }

        // remove external refrences to this object.
        internal void Destroy()
        {
            renderTarget.Manager.RemoveRenderer(this);
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
            if(!(obj is Subview))
                return false;
            Subview rend = (Subview)obj;
            if(rend._renderData==_renderData & rend.renderTarget== renderTarget )
            {
                return true;
            }

            return false;
           
        }

      

    }
}
