using CrystalCore.Model.Communication;
using CrystalCore.Rulesets;
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

        private List<List<Port>> _ports; // this agent's ports.

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

        // SHOULD BE INTERNAL
        internal Agent(Grid g, Rectangle bounds, AgentType t) : base(g, bounds)
        {
            _facing = Direction.up;
            _type = t;

            if (g.AgentsWithin(bounds).Count > 1) // it will always be at least 1, because we are in our bounds.
            {
                throw new InvalidOperationException("This Agent: " + this + " overlaps another agent.");
            }

            // if diagonal signals are allowed, then agents should not be bigger than 1 by 1
            if(Type.Ruleset.DiagonalSignalsAllowed && bounds.Size.X*bounds.Size.Y >1)
            {
                throw new InvalidOperationException("The Ruleset '"+Type.Ruleset.Name+"' has specified that diagonal signals are allowed, which requires that no agents are greater than 1 by 1 in size.");
            }


            // create ports (what a helpful comment)
            CreatePorts();

            g.AddAgent(this);

          

            // test code
            Ports[(int)CompassPoint.north][0].Transmit(1);

        }

        internal Agent(Grid g, Rectangle bounds, AgentType t, Direction facing) : this(g, bounds, t)
        {
            if (!Type.Ruleset.RotateLock)
            {
                _facing = facing;
                return;
            }
            Console.WriteLine("Warning: Ruleset '" + Type.Ruleset.Name + "' has Rotation Lock enabled, and Agents cannot be set facing any other direction than up. " +
                "\n    Agent " + this.ToString() + " has attempted to face: " + facing);
        }


        private void CreatePorts()
        {
            _ports = new List<List<Port>>();

            // should always be 8, but who knows, maybe they'll invent more directions on a compass.
            int length = Enum.GetNames(typeof(CompassPoint)).Length;
            for (int i=0; i<length; i++)
            {
                List<Port> portList = new List<Port>();
                

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
                    portList.Add(Type.Ruleset.CreatePort(facing, 0, this));
                  ;
                    continue;
                }

                portList = CreateOrthagonalPorts(facing, portList);
                _ports.Add(portList);
            }
        }


        private List<Port> CreateOrthagonalPorts(CompassPoint facing, List<Port> ports)
        {
            // create orthagonal ports. Vertical first, diagonal after.

            if (((Direction)facing.ToDirection()).IsVertical())
            {
                for (int j = 0; j < Bounds.Size.X; j++)
                {
                    ports.Add(Type.Ruleset.CreatePort(facing, j, this));
                }
                return ports;
            }

            for (int j = 0; j < Bounds.Size.Y; j++)
            {
                ports.Add(Type.Ruleset.CreatePort(facing, j, this));
            }

            return ports;

        }



        private bool IsRectangle()
        {
            return _bounds.Width != _bounds.Height;
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
                Grid.UpdateSignals(ChunksWithin);
                return;
            }

            Facing = Facing.Rotate(d);
            Grid.UpdateSignals(ChunksWithin);

        }


    }
}
