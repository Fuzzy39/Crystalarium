using CrystalCore.Model.Elements;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{
    /// <summary>
    /// A PortAgent is an agent that determines its state via its ports that use beams (signals) to communicate with other agents.
    /// </summary>
    public class PortAgent:Agent
    {
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


        public PortAgent(Grid g, Point location, AgentType t, Direction facing) : base(g, new Rectangle(location, t.Size), t, facing)
        {
        }

        public override void Destroy()
        {

            List<Chunk> toUpdate = new List<Chunk>(ChunksWithin);
            Grid g = Grid;
            base.Destroy();


            foreach (List<Port> ports in _ports)
            {
                foreach (Port port in ports)
                {
                    port.StopTransmitting();
                    port.StopReceiving();

                }
            }

            g.UpdateSignals(new List<Chunk>(toUpdate));

        }


        public override void Rotate(RotationalDirection d)
        {
            base.Rotate(d);

            RecombobulateSignals();
        }
        protected override void Init()
        {
            // if diagonal signals are allowed, then agents should not be bigger than 1 by 1
            if (Type.Ruleset.DiagonalSignalsAllowed && Bounds.Size.X * Bounds.Size.Y > 1)
            {
                throw new InvalidOperationException("The Ruleset '" + Type.Ruleset.Name + "' has specified that diagonal signals are allowed, which requires that no agents are greater than 1 by 1 in size.");
            }


            // create ports (what a helpful comment)
            CreatePorts();

            Grid.AddAgent(this);

        }

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

                    if (!Type.Ruleset.DiagonalSignalsAllowed)
                    {
                        continue;
                    }

                    // create a diagonal port.
                    Port p = new FullPort(facing, 0, this);
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
                for (int j = 0; j < Type.Size.X; j++)
                {
                    Port p = new FullPort(facing, j, this);
                    ports.Add(p);
                    p.OnStartReiceving += OnPortStatusChanged;
                    p.OnStopReiceving += OnPortStatusChanged;
                }
                return ports;
            }

            for (int j = 0; j < Type.Size.Y; j++)
            {
                Port p = new FullPort(facing, j, this);
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


        private void OnPortStatusChanged(Object Sender, EventArgs e)
        {

            StatusChanged();
        }


        // how often do you get to type recombobulate? not often!
        private void RecombobulateSignals()
        {
            // stop transmitting and receiving signals, then 'reboot'
            List<Chunk> toUpdate = new List<Chunk>();
            toUpdate.AddRange(ChunksWithin);


            foreach (Port p in PortList)
            {
                if (p.Status == PortStatus.transmitting || p.Status == PortStatus.transceiving)
                {
                    int v = p.TransmittingValue;

                    p.StopTransmitting();

                    //p.Transmit(v);
                }

                if (p.Status == PortStatus.receiving)
                {
                    toUpdate.AddRange(p.ReceivingSignal.ChunksWithin);
                }
                p.StopReceiving();


            }

            Grid.UpdateSignals(toUpdate);
            _state = Type.DefaultState;


        }

        internal Port GetPort(PortIdentifier portID)
        {
            if (!portID.CheckValidity(Type))
            {
                throw new InvalidOperationException("Bad PortID.");
            }
            return Ports[(int)portID.Facing][portID.ID];
        }


        ///<summary>
        /// 
        /// </summary>
        /// <returns> the value this port was receiving at the end of the last simulation step.</returns>
        internal int GetPortValue(PortIdentifier portID)
        {
            if (!portID.CheckValidity(Type))
            {
                throw new InvalidOperationException("Bad PortID.");
            }

            return _stalePortValues[(int)portID.Facing][portID.ID];
        }


        protected override void PreserveValues()
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

    }
}
