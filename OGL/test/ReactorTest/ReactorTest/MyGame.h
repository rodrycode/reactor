//
//  MyGame.h
//  ReactorTest
//
//  Created by Gabriel Reiser on 4/9/11.
//  Copyright 2011 Reiser Games. All rights reserved.
//
#include "common.h"
#include "RGame.h"
#include "RInput.h"
using namespace Reactor;

namespace ReactorTest {
    
    class MyGame : RGame {
    private:
        RInput* input;
    public:
        MyGame(int argc, char** argv);
        void Load();
        void Unload();
        void Update();
        void Render();
        void Idle();
        ~MyGame();
    };
    
}