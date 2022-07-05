using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View
{
    internal abstract class ViewObject
    {

        // an object in grid space to be rendered by a gridview.
        // not neceserrily an object on the grid.

        protected GridView renderTarget;

        protected ViewObject(GridView v)
        {
            renderTarget = v;
        }

        internal abstract void Destroy();
        internal abstract bool Draw( SpriteBatch sb);
    }
}
