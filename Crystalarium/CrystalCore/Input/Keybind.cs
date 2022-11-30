using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Input
{
    public class Keybind
    {


        /*
         * A keybind represents a set of keys that trigger an action.
         * 
         */


        // relevant objects
        private Controller _controller; // the controller that this keybind belongs to.
        private Action _action; // the action that this keybind

        // triggering condition
        private List<Button> _buttons; // the buttons reqired to trigger this keybind
        private Keystate _trigger; // the state the buttons need to be in to trigger this keybind.
        private string _requiredContext; // the context required for this keybind to trigger. if "", any context.
       

        // other variables
        private bool triggeredLastUpdate; // whether this keybind was triggered last update.
        private List<Keybind> supersets; // list of keybinds that contain all of the keys that we have.

        public bool DisableOnSuperset { get; set; }

        public int SupersetCount { get { return supersets.Count; } }

        // properites
        public List<Button> buttons
        {
            get => _buttons;
            set => _buttons = value;
        }

        public Keystate trigger
        {
            get => _trigger;
            set => _trigger = value;
        }

        public Controller controller
        {
            get => _controller;

        }


        internal Action action
        {
            get => _action;
            set => action = value;
        }


        // probably make a constructor or something.
        public Keybind(Controller c, Keystate state, string action, params Button[] buttons)
        {

            // and the action
            _action = c.getAction(action);
            if(_action == null)
            {
                throw new ArgumentException("Unkown Action '" + action + "'.");
            }

            // set up the triggered state.
            _trigger = state;
            triggeredLastUpdate = false;
            

            _buttons = new List<Button>();

            foreach(Button b in buttons)
            {
                _buttons.Add(b);
            }


            // set up our list of supersets before we update them.
            supersets = new List<Keybind>();

            // don't forget to set the controller!
            _controller = c;
            c.addKeybind(this); // also sets our supersets

            // context
            _requiredContext = "";

            DisableOnSuperset = true;

        }

        public Keybind(Controller c, Keystate state, string action, string requiredContext, params Button[] buttons)
            : this(c, state, action, buttons)
        {
            _requiredContext = requiredContext;


        }


        internal void UpdateSupersets()
        {
            supersets.Clear();
            foreach(Keybind k in _controller.Keybinds)
            {
                if(k==this)
                {
                    continue;
                }

                if(isSuperset(k))
                {
                    supersets.Add(k);
                }
            }

           
        }


        // does this keybind have every key that we do?
        private bool isSuperset(Keybind k)
        {
            if( _requiredContext!=null && k._requiredContext!=null && !k._requiredContext.Equals(_requiredContext))
            {
                return false;
            }

            foreach(Button b in _buttons)
            {
                if(!k.buttons.Contains(b))
                {
                    return false;
                }
            }

            return true;
        }


        public void Destroy()
        {
            _controller.removeKeybind(this);
        }

        internal void Update(InputHandler ih)
        {
            // we need to check if the condition of this keybind is met, and if it is trigger the action
            if(Active(ih))
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

            // an empty string is treated as an 'any context' requirement
            if(_requiredContext!="" & _requiredContext != controller.Context)
            {
                return false;
            }

            if(!DisableOnSuperset)
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

            switch (_trigger)
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
            foreach( Button b in _buttons)
            {
                buttons +="," + b; 
            }
            return "Keybind { \"" + action.name + "\" " +  buttons+ "}";
        }
    }
}

// I don't think I wrote this...
//bababooey
