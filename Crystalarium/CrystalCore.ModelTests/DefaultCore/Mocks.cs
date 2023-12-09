using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCoreTests.Model.DefaultCore
{

    internal class MockGrid : Grid
    {

        public Chunk _chunk;

        public MockGrid()
        {
            _chunk = new MockChunk(new(0));
        }

        public List<List<Chunk>> Chunks => throw new NotImplementedException();

        public List<Chunk> ChunkList => throw new NotImplementedException();

        public Point ChunkOrigin => throw new NotImplementedException();

        public Point ChunkSize => throw new NotImplementedException();

        public Rectangle Bounds => throw new NotImplementedException();

        public ComponentFactory ComponentFactory => throw new NotImplementedException();

        public event EventHandler? OnResize;

        public Chunk ChunkAtCoords(Point tileCoord)
        {
            throw new NotImplementedException();
        }

        public List<Chunk> ChunksIntersecting(Rectangle bounds)
        {
            List<Chunk> toReturn = new()
            {
                _chunk
            };

            return toReturn;
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Expand(Direction d)
        {
            throw new NotImplementedException();
        }

        public void ExpandToFit(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public List<MapObject> ObjectsIntersecting(Rectangle bounds)
        {
            throw new NotImplementedException();
        }

        public Point TileToChunkCoords(Point tileCoords)
        {
            throw new NotImplementedException();
        }
    }


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

        public MapObject Physical => throw new NotImplementedException();

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

        public List<MapObject> _calledRegister;

        public MockChunk(Point chunkCoords)
        {
            _chunkCoords = chunkCoords;
            _calledRegister = new List<MapObject>();    
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
           _calledRegister.Add(obj);
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
