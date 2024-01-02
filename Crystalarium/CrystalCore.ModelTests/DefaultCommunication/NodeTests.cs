using CrystalCore.Model.Communication;
using CrystalCore.Model.Communication.Default;
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
    public class NodeTests
    {

        //[TestMethod()]
        //public void TemplateTest() 
        //{
        //    Assert.Fail();
        //}

 

        // I'm going to be incredibly lazy and only test 1x1 nodes. cool? cool.

        //[TestMethod()]
        //public void CreateEmptyTest()
        //{
        //    MockGrid mg = new MockGrid();
        //    mg.ObjectsIntersecting_result = new();
           
        //    Node node = new DefaultNode(null, new MockEntityFactory(mg), new Rectangle(5,5,1,1), Direction.left, false);

        //    Assert.AreEqual(4, node.PortList.Count);

        //    foreach(Port p in node.PortList)
        //    {
        //        Assert.IsNull(p.ConnectedTo);
        //        Assert.IsNotNull(p.Connection);
        //    }

        //}


        //[TestMethod()]
        //public void CreateNeighborTest()
        //{
        //    MockGrid mg = new MockGrid();
        //    MockEntityFactory factory = new(mg);
        //    mg.ObjectsIntersecting_result = new();

        //    Node nodeA = new DefaultNode(null, factory, new Rectangle(7, 5, 1, 1), Direction.up, false);
        //    Node nodeB = new DefaultNode(null, factory, new Rectangle(5, 5, 1, 1), Direction.down, false);

        //    PortDescriptor desc = new(0, CompassPoint.west);

        //    Port AConn = nodeA.GetPort(desc);
        //    Port BConn = nodeB.GetPort(desc);

        //    Assert.AreEqual(AConn.ConnectedTo, BConn);

        //    Assert.AreEqual(4, node.PortList.Count);

        //    foreach (Port p in node.PortList)
        //    {
        //        Assert.IsNull(p.ConnectedTo);
        //    }

        //}
    }
}
