using CrystalCore.Model.Communication;
using CrystalCore.Model.Communication.Default;
using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCoreTests.Model.DefaultCommunication
{
    [TestClass()]
    public class PortTests
    {

        private class MockConnection : Connection
        {

            public MockConnection() 
            {
                FromB = 0;
            }

            public int ATransmitted = 0;
            public List<Port> PortsDisconnected = new List<Port>();

            private Port port = null;
            public Port PortA => port;

            public Port PortB => null;

            public int FromA => port.Output;

            public int FromB { get; private set; }

            public MapObject Physical => throw new NotImplementedException();

            public bool HasCollision => throw new NotImplementedException();

            public Point Size => throw new NotImplementedException();

            public bool Destroyed => false;

            public event ConnectionEventHandler OnValuesUpdated;

            public void Destroy()
            {
                throw new NotImplementedException();
            }

            public void Disconnect(Port toDisconnect)
            {
                PortsDisconnected.Add(toDisconnect);
                port = null;
            }

            public bool IsPortA(Port p)
            {
                return p == port;
            }

            public Port OtherPort(Port port)
            {
                if(IsPortA(port))
                {
                    return null;
                }

                throw new NotImplementedException();
            }

            public void Transmit(Port from, int value)
            {
                if(!IsPortA(port))
                {
                    // we're assuming it's from 'B'
                    FromB = value;
                    OnValuesUpdated?.Invoke(this, new Connection.EventArgs(false));
                }

                // don't care
                ATransmitted++;
                // do a little.
            }

            public void Update()
            {
                throw new NotImplementedException();
            }
        }


        [TestMethod()]
        public void AbsoluteFacingTest()
        {
            Port p = new DefaultPort(new(0, CompassPoint.north), Direction.up, new(1, 1, 2, 1));

            Assert.AreEqual(CompassPoint.north, p.AbsoluteFacing);

            p = new DefaultPort(new(0, CompassPoint.north), Direction.right, new(1, 1, 2, 1));

            Assert.AreEqual(CompassPoint.east, p.AbsoluteFacing);

            p = new DefaultPort(new(0, CompassPoint.west), Direction.down, new(1, 1, 2, 1));

            Assert.AreEqual(CompassPoint.east, p.AbsoluteFacing);


            p = new DefaultPort(new(1, CompassPoint.northwest), Direction.left, new(1, 1, 1,1));

            Assert.AreEqual(CompassPoint.southwest, p.AbsoluteFacing);


        }


        [TestMethod()]
        public void LocationTest()
        {

            // simple
            Port p = new DefaultPort(new(0, CompassPoint.north), Direction.up, new(1, 1, 1, 1));

            Assert.AreEqual(new Point(1,1), p.Location);

            p = new DefaultPort(new(0, CompassPoint.south), Direction.up, new(1, 1, 1, 1));

            Assert.AreEqual(new Point(1, 1), p.Location);

            p = new DefaultPort(new(0, CompassPoint.east), Direction.up, new(1, 1, 1, 1));

            Assert.AreEqual(new Point(1, 1), p.Location);

            p = new DefaultPort(new(0, CompassPoint.west), Direction.up, new(1, 1, 1, 1));

            Assert.AreEqual(new Point(1, 1), p.Location);

            //diagonal port

            p = new DefaultPort(new(0, CompassPoint.northwest), Direction.left, new(1, 1, 1, 1));

            Assert.AreEqual(new Point(1, 1), p.Location);


            // rectangular multiport
            p = new DefaultPort(new(2, CompassPoint.north), Direction.up, new(1, 1, 3, 2));

            Assert.AreEqual(new Point(3, 1), p.Location);

            p = new DefaultPort(new(0, CompassPoint.east), Direction.up, new(1, 1, 3, 2));

            Assert.AreEqual(new Point(3, 1), p.Location);

            p = new DefaultPort(new(1, CompassPoint.west), Direction.up, new(1, 1, 3, 2));

            Assert.AreEqual(new Point(1, 2), p.Location);


            // different absolute facings

            p = new DefaultPort(new(2, CompassPoint.north), Direction.left, new(1, 1, 3, 2));

            Assert.AreEqual(new Point(1, 3), p.Location);

        }

        //[TestMethod()]
        //public void ConnectionSetTest()
        //{
        //    Port p = new DefaultPort(new(), Direction.up, new(1, 1, 1, 1));

        //    bool inputUpdated = false;
        //    p.OnInputUpdated += (object sender, EventArgs e) => inputUpdated = true;


        //    p.Connection = new MockConnection();

        //    Assert.IsTrue(inputUpdated);
        //}

        //[TestMethod()]
        //public void OutputSetTest()
        //{
        //    Assert.Fail();
        //}


        //[TestMethod()]
        //public void InputChangedEventTest()
        //{
        //    Assert.Fail();
        //}
    }
}
