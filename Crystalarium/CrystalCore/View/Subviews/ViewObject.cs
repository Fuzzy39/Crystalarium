using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Subviews
{
    internal abstract class ViewObject
    {

        // an object in grid space to be rendered by a gridview.
        // not neceserrily an object on the grid.

        protected GridView renderTarget;

        public GridView RenderTarget
        {
            get { return renderTarget; }
        }
        //Jacob did unspeakable thngs here
        protected ViewObject(GridView v)
        {
            renderTarget = v;
        }

        internal abstract bool Draw(SpriteBatch sb);
    }
}
