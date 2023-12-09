using CrystalCore.Util;
using System;

namespace CrystalCore.Model.Rules
{


    // relative portID.
  


    public struct PortTransmission
    {
        public PortID portID;
        public int value;

        public PortTransmission(int value, PortID pid)
        {
            portID = pid;
            this.value = value;
        }

        public PortTransmission(int value, int portID, CompassPoint compassPoint) : this(value, new PortID(portID, compassPoint))
        {


        }
    }
}
