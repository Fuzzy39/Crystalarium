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
    internal class DefaultChunk : Chunk
    {

        private Point _chunkCoords;


        public Point ChunkCoords
        {
            get => _chunkCoords;
        }

        public List<MapObject> ObjectsWithin => throw new NotImplementedException();

        public Map Map => throw new NotImplementedException();

        public bool Destroyed => throw new NotImplementedException();

        public event EventHandler OnDestroy;
        public event EventHandler OnReady;

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Ready()
        {
            throw new NotImplementedException();
        }
    }
}
