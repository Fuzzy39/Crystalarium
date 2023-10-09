using CrystalCore.Model.Core;
using CrystalCore.Model.ObjectContract;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Simulation
{
    public class Port : MapComponent
    {

        // location.
        private CompassPoint _facing; // the direction this port is facing. if it were on the top of an agent, it is facing up/north.
                                      // This direction is relative to the direction our agent is pointing.
        private int _ID; // the id of this port (per facing direction)

        // context 
        private Agent _parent; // the agent we are part of.
        private Map map;

        private Connection connection;

        private int transmitting;

        private Raycaster pathfinder;



        public CompassPoint Facing
        {
            get { return _facing; }
        }

        public int ID
        {
            get { return _ID; }
        }

        public Agent Parent
        {
            get { return _parent; }
        }

        public CompassPoint AbsoluteFacing
        {
            get
            {

                CompassPoint toReturn = _facing;
                for (Direction i = _parent.Facing; i != Direction.up; i = i.Rotate(RotationalDirection.cw))
                {
                    toReturn = toReturn.Rotate(RotationalDirection.ccw);
                    toReturn = toReturn.Rotate(RotationalDirection.ccw);
                }
                return toReturn;
            }
        }

        public bool HasConnection
        {
            get
            {
                return connection != null;
            }
        }

        public Port ConnectedTo
        {
            get
            {
                if (connection == null)
                {
                    return null;
                }
                return connection.Other(this);
            }
        }

        public int TransmittingValue
        {
            get
            {
                return transmitting;
            }
        }


        public Point Location
        {
            get
            {
                Point anchor = _parent.Bounds.Location;
                // get actual facing direction
                Direction? d = AbsoluteFacing.ToDirection();
                if (d == null)
                {
                    //return anchor; // diagonal ports means that the agent is 1x1.
                    switch (AbsoluteFacing)
                    {
                        case CompassPoint.northwest:
                            return anchor;
                        case CompassPoint.southwest:
                            return anchor + new Point(0, _parent.Bounds.Height - 1);
                        case CompassPoint.northeast:
                            return anchor + new Point(_parent.Bounds.Width - 1, 0);
                        case CompassPoint.southeast:
                            return anchor - new Point(1) + _parent.Bounds.Size;


                    }
                    throw new InvalidOperationException("Failed to interpret compass direction to location.");

                }

                Direction facing = (Direction)d;



                int x = anchor.X + DetermineRelX(facing);
                int y = anchor.Y + DetermineRelY(facing);
                return new Point(x, y);


                // now we are facing our actual facing direction, instead of relative.
                // now for position


            }
        }

        public int Value
        {
            get
            {
                if (connection == null)
                {
                    throw new InvalidOperationException("UH oh it bork bork - that was unprofessional of me, sorry. My porthole has been empty for a while.");
                }
                return connection.Receive(this);
            }
        }

        public bool Destroyed => ((MapComponent)_parent).Destroyed;

        internal Port(CompassPoint facing, int ID, Agent parent)
        {

            _facing = facing;
            _ID = ID;
            _parent = parent;
            transmitting = 0;
            pathfinder = new Raycaster(this, parent.Map);
            parent.Map.OnResize += OnMapResize;
            map = parent.Map;

        }

        private void OnMapResize(object sender, EventArgs e)
        {

            if (sender == null)
            {
                throw new ArgumentNullException("map was null...");
            }

            // frankly, we no longer care.
            if (Destroyed)
            {
                ((Map)sender).OnResize -= OnMapResize;
                return;
            }

            if (ConnectedTo == null)
            {
                Update();
            }
        }


        public event EventHandler OnDestroy
        {
            add
            {
                ((MapComponent)_parent).OnDestroy += value;
            }

            remove
            {
                ((MapComponent)_parent).OnDestroy -= value;
            }
        }



        // tostring
        public override string ToString()
        {

            return "Port: { Location:" + Location + " ID: " + ID + ", Facing: " + _facing + " (ABS):" + AbsoluteFacing + "}";
        }


        public void Destroy()
        {
            // does nothing?
            if (HasConnection)
            {
                DestroyConnection();
            }
            pathfinder.Destroy();
            pathfinder = null;

            map.OnResize -= OnMapResize;

        }

        public void Update()
        {
            if (Destroyed)
            {
                throw new InvalidOperationException("Can't update me if I'm dead!");
            }



            Ruleset r = Parent.Type.Ruleset;


            // this warning is probably a sign that this stuff is not implemented super well...
            pathfinder.UpdateConnection(r.SignalMinLength, r.SignalMaxLength, connection);
        }

        internal void DestroyConnection()
        {
            if (connection == null)
            {
                return;
            }

            connection.Destroy();
            connection = null;
        }

        internal void Connect(Connection s)
        {

            if (connection != null && !connection.Destroyed)
            {
                throw new InvalidOperationException("Port '" + this + "' is already connected. Cannot Connect a new signal.");
            }


            if (s == null)
            {


                Update();
                return;
            }

            connection = s;


        }


        internal void Disconnect()
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Port already disconnected, no action needed.");
            }

            connection = null;
        }


        private int DetermineRelX(Direction facing)
        {
            // facing is absolute here.
            int x = 0;
            if (facing.IsVertical())
            {
                if (_parent.Facing == Direction.up || _parent.Facing == Direction.left)
                {
                    x += ID;
                }
                else
                {
                    x += _parent.Bounds.Width - 1 - ID;
                }
            }

            if (facing == Direction.right)
            {

                x += _parent.Bounds.Width - 1;
            }

            return x;
        }


        private int DetermineRelY(Direction facing)
        {
            int y = 0;
            if (facing.IsHorizontal())
            {
                if (_parent.Facing == Direction.up || _parent.Facing == Direction.right)
                {
                    y += ID;
                }
                else
                {
                    y += _parent.Bounds.Height - 1 - ID;
                }
            }

            if (facing == Direction.down)
            {
                y += _parent.Bounds.Height - 1;
            }

            return y;
        }


        // transmit a signal. 
        public void Transmit(int value)
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Port attempted to transmit before connection was established.");
            }
            transmitting = value;
            if (ConnectedTo != null)
            {
                ConnectedTo.Parent.StatusChanged();
            }
        }
    }
}
