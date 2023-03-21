using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalCore.Model.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Model.Rules;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Elements.Tests
{
    [TestClass()]
    public class MapTests
    {
        /*[TestMethod()]
        public void DestroyTest()
        {
            Assert.Fail();
        }*/

        /*[TestMethod()]
        public void ExpandGridTest()
        {
            Assert.Fail();
        }
        */

        [TestMethod()]
        public void ResetTest()
        {

            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67), null);
            Ruleset r = e.addRuleset("rule");
            e.Initialize();
            Map m = e.addGrid(r);

            // expand some

            m.ExpandToFit(new Rectangle(-1, -1, 20, 20));

            m.Reset();


            Assert.IsTrue(m.ChunkCount==1);
            Assert.IsTrue(m.Bounds==new Rectangle(0,0,16,16));
            Assert.IsTrue(m.EntitiesWithin(m.Bounds).Count == 0);

        }

     
    }
}