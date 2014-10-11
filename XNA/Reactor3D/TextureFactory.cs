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
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
namespace Reactor
{
    public class RTexture : IDisposable
    {
        internal int Width = 0;
        internal int Height = 0;
        internal int LevelCount = 0;
        internal string Name = "NONAME";
        internal string Filename = "NONE";
        internal Texture _Texture;

        internal CONST_RTEXTURE_TYPE iType;
        public int GetWidth()
        {
            return Width;
        }
        public int GetHeight()
        {
            return Height;
        }
        public void SetName(string name)
        {
            Name = name;
            _Texture.Name = name;
        }
        public string GetName()
        {
            return Name;
        }
        internal void BuildInfo()
        {
            if (iType == CONST_RTEXTURE_TYPE.Texture2D)
            {
                Texture2D t = (Texture2D)_Texture;
                Height = t.Height;
                Width = t.Width;
                LevelCount = t.LevelCount;
            }
            else if (iType == CONST_RTEXTURE_TYPE.Texture3D)
            {
                Texture3D t = (Texture3D)_Texture;
                Height = t.Height;
                Width = t.Width;
                LevelCount = t.LevelCount;
            }
            else
            {
                TextureCube t = (TextureCube)_Texture;
                LevelCount = t.LevelCount;
                Height = -1;
                Width = -1;
            }
            

        }
        public Color[] GetData()
        {
            if (iType == CONST_RTEXTURE_TYPE.Texture2D)
            {
                Texture2D t = (Texture2D)_Texture;
                Color[] color = new Color[t.Width * t.Height];
                t.GetData<Color>(color);
                return color;
            }
            else if (iType == CONST_RTEXTURE_TYPE.Texture3D)
            {
                Texture3D t = (Texture3D)_Texture;
                Color[] color = new Color[t.Width * t.Height];
                t.GetData<Color>(color);
                return color;
            }
            else
            {
                REngine.Instance.AddToLog("TextureCubes do not support GetData Functions : " + Name);
                return null;
            }
            
        }
        public void SetData(Color[] tColorArray)
        {
            if (iType == CONST_RTEXTURE_TYPE.Texture2D)
            {
                Texture2D t = (Texture2D)_Texture;
                t.SetData<Color>(tColorArray);
                _Texture = t;
            }
            else if (iType == CONST_RTEXTURE_TYPE.Texture3D)
            {
                Texture3D t = (Texture3D)_Texture;
                t.SetData<Color>(tColorArray);
                _Texture = t;
            }
            else
            {
                REngine.Instance.AddToLog("TextureCubes do not support SetData Functions : "+Name);
            }
        }
        public void Dispose()
        {
            if (_Texture != null)
            {
                _Texture.Dispose();
            }
        }
    }
    public class RTextureFactory
    {
        #region Static Instances
		static RTextureFactory _instance = new RTextureFactory();
        public static RTextureFactory Instance
        {
            get { return _instance; }
        }
        #endregion

        #region Internal Members
        internal Hashtable _textureTable = new Hashtable();
        internal List<Texture> _textureList = new List<Texture>();
        #endregion

        #region Public Methods
        public RTextureFactory()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                return;
            }
        }
        public void Destroy(int Index)
        {
            if (_instance._textureList[Index] != null)
            {
                _instance._textureList[Index].Dispose();
            }
        }
        public void DestroyAll()
        {
            if (_instance != null)
            {
                _instance = null;
            }
            else
            {
                REngine.Instance.AddToLog("RTextureFactory cannot be destroyed, cannot find instance!");
            }
        }
        public int GetTexture(string Name)
        {
            int t = (int)_instance._textureTable[Name];
            
            return t;
        }
        public int CreateTexture(string Name, int Width, int Height, int Num3DLevels, CONST_RTEXTURE_TYPE iType)
        {
            if (iType == CONST_RTEXTURE_TYPE.Texture2D)
            {
                Texture2D t = new Texture2D(REngine.Instance._graphics.GraphicsDevice, Width, Height);
                t.Tag = Name;
                t.Name = Name;
                _instance._textureList.Add(t);
                int index = _instance._textureList.IndexOf(t);
                _instance._textureTable.Add(Name, index);

                return index;
            }
            else if (iType == CONST_RTEXTURE_TYPE.Texture3D)
            {
                Texture3D t = new Texture3D(REngine.Instance._graphics.GraphicsDevice, Width, Height, Num3DLevels, true, SurfaceFormat.Color);
                t.Tag = Name;
                t.Name = Name;
                _instance._textureList.Add(t);
                int index = _instance._textureList.IndexOf(t);
                _instance._textureTable.Add(Name, index);
                return index;
            }
            else if (iType == CONST_RTEXTURE_TYPE.TextureCube)
            {
                TextureCube t = new TextureCube(REngine.Instance._graphics.GraphicsDevice, Width*Height, true, SurfaceFormat.Color);
                t.Tag = Name;
                t.Name = Name;
                _instance._textureList.Add(t);
                int index = _instance._textureList.IndexOf(t);
                _instance._textureTable.Add(Name, index);
                return index;
            }
            return -1;
        }
        public int LoadTexture(string Filename, string Name, CONST_RTEXTURE_TYPE iType)
        {
            
            if (iType == CONST_RTEXTURE_TYPE.Texture2D)
            {
                Texture2D t = REngine.Instance._content.Load<Texture2D>(Filename);
                t.Tag = Filename;
                t.Name = Name;
                _instance._textureList.Add(t);
                int index = _instance._textureList.IndexOf(t);
                _instance._textureTable.Add(Name, index);
                
                return index;
            }
            else if (iType == CONST_RTEXTURE_TYPE.Texture3D)
            {
				Texture3D t = REngine.Instance._content.Load<Texture3D>(Filename);
                t.Tag = Filename;
                t.Name = Name;
                _instance._textureList.Add(t);
                int index = _instance._textureList.IndexOf(t);
                _instance._textureTable.Add(Name, index);
                return index;
            }
            else if (iType == CONST_RTEXTURE_TYPE.TextureCube)
            {
				TextureCube t = REngine.Instance._content.Load<TextureCube>(Filename);
                t.Tag = Filename;
                t.Name = Name;
                _instance._textureList.Add(t);
                int index = _instance._textureList.IndexOf(t);
                _instance._textureTable.Add(Name, index);
                return index;
            }
            return 0;
        }

        public void SetPixel2D(int TextureID, int X, int Y, R4DVECTOR Color)
        {
            Texture2D tex = (Texture2D)_instance._textureList[TextureID];
            Color[] color = new Color[tex.Width*tex.Height];
            tex.GetData<Color>(color);
            color[(Y*(tex.Width))+X] = new Color(Color.vector);
            tex.SetData<Color>(color);
            _instance._textureList[TextureID] = tex;
        }
        public void SetTexture(RTexture texture, int TextureID)
        {
            _instance._textureList[TextureID] = (Texture)texture._Texture;
        }
        public RTexture GetTexture(int TextureID)
        {
            
            RTexture texture = new RTexture();
            texture._Texture = (Texture)_instance._textureList[TextureID];
			texture.Filename = REngine.Instance._content.RootDirectory+"\\"+(string)texture._Texture.Tag+".xnb";
            

            return texture;
            
        }
#if XBOX
        public int GetGamerProfileTextureID(int PlayerIndex)
        {
            try
            {
                foreach (SignedInGamer p in Gamer.SignedInGamers)
                {
                    
                    
                    if ((int)p.PlayerIndex == PlayerIndex)
                    {
                        
                        GamerProfile profile = Gamer.SignedInGamers[p.PlayerIndex].GetProfile();
                        if (!_instance._textureList.Contains(profile.GamerPicture))
                        {
                            _instance._textureList.Add(profile.GamerPicture);
                        }
                        return _instance._textureList.IndexOf(profile.GamerPicture);
                    }
                    else
                        continue;
                }
                return -1;
                
            }
            catch
            {
                return -2;
            }
        }
#endif
        public int BuildCubeMap(int Size, string Name, int PositiveXID, int PositiveYID, int PositiveZID, int NegativeXID, int NegativeYID, int NegativeZID, RSURFACEFORMAT Format)
        {
            Texture2D posX = ((Texture2D)_instance._textureList[PositiveXID]);
            Texture2D posY = ((Texture2D)_instance._textureList[PositiveYID]);
            Texture2D posZ = ((Texture2D)_instance._textureList[NegativeZID]);
            Texture2D negX = ((Texture2D)_instance._textureList[NegativeXID]);
            Texture2D negY = ((Texture2D)_instance._textureList[NegativeYID]);
            Texture2D negZ = ((Texture2D)_instance._textureList[PositiveZID]);
            
            TextureCube cube = new TextureCube(REngine.Instance._graphics.GraphicsDevice, Size, true, (SurfaceFormat)Format);
            Color[] data0 = new Color[(Size * Size) * 1];
            Color[] data1 = new Color[(Size * Size) * 1];
            Color[] data2 = new Color[(Size * Size) * 1];
            Color[] data3 = new Color[(Size * Size) * 1];
            Color[] data4 = new Color[(Size * Size) * 1];
            Color[] data5 = new Color[(Size * Size) * 1];
            negX.GetData<Color>(data0);
            cube.SetData<Color>(CubeMapFace.NegativeX, data0);
            negY.GetData<Color>(data1);
            cube.SetData<Color>(CubeMapFace.NegativeY, data1);
            negZ.GetData<Color>(data2);
            cube.SetData<Color>(CubeMapFace.NegativeZ, data2);
            posX.GetData<Color>(data3);
            cube.SetData<Color>(CubeMapFace.PositiveX, data3);
            posY.GetData<Color>(data4);
            cube.SetData<Color>(CubeMapFace.PositiveY, data4);
            posZ.GetData<Color>(data5);
            cube.SetData<Color>(CubeMapFace.PositiveZ, data5);

            _instance._textureList.Add(cube);
            int index = _instance._textureList.LastIndexOf(cube);
            _instance._textureTable.Add(Name, index);
            return index;
        }
        #endregion
    }
}