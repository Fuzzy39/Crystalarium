using CrystalCore.Rulesets;
using CrystalCore.Sim.Base;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Sim
{
    public class Agent : ChunkMember
    {
        /*
         * An Agent Represents most tangible things on a Grid.
         * They participate in the simulation, and they can be placed.
         */
        private Direction _facing; // the direction this agent is facing.

        private AgentType _type;

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



        // SHOULD BE INTERNAL
        internal Agent(Grid g, Rectangle bounds, AgentType t) : base(g, bounds)
        {
            _facing = Direction.up;
            _type = t;

            if(g.AgentsWithin(bounds).Count>1) // it will always be at least 1, because we are in our bounds.
            {
                throw new InvalidOperationException("This Agent: "+this+" overlaps another agent.");
            }

            g.AddAgent(this);
        }

        internal Agent(Grid g, Rectangle bounds, AgentType t, Direction facing) : this(g, bounds, t)
        {

            _facing = facing;
        }


        private bool IsRectangle()
        {
            return _bounds.Width != _bounds.Height;
        }

        public void Rotate(RotationalDirection d)
        {
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
