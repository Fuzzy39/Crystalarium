using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;


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
            this.location = a.Node.Physical.Bounds.Location - origin;
            this.facing = a.Node.Facing;
            this.type = a.Type;
        }


        // create an agent from this representation.
        public void CreateAgent(Map m, Point relativeTo)
        {
            // this could be wrong when the direction is horizontal, because bounds arent adjusted.
            // seems like a minor issue - only effects map expansion.
            

            m.Grid.ExpandToFit(new Rectangle(location+relativeTo, type.GetSize(facing)));

            if (m.IsValidPosition(type, location+relativeTo, facing))
            {

                m.CreateAgent(type, relativeTo + location, facing);
            }

        }

        public void CreateAgent(Map m)
        {
            CreateAgent(m, new(0));
        }
    }
}
