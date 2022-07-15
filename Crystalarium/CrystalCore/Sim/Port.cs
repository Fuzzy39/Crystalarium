using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Sim
{
    internal enum PortStatus
    {
        inactive,
        receiving,
        transmitting
    }


    internal class Port
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
        private PortStatus _status; // the status this port is in.
        private Signal _boundTo; // the signal this port is bound to.


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

        public Signal Binder
        {
            get => _boundTo;
        }

        public bool IsBinded
        {
            get => _boundTo != null;
        }

        public bool IsActive
        {
            get
            {
                if (!IsBinded)
                {
                    return false;
                }

                return _boundTo.Value >= _threshold;
            }
        }

        public int Value
        {
            get
            {
                if(!IsBinded)
                {
                    return 0;
                }

                return _boundTo.Value;
            }
        }

        public CompassPoint AbsoluteFacing
        {
            get
            {
                CompassPoint toReturn = _facing;
                for (Direction i = _parent.Facing; i != Direction.up; i = i.Rotate(RotationalDirection.clockwise))
                {
                    toReturn = toReturn.Rotate(RotationalDirection.clockwise);
                    toReturn= toReturn.Rotate(RotationalDirection.clockwise);
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

                int x = anchor.X;
                int y = anchor.Y;
                switch(facing)
                {
                    case Direction.up:
                        x += ID;
                        break;
                    case Direction.down:
                        x += ID;
                        y += _parent.Bounds.Height;
                        break;
                    case Direction.left:
                        y += ID;
                        break;
                    case Direction.right:
                        y += ID;
                        y += _parent.Bounds.Width;
                        break;
                }

                return new Point(x, y);
               

                // now we are facing our actual facing direction, instead of relative.
                // now for position


            }
        }
    

        internal Port(CompassPoint facing, int ID, Agent parent)
        {
            _facing = facing;
            this.ID = ID;
            this._parent = parent;

            // defaults
            _threshold = 1;
            _boundTo = null;
            _status = PortStatus.inactive;
 

        }

        // create and transmit a signal. Returns weather it successfully has done so.

        public bool Transmit(int value)
        {
            if (Status == PortStatus.receiving)
            {
                // it is not possible to transmit if we are reveiving.
                return false;

            }

            if (Status == PortStatus.transmitting)
            {
                // we may, however, overpower our own transmissions.
                if (value <= _boundTo.Value)
                {
                    return false;
                }

                this.Stop();
            }

            // something like this:
            Signal s = new Signal();
            _boundTo = s;
            _status = PortStatus.transmitting;
            return true;

        }

        public bool Transmit()
        {
            return Transmit(_threshold);
        }


        public bool Receive(Signal s)
        {
            // if nothing else, by the end of this, I'll be able to spell receive.
            if (Status != PortStatus.inactive)
            {
                return false;
            }

            _boundTo = s;
            _status = PortStatus.receiving;
            return true;
        }

        // stop transmitting or receiving.
        public void Stop()
        {
            if(Status == PortStatus.receiving)
            {
                _boundTo = null;
                _status = PortStatus.inactive;
                return;
            }

            if(Status == PortStatus.transmitting)
            {
                // this is the only time a port commands a signal. signals require a transmitter to exist.
                _boundTo.Destroy();
                _boundTo = null;
                _status = PortStatus.inactive;
                return;
            }

        }

          


    }
}

