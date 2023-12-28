using CrystalCore.Model.Communication;
using CrystalCore.Model.Communication.Default;
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
    public class NodeTests
    {

        //[TestMethod()]
        //public void TemplateTest() 
        //{
        //    Assert.Fail();
        //}

        [TestMethod()]
        public void TemplateTest()
        {

            Node node = new DefaultNode(null, Dont care, new Rectangle(5,5,2,2), Direction.left, false);
        }
    }
}
