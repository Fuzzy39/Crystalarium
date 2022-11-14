using CrystalCore.Model.Elements;
using CrystalCore.Model.Rules;
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
        private Map map;

        private Connection connection;

        private int transmitting;

        private Pathfinder pathfinder;


     
        

        public Agent Parent
        {
            get{ return _parent;}
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

        public bool Destroyed => ((IDestroyable)_parent).Destroyed;

        internal Port(CompassPoint facing, int ID, Agent parent)
        {
          
            _facing = facing;
            this.ID = ID;
            _parent = parent;
            transmitting = 0;
            pathfinder = new Pathfinder(this, parent.Map);
            parent.Map.OnResize += OnMapResize;
            map = parent.Map;

        }
        
        private void OnMapResize(object sender, EventArgs e)
        {
            // frankly, we no longer care.
            if(Destroyed)
            {
                ((Map)sender).OnResize -= OnMapResize;
                return;
            }

            if(ConnectedTo == null)
            {
                Update();
            }
        }


        public event EventHandler OnDestroy
        {
            add
            {
                ((IDestroyable)_parent).OnDestroy += value;
            }

            remove
            {
                ((IDestroyable)_parent).OnDestroy -= value;
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
            if(connection!= null)
            {
                connection.Destroy();
            }

            pathfinder.Destroy();
            pathfinder = null;
           
            map.OnResize -= OnMapResize;

        }

        public void Update()
        {
            if(Destroyed)
            {
                throw new InvalidOperationException("Can't update me if I'm dead!");
            }
            Ruleset r = Parent.Type.Ruleset;
            pathfinder.FindPath(r.SignalMinLength, r.SignalMaxLength, connection);
        }
     

        internal void Connect(Connection s)
        {

            if (connection != null && !connection.Destroyed)
            {
                throw new InvalidOperationException("Port '"+ this + "' is already connected. Cannot Connect a new signal.");
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
            if(connection == null)
            {
                throw new InvalidOperationException("Port already disconnected, no action needed.");
            }
           
            connection = null;
        }

        internal void DestroyConnection()
        {
            if (connection == null)
            {
                throw new InvalidOperationException("No connection, can't destroy.");
            }

            if (ConnectedTo != null)
            {
                throw new InvalidOperationException("A connection cannot be destroyed because it is still connecting two ports.");
            }

            connection.Destroy();
            //connection = null;
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
            transmitting = value;
            if(ConnectedTo != null)
            {
                ConnectedTo.Parent.StatusChanged();
            }
        }
    }
}
