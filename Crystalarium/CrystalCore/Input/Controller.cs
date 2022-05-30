using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Input
{
    public class Controller
    {

        private MouseState _prevMouseState;

        // actual control related things.
        private InputHandler ih;

        //keybinds
        private List<Keybind> _keybinds;
        private List<Action> _actions;

        // context system
        private string _context;


      

        public List<Keybind> Keybinds
        {
            get => _keybinds;
        }

        public List<Action> Actions
        {
            get => _actions;
        }


        public int DeltaScroll
        {
            get => Mouse.GetState().ScrollWheelValue - _prevMouseState.ScrollWheelValue;
        }

        public string Context
        {
            get => _context;
            set => _context = value;
        }


        public Controller()
        {
            ih = new InputHandler();
         

            _keybinds = new List<Keybind>();
            _actions = new List<Action>();

            _context = "";

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
            
            foreach (Keybind kb in Keybinds)
            {
               

                kb.UpdateSupersets();
            }
        }

        public void removeKeybind(Keybind k)
        {
            _keybinds.Remove(k);
           
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
