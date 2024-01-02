using CrystalCore.Model.Communication;
using CrystalCore.Model.Communication.Default;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using CrystalCoreTests.Model.DefaultCore;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCoreTests.Model.DefaultCommunication
{
    [TestClass()]
    public class ConnectionTests
    {

        


        // -- Covers situtions in which there is no portB.
        // X brand new 
        // X A destroyed
        // X B destroyed
        // X status quo (maybe grid expanded?)
        // -- Covers situtions in which the connection should have both ports connected
        // X brand new
        //   A destroyed
        //   B destroyed
        //   status quo
        // -- Covers situtions in which the connection should be destroyed.
        //   A and b destroyed


        //[TestMethod()]
        //public void TemplateTest() 
        //{
        //    Assert.Fail();
        //}

        [TestMethod()]
        public void OnCreateNoConnectTest()
        {
            // A connection is created and there is nothing else to connect to.

            MockPort p = new MockPort(new(0, 0), CompassPoint.south);

            MockGrid mg = new MockGrid
            {
                FindClosestObjectInDirection_result = null,
                FindClosestObjectInDirection_locationOut = new(0, 15)
            };

            MockMapObjectFactory cf = new MockMapObjectFactory(mg);
            Connection conn = new DefaultConnection(cf, p);
            Assert.AreEqual(p, conn.PortA);
            Assert.IsNull(conn.PortB);

            Assert.AreEqual(new Rectangle(0, 0, 1, 16), conn.Physical.Bounds);
            Assert.AreEqual(new Point(0, 0), mg.FindClosestObjectInDirection_locationIn);
        }

        [TestMethod()]
        public void OnCreateConnectTest()
        {

            // a connection is created and there is a port to connect to.

            // arrange (this feels really stupid...)
            MockPort portA = new MockPort(new(0, 0), CompassPoint.south);

            MockPort portB = new MockPort(new(0, 1), CompassPoint.north);

            MockGrid mg = new MockGrid
            {
                FindClosestObjectInDirection_locationOut = new(0, 1)
            };

            MockNode node = new()
            {
                toFind = portB,
                Physical = new MockMapObj(mg, new(0, 1, 1, 1))


            };

            ((MockMapObj)node.Physical).Entity = node;
            mg.FindClosestObjectInDirection_result = node.Physical;


            MockMapObjectFactory cf = new MockMapObjectFactory(mg);

            // act
            Connection conn = new DefaultConnection(cf, portA);

            // assert
            Assert.AreEqual(portA, conn.PortA);
            Assert.AreEqual(portB, conn.PortB);

            Assert.AreEqual(new Rectangle(0, 0, 1, 2), conn.Physical.Bounds);
            Assert.AreEqual(new Point(0, 0), mg.FindClosestObjectInDirection_locationIn);

            Assert.AreEqual(CompassPoint.north, node.lastAbsFacingSought);
            Assert.AreEqual(new Point(0, 1), node.lastPortLocationSought);
        }


        [TestMethod()]
        public void ADestroyedNoConnectTest()
        {
            // arrange (same as on create connect.)
            MockPort portA = new MockPort(new(0, 0), CompassPoint.south);

            MockPort portB = new MockPort(new(0, 1), CompassPoint.north);

            MockGrid mg = new MockGrid();


            MockNode node = new()
            {
                toFind = portB,
                Physical = new MockMapObj(mg, new(0, 1, 1, 1))


            };

            ((MockMapObj)node.Physical).Entity = node;
            mg.FindClosestObjectInDirection_result = node.Physical;
            mg.FindClosestObjectInDirection_locationOut = node.Physical.Bounds.Location;


            MockMapObjectFactory cf = new MockMapObjectFactory(mg);


            Connection conn = new DefaultConnection(cf, portA);



            // now act.
            conn.Disconnect(portA);


            // set up mocks... (the connection shouldn't find anything to connect to...
            mg.FindClosestObjectInDirection_locationOut = new(0, 0);
            mg.FindClosestObjectInDirection_result = null;

            conn.Update();


            // assert

            Assert.AreEqual(portB, conn.PortA);
            Assert.AreEqual(null, conn.PortB);

            Assert.AreEqual(new Rectangle(0, 0, 1, 2), conn.Physical.Bounds);
            Assert.AreEqual(new Point(0, 1), mg.FindClosestObjectInDirection_locationIn);

        }



        [TestMethod()]
        public void BDestroyedNoConnectTest()
        {
            // arrange (same as on create connect.)
            MockPort portA = new MockPort(new(0, 0), CompassPoint.south);

            MockPort portB = new MockPort(new(0, 1), CompassPoint.north);

            MockGrid mg = new MockGrid();


            MockNode node = new()
            {
                toFind = portB,
                Physical = new MockMapObj(mg, new(0, 1, 1, 1))


            };

            ((MockMapObj)node.Physical).Entity = node;
            mg.FindClosestObjectInDirection_result = node.Physical;
            mg.FindClosestObjectInDirection_locationOut = node.Physical.Bounds.Location;


            MockMapObjectFactory cf = new MockMapObjectFactory(mg);


            Connection conn = new DefaultConnection(cf, portA);



            // now act.
            conn.Disconnect(portB);


            // set up mocks... (the connection shouldn't find anything to connect to...
            mg.FindClosestObjectInDirection_locationOut = new(0, 15);
            mg.FindClosestObjectInDirection_result = null;

            conn.Update();


            // assert

            Assert.AreEqual(portA, conn.PortA);
            Assert.AreEqual(null, conn.PortB);

            Assert.AreEqual(new Rectangle(0, 0, 1, 16), conn.Physical.Bounds);
            Assert.AreEqual(new Point(0, 0), mg.FindClosestObjectInDirection_locationIn);

        }


        [TestMethod()]
        public void NoConnectStatusQuoTest()
        {
            // A connection is created and there is nothing else to connect to.

            MockPort p = new MockPort(new(0, 0), CompassPoint.south);

            MockGrid mg = new MockGrid
            {
                FindClosestObjectInDirection_result = null,
                FindClosestObjectInDirection_locationOut = new(0, 15)
            };

            MockMapObjectFactory cf = new MockMapObjectFactory(mg);
            Connection conn = new DefaultConnection(cf, p);
         

            mg.FindClosestObjectInDirection_result = null;
            mg.FindClosestObjectInDirection_locationOut = new(0, 31);

            conn.Update();


            Assert.AreEqual(p, conn.PortA);
            Assert.IsNull(conn.PortB);

            Assert.AreEqual(new Rectangle(0, 0, 1, 32), conn.Physical.Bounds);
            Assert.AreEqual(new Point(0, 0), mg.FindClosestObjectInDirection_locationIn);

        }



        [TestMethod()]
        public void AReplacedTest()
        {

            // There are three nodes, top, center, and bottom.
            // A connection is created from the bottom port of center (A) and should connect to bottom. (B)
            // center is 'destroyed', and when the connection is updated, it will find and connect to top (new A)

            // A and B will be switched in final testing but whatever

            MockGrid mg = new MockGrid();

            MockPort portNewA = new MockPort(new(0, 0), CompassPoint.south);

            MockPort portOldA = new MockPort(new(0, 1), CompassPoint.south);

            MockPort portB = new MockPort(new(0, 2), CompassPoint.north);

     

            MockNode Bnode = new()
            {
                toFind = portB,
                Physical = new MockMapObj(mg, new(0, 2, 1, 1))


            };

            ((MockMapObj)Bnode.Physical).Entity = Bnode;




            MockNode Anode = new()
            {
                toFind = portNewA,
                Physical = new MockMapObj(mg, new(0, 0, 1, 1))


            };

            ((MockMapObj)Anode.Physical).Entity = Anode;

            // mg needs setup.
            mg.FindClosestObjectInDirection_result = Bnode.Physical;
            mg.FindClosestObjectInDirection_locationOut = new(0, 2);


            // actually create the thing we're supposed to test.
            MockMapObjectFactory cf = new MockMapObjectFactory(mg);
            Connection conn = new DefaultConnection(cf, portOldA);

            // do a sanity check real quick before we act
            Assert.AreEqual(portOldA, conn.PortA);
            Assert.AreEqual(portB, conn.PortB);

            Assert.AreEqual(new Rectangle(0, 1, 1, 2), conn.Physical.Bounds);
            Assert.AreEqual(new Point(0, 1), mg.FindClosestObjectInDirection_locationIn);

            Assert.AreEqual(CompassPoint.north, Bnode.lastAbsFacingSought);
            Assert.AreEqual(new Point(0, 2), Bnode.lastPortLocationSought);


            // finally, act.
            conn.Disconnect(conn.PortA);


            mg.FindClosestObjectInDirection_result = Anode.Physical;
            mg.FindClosestObjectInDirection_locationOut = new(0, 0);

            conn.Update();


            // do a sanity check real quick before we act
            Assert.AreEqual(portB, conn.PortA);
            Assert.AreEqual(portNewA, conn.PortB);

            Assert.AreEqual(new Rectangle(0, 0, 1, 3), conn.Physical.Bounds);
            Assert.AreEqual(new Point(0, 2), mg.FindClosestObjectInDirection_locationIn);

            Assert.AreEqual(CompassPoint.south, Anode.lastAbsFacingSought);
            Assert.AreEqual(new Point(0, 0), Anode.lastPortLocationSought);

        }




    }
}
