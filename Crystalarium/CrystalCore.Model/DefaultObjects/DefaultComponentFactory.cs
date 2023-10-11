using CrystalCore.Model.Core;
using CrystalCore.Model.ObjectContract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.DefaultObjects
{
    internal class DefaultComponentFactory : ComponentFactory
    {
        public Chunk CreateChunk(Map map, Point chunkCoords)
        {
            return new DefaultChunk(map, chunkCoords);
        }

        public MapObject CreateObject(PARAMETERS_GO_HERE )
        {
            throw new NotImplementedException();
        }

        public MapObject CreateObjectWithCollision(PARAMETERS_GO_HERE )
        {
            throw new NotImplementedException();
        }
    }
}
