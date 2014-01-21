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


typedef bool RBOOL;
typedef char RBYTE;
typedef unsigned char RCHAR;
typedef short RINT16;
typedef unsigned short RUINT16;
typedef int RINT;
typedef unsigned int RUINT;
typedef float RFLOAT;
typedef double RDOUBLE;
typedef void RVOID;
/*
#define RBOOL bool;
#define RBYTE char;
#define RINT int;
#define RFLOAT float;
#define RDOUBLE double;
#define RVOID void;
*/

namespace Reactor
{

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
	
	struct RECT {
		RINT left, right, top, bottom;
		RECT()
		{
			left = 0;
			right = 0;
			top = 0;
			bottom = 0;
		}
		RECT(RINT X, RINT Y, RINT Width, RINT Height)
		{
			left = X;
			top = Y;
			right = X + Width;
			bottom = Y + Height;
		}
	};
struct RVector2
{
	RDOUBLE x, y;

	RVector2(RDOUBLE X, RDOUBLE Y) {
		x = X;
		y = Y;
	};

	RVector2() {
		x = 0;
		y = 0;
	};

	RVector2(const RVector2 &v) {
		this->x = v.x;
		this->y = v.y;
		delete &v;
	};
	
	//Operator Overloads...

#pragma region "Operators"

	RVector2 operator + (const RVector2& v2) {
		return RVector2(x+v2.x, y+v2.y);
	};

	RVector2 operator + (const RDOUBLE value) {
		return RVector2(x+value, y+value);
	};

	RVector2 operator + () const {
		return *this;
	};

	RVector2 operator - (const RVector2& v2) {
		return RVector2(x-v2.x, y-v2.y);
	};

	RVector2 operator - (const RDOUBLE value) {
		return RVector2(x-value, y-value);
	};

	RVector2 operator - () const {
		return RVector2(-x, -y);
	};

	RVector2& operator += (const RVector2& v2) {
		x += v2.x;
		y += v2.y;
		return *this;
	};

	RVector2& operator -= (const RVector2& v2) {
		x -= v2.x;
		y -= v2.y;
		return *this;
	};

	RVector2& operator ++ () {
		x++;
		y++;
		return *this;
	};

	RVector2& operator ++ (RINT) {
		RVector2* v = this;
		++v;
		return *v;
	};

	RVector2& operator -- () {
		x--;
		y--;
		return *this;
	};

	RVector2& operator -- (RINT) {
		RVector2* v = this;
		--v;
		return *v;
	};

	RVector2 operator * (const RVector2& v2) {
		return RVector2(x*v2.x, y*v2.y);
	};

	RVector2 operator * (const RDOUBLE value) {
		return RVector2(x*value, y*value);
	};

	RVector2& operator *= (const RVector2& v2) {
		x *= v2.x;
		y *= v2.y;
		return *this;
	};

	RVector2 operator / (const RVector2& v2) {
		return RVector2(x / v2.x, y / v2.y);
	};

	RVector2 operator / (const RDOUBLE value) {
		return RVector2(x / value, y / value);
	};

	RVector2& operator /= (const RVector2& v2) {
		x /= v2.x;
		y /= v2.y;
		return *this;
	};

	RBOOL operator == (const RVector2& v2) {
		return ((x == v2.x) && (y == v2.y));
	};

	RBOOL operator != (const RVector2& v2) {
		return ((x != v2.x) && (y != v2.y));
	};
#pragma endregion
#pragma region "Methods"

	RDOUBLE Dot(const RVector2& v)
	{
		return x*v.x + y*v.y;
	};

	RDOUBLE Length()
	{
		return sqrt(this->Dot(*this));
	};

	RVector2 Normalize()
	{
		RVector2 res;
		RDOUBLE l = this->Length();
		if(l == 0.0) return RVector2();
		res.x = this->x / l;
		res.y = this->y / l;
		return res;
	}

#pragma endregion

};

struct RVector3
{
	RDOUBLE x, y, z;

	RVector3()
	{
		x=0;
		y=0;
		z=0;
	};

	RVector3(RDOUBLE X, RDOUBLE Y, RDOUBLE Z)
	{
		x=X;
		y=Y;
		z=Z;
	};

	RVector3(const RVector3& v)
	{
		x=v.x;
		y=v.y;
		z=v.z;
	};

	#pragma region "Operators"

	RVector3 operator + (const RVector3& v3) {
		return RVector3(x+v3.x, y+v3.y, z+v3.z);
	};

	RVector3 operator + (const RDOUBLE value) {
		return RVector3(x+value, y+value, z+value);
	};

	RVector3 operator + () const {
		return *this;
	};

	RVector3 operator - (const RVector3& v3) {
		return RVector3(x-v3.x, y-v3.y, z-v3.z);
	};

	RVector3 operator - (const RDOUBLE value) {
		return RVector3(x-value, y-value, z-value);
	};

	RVector3 operator - () const {
		return RVector3(-x, -y, -z);
	};

	RVector3& operator += (const RVector3& v3) {
		x += v3.x;
		y += v3.y;
		z += v3.z;
		return *this;
	};

	RVector3& operator -= (const RVector3& v3) {
		x -= v3.x;
		y -= v3.y;
		z -= v3.z;
		return *this;
	};

	RVector3& operator ++ () {
		x++;
		y++;
		z++;
		return *this;
	};

	RVector3& operator ++ (RINT) {
		RVector3* v = this;
		++v;
		return *v;
	};

	RVector3& operator -- () {
		x--;
		y--;
		z--;
		return *this;
	};

	RVector3& operator -- (RINT) {
		RVector3* v = this;
		--v;
		return *v;
	};

	RVector3 operator * (const RVector3& v3) {
		return RVector3(x*v3.x, y*v3.y, z*v3.z);
	};

	RVector3 operator * (const RDOUBLE value) {
		return RVector3(x*value, y*value, z*value);
	};

	RVector3& operator *= (const RVector3& v3) {
		x *= v3.x;
		y *= v3.y;
		z *= v3.z;
		return *this;
	};

	RVector3 operator / (const RVector3& v3) {
		return RVector3(x / v3.x, y / v3.y, z / v3.z);
	};

	RVector3 operator / (const RDOUBLE value) {
		return RVector3(x / value, y / value, z / value);
	};

	RVector3& operator /= (const RVector3& v3) {
		x /= v3.x;
		y /= v3.y;
		z /= v3.z;
		return *this;
	};

	RBOOL operator == (const RVector3& v3) {
		return ((x == v3.x) && (y == v3.y) && (z == v3.z));
	};

	RBOOL operator != (const RVector3& v3) {
		return ((x != v3.x) && (y != v3.y) && (z != v3.z));
	};
#pragma endregion
#pragma region "Methods"
	RDOUBLE Dot(const RVector3& v)
	{
		return x*v.x + y*v.y + z*v.z;
	};

	RVector3 Cross(const RVector3& v)
	{
		RVector3 vect;
		vect.x = y*v.z-z*v.y;
		vect.y = -(x*v.z-z*v.x);
		vect.z = x*v.y-y*v.x;
		return vect;
	}
	RDOUBLE Length()
	{
		return sqrt(this->Dot(*this));
	};

	RVector3 Normalize()
	{
		RVector3 res;
		RDOUBLE l = this->Length();
		if (l == 0.0) return RVector3();
		res.x = this->x / l;
		res.y = this->y / l;
		res.z = this->z / l;
		return res;
	}

#pragma endregion
};

struct RVector4
{
	RDOUBLE x, y, z, w;
};

struct RMatrix3 
{
	RDOUBLE m11, m12, m13, m21, m22, m23, m31, m32, m33;

	RMatrix3()
	{
	}

	RMatrix3(const RMatrix3& m)
	{
		m11=m.m11;
		m12=m.m12;
		m13=m.m13;

		m21=m.m21;
		m22=m.m22;
		m23=m.m23;

		m31=m.m31;
		m32=m.m32;
		m33=m.m33;
	}

#pragma region "Operators"
	RMatrix3 operator + (const RMatrix3& m)
	{
		RMatrix3 r;
		r.m11 = m11+m.m11;
		r.m12 = m12+m.m12;
		r.m13 = m13+m.m13;
		r.m21 = m21+m.m21;
		r.m22 = m22+m.m22;
		r.m23 = m23+m.m23;
		r.m31 = m31+m.m31;
		r.m32 = m32+m.m32;
		r.m33 = m33+m.m33;
		return r;
	}

	RMatrix3 operator - (const RMatrix3& m)
	{
		RMatrix3 r;
		r.m11 = m11-m.m11;
		r.m12 = m12-m.m12;
		r.m13 = m13-m.m13;
		r.m21 = m21-m.m21;
		r.m22 = m22-m.m22;
		r.m23 = m23-m.m23;
		r.m31 = m31-m.m31;
		r.m32 = m32-m.m32;
		r.m33 = m33-m.m33;
		return r;
	}

	RMatrix3 operator * (const RMatrix3& m)
	{
		RMatrix3 r;
		r.m11 = m11*m.m11;
		r.m12 = m12*m.m12;
		r.m13 = m13*m.m13;
		r.m21 = m21*m.m21;
		r.m22 = m22*m.m22;
		r.m23 = m23*m.m23;
		r.m31 = m31*m.m31;
		r.m32 = m32*m.m32;
		r.m33 = m33*m.m33;
		return r;
	}

	RMatrix3 operator / (const RMatrix3& m)
	{
		RMatrix3 r;
		r.m11 = m11/m.m11;
		r.m12 = m12/m.m12;
		r.m13 = m13/m.m13;
		r.m21 = m21/m.m21;
		r.m22 = m22/m.m22;
		r.m23 = m23/m.m23;
		r.m31 = m31/m.m31;
		r.m32 = m32/m.m32;
		r.m33 = m33/m.m33;
		return r;
	}

	RBOOL operator == (const RMatrix3& m)
	{
		if(m.m11 == m11 && m.m12 == m12 && m.m13 == m13 && m.m21 == m21 && m.m22 == m22 && m.m23 == m23 && m.m31 == m31 && m.m32 == m32 && m.m33 == m33)
			return true;
		else
			return false;
	}

	RBOOL operator != (const RMatrix3& m)
	{
		if(m.m11 != m11 && m.m12 != m12 && m.m13 != m13 && m.m21 != m21 && m.m22 != m22 && m.m23 != m23 && m.m31 != m31 && m.m32 != m32 && m.m33 != m33)
			return true;
		else
			return false;
	}
#pragma endregion
#pragma region "Methods"

	RMatrix3 Identity()
	{
		m11=1.0; m12=0.0; m13=0.0;
		m21=0.0; m22=1.0; m23=0.0;
		m31=0.0; m32=0.0; m33=1.0;
		return *this;
	}

	RVector3 Right()
	{
		return RVector3(m11,m21,m31);
	}

	RVector3 Up()
	{
		return RVector3(m12,m22,m32);
	}

	RVector3 Forward()
	{
		return RVector3(m13,m23,m33);
	}
#pragma endregion
};

struct RMatrix
{
	RDOUBLE m11,m12,m13,m14,m21,m22,m23,m24,m31,m32,m33,m34,m41,m42,m43,m44;

	RMatrix Identity()
	{
		RMatrix m;
		m.m11=1.0; m.m12=0.0; m.m13=0.0; m.m14=0.0;
		m.m21=0.0; m.m22=1.0; m.m23=0.0; m.m24=0.0;
		m.m31=0.0; m.m32=0.0; m.m33=1.0; m.m34=0.0;
		m.m41=0.0; m.m42=0.0; m.m43=0.0; m.m44=1.0;
		return m;
	}

	RMatrix()
	{
	}

	RMatrix(const RMatrix3& m)
	{
		m11=m.m11;
		m12=m.m12;
		m13=m.m13;
		m14=0.0;

		m21=m.m21;
		m22=m.m22;
		m23=m.m23;
		m24=0.0;

		m31=m.m31;
		m32=m.m32;
		m33=m.m33;
		m34=0.0;

		m41=0.0;
		m42=0.0;
		m43=0.0;
		m44=1.0;
	}

	RMatrix(const RMatrix& m)
	{
		m11=m.m11;
		m12=m.m12;
		m13=m.m13;
		m14=m.m14;

		m21=m.m21;
		m22=m.m22;
		m23=m.m23;
		m24=m.m24;

		m31=m.m31;
		m32=m.m32;
		m33=m.m33;
		m34=m.m34;

		m41=m.m41;
		m42=m.m42;
		m43=m.m43;
		m44=m.m44;

	}
	
	RMatrix3 ToRMatrix3()
	{
		RMatrix3 m;
		m.m11 = m11;
		m.m12 = m12;
		m.m13 = m13;

		m.m21 = m21;
		m.m22 = m22;
		m.m23 = m23;

		m.m31 = m31;
		m.m32 = m32;
		m.m33 = m33;
		return m;
	}

	RVector3 Right()
	{
		return RVector3(m11, m21, m31);
	}

	RVector3 Up()
	{
		return RVector3(m12, m22, m32);
	}

	RVector3 Forward()
	{
		return RVector3(m13, m23, m33);
	}

	RVector3 Position()
	{
		return RVector3(m14, m24, m34);
	}

	void SetPosition(RVector3 v)
	{
		m14 = v.x;
		m24 = v.y;
		m34 = v.z;
	}

};


struct RColor
{
		RFLOAT r;
		RFLOAT g;
		RFLOAT b;
		RFLOAT a;
	
	RColor() {
		r=0.0f;
		g=0.0f;
		b=0.0f;
		a=0.0f;
	};

	RColor(RFLOAT R, RFLOAT G, RFLOAT B, RFLOAT A) {
		r=R;
		g=G;
		b=B;
		a=A;
	};

	RColor(const RColor& color) {
		r=color.r;
		g=color.g;
		b=color.b;
		a=color.a;
	};

	GLfloat toGLfloat() const
	{
		GLfloat color[] = {r, g, b, a};
		return *color;
	};
};

struct RMaterial
{
	RColor diffuse, ambient, specular, emmisive;
	RFLOAT specularLevel, shininess;

};

struct RLight
{
	RLIGHT_TYPE type;
	RVector3 position, direction, attenuation;
	RColor diffuse, ambient, specular;
	RFLOAT falloff, phi, theta, radius;
	RBOOL enabled;
};



struct RPlane
{
	RDOUBLE a, b, c, d;

	RPlane()
	{
		a=0.0;
		b=0.0;
		c=0.0;
		d=0.0;
	}
};
};

#endif