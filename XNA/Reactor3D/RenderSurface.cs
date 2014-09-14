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
    public class RRenderTarget2D : IDisposable
    {
        string Name;
        internal RenderTarget2D target;
        
        SurfaceFormat format;
        int width;
        int height;
        int index;
        //int texindex;
        int levels;
        internal void CreateRenderTarget(string Name, int Height, int Width,int Levels, SurfaceFormat format)
        {
            this.Name = Name;
            this.width = Width;
            this.height = Height;
            this.format = format;
            this.levels = Levels;
            target = new RenderTarget2D(REngine.Instance._game.GraphicsDevice, Width, Height, true, format, DepthFormat.Depth24Stencil8,Levels, RenderTargetUsage.PlatformContents);
            REngine.Instance._game.GraphicsDevice.SetRenderTarget(null);
            //RTextureFactory.Instance._textureList.Add(target.GetTexture());
            //int tid = RTextureFactory.Instance._textureList.Count - 1;
            target.Disposing += new EventHandler<System.EventArgs>(target_Disposing);
            target.ContentLost += new EventHandler<System.EventArgs>(target_ContentLost);
            //RTextureFactory.Instance._textureTable.Add(Name, tid);
            
            //texindex = RTextureFactory.Instance._textureList.LastIndexOf(target.GetTexture());
        }
        
        void target_Disposing(object sender, EventArgs e)
        {
            //int i = RTextureFactory.Instance._textureList.LastIndexOf(target.GetTexture());
            //RTextureFactory.Instance._textureList[texindex].Dispose();
            //RTextureFactory.Instance._textureTable.Remove(Name);
        }

        void target_ContentLost(object sender, EventArgs e)
        {
            target = new RenderTarget2D(REngine.Instance._game.GraphicsDevice, width, height, true, format, DepthFormat.Depth24Stencil8, levels, RenderTargetUsage.PlatformContents);
        }
        internal int Index
        {
            get { return index; }
            set { index = value; }
        }
        
        public void Start()
        {
            REngine.Instance._graphics.GraphicsDevice.SetRenderTarget(target);
        }
        public void End()
        {
            
            REngine.Instance._graphics.GraphicsDevice.SetRenderTarget(null);
            //RTextureFactory.Instance._textureList[texindex].Dispose();
            //RTextureFactory.Instance._textureList[texindex] = target.GetTexture();
        }
        public void Dispose()
        {
            target.Dispose();
        }
    }
}
