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
#include "../headers/RCamera.h"

namespace Reactor
{
	RCamera::RCamera()
	{
		//Init with standard OGL values:
		Position = RVector3 (0.0, 0.0, 0.0);
		ViewDir = RVector3( 0.0, 0.0, -1.0);
		RightVector = RVector3(1.0, 0.0, 0.0);
		UpVector = RVector3(0.0, 1.0, 0.0);

		ViewMatrix = RMatrix();
		//Only to be sure:
		RotatedX = RotatedY = RotatedZ = 0.0;
	}

	void RCamera::Move (const RVector3& Direction)
	{
		Position = Position + Direction;
		ViewMatrix.m14 = Position.x;
		ViewMatrix.m24 = Position.y;
		ViewMatrix.m34 = Position.z;
	}

	void RCamera::RotateX (RDOUBLE Angle)
	{
		RotatedX += Angle;
	
		ViewDir = RVector3((ViewDir*cos(Angle*PIdiv180)
						+ UpVector*sin(Angle*PIdiv180))).Normalize();
		//Rotate viewdir around the right vector:
		
		//now compute the new UpVector (by cross product)
		UpVector = ViewDir.Cross(RightVector) * -1;
		
		ViewMatrix.m13 = ViewDir.x;
		ViewMatrix.m23 = ViewDir.y;
		ViewMatrix.m33 = ViewDir.z;

		ViewMatrix.m12 = UpVector.x;
		ViewMatrix.m22 = UpVector.y;
		ViewMatrix.m33 = UpVector.z;
		
	}

	void RCamera::RotateY (RDOUBLE Angle)
	{
		RotatedY += Angle;
	
		//Rotate viewdir around the up vector:
		ViewDir = RVector3((ViewDir*cos(Angle*PIdiv180)
						+ RightVector*sin(Angle*PIdiv180))).Normalize();

		//now compute the new RightVector (by cross product)
		RightVector = ViewDir.Cross(UpVector);

		ViewMatrix.m13 = ViewDir.x;
		ViewMatrix.m23 = ViewDir.y;
		ViewMatrix.m33 = ViewDir.z;

		ViewMatrix.m11 = RightVector.x;
		ViewMatrix.m21 = RightVector.y;
		ViewMatrix.m31 = RightVector.z;
	}

	void RCamera::RotateZ (RDOUBLE Angle)
	{
		RotatedZ += Angle;
	
		//Rotate viewdir around the right vector:
		
		RightVector = RVector3((RightVector*cos(Angle*PIdiv180)
						+ UpVector*sin(Angle*PIdiv180))).Normalize();

		//now compute the new UpVector (by cross product)
		UpVector = ViewDir.Cross(RightVector)*-1;

		ViewMatrix.m11 = RightVector.x;
		ViewMatrix.m21 = RightVector.y;
		ViewMatrix.m31 = RightVector.z;

		ViewMatrix.m12 = UpVector.x;
		ViewMatrix.m22 = UpVector.y;
		ViewMatrix.m33 = UpVector.z;
	}

	void RCamera::Set( void )
	{

		//The point at which the camera looks:
		RVector3 ViewPoint = Position+ViewDir;

		//as we know the up vector, we can easily use gluLookAt:
		gluLookAt(	Position.x,Position.y,Position.z,
					ViewPoint.x,ViewPoint.y,ViewPoint.z,
					UpVector.x,UpVector.y,UpVector.z);

	}

	const RVector3& RCamera::GetLookAt()
	{
		return Position+ViewDir;
	}
	
	const RVector3& RCamera::GetViewDir()
	{
		return ViewMatrix.Forward();
	}

	void RCamera::MoveForward( RDOUBLE Distance )
	{
		Position = Position + (ViewDir*-Distance);

		ViewMatrix.m14 = Position.x;
		ViewMatrix.m24 = Position.y;
		ViewMatrix.m34 = Position.z;
	}

	void RCamera::StrafeRight ( RDOUBLE Distance )
	{
		Position = Position + (RightVector*Distance);
		ViewMatrix.m14 = Position.x;
		ViewMatrix.m24 = Position.y;
		ViewMatrix.m34 = Position.z;
	}

	void RCamera::MoveUpward( RDOUBLE Distance )
	{
		Position = Position + (UpVector*Distance);
		ViewMatrix.m14 = Position.x;
		ViewMatrix.m24 = Position.y;
		ViewMatrix.m34 = Position.z;
	}

	void RCamera::SetViewMatrix(const RMatrix& view)
	{
		ViewMatrix = view;
		Position = ViewMatrix.Position();
		UpVector = ViewMatrix.Up();
		RightVector = ViewMatrix.Right();
		ViewDir = ViewMatrix.Forward();

		
	}
};