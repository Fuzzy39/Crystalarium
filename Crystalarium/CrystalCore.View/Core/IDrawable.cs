using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Core
{
    public interface IRenderable
    {
        public void Draw(IRenderer renderer);

    }
}
