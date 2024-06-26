﻿using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
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


        public MapObject FindClosestObjectInDirection_result;
        public Point FindClosestObjectInDirection_locationIn;
        public Point FindClosestObjectInDirection_locationOut;
    

        public MapObject FindClosestObjectInDirection(ref Point location, CompassPoint direction)
        {
            FindClosestObjectInDirection_locationIn = location;
            location = FindClosestObjectInDirection_locationOut;
            return FindClosestObjectInDirection_result;
        }



        public List<MapObject> ObjectsIntersecting_result = new();
        public Rectangle ObjectsIntersecting_bounds;

        public List<MapObject> ObjectsIntersecting(Rectangle bounds)
        {
            ObjectsIntersecting_bounds = bounds;
            return ObjectsIntersecting_result;
        }

        public Point TileToChunkCoords(Point tileCoords)
        {
            throw new NotImplementedException();
        }
    }


    internal class MockMap : Map
    {
        // could cause issues, be careful with that.
        public Grid Grid => new MockGrid();

        public Ruleset Ruleset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int AgentCount => throw new NotImplementedException();

        public event ComponentEvent? OnMapComponentDestroyed;
        public event MapObjectEvent? OnMapObjectReady;
        public event EventHandler? OnReset;
        public event ComponentEvent? OnMapComponentReady;

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


        Agent Map.CreateAgent(AgentType at, Point location, Direction facing)
        {
            throw new NotImplementedException();
        }

        public void Step()
        {
            throw new NotImplementedException();
        }

        public bool IsValidPosition(AgentType at, Point location, Direction facing)
        {
            throw new NotImplementedException();
        }

        public bool IsValidPosition(Rectangle bounds)
        {
            throw new NotImplementedException();
        }

        public List<Agent> AgentsWithin(Rectangle bounds)
        {
            throw new NotImplementedException();
        }

        public Agent getAgentAtPos(Point coords)
        {
            throw new NotImplementedException();
        }

        void Map.OnComponentReady(MapComponent mapObj, EventArgs e)
        {
            return;
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
        Grid _grid;
        public MockMapObj(Grid g, Rectangle b)
        {
            _grid = g;
            _bounds = b;
        }
        public Rectangle Bounds => _bounds;

        public Entity Entity { get; set; }

        public Chunk Parent => throw new NotImplementedException();

        public Grid Grid => _grid;

        public bool Destroyed => _destroyed;

        public event ComponentEvent OnDestroy;

        public void Destroy()
        {
            _destroyed = true;
            OnDestroy?.Invoke(this, new EventArgs());

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
        Map ComponentFactory.Map => throw new NotImplementedException();

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
