using CrystalCore.Model.Objects;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{
   

    internal abstract class Port
    {
        // if only, if only...
        // if only I could spell.
        // a poty is no port.

        /*
         * Anyways...
         * A port is an inert object on the face of an agent. an agent can command it to send a signal, or a signal can command it to bind to it.
         * it registers the signal it is bound to.
         * 
         * Ports do not interact with signals. Signals and Agents command ports. Agents do not interact with signals. Agents read and write to their ports.
         * Ports can create and destroy signals when they are transmitting them.
         */


        // location.
        private CompassPoint _facing; // the direction this port is facing. if it were on the top of an agent, it is facing up/north.
                                      // This direction is relative to the direction our agent is pointing.
        private int ID; // the id of this port (per facing direction)

        // context 
        private Agent _parent; // the agent we are part of.
        private int _threshold; // the value required for this port to be activated.

        // current state.
        protected PortStatus _status; // the status this port is in.
        //private Signal _boundTo; // the signal this port is bound to.


        public CompassPoint Facing
        {
            get => _facing;
        }

        public Agent Parent
        {
            get => _parent;
        }

        public int Threshold
        {
            get => _threshold;
            set => _threshold = value;
        }

        public PortStatus Status
        {
            get => _status;
        }

        // returns true if this port is receiving a signal and its value is above the port's threshold.
        public abstract bool IsActive
        {
            get;
        }

        public abstract int TransmittingValue
        {
            get;
        }


        public CompassPoint AbsoluteFacing
        {
            get
            {
                CompassPoint toReturn = _facing;
                for (Direction i = _parent.Facing; i != Direction.up; i = i.Rotate(RotationalDirection.clockwise))
                {
                    toReturn = toReturn.Rotate(RotationalDirection.counterclockwise);
                    toReturn = toReturn.Rotate(RotationalDirection.counterclockwise);
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
                    return anchor; // diagonal ports means that the agent is 1x1.
                }

                Direction facing = (Direction)d;



                int x = anchor.X+DetermineRelX(facing);
                int y = anchor.Y+DetermineRelY(facing);
                return new Point(x, y);


                // now we are facing our actual facing direction, instead of relative.
                // now for position


            }
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

            if(facing == Direction.right)
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

            if(facing == Direction.down)
            {
                y += _parent.Bounds.Height - 1;
            }

            return y;
        }


        internal Port(CompassPoint facing, int ID, Agent parent)
        {
            _facing = facing;
            this.ID = ID;
            _parent = parent;

            // defaults
            _threshold = 1;
            _status = PortStatus.inactive;


        }

        // create and transmit a signal. Returns weather it successfully has done so.
        public abstract bool Transmit(int value);

        // receive a signal. Returns weather it successfully has done so.
        public abstract bool Receive(Signal s);


        // stop transmitting or receiving.
        public abstract void StopTransmitting();
        public abstract void StopReceiving();


        // tostring
        public override string ToString()
        {
            return "Port: { Location:"+Location+" ID: " + ID + ", Facing: " + Facing + " (ABS):" + AbsoluteFacing + "}";
        }


    }
}

