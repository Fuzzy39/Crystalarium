using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrystalCore.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Input.Tests
{
    [TestClass()]
    public class KeybindTests
    {
        [TestMethod()]
        public void KeybindSupersetTest()
        {
            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67));

            Controller c = e.Controller;

            c.AddAction("dummy", () => { });

            Keybind a =new Keybind(c, Keystate.Down, "dummy", Button.A);

            Keybind b = new Keybind(c, Keystate.Down, "dummy", Button.A, Button.B);

            Assert.IsTrue(a.Supersets.Count == 1);


        }
    }
}