﻿using CrystalCore.Model.Elements;
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


        private Signal connection;


        // events
        public event EventHandler OnStartReiceving;
        public event EventHandler OnStopReiceving;
        public event EventHandler OnDestroy;

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
                return connection.Receive(this);
            }
        }

        internal Port(CompassPoint facing, int ID, Agent parent)
        {

            _facing = facing;
            this.ID = ID;
            _parent = parent;
  
        }


        public void Destroy()
        {
            if (OnDestroy != null)
            {
                OnDestroy(this, new EventArgs());
            }
        }

        internal void SetupConnection(Signal s)
        {
            if(connection != null)
            {
                throw new InvalidOperationException();
            }


            if (s == null)
            {
                connection = new Signal(_parent.Map, this);
                return;
            }

            connection = s;

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
        }
    }
}
