using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Core
{
    public class Text : IDrawable
    {

        // the height, in pixels, of a line of text.
        private float height;
        private Vector2 position; 
        private string text;
        private FontFamily font;


        void IDrawable.Draw(IRenderer renderer)
        {
            throw new NotImplementedException();
        }
    }
}
