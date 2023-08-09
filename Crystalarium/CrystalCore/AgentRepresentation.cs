using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;


namespace CrystalCore
{

    /// <summary>
    /// An AgentRepresentation contains the information required to describe an agent that does not neccesarily exist.
    /// This includes position, facing, and the agent type.
    /// </summary>
    internal struct AgentRepresentation
    {
        private AgentType type;
        private Point location;
        private Direction facing;

        public AgentRepresentation(Point location, Direction facing, AgentType type)
        {
            this.location = location;
            this.facing = facing;
            this.type = type;
        }

        public AgentRepresentation(Agent a, Point origin)
        {
            this.location = a.Bounds.Location-origin;
            this.facing = a.Facing;
            this.type = a.Type;
        }


        // create an agent from this representation.
        public void CreateAgent(Map m, Point relativeTo)
        {
            // this could be wrong when the direction is horizontal, because bounds arent adjusted.
            // seems like a minor issue - only effects map expansion.
            

            m.ExpandToFit(Entity.CalculateBounds(location, type.UpwardsSize, facing));

            if (Entity.IsValidLocation(m, relativeTo + location, type.UpwardsSize, facing))
            {
                new Agent(m, relativeTo + location, type, facing);
            }

        }

        public void CreateAgent(Map m)
        {
            CreateAgent(m, new(0));
        }
    }
}
