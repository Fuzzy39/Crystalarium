using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Communication
{

    internal enum PortStatus
    {
        inactive,
        receiving,
        transmitting,
        transceiving
    }

    public enum PortChannelMode
    {
        halfDuplex,
        fullDuplex
    }

    internal struct PortIdentifier
    {
    }
}
