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
		public RSceneNode CloneNode(RSceneNode node, string name)
		{
			if (node.GetType() == typeof(RActor))
			{
				RActor oldActor = (RActor)node;
				RActor actor = Instance.CreateActor(name);
				actor._model = oldActor._model;
				actor._player = oldActor._player;
				actor.Position = oldActor.Position;
				actor.Rotation = oldActor.Rotation;
				actor._skinningData = oldActor._skinningData;
				actor._transforms = oldActor._transforms;
				actor.Matrix = oldActor.Matrix;
				actor._defaultEffect = oldActor._defaultEffect;
				actor._material = oldActor._material;
				actor._materials = oldActor._materials;

				actor.Scaling = oldActor.Scaling;
				actor.ParentNode = oldActor.ParentNode;
				actor.Name = name;
				return actor;
			}
			else if (node.GetType() == typeof(RMesh))
			{
				RMesh oldMesh = (RMesh)node;
				RMesh mesh = Instance.CreateMesh(name);
				mesh._basicEffect = oldMesh._basicEffect;
				mesh._boundingBox = oldMesh._boundingBox;
				mesh._material = oldMesh._material;
				mesh._materials = oldMesh._materials;
				mesh._model = oldMesh._model;
				mesh.Name = oldMesh.Name;
				mesh.Matrix = oldMesh.Matrix;
				mesh.Position = oldMesh.Position;
				mesh.Rotation = oldMesh.Rotation;
				mesh._transforms = oldMesh._transforms;

				mesh.Scaling = oldMesh.Scaling;
				mesh.ParentNode = node.ParentNode;
				return mesh;
			}
			else if (node.GetType() == typeof(RMeshBuilder))
			{
				RMeshBuilder mesh = Instance.CreateMeshBuilder(name);
				RMeshBuilder oldMesh = (RMeshBuilder)node;
				mesh.Name = name;
				mesh.Matrix = node.Matrix;
				mesh.Position = node.Position;
				mesh.Quaternion = node.Quaternion;
				mesh.Rotation = node.Rotation;
				mesh.Name = name;
				mesh._buffer = oldMesh._buffer;
				mesh._basicEffect = oldMesh._basicEffect;
				mesh._index = oldMesh._index;
				mesh._material = oldMesh._material;
				mesh.vertCount = oldMesh.vertCount;
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
				g.Name = name;
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
            landscape.Name = name;
            _instance._LandscapeList.Add(name, landscape);
            _instance._NodeList.Add(landscape);
            REngine.Instance.AddToLog("RLandscape: " + name + " Created by RScene");
            return landscape;
        }
        public RParticleEmitter CreateParticleEmitter(CONST_REACTOR_PARTICLETYPE type, string name)
        {
            RParticleEmitter emitter = new RParticleEmitter();
            emitter.CreateEmitter(type);
            emitter.Name = name;
            _instance._ParticleEmitterList.Add(name, emitter);
            _instance._NodeList.Add(emitter);
            REngine.Instance.AddToLog("RParticleEmitter: " + name + " Created by RScene");
            return emitter;
        }
        public RWater CreateWater(string name)
        {
            RWater water = new RWater();
            water.CreateWater();
            water.Name = name;
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
                    REngine.Instance.AddToLog("Could not dispose of " + actor.Name);
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
                    REngine.Instance.AddToLog("Could not dispose of " + mesh.Name);
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
                    REngine.Instance.AddToLog("Could not dispose of " + land.Name);
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
                    REngine.Instance.AddToLog("Could not dispose of " + water.Name);
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
                    REngine.Instance.AddToLog("Could not dispose of " + emitter.Name);
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
		internal Vector3 Position = Vector3.Zero;
		internal Vector3 Rotation = Vector3.Zero;
		internal Vector3 Scaling = Vector3.One;
		internal Matrix Matrix = Matrix.Identity;
        internal Quaternion Quaternion;
        internal RSceneNode ParentNode;
        internal List<RSceneNode> ChildNodes = new List<RSceneNode>();
        private string _name;
        public string Name
        {
            get { return _name; }
			internal set { _name = value; }
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
		public R3DVECTOR GetRotation()
		{
			return R3DVECTOR.FromVector3(Rotation);
		}

		public R3DVECTOR GetScale()
		{
			return R3DVECTOR.FromVector3(Scaling);
		}
		public R3DMATRIX GetMatrix()
		{
			return R3DMATRIX.FromMatrix(this.Matrix);
		}
		public void SetMatrix(R3DMATRIX Matrix)
		{
			this.Matrix = Matrix.matrix;
		}
		public void SetLookAt(R3DVECTOR vector)
		{

			Vector3 dir = Vector3.Normalize(vector.vector - Position);

			Matrix = Matrix.CreateLookAt(Position, vector.vector, Vector3.Up);
			Matrix = BuildScalingMatrix(Matrix);
			Matrix.Translation = Position;
			Quaternion q = Quaternion.CreateFromRotationMatrix(Matrix);

			Rotation.X = q.X;
			Rotation.Y = q.Y;
			Rotation.Z = q.Z;




		}
		internal Matrix BuildRotationMatrix(Matrix m)
		{   
			m *= Matrix.CreateRotationX(Rotation.X);
			m *= Matrix.CreateRotationY(Rotation.Y);
			m *= Matrix.CreateRotationZ(Rotation.Z);
			return m;
		}
		internal Matrix BuildScalingMatrix(Matrix m)
		{
			m *= Matrix.CreateScale(Scaling);
			return m;
		}
		internal Matrix BuildPositionMatrix(Matrix m)
		{
			m *= Matrix.CreateTranslation(Position);
			//_transforms[_model.Meshes[0].ParentBone.Index] = m;
			return m;
		}
		public void RotateX(float value)
		{
			Rotation.X += MathHelper.ToRadians(value);
			Matrix *= Matrix.CreateRotationX(Rotation.X);
		}
		public void RotateY(float value)
		{
			Rotation.Y += MathHelper.ToRadians(value);
			Matrix *= Matrix.CreateRotationY(Rotation.Y);
		}
		public void RotateZ(float value)
		{
			Rotation.Z += MathHelper.ToRadians(value);
			Matrix *= Matrix.CreateRotationZ(Rotation.Z);
		}
		public void Rotate(float X, float Y, float Z)
		{
			RotateX(X);
			RotateY(Y);
			RotateZ(Z);
		}
		public R3DVECTOR GetPosition()
		{
			return R3DVECTOR.FromVector3(Position);
		}
		public void SetPosition(R3DVECTOR vector)
		{
			Position = vector.vector;
			Matrix = BuildPositionMatrix(Matrix);
			return;
		}
		public void SetPosition(float x, float y, float z)
		{
			Position = new Vector3(x, y, z);
			Matrix.Translation = Position;
		}
		public void Move(R3DVECTOR vector)
		{
			Move(vector.X, vector.Y, vector.Z);
		}
		public void Move(float x, float y, float z)
		{
			Position += Matrix.Left * x;
			Position += Matrix.Up * y;
			Position += Matrix.Forward * z;

			Matrix.Translation = Position;
		}
		public void SetScale(float ScaleX, float ScaleY, float ScaleZ)
		{
			Scaling = new Vector3(ScaleX, ScaleY, ScaleZ);
			Matrix = BuildScalingMatrix(Matrix);

		}
    }
}