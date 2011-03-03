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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Reactor
{
    public class RAtmosphere
    {
        #region Static
        static RAtmosphere _instance;
        public static RAtmosphere Instance
        {
            get { return _instance; }
        }
        #endregion


        #region FullscreenQuadForSkybox
        internal VertexDeclaration QuadvertexDecl = null;
        internal VertexPositionTexture[] Quadverts = null;
        internal short[] Quadib = null;
        internal TextureCube skyTex;
        internal Effect skyEffect;
        internal bool skyEnabled = false;
        internal bool fogEnabled = false;
        internal float fogDistance = 15000.0f;
        internal Vector4 fogColor = new Vector4(1f,1f,1f,1f);
        internal Vector3 sunDirection = new Vector3(0f, -1f, 0f);
        internal Vector4 sunColor = new Vector4(1f, 1f, 1f, 1f);
        internal Vector4 ambientColor = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
        internal Model skyBox;
        internal float TextureScale = 1.0f;
        internal Matrix skyBoxWorld;
        internal ResourceContentManager skyResource;
        #endregion
        #region Public Methods
        public RAtmosphere()
        {
            if (_instance == null)
            {

                _instance = this;
                
                return;
            }
            else
            {
                return;
            }
        }
        public void Dispose()
        {
            if (_instance != null)
            {
                if (_instance.QuadvertexDecl != null)
                    _instance.QuadvertexDecl.Dispose();

                _instance.QuadvertexDecl = null;
                if (_instance.skyEffect != null)
                {
                    _instance.skyEffect.Dispose();
                    _instance.skyEffect = null;
                }
                if (_instance.skyResource != null)
                {
                    _instance.skyResource.Unload();
                    _instance.skyResource = null;
                }
                if (_instance.skyTex != null)
                {
                    _instance.skyTex.Dispose();
                    _instance.skyTex = null;
                }
                _instance = null;
                return;
            }
            else
            {
                return;
            }
        }
        public void SkyBox_Initialize()
        {
            
#if !XBOX
                if(_instance.skyTex == null)
                    _instance.skyTex = REngine.Instance._resourceContent.Load<TextureCube>("LobbyCube");
                if(_instance.skyEffect == null)
                    _instance.skyEffect = REngine.Instance._resourceContent.Load<Effect>("Skybox");
#else
            if (_instance.skyTex == null)
                    _instance.skyTex = _instance.skyResource.Load<TextureCube>("SkyCube");
                if(_instance.skyEffect == null)
                    _instance.skyEffect = _instance.skyResource.Load<Effect>("Skybox1");
#endif
                foreach (ModelMesh mesh in _instance.skyBox.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {

                        part.Effect = _instance.skyEffect;

                    }
                }

            
        }
        public void SkyBox_Enable(bool enable)
        {
            _instance.skyEnabled = enable;
            

        }
        public void SkyBox_SetTextureCube(int textureid)
        {
            
            TextureCube t = (TextureCube)RTextureFactory.Instance._textureList[textureid];
            _instance.skyTex = t;
        }
        public void Sky_SetSunlightDirection(R3DVECTOR LightDirection)
        {
            LightDirection.Normalize();
            _instance.sunDirection = LightDirection.vector;
        }
        public void Sky_SetSunlightColor(R4DVECTOR SunColor)
        {
            _instance.sunColor = SunColor.vector;
        }
        public void Sky_SetGlobalAmbientColor(R4DVECTOR AmbientColor)
        {
            AmbientColor.vector.Normalize();
            _instance.ambientColor = AmbientColor.vector;
        }
        public void Fog_Enable(bool Enable)
        {
            fogEnabled = Enable;
        }
        public void Fog_SetColor(R4DVECTOR Color)
        {
            fogColor = Color.vector;
        }
        public void Fog_SetColor(float R, float G, float B, float A)
        {
            fogColor = new Vector4(R, G, B, A);
        }
        public void Fog_SetDistance(float distance)
        {
            fogDistance = distance;
        }
        public R3DMATRIX SkyBox_GetMatrix()
        {
            return R3DMATRIX.FromMatrix(skyBoxWorld);
        }
        bool _reflected = false;
        Matrix _reflection;
        public void SkyBox_SetReflectionMatrix(R3DMATRIX Matrix)
        {
            _reflection = Matrix.matrix;
            _reflected = true;
        }
        public void SkyBox_ResetReflectionMatrix()
        {
            _reflected = false;
        }
        public void Update()
        {
            
        }
        public void SetTextureScale(float scale)
        {
            _instance.TextureScale = scale;
        }
        public void SkyBox_Render()
        {
            if (_instance.skyEnabled)
            {
                try
                {
                    R3DVECTOR target = REngine.Instance._camera.Target;
                    Matrix view = REngine.Instance._camera.ViewMatrix.matrix;
                    Matrix proj = REngine.Instance._camera.ProjectionMatrix.matrix;
                    R3DVECTOR camPos = REngine.Instance._camera.Position;
                    skyBoxWorld = Matrix.Identity * Matrix.CreateScale(REngine.Instance._camera.zfar*0.5f) * Matrix.CreateWorld(camPos.vector, Vector3.Forward, Vector3.Up);
                    
                    //skyBoxWorld = Matrix.Identity * Matrix.CreateWorld(camPos.vector, Vector3.Forward, Vector3.Up);
                    bool flip = false;
                    if (_reflected)
                    {
                        flip = true;
                        target.Y *= -1;
                        camPos.Y *= -1;

                        if (REngine.Instance._camera.Position.Y < RWater.water.World.Translation.Y)
                            flip = false;
                        //skyBoxWorld = (Matrix.CreateScale(REngine.Instance._camera.zfar) * Matrix.CreateTranslation(camPos.vector)) * _reflection;
                        skyBoxWorld *= _reflection;
                        //target.Y *= -1;
                        //camPos.Y *= -1;
                        //REngine.Instance._camera.Position = camPos;
                        //REngine.Instance._camera.LookAt(target);
                        //REngine.Instance._camera.Update();
                        //camPos.Y *= -1;
                        //view *= _reflection;
                        //view = Matrix.CreateV
                        //proj *= _reflection;
                    }
                    else
                    {
                        view = REngine.Instance._camera.viewMatrix;

                    }

                    _instance.skyEffect.Parameters["TextureScale"].SetValue(_instance.TextureScale);
                    _instance.skyEffect.Parameters["surfaceTexture"].SetValue(_instance.skyTex);
                    _instance.skyEffect.Parameters["View"].SetValue(view);
                    _instance.skyEffect.Parameters["Projection"].SetValue(proj);
                    _instance.skyEffect.Parameters["EyePosition"].SetValue(camPos.vector);
                    _instance.skyEffect.Parameters["World"].SetValue(skyBoxWorld);
                    _instance.skyEffect.Parameters["Reflected"].SetValue(flip);
                    //_instance.skyEffect.Parameters["FOGENABLE"].SetValue(RAtmosphere.Instance.fogEnabled);
                    //_instance.skyEffect.Parameters["FOGENABLE"].SetValue(false);
                    //_instance.skyEffect.Parameters["FOGCOLOR"].SetValue(RAtmosphere.Instance.fogColor);
                    //_instance.skyEffect.Parameters["FOGDIST"].SetValue(RAtmosphere.Instance.fogDistance);

                    REngine.Instance._game.GraphicsDevice.RenderState.DepthBufferEnable = false;

                    //_instance.skyEffect.Begin();

                    REngine.Instance._graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

                    foreach (ModelMesh mesh in _instance.skyBox.Meshes)
                    {

                        mesh.Draw();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    REngine.Instance.AddToLog(e.ToString());
                }
            }

            

            
        }
        void RenderQuad(Vector2 v1, Vector2 v2)
        {
            VertexDeclaration olddec = REngine.Instance._graphics.GraphicsDevice.VertexDeclaration;
            REngine.Instance._graphics.GraphicsDevice.VertexDeclaration = _instance.QuadvertexDecl;

            _instance.Quadverts[0].Position.X = v2.X;
            _instance.Quadverts[0].Position.Y = v1.Y;

            _instance.Quadverts[1].Position.X = v1.X;
            _instance.Quadverts[1].Position.Y = v1.Y;

            _instance.Quadverts[2].Position.X = v1.X;
            _instance.Quadverts[2].Position.Y = v2.Y;

            _instance.Quadverts[3].Position.X = v2.X;
            _instance.Quadverts[3].Position.Y = v2.Y;

            REngine.Instance._graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, _instance.Quadverts, 0, 4, _instance.Quadib, 0, 2);

            REngine.Instance._graphics.GraphicsDevice.VertexDeclaration = olddec;
        }
        void InitFullScreenQuad()
        {
            if (_instance.QuadvertexDecl == null)
            {
                _instance.QuadvertexDecl = new VertexDeclaration(
                                        REngine.Instance._graphics.GraphicsDevice,
                                        VertexPositionTexture.VertexElements);
            }
            if (_instance.Quadverts == null)
            {
                _instance.Quadverts = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,0))
                        };
            }
            if (_instance.Quadib == null)
            {
                _instance.Quadib = new short[] { 0, 1, 2, 2, 3, 0 };
            }

        }
        public void Initialize()
        {
            InitFullScreenQuad();
            _instance.skyResource = new ResourceContentManager(REngine.Instance._game.Services, Resource1.ResourceManager);
#if !XBOX
            _instance.skyBox = _instance.skyResource.Load<Model>("box");
#else
            _instance.skyBox = REngine.Instance._resourceContent.Load<Model>("box1");
#endif
        }
        #endregion
    }
}