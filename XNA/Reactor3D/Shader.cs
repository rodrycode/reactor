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
using System.IO;

namespace Reactor
{
    internal class RShaderManager : IDisposable
    {
        static RShaderManager _instance;
        
        public static RShaderManager Instance
        {
            get { return _instance; }
        }

        internal List<Effect> Effects = new List<Effect>();
        internal ContentManager content;
        public RShaderManager()
        {
            if (_instance == null)
            {
                _instance = this;


                _instance.content = new ContentManager(REngine.Instance._game.Services);
                _instance.content.RootDirectory = _instance.content.RootDirectory+"\\Content";


            }
            
        }

        public void Dispose()
        {
            _instance.content.Dispose();
        }
    }
    public class RShader : IDisposable
    {
        string source;
        string filename;
        
        internal Effect effect;
        public RShader()
        {
            
            source = "";
        }

        public RShader(string Filename)
        {
            
            filename = Filename;
            effect = RShaderManager.Instance.content.Load<Effect>(Filename);
        }

		internal RShader(byte[] byteCode){
			effect = new Effect(REngine.Instance._graphics.GraphicsDevice, byteCode);
		}
		internal static RShader LoadEffectResource(string name)
		{
			#if WINRT
			var assembly = typeof(Effect).GetTypeInfo().Assembly;
			#else
			var assembly = typeof(Effect).Assembly;
			#endif
			var stream = assembly.GetManifestResourceStream(name);
			using (var ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				return new RShader(ms.ToArray());
			}
		}
        public void Load(string Filename)
        {
            

            effect = RShaderManager.Instance.content.Load<Effect>(Filename);
        }

        public bool GetParamBool(string ParamName)
        {
            return effect.Parameters[ParamName].GetValueBoolean();
        }
        public int GetParamInt32(string ParamName)
        {
            return effect.Parameters[ParamName].GetValueInt32();
        }
        public R3DMATRIX GetParamMatrix(string ParamName)
        {
            R3DMATRIX matrix = R3DMATRIX.FromMatrix(effect.Parameters[ParamName].GetValueMatrix());
            return matrix;
        }
        public R3DMATRIX[] GetParamMatrixArray(string ParamName, int ArrayCount)
        {
            Matrix[] marray = effect.Parameters[ParamName].GetValueMatrixArray(ArrayCount);

            R3DMATRIX[] matrix = new R3DMATRIX[marray.Length];

            int index = 0;
            foreach (Matrix m in marray)
            {
                matrix[index] = R3DMATRIX.FromMatrix(m);
            }
            return matrix;
        }
        public RQUATERNION GetParamQuaternion(string ParamName)
        {
            RQUATERNION q = RQUATERNION.FromQuaternion(effect.Parameters[ParamName].GetValueQuaternion());
            return q;
        }
        public Single GetParamSingle(string ParamName)
        {
            return effect.Parameters[ParamName].GetValueSingle();
        }
        public Single[] GetParamSingleArray(string ParamName)
        {
            return effect.Parameters[ParamName].GetValueSingleArray();
        }
        public void SetParam(string ParamName, bool value)
        {
            effect.Parameters[ParamName].SetValue(value);
        }
        
        public void SetParam(string ParamName, float value)
        {
            effect.Parameters[ParamName].SetValue(value);
        }
        public void SetParam(string ParamName, float[] values)
        {
            effect.Parameters[ParamName].SetValue(values);
        }
        public void SetParam(string ParamName, int value)
        {
            effect.Parameters[ParamName].SetValue(value);
        }
        public void SetParam(string ParamName, R3DMATRIX value)
        {
            effect.Parameters[ParamName].SetValue(value.matrix);
            
        }
        public void SetParam(string ParamName, R3DMATRIX[] values)
        {
            Matrix[] m = new Matrix[values.Length];
            int index = 0;
            foreach (R3DMATRIX rm in values)
            {
                m[index] = rm.matrix;
            }
            effect.Parameters[ParamName].SetValue(m);
            m = null;
        }
        public void SetParam(string ParamName, RQUATERNION value)
        {
            effect.Parameters[ParamName].SetValue(value.quaternion);
        }
        
        public void SetParam(string ParamName, RTexture value)
        {

            effect.Parameters[ParamName].SetValue(value._Texture);
        }

        public void SetParam(string ParamName, int value, bool IsTextureID)
        {
            if (!IsTextureID)
                SetParam(ParamName, value);
            else
            {
                effect.Parameters[ParamName].SetValue(RTextureFactory.Instance._textureList[value]);
            }
        }
        public void SetParam(string ParamName, R2DVECTOR value)
        {
            effect.Parameters[ParamName].SetValue(value.vector);
        }
        public void SetParam(string ParamName, R3DVECTOR value)
        {
            effect.Parameters[ParamName].SetValue(value.vector);
        }
        public void SetParam(string ParamName, R4DVECTOR value)
        {
            effect.Parameters[ParamName].SetValue(value.vector);
        }
        public void SetParam(string ParamName, R2DVECTOR[] values)
        {
            Vector2[] q = new Vector2[values.Length];
            int index = 0;
            foreach (R2DVECTOR rm in values)
            {
                q[index] = rm.vector;
            }
            effect.Parameters[ParamName].SetValue(q);
            q = null;
        }
        public void SetParam(string ParamName, R3DVECTOR[] values)
        {
            Vector3[] q = new Vector3[values.Length];
            int index = 0;
            foreach (R3DVECTOR rm in values)
            {
                q[index] = rm.vector;
            }
            effect.Parameters[ParamName].SetValue(q);
            q = null;
        }
        public void SetParam(string ParamName, R4DVECTOR[] values)
        {
            Vector4[] q = new Vector4[values.Length];
            int index = 0;
            foreach (R4DVECTOR rm in values)
            {
                q[index] = rm.vector;
            }
            effect.Parameters[ParamName].SetValue(q);
            q = null;
        }

        public void SetTechnique(string TechniqueName)
        {
            effect.CurrentTechnique = effect.Techniques[TechniqueName];
        }

        internal bool ParamExists(string ParamName)
        {
            try
            {
                EffectParameter param = effect.Parameters[ParamName];
                return true;
            }
            catch
            {
                return false;
            }
            return false;
        }
        internal bool SamanticExists(string ParamName)
        {
            try
            {
                EffectParameter param = GetParameterBySemantic(ParamName);
                return true;
            }
            catch
            {
                return false;
            }
            return false;
        }
        public void Dispose()
        {
            
                source = null;
                filename = null;
            
            if (effect != null)
            {
                effect.Dispose();
                effect = null;
            }
        }

        internal EffectParameter GetParameterBySemantic(String ParamName)
        {
            foreach(EffectParameter p in effect.Parameters){
                if (p.Semantic == ParamName)
                {
                    return p;
                }
            }
            return null;
        }
    }
}