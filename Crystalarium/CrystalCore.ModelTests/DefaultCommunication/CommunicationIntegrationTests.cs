

using CrystalCore.Model.Communication;
using CrystalCore.Model.Communication.Default;
using CrystalCore.Model.Core;
using CrystalCore.Model.Core.Default;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCoreTests.Model.DefaultCommunication
{

    [TestClass()]
    public class CommunicationIntegrationTests
    {

        private class MockAgent : Agent { }

        [TestMethod()]
        public void CreateNodeFullTest()
        {
            Map m = new DefaultMap();

            ComponentFactory compFactory = m.Grid.ComponentFactory;

            // The entity factory should come standard, I'd think...
            EntityFactory entityFactory = new DefaultEntityFactory(compFactory); 

            // example entity declaration
            Node node = entityFactory.CreateNode(new MockAgent(), new Rectangle(7, 7, 1, 1), Direction.up, false);

            // if it didn't crash at this point, that's a good sign. but is it actually working?

            Assert.AreEqual(4, node.PortList.Count);

            int portsWithConnection = 0;
            node.PortList.ForEach(p => { if (p.Connection != null) portsWithConnection++; });
            Assert.AreEqual(4, portsWithConnection);

            Assert.AreEqual(5, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)));


            // grab the connections.
            List<Connection> connections= new();
            node.PortList.ForEach(p => connections.Add(p.Connection));

            connections.ForEach(conn => Assert.IsNull(conn.PortB));


            // connections will be sorted starting north, going clockwise.
            Assert.AreEqual(new Rectangle(7, 0, 1, 8), connections[0].Physical.Bounds);
            Assert.AreEqual(new Rectangle(7, 7, 9, 1), connections[1].Physical.Bounds);
            Assert.AreEqual(new Rectangle(7, 7, 1, 9), connections[2].Physical.Bounds);
            Assert.AreEqual(new Rectangle(0, 7, 8, 1), connections[3].Physical.Bounds);

            // make sure things do in fact make sense.
            PortDescriptor north = new PortDescriptor(0, CompassPoint.north);
            Port p = node.GetPort(north);

            Assert.AreEqual(north, p.Descriptor);
            Assert.AreEqual(CompassPoint.north, p.AbsoluteFacing);
            Assert.AreEqual(connections[0], p.Connection);

            p.Output = 5; // we'll remember this for later. the (relatively) north port should remain 5 later.


            node.Rotate(RotationalDirection.cw);

            Assert.AreEqual(Direction.right, node.Facing);
            // make sure things didn't change
            Assert.AreEqual(4, node.PortList.Count);

            portsWithConnection = 0;
            node.PortList.ForEach(p => { if (p.Connection != null) portsWithConnection++; });
            Assert.AreEqual(4, portsWithConnection);

            Assert.AreEqual(5, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)));


            connections = new();
            node.PortList.ForEach(p => connections.Add(p.Connection));

            connections.ForEach(conn => Assert.IsNull(conn.PortB));


            // connections will be sorted starting (absolute) east, going clockwise.
            Assert.AreEqual(new Rectangle(7, 0, 1, 8), connections[3].Physical.Bounds);
            Assert.AreEqual(new Rectangle(7, 7, 9, 1), connections[0].Physical.Bounds);
            Assert.AreEqual(new Rectangle(7, 7, 1, 9), connections[1].Physical.Bounds);
            Assert.AreEqual(new Rectangle(0, 7, 8, 1), connections[2].Physical.Bounds);

        
            p = node.GetPort(north);

            Assert.AreEqual(north, p.Descriptor);
            Assert.AreEqual(CompassPoint.east, p.AbsoluteFacing);
            Assert.AreEqual(connections[0], p.Connection);

            Assert.AreEqual(5, p.Output);
                
            m.Grid.Expand(Direction.right);

            Connection conn = p.Connection;

            Assert.IsNotNull(conn);
            Assert.AreEqual(5, conn.FromA);
            Assert.AreEqual(new Rectangle(7, 7, 9+16, 1), conn.Physical.Bounds);

            // okay, good so far.

            node.Destroy();

            Assert.AreEqual(0, m.Grid.ObjectsIntersecting(new(0, 0, 32, 16)));

        }
    }
}
