

#ifndef RMatrixH
#define RMatrixH

#include "../reactor.h"
namespace Reactor
{
    class RMatrix
    {
    public:
        static const float MATRIX_IDENTITY[16] =
                {
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, 1.0f, 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                };
        float m[16];

        RMatrix() {
            *this = RMatrix::identity();
        }

        RMatrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
                float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44) {
            set(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }

        RMatrix(const float* m){
            set(m);
        }

        RMatrix(const RMatrix& copy){
            memcpy(m, copy.m, MATRIX_SIZE);
        }

        ~RMatrix(){}

        static const RMatrix& identity(){
            static RMatrix m(
                    1, 0, 0, 0,
                    0, 1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1 );
            return m;
        }

        static const RMatrix& zero(){
            static RMatrix m(
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0,
                    0, 0, 0, 0 );
            return m;
        }

        static void createLookAt(const RVector3& eyePosition, const RVector3& targetPosition, const RVector3& up, RMatrix* dst){
            createLookAt(eyePosition.x, eyePosition.y, eyePosition.z, targetPosition.x, targetPosition.y, targetPosition.z,
                    up.x, up.y, up.z, dst);
        }

        static void createLookAt(float eyePositionX, float eyePositionY, float eyePositionZ,
                float targetCenterX, float targetCenterY, float targetCenterZ,
                float upX, float upY, float upZ, RMatrix* dst){

            RVector3 eye(eyePositionX, eyePositionY, eyePositionZ);
            RVector3 target(targetPositionX, targetPositionY, targetPositionZ);
            RVector3 up(upX, upY, upZ);
            up.normalize();

            RVector3 zaxis;
            RVector3::subtract(eye, target, &zaxis);
            zaxis.normalize();

            RVector3 xaxis;
            RVector3::cross(up, zaxis, &xaxis);
            xaxis.normalize();

            RVector3 yaxis;
            RVector3::cross(zaxis, xaxis, &yaxis);
            yaxis.normalize();

            dst->m[0] = xaxis.x;
            dst->m[1] = yaxis.x;
            dst->m[2] = zaxis.x;
            dst->m[3] = 0.0f;

            dst->m[4] = xaxis.y;
            dst->m[5] = yaxis.y;
            dst->m[6] = zaxis.y;
            dst->m[7] = 0.0f;

            dst->m[8] = xaxis.z;
            dst->m[9] = yaxis.z;
            dst->m[10] = zaxis.z;
            dst->m[11] = 0.0f;

            dst->m[12] = -RVector3::dot(xaxis, eye);
            dst->m[13] = -RVector3::dot(yaxis, eye);
            dst->m[14] = -RVector3::dot(zaxis, eye);
            dst->m[15] = 1.0f;
        }

        static void createPerspective(float fieldOfView, float aspectRatio, float zNearPlane, float zFarPlane, RMatrix* dst){

            float f_n = 1.0f / (zFarPlane - zNearPlane);
            float theta = MATH_DEG_TO_RAD(fieldOfView) * 0.5f;
            if (fabs(fmod(theta, MATH_PIOVER2)) < MATH_EPSILON)
            {
                GP_ERROR("Invalid field of view value (%d) causes attempted calculation tan(%d), which is undefined.", fieldOfView, theta);
                return;
            }
            float divisor = tan(theta);
            GP_ASSERT(divisor);
            float factor = 1.0f / divisor;

            memset(dst, 0, MATRIX_SIZE);

            dst->m[0] = (1.0f / aspectRatio) * factor;
            dst->m[5] = factor;
            dst->m[10] = (-(zFarPlane + zNearPlane)) * f_n;
            dst->m[11] = -1.0f;
            dst->m[14] = -2.0f * zFarPlane * zNearPlane * f_n;
        }

        static void createOrthographic(float width, float height, float zNearPlane, float zFarPlane, RMatrix* dst){
            float halfWidth = width / 2.0f;
            float halfHeight = height / 2.0f;
            createOrthographicOffCenter(-halfWidth, halfWidth, -halfHeight, halfHeight, zNearPlane, zFarPlane, dst);
        }

        static void createOrthographicOffCenter(float left, float right, float bottom, float top,
                float zNearPlane, float zFarPlane, RMatrix* dst){

            memset(dst, 0, MATRIX_SIZE);
            dst->m[0] = 2 / (right - left);
            dst->m[5] = 2 / (top - bottom);
            dst->m[12] = (left + right) / (left - right);
            dst->m[10] = 1 / (zNearPlane - zFarPlane);
            dst->m[13] = (top + bottom) / (bottom - top);
            dst->m[14] = zNearPlane / (zNearPlane - zFarPlane);
            dst->m[15] = 1;
        }

        static void createBillboard(const RVector3& objectPosition, const RVector3& cameraPosition,
                const RVector3& cameraUpVector, RMatrix* dst){
            createBillboardHelper(objectPosition, cameraPosition, cameraUpVector, NULL, dst);
        }

        static void createBillboard(const RVector3& objectPosition, const RVector3& cameraPosition,
                const RVector3& cameraUpVector, const RVector3& cameraForwardVector,
                RMatrix* dst){
            createBillboardHelper(objectPosition, cameraPosition, cameraUpVector, &cameraForwardVector, dst);
        }

        static void createReflection(const Plane& plane, RMatrix* dst){
            RVector3 normal(plane.getNormal());
            float k = -2.0f * plane.getDistance();

            dst->setIdentity();

            dst->m[0] -= 2.0f * normal.x * normal.x;
            dst->m[5] -= 2.0f * normal.y * normal.y;
            dst->m[10] -= 2.0f * normal.z * normal.z;
            dst->m[1] = dst->m[4] = -2.0f * normal.x * normal.y;
            dst->m[2] = dst->m[8] = -2.0f * normal.x * normal.z;
            dst->m[6] = dst->m[9] = -2.0f * normal.y * normal.z;

            dst->m[3] = k * normal.x;
            dst->m[7] = k * normal.y;
            dst->m[11] = k * normal.z;
        }

        static void createScale(const RVector3& scale, RMatrix* dst){

            memcpy(dst, MATRIX_IDENTITY, MATRIX_SIZE);

            dst->m[0] = scale.x;
            dst->m[5] = scale.y;
            dst->m[10] = scale.z;
        }

        static void createScale(float xScale, float yScale, float zScale, RMatrix* dst){

            memcpy(dst, MATRIX_IDENTITY, MATRIX_SIZE);

            dst->m[0] = xScale;
            dst->m[5] = yScale;
            dst->m[10] = zScale;
        }

        static void createRotation(const Quaternion& quat, RMatrix* dst){

            float x2 = q.x + q.x;
            float y2 = q.y + q.y;
            float z2 = q.z + q.z;

            float xx2 = q.x * x2;
            float yy2 = q.y * y2;
            float zz2 = q.z * z2;
            float xy2 = q.x * y2;
            float xz2 = q.x * z2;
            float yz2 = q.y * z2;
            float wx2 = q.w * x2;
            float wy2 = q.w * y2;
            float wz2 = q.w * z2;

            dst->m[0] = 1.0f - yy2 - zz2;
            dst->m[1] = xy2 + wz2;
            dst->m[2] = xz2 - wy2;
            dst->m[3] = 0.0f;

            dst->m[4] = xy2 - wz2;
            dst->m[5] = 1.0f - xx2 - zz2;
            dst->m[6] = yz2 + wx2;
            dst->m[7] = 0.0f;

            dst->m[8] = xz2 + wy2;
            dst->m[9] = yz2 - wx2;
            dst->m[10] = 1.0f - xx2 - yy2;
            dst->m[11] = 0.0f;

            dst->m[12] = 0.0f;
            dst->m[13] = 0.0f;
            dst->m[14] = 0.0f;
            dst->m[15] = 1.0f;
        }

        static void createRotation(const RVector3& axis, float angle, RMatrix* dst){

            float x = axis.x;
            float y = axis.y;
            float z = axis.z;

            // Make sure the input axis is normalized.
            float n = x*x + y*y + z*z;
            if (n != 1.0f)
            {
                // Not normalized.
                n = sqrt(n);
                // Prevent divide too close to zero.
                if (n > 0.000001f)
                {
                    n = 1.0f / n;
                    x *= n;
                    y *= n;
                    z *= n;
                }
            }

            float c = cos(angle);
            float s = sin(angle);

            float t = 1.0f - c;
            float tx = t * x;
            float ty = t * y;
            float tz = t * z;
            float txy = tx * y;
            float txz = tx * z;
            float tyz = ty * z;
            float sx = s * x;
            float sy = s * y;
            float sz = s * z;

            dst->m[0] = c + tx*x;
            dst->m[1] = txy + sz;
            dst->m[2] = txz - sy;
            dst->m[3] = 0.0f;

            dst->m[4] = txy - sz;
            dst->m[5] = c + ty*y;
            dst->m[6] = tyz + sx;
            dst->m[7] = 0.0f;

            dst->m[8] = txz + sy;
            dst->m[9] = tyz - sx;
            dst->m[10] = c + tz*z;
            dst->m[11] = 0.0f;

            dst->m[12] = 0.0f;
            dst->m[13] = 0.0f;
            dst->m[14] = 0.0f;
            dst->m[15] = 1.0f;
        }

        static void createRotationX(float angle, RMatrix* dst){

            memcpy(dst, MATRIX_IDENTITY, MATRIX_SIZE);

            float c = cos(angle);
            float s = sin(angle);

            dst->m[5]  = c;
            dst->m[6]  = s;
            dst->m[9]  = -s;
            dst->m[10] = c;
        }

        static void createRotationY(float angle, RMatrix* dst){

            memcpy(dst, MATRIX_IDENTITY, MATRIX_SIZE);

            float c = cos(angle);
            float s = sin(angle);

            dst->m[0]  = c;
            dst->m[2]  = -s;
            dst->m[8]  = s;
            dst->m[10] = c;
        }

        static void createRotationZ(float angle, RMatrix* dst){

            memcpy(dst, MATRIX_IDENTITY, MATRIX_SIZE);

            float c = cos(angle);
            float s = sin(angle);

            dst->m[0] = c;
            dst->m[1] = s;
            dst->m[4] = -s;
            dst->m[5] = c;
        }

        static void createTranslation(const RVector3& translation, RMatrix* dst){

            memcpy(dst, MATRIX_IDENTITY, MATRIX_SIZE);

            dst->m[12] = translation.x;
            dst->m[13] = translation.y;
            dst->m[14] = translation.z;
        }

        static void createTranslation(float xTranslation, float yTranslation, float zTranslation, RMatrix* dst){

            memcpy(dst, MATRIX_IDENTITY, MATRIX_SIZE);

            dst->m[12] = xTranslation;
            dst->m[13] = yTranslation;
            dst->m[14] = zTranslation;
        }

        void add(float scalar){
            add(scalar, this);
        }

        void add(float scalar, RMatrix* dst){
            RMathUtils::addMatrix(m, scalar, dst->m);
        }

        void add(const RMatrix& m){
            add(*this, m, this);
        }

        static void add(const RMatrix& m1, const RMatrix& m2, RMatrix* dst){
            GP_ASSERT(dst);

            RMathUtils::addMatrix(m1.m, m2.m, dst->m);
        }

        bool decompose(RVector3* scale, Quaternion* rotation, RVector3* translation) const
        {
            if (translation)
            {
                // Extract the translation.
                translation->x = m[12];
                translation->y = m[13];
                translation->z = m[14];
            }

            // Nothing left to do.
            if (scale == NULL && rotation == NULL)
                return true;

            // Extract the scale.
            // This is simply the length of each axis (row/column) in the matrix.
            RVector3 xaxis(m[0], m[1], m[2]);
            float scaleX = xaxis.length();

            RVector3 yaxis(m[4], m[5], m[6]);
            float scaleY = yaxis.length();

            RVector3 zaxis(m[8], m[9], m[10]);
            float scaleZ = zaxis.length();

            // Determine if we have a negative scale (true if determinant is less than zero).
            // In this case, we simply negate a single axis of the scale.
            float det = determinant();
            if (det < 0)
                scaleZ = -scaleZ;

            if (scale)
            {
                scale->x = scaleX;
                scale->y = scaleY;
                scale->z = scaleZ;
            }

            // Nothing left to do.
            if (rotation == NULL)
                return true;

            // Scale too close to zero, can't decompose rotation.
            if (scaleX < MATH_TOLERANCE || scaleY < MATH_TOLERANCE || fabs(scaleZ) < MATH_TOLERANCE)
                return false;

            float rn;

            // Factor the scale out of the matrix axes.
            rn = 1.0f / scaleX;
            xaxis.x *= rn;
            xaxis.y *= rn;
            xaxis.z *= rn;

            rn = 1.0f / scaleY;
            yaxis.x *= rn;
            yaxis.y *= rn;
            yaxis.z *= rn;

            rn = 1.0f / scaleZ;
            zaxis.x *= rn;
            zaxis.y *= rn;
            zaxis.z *= rn;

            // Now calculate the rotation from the resulting matrix (axes).
            float trace = xaxis.x + yaxis.y + zaxis.z + 1.0f;

            if (trace > MATH_EPSILON)
            {
                float s = 0.5f / sqrt(trace);
                rotation->w = 0.25f / s;
                rotation->x = (yaxis.z - zaxis.y) * s;
                rotation->y = (zaxis.x - xaxis.z) * s;
                rotation->z = (xaxis.y - yaxis.x) * s;
            }
            else
            {
                // Note: since xaxis, yaxis, and zaxis are normalized,
                // we will never divide by zero in the code below.
                if (xaxis.x > yaxis.y && xaxis.x > zaxis.z)
                {
                    float s = 0.5f / sqrt(1.0f + xaxis.x - yaxis.y - zaxis.z);
                    rotation->w = (yaxis.z - zaxis.y) * s;
                    rotation->x = 0.25f / s;
                    rotation->y = (yaxis.x + xaxis.y) * s;
                    rotation->z = (zaxis.x + xaxis.z) * s;
                }
                else if (yaxis.y > zaxis.z)
                {
                    float s = 0.5f / sqrt(1.0f + yaxis.y - xaxis.x - zaxis.z);
                    rotation->w = (zaxis.x - xaxis.z) * s;
                    rotation->x = (yaxis.x + xaxis.y) * s;
                    rotation->y = 0.25f / s;
                    rotation->z = (zaxis.y + yaxis.z) * s;
                }
                else
                {
                    float s = 0.5f / sqrt(1.0f + zaxis.z - xaxis.x - yaxis.y );
                    rotation->w = (xaxis.y - yaxis.x ) * s;
                    rotation->x = (zaxis.x + xaxis.z ) * s;
                    rotation->y = (zaxis.y + yaxis.z ) * s;
                    rotation->z = 0.25f / s;
                }
            }

            return true;
        }

        float determinant() const {
            float a0 = m[0] * m[5] - m[1] * m[4];
            float a1 = m[0] * m[6] - m[2] * m[4];
            float a2 = m[0] * m[7] - m[3] * m[4];
            float a3 = m[1] * m[6] - m[2] * m[5];
            float a4 = m[1] * m[7] - m[3] * m[5];
            float a5 = m[2] * m[7] - m[3] * m[6];
            float b0 = m[8] * m[13] - m[9] * m[12];
            float b1 = m[8] * m[14] - m[10] * m[12];
            float b2 = m[8] * m[15] - m[11] * m[12];
            float b3 = m[9] * m[14] - m[10] * m[13];
            float b4 = m[9] * m[15] - m[11] * m[13];
            float b5 = m[10] * m[15] - m[11] * m[14];

            // Calculate the determinant.
            return (a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0);
        }

        void getScale(RVector3* scale) const{
            decompose(scale, NULL, NULL);
        }

        bool getRotation(Quaternion* rotation) const{
            return decompose(NULL, rotation, NULL);
        }

        void getTranslation(RVector3* translation) const{
            return decompose(NULL, NULL, translation);
        }

        void getUpVector(RVector3* dst) const
        {
            dst->x = m[4];
            dst->y = m[5];
            dst->z = m[6];
        }

        void getDownVector(RVector3* dst) const
        {
            dst->x = -m[4];
            dst->y = -m[5];
            dst->z = -m[6];
        }

        void getLeftVector(RVector3* dst) const
        {
            dst->x = -m[0];
            dst->y = -m[1];
            dst->z = -m[2];
        }

        void getRightVector(RVector3* dst) const
        {
            dst->x = m[0];
            dst->y = m[1];
            dst->z = m[2];
        }

        void getForwardVector(RVector3* dst) const
        {
            dst->x = -m[8];
            dst->y = -m[9];
            dst->z = -m[10];
        }

        void getBackVector(RVector3* dst) const
        {
            dst->x = m[8];
            dst->y = m[9];
            dst->z = m[10];
        }

        bool invert(){
            return invert(this);
        }

        bool invert(RMatrix* dst) const
        {
            float a0 = m[0] * m[5] - m[1] * m[4];
            float a1 = m[0] * m[6] - m[2] * m[4];
            float a2 = m[0] * m[7] - m[3] * m[4];
            float a3 = m[1] * m[6] - m[2] * m[5];
            float a4 = m[1] * m[7] - m[3] * m[5];
            float a5 = m[2] * m[7] - m[3] * m[6];
            float b0 = m[8] * m[13] - m[9] * m[12];
            float b1 = m[8] * m[14] - m[10] * m[12];
            float b2 = m[8] * m[15] - m[11] * m[12];
            float b3 = m[9] * m[14] - m[10] * m[13];
            float b4 = m[9] * m[15] - m[11] * m[13];
            float b5 = m[10] * m[15] - m[11] * m[14];

            // Calculate the determinant.
            float det = a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;

            // Close to zero, can't invert.
            if (fabs(det) <= MATH_TOLERANCE)
                return false;

            // Support the case where m == dst.
            RMatrix inverse;
            inverse.m[0]  = m[5] * b5 - m[6] * b4 + m[7] * b3;
            inverse.m[1]  = -m[1] * b5 + m[2] * b4 - m[3] * b3;
            inverse.m[2]  = m[13] * a5 - m[14] * a4 + m[15] * a3;
            inverse.m[3]  = -m[9] * a5 + m[10] * a4 - m[11] * a3;

            inverse.m[4]  = -m[4] * b5 + m[6] * b2 - m[7] * b1;
            inverse.m[5]  = m[0] * b5 - m[2] * b2 + m[3] * b1;
            inverse.m[6]  = -m[12] * a5 + m[14] * a2 - m[15] * a1;
            inverse.m[7]  = m[8] * a5 - m[10] * a2 + m[11] * a1;

            inverse.m[8]  = m[4] * b4 - m[5] * b2 + m[7] * b0;
            inverse.m[9]  = -m[0] * b4 + m[1] * b2 - m[3] * b0;
            inverse.m[10] = m[12] * a4 - m[13] * a2 + m[15] * a0;
            inverse.m[11] = -m[8] * a4 + m[9] * a2 - m[11] * a0;

            inverse.m[12] = -m[4] * b3 + m[5] * b1 - m[6] * b0;
            inverse.m[13] = m[0] * b3 - m[1] * b1 + m[2] * b0;
            inverse.m[14] = -m[12] * a3 + m[13] * a1 - m[14] * a0;
            inverse.m[15] = m[8] * a3 - m[9] * a1 + m[10] * a0;

            multiply(inverse, 1.0f / det, dst);

            return true;
        }

        bool isIdentity() const{
            return (memcmp(m, MATRIX_IDENTITY, MATRIX_SIZE) == 0);
        }

        void multiply(float scalar){
            multiply(scalar, this);
        }

        void multiply(float scalar, RMatrix* dst) const{
            multiply(*this, scalar, dst);
        }

        static void multiply(const RMatrix& m, float scalar, RMatrix* dst){
            GP_ASSERT(dst);

            RMathUtils::multiplyMatrix(m.m, scalar, dst->m);
        }

        void multiply(const RMatrix& m){
            multiply(*this, m, this);
        }

        static void multiply(const RMatrix& m1, const RMatrix& m2, RMatrix* dst){
            GP_ASSERT(dst);

            RMathUtils::multiplyMatrix(m1.m, m2.m, dst->m);
        }

        void negate(){
            negate(this);
        }

        void negate(RMatrix* dst) const{
            GP_ASSERT(dst);

            RMathUtils::negateMatrix(m, dst->m);
        }

        void rotate(const Quaternion& q){
            rotate(q, this);
        }

        void rotate(const Quaternion& q, RMatrix* dst) const{
            RMatrix r;
            createRotation(q, &r);
            multiply(*this, r, dst);
        }

        void rotate(const RVector3& axis, float angle){
            rotate(axis, angle, this);
        }

        void rotate(const RVector3& axis, float angle, RMatrix* dst) const{
            RMatrix r;
            createRotation(axis, angle, &r);
            multiply(*this, r, dst);
        }

        void rotateX(float angle){
            rotateX(angle, this);
        }

        void rotateX(float angle, RMatrix* dst) const{
            RMatrix r;
            createRotationX(angle, &r);
            multiply(*this, r, dst);
        }

        void rotateY(float angle){
            rotateY(angle, this);
        }

        void rotateY(float angle, RMatrix* dst) const{
            RMatrix r;
            createRotationY(angle, &r);
            multiply(*this, r, dst);
        }

        void rotateZ(float angle){
            rotateZ(angle, this);
        }

        void rotateZ(float angle, RMatrix* dst) const{
            RMatrix r;
            createRotationZ(angle, &r);
            multiply(*this, r, dst);
        }

        void scale(float value){
            scale(value, this);
        }

        void scale(float value, RMatrix* dst) const{
            scale(value, value, value, dst);
        }

        void scale(float xScale, float yScale, float zScale){
            scale(xScale, yScale, zScale, this);
        }

        void scale(float xScale, float yScale, float zScale, RMatrix* dst) const{
            RMatrix s;
            createScale(xScale, yScale, zScale, &s);
            multiply(*this, s, dst);
        }

        void scale(const RVector3& s){
            scale(s.x, s.y, s.z, this);
        }

        void scale(const RVector3& s, RMatrix* dst) const{
            scale(s.x, s.y, s.z, dst);
        }

        void set(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
                float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44){
            m[0]  = m11;
            m[1]  = m21;
            m[2]  = m31;
            m[3]  = m41;
            m[4]  = m12;
            m[5]  = m22;
            m[6]  = m32;
            m[7]  = m42;
            m[8]  = m13;
            m[9]  = m23;
            m[10] = m33;
            m[11] = m43;
            m[12] = m14;
            m[13] = m24;
            m[14] = m34;
            m[15] = m44;
        }

        void set(const float* m){
            GP_ASSERT(m);
            memcpy(this->m, m, MATRIX_SIZE);
        }

        void set(const RMatrix& m){
            memcpy(this->m, m.m, MATRIX_SIZE);
        }

        void setIdentity(){
            memcpy(m, MATRIX_IDENTITY, MATRIX_SIZE);
        }

        void setZero(){
            memset(m, 0, MATRIX_SIZE);
        }

        void subtract(const RMatrix& m){
            subtract(*this, m, this);
        }

        static void subtract(const RMatrix& m1, const RMatrix& m2, RMatrix* dst){
            GP_ASSERT(dst);

            RMathUtils::subtractMatrix(m1.m, m2.m, dst->m);
        }

        void transformPoint(RVector3* point) const{
            GP_ASSERT(point);
            transformVector(point->x, point->y, point->z, 1.0f, point);
        }

        void transformPoint(const RVector3& point, RVector3* dst) const{
            transformVector(point.x, point.y, point.z, 1.0f, dst);
        }

        void transformVector(RVector3* vector) const{
            GP_ASSERT(vector);
            transformVector(vector->x, vector->y, vector->z, 0.0f, vector);
        }

        void transformVector(const RVector3& vector, RVector3* dst) const{
            transformVector(vector.x, vector.y, vector.z, 0.0f, dst);
        }

        void transformVector(float x, float y, float z, float w, RVector3* dst) const{
            GP_ASSERT(dst);

            RMathUtils::transformR4DVECTOR(m, x, y, z, w, (float*)dst);
        }

        void transformVector(R4DVECTOR* vector) const{
            GP_ASSERT(vector);
            transformVector(*vector, vector);
        }

        void transformVector(const R4DVECTOR& vector, R4DVECTOR* dst) const{
            GP_ASSERT(dst);

            RMathUtils::transformR4DVECTOR(m, (const float*) &vector, (float*)dst);
        }

        void translate(float x, float y, float z){
            translate(x, y, z, this);
        }

        void translate(float x, float y, float z, RMatrix* dst) const{
            RMatrix t;
            createTranslation(x, y, z, &t);
            multiply(*this, t, dst);
        }

        void translate(const RVector3& t){
            translate(t.x, t.y, t.z, this);
        }

        void translate(const RVector3& t, RMatrix* dst) const{
            translate(t.x, t.y, t.z, dst);
        }

        void transpose(){
            transpose(this);
        }

        void transpose(RMatrix* dst) const{
            GP_ASSERT(dst);

            RMathUtils::transposeMatrix(m, dst->m);
        }

        inline const RMatrix operator+(const RMatrix& m) const{
            RMatrix result(*this);
            result.add(m);
            return result;
        }

        inline RMatrix& operator+=(const RMatrix& m){
            add(m);
            return *this;
        }

        inline const RMatrix operator-(const RMatrix& m) const{
            RMatrix result(*this);
            result.subtract(m);
            return result;
        }

        inline RMatrix& operator-=(const RMatrix& m){
            subtract(m);
            return *this;
        }

        inline const RMatrix operator-() const{
            RMatrix m(*this);
            m.negate();
            return m;
        }

        inline const RMatrix operator*(const RMatrix& m) const{
            RMatrix result(*this);
            result.multiply(m);
            return result;
        }

        inline RMatrix& operator*=(const RMatrix& m){
            multiply(m);
            return *this;
        }

    private:

        static void createBillboardHelper(const RVector3& objectPosition, const RVector3& cameraPosition,
                const RVector3& cameraUpVector, const RVector3* cameraForwardVector,
                RMatrix* dst){
            RVector3 delta(objectPosition-cameraPosition);
            bool isSufficientDelta = delta.lengthSquared() > MATH_EPSILON;

            dst->setIdentity();
            dst->m[3] = objectPosition.x;
            dst->m[7] = objectPosition.y;
            dst->m[11] = objectPosition.z;

            // As per the contracts for the 2 variants of createBillboard, we need
            // either a safe default or a sufficient distance between object and camera.
            if (cameraForwardVector || isSufficientDelta)
            {
                RVector3 target = isSufficientDelta ? cameraPosition : (objectPosition - *cameraForwardVector);

                // A billboard is the inverse of a lookAt rotation
                RMatrix lookAt;
                createLookAt(objectPosition, target, cameraUpVector, &lookAt);
                dst->m[0] = lookAt.m[0];
                dst->m[1] = lookAt.m[4];
                dst->m[2] = lookAt.m[8];
                dst->m[4] = lookAt.m[1];
                dst->m[5] = lookAt.m[5];
                dst->m[6] = lookAt.m[9];
                dst->m[8] = lookAt.m[2];
                dst->m[9] = lookAt.m[6];
                dst->m[10] = lookAt.m[10];
            }
        }
    };

    inline RVector3& operator*=(RVector3& v, const RMatrix& m){
        m.transformVector(&v);
        return v;
    }


    inline const RVector3 operator*(const RMatrix& m, const RVector3& v){
        RVector3 x;
        m.transformVector(v, &x);
        return x;
    }


    inline R4DVECTOR& operator*=(R4DVECTOR& v, const RMatrix& m){
        m.transformVector(&v);
        return v;
    }


    inline const R4DVECTOR operator*(const RMatrix& m, const R4DVECTOR& v){
        R4DVECTOR x;
        m.transformVector(v, &x);
        return x;
    }

}
}
#endif