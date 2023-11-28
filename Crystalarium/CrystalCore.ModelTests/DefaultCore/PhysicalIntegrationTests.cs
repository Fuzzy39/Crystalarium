using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalCore.Model.Core;
using CrystalCore.Model.CoreContract;
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
    public class PhysicalIntegrationTests
    {

        private class MockEntity : Entity
        {

            private bool hasColl;
            private Point size;
            public MockEntity(bool hasColl, Point size)
            {
                this.hasColl = hasColl;
                this.size = size;
            }

            public MapObject PhysicalRepresentation => throw new NotImplementedException();

            public bool HasCollision => hasColl;

            public Point Size => size;

            public bool Destroyed => false;

            public event EventHandler OnReady;

            public void Destroy()
            {
                
            }
        }


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
            Assert.AreEqual(test, list[0] );


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

    }
}
