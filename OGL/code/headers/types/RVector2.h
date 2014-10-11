


#ifndef RVector2H
#define RVector2H

#include "../common.h"
#include <ostream>

namespace Reactor 
{
    class RVector2
            {
                    public:
                    float x, y;

                    public:
                    
                    inline RVector2()
                    {
                    }

                    inline RVector2(const float fX, const float fY )
                    : x( fX ), y( fY )
                    {
                    }

                    inline explicit RVector2( const float scaler )
                    : x( scaler), y( scaler )
                    {
                    }

                    inline explicit RVector2( const float afCoordinate[2] )
                    : x( afCoordinate[0] ),
                    y( afCoordinate[1] )
                    {
                    }

                    inline explicit RVector2( const int afCoordinate[2] )
                    {
                        x = (float)afCoordinate[0];
                        y = (float)afCoordinate[1];
                    }

                    inline explicit RVector2( float* const r )
                    : x( r[0] ), y( r[1] )
                    {
                    }

                    /** Exchange the contents of this vector with another.
                    */
                    inline void swap(RVector2& other)
                    {
                        std::swap(x, other.x);
                        std::swap(y, other.y);
                    }

                    inline float operator [] ( const size_t i ) const
                    {
                        assert( i < 2 );

                        return *(&x+i);
                    }

                    inline float& operator [] ( const size_t i )
                    {
                        assert( i < 2 );

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
                    inline RVector2& operator = ( const RVector2& rkVector )
                    {
                        x = rkVector.x;
                        y = rkVector.y;

                        return *this;
                    }

                    inline RVector2& operator = ( const float fScalar)
                    {
                        x = fScalar;
                        y = fScalar;

                        return *this;
                    }

                    inline bool operator == ( const RVector2& rkVector ) const
                    {
                        return ( x == rkVector.x && y == rkVector.y );
                    }

                    inline bool operator != ( const RVector2& rkVector ) const
                    {
                        return ( x != rkVector.x || y != rkVector.y  );
                    }

                    // arithmetic operations
                    inline RVector2 operator + ( const RVector2& rkVector ) const
                    {
                        return RVector2(
                                x + rkVector.x,
                                y + rkVector.y);
                    }

                    inline RVector2 operator - ( const RVector2& rkVector ) const
                    {
                        return RVector2(
                                x - rkVector.x,
                                y - rkVector.y);
                    }

                    inline RVector2 operator * ( const float fScalar ) const
                    {
                        return RVector2(
                                x * fScalar,
                                y * fScalar);
                    }

                    inline RVector2 operator * ( const RVector2& rhs) const
                    {
                        return RVector2(
                                x * rhs.x,
                                y * rhs.y);
                    }

                    inline RVector2 operator / ( const float fScalar ) const
                    {
                        assert( fScalar != 0.0 );

                        float fInv = 1.0f / fScalar;

                        return RVector2(
                                x * fInv,
                                y * fInv);
                    }

                    inline RVector2 operator / ( const RVector2& rhs) const
                    {
                        return RVector2(
                                x / rhs.x,
                                y / rhs.y);
                    }

                    inline const RVector2& operator + () const
                    {
                        return *this;
                    }

                    inline RVector2 operator - () const
                    {
                        return RVector2(-x, -y);
                    }

                    // overloaded operators to help RVector2
                    inline friend RVector2 operator * ( const float fScalar, const RVector2& rkVector )
                    {
                        return RVector2(
                                fScalar * rkVector.x,
                                fScalar * rkVector.y);
                    }

                    inline friend RVector2 operator / ( const float fScalar, const RVector2& rkVector )
                    {
                        return RVector2(
                                fScalar / rkVector.x,
                                fScalar / rkVector.y);
                    }

                    inline friend RVector2 operator + (const RVector2& lhs, const float rhs)
                    {
                        return RVector2(
                                lhs.x + rhs,
                                lhs.y + rhs);
                    }

                    inline friend RVector2 operator + (const float lhs, const RVector2& rhs)
                    {
                        return RVector2(
                                lhs + rhs.x,
                                lhs + rhs.y);
                    }

                    inline friend RVector2 operator - (const RVector2& lhs, const float rhs)
                    {
                        return RVector2(
                                lhs.x - rhs,
                                lhs.y - rhs);
                    }

                    inline friend RVector2 operator - (const float lhs, const RVector2& rhs)
                    {
                        return RVector2(
                                lhs - rhs.x,
                                lhs - rhs.y);
                    }

                    // arithmetic updates
                    inline RVector2& operator += ( const RVector2& rkVector )
                    {
                        x += rkVector.x;
                        y += rkVector.y;

                        return *this;
                    }

                    inline RVector2& operator += ( const float fScaler )
                    {
                        x += fScaler;
                        y += fScaler;

                        return *this;
                    }

                    inline RVector2& operator -= ( const RVector2& rkVector )
                    {
                        x -= rkVector.x;
                        y -= rkVector.y;

                        return *this;
                    }

                    inline RVector2& operator -= ( const float fScaler )
                    {
                        x -= fScaler;
                        y -= fScaler;

                        return *this;
                    }

                    inline RVector2& operator *= ( const float fScalar )
                    {
                        x *= fScalar;
                        y *= fScalar;

                        return *this;
                    }

                    inline RVector2& operator *= ( const RVector2& rkVector )
                    {
                        x *= rkVector.x;
                        y *= rkVector.y;

                        return *this;
                    }

                    inline RVector2& operator /= ( const float fScalar )
                    {
                        assert( fScalar != 0.0 );

                        float fInv = 1.0f / fScalar;

                        x *= fInv;
                        y *= fInv;

                        return *this;
                    }

                    inline RVector2& operator /= ( const RVector2& rkVector )
                    {
                        x /= rkVector.x;
                        y /= rkVector.y;

                        return *this;
                    }

                    /** Returns the length (magnitude) of the vector.
                    @warning
                    This operation requires a square root and is expensive in
                    terms of CPU operations. If you don't need to know the exact
                    length (e.g. for just comparing lengths) use squaredLength()
                    instead.
                    */
                    inline float length () const
                    {
                        return sqrt( x * x + y * y );
                    }

                    /** Returns the square of the length(magnitude) of the vector.
                    @remarks
                    This  method is for efficiency - calculating the actual
                    length of a vector requires a square root, which is expensive
                    in terms of the operations required. This method returns the
                    square of the length of the vector, i.e. the same as the
                    length but before the square root is taken. Use this if you
                    want to find the longest / shortest vector without incurring
                    the square root.
                    */
                    inline float squaredLength () const
                    {
                        return x * x + y * y;
                    }

                    /** Returns the distance to another vector.
                    @warning
                    This operation requires a square root and is expensive in
                    terms of CPU operations. If you don't need to know the exact
                    distance (e.g. for just comparing distances) use squaredDistance()
                    instead.
                    */
                    inline float distance(const RVector2& rhs) const
                    {
                        return (*this - rhs).length();
                    }

                    /** Returns the square of the distance to another vector.
                    @remarks
                    This method is for efficiency - calculating the actual
                    distance to another vector requires a square root, which is
                    expensive in terms of the operations required. This method
                    returns the square of the distance to another vector, i.e.
                    the same as the distance but before the square root is taken.
                    Use this if you want to find the longest / shortest distance
                    without incurring the square root.
                    */
                    inline float squaredDistance(const RVector2& rhs) const
                    {
                        return (*this - rhs).squaredLength();
                    }

                    /** Calculates the dot (scalar) product of this vector with another.
                    @remarks
                    The dot product can be used to calculate the angle between 2
                    vectors. If both are unit vectors, the dot product is the
                    cosine of the angle; otherwise the dot product must be
                    divided by the product of the lengths of both vectors to get
                    the cosine of the angle. This result can further be used to
                    calculate the distance of a point from a plane.
                    @param
                    vec Vector with which to calculate the dot product (together
                    with this one).
                    @return
                    A float representing the dot product value.
                    */
                    inline float dotProduct(const RVector2& vec) const
                    {
                        return x * vec.x + y * vec.y;
                    }

                    /** Normalises the vector.
                    @remarks
                    This method normalises the vector such that it's
                    length / magnitude is 1. The result is called a unit vector.
                    @note
                    This function will not crash for zero-sized vectors, but there
                    will be no changes made to their components.
                    @return The previous length of the vector.
                    */

                    inline float normalise()
                    {
                        float fLength = sqrt( x * x + y * y);

                        // Will also work for zero-sized vectors, but will change nothing
                        // We're not using epsilons because we don't need to.
                        // Read http://www.ogre3d.org/forums/viewtopic.php?f=4&t=61259
                        if ( fLength > float(0.0f) )
                        {
                            float fInvLength = 1.0f / fLength;
                            x *= fInvLength;
                            y *= fInvLength;
                        }

                        return fLength;
                    }

                    /** Returns a vector at a point half way between this and the passed
                    in vector.
                    */
                    inline RVector2 midPoint( const RVector2& vec ) const
                    {
                        return RVector2(
                                ( x + vec.x ) * 0.5f,
                                ( y + vec.y ) * 0.5f );
                    }

                    /** Returns true if the vector's scalar components are all greater
                    that the ones of the vector it is compared against.
                    */
                    inline bool operator < ( const RVector2& rhs ) const
                    {
                        if( x < rhs.x && y < rhs.y )
                            return true;
                        return false;
                    }

                    /** Returns true if the vector's scalar components are all smaller
                    that the ones of the vector it is compared against.
                    */
                    inline bool operator > ( const RVector2& rhs ) const
                    {
                        if( x > rhs.x && y > rhs.y )
                            return true;
                        return false;
                    }

                    /** Sets this vector's components to the minimum of its own and the
                    ones of the passed in vector.
                    @remarks
                    'Minimum' in this case means the combination of the lowest
                    value of x, y and z from both vectors. Lowest is taken just
                    numerically, not magnitude, so -1 < 0.
                    */
                    inline void makeFloor( const RVector2& cmp )
                    {
                        if( cmp.x < x ) x = cmp.x;
                        if( cmp.y < y ) y = cmp.y;
                    }

                    /** Sets this vector's components to the maximum of its own and the
                    ones of the passed in vector.
                    @remarks
                    'Maximum' in this case means the combination of the highest
                    value of x, y and z from both vectors. Highest is taken just
                    numerically, not magnitude, so 1 > -3.
                    */
                    inline void makeCeil( const RVector2& cmp )
                    {
                        if( cmp.x > x ) x = cmp.x;
                        if( cmp.y > y ) y = cmp.y;
                    }

                    /** Generates a vector perpendicular to this vector (eg an 'up' vector).
                    @remarks
                    This method will return a vector which is perpendicular to this
                    vector. There are an infinite number of possibilities but this
                    method will guarantee to generate one of them. If you need more
                    control you should use the Quaternion class.
                    */
                    inline RVector2 perpendicular(void) const
                    {
                        return RVector2 (-y, x);
                    }

                    /** Calculates the 2 dimensional cross-product of 2 vectors, which results
                    in a single floating point value which is 2 times the area of the triangle.
                    */
                    inline float crossProduct( const RVector2& rkVector ) const
                    {
                        return x * rkVector.y - y * rkVector.x;
                    }

                    /** Generates a new random vector which deviates from this vector by a
                    given angle in a random direction.
                    @remarks
                    This method assumes that the random number generator has already
                    been seeded appropriately.
                    @param angle
                    The angle at which to deviate in radians
                    @return
                    A random vector which deviates from this vector by angle. This
                    vector will not be normalised, normalise it if you wish
                    afterwards.
                    */
                    inline RVector2 randomDeviant(float angle) const
                    {
                        angle *= random();
                        float cosa = cos(angle);
                        float sina = sin(angle);
                        return RVector2(cosa * x - sina * y,
                                sina * x + cosa * y);
                    }

                    /** Returns true if this vector is zero length. */
                    inline bool isZeroLength(void) const
                    {
                        float sqlen = (x * x) + (y * y);
                        return (sqlen < (1e-06 * 1e-06));

                    }

                    /** As normalise, except that this vector is unaffected and the
                    normalised vector is returned as a copy. */
                    inline RVector2 normalisedCopy(void) const
                    {
                        RVector2 ret = *this;
                        ret.normalise();
                        return ret;
                    }

                    /** Calculates a reflection vector to the plane with the given normal .
                    @remarks NB assumes 'this' is pointing AWAY FROM the plane, invert if it is not.
                    */
                    inline RVector2 reflect(const RVector2& normal) const
                    {
                        return RVector2( *this - ( 2 * this->dotProduct(normal) * normal ) );
                    }

                    /// Check whether this vector contains valid values
                    inline bool isNaN() const
                    {
                        return NaN(x) || NaN(y);
                    }

                    /**  Gets the angle between 2 vectors.
                    @remarks
                    Vectors do not have to be unit-length but must represent directions.
                    */
                    inline float angleBetween(const RVector2& other) const
                    {
                        float lenProduct = length() * other.length();
                        // Divide by zero check
                        if(lenProduct < 1e-6f)
                            lenProduct = 1e-6f;

                        float f = dotProduct(other) / lenProduct;

                        f = clamp(f, (float)-1.0, (float)1.0);
                        return acos(f);

                    }

                    /**  Gets the oriented angle between 2 vectors.
                    @remarks
                    Vectors do not have to be unit-length but must represent directions.
                    The angle is comprised between 0 and 2 PI.
                    */
                    inline float angleTo(const RVector2& other) const
                    {
                        float angle = angleBetween(other);

                        if (crossProduct(other)<0)
                            angle = (float)TWO_PI - angle;

                        return angle;
                    }

                    // special points
                    static const RVector2 ZERO;
                    static const RVector2 UNIT_X;
                    static const RVector2 UNIT_Y;
                    static const RVector2 NEGATIVE_UNIT_X;
                    static const RVector2 NEGATIVE_UNIT_Y;
                    static const RVector2 UNIT_SCALE;

                    /** Function for writing to a stream.
                    */
                    inline friend std::ostream& operator <<
                    ( std::ostream& o, const RVector2& v )
                    {
                        o << "RVector2(" << v.x << ", " << v.y <<  ")";
                        return o;
                    }
            };
}
#endif