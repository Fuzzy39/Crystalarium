using CrystalCore.Model.CoreContract;
using CrystalCore.Model.DefaultObjects;
using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCoreTests.Model.DefaultCore
{
    [TestClass()]
    public class ChunkTests
    {

        private class MockMap : Map
        {
            // could cause issues, be careful with that.
            public Grid Grid => null;

            public event ComponentEvent? OnMapComponentDestroyed;
            public event MapObjectEvent? OnMapObjectReady;
            public event EventHandler? OnReset;

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Reset(Rectangle minimumBounds)
            {
                throw new NotImplementedException();
            }


            void Map.OnComponentDestroyed(MapComponent component, EventArgs e)
            {

                OnMapComponentDestroyed?.Invoke(component, e);
            }

            void Map.OnObjectReady(MapObject mapObj, EventArgs e)
            {
                throw new NotImplementedException();
            }
        }

        private class MockMapObj : MapObject
        {
            bool _destroyed = false;
            Rectangle _bounds;
            public MockMapObj(Rectangle b) 
            {
                _bounds = b;
            }
            public Rectangle Bounds => _bounds;

            public Entity Entity => throw new NotImplementedException();

            public Chunk Parent => throw new NotImplementedException();

            public Grid Grid => throw new NotImplementedException();

            public bool Destroyed => _destroyed;

            public event ComponentEvent OnDestroy;

            public void Destroy()
            {
                _destroyed = true;
                OnDestroy.Invoke(this, new EventArgs());

            }
        }


        [TestMethod()]
        public void RegisterObjectTest()
        {
            
            // arrange
            Chunk ch = new DefaultChunk(new MockMap(), new(0,0));

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
        public void DestroyTest()
        {
            // arrange
            Chunk ch = new DefaultChunk(new MockMap(), new(0, 0));
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
