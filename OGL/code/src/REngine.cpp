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
#include "../headers/REngine.h"

namespace Reactor
{
	REngine* REngine::_instance = NULL;
	REngine::REngine()
	{
		
		if(REngine::_instance == null)
		{
			REngine::_instance = new REngine();
			REngine::_instance->clearColor = RColor(0,0,0,0);
			//REngine::_instance->gldevice = rGLDevice();
		}
	}

	REngine::~REngine()
	{
		
		if(REngine::_instance)
		{
			delete REngine::_instance;
		}
		REngine::_instance = null;

	}
	REngine* REngine::getInstance()
	{
		if(REngine::_instance == null)
			REngine::_instance = new REngine();
		return REngine::_instance;
	}

	

	


	void REngine::Init3DWindowed(RECT &rect, char** title)
	{
		
		glutInitWindowSize (rect.right - rect.left, rect.bottom - rect.top);
		glutInitWindowPosition(rect.left, rect.top);
		glutCreateWindow(title[0]);
		
		REngine::_instance->_fullscreen = false;
		REngine::_instance->shaderManager.InitializeStockShaders();
	}

	void REngine::Init3DFullscreen(char** title, RINT width, RINT height, RINT color, RINT depth)
	{
		RECT rect;
		rect.left=0;
		rect.right=width;
		rect.top=0;
		rect.bottom=height;
		glutInitWindowSize (rect.right - rect.left, rect.bottom - rect.top);
		glutInitWindowPosition(rect.left, rect.top);
		glutCreateWindow(title[0]);
		std::ostringstream str;
		str << width << "x" << height << ":" << depth;
		glutGameModeString(str.str().c_str());
		
		
		REngine::_instance->_fullscreen = true;
		
	}
	
	void REngine::OnResize(RINT width, RINT height)
	{
		// Prevent a divide by zero, when window is too short
		// (you cant make a window of zero width).
		if(height == 0)
			height = 1;

		float ratio = 1.0* width / height;

		// Reset the coordinate system before modifying
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();
	
		// Set the viewport to be the entire window
		glViewport(0, 0, width, height);

		// Set the correct perspective.
		gluPerspective(45,ratio,1,1000);
		glMatrixMode(GL_MODELVIEW);
		
		//Call Update on the current Camera...
	}

	void REngine::Clear(RBOOL DepthOnly)
	{
		if(DepthOnly)
		{
			glClearDepth(1.0f);
			glClear(GL_DEPTH_BUFFER_BIT);
		}
		else
		{
			glClearColor(REngine::_instance->clearColor.r, REngine::_instance->clearColor.g, REngine::_instance->clearColor.b, REngine::_instance->clearColor.a);
			glClearDepth(1.0f);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		}
	}
	
	

	void REngine::RenderToScreen()
	{
		glFlush();
		glFinish();
		glutSwapBuffers();
	}

	void REngine::DestroyAll()
	{
		if(this->_fullscreen)
		{
			glutLeaveGameMode();
		}
		glutDestroyWindow(0);
		
		delete this;
	}

};