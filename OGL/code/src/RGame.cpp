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
#include "../headers/RGame.h"

namespace Reactor
{
	static int __ticks = 0;
    static int __frames = 0;
    static int __timebase = 0;
    static float __fps = 0.0f;
	void RGame::Run(int argc, char** argv)
	{
			//glutSetWorkingDirectory(argv[0]);
			glutInit(&argc, argv);
			glutInitDisplayMode (GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH);
	}
	RGame::~RGame()
	{	
		delete this;
	}
	
    float RGame::GetFPS(){
        return __fps;
    }
    
	void RGame::Init()
	{
		glutDisplayFunc(OnRender);
		glutReshapeFunc(OnResize);
		glutIdleFunc(OnIdle);
		glutMainLoop();
		
		
		//delete this;
	}

	void RGame::OnIdle()
	{
        
        int time = glutGet(GLUT_ELAPSED_TIME);
        if (time - __timebase > 1000) {
            __fps = __frames*1000.0f/(float)(time-__timebase);
            __timebase = time;
            __frames = 0;
        }
        if(time-__ticks > (1/60)){
            __ticks = time;
            ++__frames;
            RGame::Instance()->OnRender();
        }
		RGame::Instance()->Idle();
	}
	
	void RGame::OnResize(int width, int height)
	{
		RGame::Instance()->Reactor().OnResize(width, height);
	}

	void RGame::OnRender()
	{
        RGame::Instance()->Update();
		RGame::Instance()->Render();
		
	}

	REngine& RGame::Reactor()
	{
		return *REngine::Instance();
	}

};