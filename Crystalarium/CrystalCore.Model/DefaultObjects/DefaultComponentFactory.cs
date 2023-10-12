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
        public Chunk CreateChunk(Point chunkCoords)
        {
            return new DefaultChunk(chunkCoords);
        }

        public MapObject CreateObject( )
        {
            throw new NotImplementedException();
        }

        public MapObject CreateObjectWithCollision( )
        {
            throw new NotImplementedException();
        }
    }
}
