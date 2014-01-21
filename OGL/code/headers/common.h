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

#ifndef __RCOMMON__
#define __RCOMMON__

//#include "../../dependencies/GLTools/include/GLTools.h"

//#include "../../dependencies/GLTools/include/GLShaderManager.h"

#ifdef __WIN32__
#define WINVER 0x0700
#endif





#include <math.h>
#include <iostream>
#include <sstream>
#include <assert.h>

#ifdef __MINGW32__
#include <limits.h>
typedef int64_t GLint64EXT;
typedef uint64_t GLuint64EXT;
#define GLAPI extern
#endif


#include <stdlib.h>
#include <stdio.h>
#include <string>




#ifdef WIN32
#define _WIN32
//Windows OpenGL Declarations
#define __max(a,b)	(((a) > (b)) ? (a) : (b))
#define __min(a,b)  (((a) < (b)) ? (a) : (b))

#ifdef FREEGLUT_STATIC
#include "../../dependencies/freeglut/win32/include/freeglut.h"
#else
#include <glut.h>
#endif

#endif //end Windows OpenGL Declarations




#ifdef __APPLE__
//#include <GLTools.h>
//#include <GLShaderManager.h>
#include <OpenGL/gl.h>
#include <OpenGL/glu.h>
#include <GLUT/GLUT.h>
//#include <err.h>


#define __max(a,b)	(((a) > (b)) ? (a) : (b))
#define __min(a,b)  (((a) < (b)) ? (a) : (b))
#endif

#ifdef _iOS

#endif

typedef long RRESULT;
#define R_OK	((RRESULT)0L)
#define R_FALSE ((RRESULT)1L)
#define R_INVALIDARG	((RRESULT)0x80070057L)
#define R_OUTOFMEMORY	((RRESULT)0x8007000EL)
#define FAILED(Status) ((RRESULT)(Status)<0)

#define null NULL



#define R_API __declspec( dllexport )
#define R_IMPORT __declspec( dllimport )


#define PI 3.1415926535897932384626433832795
#define PIdiv180 (PI/180.0)


#include "rtypes.h"
#include "collection.h"


#endif

