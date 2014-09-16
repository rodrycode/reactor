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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Reactor
{
    internal enum MeshBuilderType : int
    {
        Plane = 0,
        Grid = 1,
        Box = 2,
        Sphere = 3
    }
    public class RMeshBuilder : RSceneNode, IDisposable
    {
        #region Members
        internal RMaterial _material;
        internal BasicEffect _basicEffect;
        internal VertexBuffer _buffer;
        internal IndexBuffer _index;
        internal int vertCount = 0;
        #endregion
        #region Methods
        public RMeshBuilder()
        {
            this.Rotation = Vector3.Zero;
            this.Position = Vector3.Zero;
            
        }
        internal void CreateMesh(string name)
        {
            this.Name = name;
            Matrix = Matrix.CreateScale(1.0f);
            _basicEffect = new BasicEffect(REngine.Instance._game.GraphicsDevice);
        }

        
        public void SetTexture(int TextureID)
        {
            if (_material != null)
                _material.SetTexture(TextureID);
            else
                _basicEffect.Texture = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
        }

        public void CreateSphere(R3DVECTOR Center, float verticalRadius, float horizontalRadius, int NumPhiSides, int NumThetaSides)
        {
            RVERTEXFORMAT[] vertices = new RVERTEXFORMAT[NumThetaSides * 2 * (NumPhiSides + 1)];
            vertCount = NumThetaSides * 2 * (NumPhiSides + 1);
            int index = 0;
            for (int i = 0; i < NumThetaSides; ++i)
            {
                float theta1 = ((float)i / (float)NumThetaSides) * MathHelper.ToRadians(360f);
                float theta2 = ((float)(i + 1) / (float)NumThetaSides) * MathHelper.ToRadians(360f);

                for (int j = 0; j <= NumPhiSides; ++j)
                {
                    float phi = ((float)j / (float)NumPhiSides) * MathHelper.ToRadians(180f);

                    float x1 = (float)(Math.Sin(phi) * Math.Cos(theta1)) * horizontalRadius;
                    float z1 = (float)(Math.Sin(phi) * Math.Sin(theta1)) * horizontalRadius;
                    float x2 = (float)(Math.Sin(phi) * Math.Cos(theta2)) * horizontalRadius;
                    float z2 = (float)(Math.Sin(phi) * Math.Sin(theta2)) * horizontalRadius;
                    float y = (float)Math.Cos(phi) * verticalRadius;

                    Vector3 position = Center.vector + new Vector3(x1, y, z1);

                    vertices[index] = new RVERTEXFORMAT(position,new Vector2((float)i / (float)NumThetaSides, (float)j / (float)NumPhiSides), Vector3.Zero,
                         Vector3.Zero, Vector3.Zero);
                    index++;

                    position = Center.vector + new Vector3(x2, y, z2);

                    vertices[index] = new RVERTEXFORMAT(position,new Vector2((float)(i + 1) / (float)NumThetaSides, (float)j / (float)NumPhiSides), Vector3.Zero,
                        Vector3.Zero, Vector3.Zero);
                    index++;
                }
            }
            for (int x = 1; x < NumThetaSides - 1; ++x)
                for (int y = 1; y <= NumPhiSides - 1; ++y)
                {
                    Vector3 X = vertices[y * NumPhiSides + x].position - vertices[y * NumPhiSides + x + 1].position;
                    Vector3 Z = vertices[(y) * NumPhiSides + x].position - vertices[(y + 1) * NumPhiSides + x].position;
                    Vector3 normal = Vector3.Cross(Z, X);
                    //Normal.Normalize();
                    vertices[y * NumPhiSides + x].texture = new Vector2(((float)x - 1.0f) / (float)NumThetaSides, ((float)y - 1.0f) / (float)NumPhiSides);
                    // Tangent Data.
                    if (x != 0 && x < NumThetaSides - 1)
                        vertices[y * NumPhiSides + x].tangent = vertices[y * NumPhiSides + x - 1].position - vertices[y * NumPhiSides + x + 1].position;
                    else
                        if (x == 0)
                            vertices[y * NumPhiSides + x].tangent = vertices[y * NumPhiSides + x].position - vertices[y * NumPhiSides + x+1].position;
                        else
                            vertices[y * NumPhiSides + x].tangent = vertices[y * NumPhiSides + x-1].position - vertices[y * NumPhiSides + x].position;

                    // Bi Normal Data.
                    if (y != 0 && y < NumPhiSides - 1)
                        vertices[y * NumPhiSides + x].binormal = vertices[(y - 1) * NumPhiSides + x].position - vertices[(y+1) * NumPhiSides + x].position;
                    else
                        if (y == 0)
                            vertices[y * NumPhiSides + x].binormal = vertices[y * NumPhiSides + x].position - vertices[(y+1) * NumPhiSides + x].position;
                        else
                            vertices[y * NumPhiSides + x].binormal = vertices[(y - 1) * NumPhiSides + x].position - vertices[y * NumPhiSides + x].position;
                    
                    vertices[x + y * NumThetaSides].normal = normal;
                    vertices[x + y * NumThetaSides].normal.Normalize();

                    
                }
            int[] Indices = new int[((NumThetaSides - 1) * (NumPhiSides - 1)) * 6];
            for (int i = 0; i < NumThetaSides; ++i)
            {
                for (int j = 0; j <= NumPhiSides; ++j)
                {
                    Indices[(i + j * (NumThetaSides - 1)) * 6] = ((i + 1) + (j + 1) * NumPhiSides);
                    Indices[(i + j * (NumThetaSides - 1)) * 6 + 1] = (i + j * NumPhiSides);
                    Indices[(i + j * (NumThetaSides - 1)) * 6 + 2] = ((i + 1) + j * NumPhiSides);

                    Indices[(i + j * (NumThetaSides - 1)) * 6 + 3] = ((i + 1) + (j + 1) * NumPhiSides);
                    Indices[(i + j * (NumThetaSides - 1)) * 6 + 4] = (i + (j + 1) * NumPhiSides);
                    Indices[(i + j * (NumThetaSides - 1)) * 6 + 5] = (i + j * NumPhiSides);
                }
            }

            _buffer = new VertexBuffer(REngine.Instance._game.GraphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length,
                BufferUsage.WriteOnly);

            _buffer.SetData<RVERTEXFORMAT>(vertices);
            vertices = null;

            _index = new IndexBuffer(REngine.Instance._game.GraphicsDevice, typeof(int), Indices.Length, BufferUsage.WriteOnly);
            _index.SetData<int>(Indices);
            Indices = null;


        }

        public override void Render()
        {
            if (_material != null)
                Render(_material.shader.effect.CurrentTechnique.Name);
            else
                Render(_basicEffect.CurrentTechnique.Name);
        }
        internal void Render(string techniqueName)
        {
            if (_material == null)
            {

                RCamera camera = REngine.Instance._camera;

                GraphicsDevice graphicsDevice = REngine.Instance._graphics.GraphicsDevice;

                if (RAtmosphere.Instance.fogEnabled)
                {
                   

                }
                
                
                graphicsDevice.Indices = _index;
                graphicsDevice.SetVertexBuffer(_buffer);
                //_material.shader.effect.CurrentTechnique = _material.shader.effect.Techniques[techniqueName];
                _basicEffect.CurrentTechnique = _basicEffect.Techniques[techniqueName];
                // Draw the model. A model can have multiple meshes, so loop.

                _basicEffect.EnableDefaultLighting();
                _basicEffect.PreferPerPixelLighting = true;
                _basicEffect.LightingEnabled = true;
                //effect.SpecularPower = 1.5f;
                //effect.DirectionalLight0.Enabled = true;
                //effect.DirectionalLight0.Direction = Vector3.Normalize((Vector3.One * 5f));
                //effect.FogEnabled = false;
                //effect.FogStart = REngine.Instance._camera.znear;
                //effect.FogEnd = REngine.Instance._camera.zfar;
                //effect.FogColor = new Vector3(RAtmosphere.Instance.fogColor.X, RAtmosphere.Instance.fogColor.Y, RAtmosphere.Instance.fogColor.Z);
                _basicEffect.DirectionalLight0.DiffuseColor = new Vector3(RAtmosphere.Instance.sunColor.X, RAtmosphere.Instance.sunColor.Y, RAtmosphere.Instance.sunColor.Z);
                _basicEffect.DirectionalLight0.Direction = RAtmosphere.Instance.sunDirection;
                _basicEffect.DirectionalLight0.SpecularColor = Vector3.One;
                _basicEffect.DirectionalLight0.Enabled = true;
                _basicEffect.World = Matrix.Identity * Matrix.CreateScale(Scaling) * Matrix;
                //effect.World = _objectMatrix;
                _basicEffect.View = camera.viewMatrix;
                _basicEffect.Projection = camera.projMatrix;
                foreach(EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertCount, 0, vertCount / 3);
                }
                
                //REngine.Instance._graphics.GraphicsDevice.RenderState.RangeFogEnable = false;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.FogEnable = false;

            }
            else
            {
                Material_Render(techniqueName);
            }

        }
        private void Material_Render(string techniqueName)
        {
            RCamera camera = REngine.Instance._camera;

            //REngine.Instance._graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice graphicsDevice = REngine.Instance._graphics.GraphicsDevice;

            REngine.Instance._graphics.GraphicsDevice.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            DepthStencilState ds = new DepthStencilState
            {
                DepthBufferEnable = true,
            };
            REngine.Instance._graphics.GraphicsDevice.DepthStencilState = ds;
            REngine.Instance._graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            
            for (int i = 0; i < 8; i++)
            {
                REngine.Instance._graphics.GraphicsDevice.SamplerStates[i].AddressU = TextureAddressMode.Wrap;
                REngine.Instance._graphics.GraphicsDevice.SamplerStates[i].AddressV = TextureAddressMode.Wrap;
                REngine.Instance._graphics.GraphicsDevice.SamplerStates[i].AddressW = TextureAddressMode.Wrap;
                REngine.Instance._graphics.GraphicsDevice.SamplerStates[i].Filter = TextureFilter.Point;



            }
            
            _material.shader.effect.CurrentTechnique = _material.shader.effect.Techniques[techniqueName];
            
        }
        public void Dispose()
        {
            _buffer.Dispose();
            _index.Dispose();
            
        }


        #endregion
    }
    public class RMesh : RSceneNode, IDisposable
    {
        

        #region Internal Members
        internal Model _model;
        internal Matrix[] _transforms;
        internal BasicEffect _basicEffect;

        internal RMaterial _material;
        internal BoundingBox _boundingBox;
        #endregion

        #region Public Methods
        public RMesh()
        {
            Rotation = Vector3.Zero;
            Position = Vector3.Zero;
            Scaling = Vector3.One;
        }
        internal void CreateMesh(string name)
        {
            Name = name;
            _model = new Model();
            _basicEffect = new BasicEffect(REngine.Instance._graphics.GraphicsDevice);
            _basicEffect.CurrentTechnique = _basicEffect.Techniques[0];
        }
        public void Load(string filename)
        {
			_model = REngine.Instance._content.Load<Model>(filename);

            _transforms = new Matrix[_model.Bones.Count];

            _model.CopyAbsoluteBoneTransformsTo(_transforms);

            Matrix = Matrix.CreateScale(1.0f);
            Scaling = new Vector3(1f, 1f, 1f);
            Matrix = BuildScalingMatrix(Matrix);
            
            
            foreach(ModelMesh mesh in _model.Meshes)
            {
                BoundingSphere sphere = mesh.BoundingSphere;
                BoundingBox box = BoundingBox.CreateFromSphere(sphere);
                _boundingBox = BoundingBox.CreateMerged(_boundingBox, box);
                
            }
            
            
        }
        
        public override void Update()
        {
            //_objectMatrix = BuildRotationMatrix(_objectMatrix);
            //_objectMatrix.Translation = _position;
            //_model.CopyAbsoluteBoneTransformsTo(_transforms);
            //_objectMatrix = BuildRotationMatrix(_objectMatrix);
            
            //_objectMatrix = BuildScalingMatrix(_objectMatrix);
            //_objectMatrix = _transforms[_model.Meshes[0].ParentBone.Index] * _objectMatrix;
            base.Quaternion = Quaternion.CreateFromRotationMatrix(base.Matrix);
            
        }
        public override void Render()
        {
            if (_material != null)
                Render(_material.shader.effect.CurrentTechnique.Name);
            else
                Render(_basicEffect.CurrentTechnique.Name);
        }
        public bool LightingEnabled = true;
        public void Render(string techniqueName)
        {
            if (_material == null)
            {
                
                RCamera camera = REngine.Instance._camera;

                GraphicsDevice graphicsDevice = REngine.Instance._graphics.GraphicsDevice;

                //REngine.Instance._graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;

                //REngine.Instance._graphics.GraphicsDevice.RenderState.AlphaTestEnable = false;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.RangeFogEnable = false;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.FogEnable = false;
                if (RAtmosphere.Instance.fogEnabled)
                {
                    //REngine.Instance._graphics.GraphicsDevice.RenderState.FogEnable = RAtmosphere.Instance.fogEnabled;
                    //REngine.Instance._graphics.GraphicsDevice.RenderState.FogVertexMode = FogMode.None;
                    //REngine.Instance._graphics.GraphicsDevice.RenderState.FogDensity = 0.1f;
                    //REngine.Instance._graphics.GraphicsDevice.RenderState.FogStart = REngine.Instance._camera.znear;
                    //REngine.Instance._graphics.GraphicsDevice.RenderState.FogEnd = RAtmosphere.Instance.fogDistance * 2;
                    //REngine.Instance._graphics.GraphicsDevice.RenderState.FogColor = new Color(RAtmosphere.Instance.fogColor);
                    //REngine.Instance._graphics.GraphicsDevice.RenderState.RangeFogEnable = false;
                    
                }
                //REngine.Instance._graphics.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Always;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.ReferenceAlpha = 0;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.StencilDepthBufferFail = StencilOperation.Keep;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.StencilEnable = false;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.All;
                //REngine.Instance._graphics.GraphicsDevice.RenderState.ColorWriteChannels3 = ColorWriteChannels.All;
                
                //_material.shader.effect.CurrentTechnique = _material.shader.effect.Techniques[techniqueName];
                
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.DiffuseColor = new Vector3(RAtmosphere.Instance.sunColor.X, RAtmosphere.Instance.sunColor.Y, RAtmosphere.Instance.sunColor.Z);
                        effect.AmbientLightColor = new Vector3(RAtmosphere.Instance.ambientColor.X, RAtmosphere.Instance.ambientColor.Y, RAtmosphere.Instance.ambientColor.Z);
                        //effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.LightingEnabled = LightingEnabled;
                        effect.VertexColorEnabled = false;
                        //effect.Texture = (Texture2D)RTextureFactory.Instance._textureList[texid];
                        effect.TextureEnabled = true;
                        effect.SpecularPower = 0.0f;
                        effect.SpecularColor = Vector3.Zero;
                        effect.DirectionalLight0.Enabled = true;
                        effect.DirectionalLight0.DiffuseColor = new Vector3(RAtmosphere.Instance.sunColor.X, RAtmosphere.Instance.sunColor.Y, RAtmosphere.Instance.sunColor.Z);
                        effect.DirectionalLight0.SpecularColor = Vector3.Zero;
                        effect.DirectionalLight0.Direction = RAtmosphere.Instance.sunDirection;
                        //effect.DirectionalLight1.Enabled = false;
                        effect.DirectionalLight1.Enabled = false;
                        //effect.DirectionalLight1.DiffuseColor = new Vector3(RAtmosphere.Instance.sunColor.X, RAtmosphere.Instance.sunColor.Y, RAtmosphere.Instance.sunColor.Z);
                        //effect.DirectionalLight1.Direction = RAtmosphere.Instance.sunDirection;
                        //effect.DirectionalLight2.Enabled = false;
                        effect.DirectionalLight2.Enabled = false;
                        //effect.DirectionalLight2.DiffuseColor = new Vector3(RAtmosphere.Instance.sunColor.X, RAtmosphere.Instance.sunColor.Y, RAtmosphere.Instance.sunColor.Z);
                        //effect.DirectionalLight2.Direction = RAtmosphere.Instance.sunDirection;
                        //effect.FogEnabled = false;
                        //effect.FogStart = REngine.Instance._camera.znear;
                        //effect.FogEnd = REngine.Instance._camera.zfar;
                        //effect.FogColor = new Vector3(RAtmosphere.Instance.fogColor.X, RAtmosphere.Instance.fogColor.Y, RAtmosphere.Instance.fogColor.Z);

                        effect.World = _transforms[_model.Meshes[0].ParentBone.Index] * Matrix;
                        //effect.World = _objectMatrix;
                        effect.View = camera.viewMatrix;
                        effect.Projection = camera.projMatrix;
                        
                        //effect.View = Matrix.CreateLookAt(camera.Position.vector, Vector3.Zero, Vector3.Up);
                        //effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(camera.fovx),
                        //camera.aspectRatio, camera.znear, camera.zfar);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                }
                
                
                
            }
            else
            {
                Material_Render(techniqueName);
            }
            
        }
        private void Material_Render(string techniqueName)
        {
            RCamera camera = REngine.Instance._camera;
            
            //REngine.Instance._graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice graphicsDevice = REngine.Instance._graphics.GraphicsDevice;

            _material.shader.effect.CurrentTechnique = _material.shader.effect.Techniques[techniqueName];
            this.Matrix = _transforms[_model.Meshes[0].ParentBone.Index] * Matrix;
            //_material.Prepare(this);
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in _model.Meshes)
            {
                //foreach (Effect effect in mesh.Effects)
                //{
                    //effect.Begin();

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _material.shader.effect;
                }
                    //effect.Parameters["View"].SetValue(camera.viewMatrix);
                    //effect.Parameters["Projection"].SetValue(camera.projMatrix);
                    /*foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Begin();
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            graphicsDevice.VertexDeclaration = part.VertexDeclaration;
                            graphicsDevice.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                            graphicsDevice.Indices = mesh.IndexBuffer;
                            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                        pass.End();
                    }
                    effect.End();
                     */

                //}
                mesh.Draw();
            }
        }
        
        int texid;
        public void SetTexture(int TextureID, int TextureLayer)
        {
            texid = TextureID;
            EffectParameter etexture;
            if (_material != null)
            {
                Texture t = RTextureFactory.Instance._textureList[TextureID];
                etexture = _material.shader.GetParameterBySemantic("TEXTURE"+(int)TextureLayer);
                etexture.SetValue(t);
                
                
            }
            else
            {
                _basicEffect.Texture = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
                _basicEffect.TextureEnabled = true;


                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.TextureEnabled = true;
                        effect.Texture = _basicEffect.Texture;
                        effect.VertexColorEnabled = true;
                    }
                }
            }
        }
        public void SetTexture(int TextureID, int TextureLayer, int MeshID)
        {
            EffectParameter etexture;
            if (_materials[MeshID] != null)
            {
                Texture t = RTextureFactory.Instance._textureList[TextureID];
                etexture = _material.shader.GetParameterBySemantic("TEXTURE" + (int)TextureLayer);
                etexture.SetValue(t);


            }
            else
            {
                //_basicEffect.Texture = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
                //_basicEffect.TextureEnabled = true;



                //_meshIDEffects[MeshID].Texture = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
                //_meshIDEffects[MeshID].TextureEnabled = true;
                        ((BasicEffect)_model.Meshes[MeshID].Effects[0]).TextureEnabled = true;
                        ((BasicEffect)_model.Meshes[MeshID].Effects[0]).Texture = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
                    
                
            }
        }
        public void SetMaterial(RMaterial material)
        {
            int i = 0;
            _material = material;
            _materials.Clear();
            _materials.Add(i, material);
            i++;
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    _materials.Add(i, material);
                    part.Effect = material.shader.effect;
                    i++;
                }
            }
        }
        internal Hashtable _materials = new Hashtable();
        public RMaterial GetMaterial()
        {
            return (RMaterial)_materials[0];
        }
        public RMaterial GetMaterial(int MeshID)
        {
            return (RMaterial)_materials[MeshID];
            
        }
        public void SetMaterial(RMaterial material, int MeshID)
        {
            _material = material;
            _materials[MeshID] = material;
            foreach(ModelMeshPart part in _model.Meshes[MeshID].MeshParts)
            {
                part.Effect = material.shader.effect;
            }
        }
        public void SetMaterial(RMaterial material, string MeshName)
        {
            int mid = GetMeshIDFromName(MeshName);
            SetMaterial(material, mid);
        }
        public string[] GetMeshNames()
        {
            List<string> mnames = new List<string>();
            
            foreach (ModelMesh mesh in _model.Meshes)
            {
                mnames.Add(mesh.Name);
            }
            return mnames.ToArray();
            
        }
        public int GetMeshIDFromName(string Name)
        {
            int i = 0;
            foreach(ModelMesh mesh in _model.Meshes)
            {
                if (mesh.Name == Name)
                    return i;
                else
                    i++;
            }
            return i;
        }
        
        public string GetMeshNameFromID(int MeshID)
        {
            return _model.Meshes[MeshID].Name;
        }
        public void GetMeshIDs(out int[] meshIDs)
        {
            List<int> mindex = new List<int>();
            int counter = 0;
            foreach (ModelMesh mesh in _model.Meshes)
            {
                mindex.Add(counter);
                counter++;
            }
            meshIDs = mindex.ToArray();
            mindex = null;
        }
        
        
        public void Dispose()
        {
            if (_model != null)
            {
                _basicEffect.Dispose();
                _basicEffect = null;
                _model = null;
            }
        }
        public void AdvancedCollision()
        {

        }
        #endregion
    }
}