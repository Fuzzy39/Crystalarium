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
        private MouseState _prevMouseState;

        // actual control related things.
        private InputHandler ih;

        //keybinds
        private List<Keybind> _keybinds;
        private List<Action> _actions;



        
        public Grid Grid
        {
            get => _grid;
        }

        public List<Keybind> Keybinds
        {
            get => _keybinds;
        }

        public List<Action> Actions
        {
            get => _actions;
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


        public int DeltaScroll
        {
            get => Mouse.GetState().ScrollWheelValue - _prevMouseState.ScrollWheelValue;
        }


        public Controller()
        {
            ih = new InputHandler();
            View = null;

            _keybinds = new List<Keybind>();
            _actions = new List<Action>();


        }


        public void addAction(string name, Act action)
        {

            foreach(Action a in _actions)
            {
                if(a.name==name)
                {
                    throw new InvalidOperationException();
                }
            }

            _actions.Add(new Action(name, this, action));
            
        }


        public Action getAction(string name)
        {
            foreach (Action a in _actions)
            {
                if (a.name == name)
                {
                    return a;
                }
            }

            return null;
        }

        public void addKeybind(Keybind k)
        {
            _keybinds.Add(k);
            Console.WriteLine("+++++++++++++++++++");
            foreach (Keybind kb in Keybinds)
            {
               

                kb.UpdateSupersets();
            }
        }

        public void removeKeybind(Keybind k)
        {
            _keybinds.Remove(k);
            Console.WriteLine("+++++++++++++++++++");
            foreach (Keybind kb in Keybinds)
            {
               
                kb.UpdateSupersets();
            }
        }



        public void Update()
        {
           

            // run keybindss
            foreach(Keybind k in _keybinds)
            {
                k.update(ih);
            }


            
            // Update the input handler.
            ih.Update();
            _prevMouseState = Mouse.GetState();


        }
        

        

          
    }
}
