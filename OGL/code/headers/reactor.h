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


#ifndef RTYPESH
#define RTYPESH


#include "common.h"

namespace Reactor {

    class RVector2;

    class RVector3;

    class RVector4;

    class RQuaternion;

    class RMatrix;

    class REngine;

    class RScene;

    class RCamera;

    class RGame;

    class RInput;

    class RMathUtils;

    class RNode;
}


#include "RMathUtils.h"

#include "types/RVector2.h"
#include "types/RVector3.h"
#include "types/RVector4.h"
#include "types/RQuaternion.h"
#include "types/RMatrix.h"

namespace Reactor{

    typedef enum RLIGHT_TYPE
    {
        DIRECTIONAL		=	0x0000,
        POINT			=	0x0001,
        SPOT			=	0x0002
    } RLIGHT_TYPE;

/*

	struct rGLDevice
	{

		HGLRC context;
		HDC handle;
		HWND hwnd;
		rGLDevice()
		{
			context = NULL;
			handle = NULL;
			hwnd = NULL;
		};
	};

*/




    struct RLight
    {
        RLIGHT_TYPE type;
        RVector3 position, direction, attenuation;
        RColor diffuse, ambient, specular;
        RFLOAT falloff, phi, theta, radius;
        RBOOL enabled;
    };

}
#endif