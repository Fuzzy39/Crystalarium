using System.Collections.Generic;

namespace CrystalCore.Input
{

    public delegate void Action(); // what an action does.


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

            public bool Act(string context)
            {
                if (this.context == "" || context.Equals(this.context))
                {
                    action();
                    return true;
                }

                return false;
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

        internal Control(Controller c, string name, Keystate ks)
        {
            _name = name;
            _controller = c;
            _keystate = ks;
            _actions = new List<ContextualAction>();
        }

        // method chaining is a thing!
        // neat.
        public Control AddAction(string context, Action act)
        {
            _actions.Add(new ContextualAction(act, context));
            return this;
        }

        public Control Bind(params Button[] buttons)
        {
            _controller.BindControl(this, buttons);
            return this;
        }

        /// <summary>
        /// Unbinds all keybinds
        /// </summary>
        /// <returns></returns>
        public Control Unbind()
        {
            foreach (Keybind kb in _controller.Keybinds)
            {
                if (kb.action == this)
                {
                    kb.Destroy();
                }
            }

            return this;

        }

        public Keybind FirstKeybind()
        {
            foreach (Keybind kb in _controller.Keybinds)
            {
                if (kb.action == this)
                {
                    return kb;
                }
            }
            return null;
        }

        public string FirstKeybindAsString()
        {
            Keybind kb = FirstKeybind();
            if (kb == null)
            {
                return "None";
            }

            return kb.KeysAsString();
        }
        internal void Trigger()
        {
            foreach (ContextualAction a in _actions)
            {
                if (a.Act(controller.Context))
                {
                    return;
                }
            }
        }


    }
}
