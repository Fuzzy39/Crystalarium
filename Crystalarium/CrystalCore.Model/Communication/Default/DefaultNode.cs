using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Communication.Default
{
    internal class DefaultNode : Node, Entity
    {
        private Agent _agent;
        private List<List<Port>> _ports;
        private List<List<int>> _stablePortVals;
        private bool _changedThisStep;
        private bool _changedLastStep;



        private Direction _facing;
        private Point _size;
        private MapObject _physical;
        
        private bool _destroyed;

        private EntityFactory _factory;



        public DefaultNode(Agent agent, EntityFactory factory, Rectangle bounds, Direction facing, bool createDiagonalPorts)
        {

            _agent = agent;
            _destroyed = false;
            _size = bounds.Size;
            _changedLastStep = true;
            _changedThisStep = true; // does this need to be true?
            _facing = facing;
            _factory = factory;

            _physical = factory.baseFactory.CreateObject(bounds.Location, this);
            InitPorts(createDiagonalPorts);
            OnCreate();
            

        }


        private void InitPorts(bool createDiagonalPorts)
        {

            _ports = new();
            _stablePortVals = new();

            for (int i = 0; i < Enum.GetNames(typeof(CompassPoint)).Length; i++)
            {
                _ports.Add(new());
                _stablePortVals.Add(new());

                CompassPoint portFacing = (CompassPoint)i;


                for (int j = 0; j < PortsInDirection(portFacing, createDiagonalPorts); j++)
                {
                    PortDescriptor pd = new(j, portFacing);
                    Port toAdd = _factory.CreatePort(pd, _facing, _physical.Bounds);
                    _ports[i].Add(toAdd);
                    _stablePortVals[i].Add(0);
                    toAdd.OnInputUpdated += OnPortUpdated;
                }

            }

        }

        private int PortsInDirection(CompassPoint portFacing, bool createDiagonalPorts)
        {
            if (portFacing.IsDiagonal())
            {
                if (createDiagonalPorts)
                {
                    return 1;
                }

                return 0;
            }

            Direction facing = (Direction)portFacing.ToDirection();

            if (facing.IsVertical())
            {
                return _size.Y;
            }

            return _size.X;

        }


        public Agent Agent => _agent;
        public List<List<Port>> Ports => _ports;

        public List<Port> PortList
        {
            get
            {
                List<Port> toReturn = new();
                Ports.ForEach(portlist => toReturn.AddRange(portlist));
                return toReturn;
            }
        }


        public bool ChangedLastStep => _changedLastStep;

        public Direction Facing => _facing;

        public MapObject Physical => _physical;

        public bool HasCollision => true;

        public Point Size => _size;

        public bool Destroyed => _destroyed;






        public Port GetPort(CompassPoint facing, Point location)
        {

            location -= _physical.Bounds.Location;


            if (facing.IsDiagonal() && location.Equals(new(0)))
            {
                return GetPort(new PortDescriptor(0, facing));
            }

            // now we have to get creative.

            // I mean, not very, this should be simple enough.


            // check that we have a port at the location
            if (location.X != 0 && location.X != Size.X - 1
                || location.Y != 0 && location.Y != Size.Y - 1)
            {



                throw new ArgumentException("The point (Node relative, grid orienation) [" + location + "]" +
                    " is either outside or interior node with size: [" + _size + "], and no port is associated with this location.");
            }



            Direction orthagonalFacing = (Direction)facing.ToDirection();

            if (orthagonalFacing.IsVertical())
            {
                return GetPort(new(location.Y, facing));
            }

            return GetPort(new(location.X, facing));

        }

        public Port GetPort(PortDescriptor desc)
        {
            if (_ports[(int)desc.Facing].Count <= desc.ID)
            {
                throw new ArgumentException("Port Descriptor " + desc + " is invalid for Node of size (grid relative): " + Size);
            }

            return _ports[(int)desc.Facing][desc.ID];
        }



        public int GetStablePortValue(PortDescriptor desc)
        {
            if (_ports[(int)desc.Facing].Count <= desc.ID)
            {
                throw new ArgumentException("Port Descriptor " + desc + " is invalid for Node of size (grid relative): " + Size);
            }

            return _stablePortVals[(int)desc.Facing][desc.ID];
        }


        public void OnPortUpdated(object port, EventArgs e)
        {
            _changedThisStep = true;
        }

        public void DoSimulationStep()
        {
            _changedLastStep = _changedThisStep;
            _changedThisStep = false;

            if (!_changedLastStep)
            {
                return;
            }

            int i = 0;
            foreach (List<Port> Ports in _ports)
            {
                int j = 0;
                foreach (Port Port in Ports)
                {
                    _stablePortVals[i][j] = Port.Input;
                    j++;
                }

                i++;
            }
        }


        // These methods ensure that connections are valid and that everthing is good and happy.

        private void OnCreate()
        {
            // find any connections intersecting our bounds
            List<Connection> connections =
                _physical.Grid.ObjectsIntersecting(_physical.Bounds).Where(obj => obj is Connection).Cast<Connection>().ToList();

            foreach (Connection Connection in connections)
            {
                Connection.Update();
            }

            foreach (Port p in PortList)
            {
                if (p.Connection == null)
                {
                    p.Connection = _factory.CreateConnection(p);
                }
            }
        }

        public void Rotate(RotationalDirection direction)
        {
            if (_size.X == _size.Y)
            {
                _facing = _facing.Rotate(direction);
            }
            else
            {
                // rectangles can only rotate 180 
                _facing = _facing.Opposite();
            }

            bool diagonalSignalsAllowed = _ports[(int)CompassPoint.northwest].Count()>0;
            PortList.ForEach(p => p.Destroy());

            InitPorts(diagonalSignalsAllowed);
            OnCreate();

        }

        public void Destroy()
        {
            _destroyed = true;

            _physical.Destroy();
            _physical = null;
            List<Connection> connections = new();
            foreach(Port p in  PortList) 
            { 
                connections.Add(p.Connection);
                p.Destroy();
            }

            _ports = null;

            foreach (Connection conn in connections)
            {
                conn.Update();
            }

            _stablePortVals = null;
            _agent = null;
            _factory = null;
            
            
        }
        public void OnGridResize(object sender, EventArgs e)
        {
            // LINQ is neat.
            List<Connection> voidConnections = new();
            PortList.ForEach(p => { if (p.Connection.OtherPort(p) == null) voidConnections.Add(p.Connection); });
            voidConnections.ForEach(conn => conn.Update());

            // this is impentatrable, though.
        }





    }
}
