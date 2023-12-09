using CrystalCore.Model.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Communication
{
    internal delegate void ConnectionEventHandler(Connection con, Connection.EventArgs e);

    internal interface Connection : Entity
    {

        internal class EventArgs : System.EventArgs
        {
            public EventArgs(bool portAUpdated) 
            { 
                PortAUpdated = portAUpdated;
            }

            public readonly bool PortAUpdated;
        }


        public Port PortA { get; }

        public Port PortB { get; }

        public int FromA { get; }

        public int FromB { get; }


        public event ConnectionEventHandler OnValuesUpdated;

        public bool IsPortA(Port p);
        
        public Port OtherPort(Port port);

        public void Transmit(Port from, int value);

        public void Disconnect(Port toDisconnect);


        /// <summary>
        /// Checks the ports this Connection is connected to are still valid. If not, connects to the new ports as needed.
        /// If the connection has no valid ports, it may destroy itself.
        /// </summary>
        public void Update();

        public string? ToString()
        {
            return "Connection: { A:" + (PortA == null ? "null" : PortA.Location.ToString()) + " B: " + (PortB == null ? "null" : PortB.Location.ToString()) 
                + " Bounds:" + Physical.Bounds + "}";

        }

    }

        
}
