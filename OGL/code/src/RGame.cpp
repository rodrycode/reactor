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
	RGame* RGame::_instance = NULL;
	
	RGame::RGame(int argc, char** argv)
	{
		if(RGame::_instance == null)
		{
			RGame::_instance = this;
		
			this->engine = REngine::getInstance();
			gltSetWorkingDirectory(argv[0]);
			glutInit(&argc, argv);
			glutInitDisplayMode (GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH);
			this->Init();
		}
	}

	RGame::~RGame()
	{
		if(RGame::_instance->engine)
			RGame::_instance->engine->DestroyAll();
		delete this;
	}

	void RGame::Init()
	{
		glutDisplayFunc(OnRender);
		glutReshapeFunc(OnResize);
		glutIdleFunc(OnIdle);
		RGame::_instance->Load();
		
		glutMainLoop();
		
		RGame::_instance->Unload();
		
		delete this;
	}

	void RGame::OnIdle()
	{
		RGame::_instance->Update();
	}
	
	void RGame::OnResize(int width, int height)
	{
		RGame::_instance->engine->OnResize(width, height);
	}

	void RGame::OnRender()
	{
		RGame::_instance->Render();
		RGame::_instance->Update();
	}

	

	REngine* RGame::Reactor()
	{
		return REngine::getInstance();
	}

};