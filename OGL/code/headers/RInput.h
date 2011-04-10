/*
 Reactor 3D MIT License
 
 Copyright (c) 2010 Reiser Games
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 */

#include "common.h"

#ifndef RINPUT_H
#define RINPUT_H
namespace Reactor {
    
    class RInput {
    private:
        RInput();
        ~RInput();
        static RInput* _instance;
        static void KeyFunc(unsigned char key, int x, int y);
        static void KeyUpFunc(unsigned char key, int x, int y);
        static void SpecialKeyFunc(int key, int x, int y);
        static void SpecialKeyUpFunc(int key, int x, int y);
        static void MouseFunc(int button, int state, int x, int y);
        static void JoystickFunc(unsigned int state, int x, int y, int z);
        bool* keys;
        bool* mbuttons;
        bool* jbuttons;
        RVector3 joystick;
        RVector2 mouse;
        
    public:
        static RInput* getInstance();
        void Init();
        void Destroy();
        const RVector2& GetMouse();
        void GetMouse(int &x, int &y);
        void GetMouseButtonState(bool &b1, bool &b2, bool &b3);
        const RVector3& GetJoystick();
        bool IsJoyButtonDown(int button);
        bool IsKeyDown(char key);
        bool IsKeyUp(char key);
        void SetRepeat(int milliseconds);
        
    };
};
#endif