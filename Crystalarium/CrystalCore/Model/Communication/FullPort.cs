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

        public FullPort(CompassPoint facing, int ID, Agent parent) : base(facing, ID, parent)
        {
            _sending = null;
            _receiving = null;
        }

        public override bool IsActive
        {
            get
            {
                if(Status == PortStatus.transmitting || Status == PortStatus.inactive)
                {
                    return false;
                }

                return _receiving.Value >= Threshold;
               
            }
        }

        public override bool Receive(Signal s)
        {

            if (Status == PortStatus.receiving || Status == PortStatus.transceiving)
            {
                return false;
            }

            _receiving = s;

            if (Status == PortStatus.inactive)
            {
                _status = PortStatus.receiving;
                return true;
            }

            _status = PortStatus.transceiving;
            return true;


        }

        public override void StopReceiving()
        {
            if (Status != PortStatus.receiving && Status != PortStatus.transceiving)
            {
                return;
            }

            _receiving = null;


            if (Status == PortStatus.transceiving)
            {
                _status = PortStatus.transmitting;
                return;
            }

            _status = PortStatus.inactive;
        }

      

        public override bool Transmit(int value)
        {
            if(Status == PortStatus.transmitting ||Status == PortStatus.transceiving)
            {
                return false;
            }

            // otherwise, start to transmit.
            Signal s = Parent.Type.Ruleset.CreateSignal(Parent.Grid, this, value);
            _sending = s;


            if(Status==PortStatus.receiving)
            {
                _status = PortStatus.transceiving;
                return true;
            }

            _status = PortStatus.transmitting;
            return true;

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
