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
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;


namespace Reactor
{
    
    public delegate void ViewportResized();
    public delegate void DeviceReset();
    public class REngine
    {
        static REngine _instance;

        #region Watermark
#if !XBOX
        internal bool cLicensed = true;
#else
        internal bool cLicensed = true;
#endif
        internal byte[] cwatermark;
        internal Rectangle cwaterrect;
        
        #endregion

        #region Private/Internal Members

        GraphicsAdapter _adapter;
        internal float _aspectRatio;
        internal RCamera _camera;
        internal GraphicsDeviceManager _graphics;
        internal GameTime _gameTime;
        internal Matrix _worldMatrix;
        GraphicsDevice _device;
        GameServiceContainer _services;
        GameComponentCollection _gamecomponents;
        internal ReactorGame _game;
        internal SpriteFont _systemfont;
        internal SpriteBatch _system2d;
        internal ResourceContentManager _resourceContent;
		internal ContentManager _content;
        internal TextWriter _logwriter;
        internal string _logfile;
        internal StringBuilder _log;
        internal bool _debugEnabled;
        internal double _fps = 0.0f;
        internal int _frames = 0;
        bool DisplayFPS = true;
        // viewport management
        Viewport _currentViewport = new Viewport();
        List<Viewport> _viewportList = new List<Viewport>();
        List<string> _viewportKeys = new List<string>();
        #endregion

        #region Properties
        public static REngine Instance
        {
            get { return _instance; }
        }
        #endregion

        #region Methods
        
        public REngine(ReactorGame game)
        {
            if (_instance == null)
            {
                _instance = this;
                _instance._game = game;
				_instance._content = game.Content;
                _instance._currentViewport = CreateViewport("root");
                _instance._camera = new RCamera();
                _instance._logfile = Path.GetFullPath("reactordebug.txt");
                //_instance._logwriter = (TextWriter)new StreamWriter(File.Open(_instance._logfile, FileMode.CreateNew, FileAccess.Write, FileShare.Read));
                _instance._log = new StringBuilder();
                
                
                
                
            }
            else
            {
                return;
            }
        }
        PresentationParameters pp;
        internal IntPtr _w32handle;
        
        public bool Init3DWindowed(IntPtr window)
        {
            _instance._w32handle = window;
              
            return true;
        }
#if !XBOX
        
        internal RenderTarget2D _renderTarget;
        
#endif
        
        public bool Init3DGame(int iWidth, int iHeight, bool Fullscreen)
        {

            _instance.AddToLog("Reactor 3D Starting Up");
            _instance._graphics.PreferredBackBufferHeight = iHeight;
            _instance._graphics.PreferredBackBufferWidth = iWidth;
            _instance._graphics.GraphicsDevice.DeviceReset += new EventHandler<System.EventArgs>(GraphicsDevice_DeviceReset);
            _currentViewport.Height = iHeight;
            _currentViewport.Width = iWidth;
            _aspectRatio = iWidth / iHeight;
            _instance._graphics.ApplyChanges();
            _instance.AddToLog("Graphics Device Created and Viewport resized for "+iWidth+" x "+iHeight+"");
            
            //_instance._graphics.IsFullScreen = Fullscreen;
            if (Fullscreen)
                _instance._graphics.ToggleFullScreen();
            //_instance._w32handle = _instance._game.Window.Handle;

            //_instance._graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            
            _instance._system2d = new SpriteBatch(_graphics.GraphicsDevice);

            _instance._graphics.ApplyChanges();



            return true;
        }

        void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            if(_instance.DeviceReset!=null)
                _instance.DeviceReset();
            
        }
        
        public void ShowFPS(bool displayFPS)
        {
            _instance.DisplayFPS = displayFPS;
        }
        public void ShowMouse(bool Show)
        {
           _instance._game.IsMouseVisible = Show;
        }
	    public double GetFPS()
        {
            return _fps;
        }

        public void SetWindowTitle(string Title)
        {
            _instance._game.Window.Title = Title;
        }
        public String GetWindowTitle()
        {
            return _instance._game.Window.Title;
        }

        public void SetWindowPosition(R2DVECTOR v)
        {
#if !XBOX
            Type type = typeof(OpenTKGameWindow);
            System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            OpenTK.GameWindow window = (OpenTK.GameWindow)field.GetValue(_instance._game.Window);
            window.X = (int)v.X;
            window.Y = (int)v.Y;
#endif
        }

        public void SetWindowBorderless(bool Borderless)
        {
            Type type = typeof(OpenTKGameWindow);
            System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            OpenTK.GameWindow window = (OpenTK.GameWindow)field.GetValue(_instance._game.Window);
            if (Borderless)
                window.WindowBorder = OpenTK.WindowBorder.Hidden;
            else
                window.WindowBorder = OpenTK.WindowBorder.Resizable;

        }
#if XBOX
        public Microsoft.Xna.Framework.GamerServices.GamerServicesComponent GetServices()
        {
            return _game.services;
        }
        
#endif
        public void TakeScreenshot(string savePath)
        {
            
            Texture2D texture = new Texture2D(_instance._graphics.GraphicsDevice, _instance._currentViewport.Width, _instance._currentViewport.Height, true, SurfaceFormat.Color);
            //_instance._graphics.GraphicsDevice.ResolveBackBuffer(texture);
            if (File.Exists(savePath + ".jpg"))
            {
                File.Delete(savePath + ".jpg");
            }
#if !XBOX
            texture.SaveAsJpeg(new FileStream("", FileMode.Create), texture.Width, texture.Height);            
#endif
            _instance.AddToLog("REngine Successfully Created a Screenshot at : " + savePath + ".jpg");
            texture.Dispose();
            return;
        }
        internal CONST_REACTOR_WATERMARK_POSITION watermarkposition = CONST_REACTOR_WATERMARK_POSITION.TOP_RIGHT;
        public void SetWatermarkPosition(CONST_REACTOR_WATERMARK_POSITION WatermarkPosition)
        {
            watermarkposition = WatermarkPosition;
            SetWatermarkPosition((int)WatermarkPosition);
        }
        internal void SetWatermarkPosition(int position)
        {
            if (position == 0)
            {
                cwaterrect = new Rectangle(5, 5, 180, 60);
                
#if XBOX
                cwaterrect.X += 40;
                cwaterrect.Y += 20;
#endif
            }
            if (position == 1)
            {
                cwaterrect = new Rectangle((_game.GraphicsDevice.Viewport.Width - 5) - 180, 5, 180, 60);
#if XBOX
                cwaterrect.X -= 40;
                cwaterrect.Y += 20;
#endif
            }
            if (position == 2)
            {
                cwaterrect = new Rectangle(5, (_game.GraphicsDevice.Viewport.Height - 5) - 60, 180, 60);
#if XBOX
                cwaterrect.X += 40;
                cwaterrect.Y -= 20;
#endif
            }
            if (position == 3)
            {
                cwaterrect = new Rectangle((_game.GraphicsDevice.Viewport.Width - 5) - 180, (_game.GraphicsDevice.Viewport.Height - 5) - 60, 180, 60);
#if XBOX
                cwaterrect.X -= 40;
                cwaterrect.Y -= 20;
#endif
            }
        }
        public bool IsWidescreen()
        {
            return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio <= (16 / 9);
        }
        public void ForceClear()
        {
            ForceClear(false);
        }
        public void ForceClear(bool OnlyDepth)
        {
            if (OnlyDepth)
                _game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 0.0f, 1);
            else
                _game.GraphicsDevice.Clear(Color.Black);
        }
        public void ForcePresent()
        {
            _game.GraphicsDevice.Present();
        }
        public void SetXBox360Resolution(CONST_REACTOR_XBOX_RESOLUTION Resolution)
        {
            int iHeight=0, iWidth=0;
            PresentationParameters param = new PresentationParameters();
            if (Resolution == CONST_REACTOR_XBOX_RESOLUTION.r480P)
            {
                iHeight = 480;
                if (IsWidescreen())
                    iWidth = 720;
                else
                    iWidth = 640;
            }
            if (Resolution == CONST_REACTOR_XBOX_RESOLUTION.r720P)
            {
                iHeight = 720;
                if (IsWidescreen())
                    iWidth = 1280;
                else
                    iWidth = 960;
            }
            
            _instance._graphics.PreferredBackBufferHeight = iHeight;
            _instance._graphics.PreferredBackBufferWidth = iWidth;
            
            _currentViewport.Height = iHeight;
            _currentViewport.Width = iWidth;
            _aspectRatio = iWidth / iHeight;
            _instance._graphics.ApplyChanges();
            //_instance._graphics.GraphicsDevice.Reset(param);
            
        }
        
        public CONST_REACTOR_XBOX_RESOLUTION GetXBox360Resolution()
        {
            if (_instance._device.DisplayMode.Height == 480)
                return CONST_REACTOR_XBOX_RESOLUTION.r480P;
            if (_instance._device.DisplayMode.Height == 720)
                return CONST_REACTOR_XBOX_RESOLUTION.r720P;
            
            return 0;
        }
        static int _lastSecond = 0;
        static long initialMem;
        internal void DrawFps()
        {
            
            _frames++;
            if (_instance._gameTime.TotalGameTime.Seconds != _lastSecond)
            {
                _fps = _frames;
                _frames = 0;
                _lastSecond = _instance._gameTime.TotalGameTime.Seconds;
                
            }
            
            if (_instance.DisplayFPS)
            {
                //RScreen2D.IAction_Begin2D();
                _instance._system2d.Begin();
                Vector2 pos = new Vector2(5, 5);
#if XBOX
                pos.X += 50;
                pos.Y += 20;
#endif
                _instance._system2d.DrawString(_systemfont, _fps.ToString(), pos, Color.White);
                _instance._system2d.End();
                //RScreen2D.IAction_End2D();
            }
            else
            {
                //RScreen2D.IAction_Begin2D();
                //_instance._system2d.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                //_instance._system2d.DrawString(_systemfont, "", new Vector2(5, 5), Color.White);
                //_instance._system2d.End();
                //RScreen2D.IAction_End2D();
            }
        }
        internal void DrawString(Vector2 position, string message)
        {
            _instance._system2d.Begin();
            _instance._system2d.DrawString(_systemfont, message, position, Color.White);
            _instance._system2d.End();
        }
	    public void AddToLog(string sText)
        {
            _instance._log.AppendLine(sText);
            if (_instance._debugEnabled == true)
            {
                _instance._logwriter.WriteLine(DateTime.Now.ToString()+" : "+ sText);
                _instance._logwriter.Flush();
            }
        }

        public void Resize(int Width, int Height)
        {
            _instance._graphics.PreferredBackBufferWidth = Width;
            _instance._graphics.PreferredBackBufferHeight = Height;
            _instance._graphics.ApplyChanges();
        }
	    public void SetViewport(Viewport newViewport)
        {
            _instance._currentViewport = newViewport;
            _instance._graphics.GraphicsDevice.Viewport = newViewport;
            _instance._aspectRatio = newViewport.Width / newViewport.Height;
            _instance.AddToLog("Viewport Assigned "+newViewport.ToString());
        }
	    public Viewport GetViewport()
        {
            try
            {
                return _instance._game.GraphicsDevice.Viewport;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _instance.AddToLog(e.ToString());
                return new Viewport();
            }
        }
	    public Viewport CreateViewport(string sViewportName)
        {
            if (_viewportKeys.Contains(sViewportName))
            {
                int index = _instance._viewportKeys.IndexOf(sViewportName);
                _instance.AddToLog("Viewport Name already defined : "+sViewportName);
                return _instance._viewportList[index];
            }
            else
            {
            Viewport viewport = new Viewport();
            return viewport;
            }
        }
        public event ViewportResized Resized;
        public event DeviceReset DeviceReset;
        public void AllowEscapeQuit(bool Allow)
        {
            this._game.AllowEscapeQuit = Allow;
        }
        public float AccurateTimeElapsed()
        {
            return (float)_instance._gameTime.TotalGameTime.TotalMilliseconds;
        }

        public long GetTicks()
        {
            return _instance._gameTime.ElapsedGameTime.Ticks;
        }
        
        public void SetDebugFile(string DebugFile)
        {
            _logfile = Path.GetFullPath(DebugFile);
        }
        public void SetDebugMode(bool Enable)
        {
            _instance._debugEnabled = Enable;
            if (Enable)
            {
                if (File.Exists(_instance._logfile))
                    File.Delete(_instance._logfile);
                _instance._logwriter = (TextWriter)new StreamWriter(File.Open(_instance._logfile, FileMode.CreateNew, FileAccess.Write, FileShare.Read));
                
            }
            else
            {
                _instance._logwriter.Close();
            }

        }
	    //public void SetDebugMode(bool Enable, bool WriteToDebugConsole)
        //{
        //   Have to write a console first ;)
        //}

        public RCamera GetCamera()
        {
            return _instance._camera;
        }
        public void SetCamera(RCamera newCamera)
        {
            //_instance._game.cameraComponent.camera = newCamera;
            _instance._camera = newCamera;
            
        }
        public GraphicsDevice GetInternalGraphicsDevice()
        {
            return _game.graphics.GraphicsDevice;
        }
        public void ReleaseAll()
        {
            
            if (_instance != null)
            {

                _instance.AddToLog("Reactor 3D Shutting Down");
                
                _instance._resourceContent.Unload();
                _instance._resourceContent = null;
                _instance._system2d.Dispose();
                _instance._systemfont = null;
                _instance._camera.Dispose();
                _instance.AddToLog("EffectPool cleaned");
                _instance.AddToLog("EOF");
                if (_instance._logwriter != null)
                {
                    _instance._logwriter.Close();
                    _instance._logwriter = null;
                }
                _instance = null;
            }
        }
        
        #endregion
        
    }
            
}


    