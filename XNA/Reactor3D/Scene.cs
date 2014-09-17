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
		internal Octree _octree;
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
			_instance._octree.Add(mesh, mesh.AABB);
            REngine.Instance.AddToLog("RMesh: " + name + " Created by RScene");
            return mesh;
        }
        public RMeshBuilder CreateMeshBuilder(string name)
        {
            RMeshBuilder mesh = new RMeshBuilder();
            mesh.CreateMesh(name);
            _instance._MeshBuilderList.Add(name, mesh);
			_instance._octree.Add(mesh, mesh.AABB);
			REngine.Instance.AddToLog("RMeshBuilder: " + name + " Created by RScene");
            return mesh;
        }

        public RActor CreateActor(string name)
        {
            RActor actor = new RActor();
            actor.CreateActor(name);
            _instance._ActorList.Add(name, actor);
			_instance._octree.Add(actor, actor.AABB);

            REngine.Instance.AddToLog("RActor: " + name + " Created by RScene");
            return actor;
        }
        public RLandscape CreateLandscape(string name)
        {
            RLandscape landscape = new RLandscape();
            landscape.CreateLandscape();
            landscape.Name = name;
            _instance._LandscapeList.Add(name, landscape);
			_instance._octree.Add(landscape, landscape.AABB);

            REngine.Instance.AddToLog("RLandscape: " + name + " Created by RScene");
            return landscape;
        }
        public RParticleEmitter CreateParticleEmitter(CONST_REACTOR_PARTICLETYPE type, string name)
        {
            RParticleEmitter emitter = new RParticleEmitter();
            emitter.CreateEmitter(type);
            emitter.Name = name;
            _instance._ParticleEmitterList.Add(name, emitter);
			_instance._octree.Add(emitter, emitter.AABB);
            REngine.Instance.AddToLog("RParticleEmitter: " + name + " Created by RScene");
            return emitter;
        }
        public RWater CreateWater(string name)
        {
            RWater water = new RWater();
            water.CreateWater();
            water.Name = name;
            _instance._WaterList.Add(name, water);
			_instance._octree.Add(water, water.AABB);

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
        }
        public void UpdateAll()
        {
			_octree.Update();
        }
        public void RenderAll()
        {
			_octree.Draw();
        }
        #endregion
    }

    public class RSceneNode
    {
		internal RBOUNDINGBOX AABB = new RBOUNDINGBOX();
		internal int Level = 0;
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
        
        public RSceneNode[] GetChildNodesArray()
        {
            return ChildNodes.ToArray();
        }
        public List<RSceneNode> GetChildNodes()
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
		#region movement
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
			UpdateAABB();
		}
		private void UpdateAABB()
		{
			UpdateAABB(1.0f);
		}
		private void UpdateAABB(float scale)
		{
			UpdateAABB(scale, scale, scale);
		}
		private void UpdateAABB(float scaleX, float scaleY, float scaleZ)
		{
			this.Position = this.Matrix.Translation;
			this.Quaternion = RQUATERNION.CreateFromRotationMatrix(R3DMATRIX.FromMatrix(this.Matrix)).quaternion;
			this.Rotation = new Vector3(Quaternion.X, Quaternion.Y, Quaternion.Z);
			R3DVECTOR[] bounds = this.AABB.GetCorners();
			for(int i = 0; i < bounds.Length; i++){
				bounds[i] = R3DVECTOR.Transform(bounds[i], Quaternion.CreateFromRotationMatrix(this.Matrix));
				bounds[i] += R3DVECTOR.FromVector3(Matrix.Translation);
				R3DVECTOR scaleDir = bounds[i] - this.AABB.Center;
				bounds[i] += (scaleDir * new R3DVECTOR(scaleX, scaleY, scaleZ));

			}
			this.AABB = RBOUNDINGBOX.CreateFromPoints(bounds);
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
			UpdateAABB();



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
			UpdateAABB();
		}
		public void RotateY(float value)
		{
			Rotation.Y += MathHelper.ToRadians(value);
			Matrix *= Matrix.CreateRotationY(Rotation.Y);
			UpdateAABB();
		}
		public void RotateZ(float value)
		{
			Rotation.Z += MathHelper.ToRadians(value);
			Matrix *= Matrix.CreateRotationZ(Rotation.Z);
			UpdateAABB();
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
			UpdateAABB();
		}
		public void SetPosition(float x, float y, float z)
		{
			Position = new Vector3(x, y, z);
			Matrix.Translation = Position;
			UpdateAABB();
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
			UpdateAABB();
		}
		public void SetScale(float ScaleX, float ScaleY, float ScaleZ)
		{
			Scaling = new Vector3(ScaleX, ScaleY, ScaleZ);
			Matrix = BuildScalingMatrix(Matrix);
			UpdateAABB(ScaleX, ScaleY, ScaleZ);
		}
		#endregion

    }

	public class Octree
	{
		/// 

		/// The number of children in an octree.
		/// 

		private const int ChildCount = 8;

		/// 

		/// The octree's looseness value.
		/// 

		private float looseness = 0;

		/// 

		/// The octree's depth.
		/// 

		private int depth = 0;

		//private static DebugShapesDrawer debugDraw = null;

		/// 

		/// Gets or sets the debug draw.
		/// 

		/// 
		/// The debug draw.
		/// 
		//public DebugShapesDrawer DebugDraw
		//{
		//	get { return debugDraw; }
		//	set { debugDraw = value; }
		//}

		/// 

		/// The octree's center coordinates.
		/// 

		private R3DVECTOR center = R3DVECTOR.Zero;

		/// 

		/// The octree's length.
		/// 

		private float length = 0f;

		/// 

		/// The bounding box that represents the octree.
		/// 

		private RBOUNDINGBOX bounds = default(RBOUNDINGBOX);

		/// 

		/// The objects in the octree.
		/// 

		private ArrayList objects = new ArrayList();

		public ArrayList Nodes
		{
			get { return objects; }
		}

		/// 

		/// The octree's child nodes.
		/// 

		private Octree[] children = null;

		public Octree[] Children
		{
			get { return children; }
		}

		/// 

		/// The octree's world size.
		/// 

		private float worldSize = 0f;

		/// 

		/// Creates a new octree.
		/// 

		/// The octree's world size.
		/// The octree's looseness value.
		/// The octree recursion depth.
		public Octree(float worldSize, float looseness, int depth)
			: this(worldSize, looseness, depth, 0, R3DVECTOR.Zero)
		{
		}

		public Octree(float worldSize, float looseness, int depth, R3DVECTOR center)
			: this(worldSize, looseness, depth, 0, center)
		{
		}

		/// 

		/// Creates a new octree.
		/// 

		/// The octree's world size.
		/// The octree's looseness value.
		/// The maximum depth to recurse to.
		/// The octree recursion depth.
		/// The octree's center coordinates.
		private Octree(float worldSize, float looseness,
			int maxDepth, int depth, R3DVECTOR center)
		{
			this.worldSize = worldSize;
			this.looseness = looseness;
			this.depth = depth;
			this.center = center;
			this.length = this.looseness * this.worldSize / (float)Math.Pow(2, this.depth);
			float radius = this.length / 2f;

			// Create the bounding box.
			R3DVECTOR min = this.center + new R3DVECTOR(-radius);
			R3DVECTOR max = this.center + new R3DVECTOR(radius);
			this.bounds = new RBOUNDINGBOX(min, max);

			// Split the octree if the depth hasn't been reached.
			if (this.depth < maxDepth)
			{
				this.Split(maxDepth);
			}
		}

		/// 

		/// Removes the specified obj.
		/// 

		/// The obj.
		public void Remove(RSceneNode obj)
		{
			objects.Remove(obj);
		}

		/// 

		/// Determines whether the specified obj has changed.
		/// 

		/// The obj.
		/// The transformebbox.
		/// 
		///   true if the specified obj has changed; otherwise, false.
		/// 
		public bool HasChanged(RSceneNode obj, RBOUNDINGBOX transformebbox)
		{
			return this.bounds.Contains(transformebbox) == ContainmentType.Contains;
		}

		/// 

		/// Stills inside ?
		/// 

		/// The o.
		/// The center.
		/// The radius.
		/// 
		public bool StillInside(RSceneNode o, R3DVECTOR center, float radius)
		{
			R3DVECTOR min = center - new R3DVECTOR(radius);
			R3DVECTOR max = center + new R3DVECTOR(radius);
			RBOUNDINGBOX bounds = new RBOUNDINGBOX(min, max);

			if (this.children != null)
				return false;

			if (this.bounds.Contains(bounds) == ContainmentType.Contains)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// 

		/// Stills inside ?.
		/// 

		/// The obj.
		/// Its bounds.
		/// 
		public bool StillInside(RSceneNode o, RBOUNDINGBOX bounds)
		{
			if (this.children != null)
				return false;

			if (this.bounds.Contains(bounds) == ContainmentType.Contains)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// 

		/// Adds the given object to the octree.
		/// 

		/// The object to add.
		/// The object's center coordinates.
		/// The object's radius.
		public Octree Add(RSceneNode o, R3DVECTOR center, float radius)
		{
			R3DVECTOR min = center - new R3DVECTOR(radius);
			R3DVECTOR max = center + new R3DVECTOR(radius);
			RBOUNDINGBOX bounds = new RBOUNDINGBOX(min, max);

			if (this.bounds.Contains(bounds) == ContainmentType.Contains)
			{
				return this.Add(o, bounds, center, radius);
			}
			return null;
		}


		/// 

		/// Adds the given object to the octree.
		/// 

		public Octree Add(RSceneNode o, RBOUNDINGBOX transformebbox)
		{
			float radius = (transformebbox.Max - transformebbox.Min).Length() / 2;
			R3DVECTOR center = (transformebbox.Max + transformebbox.Min) / 2;

			if (this.bounds.Contains(transformebbox) == ContainmentType.Contains)
			{
				return this.Add(o, transformebbox, center, radius);
			}
			return null;
		}


		/// 

		/// Adds the given object to the octree.
		/// 

		/// The object to add.
		/// The object's bounds.
		/// The object's center coordinates.
		/// The object's radius.
		private Octree Add(RSceneNode o, RBOUNDINGBOX bounds, R3DVECTOR center, float radius)
		{
			if (this.children != null)
			{
				// Find which child the object is closest to based on where the
				// object's center is located in relation to the octree's center.
				int index = (center.X <= this.center.X ? 0 : 1) +
					(center.Y >= this.center.Y ? 0 : 4) +
					(center.Z <= this.center.Z ? 0 : 2);

				// Add the object to the child if it is fully contained within
				// it.
				if (this.children[index].bounds.Contains(bounds) == ContainmentType.Contains)
				{
					return this.children[index].Add(o, bounds, center, radius);

				}
			}
			this.objects.Add(o);
			return this;
		}


		public void Update()
		{
			foreach(RSceneNode node in objects)
			{
				node.Update();

				if(!StillInside(node, bounds)){
					Remove(node);
					Add(node, node.AABB);
				}
			}
			foreach(Octree child in children)
			{
				child.Update();
			}
		}

		/// 

		/// Draws the octree.
		/// 

		/// The viewing matrix.
		/// The projection matrix.
		/// The objects in the octree.
		/// The number of octrees drawn.
		public int Draw()
		{
			RCamera camera = REngine.Instance.GetCamera();
			Matrix view = camera.ViewMatrix.ToMatrix();
			Matrix projection = camera.ProjectionMatrix.ToMatrix();
			BoundingFrustum frustum = new BoundingFrustum(view * projection);
			ContainmentType containment = frustum.Contains(this.bounds.ToBoundingBox());

			return this.Draw(frustum, view, projection, containment, objects);
		}

		/// 

		/// Draws the octree.
		/// 

		/// The viewing frustum used to determine if the octree is in view.
		/// The viewing matrix.
		/// The projection matrix.
		/// Determines how much of the octree is visible.
		/// The objects in the octree.
		/// The number of octrees drawn.
		private int Draw(BoundingFrustum frustum, Matrix view, Matrix projection,
			ContainmentType containment, ArrayList objects)
		{
			int count = 0;

			if (containment != ContainmentType.Contains)
			{
				containment = frustum.Contains(this.bounds.ToBoundingBox());
			}

			// Draw the octree only if it is atleast partially in view.
			if (containment != ContainmentType.Disjoint)
			{
				// Draw the octree's bounds if there are objects in the octree.
				if (this.objects.Count > 0)
				{                    
					//if (DebugDraw != null)
					//	DebugDraw.AddShape(new DebugBox(this.bounds,Color.White));
					objects.AddRange(this.objects);
					count++;
				}

				// Draw the octree's children.
				if (this.children != null)
				{
					foreach (Octree child in this.children)
					{
						count += child.Draw(frustum, view, projection, containment, objects);
					}
				}
			}

			return count;
		}

		/// 

		/// Splits the octree into eight children.
		/// 

		/// The maximum depth to recurse to.
		private void Split(int maxDepth)
		{
			this.children = new Octree[Octree.ChildCount];
			int depth = this.depth + 1;
			float quarter = this.length / this.looseness / 4f;

			this.children[0] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(-quarter, quarter, -quarter));
			this.children[1] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(quarter, quarter, -quarter));
			this.children[2] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(-quarter, quarter, quarter));
			this.children[3] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(quarter, quarter, quarter));
			this.children[4] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(-quarter, -quarter, -quarter));
			this.children[5] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(quarter, -quarter, -quarter));
			this.children[6] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(-quarter, -quarter, quarter));
			this.children[7] = new Octree(this.worldSize, this.looseness,
				maxDepth, depth, this.center + new R3DVECTOR(quarter, -quarter, quarter));
		}

	}

}