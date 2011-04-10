//
//  main.m
//  ReactorTest
//
//  Created by Gabriel Reiser on 4/8/11.
//  Copyright 2011 Reiser Games. All rights reserved.
//

//#import <Cocoa/Cocoa.h>
#import <common.h>
#import "MyGame.h"
using namespace std;
using namespace ReactorTest;
int main(int argc, char *argv[])
{
    //return NSApplicationMain(argc, (const char **)argv);
    auto_ptr<MyGame> game = auto_ptr<MyGame>(new MyGame(argc, argv));
    //MyGame* game = new MyGame(argc, argv);
    game->Load();
    //delete game;
    cout << "Unloaded Main\n";
    return 0;
}

