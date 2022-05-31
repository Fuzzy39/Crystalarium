using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Input
{

    public delegate void Act(); // what an action does.


    internal class Action
    {

        // An action is something triggered by a keybind.

        private Controller _controller; // the controller this action belongs to.
        private string _name; // The name of this action.
        private Act action; // what this action does.
        

        internal string name
        {
            get => _name;
        }

        internal Controller controller
        {
            get => _controller;
        }



        internal Action( string name, Controller c, Act action)
        {
            _name = name;
            _controller = c;
            this.action = action;
        }

        internal void Trigger()
        {
            action();
        }


       


    }
}
