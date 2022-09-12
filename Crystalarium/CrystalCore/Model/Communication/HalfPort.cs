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
      

        /// <summary>
        /// Whether the port is allowed to transmit.
        /// </summary>
      

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


        public override int TransmittingValue
        {
            get
            {
                return Status==PortStatus.transmitting ? _boundTo.Value : 0;
            }
        }

        public override Signal ReceivingSignal
        {
            get
            {
                if(Status!=PortStatus.receiving)
                {
                    throw new InvalidOperationException("This Port is not receiving.");
                }
                return _boundTo;
            }
        }

        public override Signal TransmittingSignal
        {
            get
            {
                if(Status!=PortStatus.transmitting)
                {
                    throw new InvalidOperationException("Cannot obtain transmitting signal from a port that isn't transmitting.");
                }
                return _boundTo;
            }
        }

        public HalfPort(CompassPoint facing, int ID, Agent parent) : base(facing, ID, parent)
        {
            _boundTo = null;
        }

        public override void Receive(Signal s)
        {
            // if nothing else, by the end of this, I'll be able to spell receive.
            if (Status == PortStatus.transmitting)
            {
                
                StopTransmitting();
               
            }

            if(s == null)
            {
                throw new ArgumentNullException();
            }

            _boundTo = s;
            _status = PortStatus.receiving;

            base.Receive(s);
            
        }

        public void Transmit()
        {
            Transmit(1);
        }


        public override void Transmit(int value)
        {
            if (Status == PortStatus.receiving)
            {
                // it is not possible to transmit if we are reveiving.
                //throw new InvalidOperationException("HalfPorts cannot Transmit while receiving.");
                return; // failed
            }

            if (Status == PortStatus.transmitting)
            {
                // we may, however, overpower our own transmissions.
                if(value == TransmittingValue)
                {
                    return;
                }
                StopTransmitting();
                
            }

            // something like this:
            Signal s = Parent.Type.Ruleset.CreateSignal(Parent.Grid, this, value);
            _boundTo = s;
            _status = PortStatus.transmitting;
         

        }




        public override void StopReceiving()
        {
            if( Status != PortStatus.receiving )
            {
                return;
            }

            _boundTo.Reset();
            _boundTo = null;
            _status = PortStatus.inactive;

            base.StopReceiving();
        }

        public override void StopTransmitting()
        {
            
            if(Status != PortStatus.transmitting)
            {
                return;
            }
            // this is the only time a port commands a signal. signals require a transmitter to exist.
            _status = PortStatus.inactive;
            _boundTo.Destroy();
            
            _boundTo = null;
            
           
        }


    }
}
