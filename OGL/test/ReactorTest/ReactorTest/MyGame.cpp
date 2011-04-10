//
//  MyGame.cpp
//  ReactorTest
//
//  Created by Gabriel Reiser on 4/9/11.
//  Copyright 2011 Reiser Games. All rights reserved.
//

#include "MyGame.h"

using namespace Reactor;

namespace ReactorTest {
    
    
    
    MyGame::MyGame(int argc, char** argv) : RGame::RGame(argc, argv){
        
    }
    
    void MyGame::Load(){
        
        RECT screenSize = this->Reactor()->GetScreenSize();
        
        int w = screenSize.right;
        int h = screenSize.bottom;
        w = (w/2) - (800/2);
        h = (h/2) - (600/2);
        RECT window = RECT(w, h, 800, 600);
        
        this->Reactor()->Init3DWindowed(window, "ReactorTest");
        this->input = RInput::getInstance();
        this->input->Init();
        this->Init();
        
    }
    
    void MyGame::Unload(){
        cout << "Unloaded\n";
        this->input->Destroy();
        exit(0);
    }
    void MyGame::Idle(){
        Update();
    }
    
    void MyGame::Update(){
        if(input->IsKeyDown(27))
            this->Unload();
    }
    
    void MyGame::Render(){
        //printf("%4.2f\n", GetFPS());
        this->Reactor()->Clear();
        
        this->Reactor()->RenderToScreen();
    }
    
    MyGame::~MyGame(){
        cout << "Unloaded MyGame\n";
        delete this;
    }
}