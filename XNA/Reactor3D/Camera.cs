/*
 * Reactor 3D MIT License
 * 
 * Copyright (c) 2010 Reiser Games
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace Reactor
{
    /// <remarks>
    /// Interface for a general purpose 3D camera. The default XNA right handed
    /// coordinate system is assumed. All angles are measured in degrees. The
    /// default position of the camera is at the world origin. The default
    /// camera orientation is looking straight down the world negative Z axis.
    /// </remarks>
    public interface ICamera
    {
        #region Public Methods

        /// <summary>
        /// Builds a look at style viewing matrix using the camera's current
        /// world position, and its current local y axis.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        void LookAt(R3DVECTOR target);

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>

        /// <param name="target">The target position to look at.</param>
        /// <param name="up">The up direction.</param>
        void LookAt(R3DVECTOR eye, R3DVECTOR target, R3DVECTOR up);

        /// <summary>
        /// Moves the camera by dx world units to the left or right; dy
        /// world units upwards or downwards; and dz world units forwards
        /// or backwards.
        /// </summary>
        /// <param name="dx">Distance to move left or right.</param>
        /// <param name="dy">Distance to move up or down.</param>
        /// <param name="dz">Distance to move forwards or backwards.</param>
        void Move(float dx, float dy, float dz);

        /// <summary>
        /// Moves the camera the specified distance in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        /// <param name="distance">How far to move.</param>
        void Move(R3DVECTOR direction, R3DVECTOR distance);

        /// <summary>
        /// Rotates the camera. Positive angles specify counter clockwise
        /// rotations when looking down the axis of rotation towards the
        /// origin.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        /// <param name="rollDegrees">Z axis rotation in degrees.</param>
        void Rotate(float headingDegrees, float pitchDegrees, float rollDegrees);

        /// <summary>
        /// Zooms the camera. This method functions differently depending on
        /// the camera's current behavior. When the camera is orbiting this
        /// method will move the camera closer to or further away from the
        /// orbit target. For the other camera behaviors this method will
        /// change the camera's horizontal field of view.
        /// </summary>
        ///
        /// <param name="zoom">
        /// When orbiting this parameter is how far to move the camera.
        /// For the other behaviors this parameter is the new horizontal
        /// field of view.
        /// </param>
        /// 
        /// <param name="minZoom">
        /// When orbiting this parameter is the min allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the
        /// min allowed horizontal field of view.
        /// </param>
        /// 
        /// <param name="maxZoom">
        /// When orbiting this parameter is the max allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the max
        /// allowed horizontal field of view.
        /// </param>
        void Zoom(float zoom, float minZoom, float maxZoom);

        #endregion

        #region Properties

        /// <summary>
        /// Property to get and set the camera's orientation.
        /// </summary>
        RQUATERNION Orientation
        {
            get;
            set;
        }

        /// <summary>
        /// Property to get and set the camera's position.
        /// </summary>
        R3DVECTOR Position
        {
            get;
            set;
        }

        /// <summary>
        /// Property to get the camera's perspective projection matrix.
        /// </summary>
        R3DMATRIX ProjectionMatrix
        {
            get;
        }

        /// <summary>
        /// Property to get the camera's viewing direction.
        /// </summary>
        R3DVECTOR ViewDirection
        {
            get;
        }

        /// <summary>
        /// Property to get the camera's view matrix.
        /// </summary>
        R3DMATRIX ViewMatrix
        {
            get;
        }

        /// <summary>
        /// Property to get the camera's concatenated view-projection matrix.
        /// </summary>
        R3DMATRIX ViewProjectionMatrix
        {
            get;
        }

        /// <summary>
        /// Property to get the camera's local x axis.
        /// </summary>
        R3DVECTOR XAxis
        {
            get;
        }

        /// <summary>
        /// Property to get the camera's local y axis.
        /// </summary>
        R3DVECTOR YAxis
        {
            get;
        }

        /// <summary>
        /// Property to get the camera's local z axis.
        /// </summary>
        R3DVECTOR ZAxis
        {
            get;
        }

        #endregion
    }

    public class RCamera : RSceneNode, ICamera, IDisposable
    {
        public enum Behavior
        {
            FirstPerson,
            Spectator,
            Flight,
            Orbit,
            Free
        };

        public const float DEFAULT_FOVX = 90.0f;
        public const float DEFAULT_ZNEAR = 0.1f;
        public const float DEFAULT_ZFAR = 1000.0f;

        public const float DEFAULT_ORBIT_MIN_ZOOM = DEFAULT_ZNEAR + 1.0f;
        public const float DEFAULT_ORBIT_MAX_ZOOM = DEFAULT_ZFAR * 0.5f;

        public const float DEFAULT_ORBIT_OFFSET_LENGTH = DEFAULT_ORBIT_MIN_ZOOM +
            (DEFAULT_ORBIT_MAX_ZOOM - DEFAULT_ORBIT_MIN_ZOOM) * 0.25f;

        private static Vector3 WORLD_X_AXIS = new Vector3(1.0f, 0.0f, 0.0f);
        private static Vector3 WORLD_Y_AXIS = new Vector3(0.0f, 1.0f, 0.0f);
        private static Vector3 WORLD_Z_AXIS = new Vector3(0.0f, 0.0f, 1.0f);

        private Behavior behavior;
        private bool preferTargetYAxisOrbiting;

        private float fovx;
        private float aspectRatio;
        internal float znear;
        internal float zfar;
        private float accumPitchDegrees;
        private float orbitMinZoom;
        private float orbitMaxZoom;
        private float orbitOffsetLength;
        private float firstPersonYOffset;

        private Vector3 eye;
        private Vector3 target;
        private Vector3 targetYAxis;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        internal Vector3 viewDir;

        internal Quaternion orientation;
        internal Matrix viewMatrix;
        internal Matrix projMatrix;
        internal Matrix worldMatrix;
        private Quaternion savedOrientation;
        private Vector3 savedEye;
        private float savedAccumPitchDegrees;

        #region Public Methods

        /// <summary>
        /// Constructs a new instance of the camera class. The camera will
        /// have a flight behavior, and will be initially positioned at the
        /// world origin looking down the world negative z axis.
        /// </summary>
        public RCamera()
        {
            behavior = Behavior.Flight;
            preferTargetYAxisOrbiting = true;

            fovx = DEFAULT_FOVX;
            znear = DEFAULT_ZNEAR;
            zfar = DEFAULT_ZFAR;

            accumPitchDegrees = 0.0f;
            orbitMinZoom = DEFAULT_ORBIT_MIN_ZOOM;
            orbitMaxZoom = DEFAULT_ORBIT_MAX_ZOOM;
            orbitOffsetLength = DEFAULT_ORBIT_OFFSET_LENGTH;
            firstPersonYOffset = 0.0f;

            eye = Vector3.Zero;
            target = Vector3.Zero;
            targetYAxis = Vector3.UnitY;
            xAxis = Vector3.UnitX;
            yAxis = Vector3.UnitY;
            zAxis = Vector3.UnitZ;

            orientation = Quaternion.Identity;
            viewMatrix = Matrix.Identity;

            savedEye = eye;
            savedOrientation = orientation;
            savedAccumPitchDegrees = 0.0f;
        }

        public void Dispose()
        {

        }
        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        public void LookAt(R3DVECTOR target)
        {
            LookAt(R3DVECTOR.FromVector3(eye), target, R3DVECTOR.FromVector3(yAxis));
        }
        public R3DVECTOR Target
        {
            get { return R3DVECTOR.FromVector3(this.target); }

        }
        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="eye">The camera position.</param>
        /// <param name="target">The target position to look at.</param>
        /// <param name="up">The up direction.</param>
        public void LookAt(R3DVECTOR eye, R3DVECTOR target, R3DVECTOR up)
        {
            this.eye = eye.vector;
            this.target = target.vector;

            zAxis = eye.vector - target.vector;
            zAxis.Normalize();

            viewDir.X = -zAxis.X;
            viewDir.Y = -zAxis.Y;
            viewDir.Z = -zAxis.Z;
            viewDir.Normalize();
            Vector3.Cross(ref up.vector, ref zAxis, out xAxis);
            xAxis.Normalize();

            Vector3.Cross(ref zAxis, ref xAxis, out yAxis);
            yAxis.Normalize();
            xAxis.Normalize();

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            Vector3.Dot(ref xAxis, ref eye.vector, out viewMatrix.M41);
            viewMatrix.M41 = -viewMatrix.M41;

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            Vector3.Dot(ref yAxis, ref eye.vector, out viewMatrix.M42);
            viewMatrix.M42 = -viewMatrix.M42;

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            Vector3.Dot(ref zAxis, ref eye.vector, out viewMatrix.M43);
            viewMatrix.M43 = -viewMatrix.M43;

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;

            accumPitchDegrees = MathHelper.ToDegrees((float)Math.Asin(viewMatrix.M23));
            Quaternion.CreateFromRotationMatrix(ref viewMatrix, out orientation);
            
            Pitch = MathHelper.ToDegrees(orientation.X);
            Heading = MathHelper.ToDegrees(orientation.Y);
            Roll = MathHelper.ToDegrees(orientation.Z);
            
        }

        /// <summary>
        /// Moves the camera by dx world units to the left or right; dy
        /// world units upwards or downwards; and dz world units forwards
        /// or backwards.
        /// </summary>
        /// <param name="dx">Distance to move left or right.</param>
        /// <param name="dy">Distance to move up or down.</param>
        /// <param name="dz">Distance to move forwards or backwards.</param>
        public void Move(float dx, float dy, float dz)
        {
            if (behavior == Behavior.Orbit)
            {
                // Orbiting camera is always positioned relative to the target
                // position. See UpdateViewMatrix().
                return;
            }

            Vector3 forwards;

            if (behavior == Behavior.FirstPerson)
            {
                // Calculate the forwards direction. Can't just use the
                // camera's view direction as doing so will cause the camera to
                // move more slowly as the camera's view approaches 90 degrees
                // straight up and down.

                forwards = Vector3.Normalize(Vector3.Cross(WORLD_Y_AXIS, xAxis));
            }
            else
            {
                forwards = viewDir;
            }

            eye += xAxis * dx;
            eye += WORLD_Y_AXIS * dy;
            eye += forwards * dz;

            Position = R3DVECTOR.FromVector3(eye);
        }

        /// <summary>
        /// Moves the camera the specified distance in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        /// <param name="distance">How far to move.</param>
        public void Move(R3DVECTOR direction, R3DVECTOR distance)
        {
            if (behavior == Behavior.Orbit)
            {
                // Orbiting camera is always positioned relative to the target
                // position. See UpdateViewMatrix().
                return;
            }

            eye.X += direction.X * distance.X;
            eye.Y += direction.Y * distance.Y;
            eye.Z += direction.Z * distance.Z;

            UpdateViewMatrix();
        }

        /// <summary>
        /// Builds a perspective projection matrix based on a horizontal field
        /// of view.
        /// </summary>
        /// <param name="fovx">Horizontal field of view in degrees.</param>
        /// <param name="aspect">The viewport's aspect ratio.</param>
        /// <param name="znear">The distance to the near clip plane.</param>
        /// <param name="zfar">The distance to the far clip plane.</param>
        public void Perspective(float fovx, float aspect, float znear, float zfar)
        {
            this.fovx = fovx;
            this.aspectRatio = aspect;
            this.znear = znear;
            this.zfar = zfar;

            float aspectInv = 1.0f / aspect;
            float e = 1.0f / (float)Math.Tan(MathHelper.ToRadians(fovx) / 2.0f);
            float fovy = 2.0f * (float)Math.Atan(aspectInv / e);
            float xScale = 1.0f / (float)Math.Tan(0.5f * fovy);
            float yScale = xScale / aspectInv;

            projMatrix.M11 = xScale;
            projMatrix.M12 = 0.0f;
            projMatrix.M13 = 0.0f;
            projMatrix.M14 = 0.0f;

            projMatrix.M21 = 0.0f;
            projMatrix.M22 = yScale;
            projMatrix.M23 = 0.0f;
            projMatrix.M24 = 0.0f;

            projMatrix.M31 = 0.0f;
            projMatrix.M32 = 0.0f;
            projMatrix.M33 = (zfar + znear) / (znear - zfar);
            projMatrix.M34 = -1.0f;

            projMatrix.M41 = 0.0f;
            projMatrix.M42 = 0.0f;
            projMatrix.M43 = (2.0f * zfar * znear) / (znear - zfar);
            projMatrix.M44 = 0.0f;
        }

        /// <summary>
        /// Rotates the camera. Positive angles specify counter clockwise
        /// rotations when looking down the axis of rotation towards the
        /// origin.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        /// <param name="rollDegrees">Z axis rotation in degrees.</param>
        public void Rotate(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            headingDegrees = -headingDegrees;
            pitchDegrees = -pitchDegrees;
            rollDegrees = -rollDegrees;

            switch (behavior)
            {
                case Behavior.FirstPerson:
                case Behavior.Spectator:
                    RotateFirstPerson(headingDegrees, pitchDegrees);
                    break;

                case Behavior.Flight:
                    RotateFlight(headingDegrees, pitchDegrees, rollDegrees);
                    break;

                case Behavior.Orbit:
                    RotateOrbit(headingDegrees, pitchDegrees, rollDegrees);
                    break;

                case Behavior.Free:
                    RotateFlight(headingDegrees, pitchDegrees, rollDegrees);
                    break;
                default:
                    break;
            }

            UpdateViewMatrix();
        }
        public void RotateX(float value)
        {
            this.Rotation.X = MathHelper.ToRadians(value);
            //float heading = -(value * 0.025f * (float)REngine.Instance._gameTime.ElapsedGameTime.TotalMilliseconds);
            //Quaternion rotation = Quaternion.Identity;
            //Vector3 WXAXIS = WORLD_X_AXIS;
            //Quaternion.CreateFromAxisAngle(ref WXAXIS, heading, out rotation);
            //Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
            Rotate(0, value, 0);
            //UpdateViewMatrix();
        }
        public void RotateY(float value)
        {
            this.Rotation.Y = MathHelper.ToRadians(value);
            Rotate(value, 0, 0);

        }
        public bool InView(RSceneNode node)
        {
            BoundingFrustum frus = new BoundingFrustum(viewMatrix * projMatrix);
            if (frus.Contains(node.Position) != ContainmentType.Disjoint)
                return true;
            else
                return false;
        }
        public bool InView(RMesh node)
        {
            BoundingFrustum frus = new BoundingFrustum(viewMatrix * projMatrix);
            BoundingSphere sphere = new BoundingSphere();
            foreach(ModelMesh mesh in node._model.Meshes)
                sphere = BoundingSphere.CreateMerged(sphere, mesh.BoundingSphere);
            sphere.Radius *= node.Scaling.Length();
            sphere.Center = node.Position;
            if (frus.Contains(sphere) != ContainmentType.Disjoint)
                return true;
            else
                return false;
        }
        public bool InView(RActor node)
        {
            BoundingFrustum frus = new BoundingFrustum(viewMatrix * projMatrix);
            BoundingSphere sphere = new BoundingSphere();
            foreach(ModelMesh mesh in node._model.Meshes)
                sphere = BoundingSphere.CreateMerged(sphere, mesh.BoundingSphere);
            sphere.Radius *= node.Scaling.Length();
            sphere.Center = node.Position;
            if (frus.Contains(sphere) != ContainmentType.Disjoint)
                return true;
            else
                return false;
        }
        public void RotateZ(float value)
        {
            this.Rotation.Z = MathHelper.ToRadians(value);
            Rotate(0, 0, value);
        }
        /// <summary>
        /// Undo any camera rolling by leveling the camera. When the camera is
        /// orbiting this method will cause the camera to become level with the
        /// orbit target.
        /// </summary>
        public void UndoRoll()
        {
            if (behavior == Behavior.Orbit)
                LookAt(R3DVECTOR.FromVector3(eye), R3DVECTOR.FromVector3(target), R3DVECTOR.FromVector3(targetYAxis));
            else if(behavior != Behavior.FirstPerson)
                LookAt(R3DVECTOR.FromVector3(eye), R3DVECTOR.FromVector3(eye + viewDir), R3DVECTOR.FromVector3(WORLD_Y_AXIS));
        }
        public void LevelRoll()
        {
            UndoRoll();
        }
        /// <summary>
        /// Zooms the camera. This method functions differently depending on
        /// the camera's current behavior. When the camera is orbiting this
        /// method will move the camera closer to or further away from the
        /// orbit target. For the other camera behaviors this method will
        /// change the camera's horizontal field of view.
        /// </summary>
        ///
        /// <param name="zoom">
        /// When orbiting this parameter is how far to move the camera.
        /// For the other behaviors this parameter is the new horizontal
        /// field of view.
        /// </param>
        /// 
        /// <param name="minZoom">
        /// When orbiting this parameter is the min allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the
        /// min allowed horizontal field of view.
        /// </param>
        /// 
        /// <param name="maxZoom">
        /// When orbiting this parameter is the max allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the max
        /// allowed horizontal field of view.
        /// </param>
        public void Zoom(float zoom, float minZoom, float maxZoom)
        {
            if (behavior == Behavior.Orbit)
            {
                Vector3 offset = eye - target;

                orbitOffsetLength = offset.Length();
                offset.Normalize();
                orbitOffsetLength += zoom;
                orbitOffsetLength = Math.Min(Math.Max(orbitOffsetLength, minZoom), maxZoom);
                offset *= orbitOffsetLength;
                eye = offset + target;
                UpdateViewMatrix();
            }
            else
            {
                zoom = Math.Min(Math.Max(zoom, minZoom), maxZoom);
                Perspective(zoom, aspectRatio, znear, zfar);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Change to a new camera behavior.
        /// </summary>
        /// <param name="newBehavior">The new camera behavior.</param>
        private void ChangeBehavior(Behavior newBehavior)
        {
            Behavior prevBehavior = behavior;

            if (prevBehavior == newBehavior)
                return;

            behavior = newBehavior;

            switch (newBehavior)
            {
                case Behavior.FirstPerson:
                    switch (prevBehavior)
                    {
                        case Behavior.Free:
                        case Behavior.Flight:
                        case Behavior.Spectator:
                            eye.Y = firstPersonYOffset;
                            UpdateViewMatrix();
                            break;

                        case Behavior.Orbit:
                            eye.X = savedEye.X;
                            eye.Z = savedEye.Z;
                            eye.Y = firstPersonYOffset;
                            orientation = savedOrientation;
                            accumPitchDegrees = savedAccumPitchDegrees;
                            UpdateViewMatrix();
                            break;

                        default:
                            break;
                    }

                    UndoRoll();
                    break;

                case Behavior.Spectator:
                    switch (prevBehavior)
                    {
                        case Behavior.Free:
                            UpdateViewMatrix();
                            break;
                        case Behavior.Flight:
                            UpdateViewMatrix();
                            break;

                        case Behavior.Orbit:
                            eye = savedEye;
                            orientation = savedOrientation;
                            accumPitchDegrees = savedAccumPitchDegrees;
                            UpdateViewMatrix();
                            break;

                        default:
                            break;
                    }

                    UndoRoll();
                    break;

                case Behavior.Flight:
                    if (prevBehavior == Behavior.Orbit)
                    {
                        eye = savedEye;
                        orientation = savedOrientation;
                        accumPitchDegrees = savedAccumPitchDegrees;
                        UpdateViewMatrix();
                    }
                    else
                    {
                        savedEye = eye;
                        UpdateViewMatrix();
                    }
                    break;

                case Behavior.Orbit:
                    if (prevBehavior == Behavior.FirstPerson)
                        firstPersonYOffset = eye.Y;

                    savedEye = eye;
                    savedOrientation = orientation;
                    savedAccumPitchDegrees = accumPitchDegrees;

                    targetYAxis = yAxis;

                    Vector3 newEye = eye + zAxis * orbitOffsetLength;

                    LookAt(R3DVECTOR.FromVector3(newEye), R3DVECTOR.FromVector3(eye), R3DVECTOR.FromVector3(targetYAxis));
                    break;
                case Behavior.Free:
                    if (prevBehavior == Behavior.Orbit)
                    {
                        eye = savedEye;
                        orientation = savedOrientation;
                        accumPitchDegrees = savedAccumPitchDegrees;
                        UpdateViewMatrix();
                    }
                    else
                    {
                        savedEye = eye;
                        UpdateViewMatrix();
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Sets a new camera orientation.
        /// </summary>
        /// <param name="newOrientation">The new orientation.</param>
        private void ChangeOrientation(Quaternion newOrientation)
        {
            Matrix m = Matrix.CreateFromQuaternion(newOrientation);

            // Store the pitch for this new orientation.
            // First person and spectator behaviors limit pitching to
            // 90 degrees straight up and down.

            float pitch = (float)Math.Asin(m.M23);

            accumPitchDegrees = MathHelper.ToDegrees(pitch);

            // First person and spectator behaviors don't allow rolling.
            // Negate any rolling that might be encoded in the new orientation.

            orientation = newOrientation;

            if (behavior == Behavior.FirstPerson || behavior == Behavior.Spectator)
                LookAt(R3DVECTOR.FromVector3(eye), R3DVECTOR.FromVector3(eye + Vector3.Negate(zAxis)), R3DVECTOR.FromVector3(WORLD_Y_AXIS));

            UpdateViewMatrix();
        }

        /// <summary>
        /// Rotates the camera for first person and spectator behaviors.
        /// Pitching is limited to 90 degrees straight up and down.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation angle.</param>
        /// <param name="pitchDegrees">X axis rotation angle.</param>
        public void RotateFirstPerson(float headingDegrees, float pitchDegrees)
        {
            accumPitchDegrees += pitchDegrees;

            if (accumPitchDegrees > 90.0f)
            {
                pitchDegrees = 90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = 90.0f;
            }

            if (accumPitchDegrees < -90.0f)
            {
                pitchDegrees = -90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = -90.0f;
            }

            float heading = MathHelper.ToRadians(headingDegrees);
            float pitch = MathHelper.ToRadians(pitchDegrees);
            Quaternion rotation = Quaternion.Identity;

            // Rotate the camera about the world Y axis.
            if (heading != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_Y_AXIS, heading, out rotation);
                Quaternion.Concatenate(ref rotation, ref orientation, out orientation);
            }

            // Rotate the camera about its local X axis.
            if (pitch != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_X_AXIS, pitch, out rotation);
                Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
            }
        }

        /// <summary>
        /// Rotates the camera for flight behavior.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation angle.</param>
        /// <param name="pitchDegrees">X axis rotation angle.</param>
        /// <param name="rollDegrees">Z axis rotation angle.</param>
        public void RotateFlight(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            accumPitchDegrees += pitchDegrees;

            if (accumPitchDegrees > 360.0f)
                accumPitchDegrees -= 360.0f;

            if (accumPitchDegrees < -360.0f)
                accumPitchDegrees += 360.0f;

            Heading = headingDegrees;
            Pitch = pitchDegrees;
            Roll = rollDegrees;
            float heading = MathHelper.ToRadians(headingDegrees);
            float pitch = MathHelper.ToRadians(pitchDegrees);
            float roll = MathHelper.ToRadians(rollDegrees);

            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(heading, pitch, roll);
            Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
            UpdateViewMatrix();
        }
        public void SetClipPlanes(float znear, float zfar)
        {
            Perspective(this.fovx, REngine.Instance.GetViewport().AspectRatio, znear, zfar);
        }
        public void SetFieldOfView(float value)
        {

            Perspective(value, REngine.Instance.GetViewport().AspectRatio, znear, zfar);
        }
        /// <summary>
        /// Rotates the camera for orbit behavior. Rotations are either about
        /// the camera's local y axis or the orbit target's y axis. The property
        /// PreferTargetYAxisOrbiting controls which rotation method to use.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation angle.</param>
        /// <param name="pitchDegrees">X axis rotation angle.</param>
        /// <param name="rollDegrees">Z axis rotation angle.</param>
        public void RotateOrbit(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            float heading = MathHelper.ToRadians(headingDegrees);
            float pitch = MathHelper.ToRadians(pitchDegrees);

            if (preferTargetYAxisOrbiting)
            {
                Quaternion rotation = Quaternion.Identity;

                if (heading != 0.0f)
                {
                    Quaternion.CreateFromAxisAngle(ref targetYAxis, heading, out rotation);
                    Quaternion.Concatenate(ref rotation, ref orientation, out orientation);
                }

                if (pitch != 0.0f)
                {
                    Quaternion.CreateFromAxisAngle(ref WORLD_X_AXIS, pitch, out rotation);
                    Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
                }
            }
            else
            {
                float roll = MathHelper.ToRadians(rollDegrees);
                Quaternion rotation = Quaternion.CreateFromYawPitchRoll(heading, pitch, roll);
                Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
            }
        }

        /// <summary>
        /// Rebuild the view matrix.
        /// </summary>
        private void UpdateViewMatrix()
        {
            Matrix.CreateFromQuaternion(ref orientation, out viewMatrix);

            xAxis.X = viewMatrix.M11;
            xAxis.Y = viewMatrix.M21;
            xAxis.Z = viewMatrix.M31;

            yAxis.X = viewMatrix.M12;
            yAxis.Y = viewMatrix.M22;
            yAxis.Z = viewMatrix.M32;

            zAxis.X = viewMatrix.M13;
            zAxis.Y = viewMatrix.M23;
            zAxis.Z = viewMatrix.M33;

            if (behavior == Behavior.Orbit)
            {
                // Calculate the new camera position based on the current
                // orientation. The camera must always maintain the same
                // distance from the target. Use the current offset vector
                // to determine the correct distance from the target.

                eye = target + zAxis * orbitOffsetLength;
            }

            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            viewDir.X = -zAxis.X;
            viewDir.Y = -zAxis.Y;
            viewDir.Z = -zAxis.Z;
            viewDir.Normalize();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property to get and set the camera's behavior.
        /// </summary>
        public Behavior CurrentBehavior
        {
            get { return behavior; }
            set { ChangeBehavior(value); }
        }

        /// <summary>
        /// Property to get and set the max orbit zoom distance.
        /// </summary>
        public float OrbitMaxZoom
        {
            get { return orbitMaxZoom; }
            set { orbitMaxZoom = value; }
        }

        /// <summary>
        /// Property to get and set the min orbit zoom distance.
        /// </summary>
        public float OrbitMinZoom
        {
            get { return orbitMinZoom; }
            set { orbitMinZoom = value; }
        }

        /// <summary>
        /// Property to get and set the distance from the target when orbiting.
        /// </summary>
        public float OrbitOffsetDistance
        {
            get { return orbitOffsetLength; }
            set { orbitOffsetLength = value; }
        }
        public float Pitch
        {
            get;
            set;
        }
        public float Heading
        {
            get;
            set;
        }
        public float Roll
        {
            get;
            set;
        }
        /// <summary>
        /// Property to get and set the camera orbit target position.
        /// </summary>
        public R3DVECTOR OrbitTarget
        {
            get { return R3DVECTOR.FromVector3(target); }
            set { target = value.vector; }
        }

        /// <summary>
        /// Property to get and set the camera orientation.
        /// </summary>
        public RQUATERNION Orientation
        {
            get { return RQUATERNION.FromQuaternion(orientation); }
            set { ChangeOrientation(value.quaternion); }
        }

        /// <summary>
        /// Property to get and set the camera position.
        /// </summary>
        public R3DVECTOR Position
        {
            get { return R3DVECTOR.FromVector3(eye); }

            set
            {
                eye = value.vector;
                UpdateViewMatrix();
            }
        }

        /// <summary>
        /// Property to get and set the flag to force the camera
        /// to orbit around the orbit target's Y axis rather than the camera's
        /// local Y axis.
        /// </summary>
        public bool PreferTargetYAxisOrbiting
        {
            get { return preferTargetYAxisOrbiting; }

            set
            {
                preferTargetYAxisOrbiting = value;

                if (preferTargetYAxisOrbiting)
                    UndoRoll();
            }
        }

        /// <summary>
        /// Property to get the perspective projection matrix.
        /// </summary>
        public R3DMATRIX ProjectionMatrix
        {
            get { return R3DMATRIX.FromMatrix(projMatrix); }
        }

        /// <summary>
        /// Property to get the viewing direction vector.
        /// </summary>
        public R3DVECTOR ViewDirection
        {
            get { return R3DVECTOR.FromVector3(viewDir); }
        }

        /// <summary>
        /// Property to get the view matrix.
        /// </summary>
        public R3DMATRIX ViewMatrix
        {
            get { return R3DMATRIX.FromMatrix(viewMatrix); }
            set { viewMatrix = value.matrix; }
        }

        /// <summary>
        /// Property to get the concatenated view-projection matrix.
        /// </summary>
        public R3DMATRIX ViewProjectionMatrix
        {
            get { return R3DMATRIX.FromMatrix(viewMatrix * projMatrix); }
        }

        /// <summary>
        /// Property to get the camera's local X axis.
        /// </summary>
        public R3DVECTOR XAxis
        {
            get { return R3DVECTOR.FromVector3(xAxis); }
        }

        /// <summary>
        /// Property to get the camera's local Y axis.
        /// </summary>
        public R3DVECTOR YAxis
        {
            get { return R3DVECTOR.FromVector3(yAxis); }
        }

        /// <summary>
        /// Property to get the camera's local Z axis.
        /// </summary>
        public R3DVECTOR ZAxis
        {
            get { return R3DVECTOR.FromVector3(zAxis); }
        }

        #endregion
    }
#if !XBOX
    /// <summary>
    /// A general purpose quaternion based camera component for XNA. This
    /// camera component provides the necessary bindings to the XNA framework
    /// to allow the camera to be manipulated by the keyboard, mouse, and game
    /// pad. This camera component is implemented in terms of the Camera class.
    /// As a result the camera component supports all of the features of the
    /// Camera class. The camera component maps input to a series of actions.
    /// These actions are defined by the Actions enumeration. Methods are
    /// provided to remap the camera components default bindings.
    /// </summary>
    internal class RCameraComponent : GameComponent
    {
        public enum Actions
        {
            FlightYawLeftPrimary,
            FlightYawLeftAlternate,
            FlightYawRightPrimary,
            FlightYawRightAlternate,

            MoveForwardsPrimary,
            MoveForwardsAlternate,
            MoveBackwardsPrimary,
            MoveBackwardsAlternate,

            MoveDownPrimary,
            MoveDownAlternate,
            MoveUpPrimary,
            MoveUpAlternate,

            OrbitRollLeftPrimary,
            OrbitRollLeftAlternate,
            OrbitRollRightPrimary,
            OrbitRollRightAlternate,

            PitchUpPrimary,
            PitchUpAlternate,
            PitchDownPrimary,
            PitchDownAlternate,

            YawLeftPrimary,
            YawLeftAlternate,
            YawRightPrimary,
            YawRightAlternate,

            RollLeftPrimary,
            RollLeftAlternate,
            RollRightPrimary,
            RollRightAlternate,

            StrafeRightPrimary,
            StrafeRightAlternate,
            StrafeLeftPrimary,
            StrafeLeftAlternate
        };

        private const float DEFAULT_ACCELERATION_X = 8.0f;
        private const float DEFAULT_ACCELERATION_Y = 8.0f;
        private const float DEFAULT_ACCELERATION_Z = 8.0f;
        private const float DEFAULT_MOUSE_SMOOTHING_SENSITIVITY = 0.5f;
        private const float DEFAULT_SPEED_FLIGHT_YAW = 100.0f;
        private const float DEFAULT_SPEED_MOUSE_WHEEL = 1.0f;
        private const float DEFAULT_SPEED_ORBIT_ROLL = 100.0f;
        private const float DEFAULT_SPEED_ROTATION = 0.3f;
        private const float DEFAULT_VELOCITY_X = 1.0f;
        private const float DEFAULT_VELOCITY_Y = 1.0f;
        private const float DEFAULT_VELOCITY_Z = 1.0f;

        private const int MOUSE_SMOOTHING_CACHE_SIZE = 10;

        internal RCamera camera;
        private bool movingAlongPosX;
        private bool movingAlongNegX;
        private bool movingAlongPosY;
        private bool movingAlongNegY;
        private bool movingAlongPosZ;
        private bool movingAlongNegZ;
        private int savedMousePosX;
        private int savedMousePosY;
        private int mouseIndex;
        private float rotationSpeed;
        private float orbitRollSpeed;
        private float flightYawSpeed;
        private float mouseSmoothingSensitivity;
        private float mouseWheelSpeed;
        private Vector3 acceleration;
        private Vector3 currentVelocity;
        private Vector3 velocity;
        private Vector2[] mouseMovement;
        private Vector2[] mouseSmoothingCache;
        private Vector2 smoothedMouseMovement;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private KeyboardState currentKeyboardState;
        private Dictionary<Actions, Keys> actionKeys;

        #region Public Methods

        /// <summary>
        /// Constructs a new instance of the CameraComponent class. The
        /// camera will have a spectator behavior, and will be initially
        /// positioned at the world origin looking down the world negative
        /// z axis. An initial perspective projection matrix is created
        /// as well as setting up initial key bindings to the actions.
        /// </summary>
        public RCameraComponent(Game game)
            : base(game)
        {
            camera = new RCamera();
            camera.CurrentBehavior = RCamera.Behavior.Spectator;
            
            movingAlongPosX = false;
            movingAlongNegX = false;
            movingAlongPosY = false;
            movingAlongNegY = false;
            movingAlongPosZ = false;
            movingAlongNegZ = false;

            savedMousePosX = -1;
            savedMousePosY = -1;

            rotationSpeed = DEFAULT_SPEED_ROTATION;
            orbitRollSpeed = DEFAULT_SPEED_ORBIT_ROLL;
            flightYawSpeed = DEFAULT_SPEED_FLIGHT_YAW;
            mouseWheelSpeed = DEFAULT_SPEED_MOUSE_WHEEL;
            mouseSmoothingSensitivity = DEFAULT_MOUSE_SMOOTHING_SENSITIVITY;
            acceleration = new Vector3(DEFAULT_ACCELERATION_X, DEFAULT_ACCELERATION_Y, DEFAULT_ACCELERATION_Z);
            velocity = new Vector3(DEFAULT_VELOCITY_X, DEFAULT_VELOCITY_Y, DEFAULT_VELOCITY_Z);
            mouseSmoothingCache = new Vector2[MOUSE_SMOOTHING_CACHE_SIZE];

            mouseIndex = 0;
            mouseMovement = new Vector2[2];
            mouseMovement[0].X = 0.0f;
            mouseMovement[0].Y = 0.0f;
            mouseMovement[1].X = 0.0f;
            mouseMovement[1].Y = 0.0f;

            Rectangle clientBounds = game.Window.ClientBounds;
            float aspect = (float)clientBounds.Width / (float)clientBounds.Height;

            Perspective(RCamera.DEFAULT_FOVX, aspect, RCamera.DEFAULT_ZNEAR, RCamera.DEFAULT_ZFAR);

            actionKeys = new Dictionary<Actions, Keys>();

            actionKeys.Add(Actions.FlightYawLeftPrimary, Keys.Left);
            actionKeys.Add(Actions.FlightYawLeftAlternate, Keys.A);
            actionKeys.Add(Actions.FlightYawRightPrimary, Keys.Right);
            actionKeys.Add(Actions.FlightYawRightAlternate, Keys.D);
            actionKeys.Add(Actions.MoveForwardsPrimary, Keys.Up);
            actionKeys.Add(Actions.MoveForwardsAlternate, Keys.W);
            actionKeys.Add(Actions.MoveBackwardsPrimary, Keys.Down);
            actionKeys.Add(Actions.MoveBackwardsAlternate, Keys.S);
            actionKeys.Add(Actions.MoveDownPrimary, Keys.Q);
            actionKeys.Add(Actions.MoveDownAlternate, Keys.PageDown);
            actionKeys.Add(Actions.MoveUpPrimary, Keys.E);
            actionKeys.Add(Actions.MoveUpAlternate, Keys.PageUp);
            actionKeys.Add(Actions.OrbitRollLeftPrimary, Keys.Left);
            actionKeys.Add(Actions.OrbitRollLeftAlternate, Keys.A);
            actionKeys.Add(Actions.OrbitRollRightPrimary, Keys.Right);
            actionKeys.Add(Actions.OrbitRollRightAlternate, Keys.D);
            actionKeys.Add(Actions.StrafeRightPrimary, Keys.Right);
            actionKeys.Add(Actions.StrafeRightAlternate, Keys.D);
            actionKeys.Add(Actions.StrafeLeftPrimary, Keys.Left);
            actionKeys.Add(Actions.StrafeLeftAlternate, Keys.A);

            Game.Activated += new EventHandler<System.EventArgs>(HandleGameActivatedEvent);
            Game.Deactivated += new EventHandler<System.EventArgs>(HandleGameDeactivatedEvent);

            UpdateOrder = 1;
        }

        /// <summary>
        /// Initializes the CameraComponent class. This method repositions the
        /// mouse to the center of the game window.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Rectangle clientBounds = Game.Window.ClientBounds;
            Mouse.SetPosition(clientBounds.Width / 2, clientBounds.Height / 2);
            REngine.Instance._camera = camera;
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        public void LookAt(R3DVECTOR target)
        {
            camera.LookAt(target);
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="eye">The camera position.</param>
        /// <param name="target">The target position to look at.</param>
        /// <param name="up">The up direction.</param>
        public void LookAt(R3DVECTOR eye, R3DVECTOR target, R3DVECTOR up)
        {
            camera.LookAt(eye, target, up);
        }

        /// <summary>
        /// Binds an action to a keyboard key.
        /// </summary>
        /// <param name="action">The action to bind.</param>
        /// <param name="key">The key to map the action to.</param>
        public void MapActionToKey(Actions action, Keys key)
        {
            actionKeys[action] = key;
        }

        /// <summary>
        /// Moves the camera by dx world units to the left or right; dy
        /// world units upwards or downwards; and dz world units forwards
        /// or backwards.
        /// </summary>
        /// <param name="dx">Distance to move left or right.</param>
        /// <param name="dy">Distance to move up or down.</param>
        /// <param name="dz">Distance to move forwards or backwards.</param>
        public void Move(float dx, float dy, float dz)
        {
            camera.Move(dx, dy, dz);
        }

        /// <summary>
        /// Moves the camera the specified distance in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        /// <param name="distance">How far to move.</param>
        public void Move(R3DVECTOR direction, R3DVECTOR distance)
        {
            camera.Move(direction, distance);
        }

        /// <summary>
        /// Builds a perspective projection matrix based on a horizontal field
        /// of view.
        /// </summary>
        /// <param name="fovx">Horizontal field of view in degrees.</param>
        /// <param name="aspect">The viewport's aspect ratio.</param>
        /// <param name="znear">The distance to the near clip plane.</param>
        /// <param name="zfar">The distance to the far clip plane.</param>
        public void Perspective(float fovx, float aspect, float znear, float zfar)
        {
            camera.Perspective(fovx, aspect, znear, zfar);
        }

        /// <summary>
        /// Rotates the camera. Positive angles specify counter clockwise
        /// rotations when looking down the axis of rotation towards the
        /// origin.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        /// <param name="rollDegrees">Z axis rotation in degrees.</param>
        public void Rotate(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            camera.Rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        /// <summary>
        /// Updates the state of the CameraComponent class.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateInput();
            UpdateCamera(gameTime);
        }

        /// <summary>
        /// Undo any camera rolling by leveling the camera. When the camera is
        /// orbiting this method will cause the camera to become level with the
        /// orbit target.
        /// </summary>
        public void UndoRoll()
        {
            camera.UndoRoll();
        }

        /// <summary>
        /// Zooms the camera. This method functions differently depending on
        /// the camera's current behavior. When the camera is orbiting this
        /// method will move the camera closer to or further away from the
        /// orbit target. For the other camera behaviors this method will
        /// change the camera's horizontal field of view.
        /// </summary>
        ///
        /// <param name="zoom">
        /// When orbiting this parameter is how far to move the camera.
        /// For the other behaviors this parameter is the new horizontal
        /// field of view.
        /// </param>
        /// 
        /// <param name="minZoom">
        /// When orbiting this parameter is the min allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the
        /// min allowed horizontal field of view.
        /// </param>
        /// 
        /// <param name="maxZoom">
        /// When orbiting this parameter is the max allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the max
        /// allowed horizontal field of view.
        /// </param>
        public void Zoom(float zoom, float minZoom, float maxZoom)
        {
            camera.Zoom(zoom, minZoom, maxZoom);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines which way to move the camera based on player input.
        /// The returned values are in the range [-1,1].
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        private void GetMovementDirection(out Vector3 direction)
        {
            direction.X = 0.0f;
            direction.Y = 0.0f;
            direction.Z = 0.0f;

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveForwardsPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveForwardsAlternate]))
            {
                if (!movingAlongNegZ)
                {
                    movingAlongNegZ = true;
                    currentVelocity.Z = 0.0f;
                }

                direction.Z += 1.0f;
            }
            else
            {
                movingAlongNegZ = false;
            }

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveBackwardsPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveBackwardsAlternate]))
            {
                if (!movingAlongPosZ)
                {
                    movingAlongPosZ = true;
                    currentVelocity.Z = 0.0f;
                }

                direction.Z -= 1.0f;
            }
            else
            {
                movingAlongPosZ = false;
            }

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveUpPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveUpAlternate]))
            {
                if (!movingAlongPosY)
                {
                    movingAlongPosY = true;
                    currentVelocity.Y = 0.0f;
                }

                direction.Y += 1.0f;
            }
            else
            {
                movingAlongPosY = false;
            }

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveDownPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveDownAlternate]))
            {
                if (!movingAlongNegY)
                {
                    movingAlongNegY = true;
                    currentVelocity.Y = 0.0f;
                }

                direction.Y -= 1.0f;
            }
            else
            {
                movingAlongNegY = false;
            }

            switch (CurrentBehavior)
            {
                case RCamera.Behavior.FirstPerson:
                case RCamera.Behavior.Spectator:
                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeRightPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeRightAlternate]))
                    {
                        if (!movingAlongPosX)
                        {
                            movingAlongPosX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X += 1.0f;
                    }
                    else
                    {
                        movingAlongPosX = false;
                    }

                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeLeftPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeLeftAlternate]))
                    {
                        if (!movingAlongNegX)
                        {
                            movingAlongNegX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X -= 1.0f;
                    }
                    else
                    {
                        movingAlongNegX = false;
                    }

                    break;

                case RCamera.Behavior.Flight:
                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawLeftPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawLeftAlternate]))
                    {
                        if (!movingAlongPosX)
                        {
                            movingAlongPosX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X += 1.0f;
                    }
                    else
                    {
                        movingAlongPosX = false;
                    }

                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawRightPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawRightAlternate]))
                    {
                        if (!movingAlongNegX)
                        {
                            movingAlongNegX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X -= 1.0f;
                    }
                    else
                    {
                        movingAlongNegX = false;
                    }
                    break;

                case RCamera.Behavior.Orbit:
                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollLeftPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollLeftAlternate]))
                    {
                        if (!movingAlongPosX)
                        {
                            movingAlongPosX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X += 1.0f;
                    }
                    else
                    {
                        movingAlongPosX = false;
                    }

                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollRightPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollRightAlternate]))
                    {
                        if (!movingAlongNegX)
                        {
                            movingAlongNegX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X -= 1.0f;
                    }
                    else
                    {
                        movingAlongNegX = false;
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Determines which way the mouse wheel has been rolled.
        /// The returned value is in the range [-1,1].
        /// </summary>
        /// <returns>
        /// A positive value indicates that the mouse wheel has been rolled
        /// towards the player. A negative value indicates that the mouse
        /// wheel has been rolled away from the player.
        /// </returns>
        private float GetMouseWheelDirection()
        {
            int currentWheelValue = currentMouseState.ScrollWheelValue;
            int previousWheelValue = previousMouseState.ScrollWheelValue;

            if (currentWheelValue > previousWheelValue)
                return -1.0f;
            else if (currentWheelValue < previousWheelValue)
                return 1.0f;
            else
                return 0.0f;
        }

        /// <summary>
        /// Event handler for when the game window acquires input focus.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        private void HandleGameActivatedEvent(object sender, EventArgs e)
        {
            if (savedMousePosX >= 0 && savedMousePosY >= 0)
                Mouse.SetPosition(savedMousePosX, savedMousePosY);
        }

        /// <summary>
        /// Event hander for when the game window loses input focus.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        private void HandleGameDeactivatedEvent(object sender, EventArgs e)
        {
            MouseState state = Mouse.GetState();

            savedMousePosX = state.X;
            savedMousePosY = state.Y;
        }

        /// <summary>
        /// Filters the mouse movement based on a weighted sum of mouse
        /// movement from previous frames.
        /// <para>
        /// For further details see:
        ///  Nettle, Paul "Smooth Mouse Filtering", flipCode's Ask Midnight column.
        ///  http://www.flipcode.com/cgi-bin/fcarticles.cgi?show=64462
        /// </para>
        /// </summary>
        /// <param name="x">Horizontal mouse distance from window center.</param>
        /// <param name="y">Vertical mouse distance from window center.</param>
        private void PerformMouseFiltering(float x, float y)
        {
            // Shuffle all the entries in the cache.
            // Newer entries at the front. Older entries towards the back.
            for (int i = mouseSmoothingCache.Length - 1; i > 0; --i)
            {
                mouseSmoothingCache[i].X = mouseSmoothingCache[i - 1].X;
                mouseSmoothingCache[i].Y = mouseSmoothingCache[i - 1].Y;
            }

            // Store the current mouse movement entry at the front of cache.
            mouseSmoothingCache[0].X = x;
            mouseSmoothingCache[0].Y = y;

            float averageX = 0.0f;
            float averageY = 0.0f;
            float averageTotal = 0.0f;
            float currentWeight = 1.0f;

            // Filter the mouse movement with the rest of the cache entries.
            // Use a weighted average where newer entries have more effect than
            // older entries (towards the back of the cache).
            for (int i = 0; i < mouseSmoothingCache.Length; ++i)
            {
                averageX += mouseSmoothingCache[i].X * currentWeight;
                averageY += mouseSmoothingCache[i].Y * currentWeight;
                averageTotal += 1.0f * currentWeight;
                currentWeight *= mouseSmoothingSensitivity;
            }

            // Calculate the new smoothed mouse movement.
            smoothedMouseMovement.X = averageX / averageTotal;
            smoothedMouseMovement.Y = averageY / averageTotal;
        }

        /// <summary>
        /// Averages the mouse movement over a couple of frames to smooth out
        /// the mouse movement.
        /// </summary>
        /// <param name="x">Horizontal mouse distance from window center.</param>
        /// <param name="y">Vertical mouse distance from window center.</param>
        private void PerformMouseSmoothing(float x, float y)
        {
            mouseMovement[mouseIndex].X = x;
            mouseMovement[mouseIndex].Y = y;

            smoothedMouseMovement.X = (mouseMovement[0].X + mouseMovement[1].X) * 0.5f;
            smoothedMouseMovement.Y = (mouseMovement[0].Y + mouseMovement[1].Y) * 0.5f;

            mouseIndex ^= 1;
            mouseMovement[mouseIndex].X = 0.0f;
            mouseMovement[mouseIndex].Y = 0.0f;
        }

        /// <summary>
        /// Dampens the rotation by applying the rotation speed to it.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        /// <param name="rollDegrees">Z axis rotation in degrees.</param>
        private void RotateSmoothly(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            headingDegrees *= rotationSpeed;
            pitchDegrees *= rotationSpeed;
            rollDegrees *= rotationSpeed;

            Rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        /// <summary>
        /// Gathers and updates input from all supported input devices for use
        /// by the CameraComponent class.
        /// </summary>
        private void UpdateInput()
        {
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            Rectangle clientBounds = Game.Window.ClientBounds;

            int centerX = clientBounds.Width / 2;
            int centerY = clientBounds.Height / 2;
            int deltaX = centerX - currentMouseState.X;
            int deltaY = centerY - currentMouseState.Y;

            Mouse.SetPosition(centerX, centerY);

            PerformMouseFiltering((float)deltaX, (float)deltaY);
            PerformMouseSmoothing(smoothedMouseMovement.X, smoothedMouseMovement.Y);
        }

        /// <summary>
        /// Updates the camera's velocity based on the supplied movement
        /// direction and the elapsed time (since this method was last
        /// called). The movement direction is the in the range [-1,1].
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        private void UpdateVelocity(ref Vector3 direction, float elapsedTimeSec)
        {
            if (direction.X != 0.0f)
            {
                // Camera is moving along the x axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.X += direction.X * acceleration.X * elapsedTimeSec;

                if (currentVelocity.X > velocity.X)
                    currentVelocity.X = velocity.X;
                else if (currentVelocity.X < -velocity.X)
                    currentVelocity.X = -velocity.X;
            }
            else
            {
                // Camera is no longer moving along the x axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.X > 0.0f)
                {
                    if ((currentVelocity.X -= acceleration.X * elapsedTimeSec) < 0.0f)
                        currentVelocity.X = 0.0f;
                }
                else
                {
                    if ((currentVelocity.X += acceleration.X * elapsedTimeSec) > 0.0f)
                        currentVelocity.X = 0.0f;
                }
            }

            if (direction.Y != 0.0f)
            {
                // Camera is moving along the y axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.Y += direction.Y * acceleration.Y * elapsedTimeSec;

                if (currentVelocity.Y > velocity.Y)
                    currentVelocity.Y = velocity.Y;
                else if (currentVelocity.Y < -velocity.Y)
                    currentVelocity.Y = -velocity.Y;
            }
            else
            {
                // Camera is no longer moving along the y axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Y > 0.0f)
                {
                    if ((currentVelocity.Y -= acceleration.Y * elapsedTimeSec) < 0.0f)
                        currentVelocity.Y = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Y += acceleration.Y * elapsedTimeSec) > 0.0f)
                        currentVelocity.Y = 0.0f;
                }
            }

            if (direction.Z != 0.0f)
            {
                // Camera is moving along the z axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.Z += direction.Z * acceleration.Z * elapsedTimeSec;

                if (currentVelocity.Z > velocity.Z)
                    currentVelocity.Z = velocity.Z;
                else if (currentVelocity.Z < -velocity.Z)
                    currentVelocity.Z = -velocity.Z;
            }
            else
            {
                // Camera is no longer moving along the z axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Z > 0.0f)
                {
                    if ((currentVelocity.Z -= acceleration.Z * elapsedTimeSec) < 0.0f)
                        currentVelocity.Z = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Z += acceleration.Z * elapsedTimeSec) > 0.0f)
                        currentVelocity.Z = 0.0f;
                }
            }
        }

        /// <summary>
        /// Moves the camera based on player input.
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        private void UpdatePosition(ref Vector3 direction, float elapsedTimeSec)
        {
            if (currentVelocity.LengthSquared() != 0.0f)
            {
                // Only move the camera if the velocity vector is not of zero
                // length. Doing this guards against the camera slowly creeping
                // around due to floating point rounding errors.

                Vector3 displacement = (currentVelocity * elapsedTimeSec) +
                    (0.5f * acceleration * elapsedTimeSec * elapsedTimeSec);

                // Floating point rounding errors will slowly accumulate and
                // cause the camera to move along each axis. To prevent any
                // unintended movement the displacement vector is clamped to
                // zero for each direction that the camera isn't moving in.
                // Note that the UpdateVelocity() method will slowly decelerate
                // the camera's velocity back to a stationary state when the
                // camera is no longer moving along that direction. To account
                // for this the camera's current velocity is also checked.

                if (direction.X == 0.0f && (float)Math.Abs(currentVelocity.X) < 1e-6f)
                    displacement.X = 0.0f;

                if (direction.Y == 0.0f && (float)Math.Abs(currentVelocity.Y) < 1e-6f)
                    displacement.Y = 0.0f;

                if (direction.Z == 0.0f && (float)Math.Abs(currentVelocity.Z) < 1e-6f)
                    displacement.Z = 0.0f;

                Move(displacement.X, displacement.Y, displacement.Z);
            }

            // Continuously update the camera's velocity vector even if the
            // camera hasn't moved during this call. When the camera is no
            // longer being moved the camera is decelerating back to its
            // stationary state.

            UpdateVelocity(ref direction, elapsedTimeSec);
        }

        /// <summary>
        /// Updates the state of the camera based on player input.
        /// </summary>
        /// <param name="gameTime">Elapsed game time.</param>
        private void UpdateCamera(GameTime gameTime)
        {
            float elapsedTimeSec = 0.0f;

            if (Game.IsFixedTimeStep)
                elapsedTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                elapsedTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 direction = new Vector3();

            GetMovementDirection(out direction);

            float dx = 0.0f;
            float dy = 0.0f;
            float dz = 0.0f;

            switch (camera.CurrentBehavior)
            {
                case RCamera.Behavior.FirstPerson:
                case RCamera.Behavior.Spectator:
                    dx = smoothedMouseMovement.X;
                    dy = smoothedMouseMovement.Y;

                    RotateSmoothly(dx, dy, 0.0f);
                    UpdatePosition(ref direction, elapsedTimeSec);
                    break;

                case RCamera.Behavior.Flight:
                    dy = -smoothedMouseMovement.Y;
                    dz = smoothedMouseMovement.X;

                    RotateSmoothly(0.0f, dy, dz);

                    if ((dx = direction.X * flightYawSpeed * elapsedTimeSec) != 0.0f)
                        camera.Rotate(dx, 0.0f, 0.0f);

                    direction.X = 0.0f; // ignore yaw motion when updating camera's velocity
                    UpdatePosition(ref direction, elapsedTimeSec);
                    break;
                case RCamera.Behavior.Free:
                    dy = smoothedMouseMovement.Y;
                    dz = smoothedMouseMovement.X;

                    RotateSmoothly(dz, dy, 0.0f);
                    UpdatePosition(ref direction, elapsedTimeSec);
                    break;
                case RCamera.Behavior.Orbit:
                    dx = -smoothedMouseMovement.X;
                    dy = -smoothedMouseMovement.Y;

                    RotateSmoothly(dx, dy, 0.0f);

                    if (!camera.PreferTargetYAxisOrbiting)
                    {
                        if ((dz = direction.X * orbitRollSpeed * elapsedTimeSec) != 0.0f)
                            camera.Rotate(0.0f, 0.0f, dz);
                    }

                    if ((dz = GetMouseWheelDirection() * mouseWheelSpeed) != 0.0f)
                        camera.Zoom(dz, camera.OrbitMinZoom, camera.OrbitMaxZoom);

                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property to get and set the camera's acceleration.
        /// </summary>
        public R3DVECTOR Acceleration
        {
            get { return R3DVECTOR.FromVector3(acceleration); }
            set { acceleration = value.vector; }
        }

        /// <summary>
        /// Property to get and set the camera's behavior.
        /// </summary>
        public RCamera.Behavior CurrentBehavior
        {
            get { return camera.CurrentBehavior; }
            set { camera.CurrentBehavior = value; }
        }

        /// <summary>
        /// Property to get the camera's current velocity.
        /// </summary>
        public R3DVECTOR CurrentVelocity
        {
            get { return R3DVECTOR.FromVector3(currentVelocity); }
        }

        /// <summary>
        /// Property to get and set the flight behavior's yaw speed.
        /// </summary>
        public float FlightYawSpeed
        {
            get { return flightYawSpeed; }
            set { flightYawSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the sensitivity value used to smooth
        /// mouse movement.
        /// </summary>
        public float MouseSmoothingSensitivity
        {
            get { return mouseSmoothingSensitivity; }
            set { mouseSmoothingSensitivity = value; }
        }

        /// <summary>
        /// Property to get and set the speed of the mouse wheel.
        /// This is used to zoom in and out when the camera is orbiting.
        /// </summary>
        public float MouseWheelSpeed
        {
            get { return mouseWheelSpeed; }
            set { mouseWheelSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the max orbit zoom distance.
        /// </summary>
        public float OrbitMaxZoom
        {
            get { return camera.OrbitMaxZoom; }
            set { camera.OrbitMaxZoom = value; }
        }

        /// <summary>
        /// Property to get and set the min orbit zoom distance.
        /// </summary>
        public float OrbitMinZoom
        {
            get { return camera.OrbitMinZoom; }
            set { camera.OrbitMinZoom = value; }
        }

        /// <summary>
        /// Property to get and set the distance from the target when orbiting.
        /// </summary>
        public float OrbitOffsetDistance
        {
            get { return camera.OrbitOffsetDistance; }
            set { camera.OrbitOffsetDistance = value; }
        }

        /// <summary>
        /// Property to get and set the orbit behavior's rolling speed.
        /// This only applies when PreferTargetYAxisOrbiting is set to false.
        /// Orbiting with PreferTargetYAxisOrbiting set to true will ignore
        /// any camera rolling.
        /// </summary>
        public float OrbitRollSpeed
        {
            get { return orbitRollSpeed; }
            set { orbitRollSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the camera orbit target position.
        /// </summary>
        public R3DVECTOR OrbitTarget
        {
            get { return camera.OrbitTarget; }
            set { camera.OrbitTarget = value; }
        }

        /// <summary>
        /// Property to get and set the camera orientation.
        /// </summary>
        public RQUATERNION Orientation
        {
            get { return camera.Orientation; }
            set { camera.Orientation = value; }
        }

        /// <summary>
        /// Property to get and set the camera position.
        /// </summary>
        public R3DVECTOR Position
        {
            get { return camera.Position; }
            set { camera.Position = value; }
        }

        /// <summary>
        /// Property to get and set the flag to force the camera
        /// to orbit around the orbit target's Y axis rather than the camera's
        /// local Y axis.
        /// </summary>
        public bool PreferTargetYAxisOrbiting
        {
            get { return camera.PreferTargetYAxisOrbiting; }
            set { camera.PreferTargetYAxisOrbiting = value; }
        }

        /// <summary>
        /// Property to get the perspective projection matrix.
        /// </summary>
        public R3DMATRIX ProjectionMatrix
        {
            get { return camera.ProjectionMatrix; }
        }

        /// <summary>
        /// Property to get and set the mouse rotation speed.
        /// </summary>
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the camera's velocity.
        /// </summary>
        public R3DVECTOR Velocity
        {
            get { return R3DVECTOR.FromVector3(velocity); }
            set { velocity = value.vector; }
        }

        /// <summary>
        /// Property to get the viewing direction vector.
        /// </summary>
        public R3DVECTOR ViewDirection
        {
            get { return camera.ViewDirection; }
        }

        /// <summary>
        /// Property to get the view matrix.
        /// </summary>
        public R3DMATRIX ViewMatrix
        {
            get { return camera.ViewMatrix; }
        }

        /// <summary>
        /// Property to get the concatenated view-projection matrix.
        /// </summary>
        public R3DMATRIX ViewProjectionMatrix
        {
            get { return camera.ViewProjectionMatrix; }
        }

        /// <summary>
        /// Property to get the camera's local X axis.
        /// </summary>
        public R3DVECTOR XAxis
        {
            get { return camera.XAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Y axis.
        /// </summary>
        public R3DVECTOR YAxis
        {
            get { return camera.YAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Z axis.
        /// </summary>
        public R3DVECTOR ZAxis
        {
            get { return camera.ZAxis; }
        }

        #endregion
    }
#endif
}