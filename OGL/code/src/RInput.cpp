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
#include "RInput.h"

namespace Reactor {
    RInput* RInput::_instance = NULL;
    
    
    RInput::RInput(){
        if(_instance == null)
            _instance = this;
        
    }
    
    RInput::~RInput(){
        if(keys != null){
        delete keys;
        delete mbuttons;
        }
    }
    
    RInput* RInput::getInstance(){
        if(_instance == null){
            _instance = new RInput();
        
        } 
        return _instance;
    }
    
    void RInput::Init(){
        keys = new bool[256];
        for(int i=0; i<256; i++){
            keys[i] = false;
        }
        mbuttons = new bool[3];
        for(int i=0; i<3; i++){
            mbuttons[i] = false;
        }
        jbuttons = new bool[32];
        for(int i=0; i<32; i++){
            jbuttons[i] = false;
        }
        mouse = RVector2(0,0);
        glutKeyboardFunc(KeyFunc);
        glutKeyboardUpFunc(KeyUpFunc);
        glutSpecialFunc(SpecialKeyFunc);
        glutSpecialUpFunc(SpecialKeyUpFunc);
        glutSetKeyRepeat(0);
        glutJoystickFunc(JoystickFunc, 5);
        glutMouseFunc(MouseFunc);   
    }
    
    void RInput::JoystickFunc(unsigned int state, int x, int y, int z){
        for(int i=0;i<32;i++){      
            _instance->jbuttons[i] =  (state & (1<<i));
        }
        _instance->joystick = RVector3(x, y, z);
        
    }
    void RInput::KeyFunc(unsigned char key, int x, int y){
        fprintf(stdout, "Key Out: %c", key);
        
        _instance->keys[key] = true;
        _instance->mouse = RVector2(x, y);
        
    }
    
    void RInput::KeyUpFunc(unsigned char key, int x, int y){
        fprintf(stdout, "Key Up: %c", key);
        _instance->keys[key] = false;
        _instance->mouse = RVector2(x, y);
        
    }
    
    void RInput::SpecialKeyFunc(int key, int x, int y){
        fprintf(stdout, "Key Up: %c", key);
        _instance->keys[key] = true;
        _instance->mouse = RVector2(x, y);
    }
    
    void RInput::SpecialKeyUpFunc(int key, int x, int y){
        fprintf(stdout, "Key Up: %c", key);
        _instance->keys[key] = false;
        _instance->mouse = RVector2(x, y);
    }
    
    void RInput::MouseFunc(int button, int state, int x, int y){
        
        if(state == GLUT_UP)
            _instance->mbuttons[button] = false;
        else
            _instance->mbuttons[button] = true;
    }
    
    void RInput::Destroy(){
        delete keys;
        delete mbuttons;
        delete jbuttons;
        keys = null;
        mbuttons = null;
        jbuttons = null;
        delete _instance;
    }
    
    bool RInput::IsKeyDown(char key){
        bool ret = keys[key];
        return ret;
    }
    
    bool RInput::IsKeyUp(char key){
        bool ret = !(keys[key]);
        return ret;
    }
    
    bool RInput::IsJoyButtonDown(int button){
        if(button<32){
            bool ret = jbuttons[button];
            return ret;
        } else {
            return false;
        }
    }
    
    void RInput::SetRepeat(int milliseconds){
        glutSetKeyRepeat(milliseconds);
    }
    
    const RVector2& RInput::GetMouse(){
        return mouse;
    }
    
    void RInput::GetMouse(int &x, int &y){
        x = (int)mouse.x;
        y = (int)mouse.y;
        return;
    }
    
    void RInput::GetMouseButtonState(bool &b1, bool &b2, bool &b3){
        b1 = mbuttons[GLUT_LEFT_BUTTON];
        b2 = mbuttons[GLUT_MIDDLE_BUTTON];
        b3 = mbuttons[GLUT_RIGHT_BUTTON];
        return;
    }
    
    const RVector3& RInput::GetJoystick(){
        return joystick;
    }
    
};