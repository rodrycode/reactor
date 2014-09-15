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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
namespace Reactor
{
    public class RGame : IDisposable
    {
        ReactorGame game;
        public REngine Reactor;
        public RGame()
        {
            game = new ReactorGame(this);
            Reactor = new REngine(game);
        }
        public void Run()
        {
            game.Run();
            Dispose();
        }
        public void Exit()
        {
            game.Exit();
        }
#if XBOX
        public bool IsTrialMode()
        {
            return Guide.IsTrialMode;
        }
#endif
        public virtual void Init() { }

        public virtual void Render() { }
        public virtual void Render2D() { }
        public virtual void Update() { }

        public virtual void Destroy()
        {
           
        }

        public virtual void Dispose()
        {
            
        }

        public virtual void Load()
        {
            
        }

    }
        /// <summary>
        /// This is the main type for your game
        /// </summary>
        public class ReactorGame : Game
        {
            internal GraphicsDeviceManager graphics;
            internal Texture2D watermark;
            internal SpriteBatch waterrenderer;
            internal Rectangle waterrect;
            internal ContentManager loader;
#if XBOX
            internal GamerServicesComponent services;
#endif
            //internal RCameraComponent cameraComponent;
            //internal pmv1 pmv;
            internal RGame game;
            public ReactorGame(RGame RGameInstance)
            {
                game = RGameInstance;
                graphics = new GraphicsDeviceManager(this);
                //RGraphicEffect effect = new RGraphicEffect();
                //effect.BloomInit(RBloomSettings.PresetSettings[0], this);
                //cameraComponent = new RCameraComponent(this);
                //Components.Add(cameraComponent);
                
            }
            internal bool AllowEscapeQuit = true;
            internal void ReadWatermark()
            {
                //System.IO.Stream stream = (System.IO.Stream)System.IO.File.Open("watermark.dat", System.IO.FileMode.Open);
                //System.IO.TextReader reader = System.IO.File.OpenText("watermark.dat");
                watermark = new Texture2D(graphics.GraphicsDevice, 180, 60, false, SurfaceFormat.Color);
                List<Color> cwatermark = new List<Color>();
                
                // No Longer Included...
                //REngine.Instance.PrepareWatermark();
                int i = 0;
                for (int it = 0; it < REngine.Instance.cwatermark.Length / 4; it++)
                {
                    Color c = new Color(REngine.Instance.cwatermark[i], REngine.Instance.cwatermark[i + 1], REngine.Instance.cwatermark[i + 2], REngine.Instance.cwatermark[i + 3]);
                    cwatermark.Add(c);
                    i += 4;

                    
                }
                watermark.SetData<Color>(cwatermark.ToArray());
                cwatermark.Clear();
                cwatermark = null;
                REngine.Instance.cwatermark = null;
                //reader.Close();
            }
            internal void WriteWatermark()
            {
                //System.IO.Stream stream = (System.IO.Stream)System.IO.File.Open("watermark.txt", System.IO.FileMode.Create);
                System.IO.TextWriter writer = (System.IO.StreamWriter)System.IO.File.CreateText("watermark.txt");
                loader = new ContentManager(Services);
                watermark = loader.Load<Texture2D>(@"C:\\watermark");
                Color[] cwatermark = new Color[watermark.Width * watermark.Height];
                watermark.GetData<Color>(cwatermark);
                
                foreach (Color c in cwatermark)
                {
                    
                    writer.WriteLine(c.R.ToString()+",");
                    writer.WriteLine(c.G.ToString() + ",");
                    writer.WriteLine(c.B.ToString() + ",");
                    writer.WriteLine(c.A.ToString() + ",");

                }
                writer.Close();
            }
            int wftimer = 0;
            internal void RenderWatermark()
            {

                if (!REngine.Instance.cLicensed)
                {
                    try
                    {
                        REngine.Instance.SetWatermarkPosition(REngine.Instance.watermarkposition);
                        
                        RScreen2D.Instance._spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                        RScreen2D.Instance._spritebatch.Draw(watermark, REngine.Instance.cwaterrect, new Color(wc.ToVector4()));
                        RScreen2D.Instance._spritebatch.End();
                        //RScreen2D.IAction_End2D();

                        if (wftimer < 5000)
                        {
                            wftimer++;
                        }
                        if (wftimer > 900)
                        {
                            Vector4 vt = new Vector4(1f, 1f, 1f, ((wc.A - 1) / 256f * (0.0000000000001f * REngine.Instance.AccurateTimeElapsed())));
                            wc = new Color(vt);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        REngine.Instance.AddToLog(e.ToString());
                    }
                    
                }

            }
            Color wc = Color.White;
            /// <summary>
            /// Allows the game to perform any initialization it needs to before starting to run.
            /// This is where it can query for any required services and load any non-graphic
            /// related content.  Calling base.Initialize will enumerate through any components
            /// and initialize them as well.
            /// </summary>
            protected override void Initialize()
            {
                // TODO: Add your initialization logic here

                
                base.Initialize();
                
                REngine.Instance._graphics = graphics;
                

                REngine.Instance._resourceContent = new ResourceContentManager(Services, Resources.ResourceManager);
#if !XBOX
                REngine.Instance._systemfont = REngine.Instance._resourceContent.Load<SpriteFont>("Tahoma");
#else
                REngine.Instance._systemfont = REngine.Instance._resourceContent.Load<SpriteFont>("Tahoma1");
#endif
                game.Init();

#if XBOX
                services = new GamerServicesComponent(this);
                services.Initialize();
                this.Components.Add(services);

#endif
                
                //pmv = new pmv1(this, graphics);
                //pmv.Initialize();
                //Components.Add(pmv);
#if !XBOX
                //this.TargetElapsedTime = TimeSpan.FromMilliseconds(1.0);
                //this.InactiveSleepTime = TimeSpan.FromMilliseconds(1.0);
                //this.IsFixedTimeStep = false;
#endif
                //graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
                //graphics.PreferredDepthStencilFormat = DepthFormat.Depth32;
#if !XBOX
                this.graphics.SynchronizeWithVerticalRetrace = true;
                this.graphics.PreferMultiSampling = false;
#else
                this.graphics.SynchronizeWithVerticalRetrace = true;
                this.graphics.PreferMultiSampling = false;
#endif
                
                this.graphics.ApplyChanges();
#if !XBOX || !XBOX360
                //this.graphics.MinimumPixelShaderProfile = ShaderProfile.PS_3_0;
                //this.graphics.MinimumVertexShaderProfile = ShaderProfile.VS_3_0;               
#endif
                
                game.Load();
                
            }

            
            

            
            
            /// <summary>
            /// Load your graphics content.  If loadAllContent is true, you should
            /// load content from both ResourceManagementMode pools.  Otherwise, just
            /// load ResourceManagementMode.Manual content.
            /// </summary>
            /// <param name="loadAllContent">Which type of content to load.</param>
            protected override void LoadContent()
            {
                
                    if (waterrenderer == null)
                    {
                        waterrenderer = new SpriteBatch(graphics.GraphicsDevice);
                        
                        
                    }

                    
                
                
                // TODO: Load any ResourceManagementMode.Manual content
            }


            /// <summary>
            /// Unload your graphics content.  If unloadAllContent is true, you should
            /// unload content from both ResourceManagementMode pools.  Otherwise, just
            /// unload ResourceManagementMode.Manual content.  Manual content will get
            /// Disposed by the GraphicsDevice during a Reset.
            /// </summary>
            /// <param name="unloadAllContent">Which type of content to unload.</param>
            protected override void UnloadContent()
            {
                
                    // TODO: Unload any ResourceManagementMode.Automatic content
                    try
                    {
                       
                        //watermark = null;
                        //loader.Unload();
                        //loader = null;
                        //waterrenderer.Dispose();
                        //waterrenderer = null;
                    }
                    catch
                    {
                    }
                    
                
                //game.Destroy();
                // TODO: Unload any ResourceManagementMode.Manual content
            }


            /// <summary>
            /// Allows the game to run logic such as updating the world,
            /// checking for collisions, gathering input and playing audio.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Update(GameTime gameTime)
            {
                REngine.Instance._gameTime = gameTime;
                
                game.Update();
#if XBOX
                services.Update(gameTime);
#endif
                // Allows the game to exit in case of emergency
                if (AllowEscapeQuit)
                {
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                        this.Exit();
                }
                //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
                //pmv.Update(gameTime);
                // TODO: Add your update logic here
                base.Update(gameTime);
                
                
            }

            
            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Draw(GameTime gameTime)
            {
                if (RScene.Instance != null)
                {
                    if (RScene.Instance._LandscapeList.Count > 0)
                    {
                        foreach (object land in RScene.Instance._LandscapeList)
                        {
                            if (land is RLandscape)
                            {
                                ((RLandscape)land).OcclusionRender();
                            }
                        }
                    }
                    
                }
                if (RGraphicEffect._instance != null)
                {
                    RGraphicEffect._instance.Bloom_Render();
                }
                if (RWater.water != null)
                {
                    try
                    {
                        RWater.water.UpdateWaterMaps();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        REngine.Instance.AddToLog(e.ToString());
                    }
                    graphics.GraphicsDevice.Clear(new Color(RWater.water.Options.WaterColor.vector));
                }
                else
                {
                    graphics.GraphicsDevice.Clear(Color.Black);
                }
                
                game.Render();
                // TODO: Add your drawing code here
                
                //RenderWatermark();
                //REngine.Instance.DrawFps();

                
                //pmv.Draw(gameTime);
                base.Draw(gameTime);
                if (RScreen2D.Instance != null)
                    RScreen2D.Instance.Update();
                if (REngine.Instance._w32handle != null)
                {
                    if (REngine.Instance._w32handle.ToInt32() != 0)
                    {
                        REngine.Instance._graphics.GraphicsDevice.Present();
                    }
                }
                //graphics.GraphicsDevice.SetRenderTarget(0, null);
                
            }
            
        }
    
}