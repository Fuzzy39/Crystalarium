using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalCore.Model.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.Model.Rules;
using Microsoft.Xna.Framework;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.Model.Interface;
using CrystalCore.Model.Language;

namespace CrystalCore.Model.Elements.Tests
{
    [TestClass()]
    public class MapSaverTests
    {
        [TestMethod()]
        public void SaveLoadChunkTest()
        {
            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67), null);

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
            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67), null);

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


        [TestMethod()]
        public void SaveLoadAgentTest()
        {
            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67), null);

            Ruleset r = e.addRuleset("dummy");

            AgentType at = r.CreateType("agent", new Point(1));

            e.Initialize();

            Map m = e.addGrid(r);




            // act
            new Agent(m, new Point(3, 4), at, Util.Direction.left);

            e.saveManager.Save("TestingSave.xml", m);

            m.Reset();

            e.saveManager.Load("TestingSave.xml", m);


            Assert.IsTrue(m.getAgentAtPos(new Point(3, 4)).Type == at);
            Assert.IsTrue(m.getAgentAtPos(new Point(3, 4)).Facing == Direction.left);
        }




        }//you'll never find me!!!!

}