using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Core
{
    public interface IBatchRenderer : IRenderer
    {

        public void Begin();
        public void End();

    }
}
