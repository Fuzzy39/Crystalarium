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
    internal class MockMap : Map
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

    internal class MockEntity : Entity
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

    internal class MockMapObj : MapObject
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

    internal class MockChunk : Chunk, MapComponent
    {
        private Point _chunkCoords;

        public MockChunk(Point chunkCoords)
        {
            _chunkCoords = chunkCoords;
        }


        public Point ChunkCoords => _chunkCoords;

        public List<MapObject> ObjectsIntersecting => throw new NotImplementedException();

        public Map Map => throw new NotImplementedException();

        public bool Destroyed => throw new NotImplementedException();

        public Grid Grid => throw new NotImplementedException();

        public event EventHandler OnDestroy = null;
        public event EventHandler OnReady = null;

        event ComponentEvent MapComponent.OnDestroy
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }


        public void Ready()
        {
            throw new NotImplementedException();
        }

        public void RegisterObject(MapObject obj)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "MockChunk @ " + _chunkCoords;
        }
    }

    internal class MockChunkComponentFactory : ComponentFactory
    {
        public Chunk CreateChunk(Point chunkCoords)
        {
            return new MockChunk(chunkCoords);
        }

        public MapObject CreateObject(Point position, Entity entity)
        {
            throw new NotImplementedException();
        }

        public bool IsValidPosition(Point position, Entity entity)
        {
            throw new NotImplementedException();
        }

        public bool IsValidPosition(Rectangle bounds, bool hasCollision)
        {
            throw new NotImplementedException();
        }
    }

}
