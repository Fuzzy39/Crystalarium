using CrystalCore.Model.Communication;
using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using CrystalCoreTests.Model.DefaultCore;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        Map ComponentFactory.Map => throw new NotImplementedException();

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
        private PortDescriptor _discriptor;

        public MockPort(Point loc, CompassPoint absFacing)
        {
            this.absFacing = absFacing;
            location = loc;
        }


        public MockPort(PortDescriptor desc)
        {
            _discriptor= desc;
        }
        public PortDescriptor Descriptor => _discriptor;

        public CompassPoint AbsoluteFacing => absFacing;

        public Point Location => location;

        public Connection Connection { get => _connection; set => _connection = value; }

        public Port ConnectedTo => Connection == null ? null : Connection.OtherPort(this);
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

        Agent Node.Agent { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } } 

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

    internal class MockConnection : Connection
    {
        public int FromA => throw new NotImplementedException();

        public int FromB => throw new NotImplementedException();

        public MapObject Physical => throw new NotImplementedException();

        public bool HasCollision => throw new NotImplementedException();

        public Point Size => throw new NotImplementedException();

        public bool Destroyed => throw new NotImplementedException();

        Port Connection.PortA => throw new NotImplementedException();

        Port Connection.PortB => throw new NotImplementedException();

        event ConnectionEventHandler Connection.OnValuesUpdated
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

        public int timesUpdated = 0;

        public void Update()
        {
            timesUpdated++;
        }

        void Connection.Disconnect(Port toDisconnect)
        {
            throw new NotImplementedException();
        }

        bool Connection.IsPortA(Port p)
        {
            throw new NotImplementedException();
        }

        Port Connection.OtherPort(Port port)
        {
            throw new NotImplementedException();
        }

        void Connection.Transmit(Port from, int value)
        {
            throw new NotImplementedException();
        }
    }


    internal class MockEntityFactory : EntityFactory
    {
        private ComponentFactory compFact;
        public ComponentFactory baseFactory => compFact;

        public MockEntityFactory(MockGrid g)
        {
            compFact = new MockMapObjectFactory(g);
        }

        Connection EntityFactory.CreateConnection(Port initial)
        {
            return new MockConnection();
        }

        Node EntityFactory.CreateNode(Rectangle bounds, Direction facing, bool createDiagonalPorts)
        {
            throw new NotImplementedException();
        }

        Port EntityFactory.CreatePort(PortDescriptor descriptor, Direction parentRotation, Rectangle parentBounds)
        {
            return new MockPort(descriptor);
        }
    }

}
