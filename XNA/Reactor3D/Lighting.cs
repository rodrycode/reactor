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
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Resources;
#if !XBOX
using System.Runtime.Serialization.Formatters.Binary;
#endif
#endregion

namespace Reactor
{
    public class RMaterial
    {
        internal int ID;
        public int GetID() { return ID; }
        internal RShader shader;
        internal string name;
        internal RSceneNode node;
        
        public void SetShader(RShader Shader)
        {
            
            shader.effect = Shader.effect;
            
        }
        public void GetShader(ref RShader Shader)
        {
            Shader = shader;
        }
        public RShader GetShader()
        {
            return shader;
        }
        public RMaterial Clone()
        {
            int num = 1;
            for(int i=0; i<RMaterialFactory.Instance._MaterialList.Count; i++)
                if (RMaterialFactory.Instance._MaterialList.ContainsKey(this.name+num))
                {
                    num++;
                }
            RMaterial mat = new RMaterial();
            RShader sh = new RShader();
            sh.effect = shader.effect.Clone(REngine.Instance._graphics.GraphicsDevice);
            mat.name = this.name+num;
            mat.shader = sh;
            mat.ambColor = ambColor;
            mat.diffColor = diffColor;
            mat.ID = RMaterialFactory._instance._MaterialList.Count + 1;
            mat.specColor = specColor;
            mat.specPower = specPower;
            RMaterialFactory._instance._MaterialList.Add(mat.name, mat);
            return mat;
        }
        internal Color diffColor;
        public void SetDiffuseColor(float r, float g, float b, float a)
        {
            diffColor = new Color(new Vector4(r, g, b, a));
        }
        public R4DVECTOR GetDiffuseColor()
        {
            R4DVECTOR vector = new R4DVECTOR(diffColor.R, diffColor.G, diffColor.B, diffColor.A);
            return vector;
        }
        internal Color ambColor;
        public void SetAmbientColor(float r, float g, float b, float a)
        {
            ambColor = new Color(new Vector4(r, g, b, a));
        }
        public R4DVECTOR GetAmbientColor()
        {
            R4DVECTOR vector = new R4DVECTOR(ambColor.R, ambColor.G, ambColor.B, ambColor.A);
            return vector;
        }

        internal Color specColor;
        public void SetSpecularColor(float r, float g, float b, float a)
        {
            specColor = new Color(new Vector4(r, g, b, a));
        }
        public R4DVECTOR GetSpecularColor()
        {
            R4DVECTOR vector = new R4DVECTOR(specColor.R, specColor.G, specColor.B, specColor.A);
            return vector;
        }
        internal float specPower;
        public void SetSpecularPower(float power)
        {
            specPower = power;
        }
        public float GetSpecularPower()
        {
            return specPower;
        }
        public void SetTexture(int TextureID)
        {
            Texture texture = RTextureFactory.Instance._textureList[TextureID];
            EffectParameter param = shader.effect.Parameters.GetParameterBySemantic("texture0");
            param.SetValue(texture);
            
        }
        public void SetTexture(int TextureID, CONST_REACTOR_TEXTURELAYER TextureLayer)
        {
            Texture texture = RTextureFactory.Instance._textureList[TextureID];
            EffectParameter param = shader.effect.Parameters.GetParameterBySemantic("texture" + ((int)TextureLayer).ToString());
            param.SetValue(texture);
        }
        internal void Prepare(RSceneNode node)
        {
            EffectParameter param;
            try
            {
                param = shader.effect.Parameters.GetParameterBySemantic("View");
                param.SetValue(REngine.Instance._camera.viewMatrix);
            }
            catch { }
            
            try
            {
                param = shader.effect.Parameters.GetParameterBySemantic("ViewProjection");
                param.SetValue(REngine.Instance._camera.ViewProjectionMatrix.matrix);
            }
            catch { }
            
            try
            {
                param = shader.effect.Parameters.GetParameterBySemantic("World");
                param.SetValue(node.Matrix);
            }
            catch { }
            
            try
            {
                param = shader.effect.Parameters.GetParameterBySemantic("WorldViewProjection");
                param.SetValue(node.Matrix * REngine.Instance._camera.ViewProjectionMatrix.matrix);
            }
            catch { }

            try
            {
                
                param = shader.effect.Parameters.GetParameterBySemantic("RLight");
                if (node != null)
                {
                    //param.StructureMembers;
                    //param.Elements["Rlights"].SetValue(RLightingFactory.Instance.GetClosestActiveLights(node.Position));
                }
                else
                {
                    //param.SetValue(RLightingFactory.Instance.GetClosestActiveLights(REngine.Instance._camera.Position.vector));
                }
            }
            catch { }

        }
        internal void Dispose()
        {
            shader.Dispose();
        }
        
    }

    public class RMaterialFactory : IDisposable
    {
        internal static RMaterialFactory _instance;
        public static RMaterialFactory Instance
        {
            get { return _instance; }
        }

        internal Hashtable _MaterialList = new Hashtable();
        public RMaterialFactory()
        {
            RShaderManager manager = new RShaderManager();
            if (_instance == null)
                _instance = this;
        }
        public RMaterial CreateMaterial(string Name)
        {
            if (!_instance._MaterialList.ContainsKey(Name))
            {
                RMaterial material = new RMaterial();
                material.name = Name;
                material.ambColor = new Color(0, 0, 0);
                material.diffColor = Color.White;
                material.specColor = Color.White;
                material.specPower = 100;
                material.ID = _instance._MaterialList.Count;
                material.shader = new RShader();
                material.shader.effect = new BasicEffect(REngine.Instance._graphics.GraphicsDevice, REngine.Instance._effectPool);
                _instance._MaterialList.Add(Name, material);
                return material;
            }
            else
            {
                REngine.Instance.AddToLog("RMaterialFactory tried to create "+Name+" Material when its name was already taken!");
                return null;
            }
        }
#if !XBOX
        public RMaterial LoadMaterial(string filename)
        {
            StreamReader reader = new System.IO.StreamReader(REngine.Instance._game.Content.RootDirectory + filename);
            BinaryFormatter formatter = new BinaryFormatter();
            RMaterial material = (RMaterial)formatter.Deserialize(reader.BaseStream);
            reader.Close();
            return material;
        }
        public void SaveMaterial(RMaterial material, string filename)
        {
            StreamWriter writer = new System.IO.StreamWriter(REngine.Instance._game.Content.RootDirectory + filename);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writer.BaseStream, material);
            writer.Close();
        }
#endif
        public bool DeleteMaterial(RMaterial material)
        {
            if (_instance._MaterialList.Contains(material.name))
            {
                _instance._MaterialList.Remove(material.name);
                material = null;
                return true;
            }
            else
            {
                return false;
            }
            return false;

        }
        public void Dispose()
        {
            if (_instance != null)
            {
                _instance._MaterialList = null;
                _instance = null;
                
            }
            else
            {
                REngine.Instance.AddToLog("RMaterialFactory cannot be destroyed, cannot find instance!");
            }
        }
        
    }

    public class RLightingFactory : IDisposable
    {
        internal static RLightingFactory _instance;
        internal static RLightingFactory Instance
        {
            get { return _instance; }
        }

        internal Hashtable _LightList = new Hashtable();
        public RLightingFactory()
        {
            if (_instance == null)
                _instance = this;
        }
        public void Dispose()
        {
            _LightList.Clear();

        }
        internal void UpdateLights()
        {
            foreach (RLIGHT light in _LightList.Values)
            {
                
            }
        }
        internal RLIGHT[] GetClosestActiveLights(Vector3 Position)
        {
            BoundingSphere posSphere = new BoundingSphere(Position, 1.0f);
            SortedList<float,RLIGHT> lights = new SortedList<float,RLIGHT>(32);
            Hashtable distances = new Hashtable(32);
            foreach (RLIGHT light in _LightList.Values)
            {
                
                float Distance = Vector3.Distance(light.Position.vector, Position);
                if (light.Enabled)
                {
                    switch (light.LightType)
                    {
                        case 0:
                            lights.Add(Distance, light);
                            break;
                        case 1:
                            
                            if(lights.Count >= 32)
                            {
                                int i = 0;
                                foreach(float d in lights.Keys)
                                {
                                    if(lights[d].LightType != 0)
                                    {
                                        if(Distance < d)
                                        {
                                            lights.RemoveAt(i);
                                            break;
                                        }
                                    }
                                    i++;
                                }
                            }
                            BoundingSphere sphere = new BoundingSphere(light.Position.vector, light.Radius);
                            if (sphere.Intersects(posSphere))
                            {
                                lights.Add(Distance, light);
                            }
                            break;
                        case 2:
                            lights.Add(Distance, light);
                            break;
                        default:
                            break;
                    }
                }
                
            }
            lights.TrimExcess();
            List<RLIGHT> l = new List<RLIGHT>(lights.Values);
            
            return l.ToArray();
        }
    }
}