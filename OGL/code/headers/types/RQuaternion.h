

#ifndef RQUATERNIONH
#define RQUATERNIONH

#include "../reactor.h"
#include "RMatrix.h"

namespace Reactor
{
    class RQuaternion
            {
                    public:
                    /// Default constructor, initializes to identity rotation (aka 0°)
                    inline RQuaternion ()
                    : w(1), x(0), y(0), z(0)
                    {
                    }
                    /// Construct from an explicit list of values
                    inline RQuaternion (
                    float fW,
                    float fX, float fY, float fZ)
                    : w(fW), x(fX), y(fY), z(fZ)
                    {
                    }
                    /// Construct a quaternion from a rotation matrix
                    inline RQuaternion(const RMatrix& rot)
                    {
                        this->FromRotationMatrix(rot);
                    }
                    /// Construct a quaternion from an angle/axis
                    inline RQuaternion(const float& rfAngle, const RVector3& rkAxis)
                    {
                        this->FromAngleAxis(rfAngle, rkAxis);
                    }
                    /// Construct a quaternion from 3 orthonormal local axes
                    inline RQuaternion(const RVector3& xaxis, const RVector3& yaxis, const RVector3& zaxis)
                    {
                        this->FromAxes(xaxis, yaxis, zaxis);
                    }
                    /// Construct a quaternion from 3 orthonormal local axes
                    inline RQuaternion(const RVector3* akAxis)
                    {
                        this->FromAxes(akAxis);
                    }
                    /// Construct a quaternion from 4 manual w/x/y/z values
                    inline RQuaternion(float* valptr)
                    {
                        memcpy(&w, valptr, sizeof(float)*4);
                    }

                    /** Exchange the contents of this quaternion with another.
                    */
                    inline void swap(RQuaternion& other)
                    {
                        std::swap(w, other.w);
                        std::swap(x, other.x);
                        std::swap(y, other.y);
                        std::swap(z, other.z);
                    }

                    /// Array accessor operator
                    inline float operator [] ( const size_t i ) const
                    {
                        assert( i < 4 );

                        return *(&w+i);
                    }

                    /// Array accessor operator
                    inline float& operator [] ( const size_t i )
                    {
                        assert( i < 4 );

                        return *(&w+i);
                    }

                    /// Pointer accessor for direct copying
                    inline float* ptr()
                    {
                        return &w;
                    }

                    /// Pointer accessor for direct copying
                    inline const float* ptr() const
                    {
                        return &w;
                    }

                    void FromRotationMatrix (const RMatrix& kRot){
                        // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
                        // article "Quaternion Calculus and Fast Animation".

                        float fTrace = kRot[0][0]+kRot[1][1]+kRot[2][2];
                        float fRoot;

                        if ( fTrace > 0.0 )
                        {
                            // |w| > 1/2, may as well choose w > 1/2
                            fRoot = sqrt(fTrace + 1.0f);  // 2w
                            w = 0.5f*fRoot;
                            fRoot = 0.5f/fRoot;  // 1/(4w)
                            x = (kRot[2][1]-kRot[1][2])*fRoot;
                            y = (kRot[0][2]-kRot[2][0])*fRoot;
                            z = (kRot[1][0]-kRot[0][1])*fRoot;
                        }
                        else
                        {
                            // |w| <= 1/2
                            static size_t s_iNext[3] = { 1, 2, 0 };
                            size_t i = 0;
                            if ( kRot[1][1] > kRot[0][0] )
                                i = 1;
                            if ( kRot[2][2] > kRot[i][i] )
                                i = 2;
                            size_t j = s_iNext[i];
                            size_t k = s_iNext[j];

                            fRoot = sqrt(kRot[i][i]-kRot[j][j]-kRot[k][k] + 1.0f);
                            float* apkQuat[3] = { &x, &y, &z };
                            *apkQuat[i] = 0.5f*fRoot;
                            fRoot = 0.5f/fRoot;
                            w = (kRot[k][j]-kRot[j][k])*fRoot;
                            *apkQuat[j] = (kRot[j][i]+kRot[i][j])*fRoot;
                            *apkQuat[k] = (kRot[k][i]+kRot[i][k])*fRoot;
                        }
                    }
                    void ToRotationMatrix (RMatrix& kRot) const{
                        float fTx  = x+x;
                        float fTy  = y+y;
                        float fTz  = z+z;
                        float fTwx = fTx*w;
                        float fTwy = fTy*w;
                        float fTwz = fTz*w;
                        float fTxx = fTx*x;
                        float fTxy = fTy*x;
                        float fTxz = fTz*x;
                        float fTyy = fTy*y;
                        float fTyz = fTz*y;
                        float fTzz = fTz*z;

                        kRot[0][0] = 1.0f-(fTyy+fTzz);
                        kRot[0][1] = fTxy-fTwz;
                        kRot[0][2] = fTxz+fTwy;
                        kRot[1][0] = fTxy+fTwz;
                        kRot[1][1] = 1.0f-(fTxx+fTzz);
                        kRot[1][2] = fTyz-fTwx;
                        kRot[2][0] = fTxz-fTwy;
                        kRot[2][1] = fTyz+fTwx;
                        kRot[2][2] = 1.0f-(fTxx+fTyy);
                    }
                    /** Setups the quaternion using the supplied vector, and "roll" around
                    that vector by the specified radians.
                    */
                    void FromAngleAxis (const float& rfAngle, const RVector3& rkAxis){
                        // assert:  axis[] is unit length
                        //
                        // The quaternion representing the rotation is
                        //   q = cos(A/2)+sin(A/2)*(x*i+y*j+z*k)

                        float fHalfAngle ( 0.5*rfAngle );
                        float fSin = sin(fHalfAngle);
                        w = cos(fHalfAngle);
                        x = fSin*rkAxis.x;
                        y = fSin*rkAxis.y;
                        z = fSin*rkAxis.z;
                    }
                    inline void ToAngleAxis (float& rfAngle, RVector3& rkAxis) const{
                        // The quaternion representing the rotation is
                        //   q = cos(A/2)+sin(A/2)*(x*i+y*j+z*k)

                        float fSqrLength = x*x+y*y+z*z;
                        if ( fSqrLength > 0.0 )
                        {
                            rfAngle = 2.0*acos(w);
                            float fInvLength = sqrt(fSqrLength) * -1.0f;
                            rkAxis.x = x*fInvLength;
                            rkAxis.y = y*fInvLength;
                            rkAxis.z = z*fInvLength;
                        }
                        else
                        {
                            // angle is 0 (mod 2*pi), so any axis will do
                            rfAngle = 0.0;
                            rkAxis.x = 1.0;
                            rkAxis.y = 0.0;
                            rkAxis.z = 0.0;
                        }
                    }

                    /** Constructs the quaternion using 3 axes, the axes are assumed to be orthonormal
                    @see FromAxes
                    */
                    void FromAxes (const RVector3* akAxis){
                        RMatrix kRot;

                        for (size_t iCol = 0; iCol < 3; iCol++)
                        {
                            kRot.m[4*iCol] = akAxis[iCol].x;
                            kRot.m[4*iCol+1] = akAxis[iCol].y;
                            kRot.m[4*iCol+2] = akAxis[iCol].z;
                        }

                        FromRotationMatrix(kRot);
                    }
                    void FromAxes (const RVector3& xAxis, const RVector3& yAxis, const RVector3& zAxis){
                        RMatrix kRot;

                        kRot.m[0] = xaxis.x;
                        kRot.m[1] = xaxis.y;
                        kRot.m[2] = xaxis.z;

                        kRot.m[4] = yaxis.x;
                        kRot.m[5] = yaxis.y;
                        kRot.m[6] = yaxis.z;

                        kRot.m[8] = zaxis.x;
                        kRot.m[9] = zaxis.y;
                        kRot.m[10] = zaxis.z;

                        FromRotationMatrix(kRot);

                    }
                    /** Gets the 3 orthonormal axes defining the quaternion. @see FromAxes */
                    void ToAxes (RVector3* akAxis) const{
                        RMatrix kRot;

                        ToRotationMatrix(kRot);

                        for (size_t iCol = 0; iCol < 3; iCol++)
                        {
                            akAxis[iCol].x = kRot.m[4*iCol];
                            akAxis[iCol].y = kRot.m[4*iCol+1];
                            akAxis[iCol].z = kRot.m[4*iCol+2];
                        }
                    }
                    void ToAxes (RVector3& xAxis, RVector3& yAxis, RVector3& zAxis) const{
                        RMatrix kRot;

                        ToRotationMatrix(kRot);

                        xaxis.x = kRot.m[0];
                        xaxis.y = kRot.m[1];
                        xaxis.z = kRot.m[2];

                        yaxis.x = kRot.m[4];
                        yaxis.y = kRot.m[5];
                        yaxis.z = kRot.m[6];

                        zaxis.x = kRot.m[8];
                        zaxis.y = kRot.m[9];
                        zaxis.z = kRot.m[10];
                    }

                    /** Returns the X orthonormal axis defining the quaternion. Same as doing
                    xAxis = RVector3::UNIT_X * this. Also called the local X-axis
                    */
                    RVector3 xAxis() const{
                        //float fTx  = 2.0*x;
                        float fTy  = 2.0f*y;
                        float fTz  = 2.0f*z;
                        float fTwy = fTy*w;
                        float fTwz = fTz*w;
                        float fTxy = fTy*x;
                        float fTxz = fTz*x;
                        float fTyy = fTy*y;
                        float fTzz = fTz*z;

                        return RVector3(1.0f-(fTyy+fTzz), fTxy+fTwz, fTxz-fTwy);
                    }

                    /** Returns the Y orthonormal axis defining the quaternion. Same as doing
                    yAxis = RVector3::UNIT_Y * this. Also called the local Y-axis
                    */
                    RVector3 yAxis() const{
                        float fTx  = 2.0f*x;
                        float fTy  = 2.0f*y;
                        float fTz  = 2.0f*z;
                        float fTwx = fTx*w;
                        float fTwz = fTz*w;
                        float fTxx = fTx*x;
                        float fTxy = fTy*x;
                        float fTyz = fTz*y;
                        float fTzz = fTz*z;

                        return RVector3(fTxy-fTwz, 1.0f-(fTxx+fTzz), fTyz+fTwx);
                    }

                    /** Returns the Z orthonormal axis defining the quaternion. Same as doing
                    zAxis = RVector3::UNIT_Z * this. Also called the local Z-axis
                    */
                    RVector3 zAxis(void) const{
                        float fTx  = 2.0f*x;
                        float fTy  = 2.0f*y;
                        float fTz  = 2.0f*z;
                        float fTwx = fTx*w;
                        float fTwy = fTy*w;
                        float fTxx = fTx*x;
                        float fTxz = fTz*x;
                        float fTyy = fTy*y;
                        float fTyz = fTz*y;

                        return RVector3(fTxz+fTwy, fTyz-fTwx, 1.0f-(fTxx+fTyy));
                    }

                    inline RQuaternion& operator= (const RQuaternion& rkQ)
                    {
                        w = rkQ.w;
                        x = rkQ.x;
                        y = rkQ.y;
                        z = rkQ.z;
                        return *this;
                    }
                    RQuaternion operator+ (const RQuaternion& rkQ) const{
                        return RQuaternion(w+rkQ.w,x+rkQ.x,y+rkQ.y,z+rkQ.z);
                    }
                    RQuaternion operator- (const RQuaternion& rkQ) const{
                        return RQuaternion(w-rkQ.w,x-rkQ.x,y-rkQ.y,z-rkQ.z);
                    }
                    RQuaternion operator* (const RQuaternion& rkQ) const{
                        // NOTE:  Multiplication is not generally commutative, so in most
                        // cases p*q != q*p.

                        return RQuaternion
                                (
                                        w * rkQ.w - x * rkQ.x - y * rkQ.y - z * rkQ.z,
                                        w * rkQ.x + x * rkQ.w + y * rkQ.z - z * rkQ.y,
                                        w * rkQ.y + y * rkQ.w + z * rkQ.x - x * rkQ.z,
                                        w * rkQ.z + z * rkQ.w + x * rkQ.y - y * rkQ.x
                                );
                    }
                    RQuaternion operator* (float fScalar) const{
                        return RQuaternion(fScalar*w,fScalar*x,fScalar*y,fScalar*z);
                    }
                    friend RQuaternion operator* (float fScalar, const RQuaternion& rkQ){
                        return RQuaternion(fScalar*rkQ.w,fScalar*rkQ.x,fScalar*rkQ.y, fScalar*rkQ.z);
                    }
                    RQuaternion operator- () const{
                        return RQuaternion(-w,-x,-y,-z);
                    }
                    inline bool operator== (const RQuaternion& rhs) const
                    {
                        return (rhs.x == x) && (rhs.y == y) &&
                                (rhs.z == z) && (rhs.w == w);
                    }
                    inline bool operator!= (const RQuaternion& rhs) const
                    {
                        return !operator==(rhs);
                    }
                    // functions of a quaternion
                    /// Returns the dot product of the quaternion
                    float Dot (const RQuaternion& rkQ) const{
                        return w*rkQ.w+x*rkQ.x+y*rkQ.y+z*rkQ.z;
                    }
                    /* Returns the normal length of this quaternion.
                        @note This does <b>not</b> alter any values.
                    */
                    float Norm () const{
                        return w*w+x*x+y*y+z*z;
                    }
                    /// Normalises this quaternion, and returns the previous length
                    float normalise(void){
                        float len = Norm();
                        float factor = 1.0f / sqrt(len);
                        *this = *this * factor;
                        return len;
                    }
                    RQuaternion Inverse () const{
                        float fNorm = w*w+x*x+y*y+z*z;
                        if ( fNorm > 0.0 )
                        {
                            float fInvNorm = 1.0f/fNorm;
                            return RQuaternion(w*fInvNorm,-x*fInvNorm,-y*fInvNorm,-z*fInvNorm);
                        }
                        else
                        {
                            // return an invalid result to flag the error
                            return ZERO;
                        }
                    }
                    RQuaternion UnitInverse () const{
                        // assert:  'this' is unit length
                        return RQuaternion(w,-x,-y,-z);
                    }
                    RQuaternion Exp () const{
                        // If q = A*(x*i+y*j+z*k) where (x,y,z) is unit length, then
                        // exp(q) = cos(A)+sin(A)*(x*i+y*j+z*k).  If sin(A) is near zero,
                        // use exp(q) = cos(A)+A*(x*i+y*j+z*k) since A/sin(A) has limit 1.

                        float fAngle = sqrt(x*x+y*y+z*z);
                        float fSin = sin(fAngle);

                        RQuaternion kResult;
                        kResult.w = cos(fAngle);

                        if ( abs(fSin) >= msEpsilon )
                        {
                            float fCoeff = fSin/fAngle;
                            kResult.x = fCoeff*x;
                            kResult.y = fCoeff*y;
                            kResult.z = fCoeff*z;
                        }
                        else
                        {
                            kResult.x = x;
                            kResult.y = y;
                            kResult.z = z;
                        }

                        return kResult;
                    }
                    RQuaternion Log () const{
                        // If q = cos(A)+sin(A)*(x*i+y*j+z*k) where (x,y,z) is unit length, then
                        // log(q) = A*(x*i+y*j+z*k).  If sin(A) is near zero, use log(q) =
                        // sin(A)*(x*i+y*j+z*k) since sin(A)/A has limit 1.

                        RQuaternion kResult;
                        kResult.w = 0.0;

                        if ( abs(w) < 1.0 )
                        {
                            float fAngle = acos(w);
                            float fSin = sin(fAngle);
                            if ( abs(fSin) >= msEpsilon )
                            {
                                float fCoeff = fAngle/fSin;
                                kResult.x = fCoeff*x;
                                kResult.y = fCoeff*y;
                                kResult.z = fCoeff*z;
                                return kResult;
                            }
                        }

                        kResult.x = x;
                        kResult.y = y;
                        kResult.z = z;

                        return kResult;
                    }

                    /// Rotation of a vector by a quaternion
                    RVector3 operator* (const RVector3& rkVector) const{
                        // nVidia SDK implementation
                        RVector3 uv, uuv;
                        RVector3 qvec(x, y, z);
                        uv = qvec.crossProduct(v);
                        uuv = qvec.crossProduct(uv);
                        uv *= (2.0f * w);
                        uuv *= 2.0f;

                        return v + uv + uuv;

                    }

                    /** Calculate the local roll element of this quaternion.
                    @param reprojectAxis By default the method returns the 'intuitive' result
                    that is, if you projected the local Y of the quaternion onto the X and
                    Y axes, the angle between them is returned. If set to false though, the
                    result is the actual yaw that will be used to implement the quaternion,
                    which is the shortest possible path to get to the same orientation and
                    may involve less axial rotation.  The co-domain of the returned value is
                    from -180 to 180 degrees.
                    */
                    float getRoll(bool reprojectAxis = true) const{
                        if (reprojectAxis)
                        {
                            // roll = atan2(localx.y, localx.x)
                            // pick parts of xAxis() implementation that we need
                            float fTy  = 2.0f*y;
                            float fTz  = 2.0f*z;
                            float fTwz = fTz*w;
                            float fTxy = fTy*x;
                            float fTyy = fTy*y;
                            float fTzz = fTz*z;

                            // Vector3(1.0-(fTyy+fTzz), fTxy+fTwz, fTxz-fTwy);

                            return float(atan2(fTxy+fTwz, 1.0f-(fTyy+fTzz)));

                        }
                        else
                        {
                            return float(atan2(2*(x*y + w*z), w*w + x*x - y*y - z*z));
                        }
                    }
                    /** Calculate the local pitch element of this quaternion
                    @param reprojectAxis By default the method returns the 'intuitive' result
                    that is, if you projected the local Z of the quaternion onto the X and
                    Y axes, the angle between them is returned. If set to true though, the
                    result is the actual yaw that will be used to implement the quaternion,
                    which is the shortest possible path to get to the same orientation and
                    may involve less axial rotation.  The co-domain of the returned value is
                    from -180 to 180 degrees.
                    */
                    float getPitch(bool reprojectAxis = true) const{
                        if (reprojectAxis)
                        {
                            // pitch = atan2(localy.z, localy.y)
                            // pick parts of yAxis() implementation that we need
                            float fTx  = 2.0f*x;
                            float fTz  = 2.0f*z;
                            float fTwx = fTx*w;
                            float fTxx = fTx*x;
                            float fTyz = fTz*y;
                            float fTzz = fTz*z;

                            // Vector3(fTxy-fTwz, 1.0-(fTxx+fTzz), fTyz+fTwx);
                            return float(atan2(fTyz+fTwx, 1.0f-(fTxx+fTzz)));
                        }
                        else
                        {
                            // internal version
                            return float(atan2(2*(y*z + w*x), w*w - x*x - y*y + z*z));
                        }
                    }
                    /** Calculate the local yaw element of this quaternion
                    @param reprojectAxis By default the method returns the 'intuitive' result
                    that is, if you projected the local Y of the quaternion onto the X and
                    Z axes, the angle between them is returned. If set to true though, the
                    result is the actual yaw that will be used to implement the quaternion,
                    which is the shortest possible path to get to the same orientation and
                    may involve less axial rotation. The co-domain of the returned value is
                    from -180 to 180 degrees.
                    */
                    float getYaw(bool reprojectAxis = true) const{
                        if (reprojectAxis)
                        {
                            // yaw = atan2(localz.x, localz.z)
                            // pick parts of zAxis() implementation that we need
                            float fTx  = 2.0f*x;
                            float fTy  = 2.0f*y;
                            float fTz  = 2.0f*z;
                            float fTwy = fTy*w;
                            float fTxx = fTx*x;
                            float fTxz = fTz*x;
                            float fTyy = fTy*y;

                            // Vector3(fTxz+fTwy, fTyz-fTwx, 1.0-(fTxx+fTyy));

                            return float(atan2(fTxz+fTwy, 1.0f-(fTxx+fTyy)));

                        }
                        else
                        {
                            // internal version
                            return float(asin(-2*(x*z - w*y)));
                        }
                    }

                    /** Equality with tolerance (tolerance is max angle difference)
                    @remark Both equals() and orientationEquals() measure the exact same thing.
                    One measures the difference by angle, the other by a different, non-linear metric.
                    */
                    bool equals(const RQuaternion& rhs, const float& tolerance) const{
                        float d = Dot(rhs);
                        float angle = acos(2.0f * d*d - 1.0f);

                        return abs(angle) <= tolerance;
                    }

                    /** Compare two quaternions which are assumed to be used as orientations.
                    @remark Both equals() and orientationEquals() measure the exact same thing.
                    One measures the difference by angle, the other by a different, non-linear metric.
                    @return true if the two orientations are the same or very close, relative to the given tolerance.
                    Slerp ( 0.75f, A, B ) != Slerp ( 0.25f, B, A );
                    therefore be careful if your code relies in the order of the operands.
                    This is specially important in IK animation.
                    */
                    inline bool orientationEquals( const RQuaternion& other, float tolerance = 1e-3 ) const
                    {
                        float d = this->Dot(other);
                        return 1 - d*d < tolerance;
                    }

                    /** Performs Spherical linear interpolation between two quaternions, and returns the result.
                    Slerp ( 0.0f, A, B ) = A
                    Slerp ( 1.0f, A, B ) = B
                    @return Interpolated quaternion
                    @remarks
                    Slerp has the proprieties of performing the interpolation at constant
                    velocity, and being torque-minimal (unless shortestPath=false).
                    However, it's NOT commutative, which means
                    Slerp ( 0.75f, A, B ) != Slerp ( 0.25f, B, A );
                    therefore be careful if your code relies in the order of the operands.
                    This is specially important in IK animation.
                    */
                    static RQuaternion Slerp (float fT, const RQuaternion& rkP,
                    const RQuaternion& rkQ, bool shortestPath = false){
                        float fCos = rkP.Dot(rkQ);
                        RQuaternion rkT;

                        // Do we need to invert rotation?
                        if (fCos < 0.0f && shortestPath)
                        {
                            fCos = -fCos;
                            rkT = -rkQ;
                        }
                        else
                        {
                            rkT = rkQ;
                        }

                        if (abs(fCos) < 1 - msEpsilon)
                        {
                            // Standard case (slerp)
                            float fSin = sqrt(1 - sqrt(fCos));
                            float fAngle = atan2(fSin, fCos);
                            float fInvSin = 1.0f / fSin;
                            float fCoeff0 = sin((1.0f - fT) * fAngle) * fInvSin;
                            float fCoeff1 = sin(fT * fAngle) * fInvSin;
                            return fCoeff0 * rkP + fCoeff1 * rkT;
                        }
                        else
                        {
                            // There are two situations:
                            // 1. "rkP" and "rkQ" are very close (fCos ~= +1), so we can do a linear
                            //    interpolation safely.
                            // 2. "rkP" and "rkQ" are almost inverse of each other (fCos ~= -1), there
                            //    are an infinite number of possibilities interpolation. but we haven't
                            //    have method to fix this case, so just use linear interpolation here.
                            RQuaternion t = (1.0f - fT) * rkP + fT * rkT;
                            // taking the complement requires renormalisation
                            t.normalise();
                            return t;
                        }
                    }

                    /** @see Slerp. It adds extra "spins" (i.e. rotates several times) specified
                    by parameter 'iExtraSpins' while interpolating before arriving to the
                    final values
                    */
                    static RQuaternion SlerpExtraSpins (float fT,
                    const RQuaternion& rkP, const RQuaternion& rkQ,
                    int iExtraSpins){
                        float fCos = rkP.Dot(rkQ);
                        float fAngle ( acos(fCos) );

                        if ( abs(fAngle) < msEpsilon )
                            return rkP;

                        float fSin = sin(fAngle);
                        float fPhase ( PI*iExtraSpins*fT );
                        float fInvSin = 1.0f/fSin;
                        float fCoeff0 = sin((1.0f-fT)*fAngle - fPhase)*fInvSin;
                        float fCoeff1 = sin(fT*fAngle + fPhase)*fInvSin;
                        return fCoeff0*rkP + fCoeff1*rkQ;
                    }

                    /// Setup for spherical quadratic interpolation
                    static void Intermediate (const RQuaternion& rkQ0,
                    const RQuaternion& rkQ1, const RQuaternion& rkQ2,
                    RQuaternion& rka, RQuaternion& rkB){
                        // assert:  q0, q1, q2 are unit quaternions

                        RQuaternion kQ0inv = rkQ0.UnitInverse();
                        RQuaternion kQ1inv = rkQ1.UnitInverse();
                        RQuaternion rkP0 = kQ0inv*rkQ1;
                        RQuaternion rkP1 = kQ1inv*rkQ2;
                        RQuaternion kArg = 0.25*(rkP0.Log()-rkP1.Log());
                        RQuaternion kMinusArg = -kArg;

                        rkA = rkQ1*kArg.Exp();
                        rkB = rkQ1*kMinusArg.Exp();
                    }

                    /// Spherical quadratic interpolation
                    static RQuaternion Squad (float fT, const RQuaternion& rkP,
                    const RQuaternion& rkA, const RQuaternion& rkB,
                    const RQuaternion& rkQ, bool shortestPath = false){
                        float fSlerpT = 2.0f*fT*(1.0f-fT);
                        RQuaternion kSlerpP = Slerp(fT, rkP, rkQ, shortestPath);
                        RQuaternion kSlerpQ = Slerp(fT, rkA, rkB);
                        return Slerp(fSlerpT, kSlerpP ,kSlerpQ);
                    }

                    /** Performs Normalised linear interpolation between two quaternions, and returns the result.
                    nlerp ( 0.0f, A, B ) = A
                    nlerp ( 1.0f, A, B ) = B
                    @remarks
                    Nlerp is faster than Slerp.
                    Nlerp has the proprieties of being commutative (@see Slerp;
                    commutativity is desired in certain places, like IK animation), and
                    being torque-minimal (unless shortestPath=false). However, it's performing
                    the interpolation at non-constant velocity; sometimes this is desired,
                    sometimes it is not. Having a non-constant velocity can produce a more
                    natural rotation feeling without the need of tweaking the weights; however
                    if your scene relies on the timing of the rotation or assumes it will point
                    at a specific angle at a specific weight value, Slerp is a better choice.
                    */
                    static RQuaternion nlerp(float fT, const RQuaternion& rkP,
                    const RQuaternion& rkQ, bool shortestPath = false){
                        RQuaternion result;
                        float fCos = rkP.Dot(rkQ);
                        if (fCos < 0.0f && shortestPath)
                        {
                            result = rkP + fT * ((-rkQ) - rkP);
                        }
                        else
                        {
                            result = rkP + fT * (rkQ - rkP);
                        }
                        result.normalise();
                        return result;
                    }

                    /// Cutoff for sine near zero
                    static const float msEpsilon;

                    // special values
                    static const RQuaternion ZERO;
                    static const RQuaternion IDENTITY;

                    float w, x, y, z;

                    /// Check whether this quaternion contains valid values
                    inline bool isNaN() const
                    {
                        return Math::isNaN(x) || Math::isNaN(y) || Math::isNaN(z) || Math::isNaN(w);
                    }

                    /** Function for writing to a stream. Outputs "RQuaternion(w, x, y, z)" with w,x,y,z
                    being the member values of the quaternion.
                    */
                    inline friend std::ostream& operator <<
                    ( std::ostream& o, const RQuaternion& q )
                    {
                        o << "RQuaternion(" << q.w << ", " << q.x << ", " << q.y << ", " << q.z << ")";
                        return o;
                    }

            };
}
#endif