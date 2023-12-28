using CrystalCore.Model.Communication;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using CrystalCoreTests.Model.DefaultCore;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCoreTests.Model.DefaultCommunication
{
     class MockMapObjectFactory : ComponentFactory
    {
        public MockGrid grid;

        public MockMapObjectFactory(MockGrid g)
        {
            grid = g;
        }

        public Chunk CreateChunk(Point chunkCoords)
        {
            throw new NotImplementedException();
        }

        public MapObject CreateObject(Point position, Entity entity)
        {
            //  return new ??
            return new DefaultCore.MockMapObj(grid, new(position, entity.Size));
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

    class MockPort : Port
    {
        private Connection _connection;
        private Point location;
        private CompassPoint absFacing;

        public MockPort(Point loc, CompassPoint absFacing)
        {
            this.absFacing = absFacing;
            location = loc;
        }

        public PortDescriptor Descriptor => throw new NotImplementedException();

        public CompassPoint AbsoluteFacing => absFacing;

        public Point Location => location;

        public Connection Connection { get => _connection; set => _connection = value; }

        public Port ConnectedTo => throw new NotImplementedException();

        public bool Destroyed => throw new NotImplementedException();

        public int Output { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Input => throw new NotImplementedException();

        public event EventHandler? OnDestroy;
        public event EventHandler? OnInputUpdated;

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }


    public class MockNode : Node
    {
        public Direction Facing => throw new NotImplementedException();

        public bool ChangedLastStep => throw new NotImplementedException();

        public MapObject Physical { get; set; }

        public bool HasCollision => throw new NotImplementedException();

        public Point Size => throw new NotImplementedException();

        public bool Destroyed => throw new NotImplementedException();

        Agent Node.Agent => throw new NotImplementedException();

        List<List<Port>> Node.Ports => throw new NotImplementedException();

        List<Port> Node.PortList => throw new NotImplementedException();

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void DoSimulationStep()
        {
            throw new NotImplementedException();
        }

        public int GetStablePortValue(PortDescriptor desc)
        {
            throw new NotImplementedException();
        }

        public void OnGridResize(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnPortUpdated(object port, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Rotate(RotationalDirection direction)
        {
            throw new NotImplementedException();
        }

        internal Port toFind;
        public CompassPoint lastAbsFacingSought;
        public Point lastPortLocationSought;

        Port Node.GetPort(CompassPoint facing, Point Location)
        {
            lastAbsFacingSought = facing;
            lastPortLocationSought = Location;
            return toFind;
        }

        Port Node.GetPort(PortDescriptor desc)
        {
            throw new NotImplementedException();
        }
    }
}
