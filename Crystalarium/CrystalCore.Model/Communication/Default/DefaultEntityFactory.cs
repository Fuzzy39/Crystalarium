using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Communication.Default
{
    internal class DefaultEntityFactory : EntityFactory
    {

        private ComponentFactory _componentFactory;

        public DefaultEntityFactory(ComponentFactory componentFactory)
        {
            _componentFactory = componentFactory;
        }

        public ComponentFactory baseFactory => _componentFactory;

        public Connection CreateConnection(Port initial)
        {
            Connection c =  new DefaultConnection(_componentFactory, initial);
            _componentFactory.Map.OnComponentReady(c.Physical, new());
            return c;
        }

        public Node CreateNode( Rectangle bounds, Direction facing, bool createDiagonalPorts)
        {
            return new DefaultNode( this, bounds, facing, createDiagonalPorts);
        }


        // technically doesn't belong here, but it's convienent.
        public Port CreatePort(PortDescriptor descriptor, Direction parentRotation, Rectangle parentBounds)
        {
            return new DefaultPort(descriptor, parentRotation, parentBounds);   
        }
    }
}
