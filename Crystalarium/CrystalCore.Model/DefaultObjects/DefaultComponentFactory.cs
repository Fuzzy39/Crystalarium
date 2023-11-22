using CrystalCore.Model.Core;
using CrystalCore.Model.CoreContract;
using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.DefaultObjects
{
    internal class DefaultComponentFactory : ComponentFactory
    {
        private Map _map;

        public DefaultComponentFactory(Map map)
        {
            _map = map;
        }

        public Chunk CreateChunk(Point chunkCoords)
        {
            return new DefaultChunk(_map, chunkCoords);
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
