using CrystalCore.Model.Physical;
using CrystalCore.View.Core;

namespace CrystalCore.View.Subviews
{
    /// <summary>
    ///   A Subview renders a piece of the whole grid. 
    /// </summary>
    internal abstract class Subview : IRenderable
    {

        protected MapComponent _renderData; // the thing we view
        protected GridView renderTarget;

        internal MapComponent RenderData
        {
            get => _renderData;
        }


        protected Subview(GridView v, MapComponent o)
        {

            _renderData = o;
            renderTarget = v;


        }

        /// <summary>
        /// Draw this GridObject.
        /// </summary>
        /// <param name="sb"></param>
        /// <returns>whether the object being drawn exists </returns>

        public virtual bool Draw(IRenderer rend)
        {

            return !RenderData.Destroyed;


        }




    }

}
