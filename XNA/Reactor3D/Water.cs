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

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Resources;
#endregion

namespace Reactor
{

    public class RWater : RSceneNode, IDisposable
    {
        
        internal static Water water;
        ResourceContentManager _resources;
        internal void CreateWater()
        {
            _resources = REngine.Instance._resourceContent;
            water = new Water();
            this.Position = Vector3.Zero;
            
        }
        public void SetOptions(RWaterOptions Options)
        {
            water.Options = Options;
        }
        public void SetReflectionRenderDelegate(ReflectionRenderDelegate Delegate)
        {
            water.RenderObjects = Delegate;
        }
        public void Init(int Size, float TextureScale, R2DVECTOR WaveDirection)
        {
            //ocean.Width = Size;
            //ocean.Height = Size;
            //ocean.TextureScale = TextureScale;
            //ocean.BumpMapSpeed = WaveDirection.vector;
            //ocean.Scale = new Vector3(Scale, 1.0f, Scale);
            //ocean.LoadContent();
            water.Initialize();
            water.LoadContent();
            
        }
        Vector3 scale = Vector3.One;
        
        public void SetScale(float Scale)
        {
            R3DMATRIX world = R3DMATRIX.FromMatrix(Matrix.CreateScale(Scale));

            scale.X = scale.Y = scale.Z = Scale;
            water.World = world;
            //ocean.Scale = new Vector3(Scale, Scale, Scale);
            
        }

        public void SetScale(R3DVECTOR Scale)
        {
            scale = Scale.vector;
            R3DMATRIX world = R3DMATRIX.FromMatrix(Matrix.CreateScale(Scale.vector));

            
            water.World = world;
            //ocean.Scale = Scale.vector;
        }

        
        public void SetWaveNormalTexture(int TextureID)
        {
            //water.Options.
            //ocean.NormalMap(TextureID);
        }
        
        public void SetPosition(R3DVECTOR Position)
        {
            this.Position = Position.vector;
            Matrix world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(Position.vector);
            
            //world.Translation = Position.vector;
            water.World = R3DMATRIX.FromMatrix(world);
            //ocean.Position = Position.vector;
        }
        public R3DVECTOR GetPosition()
        {
            return R3DVECTOR.FromVector3(this.Position);
        }
        public void Update()
        {
            
            
            water.Update();
            base.Update();
        }

        public override void Render()
        {

            
            
            //REngine.Instance._game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 1);
            water.Draw();

            base.Render();
        }

        public void Dispose()
        {
            water = null;
            
        }
    }

    //delegate that the water component calls to render the objects in the scene
    public delegate void ReflectionRenderDelegate(R3DMATRIX reflectionMatrix);

    /// <summary>
    /// Options that must be passed to the water component before Initialization
    /// </summary>
    public class RWaterOptions
    {
        //the number of vertices in the x and z plane (must be of the form 2^n + 1)
        //and the amount of spacing between vertices
        public int Width = 257;
        public int Height = 257;
        public float CellSpacing = .5f;

        //how large to scale the wave map texture in the shader
        //higher than 1 and the texture will repeat providing finer detail normals
        public float WaveMapScale = 1.0f;

        //size of the reflection and refraction render targets' width and height
        public int RenderTargetSize = 512;

        //offsets for the texcoords of the wave maps updated every frame
        //these are used in combination with the velocities to scroll
        //the normal maps over the water plane. Resulting in the appearence
        //of moving ripples across the water plane
        public R2DVECTOR WaveMapOffset0;
        public R2DVECTOR WaveMapOffset1;

        //the direction to offset the texcoords of the wave maps
        public R2DVECTOR WaveMapVelocity0;
        public R2DVECTOR WaveMapVelocity1;

        //asset names for the normal/wave maps
        public int WaveMapAsset0ID;
        public int WaveMapAsset1ID;

        //water color and sun light properties
        public R4DVECTOR WaterColor;
        
        public float SunFactor; //the intensity of the sun specular term.
        public float SunPower;  //how shiny we want the sun specular term on the water to be.
    }

    /// <summary>
    /// Drawable game component for water rendering. Renders the scene to reflection and refraction
    /// maps that are projected onto the water plane and are distorted based on two scrolling normal
    /// maps.
    /// </summary>
    internal class Water
    {
        #region Fields
        private ReflectionRenderDelegate mDrawFunc;

        //vertex and index buffers for the water plane
        private VertexBuffer mVertexBuffer;
        private IndexBuffer mIndexBuffer;
        private VertexDeclaration mDecl;

        //water shader
        private Effect mEffect;

        //camera properties
        private Vector3 mViewPos;
        private Matrix mViewProj;
        private Matrix mWorld;

        //maps to render the refraction/reflection to
        private RenderTarget2D mRefractionMap;
        private RenderTarget2D mReflectionMap;

        //scrolling normal maps that we will use as a
        //a normal for the water plane in the shader
        private Texture mWaveMap0;
        private Texture mWaveMap1;

        //user specified options to configure the water object
        private RWaterOptions mOptions;

        //tells the water object if it needs to update the refraction
        //map itself or not. Since refraction just needs the scene drawn
        //regularly, we can:
        // --Draw the objects we want refracted
        // --Resolve the back buffer and send it to the water
        // --Skip computing the refraction map in the water object
        // This is useful if you are already drawing the scene to a render target
        // Prevents from rendering the scene objects multiple times
        private bool mGrabRefractionFromFB = false;

        private int mNumVertices;
        private int mNumTris;
        #endregion

        #region Properties

        public ReflectionRenderDelegate RenderObjects
        {
            set { mDrawFunc = value; }
        }

        /// <summary>
        /// The render target that the refraction is rendered to.
        /// </summary>
        

        /// <summary>
        /// The render target that the reflection is rendered to.
        /// </summary>
        

        /// <summary>
        /// Options to configure the water. Must be set before
        /// the water is initialized. Should be set immediately
        /// following the instantiation of the object.
        /// </summary>
        public RWaterOptions Options
        {
            get { return mOptions; }
            set { mOptions = value; }
        }

        /// <summary>
        /// The world matrix of the water.
        /// </summary>
        public R3DMATRIX World
        {
            get { return R3DMATRIX.FromMatrix(mWorld); }
            set { mWorld = value.matrix; }
        }

        #endregion

        public Water()
        {

        }

        public void Initialize()
        {
            

            //build the water mesh
            mNumVertices = mOptions.Width * mOptions.Height;
            mNumTris = (mOptions.Width-1) * (mOptions.Height-1) * 2;
            VertexPositionTexture[] vertices = new VertexPositionTexture[mNumVertices];

            Vector3[] verts;
            int[] indices;

            //create the water vertex grid positions and indices
            GenTriGrid(mOptions.Height, mOptions.Width, mOptions.CellSpacing, mOptions.CellSpacing,
                        new Vector3(-mOptions.Width * 0.5f, 0, -mOptions.Height * 0.5f), out verts, out indices);

            //copy the verts into our PositionTextured array
            for (int i = 0; i < mOptions.Width; ++i)
            {
                for (int j = 0; j < mOptions.Height; ++j)
                {
                    int index = i * mOptions.Width + j;
                    vertices[index].Position = verts[index];
                    vertices[index].TextureCoordinate = new Vector2((float)j / mOptions.Width, (float)i / mOptions.Height);
                }
            }

            mVertexBuffer = new VertexBuffer(REngine.Instance._game.GraphicsDevice,
                                             VertexPositionTexture.SizeInBytes * mOptions.Width * mOptions.Height,
                                             BufferUsage.WriteOnly);
            mVertexBuffer.SetData(vertices);

            mIndexBuffer = new IndexBuffer(REngine.Instance._game.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            mIndexBuffer.SetData(indices);

            mDecl = new VertexDeclaration(REngine.Instance._game.GraphicsDevice, VertexPositionTexture.VertexElements);

            //normalzie the sun direction in case the user didn't
        }

        public void LoadContent()
        {
            

            //load the wave maps
            mWaveMap0 = (Texture2D)RTextureFactory.Instance._textureList[mOptions.WaveMapAsset0ID];
            mWaveMap1 = (Texture2D)RTextureFactory.Instance._textureList[mOptions.WaveMapAsset1ID];

            //get the attributes of the back buffer
            PresentationParameters pp = REngine.Instance._game.GraphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;
            MultiSampleType msType = pp.MultiSampleType;
            int msQuality = pp.MultiSampleQuality;

            //create the reflection and refraction render targets
            //using the backbuffer attributes
            mRefractionMap = new RenderTarget2D(REngine.Instance._game.GraphicsDevice, mOptions.RenderTargetSize, mOptions.RenderTargetSize,
                                                1, format, msType, msQuality);
            mReflectionMap = new RenderTarget2D(REngine.Instance._game.GraphicsDevice, mOptions.RenderTargetSize, mOptions.RenderTargetSize,
                                                1, format, msType, msQuality);

#if !XBOX
            mEffect = REngine.Instance._resourceContent.Load<Effect>("Water");
#else
            mEffect = REngine.Instance._resourceContent.Load<Effect>("Water1");
#endif

            //set the parameters that shouldn't change.
            //Some of these might need to change every once in awhile,
            //move them to updateEffectParams function if you need that functionality.
            if (mEffect != null)
            {
                mEffect.Parameters["WaveMap0"].SetValue(mWaveMap0);
                mEffect.Parameters["WaveMap1"].SetValue(mWaveMap1);

                mEffect.Parameters["TexScale"].SetValue(mOptions.WaveMapScale);

                mEffect.Parameters["WaterColor"].SetValue(mOptions.WaterColor.vector);
                mEffect.Parameters["SunColor"].SetValue(RAtmosphere.Instance.sunColor);
                mEffect.Parameters["SunDirection"].SetValue(RAtmosphere.Instance.sunDirection);
                mEffect.Parameters["SunFactor"].SetValue(mOptions.SunFactor);
                mEffect.Parameters["SunPower"].SetValue(mOptions.SunPower);

                mEffect.Parameters["World"].SetValue(mWorld);
                mEffect.Parameters["View"].SetValue(REngine.Instance._camera.viewMatrix);
                if (RAtmosphere.Instance != null)
                {
                    mEffect.Parameters["FOGENABLE"].SetValue(RAtmosphere.Instance.fogEnabled);
                    mEffect.Parameters["FOGCOLOR"].SetValue(RAtmosphere.Instance.fogColor);
                    mEffect.Parameters["FOGDIST"].SetValue(RAtmosphere.Instance.fogDistance);
                }
            }
        }

        public void Update()
        {
            float timeDelta = (float)REngine.Instance._gameTime.ElapsedGameTime.TotalSeconds;

            //update the wave map offsets so that they will scroll across the water
            mOptions.WaveMapOffset0 += mOptions.WaveMapVelocity0 * timeDelta;
            mOptions.WaveMapOffset1 += mOptions.WaveMapVelocity1 * timeDelta;

            if (mOptions.WaveMapOffset0.X >= 1.0f || mOptions.WaveMapOffset0.X <= -1.0f)
                mOptions.WaveMapOffset0.X = 0.0f;
            if (mOptions.WaveMapOffset1.X >= 1.0f || mOptions.WaveMapOffset1.X <= -1.0f)
                mOptions.WaveMapOffset1.X = 0.0f;
            if (mOptions.WaveMapOffset0.Y >= 1.0f || mOptions.WaveMapOffset0.Y <= -1.0f)
                mOptions.WaveMapOffset0.Y = 0.0f;
            if (mOptions.WaveMapOffset1.Y >= 1.0f || mOptions.WaveMapOffset1.Y <= -1.0f)
                mOptions.WaveMapOffset1.Y = 0.0f;
        }

        public void Draw()
        {
            REngine.Instance._game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            REngine.Instance._game.GraphicsDevice.RenderState.AlphaTestEnable = false;
            REngine.Instance._game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            REngine.Instance._game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            //don't cull back facing triangles since we want the water to be visible
            //from beneath the water plane too
            REngine.Instance._game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            if (RAtmosphere.Instance != null)
            {
                mEffect.Parameters["FOGENABLE"].SetValue(RAtmosphere.Instance.fogEnabled);
                mEffect.Parameters["FOGCOLOR"].SetValue(RAtmosphere.Instance.fogColor);
                mEffect.Parameters["FOGDIST"].SetValue(RAtmosphere.Instance.fogDistance);
                
            }
            UpdateEffectParams();

            REngine.Instance._game.GraphicsDevice.Indices = mIndexBuffer;
            REngine.Instance._game.GraphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            REngine.Instance._game.GraphicsDevice.VertexDeclaration = mDecl;

            mEffect.Begin(SaveStateMode.None);

            foreach (EffectPass pass in mEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                REngine.Instance._game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mNumVertices, 0, mNumTris);
                pass.End();
            }

            mEffect.End();
            REngine.Instance._game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            REngine.Instance._game.GraphicsDevice.RenderState.AlphaTestEnable = false;
            REngine.Instance._game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            REngine.Instance._game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;


            //REngine.Instance._game.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            //RScreen2D.IAction_End2D();
        }

        /// <summary>
        /// Set the ViewProjection matrix and position of the Camera.
        /// </summary>
        /// <param name="viewProj"></param>
        /// <param name="pos"></param>
        public void SetCamera(Matrix viewProj, Vector3 pos)
        {
            mViewProj = viewProj;
            mViewPos = pos;
        }
        Vector4 waterPlaneL = new Vector4(0.0f, -1.0f, 0.0f, 0.0f);
        Matrix wInvTrans;
        Vector4 waterPlaneW;
        Matrix wvpInvTrans;
        Vector4 waterPlaneH;
        Matrix reflectionMatrix;
        Plane pwaterPlaneH;
        Plane pwaterPlaneW;
        /// <summary>
        /// Updates the reflection and refraction maps. Called
        /// on update.
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateWaterMaps()
        {
            /*------------------------------------------------------------------------------------------
             * Render to the Reflection Map
             */
            //clip objects below the water line, and render the scene upside down
            REngine.Instance._game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            //RenderTarget oldTarget = REngine.Instance._game.GraphicsDevice.GetRenderTarget(0);
            REngine.Instance._game.GraphicsDevice.SetRenderTarget(0, mReflectionMap);
            REngine.Instance._game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, mOptions.WaterColor.vector, 1.0f, 0);

            //reflection plane in local space


            wInvTrans = Matrix.Invert(mWorld);
            wInvTrans = Matrix.Transpose(wInvTrans);

            //reflection plane in world space
            Vector4 waterPlaneW = Vector4.Transform(waterPlaneL, wInvTrans);

            Matrix wvpInvTrans = Matrix.Invert(mWorld * REngine.Instance._camera.ViewProjectionMatrix.matrix);
            wvpInvTrans = Matrix.Transpose(wvpInvTrans);

            //reflection plane in homogeneous space
            Vector4 waterPlaneH = Vector4.Transform(waterPlaneL, wvpInvTrans);
            pwaterPlaneH = new Plane(waterPlaneH);
            pwaterPlaneW = new Plane(waterPlaneW);
            REngine.Instance._game.GraphicsDevice.ClipPlanes[0].IsEnabled = true;
            REngine.Instance._game.GraphicsDevice.ClipPlanes[0].Plane = pwaterPlaneH;

            reflectionMatrix = Matrix.CreateReflection(pwaterPlaneW);

            if (mDrawFunc != null)
            {
                mDrawFunc(R3DMATRIX.FromMatrix(reflectionMatrix));
            }

            REngine.Instance._game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            REngine.Instance._game.GraphicsDevice.ClipPlanes[0].IsEnabled = false;
            

            REngine.Instance._game.GraphicsDevice.SetRenderTarget(0, null);
            //RScreen2D.IAction_End2D();
            


            /*------------------------------------------------------------------------------------------
             * Render to the Refraction Map
             */

            //if the application is going to send us the refraction map
            //exit early. The refraction map must be given to the water component
            //before it renders. 
            //***This option can be handy if you're already drawing your scene to a render target***
            if (mGrabRefractionFromFB)
            {
                return;
            }

            //update the refraction map, clip objects above the water line
            //so we don't get artifacts
            REngine.Instance._game.GraphicsDevice.SetRenderTarget(0, mRefractionMap);
            REngine.Instance._game.GraphicsDevice.Clear(new Color(mOptions.WaterColor.vector));

            if (mDrawFunc != null)
                mDrawFunc(R3DMATRIX.FromMatrix(Matrix.Identity));

            REngine.Instance._game.GraphicsDevice.SetRenderTarget(0, null);
        }

        /// <summary>
        /// Updates effect parameters related to the water shader
        /// </summary>
        private void UpdateEffectParams()
        {
            //update the reflection and refraction textures
            mEffect.Parameters["ReflectMap"].SetValue(mReflectionMap.GetTexture());
            mEffect.Parameters["RefractMap"].SetValue(mRefractionMap.GetTexture());

            //normal map offsets
            mEffect.Parameters["WaveMapOffset0"].SetValue(mOptions.WaveMapOffset0.vector);
            mEffect.Parameters["WaveMapOffset1"].SetValue(mOptions.WaveMapOffset1.vector);

            mEffect.Parameters["WorldViewProj"].SetValue(mWorld * REngine.Instance._camera.ViewProjectionMatrix.matrix);
            mEffect.Parameters["SunColor"].SetValue(RAtmosphere.Instance.sunColor);
            mEffect.Parameters["SunDirection"].SetValue(RAtmosphere.Instance.sunDirection);
            mEffect.Parameters["SunFactor"].SetValue(mOptions.SunFactor);
            mEffect.Parameters["SunPower"].SetValue(mOptions.SunPower);

            mEffect.Parameters["WaterLevel"].SetValue(mWorld.Translation.Y);
            //pass the position of the camera to the shader
            mEffect.Parameters["EyePos"].SetValue(REngine.Instance._camera.Position.vector);
        }

        /// <summary>
        /// Generates a grid of vertices to use for the water plane.
        /// </summary>
        /// <param name="numVertRows">Number of rows. Must be 2^n + 1. Ex. 129, 257, 513.</param>
        /// <param name="numVertCols">Number of columns. Must be 2^n + 1. Ex. 129, 257, 513.</param>
        /// <param name="dx">Cell spacing in the x dimension.</param>
        /// <param name="dz">Cell spacing in the y dimension.</param>
        /// <param name="center">Center of the plane.</param>
        /// <param name="verts">Outputs the constructed vertices for the plane.</param>
        /// <param name="indices">Outpus the constructed triangle indices for the plane.</param>
        private void GenTriGrid(int numVertRows, int numVertCols, float dx, float dz,
                                Vector3 center, out Vector3[] verts, out int[] indices)
        {
            int numVertices = numVertRows * numVertCols;
            int numCellRows = numVertRows - 1;
            int numCellCols = numVertCols - 1;

            int mNumTris = numCellRows * numCellCols * 2;

            float width = (float)numCellCols * dx;
            float depth = (float)numCellRows * dz;

            //===========================================
            // Build vertices.

            // We first build the grid geometry centered about the origin and on
            // the xz-plane, row-by-row and in a top-down fashion.  We then translate
            // the grid vertices so that they are centered about the specified 
            // parameter 'center'.

            //verts.resize(numVertices);
            verts = new Vector3[numVertices];

            // Offsets to translate grid from quadrant 4 to center of 
            // coordinate system.
            float xOffset = -width * 0.5f;
            float zOffset = depth * 0.5f;

            int k = 0;
            for (float i = 0; i < numVertRows; ++i)
            {
                for (float j = 0; j < numVertCols; ++j)
                {
                    // Negate the depth coordinate to put in quadrant four.  
                    // Then offset to center about coordinate system.
                    verts[k] = new Vector3(0, 0, 0);
                    verts[k].X = j * dx + xOffset;
                    verts[k].Z = -i * dz + zOffset;
                    verts[k].Y = 0.0f;

                    Matrix translation = Matrix.CreateTranslation(center);
                    verts[k] = Vector3.Transform(verts[k], translation);

                    ++k; // Next vertex
                }
            }

            //===========================================
            // Build indices.

            //indices.resize(mNumTris * 3);
            indices = new int[mNumTris * 3];

            // Generate indices for each quad.
            k = 0;
            for (int i = 0; i < numCellRows; ++i)
            {
                for (int j = 0; j < numCellCols; ++j)
                {
                    indices[k] = i * numVertCols + j;
                    indices[k + 1] = i * numVertCols + j + 1;
                    indices[k + 2] = (i + 1) * numVertCols + j;

                    indices[k + 3] = (i + 1) * numVertCols + j;
                    indices[k + 4] = i * numVertCols + j + 1;
                    indices[k + 5] = (i + 1) * numVertCols + j + 1;

                    // next quad
                    k += 6;
                }
            }
             
            

            

            
        }
    }
}
