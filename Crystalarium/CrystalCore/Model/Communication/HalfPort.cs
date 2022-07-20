using CrystalCore.Model.Objects;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{
    internal class HalfPort: Port
    {

        /*
         * A HalfPort is a Port that is only capable of Half-Duplex Communication.
         */

        private Signal _boundTo;


        public override bool IsActive
        {
            get
            {
                if (!IsBinded)
                {
                    return false;
                }

                return _boundTo.Value >= Threshold;
            }
        }

        private bool IsBinded
        {
            get 
            { 
                return _boundTo != null;
            }
        }


        public int Value
        {
            get
            {
                if (!IsBinded)
                {
                    return 0;
                }

                return _boundTo.Value;
            }

        }



        public HalfPort(CompassPoint facing, int ID, Agent parent) : base(facing, ID, parent)
        {
            _boundTo = null;
        }

        public override bool Receive(Signal s)
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

        public bool Transmit()
        {
            return Transmit(Threshold);
        }


        public override bool Transmit(int value)
        {
            if (Status == PortStatus.receiving)
            {
                // it is not possible to transmit if we are reveiving.
                return false;

            }

            if (Status == PortStatus.transmitting)
            {
                // we may, however, overpower our own transmissions.
                return false;
                
            }

            // something like this:
            Signal s = Parent.Type.Ruleset.CreateSignal(Parent.Grid, this, value);
            _boundTo = s;
            _status = PortStatus.transmitting;
            return true;

        }




        public override void StopReceiving()
        {
            if( Status != PortStatus.receiving )
            {
                return;
            }
            _boundTo = null;
            _status = PortStatus.inactive;
            
        }

        public override void StopTransmitting()
        {
            if(Status != PortStatus.transmitting)
            {
                return;
            }
            // this is the only time a port commands a signal. signals require a transmitter to exist.
            _boundTo.Destroy();
            _boundTo = null;
            _status = PortStatus.inactive;
            
        }


    }
}
