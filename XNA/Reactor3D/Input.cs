/*
 * Reactor 3D MIT License
 * 
 * Copyright (c) 2010 Reiser Games
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System;
namespace Reactor
{
    public class RInput
    {
        static RInput _instance;
        public static RInput Instance
        {
            get { return _instance; }
        }
        public RInput()
        {
            if (REngine.Instance == null)
                throw new Exception("Reactor Engine must first be initialized");
            else
            {
                if (RInput.Instance == null)
                {
                    _instance = this;
                }
                else
                {
                    return;
                }
            }
        }
#if !XBOX
        public R2DVECTOR GetMouseScreenPosition()
        {
            

            MouseState state = Mouse.GetState();
            Vector2 mouse = new Vector2(state.X, state.Y);
            return R2DVECTOR.FromVector(mouse);

            //Vector2 mouseMoved = new Vector2(lastMouseLocation.X - state.X, lastMouseLocation.Y - state.Y);
            //lastMouseLocation = new Vector2(state.X, state.Y);
        }

        public void GetMouse(out int X, out int Y, out int Wheel, out bool B1, out bool B2)
        {

            B1 = false; B2 = false;
            Wheel = -1; X = -1; Y = -1;
            MouseState state = Mouse.GetState();
            X = state.X; Y = state.Y;
            Wheel = state.ScrollWheelValue;
            if (state.LeftButton == ButtonState.Pressed)
                B1 = true;
            if (state.RightButton == ButtonState.Pressed)
                B2 = true;



        }

        public void GetMouse(out int X, out int Y, out int Wheel, out bool B1, out bool B2, out bool B3)
        {

            B1 = false; B2 = false; B3 = false;
            Wheel = -1; X = -1; Y = -1;
            MouseState state = Mouse.GetState();
            
            X = state.X; Y = state.Y;
            Wheel = state.ScrollWheelValue;
            if (state.LeftButton == ButtonState.Pressed)
                B1 = true;
            if (state.RightButton == ButtonState.Pressed)
                B2 = true;
            if (state.MiddleButton == ButtonState.Pressed)
                B3 = true;
            

        }
        public void GetMouse(out int X, out int Y,out int Wheel, out bool B1, out bool B2, out bool B3,out bool B4, out bool B5)
        {

            B1 = false; B2 = false; B3 = false; B4 = false; B5 = false;
            Wheel = -1; X = -1; Y = -1;
            MouseState state = Mouse.GetState();
            X = state.X; Y = state.Y;
            Wheel = state.ScrollWheelValue;
            if (state.LeftButton == ButtonState.Pressed)
                B1 = true;
            if (state.RightButton == ButtonState.Pressed)
                B2 = true;
            if (state.MiddleButton == ButtonState.Pressed)
                B3 = true;
            if (state.XButton1 == ButtonState.Pressed)
                B4 = true;
            if (state.XButton2 == ButtonState.Pressed)
                B5 = true;

        }
        public void SetMousePosition(int X, int Y)
        {

            Mouse.SetPosition(X, Y);

        }
        public void GetCenteredMouse(out int X, out int Y, out int Wheel, out bool B1, out bool B2, out bool B3, out bool B4, out bool B5)
        {

            B1 = false; B2 = false; B3 = false; B4 = false; B5 = false;
            Wheel = -1; X = -1; Y = -1;
            MouseState state = Mouse.GetState();
            X = state.X - (REngine.Instance.GetViewport().Width/2); Y = state.Y - (REngine.Instance.GetViewport().Height/2);
            Wheel = state.ScrollWheelValue;
            if (state.LeftButton == ButtonState.Pressed)
                B1 = true;
            if (state.RightButton == ButtonState.Pressed)
                B2 = true;
            if (state.MiddleButton == ButtonState.Pressed)
                B3 = true;
            if (state.XButton1 == ButtonState.Pressed)
                B4 = true;
            if (state.XButton2 == ButtonState.Pressed)
                B5 = true;

                Mouse.SetPosition(REngine.Instance.GetViewport().Width / 2, REngine.Instance.GetViewport().Height / 2);

        }

        public void GetCenteredMouse(out int X, out int Y, out int Wheel, out bool B1, out bool B2, out bool B3)
        {

            B1 = false; B2 = false; B3 = false; 
            Wheel = -1; X = -1; Y = -1;
            MouseState state = Mouse.GetState();
            X = state.X - (REngine.Instance.GetViewport().Width / 2); Y = state.Y - (REngine.Instance.GetViewport().Height / 2);
            Wheel = state.ScrollWheelValue;
            if (state.LeftButton == ButtonState.Pressed)
                B1 = true;
            if (state.RightButton == ButtonState.Pressed)
                B2 = true;
            if (state.MiddleButton == ButtonState.Pressed)
                B3 = true;

            Mouse.SetPosition(REngine.Instance.GetViewport().Width / 2, REngine.Instance.GetViewport().Height / 2);
            

        }
        public void GetCenteredMouse(out int X, out int Y, out int Wheel, out bool B1, out bool B2)
        {

            B1 = false; B2 = false;
            Wheel = -1; X = -1; Y = -1;
            MouseState state = Mouse.GetState();
            X = state.X - (REngine.Instance.GetViewport().Width / 2); Y = state.Y - (REngine.Instance.GetViewport().Height / 2);
            Wheel = state.ScrollWheelValue;
            if (state.LeftButton == ButtonState.Pressed)
                B1 = true;
            if (state.RightButton == ButtonState.Pressed)
                B2 = true;

            Mouse.SetPosition(REngine.Instance.GetViewport().Width / 2, REngine.Instance.GetViewport().Height / 2);


        }
        public bool IsKeyDown(CONST_REACTOR_KEY key)
        {
            if (Keyboard.GetState().IsKeyDown((Keys)key))
                return true;
            else
                return false;
        }

        public bool IsKeyUp(CONST_REACTOR_KEY key)
        {
            if (Keyboard.GetState().IsKeyUp((Keys)key))
                return true;
            else
                return false;
        }
#endif
#if WINDOWS
        int lastMouseX = 0, lastMouseY = 0;
        public void GetMouseState(ref int intX, ref int intY, ref bool mb1, ref bool mb2, ref bool mb3,ref int wheel, bool AbsolutePosition)
        {

            MouseState state = Mouse.GetState();
            if (AbsolutePosition)
            {
                intX = state.X;
                intY = state.Y;
            }
            else
            {
                intX = state.X - lastMouseX;
                intY = state.Y - lastMouseY;
            }
            if (state.LeftButton == ButtonState.Pressed)
                mb1 = true;
            if (state.MiddleButton == ButtonState.Pressed)
                mb2 = true;
            if (state.RightButton == ButtonState.Pressed)
                mb3 = true;

            wheel = state.ScrollWheelValue;

            lastMouseX = state.X;
            lastMouseY = state.Y;

        }
        public void GetMousePosition(ref int X, ref int Y)
        {
            MouseState state = Mouse.GetState();
            X = state.X;
            Y = state.Y;
        }
#endif


        public GamePadState GetControllerState(int index)
        {
            GamePadState pad = GamePad.GetState((PlayerIndex)index);
            return pad;
        }

    }
}