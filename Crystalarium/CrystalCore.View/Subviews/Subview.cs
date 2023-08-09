using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Model;
using CrystalCore.Model.Elements;
using CrystalCore.View.Core;
using CrystalCore.View.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Subviews
{
    /// <summary>
    ///   A Subview renders a piece of the whole grid. 
    /// </summary>
    internal abstract class Subview : IRenderable
    {

        protected MapObject _renderData; // the thing we view
        protected GridView renderTarget;

        internal MapObject RenderData
        {
            get => _renderData;
        }


        protected Subview(GridView v, MapObject o)
        {
        
            _renderData = o;
            renderTarget = v;


        }

        /// <summary>
        /// Draw this GridObject.
        /// </summary>
        /// <param name="sb"></param>
        /// <returns>whether the object being drawn exists </returns>

        public bool Draw(IRenderer rend)
        {

            if(RenderData.Destroyed) return false;

            // check that we are visible on screen.
            if (renderTarget.Camera.TileBounds.Intersects(_renderData.Bounds))
            {
                

                Render(rend);
               
            }
           
            return true;


        }

        /// <summary>
        /// Subclasses of Subview use this method to do the actual drawing.
        /// </summary>
        /// <param name="sb"></param>
        protected abstract void Render(IRenderer rend);


    }

}
