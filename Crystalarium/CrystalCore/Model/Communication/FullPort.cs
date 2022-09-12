using CrystalCore.Model.Objects;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{
    internal class FullPort : Port
    {

        private Signal _sending;
        private Signal _receiving;

        public override Signal ReceivingSignal
        {
            get
            {
                if (Status != PortStatus.receiving & Status !=PortStatus.transceiving)
                {
                    throw new InvalidOperationException("This Port is not receiving.");
                }
                return _receiving;
            }
        }

        public override Signal TransmittingSignal
        {
            get
            {
                if (Status != PortStatus.transmitting & Status != PortStatus.transceiving)
                {
                    throw new InvalidOperationException("This Port is not receiving.");
                }
                return _sending;
            }
        }

        public FullPort(CompassPoint facing, int ID, Agent parent) : base(facing, ID, parent)
        {
            _sending = null;
            _receiving = null;
        }

       

        public override int TransmittingValue
        {
            get
            {
                return _sending==null?0:_sending.Value;
            }
        }

        public override void Receive(Signal s)
        {

            if (Status == PortStatus.receiving || Status == PortStatus.transceiving)
            {
                throw new InvalidOperationException("Port in incorrect state to receive.");
            }

            base.Receive(s);

            _receiving = s;

            if (Status == PortStatus.inactive)
            {
                _status = PortStatus.receiving;
                return;
            }

            _status = PortStatus.transceiving;
            
          

        }

        public override void StopReceiving()
        {
            base.StopReceiving();

            if (Status != PortStatus.receiving && Status != PortStatus.transceiving)
            {
                return;
            }
            _receiving.Reset();
            _receiving = null;


            if (Status == PortStatus.transceiving)
            {
                _status = PortStatus.transmitting;
                return;
            }

            _status = PortStatus.inactive;

         
        }

      

        public override void Transmit(int value)
        {
            if(Status == PortStatus.transmitting || Status == PortStatus.transceiving)
            {
                if (value == TransmittingValue)
                {
                    return;
                }
                
                StopTransmitting();
                 
                
            }

            // otherwise, start to transmit.
            Signal s = Parent.Type.Ruleset.CreateSignal(Parent.Grid, this, value);
            _sending = s;


            if(Status==PortStatus.receiving)
            {
                _status = PortStatus.transceiving;
                return;
            }

            _status = PortStatus.transmitting;
 

        }

        public override void StopTransmitting()
        {
            if (Status != PortStatus.transmitting && Status != PortStatus.transceiving)
            {
                return;
            }

            // this is the only time a port commands a signal. signals require a transmitter to exist.
            _sending.Destroy();
            _sending = null;

            if(Status == PortStatus.transceiving)
            {
                _status = PortStatus.receiving;
                return;
            }

            _status = PortStatus.inactive;
        }

    }
}
