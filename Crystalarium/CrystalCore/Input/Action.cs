using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Input
{

    public delegate void Act(); // what an action does.


    public class Action
    {

        // An action is something triggered by a keybind.

        private Controller _controller; // the controller this action belongs to.
        private string _name; // The name of this action.
        private Act action; // what this action does.
        

        public string name
        {
            get => _name;
        }

        public Controller controller
        {
            get => _controller;
        }



        public Action( string name, Controller c, Act action)
        {
            _name = name;
            _controller = c;
            this.action = action;
        }

        public void Trigger()
        {
            action();
        }


       


    }
}
