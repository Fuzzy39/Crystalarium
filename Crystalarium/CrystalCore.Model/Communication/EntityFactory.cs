using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Communication
{
    internal interface EntityFactory
    {

        public ComponentFactory baseFactory { get; }
        public Node CreateNode(Rectangle bounds, Direction facing, bool createDiagonalPorts);

        public Connection CreateConnection(Port initial);

        public Port CreatePort(PortDescriptor descriptor, Direction parentRotation, Rectangle parentBounds);
    }
}
