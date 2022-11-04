using CrystalCore.Model.Rules;
using CrystalCore.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{
    /// <summary>
    ///  A port interface deals with all the dirty work of managing ports that an agent doesn't want to do.
    /// </summary>
    internal class PortInterface
    {


        private AgentType type;
        private Agent parent;

        private bool statusChanged; // whether a port started/stopped receiving this step.
        private bool statusHadChanged; // whether a port started/stopped receiging last step.

        private List<List<Port>> _ports; // this agent's ports, stored by direction relative to the agent.
        private List<List<int>> _stalePortValues; // the value each port was receiving at the end of the last simulation step.

        internal List<List<Port>> Ports { get { return _ports; } }
        internal List<Port> PortList
        {
            get
            {
                List<Port> toReturn = new List<Port>();
                foreach (List<Port> ports in Ports)
                {
                    toReturn.AddRange(ports);
                }

                return toReturn;
            }

        }

        internal bool StatusHadChanged {  get { return statusHadChanged; } }


        internal PortInterface(AgentType at, Agent parent)
        {
            statusChanged = true; // this is true at initialization so the agent can do things of it's own accord when it is created

            type = at;
            this.parent = parent;

            // create ports (what a helpful comment)
            CreatePorts();
        }


        // PORT RELATED
        private void CreatePorts()
        {
            _ports = new List<List<Port>>();


            // should always be 8, but who knows, maybe they'll invent more directions on a compass.
            int length = Enum.GetNames(typeof(CompassPoint)).Length;

            for (int i = 0; i < length; i++)
            {
                List<Port> portList = new List<Port>();
                List<int> staleValues = new List<int>();

                // add relevant ports to this list.
                CompassPoint facing = (CompassPoint)i;

                if (facing.IsDiagonal())
                {
                    _ports.Add(portList);

                    if (!type.Ruleset.DiagonalSignalsAllowed)
                    {
                        continue;
                    }

                    // create a diagonal port.
                    Port p = new FullPort(facing, 0, parent);
                    portList.Add(p);
                    p.OnStartReiceving += OnPortStatusChanged;
                    p.OnStopReiceving += OnPortStatusChanged;

                    continue;
                }

                portList = CreateOrthagonalPorts(facing, portList);
                _ports.Add(portList);

            }

            CreateStaleVals();
        }



        // facing is direction of ports to be made relative to upwards face of agent
        private List<Port> CreateOrthagonalPorts(CompassPoint facing, List<Port> ports)
        {
            // create orthagonal ports. Vertical first, diagonal after.

            if (((Direction)facing.ToDirection()).IsVertical())
            {
                for (int j = 0; j < type.Size.X; j++)
                {
                    Port p = new FullPort(facing, j, parent);
                    ports.Add(p);
                    p.OnStartReiceving += OnPortStatusChanged;
                    p.OnStopReiceving += OnPortStatusChanged;
                }
                return ports;
            }

            for (int j = 0; j < type.Size.Y; j++)
            {
                Port p = new FullPort(facing, j, parent);
                ports.Add(p);
                p.OnStartReiceving += OnPortStatusChanged;
                p.OnStopReiceving += OnPortStatusChanged;
            }

            return ports;

        }

        private void CreateStaleVals()
        {
            _stalePortValues = new List<List<int>>();
            foreach (List<Port> list in _ports)
            {
                List<int> staleList = new List<int>();
                foreach (Port port in list)
                {
                    staleList.Add(0);
                }
                _stalePortValues.Add(staleList);
            }
        }

        public void Destroy()
        {

            foreach (List<Port> ports in _ports)
            {
                foreach (Port port in ports)
                {
                    port.Destroy();

                }
            }

        }


        private void OnPortStatusChanged(Object Sender, EventArgs e)
        {

            StatusChanged();
        }

        internal void StatusChanged()
        {
            statusChanged = true;
        }

        internal void PreserveState()
        {
            statusHadChanged = statusChanged;
            statusChanged = false;
            if (!statusHadChanged)
            {
                return;
            }

            PreserveValues();
        }


        protected void PreserveValues()
        {
            // loop through all directions
            for (int i = 0; i < _ports.Count; i++)
            {
                List<Port> list = _ports[i];

                // and every port in that direction...
                for (int j = 0; j < list.Count; j++)
                {
                    // and update its stale value.
                    Port p = list[j];
                    if (p.Status == PortStatus.receiving)
                    {
                        _stalePortValues[i][j] = p.ReceivingSignal.Value;
                        continue;
                    }

                    _stalePortValues[i][j] = 0;


                }
            }
        }

        internal void OnlyTransmitOn(List<Port> ports, PortTransmission[] transmits)
        {
            

            List<Port> toTurnOff = new List<Port>(PortList);
            for(int i = 0; i<ports.Count; i++)
            {
                toTurnOff.Remove(ports[i]);
                ports[i].Transmit(transmits[i].value);
            }

            foreach(Port p in toTurnOff)
            {
                p.StopTransmitting();
            }

        }
    }
}
