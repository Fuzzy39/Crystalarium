using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Input
{
    public class Keybind
    {

      
        /*
         * A keybind represents a set of keys that trigger an action.
         * 
         */


        private List<Button> _buttons; // the buttons reqired to trigger this keybind
        private Keystate _trigger; // the state the buttons need to be in to trigger this keybind.
        private Controller _controller; // the controller that this keybind belongs to.
        private Action _action; // the action that this keybind
        private bool triggeredLastUpdate; // whether this keybind was triggered last update.
        private List<Keybind> supersets; // list of keybinds that contain all of the keys that we have.


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


        public Action action
        {
            get => _action;
            set => action = value;
        }


        // probably make a constructor or something.

        public Keybind(Controller c, Keystate state, string action, params Button[] buttons)
        {



            // and the action
            _action = c.getAction(action);

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
            c.addKeybind(this);

        }


        
        public void UpdateSupersets()
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

            string ss = "";
            foreach(Keybind k in supersets)
            {
                ss += "\n    " + k;
            }
            Console.WriteLine("Keybind: " + this + "\nSupersets: " + ss+"\n");
        }


        // does this keybind have every key that we do?
        private bool isSuperset(Keybind k)
        {
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

        public void update(InputHandler ih)
        {
            // we need to check if the condition of this keybind is met, and if it is trigger the action
            if(Active(ih))
            {
               
                action.Trigger();
                
            }

            triggeredLastUpdate = ButtonsDown(ih);
        }


        // returns weather the action behind this keybind should be run.
        public bool Active(InputHandler ih)
        {
            if(Triggered(ih))
            {

                // check that a superset of this keybind is not also triggered.
                foreach(Keybind k in supersets)
                {
                    if(k.Triggered(ih))
                    {
                        return false;
                    }
                }

                // we are good to go.
                return true;
            }

            return false;
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


        public bool ButtonsDown(InputHandler ih)
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
