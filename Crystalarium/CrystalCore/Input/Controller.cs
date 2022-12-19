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
        private List<Control> _controls;

        // context system
        private string _context;


      

        public List<Keybind> Keybinds
        {
            get => new List<Keybind>( _keybinds);
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
            _controls = new List<Control>();

            _context = "";

        }

        public List<Keybind> ConflictingKeybinds()
        {
            List<Keybind> toReturn = new List<Keybind>();

            foreach(Keybind kb in Keybinds)
            {
                if(kb.HasConflicts)
                {
                    toReturn.Add(kb);
                }
            }

            return toReturn;

        }


        public Control CreateControl(string name, Keystate ks)
        {

            foreach(Control a in _controls)
            {
                if(a.Name==name)
                {
                    throw new InvalidOperationException();
                }
            }

            Control c = new Control(this, name, ks);
            _controls.Add(c);
            return c;
            
        }


        public Control GetAction(string name)
        {
            foreach (Control a in _controls)
            {
                if (a.Name == name)
                {
                    return a;
                }
            }

            return null;
        }

        public Keybind BindControl(Control c, params Button[] buttons)
        {
            Keybind k = new Keybind(this, c, buttons);

            _keybinds.Add(k);
            
            foreach (Keybind kb in Keybinds)
            {
              

                kb.UpdateSupersets();
            }

            return k;
        }

        internal void RemoveKeybind(Keybind k)
        {
            _keybinds.Remove(k);
           
            foreach (Keybind kb in Keybinds)
            {
               
                kb.UpdateSupersets();
            }
        }



        internal void Update()
        {
           

            // run keybindss
            foreach(Keybind k in _keybinds)
            {
                k.Update(ih);
            }


            
            // Update the input handler.
            ih.Update();
            _prevMouseState = Mouse.GetState();


        }
        

        

          
    }
}
