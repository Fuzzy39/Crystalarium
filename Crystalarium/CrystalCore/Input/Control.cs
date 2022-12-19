using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Input
{

    public delegate void Action (); // what an action does.


    public class Control
    {

        private struct ContextualAction
        {
            private Action action;
            private string context;

            public ContextualAction(Action a, string context)
            {
                this.action = a;
                this.context = context;
            }

            public void Act(string context)
            {
                if(this.context == null || context.Equals(this.context))
                {
                    action();
                }
            }
        }


        // An action is something triggered by a keybind.

        private Controller _controller; // the controller this action belongs to.
        private string _name; // The name of this action.
        private Keystate _keystate; // the keystate required for our actions to be triggered.
        List<ContextualAction> _actions;
        

        internal string Name
        {
            get => _name;
        }

        internal Controller controller
        {
            get => _controller;
        }

        internal Keystate keystate
        {
            get => _keystate;
        }

        internal Control( Controller c, string name, Keystate ks)
        {
            _name = name;
            _controller = c;
            _keystate = ks;
            _actions = new List<ContextualAction>();
        }

        public void AddAction(string context, Action act)
        {
            _actions.Add(new ContextualAction(act, context));
        }

        internal void Trigger()
        {
            foreach(ContextualAction a in _actions)
            {
                a.Act(controller.Context);
            }
        }


    }
}
