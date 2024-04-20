using CrystalCore.Model.Communication;
using CrystalCore.Util;
using System;

namespace CrystalCore.Model.Rules
{


    // relative portID.
  


    public struct PortTransmission
    {
        public PortDescriptor descriptor;
        public int value;

        public PortTransmission(int value, PortDescriptor pid)
        {
            descriptor = pid;
            this.value = value;
        }

        public PortTransmission(int value, int portID, CompassPoint compassPoint) : this(value, new (portID, compassPoint))
        {


        }
    }
}
