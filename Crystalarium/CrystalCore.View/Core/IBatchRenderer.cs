using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Core
{
    internal interface IBatchRenderer : IRenderer
    {

        internal void begin();
        internal void end();

    }
}
