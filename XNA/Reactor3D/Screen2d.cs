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

    public class RScreen2D
    {
        #region Static
        static RScreen2D _instance;
        public static RScreen2D Instance
        {
            get { return _instance; }
        }
        #endregion

        #region Internal Members
        internal SpriteBatch _spritebatch;
        internal BasicEffect _basicEffect;
        internal VertexDeclaration _lineDeclaration;
        internal Texture2D _lineTexture;

        #endregion

        #region Public Methods
        public RScreen2D()
        {
            if (_instance == null)
            {
                _instance = this;
                _instance._spritebatch = new SpriteBatch(REngine.Instance._graphics.GraphicsDevice);
                _instance._basicEffect = new BasicEffect(REngine.Instance._graphics.GraphicsDevice);
                _instance._lineDeclaration = VertexPositionColor.VertexDeclaration;
                _instance._lineTexture = new Texture2D(REngine.Instance._graphics.GraphicsDevice, 1, 1);
                _instance._lineTexture.SetData(new Color[] { Color.White });
            }
            else
            {
            }
        }
        
        public void Destroy()
        {
            _instance._spritebatch.Dispose();
        }
        public void Action_Begin2D()
        {
            _instance._spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        }
        public void Action_End2D()
        {
            _instance._spritebatch.End();
        }
        internal static void IAction_Begin2D()
        {

            REngine.Instance._graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            
            if (RAtmosphere.Instance != null)
                if (RAtmosphere.Instance.fogEnabled)
                    REngine.Instance._graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            
                //_instance._spritebatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);
        }
        internal void Update()
        {
            IAction_Begin2D();
            REngine.Instance._game.game.Render2D();
            REngine.Instance.DrawFps();
            REngine.Instance._game.RenderWatermark();
            IAction_End2D();
        }
        
        internal static void IAction_End2D()
        {
            
        }
        
        public RFONT Create_TextureFont(string FontName, string FileName)
        {
            RFONT font = new RFONT();
            font._name = FontName;
            font._spriteFont = REngine.Instance._content.Load<SpriteFont>(FileName);
            return font;
        }
        public void Draw_TextureFont(RFONT font, int X, int Y, string Message)
        {
            Draw_TextureFont(font, X, Y, Message, new R4DVECTOR(1f, 1f, 1f, 1f));
        }
        public void Draw_Rect2D(int X, int Y, int Width, int Height, R4DVECTOR color)
        {
            Color c = new Color(color.vector);


            try
            {
                _instance._spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _instance._spritebatch.Draw(_instance._lineTexture, new Rectangle(X, Y, Width, Height), c);
                _instance._spritebatch.End();

            }
            catch { }
        }
        public void Draw_Line3D(R3DVECTOR start, R3DVECTOR end, R4DVECTOR color)
        {
            VertexPositionColor[] v = new VertexPositionColor[3];
            v[0] = new VertexPositionColor(start.vector, new Color(color.vector));
            v[1] = new VertexPositionColor(end.vector, new Color(color.vector));
            
           
            _instance._basicEffect.View = REngine.Instance._camera.viewMatrix;
            _instance._basicEffect.Projection = REngine.Instance._camera.projMatrix;
            _instance._basicEffect.World = Matrix.Identity;
            _instance._basicEffect.VertexColorEnabled = true;

            
 
            
            _instance._basicEffect.CurrentTechnique.Passes[0].Apply();

            // Draw the triangle.

            REngine.Instance._graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                              v, 0, 1);


        }
        public void Draw_TextureFont(RFONT font, int X, int Y, string Message, R4DVECTOR color)
        {
            Color c = new Color(color.vector);
            
            
            try
            {
                _instance._spritebatch.Begin();
                _instance._spritebatch.DrawString(font._spriteFont, Message, new Vector2(X, Y), c);
                _instance._spritebatch.End();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                REngine.Instance.AddToLog(e.ToString());
            }
            

        }
        public void Draw_TextureFont(RFONT font, int X, int Y, string Message, R4DVECTOR color, float Rotation, R2DVECTOR RotationOrigin, R2DVECTOR Scale)
        {
            Color c = new Color(color.vector);


            try
            {
                _instance._spritebatch.Begin();
                _instance._spritebatch.DrawString(font._spriteFont, Message, new Vector2(X, Y), c, Rotation, RotationOrigin.vector, Scale.vector, SpriteEffects.None, 0);
                _instance._spritebatch.End();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                REngine.Instance.AddToLog(e.ToString());
            }


        }
        public R3DVECTOR Unproject(R3DVECTOR Position, RSceneNode Node)
        {
            Vector3 v = REngine.Instance.GetViewport().Unproject(Position.vector, REngine.Instance._camera.projMatrix, REngine.Instance._camera.viewMatrix, Matrix.Identity);
            return R3DVECTOR.FromVector3(v);
        }
        public R3DVECTOR Project(R3DVECTOR Position, RSceneNode Node)
        {
            Vector3 v = REngine.Instance.GetViewport().Project(Position.vector, REngine.Instance._camera.projMatrix, REngine.Instance._camera.viewMatrix, Matrix.Identity);
            return R3DVECTOR.FromVector3(v);
        }
        public void Draw_Texture2D(int TextureID, int X, int Y, int Width, int Height, int scaleX, int scaleY)
        {
            
            Texture2D tex = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
            Rectangle rect = new Rectangle(X,Y,Width * scaleX,Height * scaleY);
            _instance._spritebatch.Begin();
            _instance._spritebatch.Draw(tex, rect, Color.White);
            _instance._spritebatch.End();
            
        }
        public void Draw_Texture2D(int TextureID, int X, int Y, int Width, int Height, int scaleX, int scaleY, R4DVECTOR color)
        {
            Color c = new Color(color.vector);
            Texture2D tex = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            _instance._spritebatch.Begin();
            _instance._spritebatch.Draw(tex, rect, c);
            _instance._spritebatch.End();

        }
        public void Draw_Texture2D(int TextureID, int X, int Y, int Width, int Height, int SourceX, int SourceY, int SourceWidth, int SourceHeight, int scaleX, int scaleY, R4DVECTOR color)
        {
            Color c = new Color(color.vector);
            Texture2D tex = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            Rectangle sourcerect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);

            _instance._spritebatch.Begin();
            _instance._spritebatch.Draw(tex, rect, sourcerect, c);
            _instance._spritebatch.End();


        }
        public void Draw_Texture2D(int TextureID, int X, int Y, int Width, int Height, int SourceX, int SourceY, int SourceWidth, int SourceHeight, int scaleX, int scaleY, R4DVECTOR color, float Rotation)
        {
            Color c = new Color(color.vector);
            Texture2D tex = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            Rectangle sourcerect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);

            _instance._spritebatch.Begin();
            _instance._spritebatch.Draw(tex, rect, sourcerect, c, Rotation, new Vector2(SourceWidth/2, SourceHeight/2), SpriteEffects.None, 1.0f);
            _instance._spritebatch.End();

        }
        public void Draw_Texture2D(int TextureID, int X, int Y, int Width, int Height, int SourceX, int SourceY, int SourceWidth, int SourceHeight, int scaleX, int scaleY, R4DVECTOR color, float Rotation, bool FlipHorizontal)
        {
            Color c = new Color(color.vector);
            Texture2D tex = (Texture2D)RTextureFactory.Instance._textureList[TextureID];
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            Rectangle sourcerect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);
            SpriteEffects effects = SpriteEffects.None;
            if (FlipHorizontal)
                effects = SpriteEffects.FlipHorizontally;
            _instance._spritebatch.Begin();
            _instance._spritebatch.Draw(tex, rect, sourcerect, c, Rotation, new Vector2((rect.Width / 2), (rect.Height / 2)), effects, 1.0f);
            _instance._spritebatch.End();

        }
        
        #endregion
    }
}