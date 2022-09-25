using CrystalCore.Model.Communication;
using CrystalCore.Model.Grids;
using CrystalCore.Model.Rulesets;
using CrystalCore.Model.Rulesets.Conditions;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Objects
{
    public class Agent : ChunkMember
    {
        /*
         * An Agent Represents most tangible things on a Grid.
         * They participate in the simulation, and they can be placed.
         */
        private Direction _facing; // the direction this agent is facing.

        private AgentType _type;

        private List<List<Port>> _ports; // this agent's ports, stored by direction relative to the agent.

        private AgentState _state;

        private List<List<int>> _stalePortValues; // the value each port was receiving at the end of the last simulation step. 

        private bool statusChanged; // whether a port started/stopped receiving this step.
        private bool statusHadChanged; // whether a port started/stopped receiging last step.
        
        // properties
        public Direction Facing
        {
            get => _facing;
            set
            {
                if (value == _facing) { return; }

                if (IsRectangle())
                {
                    if (_facing != value.Opposite())
                    {
                        throw new ArgumentException("Rectangular Agents may not be rotated freely. " +
                            "\nThey are limited to the direction they were placed with and their opposite.");
                    }

                }

                _facing = value;
            }
        }

        public AgentType Type
        {
            get => _type;
        }

        internal List<List<Port>> Ports { get { return _ports; }  }
        internal List<Port> PortList 
        { 
            get
            {
                List<Port> toReturn = new List<Port>();
                foreach(List<Port> ports in Ports)
                {
                    toReturn.AddRange(ports);
                }

                return toReturn;
            }
                
        }

        public AgentState State
        {
            get { return _state; }
        }


        // Constructors
        internal Agent(Grid g, Rectangle bounds, AgentType t, Direction facing) : base(g, bounds)
        {


            _type = t;
            statusChanged = true; // this is true at initialization so the agent can do things of it's own accord when it is created


            if (Type.Ruleset.RotateLock)
            {
                _facing = Direction.up;
            }
            else
            {
                _facing = facing;
            }


            if (g.AgentsWithin(bounds).Count > 1) // it will always be at least 1, because we are in our bounds.
            {
                throw new InvalidOperationException("This Agent: " + this + " overlaps another agent.");
            }

            // if diagonal signals are allowed, then agents should not be bigger than 1 by 1
            if (Type.Ruleset.DiagonalSignalsAllowed && bounds.Size.X * bounds.Size.Y > 1)
            {
                throw new InvalidOperationException("The Ruleset '" + Type.Ruleset.Name + "' has specified that diagonal signals are allowed, which requires that no agents are greater than 1 by 1 in size.");
            }


            // create ports (what a helpful comment)
            CreatePorts();

            g.AddAgent(this);

            // do the default thing.
            _state = Type.DefaultState;
            _state.Execute(this);

        }


        private void CreatePorts()
        {
            _ports = new List<List<Port>>();
            

            // should always be 8, but who knows, maybe they'll invent more directions on a compass.
            int length = Enum.GetNames(typeof(CompassPoint)).Length;

            for (int i=0; i<length; i++)
            {
                List<Port> portList = new List<Port>();
                List<int> staleValues = new List<int>();

                // add relevant ports to this list.
                CompassPoint facing = (CompassPoint)i;

                if(facing.IsDiagonal())
                {
                    _ports.Add(portList);
                    
                    if (!Type.Ruleset.DiagonalSignalsAllowed)
                    {
                        continue;
                    }

                    // create a diagonal port.
                    Port p = Type.Ruleset.CreatePort(facing, 0, this);
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
           
            statusChanged = true;
        }

        public override void Destroy()
        {


            //Grid g = Grid;
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

        // facing is direction of ports to be made relative to upwards face of agent
        private List<Port> CreateOrthagonalPorts(CompassPoint facing, List<Port> ports)
        {
            // create orthagonal ports. Vertical first, diagonal after.

            if (((Direction)facing.ToDirection()).IsVertical())
            {
                for (int j = 0; j < Type.Size.X; j++)
                {
                    Port p = Type.Ruleset.CreatePort(facing, j, this);
                    ports.Add(p);
                    p.OnStartReiceving += OnPortStatusChanged;
                    p.OnStopReiceving += OnPortStatusChanged;
                }
                return ports;
            }

            for (int j = 0; j < Type.Size.Y; j++)
            {
                Port p = Type.Ruleset.CreatePort(facing, j, this);
                ports.Add(p);
                p.OnStartReiceving += OnPortStatusChanged;
                p.OnStopReiceving += OnPortStatusChanged;
            }

            return ports;

        }



        private bool IsRectangle()
        {
            return Bounds.Width != Bounds.Height;
        }

        public void Rotate(RotationalDirection d)
        {
            if(Type.Ruleset.RotateLock)
            {
                Console.WriteLine("Warning: Ruleset '" + Type.Ruleset.Name + "' has Rotation Lock enabled, and Agents cannot be set facing any other direction than up. " +
               "\n    Agent '" + this.ToString() + "' has attempted to rotate " + d+".");
                return;
            }



            if (IsRectangle())
            {
                Facing = Facing.Opposite();
                RecombobulateSignals();
                return;
            }

            Facing = Facing.Rotate(d);
            RecombobulateSignals();


        }

        // how often do you get to type recombobulate? not often!
        private void RecombobulateSignals()
        {
            // stop transmitting and receiving signals, then 'reboot'
            List<Chunk> toUpdate = new List<Chunk>();
            toUpdate.AddRange(ChunksWithin);
         

            foreach ( Port p in PortList)
            {
                if (p.Status == PortStatus.transmitting || p.Status == PortStatus.transceiving)
                {
                    int v = p.TransmittingValue;
                    
                    p.StopTransmitting();

                    //p.Transmit(v);
                }

                if(p.Status == PortStatus.receiving)
                {
                    toUpdate.AddRange(p.ReceivingSignal.ChunksWithin);
                }
                p.StopReceiving();
                    
                
            }

            Grid.UpdateSignals(toUpdate);
            _state = Type.DefaultState;


        }

        public override string ToString()
        {
            return "Agent { Type:\"" + Type.Name + "\", Location:" + Bounds.Location + ", Facing:" + Facing + " }";
        }

        public void PortReport()
        {
            Console.WriteLine(ToString());
            for (int i = 0; i<_ports.Count; i++)
            {
                Console.WriteLine(((CompassPoint)i) + ": " + _ports[i].Count);
            }
        }

        internal Port GetPort(PortIdentifier portID)
        {
            if(!portID.CheckValidity(Type))
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


        internal void UpdateState()
        {
            statusHadChanged = statusChanged;
            statusChanged = false;
            if(!statusHadChanged)
            {
                return;
            }

            UpdatePortValues();
            _state = DetermineState();

        }


   

        private AgentState DetermineState()
        {
            
            foreach (AgentState state in Type.States)
            {
                // if a state has no requirements, it fits the bill!
                if (state.Requirements == null)
                {
                    return state;
                }

                // otherwise, check if we meet the requirements.
                Token t = state.Requirements.Resolve(this);
                if ((bool)t.Value)
                {

                    return state;
                }
            }

            return Type.DefaultState;
        }

        internal void UpdatePortValues()
        {
            // loop through all directions
            for(int i = 0; i<_ports.Count; i++)
            {
                List<Port> list = _ports[i];

                // and every port in that direction...
                for(int j = 0; j<list.Count; j++)
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

        internal void Update()
        {
            if(!statusHadChanged)
            {
                return;
            }
           
            _state.Execute(this);

            

        }
    }
}
