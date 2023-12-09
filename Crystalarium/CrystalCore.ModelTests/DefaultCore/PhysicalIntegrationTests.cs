using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Core.Default;

namespace CrystalCoreTests.Model.DefaultCore
{
    [TestClass()]
    public class PhysicalIntegrationTests
    {

       


        [TestMethod()]
        public void ObjectTest()
        {
            // arrange
            Map m = new DefaultMap();
            m.Grid.ExpandToFit(new(-1, -1, 18, 18)); // get a grid of 3x3 chunks centered on the origin chunk.

            ComponentFactory f = m.Grid.ComponentFactory;

            // act
            MapObject test = f.CreateObject(new(0), new MockEntity(false, new(2)));

            //assert

            Assert.IsNotNull(test);
            Assert.AreEqual(m.Grid.ChunkAtCoords(new(1)), test.Parent);

            Assert.AreEqual(new(0), test.Bounds.Location);
            Assert.AreEqual(new(2), test.Bounds.Size);

            List<MapObject> list = m.Grid.ObjectsIntersecting(new(-16, -16, 48, 48));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(test, list[0]);


            // arrange again, hmm
            bool ObjectRaised = false;
            int timesMapRaised = 0;
            MapObject MapRaised = null;

            test.OnDestroy += (MapComponent mc, EventArgs e) => ObjectRaised = true;
            m.OnMapComponentDestroyed += (MapComponent mc, EventArgs e) => { timesMapRaised++; MapRaised = (MapObject)mc; };


            // act again
            test.Destroy();
            list = m.Grid.ObjectsIntersecting(new(-16, -16, 48, 48));

            // assert again
            Assert.AreEqual(0, list.Count);
            Assert.IsTrue(test.Destroyed);
            Assert.IsTrue(ObjectRaised);
            Assert.AreEqual(1, timesMapRaised);
            Assert.AreEqual(test, MapRaised);




        }




        [TestMethod()]
        public void ObjectsTest()
        {
            // arrange
            Map m = new DefaultMap();
            Grid g = m.Grid;
            g.ExpandToFit(new(-1, -1, 18, 18)); // get a grid of 3x3 chunks centered on the origin chunk.
            ComponentFactory f = g.ComponentFactory;

            // act
            MapObject one = f.CreateObject(new(15, 0), new MockEntity(false, new(2))); // should be between two chunks
            MapObject two = f.CreateObject(new(14, 1), new MockEntity(false, new(2)));

            // the objects exist.
            Assert.AreEqual(2, g.ObjectsIntersecting(new(0,0,32,16)).Count);
            Assert.AreEqual(2, g.ObjectsIntersecting(new(15, 1, 1, 1)).Count);

            Assert.AreEqual(1, g.ChunkAtCoords(new(20, 5)).ObjectsIntersecting.Count);
            Assert.AreEqual(one, g.ChunkAtCoords(new(20, 5)).ObjectsIntersecting[0]);

            Chunk origin = g.ChunkAtCoords(new(0, 0));
            Assert.AreEqual(2, origin.ObjectsIntersecting.Count);
            Assert.IsTrue(origin.ObjectsIntersecting.Contains(one));
            Assert.IsTrue(origin.ObjectsIntersecting.Contains(two));

            // act again
            one.Destroy();


            // the objects exist.
            Assert.AreEqual(1, g.ObjectsIntersecting(new(0, 0, 32, 16)).Count);
            Assert.AreEqual(1, g.ObjectsIntersecting(new(15, 1, 1, 1)).Count);

            Assert.AreEqual(0, g.ChunkAtCoords(new(20, 5)).ObjectsIntersecting.Count);
        
            Assert.AreEqual(1, origin.ObjectsIntersecting.Count);
            Assert.IsFalse(origin.ObjectsIntersecting.Contains(one));
            Assert.IsTrue(origin.ObjectsIntersecting.Contains(two));



        }


        [TestMethod()]
        public void CollisionTest()
        {
            Map m = new DefaultMap();
            Grid g = m.Grid;
            g.ExpandToFit(new(-1, -1, 18, 18)); // get a grid of 3x3 chunks centered on the origin chunk.
            ComponentFactory f = g.ComponentFactory;
            MockEntity entity = new(true, new(2));

            f.CreateObject(new(15, 0),entity);

            Assert.IsFalse(f.IsValidPosition(new(16, 0), entity));
            Assert.IsTrue(f.IsValidPosition(new(16, 0), new MockEntity(false, new(2))));
            Assert.IsFalse(f.IsValidPosition(new(14, -1), entity));
            Assert.IsTrue(f.IsValidPosition(new(13, 0), entity));


        }
    }
}
