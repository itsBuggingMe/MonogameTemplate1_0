using Apos.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameTemplate1_0
{
    internal static class InputHelper
    {
        public static KeyboardState KeyboardState { get; private set; }

        public static MouseState MouseState { get; private set; }

        public static KeyboardState PrevKeyboardState { get; private set; }

        public static MouseState PrevMouseState { get; private set; }

        public static void TickUpdate()
        {
            PrevKeyboardState = KeyboardState;
            PrevMouseState = MouseState;
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }

        public static bool RisingEdge(Keys key)
        {
            return KeyboardState.IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key);
        }

        public static bool FallingEdge(Keys key)
        {
            return !KeyboardState.IsKeyDown(key) && PrevKeyboardState.IsKeyDown(key);
        }

        public static bool RisingEdge(bool IsLeftButton)
        {
            if(IsLeftButton)
            {
                return MouseState.LeftButton != PrevMouseState.LeftButton && MouseState.LeftButton == ButtonState.Released;
            }
            return MouseState.RightButton != PrevMouseState.RightButton && MouseState.RightButton == ButtonState.Released;
        }

        public static bool FallingEdge(bool IsLeftButton)
        {
            if (IsLeftButton)
            {
                return MouseState.LeftButton != PrevMouseState.LeftButton && MouseState.LeftButton == ButtonState.Pressed;
            }
            return MouseState.RightButton != PrevMouseState.RightButton && MouseState.RightButton == ButtonState.Pressed;
        }

        public static bool Down(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

        public static bool Down(bool IsLeftButton)
        {
            if(IsLeftButton)
            {
                return MouseState.LeftButton == ButtonState.Pressed;
            }
            return MouseState.RightButton == ButtonState.Pressed;
        }
    }
}
