using CrystalCore.Model.Physical;
using CrystalCore.Model.Physical.Default;

namespace CrystalCoreTests.Model.DefaultCore
{
    [TestClass()]
    public class ChunkTests
    {

     

        

        [TestMethod()]
        public void RegisterObjectTest()
        {
            
            // arrange
            Chunk ch = new DefaultChunk(new MockGrid(), new(0,0));

            MockMapObj obj = new MockMapObj(new(0, 0, 1, 1));


            //act
            ch.RegisterObject(obj);
    
            // did the object get registered?
            Assert.AreEqual(1, ch.ObjectsIntersecting.Count);
            Assert.AreEqual(obj, ch.ObjectsIntersecting[0]);

         
            obj.Destroy();

         
            // did the object get un-registered?
            Assert.AreEqual(0, ch.ObjectsIntersecting.Count);
         
        }

        [TestMethod()]
        public void RegisterObjectTest2()
        {

            // arrange
            Chunk ch = new DefaultChunk(new MockGrid(), new(0, 0));

            MockMapObj obj = new MockMapObj(new(15, 0, 2, 1));


            //act
            ch.RegisterObject(obj);

            // did the object get registered?
            Assert.AreEqual(1, ch.ObjectsIntersecting.Count);
            Assert.AreEqual(obj, ch.ObjectsIntersecting[0]);


            obj.Destroy();


            // did the object get un-registered?
            Assert.AreEqual(0, ch.ObjectsIntersecting.Count);

        }

        [TestMethod()]
        public void DestroyTest()
        {
            // arrange
            Chunk ch = new DefaultChunk(new MockGrid(), new(0, 0));
            MockMapObj obj = new MockMapObj(new(0, 0, 1, 1));
            ch.RegisterObject(obj);

            bool eventRaised = false;
            ch.OnDestroy += (MapComponent mc, EventArgs e) => eventRaised = true;

            // act
            ch.Destroy();

            // assert
            Assert.IsTrue(eventRaised);
            Assert.IsTrue(obj.Destroyed);
            Assert.IsTrue(ch.Destroyed);

        }
    }
}
