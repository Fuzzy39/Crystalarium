using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalCore.Model.Core;
using CrystalCore.Model.ObjectContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using CrystalCore.Util;
using NuGet.Frameworks;

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

        [TestMethod()]
        public void UpTest()
        {

            Grid g = new Grid(new MockChunkComponentFactory());

            // expand up 4 times
            for(int i = 0; i<4; i++)
            {
                g.Expand(Direction.up);
            }

            // about the grid
            Assert.IsTrue(g.ChunkList.Count == 5);
            Assert.AreEqual(g.ChunkOrigin, new Point(0, -4));
            Assert.AreEqual(g.ChunkSize, new Point(1, 5));
            Assert.IsTrue(g.Chunks.Count == 1 && g.Chunks[0].Count == 5);


            // chunks are not tested.
            foreach (Chunk chunk in g.ChunkList)
            {
                Assert.IsNotNull(chunk);
            }

        }

        [TestMethod()]
        public void DownTest()
        {

            Grid g = new Grid(new MockChunkComponentFactory());

            // expand up 4 times
            for (int i = 0; i < 4; i++)
            {
                g.Expand(Direction.down);
            }

            // about the grid
            Assert.IsTrue(g.ChunkList.Count == 5);
            Assert.AreEqual(g.ChunkOrigin, new Point(0, 0));
            Assert.AreEqual(g.ChunkSize, new Point(1, 5));
            Assert.IsTrue(g.Chunks.Count == 1 && g.Chunks[0].Count == 5);


            // chunks are not tested.
            foreach (Chunk chunk in g.ChunkList)
            {
                Assert.IsNotNull(chunk);
            }

        }

        [TestMethod()]
        public void LeftTest()
        {

            Grid g = new Grid(new MockChunkComponentFactory());

            // expand up 4 times
            for (int i = 0; i < 4; i++)
            {
                g.Expand(Direction.left);
            }

            // about the grid
            Assert.AreEqual(g.ChunkList.Count, 5);
            Assert.AreEqual(new Point(-4, 0), g.ChunkOrigin );
            Assert.AreEqual(new Point(5, 1), g.ChunkSize);
            Assert.IsTrue(g.Chunks.Count == 5 && g.Chunks[0].Count == 1);

            foreach(Chunk chunk in g.ChunkList)
            {
                Assert.IsNotNull(chunk);
            }
            // chunks are not tested.


        }

        [TestMethod()]
        public void SquareTest()
        {
            Grid g = new Grid(new MockChunkComponentFactory());

            

            Direction d = Direction.up;
            do
            {
                g.Expand(d);
                d= d.Rotate(RotationalDirection.cw);
            }
            while (d != Direction.up);


            // about the grid
            Assert.AreEqual(9, g.ChunkList.Count);
            Assert.AreEqual(new Point(-1, -1), g.ChunkOrigin);
            Assert.AreEqual(new Point(3, 3), g.ChunkSize);
            Assert.IsTrue(g.Chunks.Count == 3 && g.Chunks[0].Count == 3);

            foreach (Chunk chunk in g.ChunkList)
            {
                Assert.IsNotNull(chunk);
            }
        }
    }
}