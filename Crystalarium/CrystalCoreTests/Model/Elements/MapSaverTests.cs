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
    public class MapSaverTests
    {
        [TestMethod()]
        public void SaveLoadChunkTest()
        {
            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67));

            Ruleset r = e.addRuleset("dummy");

            e.Initialize();

            Map m = e.addGrid(r);

            m.ExpandToFit(new Rectangle(-1, -1, 20, 20));


            // act
            e.saveManager.Save("TestingSave.xml", m);

            m.Reset();

            e.saveManager.Load("TestingSave.xml", m);

         
            Assert.IsTrue(m.Bounds.Equals(new Rectangle(-16, -16, 48, 48)));
        }


        [TestMethod()]
        public void SaveLoadChunkNoResetTest()
        {
            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67));

            Ruleset r = e.addRuleset("dummy");

            e.Initialize();

            Map m = e.addGrid(r);




            // act
            m.ExpandToFit(new Rectangle(-1, -1, 20, 20));

            e.saveManager.Save("TestingSave.xml", m);

            m.ExpandGrid(Util.Direction.down);

            e.saveManager.Load("TestingSave.xml", m);


            Assert.IsTrue(m.Bounds.Equals(new Rectangle(-16, -16, 48, 48)));
        }


        // tests for entities, agents, etc

    }
}