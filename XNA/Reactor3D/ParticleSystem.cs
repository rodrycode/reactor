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
    internal struct BillboardParticleElement
    {
        Vector3 position;
        Vector2 textureCoordinate;
        Color color;
        Vector4 data;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 TextureCoordinate
        {
            get { return textureCoordinate; }
            set { textureCoordinate = value; }
        }
        public Vector4 Data
        {
            get { return data; }
            set { data = value; }
        }
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public float SpriteSize
        {
            get { return data.X; }
            set { data.X = value; }
        }
        public float AlphaValue
        {
            get { return data.Y; }
            set { data.Y = value; }
        }

        public BillboardParticleElement(Vector3 Position, Vector2 TextureCoordinate, Vector2 XYScale, Color Color)
        {
            position = Position;
            textureCoordinate = TextureCoordinate;
            data = Vector4.One;
            color = Color;
        }
        static public VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement(0,0,VertexElementFormat.Vector3,VertexElementMethod.Default,VertexElementUsage.Position,0),
            new VertexElement(0,4*3,VertexElementFormat.Vector2,VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,0),                
            new VertexElement(0,4*5,VertexElementFormat.Color ,VertexElementMethod.Default,VertexElementUsage.Color,0),
            new VertexElement(0,4*6,VertexElementFormat.Vector4,VertexElementMethod.Default,VertexElementUsage.Position,1),
        };

        public static int SizeInBytes = (3 + 2 + 1 + 4) * 4;
    }

    internal class BillboardParticleEmitter
    {
        private DynamicVertexBuffer vb;
        private DynamicIndexBuffer ib;
        VertexDeclaration m_vDec;

        public BillboardParticleElement[] particleArray;

        public Vector3 myPosition;
        public Vector3 myScale = new Vector3(1f,1f,1f);
        public Quaternion myRotation;

        public Color particleColor;

        public int partCount;

        Effect shader;
        Texture2D texture;
        

        int nextParticle = 0;
        public Vector3 targetPos;
        Vector3 myLastpos;

        public BillboardParticleEmitter()
        {
            
            myPosition = Vector3.One;
            myScale = Vector3.One;
            myRotation = new Quaternion(0, 0, 0, 1);

            
            targetPos = Vector3.Zero;

            particleColor = Color.White;
        }
        public void SetTexture(int TextureID)
        {
            texture = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
            

        }
        public void LoadContent()
        {
            m_vDec = new VertexDeclaration(REngine.Instance._game.GraphicsDevice, BillboardParticleElement.VertexElements);

            shader = REngine.Instance._resourceContent.Load<Effect>("Billboard");
            


            myLastpos = targetPos;

            
        }
        public void LoadParticles()
        {
            particleArray = new BillboardParticleElement[partCount * 4];

            for (int p = 0; p < partCount; p += 4)
            {
                for (int thisP = 0; thisP < 4; thisP++)
                {
                    int currentParticle = p + thisP;

                    particleArray[currentParticle] = new BillboardParticleElement();
                    particleArray[currentParticle].Position = myPosition;
                    particleArray[currentParticle].Color = particleColor;
                    particleArray[currentParticle].Data = new Vector4(1f, 1f, 0f, 0f);
                    switch (thisP)
                    {
                        case 0:
                            particleArray[currentParticle].TextureCoordinate = Vector2.Zero;
                            break;
                        case 1:
                            particleArray[currentParticle].TextureCoordinate = new Vector2(1, 0);
                            break;
                        case 2:
                            particleArray[currentParticle].TextureCoordinate = new Vector2(0, 1);
                            break;
                        case 3:
                            particleArray[currentParticle].TextureCoordinate = Vector2.One;
                            break;
                    }
                }
            }

            short[] indices = new short[6 * partCount];

            for (int part = 0; part < partCount; part++)
            {
                int off = part * 6;
                int offVal = part * 4;

                indices[off + 0] = (short)(0 + offVal);
                indices[off + 1] = (short)(1 + offVal);
                indices[off + 2] = (short)(2 + offVal);

                indices[off + 3] = (short)(1 + offVal);
                indices[off + 4] = (short)(3 + offVal);
                indices[off + 5] = (short)(2 + offVal);
            }

            ib = new DynamicIndexBuffer(REngine.Instance._game.GraphicsDevice, typeof(short), 6 * partCount, BufferUsage.WriteOnly);
            ib.SetData(indices);
        }
        public void AddParticle(Vector3 Position, Vector3 Velocity)
        {
            for (int p = 0; p < particleArray.Length; p += 4)
            {
                for (int thisP = 0; thisP < 4; thisP++)
                {
                    if (p == nextParticle && myLastpos != targetPos)
                    {
                        
                        particleArray[p + thisP].Position = Position;
                        particleArray[p + thisP].Color = particleColor;

                    }

                    particleArray[p + thisP].SpriteSize = (Vector3.Distance(particleArray[p + thisP].Position, targetPos) / myScale.X);
                }
            }
            nextParticle++;

            if (nextParticle >= particleArray.Length / 4)
                nextParticle = 0;

            vb = new DynamicVertexBuffer(REngine.Instance._game.GraphicsDevice, typeof(BillboardParticleElement), 4 * partCount, BufferUsage.WriteOnly);
            vb.SetData(particleArray);
        }
        public float Size = 50f;
        public void Update()
        {
            for (int p = 0; p < particleArray.Length; p += 4)
            {
                for (int thisP = 0; thisP < 4; thisP++)
                {
                    if (p == nextParticle && myLastpos != targetPos)
                    {
                        particleArray[p + thisP].Position = myLastpos;
                        particleArray[p + thisP].Color = particleColor;
                    }

                    particleArray[p + thisP].SpriteSize = Size;
                }
            }
            nextParticle++;

            if (nextParticle >= particleArray.Length / 4)
                nextParticle = 0;

            vb = new DynamicVertexBuffer(REngine.Instance._game.GraphicsDevice, typeof(BillboardParticleElement), 4 * partCount, BufferUsage.WriteOnly);
            vb.SetData(particleArray);

            myLastpos = targetPos;
        }
        public void Render()
        {
            GameTime gameTime = REngine.Instance._gameTime;
            REngine.Instance._graphics.GraphicsDevice.VertexDeclaration = m_vDec;
            REngine.Instance._graphics.GraphicsDevice.Vertices[0].SetSource(vb, 0, BillboardParticleElement.SizeInBytes);
            REngine.Instance._graphics.GraphicsDevice.Indices = ib;

            bool AlphaBlendEnable = REngine.Instance._game.GraphicsDevice.RenderState.AlphaBlendEnable;
            Blend DestinationBlend = REngine.Instance._game.GraphicsDevice.RenderState.DestinationBlend;
            Blend SourceBlend = REngine.Instance._game.GraphicsDevice.RenderState.SourceBlend;
            bool DepthBufferWriteEnable = REngine.Instance._game.GraphicsDevice.RenderState.DepthBufferWriteEnable;
            BlendFunction BlendFunc = REngine.Instance._game.GraphicsDevice.RenderState.BlendFunction;
            REngine.Instance._graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            REngine.Instance._graphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            REngine.Instance._graphics.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            //REngine.Instance._graphics.GraphicsDevice.RenderState.FogEnable = false;
            REngine.Instance._graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            REngine.Instance._graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
            REngine.Instance._graphics.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
            if (RAtmosphere.Instance != null)
                if (RAtmosphere.Instance.fogEnabled)
                    REngine.Instance._graphics.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //Matrix World = Matrix.CreateBillboard(myPosition, REngine.Instance._camera.Position.vector, REngine.Instance._camera.viewMatrix.Up, null);
            Matrix World = Matrix.CreateScale(myScale) * Matrix.CreateFromQuaternion(myRotation) * Matrix.CreateTranslation(myPosition);
            shader.Parameters["world"].SetValue(World);
            Matrix vp = REngine.Instance._camera.viewMatrix * REngine.Instance._camera.projMatrix;
            shader.Parameters["vp"].SetValue(vp);

            shader.Parameters["particleTexture"].SetValue(texture);
            shader.Begin();
            for (int ps = 0; ps < shader.CurrentTechnique.Passes.Count; ps++)
            {
                shader.CurrentTechnique.Passes[ps].Begin();
                REngine.Instance._game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4 * partCount, 0, partCount * 2);
                shader.CurrentTechnique.Passes[ps].End();
            }
            shader.End();

            REngine.Instance._graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            REngine.Instance._graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            
            REngine.Instance._graphics.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;

            REngine.Instance._game.GraphicsDevice.RenderState.AlphaBlendEnable = AlphaBlendEnable;

            REngine.Instance._game.GraphicsDevice.RenderState.DestinationBlend = DestinationBlend;
            REngine.Instance._game.GraphicsDevice.RenderState.SourceBlend = SourceBlend;

        }
    }

    internal class PointParticleEmitter
    {
        #region Fields

        PointParticleSystem particleSystem;
        float timeBetweenParticles;
        Vector3 previousPosition;
        float timeLeftOver;

        #endregion


        /// <summary>
        /// Constructs a new particle emitter object.
        /// </summary>
        public PointParticleEmitter(PointParticleSystem particleSystem,
                               float particlesPerSecond, Vector3 initialPosition)
        {
            this.particleSystem = particleSystem;

            timeBetweenParticles = 1.0f / particlesPerSecond;

            previousPosition = initialPosition;
        }


        /// <summary>
        /// Updates the emitter, creating the appropriate number of particles
        /// in the appropriate positions.
        /// </summary>
        public void Update(GameTime gameTime, Vector3 newPosition)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            // Work out how much time has passed since the previous update.
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > 0)
            {
                // Work out how fast we are moving.
                Vector3 velocity = (newPosition - previousPosition) / elapsedTime;

                // If we had any time left over that we didn't use during the
                // previous update, add that to the current elapsed time.
                float timeToSpend = timeLeftOver + elapsedTime;

                // Counter for looping over the time interval.
                float currentTime = -timeLeftOver;

                // Create particles as long as we have a big enough time interval.
                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    // Work out the optimal position for this particle. This will produce
                    // evenly spaced particles regardless of the object speed, particle
                    // creation frequency, or game update rate.
                    float mu = currentTime / elapsedTime;

                    Vector3 position = Vector3.Lerp(previousPosition, newPosition, mu);

                    // Create the particle.
                    particleSystem.AddParticle(position, velocity);
                }

                // Store any time we didn't use, so it can be part of the next update.
                timeLeftOver = timeToSpend;
            }

            previousPosition = newPosition;
        }
    }

    internal class PointParticleSystem : DrawableGameComponent
    {
        #region Fields


        // Settings class controls the appearance and animation of this particle system.
        internal PointParticleSettings settings = new PointParticleSettings();

        int TextureID;
        // For loading the effect and particle texture.
        ContentManager content;


        // Custom effect for drawing point sprite particles. This computes the particle
        // animation entirely in the vertex shader: no per-particle CPU work required!
        Effect particleEffect;


        // Shortcuts for accessing frequently changed effect parameters.
        EffectParameter effectViewParameter;
        EffectParameter effectProjectionParameter;
        EffectParameter effectViewportHeightParameter;
        EffectParameter effectTimeParameter;


        // An array of particles, treated as a circular queue.
        PointParticleVertex[] particles;


        // A vertex buffer holding our particles. This contains the same data as
        // the particles array, but copied across to where the GPU can access it.
        DynamicVertexBuffer vertexBuffer;


        // Vertex declaration describes the format of our ParticleVertex structure.
        VertexDeclaration vertexDeclaration;


        // The particles array and vertex buffer are treated as a circular queue.
        // Initially, the entire contents of the array are free, because no particles
        // are in use. When a new particle is created, this is allocated from the
        // beginning of the array. If more than one particle is created, these will
        // always be stored in a consecutive block of array elements. Because all
        // particles last for the same amount of time, old particles will always be
        // removed in order from the start of this active particle region, so the
        // active and free regions will never be intermingled. Because the queue is
        // circular, there can be times when the active particle region wraps from the
        // end of the array back to the start. The queue uses modulo arithmetic to
        // handle these cases. For instance with a four entry queue we could have:
        //
        //      0
        //      1 - first active particle
        //      2 
        //      3 - first free particle
        //
        // In this case, particles 1 and 2 are active, while 3 and 4 are free.
        // Using modulo arithmetic we could also have:
        //
        //      0
        //      1 - first free particle
        //      2 
        //      3 - first active particle
        //
        // Here, 3 and 0 are active, while 1 and 2 are free.
        //
        // But wait! The full story is even more complex.
        //
        // When we create a new particle, we add them to our managed particles array.
        // We also need to copy this new data into the GPU vertex buffer, but we don't
        // want to do that straight away, because setting new data into a vertex buffer
        // can be an expensive operation. If we are going to be adding several particles
        // in a single frame, it is faster to initially just store them in our managed
        // array, and then later upload them all to the GPU in one single call. So our
        // queue also needs a region for storing new particles that have been added to
        // the managed array but not yet uploaded to the vertex buffer.
        //
        // Another issue occurs when old particles are retired. The CPU and GPU run
        // asynchronously, so the GPU will often still be busy drawing the previous
        // frame while the CPU is working on the next frame. This can cause a
        // synchronization problem if an old particle is retired, and then immediately
        // overwritten by a new one, because the CPU might try to change the contents
        // of the vertex buffer while the GPU is still busy drawing the old data from
        // it. Normally the graphics driver will take care of this by waiting until
        // the GPU has finished drawing inside the VertexBuffer.SetData call, but we
        // don't want to waste time waiting around every time we try to add a new
        // particle! To avoid this delay, we can specify the SetDataOptions.NoOverwrite
        // flag when we write to the vertex buffer. This basically means "I promise I
        // will never try to overwrite any data that the GPU might still be using, so
        // you can just go ahead and update the buffer straight away". To keep this
        // promise, we must avoid reusing vertices immediately after they are drawn.
        //
        // So in total, our queue contains four different regions:
        //
        // Vertices between firstActiveParticle and firstNewParticle are actively
        // being drawn, and exist in both the managed particles array and the GPU
        // vertex buffer.
        //
        // Vertices between firstNewParticle and firstFreeParticle are newly created,
        // and exist only in the managed particles array. These need to be uploaded
        // to the GPU at the start of the next draw call.
        //
        // Vertices between firstFreeParticle and firstRetiredParticle are free and
        // waiting to be allocated.
        //
        // Vertices between firstRetiredParticle and firstActiveParticle are no longer
        // being drawn, but were drawn recently enough that the GPU could still be
        // using them. These need to be kept around for a few more frames before they
        // can be reallocated.

        int firstActiveParticle;
        int firstNewParticle;
        int firstFreeParticle;
        int firstRetiredParticle;


        // Store the current time, in seconds.
        float currentTime;


        // Count how many times Draw has been called. This is used to know
        // when it is safe to retire old particles back into the free list.
        int drawCounter;


        // Shared random number generator.
        static Random random = new Random();


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PointParticleSystem(Game game)
            : base(game)
        {
            
        }


        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Initialize()
        {
            //InitializeSettings(settings);

            //particles = new PointParticleVertex[settings.MaxParticles];

            base.Initialize();
        }
        public void Scale(float size)
        {
            settings.MinStartSize = size;
            settings.MaxStartSize = size;
        }

        /// <summary>
        /// Derived particle system classes should override this method
        /// and use it to initalize their tweakable settings.
        /// </summary>
        public void InitializeSettings(PointParticleSettings Settings)
        {
            settings = Settings;
            particles = new PointParticleVertex[settings.MaxParticles];
            //Initialize();
        }


        /// <summary>
        /// Loads graphics for the particle system.
        /// </summary>
        public void LoadContent()
        {

            LoadParticleEffect();
            vertexDeclaration = new VertexDeclaration(REngine.Instance._game.GraphicsDevice,
                                                      PointParticleVertex.VertexElements);

            // Create a dynamic vertex buffer.
            int size = PointParticleVertex.SizeInBytes * particles.Length;

            vertexBuffer = new DynamicVertexBuffer(REngine.Instance._game.GraphicsDevice, size, 
                                                   BufferUsage.WriteOnly |
                                                   BufferUsage.Points);
            
        }


        /// <summary>
        /// Helper for loading and initializing the particle effect.
        /// </summary>
        void LoadParticleEffect()
        {
#if !XBOX
            Effect effect = REngine.Instance._resourceContent.Load<Effect>("ParticleSystem");
#else
            Effect effect = REngine.Instance._resourceContent.Load<Effect>("ParticleSystem1");
#endif



            particleEffect = effect;

            EffectParameterCollection parameters = particleEffect.Parameters;

            // Look up shortcuts for parameters that change every frame.
            effectViewParameter = parameters["View"];
            effectProjectionParameter = parameters["Projection"];
            effectViewportHeightParameter = parameters["ViewportHeight"];
            effectTimeParameter = parameters["CurrentTime"];

            // Set the values of parameters that do not change.
            parameters["Duration"].SetValue((float)settings.Duration.TotalSeconds);
            parameters["DurationRandomness"].SetValue(settings.DurationRandomness);
            parameters["Gravity"].SetValue(settings.Gravity);
            parameters["EndVelocity"].SetValue(settings.EndVelocity);
            parameters["MinColor"].SetValue(settings.MinColor.ToVector4());
            parameters["MaxColor"].SetValue(settings.MaxColor.ToVector4());

            parameters["RotateSpeed"].SetValue(
                new Vector2(settings.MinRotateSpeed, settings.MaxRotateSpeed));
            
            parameters["StartSize"].SetValue(
                new Vector2(settings.MinStartSize, settings.MaxStartSize));
            
            parameters["EndSize"].SetValue(
                new Vector2(settings.MinEndSize, settings.MaxEndSize));

            // Load the particle texture, and set it onto the effect.
            //Texture2D texture = content.Load<Texture2D>(settings.TextureName);

            parameters["Texture"].SetValue(RTextureFactory.Instance._textureList[settings.TextureID]);

            // Choose the appropriate effect technique. If these particles will never
            // rotate, we can use a simpler pixel shader that requires less GPU power.
            string techniqueName;

            if ((settings.MinRotateSpeed == 0) && (settings.MaxRotateSpeed == 0))
                techniqueName = "NonRotatingParticles";
            else
                techniqueName = "RotatingParticles";

            particleEffect.CurrentTechnique = particleEffect.Techniques[techniqueName];
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the particle system.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            RetireActiveParticles();
            FreeRetiredParticles();

            // If we let our timer go on increasing for ever, it would eventually
            // run out of floating point precision, at which point the particles
            // would render incorrectly. An easy way to prevent this is to notice
            // that the time value doesn't matter when no particles are being drawn,
            // so we can reset it back to zero any time the active queue is empty.

            if (firstActiveParticle == firstFreeParticle)
                currentTime = 0;

            if (firstRetiredParticle == firstActiveParticle)
                drawCounter = 0;
        }


        /// <summary>
        /// Helper for checking when active particles have reached the end of
        /// their life. It moves old particles from the active area of the queue
        /// to the retired section.
        /// </summary>
        void RetireActiveParticles()
        {
            float particleDuration = (float)settings.Duration.TotalSeconds;

            while (firstActiveParticle != firstNewParticle)
            {
                // Is this particle old enough to retire?
                float particleAge = currentTime - particles[firstActiveParticle].Time;

                if (particleAge < particleDuration)
                    break;

                // Remember the time at which we retired this particle.
                particles[firstActiveParticle].Time = drawCounter;

                // Move the particle from the active to the retired queue.
                firstActiveParticle++;

                if (firstActiveParticle >= particles.Length)
                    firstActiveParticle = 0;
            }
        }


        /// <summary>
        /// Helper for checking when retired particles have been kept around long
        /// enough that we can be sure the GPU is no longer using them. It moves
        /// old particles from the retired area of the queue to the free section.
        /// </summary>
        void FreeRetiredParticles()
        {
            while (firstRetiredParticle != firstActiveParticle)
            {
                // Has this particle been unused long enough that
                // the GPU is sure to be finished with it?
                int age = drawCounter - (int)particles[firstRetiredParticle].Time;

                // The GPU is never supposed to get more than 2 frames behind the CPU.
                // We add 1 to that, just to be safe in case of buggy drivers that
                // might bend the rules and let the GPU get further behind.
                if (age < 3)
                    break;

                // Move the particle from the retired to the free queue.
                firstRetiredParticle++;

                if (firstRetiredParticle >= particles.Length)
                    firstRetiredParticle = 0;
            }
        }

        
        /// <summary>
        /// Draws the particle system.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = REngine.Instance._graphics.GraphicsDevice;

            // Restore the vertex buffer contents if the graphics device was lost.
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(particles);
            }

            // If there are any particles waiting in the newly added queue,
            // we'd better upload them to the GPU ready for drawing.
            if (firstNewParticle != firstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            // If there are any active particles, draw them now!
            if (firstActiveParticle != firstFreeParticle)
            {
                SetParticleRenderStates(device.RenderState);

                // Set an effect parameter describing the viewport size. This is needed
                // to convert particle sizes into screen space point sprite sizes.
                effectViewportHeightParameter.SetValue(REngine.Instance.GetViewport().Height);
                effectViewParameter.SetValue(REngine.Instance._camera.viewMatrix);
                effectProjectionParameter.SetValue(REngine.Instance._camera.projMatrix);
                // Set an effect parameter describing the current time. All the vertex
                // shader particle animation is keyed off this value.
                effectTimeParameter.SetValue(currentTime);

                // Set the particle vertex buffer and vertex declaration.
                device.Vertices[0].SetSource(vertexBuffer, 0,
                                             PointParticleVertex.SizeInBytes);

                device.VertexDeclaration = vertexDeclaration;

                // Activate the particle effect.
                particleEffect.Begin();

                foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    if (firstActiveParticle < firstFreeParticle)
                    {
                        // If the active particles are all in one consecutive range,
                        // we can draw them all in a single call.
                        device.DrawPrimitives(PrimitiveType.PointList,
                                              firstActiveParticle,
                                              firstFreeParticle - firstActiveParticle);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        device.DrawPrimitives(PrimitiveType.PointList,
                                              firstActiveParticle,
                                              particles.Length - firstActiveParticle);

                        if (firstFreeParticle > 0)
                        {
                            device.DrawPrimitives(PrimitiveType.PointList,
                                                  0,
                                                  firstFreeParticle);
                        }
                    }

                    pass.End();
                }

                particleEffect.End();

                // Reset a couple of the more unusual renderstates that we changed,
                // so as not to mess up any other subsequent drawing.
                device.RenderState.PointSpriteEnable = false;
                device.RenderState.DepthBufferWriteEnable = true;
            }

            drawCounter++;
        }


        /// <summary>
        /// Helper for uploading new particles from our managed
        /// array to the GPU vertex buffer.
        /// </summary>
        void AddNewParticlesToVertexBuffer()
        {
            int stride = PointParticleVertex.SizeInBytes;

            if (firstNewParticle < firstFreeParticle)
            {
                // If the new particles are all in one consecutive range,
                // we can upload them all in a single call.
                vertexBuffer.SetData(firstNewParticle * stride, particles,
                                     firstNewParticle,
                                     firstFreeParticle - firstNewParticle,
                                     stride, SetDataOptions.NoOverwrite);
            }
            else
            {
                // If the new particle range wraps past the end of the queue
                // back to the start, we must split them over two upload calls.
                vertexBuffer.SetData(firstNewParticle * stride, particles,
                                     firstNewParticle,
                                     particles.Length - firstNewParticle,
                                     stride, SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(0, particles,
                                         0, firstFreeParticle,
                                         stride, SetDataOptions.NoOverwrite);
                }
            }

            // Move the particles we just uploaded from the new to the active queue.
            firstNewParticle = firstFreeParticle;
        }


        /// <summary>
        /// Helper for setting the renderstates used to draw particles.
        /// </summary>
        void SetParticleRenderStates(RenderState renderState)
        {
            // Enable point sprites.
            renderState.PointSpriteEnable = true;
            renderState.PointSizeMax = 256;

            // Set the alpha blend mode.
            renderState.AlphaBlendEnable = true;
            renderState.AlphaBlendOperation = BlendFunction.Add;
            renderState.SourceBlend = settings.SourceBlend;
            renderState.DestinationBlend = settings.DestinationBlend;

            // Set the alpha test mode.
            renderState.AlphaTestEnable = true;
            renderState.AlphaFunction = CompareFunction.Greater;
            renderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;

            //renderState.FogEnable = false;
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Sets the camera view and projection matrices
        /// that will be used to draw this particle system.
        /// </summary>
        public void SetCamera(Matrix view, Matrix projection)
        {
            effectViewParameter.SetValue(view);
            effectProjectionParameter.SetValue(projection);
        }


        /// <summary>
        /// Adds a new particle to the system.
        /// </summary>
        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= particles.Length)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == firstRetiredParticle)
                return;

            // Adjust the input velocity based on how much
            // this particle system wants to be affected by it.
            velocity *= settings.EmitterVelocitySensitivity;

            // Add in some random amount of horizontal velocity.
            float horizontalVelocity = MathHelper.Lerp(settings.MinHorizontalVelocity,
                                                       settings.MaxHorizontalVelocity,
                                                       (float)random.NextDouble());

            double horizontalAngle = random.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            // Add in some random amount of vertical velocity.
            velocity.Y += MathHelper.Lerp(settings.MinVerticalVelocity,
                                          settings.MaxVerticalVelocity,
                                          (float)random.NextDouble());

            // Choose four random control values. These will be used by the vertex
            // shader to give each particle a different size, rotation, and color.
            Color randomValues = new Color((byte)random.Next(255),
                                           (byte)random.Next(255),
                                           (byte)random.Next(255),
                                           (byte)random.Next(255));

            // Fill in the particle vertex structure.
            particles[firstFreeParticle].Position = position;
            particles[firstFreeParticle].Velocity = velocity;
            particles[firstFreeParticle].Random = randomValues;
            particles[firstFreeParticle].Time = currentTime;

            firstFreeParticle = nextFreeParticle;
        }


        #endregion
    }

    internal class PointParticleSettings
    {
        // Name of the texture used by this particle system.
        public int TextureID = 0;


        // Maximum number of particles that can be displayed at one time.
        public int MaxParticles = 100;


        // How long these particles will last.
        public TimeSpan Duration = TimeSpan.FromSeconds(1);


        // If greater than zero, some particles will last a shorter time than others.
        public float DurationRandomness = 0;


        // Controls how much particles are influenced by the velocity of the object
        // which created them. You can see this in action with the explosion effect,
        // where the flames continue to move in the same direction as the source
        // projectile. The projectile trail particles, on the other hand, set this
        // value very low so they are less affected by the velocity of the projectile.
        public float EmitterVelocitySensitivity = 1;


        // Range of values controlling how much X and Z axis velocity to give each
        // particle. Values for individual particles are randomly chosen from somewhere
        // between these limits.
        public float MinHorizontalVelocity = 0;
        public float MaxHorizontalVelocity = 0;


        // Range of values controlling how much Y axis velocity to give each particle.
        // Values for individual particles are randomly chosen from somewhere between
        // these limits.
        public float MinVerticalVelocity = 0;
        public float MaxVerticalVelocity = 0;


        // Direction and strength of the gravity effect. Note that this can point in any
        // direction, not just down! The fire effect points it upward to make the flames
        // rise, and the smoke plume points it sideways to simulate wind.
        public Vector3 Gravity = Vector3.Zero;


        // Controls how the particle velocity will change over their lifetime. If set
        // to 1, particles will keep going at the same speed as when they were created.
        // If set to 0, particles will come to a complete stop right before they die.
        // Values greater than 1 make the particles speed up over time.
        public float EndVelocity = 1;


        // Range of values controlling the particle color and alpha. Values for
        // individual particles are randomly chosen from somewhere between these limits.
        public Color MinColor = Color.White;
        public Color MaxColor = Color.White;


        // Range of values controlling how fast the particles rotate. Values for
        // individual particles are randomly chosen from somewhere between these
        // limits. If both these values are set to 0, the particle system will
        // automatically switch to an alternative shader technique that does not
        // support rotation, and thus requires significantly less GPU power. This
        // means if you don't need the rotation effect, you may get a performance
        // boost from leaving these values at 0.
        public float MinRotateSpeed = 0;
        public float MaxRotateSpeed = 0;


        // Range of values controlling how big the particles are when first created.
        // Values for individual particles are randomly chosen from somewhere between
        // these limits.
        public float MinStartSize = 100;
        public float MaxStartSize = 100;


        // Range of values controlling how big particles become at the end of their
        // life. Values for individual particles are randomly chosen from somewhere
        // between these limits.
        public float MinEndSize = 100;
        public float MaxEndSize = 100;


        // Alpha blending settings.
        public Blend SourceBlend = Blend.SourceAlpha;
        public Blend DestinationBlend = Blend.InverseSourceAlpha;
    }
    


    public class RParticleEmitter : RSceneNode, IDisposable
    {
        internal BillboardParticleEmitter _emitter;
        internal PointParticleSystem _psystem;
        internal List<PointParticleEmitter> _emitters;
        internal CONST_REACTOR_PARTICLETYPE _type;
        internal int NumParticles = 0;
        internal int TextureID = 0;
        internal Vector3 OverallVelocity;
        internal PointParticleSettings settings;
        public void AddPointParticle(R3DVECTOR Position, R3DVECTOR Velocity)
        {
            _psystem.AddParticle(Position.vector, Velocity.vector);
        }
        internal void CreateEmitter(CONST_REACTOR_PARTICLETYPE type)
        {
            _type = type;
            if (type == CONST_REACTOR_PARTICLETYPE.Billboard)
            {
                _emitter = new BillboardParticleEmitter();
                _emitter.LoadContent();
            }
            else if (type == CONST_REACTOR_PARTICLETYPE.Point)
            {
                _psystem = new PointParticleSystem(REngine.Instance._game);
                _psystem.Initialize();
            }
        }
        public void SetPointSettings(int Texture)
        {
            SetPointSettings(Texture, 600, 10, 0, 0, 10, 20, new R3DVECTOR(0, 0, 0), 0.75f, -1f, 1f, 5f, 10f, 50f, 200f);
        }
        public void SetPointSettings(int Texture, int NumParticles)
        {
            SetPointSettings(Texture, NumParticles, 10, 0, 0, 10, 20, new R3DVECTOR(0, 0, 0), 0.75f, -1f, 1f, 5f, 10f, 50f, 200f);
        }
        public void SetPointSettings(int Texture, int NumParticles, int Duration)
        {
            SetPointSettings(Texture, NumParticles, Duration, 0, 0, 1, 5, new R3DVECTOR(0, 0, 0), 0.75f, -1f, 1f, 10f, 10f, 10f, 10f);
        }
        public void SetPointSettings(int Texture, int NumParticles, int DurationSeconds, float MinHorizontalVelocity, float MaxHorizontalVelocity, float MinVerticalVelocity, float MaxVerticalVelocity, R3DVECTOR Gravity, float EndVelocity, float MinRotateSpeed, float MaxRotateSpeed, float MinStartSize, float MaxStartSize, float MinEndSize, float MaxEndSize)
        {
            settings = new PointParticleSettings();
            settings.TextureID = Texture;

            settings.MaxParticles = NumParticles;

            settings.Duration = TimeSpan.FromSeconds(DurationSeconds);

            settings.MinHorizontalVelocity = MinHorizontalVelocity;
            settings.MaxHorizontalVelocity = MaxHorizontalVelocity;

            settings.MinVerticalVelocity = MinVerticalVelocity;
            settings.MaxVerticalVelocity = MaxVerticalVelocity;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = Gravity.vector;

            settings.EndVelocity = EndVelocity;

            settings.MinRotateSpeed = MinRotateSpeed;
            settings.MaxRotateSpeed = MaxRotateSpeed;

            settings.MinStartSize = MinStartSize;
            settings.MaxStartSize = MaxStartSize;

            settings.MinEndSize = MinEndSize;
            settings.MaxEndSize = MaxEndSize;
            
            _psystem.InitializeSettings(settings);
        }
        public void BuildEmitter(int TotalParticleCount)
        {
            NumParticles = TotalParticleCount;
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            {
                _emitter.partCount = TotalParticleCount;
                _emitter.LoadParticles();
            }
            else if (_type == CONST_REACTOR_PARTICLETYPE.Point)
            {
                OverallVelocity = new Vector3(0, 0, 0);
                SetPointSettings(TextureID, NumParticles);
                _psystem.LoadContent();
            }
        }
        public void SetPosition(R3DVECTOR Position)
        {
            this.Position = Position.vector;
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
                _emitter.targetPos = Position.vector;
            
                
            
        }
        public void SetPosition(float x, float y, float z)
        {
            Vector3 v = new Vector3(x, y, z);
            this.Position = v;
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            _emitter.targetPos = v;
        }
        
        public R3DVECTOR GetPosition()
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
                return R3DVECTOR.FromVector3(_emitter.targetPos);
            else
                return R3DVECTOR.FromVector3(this.Position);
        }

        public void SetRotation(RQUATERNION Quaternion)
        {
            Quaternion q = Quaternion.quaternion;
            this.Quaternion = q;
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            _emitter.myRotation = q;
        }
        public RQUATERNION GetRotation()
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
                return new RQUATERNION(_emitter.myRotation);
            else
                return new RQUATERNION(Quaternion.Identity);
        }
        public void Rotate(float x, float y, float z)
        {
            this.Rotation *= new Vector3(x, y, z);
            Matrix m = Matrix.CreateFromQuaternion(_emitter.myRotation);
            m *= Matrix.CreateRotationX(MathHelper.ToRadians(x));
            m *= Matrix.CreateRotationY(MathHelper.ToRadians(y));
            m *= Matrix.CreateRotationZ(MathHelper.ToRadians(z));
            Rotate(R3DMATRIX.FromMatrix(m));

        }
        internal void Rotate(R3DMATRIX RotationMatrix)
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            _emitter.myRotation = Quaternion.CreateFromRotationMatrix(RotationMatrix.matrix);
        }

        public void Rotate(R3DVECTOR RotationVectors)
        {
            Rotate(RotationVectors.X, RotationVectors.Y, RotationVectors.Z);
        }
        public void SetBillboardSize(float Size)
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            {
                _emitter.Size = Size;
            }
        }
        public void SetScale(float scale)
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            _emitter.myScale = new Vector3(scale, scale, scale);
        }

        public void SetScale(R3DVECTOR Scale)
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            _emitter.myScale = Scale.vector;
        }

        public void SetTexture(int TextureID)
        {
            this.TextureID = TextureID;
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            _emitter.SetTexture(TextureID);
            
        }
        public void SetDirection(R3DVECTOR Direction)
        {
            OverallVelocity = Direction.vector;
        }
        public void Update()
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
            {
                //_emitter.targetPos += Vector3.Normalize(OverallVelocity) * 0.05f;
                _emitter.Update();
            }
            else if (_type == CONST_REACTOR_PARTICLETYPE.Point)
            {
                try
                {
                    _psystem.AddParticle(this.Position, OverallVelocity);
                    _psystem.Update(REngine.Instance._gameTime);
                    _psystem.Visible = true;
                    _psystem.Enabled = true;
                }
                catch { }
            }
            else { }
        }

        public void Render()
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
                _emitter.Render();
            else if (_type == CONST_REACTOR_PARTICLETYPE.Point)
            {
                //_psystem.SetCamera(REngine.Instance._camera.viewMatrix, REngine.Instance._camera.projMatrix);
                _psystem.Draw(REngine.Instance._gameTime);
            }
            else { }
        }
        public void Dispose()
        {
            if (_type == CONST_REACTOR_PARTICLETYPE.Billboard)
                _emitter = null;
            else if (_type == CONST_REACTOR_PARTICLETYPE.Point)
                _psystem.Dispose();
            else { }
        }
    }
}