using Crystalarium.Render;
using Crystalarium.Sim;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Input
{
    public class Controller
    {

        private Grid _grid;
        private GridView _view;

        // actual control related things.
        private InputHandler ih;

        //keybinds
        private List<Keybind> _keybinds;




        public Grid Grid
        {
            get => _grid;
        }

        public List<Keybind> Keybinds
        {
            get => _keybinds;
        }

        public GridView View
        {
            get => _view;
            set
            {
               

                

                if (value != null)
                {
                    // Make it clear to the previous gridview that it is no longer in focus.
                    _view.Controller = null;

                    
                    value.Controller = this;

                    // get the grid from our new view.
                    _grid = value.Grid;

                }
                else
                {
                    _grid = null;
                }

                // set our view and tell it that we are controlling it.
                _view = value;
            }
        }


        public Controller()
        {
            ih = new InputHandler();
            View = null;

        }


        public void Update()
        {
           

            // test code.
            if(ih.KeyIsState(Button.Enter, Keystate.OnPress))
            {
                Console.WriteLine("Enter Pressed.");
            }

            if (ih.KeyIsState(Button.Enter, Keystate.OnRelease))
            {
                Console.WriteLine("Enter Released.");
            }

            if (ih.KeyIsState(Button.MouseRight, Keystate.OnPress))
            {
                Console.WriteLine("Right Mouse Button Clicked.");
            }

            if (ih.KeyIsState(Button.MouseRight, Keystate.OnRelease))
            {
                Console.WriteLine("Right Mouse Button Released.");
            }

            // Update the input handler.
            ih.Update();


        }
        

        

          
    }
}
