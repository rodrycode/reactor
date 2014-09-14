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
    
    public class RScene
    {
        #region Static Instances
        static RScene _instance;
        public static RScene Instance
        {
            get { return _instance; }
        }
        public RSceneNode CloneNode(RSceneNode node, string name)
        {
            if (node.GetType() == typeof(RActor))
            {
                RActor actor = Instance.CreateActor(name);
                actor._model = ((RActor)node)._model;
                actor._content = ((RActor)node)._content;
                actor._player = ((RActor)node)._player;
                actor._position = node.Position;
                actor._rotation = node.Rotation;
                actor._skinningData = ((RActor)node)._skinningData;
                actor._transforms = ((RActor)node)._transforms;
                actor._objectMatrix = ((RActor)node)._objectMatrix;
                actor._defaultEffect = ((RActor)node)._defaultEffect;
                actor._material = ((RActor)node)._material;
                actor._materials = ((RActor)node)._materials;
                
                actor.scaling = ((RActor)node).scaling;
                actor.ParentNode = ((RActor)node).ParentNode;
                return actor;
            }
            else if (node.GetType() == typeof(RMesh))
            {
                RMesh mesh = Instance.CreateMesh(name);
                mesh._basicEffect = ((RMesh)node)._basicEffect;
                mesh._boundingBox = ((RMesh)node)._boundingBox;
                mesh._content = ((RMesh)node)._content;
                mesh._material = ((RMesh)node)._material;
                mesh._materials = ((RMesh)node)._materials;
                mesh._model = ((RMesh)node)._model;
                mesh._name = ((RMesh)node)._name;
                mesh._objectMatrix = ((RMesh)node)._objectMatrix;
                mesh._position = ((RMesh)node)._position;
                mesh._rotation = ((RMesh)node)._rotation;
                mesh._transforms = ((RMesh)node)._transforms;
                
                mesh.scaling = ((RMesh)node).scaling;
                mesh.ParentNode = node.ParentNode;
                return mesh;
            }
            else if (node.GetType() == typeof(RMeshBuilder))
            {
                RMeshBuilder mesh = Instance.CreateMeshBuilder(name);
                mesh._name = name;
                mesh.Matrix = node.Matrix;
                mesh.Position = node.Position;
                mesh.Quaternion = node.Quaternion;
                mesh.Rotation = node.Rotation;
                return mesh;
            }
            else if (node.GetType() == typeof(RParticleEmitter))
            {
                RParticleEmitter particles = Instance.CreateParticleEmitter(((RParticleEmitter)node)._type, name);
                particles._emitter = ((RParticleEmitter)node)._emitter;
                particles._emitters = ((RParticleEmitter)node)._emitters;
                particles._psystem = ((RParticleEmitter)node)._psystem;
                particles._type = ((RParticleEmitter)node)._type;
                particles.Position = node.Position;
                particles.Quaternion = node.Quaternion;
                particles.Matrix = node.Matrix;
                particles.Rotation = node.Rotation;
                particles.TextureID = ((RParticleEmitter)node).TextureID;
                particles.settings = ((RParticleEmitter)node).settings;
                particles.ParentNode = node.ParentNode;
                return particles;
            }
            else if (node.GetType() == typeof(GrassBillboard))
            {
                GrassBillboard g = new GrassBillboard();
                g.Position = node.Position;
                g.Rotation = node.Rotation;
                g.Quaternion = node.Quaternion;
                g.normal = ((GrassBillboard)node).normal;
                g.buffer = ((GrassBillboard)node).buffer;
                g.indices = ((GrassBillboard)node).indices;
                g.maxDistance = ((GrassBillboard)node).maxDistance;
                g.Matrix = node.Matrix;
                g._name = name;
                g.height = ((GrassBillboard)node).height;
                g.width = ((GrassBillboard)node).width;
                g.texture = ((GrassBillboard)node).texture;
                g.type = ((GrassBillboard)node).type;
                g.vegEffect = ((GrassBillboard)node).vegEffect;
                return g;
            }
            else
            {
                throw new Exception("SceneNode Not Supported by Clone : " + node.GetType().ToString());
            }
            
        }
        #endregion

        #region Internal Members
        internal Hashtable _MeshBuilderList = new Hashtable();
        internal Hashtable _MeshList = new Hashtable();
        internal Hashtable _ActorList = new Hashtable();
        internal Hashtable _ParticleEmitterList = new Hashtable();
        internal Hashtable _LandscapeList = new Hashtable();
        internal Hashtable _WaterList = new Hashtable();
        internal Hashtable _RenderTargets = new Hashtable();
        internal List<RSceneNode> _NodeList = new List<RSceneNode>();
        #endregion

        #region Public Methods
        public RScene()
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
        public void Destroy()
        {
            if (_instance != null)
            {
                _instance = null;
                
            }
            else
            {
                REngine.Instance.AddToLog("RScene cannot be destroyed, cannot find instance!");
            }
        }
        public void AdvancedCollision()
        {
            
        }
        public RRenderTarget2D CreateRenderTarget2D(string Name, int Width, int Height, int Levels, RSURFACEFORMAT Format)
        {
            RRenderTarget2D target = new RRenderTarget2D();
            target.CreateRenderTarget(Name, Height, Width, Levels, (SurfaceFormat)Format);
            target.Index = _instance._RenderTargets.Count + 1;
            _instance._RenderTargets.Add(Name, target);
            return target;
        }
        public RMesh CreateMesh(string name)
        {
            RMesh mesh = new RMesh();
            mesh.CreateMesh(name);
            _instance._MeshList.Add(name, mesh);
            _instance._NodeList.Add(mesh);
            REngine.Instance.AddToLog("RMesh: " + name + " Created by RScene");
            return mesh;
        }
        public RMeshBuilder CreateMeshBuilder(string name)
        {
            RMeshBuilder mesh = new RMeshBuilder();
            mesh.CreateMesh(name);
            _instance._MeshBuilderList.Add(name, mesh);
            _instance._NodeList.Add(mesh);
            REngine.Instance.AddToLog("RMeshBuilder: " + name + " Created by RScene");
            return mesh;
        }

        public RActor CreateActor(string name)
        {
            RActor actor = new RActor();
            actor.CreateActor(name);
            _instance._ActorList.Add(name, actor);
            _instance._NodeList.Add(actor);
            REngine.Instance.AddToLog("RActor: " + name + " Created by RScene");
            return actor;
        }
        public RLandscape CreateLandscape(string name)
        {
            RLandscape landscape = new RLandscape();
            landscape.CreateLandscape();
            landscape._name = name;
            _instance._LandscapeList.Add(name, landscape);
            _instance._NodeList.Add(landscape);
            REngine.Instance.AddToLog("RLandscape: " + name + " Created by RScene");
            return landscape;
        }
        public RParticleEmitter CreateParticleEmitter(CONST_REACTOR_PARTICLETYPE type, string name)
        {
            RParticleEmitter emitter = new RParticleEmitter();
            emitter.CreateEmitter(type);
            emitter._name = name;
            _instance._ParticleEmitterList.Add(name, emitter);
            _instance._NodeList.Add(emitter);
            REngine.Instance.AddToLog("RParticleEmitter: " + name + " Created by RScene");
            return emitter;
        }
        public RWater CreateWater(string name)
        {
            RWater water = new RWater();
            water.CreateWater();
            water._name = name;
            _instance._WaterList.Add(name, water);
            _instance._NodeList.Add(water);
            REngine.Instance.AddToLog("RWater: " + name + " Created by RScene");
            return water;
        }
        public RSceneNode CreateNode<T>(string name)
        {
            if (typeof(T) == typeof(RActor))
            {
                RActor node = Instance.CreateActor(name);
                return (RSceneNode)node;
            }
            else if (typeof(T) == typeof(RMesh))
            {
                RMesh node = Instance.CreateMesh(name);
                return (RSceneNode)node;
            }
            else if (typeof(T) == typeof(RMeshBuilder))
            {
                RMeshBuilder node = Instance.CreateMeshBuilder(name);
                return (RSceneNode)node;
            }
            else if (typeof(T) == typeof(RLandscape))
            {
                RLandscape node = Instance.CreateLandscape(name);
                return (RSceneNode)node;
            }
            else if (typeof(T) == typeof(RParticleEmitter))
            {
                RParticleEmitter node = Instance.CreateParticleEmitter(CONST_REACTOR_PARTICLETYPE.Point, name);
                return (RSceneNode)node;
            }
            else if (typeof(T) == typeof(RWater))
            {
                RWater node = Instance.CreateWater(name);
                return (RWater)node;
            }
            else
                return null;
            return null;
        }
        public void SetShadeMode(CONST_REACTOR_FILLMODE ShadeMode)
        {
            if (ShadeMode == CONST_REACTOR_FILLMODE.Wireframe)
                REngine.Instance._graphics.GraphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;
            if (ShadeMode == CONST_REACTOR_FILLMODE.Solid)
                REngine.Instance._graphics.GraphicsDevice.RasterizerState.FillMode = FillMode.Solid;

        }
        public void DestroyAll()
        {
            foreach (RActor actor in _instance._ActorList)
            {
                try
                {
                    actor.Dispose();
                }
                catch
                {
                    REngine.Instance.AddToLog("Could not dispose of " + actor._name);
                }
            }
            foreach (RMesh mesh in _instance._MeshList)
            {
                try
                {
                    mesh.Dispose();
                }
                catch
                {
                    REngine.Instance.AddToLog("Could not dispose of " + mesh._name);
                }
            }
            foreach (RLandscape land in _instance._LandscapeList)
            {
                try
                {
                    land.Dispose();
                }
                catch
                {
                    REngine.Instance.AddToLog("Could not dispose of " + land._name);
                }
            }
            foreach (RWater water in _instance._WaterList)
            {
                try
                {
                    water.Dispose();
                }
                catch
                {
                    REngine.Instance.AddToLog("Could not dispose of " + water._name);
                }
            }
            foreach (RParticleEmitter emitter in _instance._ParticleEmitterList)
            {
                try
                {
                    emitter.Dispose();
                }
                catch
                {
                    REngine.Instance.AddToLog("Could not dispose of " + emitter._name);
                }
            }
            try
            {
                _instance._NodeList.Clear();
            }
            catch(Exception e)
            {
                REngine.Instance.AddToLog("Could not dispose of NodeList! " + e.Message);
            }
        }
        public void UpdateAll()
        {
            foreach (RSceneNode node in _instance._NodeList)
            {
                node.Update();
            }
        }
        public void RenderAll()
        {
            foreach (RMesh mesh in _instance._MeshList.Values)
            {
                mesh.Render();
            }
        }
        #endregion
    }

    public class RSceneNode
    {
        internal Vector3 Position;
        internal Vector3 Rotation;
        internal Matrix Matrix;
        internal Quaternion Quaternion;
        internal RSceneNode ParentNode;
        internal List<RSceneNode> ChildNodes = new List<RSceneNode>();
        internal string _name;
        public string Name
        {
            get { return _name; }
        }
        public RSceneNode GetParentNode()
        {
            return ParentNode;
        }
        public void SetParentNode(RSceneNode node)
        {
            ParentNode = node;
        }
        
        public RSceneNode[] GetChildNodes()
        {
            return ChildNodes.ToArray();
        }
        public List<RSceneNode> GetChildNodeList()
        {
            return ChildNodes;
        }
        public void SetChildNodes(RSceneNode[] node_array)
        {
            ChildNodes = new List<RSceneNode>(node_array);
        }
        public void SetChildNodes(List<RSceneNode> node_list)
        {
            ChildNodes = node_list;
        }
        public void AddChildNode(RSceneNode node)
        {
            ChildNodes.Add(node);
        }
        public void RemoveChildNode(RSceneNode node)
        {
            ChildNodes.Remove(node);
        }
        public virtual void Update()
        {
        }
        public virtual void Render()
        {
        }
    }
}