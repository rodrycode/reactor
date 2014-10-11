

#ifndef RVector4H
#define RVector4H

#import "../reactor.h"

namespace Reactor
{
    class RVector4
            {
                    public:
                    float x, y, z, w;

                    public:
                    /** Default constructor.
                    @note
                    It does <b>NOT</b> initialize the vector for efficiency.
                    */
                    inline RVector4()
                    {
                    }

                    inline RVector4( const float fX, const float fY, const float fZ, const float fW )
                    : x( fX ), y( fY ), z( fZ ), w( fW )
                    {
                    }

                    inline explicit RVector4( const float afCoordinate[4] )
                    : x( afCoordinate[0] ),
                    y( afCoordinate[1] ),
                    z( afCoordinate[2] ),
                    w( afCoordinate[3] )
                    {
                    }

                    inline explicit RVector4( const int afCoordinate[4] )
                    {
                        x = (float)afCoordinate[0];
                        y = (float)afCoordinate[1];
                        z = (float)afCoordinate[2];
                        w = (float)afCoordinate[3];
                    }

                    inline explicit RVector4( float* const r )
                    : x( r[0] ), y( r[1] ), z( r[2] ), w( r[3] )
                    {
                    }

                    inline explicit RVector4( const float scaler )
                    : x( scaler )
                    , y( scaler )
                    , z( scaler )
                    , w( scaler )
                    {
                    }

                    inline explicit RVector4(const RVector3& rhs)
                    : x(rhs.x), y(rhs.y), z(rhs.z), w(1.0f)
                    {
                    }

                    /** Exchange the contents of this vector with another.
                    */
                    inline void swap(RVector4& other)
                    {
                        std::swap(x, other.x);
                        std::swap(y, other.y);
                        std::swap(z, other.z);
                        std::swap(w, other.w);
                    }

                    inline float operator [] ( const size_t i ) const
                    {
                        assert( i < 4 );

                        return *(&x+i);
                    }

                    inline float& operator [] ( const size_t i )
                    {
                        assert( i < 4 );

                        return *(&x+i);
                    }

                    /// Pointer accessor for direct copying
                    inline float* ptr()
                    {
                        return &x;
                    }
                    /// Pointer accessor for direct copying
                    inline const float* ptr() const
                    {
                        return &x;
                    }

                    /** Assigns the value of the other vector.
                    @param
                    rkVector The other vector
                    */
                    inline RVector4& operator = ( const RVector4& rkVector )
                    {
                        x = rkVector.x;
                        y = rkVector.y;
                        z = rkVector.z;
                        w = rkVector.w;

                        return *this;
                    }

                    inline RVector4& operator = ( const float fScalar)
                    {
                        x = fScalar;
                        y = fScalar;
                        z = fScalar;
                        w = fScalar;
                        return *this;
                    }

                    inline bool operator == ( const RVector4& rkVector ) const
                    {
                        return ( x == rkVector.x &&
                                y == rkVector.y &&
                                z == rkVector.z &&
                                w == rkVector.w );
                    }

                    inline bool operator != ( const RVector4& rkVector ) const
                    {
                        return ( x != rkVector.x ||
                                y != rkVector.y ||
                                z != rkVector.z ||
                                w != rkVector.w );
                    }

                    inline RVector4& operator = (const RVector3& rhs)
                    {
                        x = rhs.x;
                        y = rhs.y;
                        z = rhs.z;
                        w = 1.0f;
                        return *this;
                    }

                    // arithmetic operations
                    inline RVector4 operator + ( const RVector4& rkVector ) const
                    {
                        return RVector4(
                                x + rkVector.x,
                                y + rkVector.y,
                                z + rkVector.z,
                                w + rkVector.w);
                    }

                    inline RVector4 operator - ( const RVector4& rkVector ) const
                    {
                        return RVector4(
                                x - rkVector.x,
                                y - rkVector.y,
                                z - rkVector.z,
                                w - rkVector.w);
                    }

                    inline RVector4 operator * ( const float fScalar ) const
                    {
                        return RVector4(
                                x * fScalar,
                                y * fScalar,
                                z * fScalar,
                                w * fScalar);
                    }

                    inline RVector4 operator * ( const RVector4& rhs) const
                    {
                        return RVector4(
                                rhs.x * x,
                                rhs.y * y,
                                rhs.z * z,
                                rhs.w * w);
                    }

                    inline RVector4 operator / ( const float fScalar ) const
                    {
                        assert( fScalar != 0.0 );

                        float fInv = 1.0f / fScalar;

                        return RVector4(
                                x * fInv,
                                y * fInv,
                                z * fInv,
                                w * fInv);
                    }

                    inline RVector4 operator / ( const RVector4& rhs) const
                    {
                        return RVector4(
                                x / rhs.x,
                                y / rhs.y,
                                z / rhs.z,
                                w / rhs.w);
                    }

                    inline const RVector4& operator + () const
                    {
                        return *this;
                    }

                    inline RVector4 operator - () const
                    {
                        return RVector4(-x, -y, -z, -w);
                    }

                    inline friend RVector4 operator * ( const float fScalar, const RVector4& rkVector )
                    {
                        return RVector4(
                                fScalar * rkVector.x,
                                fScalar * rkVector.y,
                                fScalar * rkVector.z,
                                fScalar * rkVector.w);
                    }

                    inline friend RVector4 operator / ( const float fScalar, const RVector4& rkVector )
                    {
                        return RVector4(
                                fScalar / rkVector.x,
                                fScalar / rkVector.y,
                                fScalar / rkVector.z,
                                fScalar / rkVector.w);
                    }

                    inline friend RVector4 operator + (const RVector4& lhs, const float rhs)
                    {
                        return RVector4(
                                lhs.x + rhs,
                                lhs.y + rhs,
                                lhs.z + rhs,
                                lhs.w + rhs);
                    }

                    inline friend RVector4 operator + (const float lhs, const RVector4& rhs)
                    {
                        return RVector4(
                                lhs + rhs.x,
                                lhs + rhs.y,
                                lhs + rhs.z,
                                lhs + rhs.w);
                    }

                    inline friend RVector4 operator - (const RVector4& lhs, float rhs)
                    {
                        return RVector4(
                                lhs.x - rhs,
                                lhs.y - rhs,
                                lhs.z - rhs,
                                lhs.w - rhs);
                    }

                    inline friend RVector4 operator - (const float lhs, const RVector4& rhs)
                    {
                        return RVector4(
                                lhs - rhs.x,
                                lhs - rhs.y,
                                lhs - rhs.z,
                                lhs - rhs.w);
                    }

                    // arithmetic updates
                    inline RVector4& operator += ( const RVector4& rkVector )
                    {
                        x += rkVector.x;
                        y += rkVector.y;
                        z += rkVector.z;
                        w += rkVector.w;

                        return *this;
                    }

                    inline RVector4& operator -= ( const RVector4& rkVector )
                    {
                        x -= rkVector.x;
                        y -= rkVector.y;
                        z -= rkVector.z;
                        w -= rkVector.w;

                        return *this;
                    }

                    inline RVector4& operator *= ( const float fScalar )
                    {
                        x *= fScalar;
                        y *= fScalar;
                        z *= fScalar;
                        w *= fScalar;
                        return *this;
                    }

                    inline RVector4& operator += ( const float fScalar )
                    {
                        x += fScalar;
                        y += fScalar;
                        z += fScalar;
                        w += fScalar;
                        return *this;
                    }

                    inline RVector4& operator -= ( const float fScalar )
                    {
                        x -= fScalar;
                        y -= fScalar;
                        z -= fScalar;
                        w -= fScalar;
                        return *this;
                    }

                    inline RVector4& operator *= ( const RVector4& rkVector )
                    {
                        x *= rkVector.x;
                        y *= rkVector.y;
                        z *= rkVector.z;
                        w *= rkVector.w;

                        return *this;
                    }

                    inline RVector4& operator /= ( const float fScalar )
                    {
                        assert( fScalar != 0.0 );

                        float fInv = 1.0f / fScalar;

                        x *= fInv;
                        y *= fInv;
                        z *= fInv;
                        w *= fInv;

                        return *this;
                    }

                    inline RVector4& operator /= ( const RVector4& rkVector )
                    {
                        x /= rkVector.x;
                        y /= rkVector.y;
                        z /= rkVector.z;
                        w /= rkVector.w;

                        return *this;
                    }

                    /** Calculates the dot (scalar) product of this vector with another.
                    @param
                    vec Vector with which to calculate the dot product (together
                    with this one).
                    @return
                    A float representing the dot product value.
                    */
                    inline float dotProduct(const RVector4& vec) const
                    {
                        return x * vec.x + y * vec.y + z * vec.z + w * vec.w;
                    }
                    /// Check whether this vector contains valid values
                    inline bool isNaN() const
                    {
                        return Math::isNaN(x) || Math::isNaN(y) || Math::isNaN(z) || Math::isNaN(w);
                    }
                    /** Function for writing to a stream.
                    */
                    inline friend std::ostream& operator <<
                    ( std::ostream& o, const RVector4& v )
                    {
                        o << "RVector4(" << v.x << ", " << v.y << ", " << v.z << ", " << v.w << ")";
                        return o;
                    }
                    // special
                    static const RVector4 ZERO;
            };
}
#endif