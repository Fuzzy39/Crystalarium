using System;
using System.Collections.Generic;

namespace CrystalCore.Input
{
    public class Keybind
    {


        /*
         * A keybind represents a set of keys that trigger an action.
         * 
         */


        // relevant objects
        private Controller _controller;
        private Control _action; // the control that this keybind triggers

        // triggering condition
        private List<Button> _buttons; // the buttons reqired to trigger this keybind


        // other variables
        private bool triggeredLastUpdate; // whether this keybind was triggered last update.
        private List<Keybind> supersets; // list of keybinds that contain all of the keys that we have.

        public bool DisableOnSuperset { get; set; }



        // properites
        public List<Button> buttons
        {
            get => _buttons;
            set => _buttons = value;
        }

        public Keystate Trigger
        {
            get => _action.keystate;

        }




        internal Control action
        {
            get => _action;
        }



        public List<Keybind> Supersets
        {
            get
            {
                // how have I never thought of this before?
                return new List<Keybind>(supersets);
            }
        }


        public bool HasConflicts
        {
            get
            {
                if (!DisableOnSuperset)
                {
                    return false;
                }

                foreach (Keybind kb in supersets)
                {
                    if (kb.supersets.Contains(this))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // probably make a constructor or something.
        internal Keybind(Controller c, Control control, params Button[] buttons)
        {

            _controller = c;
            // and the action
            _action = control;
            if (_action == null)
            {
                throw new ArgumentException("Unkown Action '" + action + "'.");
            }

            // set up the triggered state.
            triggeredLastUpdate = false;


            _buttons = new List<Button>(buttons);


            // set up our list of supersets before we update them.
            supersets = new List<Keybind>();

            // don't forget to set the controller!

            // c.AddKeybind(this); // also sets our supersets

            DisableOnSuperset = true;

        }




        internal void UpdateSupersets()
        {
            supersets.Clear();
            if (!DisableOnSuperset)
            {
                return;
            }


            foreach (Keybind k in _controller.Keybinds)
            {
                if (k == this)
                {
                    continue;
                }

                if (isSuperset(k))
                {
                    supersets.Add(k);
                }
            }


        }


        // does this keybind have every key that we do?
        private bool isSuperset(Keybind k)
        {

            if (k.Trigger != Trigger)
            {
                return false;
            }

            foreach (Button b in _buttons)
            {
                if (!k.buttons.Contains(b))
                {
                    return false;
                }
            }

            return true;
        }


        public void Destroy()
        {
            _controller.RemoveKeybind(this);
        }

        internal void Update(InputHandler ih)
        {
            // we need to check if the condition of this keybind is met, and if it is trigger the action
            if (Active(ih))
            {

                action.Trigger();

            }

            triggeredLastUpdate = ButtonsDown(ih);
        }


        // returns weather the action behind this keybind should be run.
        internal bool Active(InputHandler ih)
        {
            // check that this key bind has been activated by the user.
            if (!Triggered(ih))
            {
                return false;
            }

            // we need to check the context, as well.



            if (!DisableOnSuperset)
            {

                return true;
            }

            // check that a superset of this keybind is not also triggered.
            foreach (Keybind k in supersets)
            {

                if (k.ButtonsDown(ih))
                {
                    // we do not need to run, another keybind is taking priority
                    return false;
                }
            }


            //  we are good to go.
            return true;


        }



        // returns whether the keystate is being pressed.
        private bool Triggered(InputHandler ih)
        {

            bool down = ButtonsDown(ih);

            switch (Trigger)
            {

                case Keystate.Down:
                    return down;
                case Keystate.Up:
                    return !down;

                case Keystate.OnPress:
                    return down & !triggeredLastUpdate;

                case Keystate.OnRelease:
                    return !down & triggeredLastUpdate;

            }


            return false;


        }


        internal bool ButtonsDown(InputHandler ih)
        {


            foreach (Button b in _buttons)
            {
                if (ih.KeyIsState(b, Keystate.Up))
                {
                    return false;
                }
            }
            return true;
        }

        // one must ascend to gremlintopia eventually.

        // Huh, that was ominious.



        // I only make a toString when I need to debug something, I guess.
        // definitely not good practice...
        public override string ToString()
        {
            string buttons = "";
            foreach (Button b in _buttons)
            {
                buttons += ", " + b;
            }
            return "Keybind { \"" + action.Name + "\" " + buttons + " }";
        }

        public string KeysAsString()
        {
            string s = "";
            foreach (Button b in _buttons)
            {
                s += b + " + ";
            }
            return s.Substring(0, s.Length - 3);
        }
    }
}

// I don't think I wrote this...
//bababooey
