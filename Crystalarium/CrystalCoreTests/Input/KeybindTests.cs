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
            Engine e = new Engine(TimeSpan.FromMilliseconds(16.67), null);

            Controller c = e.Controller;

            Control Control = c.CreateControl("dummy", Keystate.Down);

            Keybind a = c.BindControl(Control, Button.A);

            Keybind b = c.BindControl(Control, Button.A, Button.B);


            Assert.IsTrue(a.Supersets.Count == 1);


        }
    }
}