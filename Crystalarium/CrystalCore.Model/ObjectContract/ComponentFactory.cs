using CrystalCore.Model.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.ObjectContract
{
    internal interface ComponentFactory
    {

        public Chunk CreateChunk(Map map, Point chunkCoords);

        public MapObject CreateObject(PARAMETERS_GO_HERE);

        public MapObject CreateObjectWithCollision(PARAMETERS_GO_HERE);
       

    }
}
s