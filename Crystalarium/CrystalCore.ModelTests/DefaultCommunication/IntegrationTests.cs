

using CrystalCore.Model.Communication;
using CrystalCore.Model.Communication.Default;
using CrystalCore.Model.Core;
using CrystalCore.Model.Core.Default;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace CrystalCoreTests.Model.DefaultCommunication
{

    [TestClass()]
    public class IntegrationTests
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

            Assert.AreEqual(5, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);


            // grab the connections.
            List<Connection> connections = new();
            node.PortList.ForEach(p =>
            {
                Assert.AreEqual(p, p.Connection.PortA);
                connections.Add(p.Connection);
            });

            connections.ForEach(conn => Assert.IsNull(conn.PortB));
            connections.ForEach(conn => Assert.IsNotNull(conn.PortA));


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

            Assert.AreEqual(5, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);


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
            Assert.AreEqual(new Rectangle(7, 7, 9 + 16, 1), conn.Physical.Bounds);

            // okay, good so far.

            node.Destroy();

            Assert.AreEqual(0, m.Grid.ObjectsIntersecting(new(0, 0, 32, 16)).Count);

        }



        [TestMethod()]
        public void CreateNodesConnectionTest()
        {

            Map m = new DefaultMap();

            ComponentFactory compFactory = m.Grid.ComponentFactory;

            // The entity factory should come standard, I'd think...
            EntityFactory entityFactory = new DefaultEntityFactory(compFactory);

            // create two nodes.
            Node nodeA = entityFactory.CreateNode(new MockAgent(), new Rectangle(7, 7, 1, 1), Direction.up, false);

            Node nodeB = entityFactory.CreateNode(new MockAgent(), new Rectangle(0, 6, 1, 2), Direction.left, false);

            Port portA = nodeA.GetPort(new PortDescriptor(0, CompassPoint.west));
            Port portB = nodeB.GetPort(new PortDescriptor(0, CompassPoint.south));


            Assert.AreEqual(portB, portA.ConnectedTo);
            Assert.AreEqual(portA.Connection, portB.Connection);

            foreach (Port p in nodeA.PortList)
            {
                if (p == portA) continue;

                Assert.IsNull(p.ConnectedTo);
            }

            foreach (Port p in nodeB.PortList)
            {
                if (p == portB) continue;

                Assert.IsNull(p.ConnectedTo);
            }

            portA.Output = 5;

            Assert.AreEqual(5, portB.Input);
            portA.Output = 0;


            // 2 nodes, 9 connections (3 from A, 5 from B, 1 shared)
            Assert.AreEqual(2 + 9, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);


            // phase 2. rotate things
            nodeA.Rotate(RotationalDirection.cw);

            portA = nodeA.GetPort(new(0, CompassPoint.south));


            Assert.AreEqual(portA.ConnectedTo, portB);
            Assert.AreEqual(portA.Connection, portB.Connection);

            foreach (Port p in nodeA.PortList)
            {
                if (p == portA) continue;

                Assert.IsNull(p.ConnectedTo);
            }

            foreach (Port p in nodeB.PortList)
            {
                if (p == portB) continue;

                Assert.IsNull(p.ConnectedTo);
            }

            portA.Output = 5;

            Assert.AreEqual(5, portB.Input);
            portA.Output = 0;


            // 2 nodes, 9 connections (3 from A, 5 from B, 1 shared)
            Assert.AreEqual(2 + 9, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);

            // phase 3. delete node A.
            nodeA.Destroy();


            Assert.AreEqual(portB.ConnectedTo, null);



            foreach (Port p in nodeB.PortList)
            {

                Assert.IsNull(p.ConnectedTo);
            }



            // 2 nodes, 9 connections (3 from A, 5 from B, 1 shared)
            Assert.AreEqual(1 + 6, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);

        }



        [TestMethod()]
        public void IntegrationTest3()
        {

            // now we get a whole bunch of nodes in here so we can test node's stuff.

            Map m = new DefaultMap();

            ComponentFactory compFactory = m.Grid.ComponentFactory;

            // The entity factory should come standard, I'd think...
            EntityFactory entityFactory = new DefaultEntityFactory(compFactory);

            // create the nodes we aren't really directly testing.
            Node useless = entityFactory.CreateNode(new MockAgent(), new Rectangle(1, 0, 1, 1), Direction.up, false);

            Node  left = entityFactory.CreateNode(new MockAgent(), new Rectangle(0, 1, 1, 2), Direction.right, false);
            Node right = entityFactory.CreateNode(new MockAgent(), new Rectangle(4, 2, 2, 1), Direction.down, false);

            Port leftPort = left.GetPort(new PortDescriptor(1, CompassPoint.north));
            Port rightPort = right.GetPort(new PortDescriptor(0, CompassPoint.east));

            Assert.AreEqual(18, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);

            Assert.AreEqual(leftPort.ConnectedTo, rightPort);
            Assert.AreEqual(leftPort.Connection, rightPort.Connection);

            leftPort.Output = 5;
            Assert.AreEqual(5, rightPort.Input);


            // time for the most interesting node.

            Node center = entityFactory.CreateNode(new MockAgent(), new Rectangle(2, 2, 1, 1), Direction.up, false);

            Port centerLeft = center.GetPort(new PortDescriptor(0, CompassPoint.west));

            Port centerRight = center.GetPort(new PortDescriptor(0, CompassPoint.east));

            Assert.AreEqual(rightPort.Connection, centerRight.Connection);
            Assert.AreEqual(leftPort.Connection, centerLeft.Connection);

            centerLeft.Output = 4;

            Assert.AreEqual(4, leftPort.Input);
            Assert.AreEqual(0, rightPort.Input);
            Assert.AreEqual(5, centerLeft.Input);
            Assert.AreEqual(0, centerRight.Input);

            Assert.AreEqual(22, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);

            // rotate!
            center.Rotate(RotationalDirection.cw);
            center.Rotate(RotationalDirection.cw);

            // rotation destroys ports, so we'll have to grab them again.
            centerLeft = center.GetPort(new PortDescriptor(0, CompassPoint.west));
            centerRight = center.GetPort(new PortDescriptor(0, CompassPoint.east));

            Assert.AreEqual(rightPort.Connection, centerLeft.Connection);
            Assert.AreEqual(leftPort.Connection, centerRight.Connection);

            Assert.AreEqual(4, centerLeft.Output);

            Assert.AreEqual(0, leftPort.Input);
            Assert.AreEqual(4, rightPort.Input);
            Assert.AreEqual(0, centerLeft.Input);
            Assert.AreEqual(5, centerRight.Input);

            Assert.AreEqual(22, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);

            center.Destroy();

            Assert.AreEqual(18, m.Grid.ObjectsIntersecting(new(0, 0, 16, 16)).Count);

            Assert.AreEqual(leftPort.ConnectedTo, rightPort);
            Assert.AreEqual(leftPort.Connection, rightPort.Connection);
            Assert.AreEqual(5, rightPort.Input);



        }



        [TestMethod()]
        public void SimulationTest()
        {

            Map m = new DefaultMap();

            ComponentFactory compFactory = m.Grid.ComponentFactory;

            // The entity factory should come standard, I'd think...
            EntityFactory entityFactory = new DefaultEntityFactory(compFactory);

            Node[] nodes = new Node[3];
            nodes[0] = new DefaultNode(new MockAgent(), entityFactory, new(0, 0, 1, 1), Direction.up, false);
            nodes[1] = new DefaultNode(new MockAgent(), entityFactory, new(2, 0, 1, 3), Direction.right, false);
            nodes[2] = new DefaultNode(new MockAgent(), entityFactory, new(0, 2, 1, 1), Direction.down, false);

            // simulation step 1 (of 3)

            Port zeroEast = nodes[0].GetPort(new(0, CompassPoint.east));
            PortDescriptor south = new(0, CompassPoint.south);
            Assert.AreEqual(zeroEast.ConnectedTo, nodes[1].GetPort(south));

            zeroEast.Output = 1;

            Assert.AreEqual(0, nodes[1].GetStablePortValue(south));

            foreach(Node n in nodes)
            {
                n.DoSimulationStep();
            }

            Assert.AreEqual(1, nodes[1].GetStablePortValue(south));


            // actually just the one is sufficient as far as I'm concerned
            // I don't think there are any edge cases here, the idea's pretty simple
            // I guess rotation?





        }
    }
}
