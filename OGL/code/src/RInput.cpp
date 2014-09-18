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
#include "../headers/RInput.h"

namespace Reactor {
    std::shared_ptr<RInput> RInput::_instance = NULL;
    
    
    RInput::RInput(){
        if(RInput::_instance == NULL)
            RInput::_instance = std::make_shared<RInput>();
        
    }
    
    RInput::~RInput(){
        if(keys != null){
        delete keys;
        delete mbuttons;
        }
    }
    
    std::shared_ptr<RInput> RInput::GetInstance(){
        if(RInput::_instance == NULL){
            RInput::_instance = std::make_shared<RInput>();
        
        } 
        return RInput::_instance;
    }
    
    RVOID RInput::Init(){
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
    
    RVOID RInput::JoystickFunc(RUINT state, RINT x, RINT y, RINT z){
        for(int i=0;i<32;i++){      
            _instance->jbuttons[i] =  (state & (1<<i));
        }
        _instance->joystick = RVector3(x, y, z);
        
    }
    RVOID RInput::KeyFunc(RCHAR key, RINT x, RINT y){
        fprintf(stdout, "Key Out: %c", key);
        
        _instance->keys[key] = true;
        _instance->mouse = RVector2(x, y);
        
    }
    
    RVOID RInput::KeyUpFunc(RCHAR key, RINT x, RINT y){
        fprintf(stdout, "Key Up: %c", key);
        _instance->keys[key] = false;
        _instance->mouse = RVector2(x, y);
        
    }
    
    RVOID RInput::SpecialKeyFunc(RINT key, RINT x, RINT y){
        fprintf(stdout, "Key Up: %c", key);
        _instance->keys[key] = true;
        _instance->mouse = RVector2(x, y);
    }
    
    RVOID RInput::SpecialKeyUpFunc(RINT key, RINT x, RINT y){
        fprintf(stdout, "Key Up: %c", key);
        _instance->keys[key] = false;
        _instance->mouse = RVector2(x, y);
    }
    
    RVOID RInput::MouseFunc(RINT button, RINT state, RINT x, RINT y){
        
        if(state == GLUT_UP)
            _instance->mbuttons[button] = false;
        else
            _instance->mbuttons[button] = true;
    }
    
    RVOID RInput::Destroy(){
        delete keys;
        delete mbuttons;
        delete jbuttons;
        keys = null;
        mbuttons = null;
        jbuttons = null;
    }
    
    RBOOL RInput::IsKeyDown(RBYTE key){
        bool ret = keys[key];
        return ret;
    }
    
    RBOOL RInput::IsKeyUp(RBYTE key){
        bool ret = !(keys[key]);
        return ret;
    }
    
    RBOOL RInput::IsJoyButtonDown(RINT button){
        if(button<32){
            bool ret = jbuttons[button];
            return ret;
        } else {
            return false;
        }
    }
    
    RVOID RInput::SetRepeat(RINT milliseconds){
        glutSetKeyRepeat(milliseconds);
    }
    
    const RVector2& RInput::GetMouse(){
        return mouse;
    }
    
    RVOID RInput::GetMouse(RINT &x, RINT &y){
        x = (int)mouse.x;
        y = (int)mouse.y;
        return;
    }
    
    RVOID RInput::GetMouseButtonState(RBOOL &b1, RBOOL &b2, RBOOL &b3){
        b1 = mbuttons[GLUT_LEFT_BUTTON];
        b2 = mbuttons[GLUT_MIDDLE_BUTTON];
        b3 = mbuttons[GLUT_RIGHT_BUTTON];
        return;
    }
    
    const RVector3& RInput::GetJoystick(){
        return joystick;
    }
    
};