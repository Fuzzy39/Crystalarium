using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Model;
using CrystalCore.Model.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Subviews
{
    /// <summary>
    ///   A Subview renders a piece of the whole grid. 
    /// </summary>
    internal abstract class Subview : ViewObject
    {

        protected GridObject _renderData; // the thing we view
        private List<Subview> others; // I forget what this is for...

        internal GridObject RenderData
        {
            get => _renderData;
        }


        protected Subview(GridView v, GridObject o, List<Subview> others) : base(v)
        {
            // check that we don't already exist
            foreach (Subview r in others)
            {
                if (r.RenderData == o)
                    //return;
                    throw new InvalidOperationException("Attempted to create an already existing Renderer.");

            }
            this.others = others;
            _renderData = o;

            // add ourselves to the list of renderers.
            others.Add(this);

        }


        /// <summary>
        /// remove external refrences to this object.
        /// </summary>
        internal void Destroy()
        {
            others.Remove(this);
        }


        /// <summary>
        /// Draw this GridObject.
        /// </summary>
        /// <param name="sb"></param>
        /// <returns>whether drawing was successful. Subview is destroyed if false. </returns>

        internal override bool Draw(SpriteBatch sb)
        {
            // probably don't kill anybody.
            // we might have to kill ourselves, if we aren't rendering anything.
            if (_renderData == null || RenderData.Bounds.IsEmpty)
            {
                Destroy();
                return false;
            }

            // check that we are visible on screen.
            if (renderTarget.Camera.TileBounds().Intersects(_renderData.Bounds))
            {

                Render(sb);
                return true;
            }
            else
            {

                Destroy();
                return false;

            }

        }

        /// <summary>
        /// Subclasses of Subview use this method to do the actual drawing.
        /// </summary>
        /// <param name="sb"></param>
        protected abstract void Render(SpriteBatch sb);


    }

    //shh... it's a secret class, we don't talk about this one
    internal static class SubviewListExtensions
    {
        /// <summary>
        ///  Whether this list of subviews has a subview that renders this gridobject.
        /// </summary>
        /// <param name="views"></param>
        /// <param name="gobj"></param>
        /// <returns></returns>
        public static bool ViewExistsFor(this List<Subview> views, GridObject gobj)
        {
            foreach (Subview view in views)
            {
                if (view.RenderData == gobj)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
