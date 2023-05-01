using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Core
{
    internal interface IPreRenderable : IRenderable
    {
      
        public void PreDraw(IBatchRenderer renderer);
    }
}
