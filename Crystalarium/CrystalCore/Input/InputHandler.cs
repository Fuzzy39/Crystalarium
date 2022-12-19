using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Input
{
   

    public enum Keystate
    {
        OnPress,
        OnRelease,
        Up,
        Down

    }

    internal class InputHandler
    {

        // Input handler's job is to make it relatively easy to grab user input, in a meaningful way.
        // It will make it possible to have coherent and useful keybinds.

        private KeyboardState prevKeyState;
        private MouseState prevMouseState;

        internal InputHandler()
        {
            Update();
        }

        // This needs to be called periodically for InputHandler to be useful.
        internal void Update()
        {
            prevKeyState = Keyboard.GetState();
            prevMouseState = Mouse.GetState();
        }

        // The main job of InputHandler. Is this key/button in this state?\
        // ugly code.
        internal bool KeyIsState(Button b, Keystate state)
        {
            if((int)b>254)
            {
                // mouse key
                switch (b)
                {
                    // ugly.
                    case Button.MouseLeft:
                        return ButtonIsState(
                            Mouse.GetState().LeftButton, 
                            prevMouseState.LeftButton, 
                            state);

                    case Button.MouseRight:
                        return ButtonIsState(
                            Mouse.GetState().RightButton,
                            prevMouseState.RightButton,
                            state);


                    case Button.MouseMiddle:
                        return ButtonIsState(
                            Mouse.GetState().MiddleButton,
                            prevMouseState.MiddleButton,
                            state);

                }
                return false;
            }

            // keyboard key

            ButtonState current;
            if( Keyboard.GetState().IsKeyDown( (Keys)(int)b ) )
            {
                current = ButtonState.Pressed;
            }
            else
            {
                current = ButtonState.Released;
            }

            ButtonState prev;
            if (prevKeyState.IsKeyDown((Keys)((int)b)))
            {
                prev = ButtonState.Pressed;//this is one of the three comments in this file
            }
            else
            {
                prev = ButtonState.Released;
            }

            return ButtonIsState(current, prev, state);

        }

        private bool ButtonIsState(ButtonState current, ButtonState prev, Keystate state)
        {
            switch (state)
            {
                case Keystate.Down:
                    return current == ButtonState.Pressed;
                case Keystate.Up:
                    return current == ButtonState.Released;
                case Keystate.OnPress:
                    return (current == ButtonState.Pressed) & (prev == ButtonState.Released);
                case Keystate.OnRelease:
                    return (current == ButtonState.Released) & (prev == ButtonState.Pressed);

            }
            return false;

        }


    }






}
