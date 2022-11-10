using CrystalCore.Model.Elements;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{
    internal class Port : IDestroyable
    {

        // location.
        private CompassPoint _facing; // the direction this port is facing. if it were on the top of an agent, it is facing up/north.
                                      // This direction is relative to the direction our agent is pointing.
        private int ID; // the id of this port (per facing direction)

        // context 
        private Agent _parent; // the agent we are part of.


        private Connection connection;

        private int transmitting;


        // events
        public event EventHandler OnValueChange;
        public event EventHandler OnConnect;
      
        public bool Destroyed => ((IDestroyable)_parent).Destroyed;


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
                if(connection == null)
                {
                    throw new InvalidOperationException("UH oh it bork bork - that was unprofessional of me, sorry. My porthole has been empty for a while.");
                }
                return connection.Receive(this);
            }
        }

        internal Port(CompassPoint facing, int ID, Agent parent)
        {

            _facing = facing;
            this.ID = ID;
            _parent = parent;
            transmitting = 0;

        }

        public event EventHandler OnDestroy
        {
            add
            {
                _parent.OnPortsDestroyed += value;
            }

            remove
            {
                _parent.OnPortsDestroyed -= value;
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
        }

        internal void ValueChange()
        {
            if(OnValueChange != null)
            {
                OnValueChange(this, new EventArgs());
            }
        }

        internal void Connect(Connection s)
        {

            if (OnConnect != null)
            {
                OnConnect(s, null);
            }

            if (connection != null)
            {
                throw new InvalidOperationException("Port is already connected. Cannot Connect a new signal.");
            }


            if (s == null)
            {
                connection = new Connection(_parent.Map, this );
                //connection.Connect(this); // signal constructor does this now

                return;
            }

            connection = s;
            connection.Connect(this);
            
        }


        internal void Disconnect()
        {
            if(connection == null)
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
            if(connection==null)
            {
                throw new InvalidOperationException("Port attempted to transmit before connection was established.");
            }
            connection.Transmit(this, value);
            transmitting = value;
        }
    }
}
