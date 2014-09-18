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
        
        static std::shared_ptr<RInput> _instance;
        static RVOID KeyFunc(RCHAR key, RINT x, RINT y);
        static RVOID KeyUpFunc(RCHAR key, RINT x, RINT y);
        static RVOID SpecialKeyFunc(RINT key, RINT x, RINT y);
        static RVOID SpecialKeyUpFunc(RINT key, RINT x, RINT y);
        static RVOID MouseFunc(RINT button, RINT state, RINT x, RINT y);
        static RVOID JoystickFunc(RUINT state, RINT x, RINT y, RINT z);
        RBOOL* keys;
        RBOOL* mbuttons;
        RBOOL* jbuttons;
        RVector3 joystick;
        RVector2 mouse;
        
    public:
	    RInput();
        ~RInput();
        static std::shared_ptr<RInput> GetInstance();
        RVOID Init();
        RVOID Destroy();
        const RVector2& GetMouse();
        RVOID GetMouse(RINT &x, RINT &y);
        RVOID GetMouseButtonState(RBOOL &b1, RBOOL &b2, RBOOL &b3);
        const RVector3& GetJoystick();
        RBOOL IsJoyButtonDown(RINT button);
        RBOOL IsKeyDown(RBYTE key);
        RBOOL IsKeyUp(RBYTE key);
        RVOID SetRepeat(RINT milliseconds);
        
    };
};
#endif