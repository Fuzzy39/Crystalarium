using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.View.Core
{
    public interface IRenderable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns>whether the object was rendered.</returns>
        public bool Draw(IRenderer renderer);

    }
}
