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
            
            // set the controller
            _controller = c;
            c.addKeybind(this);

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
        }

        public void Destroy()
        {
            _controller.removeKeybind(this);
        }

        public void update(InputHandler ih)
        {
            // we need to check if the condition of this keybind is met, and if it is trigger the action
            if(active(ih))
            {
               
                action.Trigger();
                
            }

            triggeredLastUpdate = ButtonsDown(ih);
        }


        public bool active(InputHandler ih)
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

    }
}


//bababooey
