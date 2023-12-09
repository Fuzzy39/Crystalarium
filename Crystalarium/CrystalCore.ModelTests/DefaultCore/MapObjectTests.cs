using CrystalCore.Model.Physical;
using CrystalCore.Model.Physical.Default;
using Microsoft.Xna.Framework;

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
