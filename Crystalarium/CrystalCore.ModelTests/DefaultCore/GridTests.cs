using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using CrystalCore.Util;
using NuGet.Frameworks;
using CrystalCore.Model.Core.Default;
using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;

namespace CrystalCoreTests.Model.DefaultCore
{
    [TestClass()]
    public class GridTests
    {

        


       

        [TestMethod()]
        public void BasicTest()
        {


            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());

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

            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());

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

            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());

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

            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());

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
            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());

            

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

        [TestMethod()]
        public void ExpandToFitTest()
        {
            // arrange
            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());

            // act
            g.ExpandToFit(new(-1, -1, 18, 18));

            // assert
            Assert.AreEqual(9, g.ChunkList.Count);
            Assert.AreEqual(new Point(-1, -1), g.ChunkOrigin);
            Assert.AreEqual(new Point(3, 3), g.ChunkSize);
            Assert.IsTrue(g.Chunks.Count == 3 && g.Chunks[0].Count == 3);

        }

        [TestMethod()]
        public void ChunkAtCoordsTest()
        {

            // arrange
            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());
            g.ExpandToFit(new(-1, -1, 18, 18));

           

            // assert
            Assert.AreEqual(g.Chunks[0][0], g.ChunkAtCoords(new(-7, -7)));

            Assert.AreEqual(g.Chunks[1][1], g.ChunkAtCoords(new(15, 0)));

            Assert.AreEqual(g.Chunks[1][2], g.ChunkAtCoords(new(0, 16)));


        }

        [TestMethod()]
        public void ChunksIntersectingTest()
        {
            // arrange
            DefaultGrid g = new DefaultGrid(new MockChunkComponentFactory());
            g.ExpandToFit(new(-1, -1, 18, 18));


            // act
            List < Chunk > chunks = g.ChunksIntersecting(new(-10, 0, 50, 16));

            // assert
            Assert.IsTrue(chunks.Contains(g.Chunks[0][1]));
            Assert.IsTrue(chunks.Contains(g.Chunks[1][1]));
            Assert.IsTrue(chunks.Contains(g.Chunks[2][1]));
            Assert.AreEqual(3, chunks.Count);


        }

        /*[TestMethod()]
        public void ObjectsIntersectingTest()
        {
           // Assert.Fail();
        }*/
    }
}