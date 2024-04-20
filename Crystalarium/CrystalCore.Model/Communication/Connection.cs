using CrystalCore.Model.Physical;

namespace CrystalCore.Model.Communication
{
    internal delegate void ConnectionEventHandler(Connection con, Connection.EventArgs e);

    public interface Connection : Entity
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


        internal event ConnectionEventHandler OnValuesUpdated;

        public bool IsPortA(Port p);

        public Port OtherPort(Port port);

        public void Transmit(Port from, int value);


        /// <summary>
        /// Note that disconnect does not ensure that the connection is valid. Make sure that Update is called.
        /// </summary>
        /// <param name="toDisconnect"></param>
        public void Disconnect(Port toDisconnect);


        /// <summary>
        /// Checks the ports this Connection is connected to are still valid. If not, connects to the new ports as needed.
        /// If the connection has no valid ports, it may destroy itself.
        /// </summary>
        public void Update();

    

    }


}
