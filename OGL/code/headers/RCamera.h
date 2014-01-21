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

#ifndef RCAMERA_H
#define RCAMERA_H


namespace Reactor
{
	class RCamera
	{
	private:
	
		RVector3 ViewDir;
		RVector3 RightVector;	
		RVector3 UpVector;
		RVector3 Position;

		RMatrix ViewMatrix;
		RDOUBLE RotatedX, RotatedY, RotatedZ;	
	
	public:
		RCamera();				//inits the values (Position: (0|0|0) Target: (0|0|-1) )
		void Set ( void );	//executes some glRotates and a glTranslate command
								//Note: You should call glLoadIdentity before using Render

		void Move ( const RVector3& Direction );
		void Move ( RDOUBLE Left, RDOUBLE Front, RDOUBLE Up );
		void RotateX ( RDOUBLE Angle );
		void RotateY ( RDOUBLE Angle );
		void RotateZ ( RDOUBLE Angle );
		const RVector3& GetLookAt();
		const RVector3& GetViewDir();
		void MoveForward ( RDOUBLE Distance );
		void MoveUpward ( RDOUBLE Distance );
		void StrafeRight ( RDOUBLE Distance );

		void SetViewMatrix( const RMatrix& View );
		const RMatrix& GetViewMatrix();


	};
};

#endif