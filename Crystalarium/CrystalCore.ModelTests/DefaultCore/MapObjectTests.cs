using CrystalCore.Model.CoreContract;
using CrystalCore.Model.DefaultObjects;
using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCoreTests.Model.DefaultCore
{
    [TestClass()]
    public class MapObjectTests
    {
      

        [TestMethod()]
        public void CreateTest()
        {
            MapObject obj = new DefaultMapObject(new MockGrid(), new(2), new MockEntity(false, new(2)));

            Assert.AreEqual(new Rectangle(2, 2, 2, 2), obj.Bounds);

            // for the mock grid, this will always return the same chunk.
            Chunk ch = ((MockGrid)obj.Grid)._chunk;

            Assert.AreEqual(1, ((MockChunk)ch)._calledRegister.Count);
            Assert.AreEqual(obj, ((MockChunk)ch)._calledRegister[0]);



        }

  



    }
}
