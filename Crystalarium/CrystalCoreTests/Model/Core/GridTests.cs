using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalCore.Model.Core;
using CrystalCore.Model.ObjectContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CrystalCoreTests.Model.Core
{
    [TestClass()]
    public class GridTests
    {

        public class MockChunk : Chunk, MapComponent
        {
            private Point _chunkCoords;

            public MockChunk(Point chunkCoords)
            {
                _chunkCoords = chunkCoords;
            }


            public Point ChunkCoords => _chunkCoords;

            public List<MapObject> ObjectsWithin => throw new NotImplementedException();

            public Map Map => throw new NotImplementedException();

            public bool Destroyed => throw new NotImplementedException();


            public event EventHandler OnDestroy = null;
            public event EventHandler OnReady = null;

            public void Destroy()
            {
                throw new NotImplementedException();
            }

        
            public void Ready()
            {
                throw new NotImplementedException();
            }
        }


        public class MockChunkComponentFactory : ComponentFactory
        {
            public Chunk CreateChunk(Point chunkCoords)
            {
                return new MockChunk(chunkCoords);
            }

            public MapObject CreateObject()
            {
                throw new NotImplementedException();
            }

            public MapObject CreateObjectWithCollision()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod()]
        public void BasicTest()
        {


            Grid g = new Grid(new MockChunkComponentFactory());

            // about the grid
            Assert.IsTrue(g.ChunkList.Count == 1);
            Assert.IsTrue(g.ChunkOrigin.Equals(new Point(0)));
            Assert.IsTrue(g.ChunkSize.Equals(new Point(1)));

            // about the chunk
            Chunk ch = g.ChunkList[0];
            Assert.IsNotNull(ch);
            Assert.IsTrue(ch.ChunkCoords.Equals(new Point(0)));

        }
    }
}