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
using System.Collections.Specialized;
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
    internal struct VertexTerrain
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 Binormal;
        public Vector4 TerrainColorWeight;

        public static int SizeInBytes = (3 + 3 + 3 + 3 + 4) * 4;
        public static VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
            new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
            new VertexElement( sizeof(float) * 6, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0 ),
            new VertexElement( sizeof(float) * 9, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0 ),
            new VertexElement( sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0 ),
        };
    }

    //Grass doesn't work well... Don't use it....
    internal class GrassBillboard : RSceneNode, IDisposable
    {
        public static VertexDeclaration dec;
        internal static Random random;
        internal DynamicVertexBuffer buffer;
        internal IndexBuffer indices;
        internal Vector3 normal;
        internal int type = 0;
        internal float maxDistance = 1000;
        internal Effect vegEffect;
        internal int texture = 0;
        internal float Random;
        internal bool enabled = true;
        public float width, height;
        public int Type
        {
            get { return type; }
        }
        public GrassBillboard()
        {
            if (dec == null)
                dec = new VertexDeclaration(VertexBillboard.VertexElements);
            
            Random = (float)random.NextDouble() * 2 - 1;
        }
        public GrassBillboard(Vector3 Position, Vector3 Normal, int Type, float MaxDistance)
        {
            this.Position = Position;
            VertexBillboard[] verts = new VertexBillboard[4];
            int[] index = new int[6];
            type = Type;
            normal = Normal;
            Random = (float)random.NextDouble() * 2 - 1;
            for(int i=0; i<4; i++)
            {
                verts[i] = new VertexBillboard();
                verts[i].Position = Vector3.One;
                verts[i].Normal = Normal;
                verts[i].TextureCoordinate = ComputeTexCoord(i);
                
            }
            index[0] = 0;
            index[1] = 1;
            index[2] = 2;
            index[3] = 0;
            index[4] = 2;
            index[5] = 3;
            indices = new IndexBuffer(REngine.Instance._graphics.GraphicsDevice, typeof(int), 6, BufferUsage.None);
            indices.SetData<int>(index);
            buffer = new DynamicVertexBuffer(REngine.Instance._graphics.GraphicsDevice, typeof(VertexBillboard), 4, BufferUsage.None);
            buffer.SetData<VertexBillboard>(verts);
            texture = 0;
            width = 3.0f;
            height = 3.0f;
            if(dec == null)
                dec = new VertexDeclaration( VertexBillboard.VertexElements);
        }
        public void SetEffect(ref Effect effect)
        {
            vegEffect = effect;
            //vegEffect = new BasicEffect(REngine.Instance._graphics.GraphicsDevice, REngine.Instance._effectPool);
        }
        public void SetPosition(Vector3 Pos)
        {
            VertexBillboard[] data = new VertexBillboard[4];
            buffer.GetData<VertexBillboard>(data);
            data[0].Position = Pos;
            data[1].Position = Pos;
            data[2].Position = Pos;
            data[3].Position = Pos;
            buffer.SetData<VertexBillboard>(data);
        }
        public void SetTexture(int VegTexture)
        {
            texture = VegTexture;
        }
        
        public void SetSize(float Width, float Height)
        {
            width = Width;
            height = Height;
        }
        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }
        private class DistanceComparer : IComparer<GrassBillboard>
        {
            public int Compare(GrassBillboard first, GrassBillboard second)
            {
                RCamera cam = REngine.Instance._camera;
                if (first == null & second == null)
                {
                    return 0;
                }

                else if (first == null)
                {
                    return -1;
                }

                else if (second == null)
                {
                    return 1;
                }

                else if (Vector3.Distance(first.Position, cam.Position.vector) < Vector3.Distance(second.Position, cam.Position.vector))
                {
                    return 1;
                }

                else if (Vector3.Distance(first.Position, cam.Position.vector) == Vector3.Distance(second.Position, cam.Position.vector))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }
        

        public static IComparer<GrassBillboard> DistanceSorter
        {
            get { return new DistanceComparer(); }
        }
        public override void Render()
        {
            if (enabled)
            {
                
                float distancecalc = Vector3.Distance(REngine.Instance._camera.Position.vector, Position);
                float frac = maxDistance * 0.25f;
                float min = maxDistance - frac;
                float dist = distancecalc > min ? (distancecalc - min / maxDistance - min) : 1.0f;

                REngine.Instance._game.GraphicsDevice.SetVertexBuffer(buffer);
                    REngine.Instance._game.GraphicsDevice.Indices = indices;
                
                vegEffect.CurrentTechnique = vegEffect.Techniques["Billboards"];
                vegEffect.Parameters["Distance"].SetValue(dist);
                vegEffect.Parameters["Texture0"].SetValue((Texture2D)RTextureFactory.Instance._textureList[texture]);
                //vegEffect.Parameters["Texture1"].SetValue((Texture2D)RTextureFactory.Instance._textureList[textures[1]]);
                //vegEffect.Parameters["Texture2"].SetValue((Texture2D)RTextureFactory.Instance._textureList[textures[2]]);
                vegEffect.Parameters["ViewDirection"].SetValue(REngine.Instance._camera.viewDir);
                vegEffect.Parameters["BillboardWidth"].SetValue(width);
                vegEffect.Parameters["BillboardHeight"].SetValue(height);
                vegEffect.Parameters["View"].SetValue(REngine.Instance._camera.viewMatrix);
                vegEffect.Parameters["Projection"].SetValue(REngine.Instance._camera.projMatrix);
                vegEffect.Parameters["Position"].SetValue(Position);
                vegEffect.Parameters["Random"].SetValue(Random);
                vegEffect.Parameters["LightDirection"].SetValue(RAtmosphere.Instance.sunDirection);
                vegEffect.Parameters["LightColor"].SetValue(RAtmosphere.Instance.sunColor);
                vegEffect.Parameters["WindTime"].SetValue((float)REngine.Instance._gameTime.TotalGameTime.TotalSeconds * 0.133f);
                if (RAtmosphere.Instance != null)
                {
                    vegEffect.Parameters["FOGENABLE"].SetValue(RAtmosphere.Instance.fogEnabled);
                    vegEffect.Parameters["FOGCOLOR"].SetValue(RAtmosphere.Instance.fogColor);
                    vegEffect.Parameters["FOGDIST"].SetValue(RAtmosphere.Instance.fogDistance);
                }
                foreach (EffectPass pass in vegEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    REngine.Instance._graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
                }




                //REngine.Instance._graphics.GraphicsDevice.Indices = null;
                //REngine.Instance._graphics.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
            }
            
        }
        public override void Update()
        {
            VertexBillboard[] verts = new VertexBillboard[4];
            buffer.GetData<VertexBillboard>(verts);
            verts[0].Position = Position;
            verts[0].Normal = normal;
            verts[1].Position = Position;
            verts[1].Normal = normal;
            verts[2].Position = Position;
            verts[2].Normal = normal;
            verts[3].Position = Position;
            verts[3].Normal = normal;
            buffer.SetData<VertexBillboard>(verts);
            base.Update();
        }
        Vector2 ComputeTexCoord(int i)
        {
            switch (i)
            {
                case 0:
                    return new Vector2(0, 0);
                    break;
                case 1:
                    return new Vector2(1, 0);
                    break;
                case 2:
                    return new Vector2(1, 1);
                    break;
                case 3:
                    return new Vector2(0, 1);
                    break;
                default:
                    return new Vector2(0, 0);
                    break;
            }
        }
        public void Dispose()
        {
            //vegEffect.Dispose();
            indices.Dispose();
            buffer.Dispose();
        }
    }
    internal class QuadTree
        {
            #region Properties
            #endregion

            #region Fields
            public RLandscape Terrain;        // Terrain this quad-tree belongs to

            BoundingBox TreeBoundingBox;    // Holds bounding box used for culling

            // This holds the references to the 4 child nodes.
            // These remain null if this quadtree is a leaf node.
            QuadTree TopLeft;
            QuadTree TopRight;
            QuadTree BottomLeft;
            QuadTree BottomRight;
            List<QuadTree> TreeList;
            List<GrassBillboard> GrassList;
            bool Leaf = false;

            Vector2 FirstCorner = Vector2.Zero;
            Vector2 LastCorner = Vector2.Zero;
            int QuadSize = 0;
            //VertexBuffer LeafVertexBuffer;

            TerrainPatch LeafPatch;
            DynamicVertexBuffer VegBuffer;
            DynamicIndexBuffer VegIndex;
            public int Width;
            public int Height;
            public int OffsetX;
            public int OffsetY;

            public int VertexBufferOffset = 0;
            public int VertexBufferOffsetEnd = 0;
            public int VegVertBufferOffset = 0;
            public int VegVertBufferOffsetEnd = 0;
            OcclusionQuery Query;
            // ==========================================================
            // Used for debug only to show how many leaves are drawn
            public int LeavesDrawn
            {
                get { return leavesDrawn; }
                set { leavesDrawn = 0; }
            }
            static int leavesDrawn = 0;
            // ==========================================================

            public bool BoundingBoxHit
            {
                get { return boundingBoxHit; }
                set { boundingBoxHit = value; }
            }
            static bool boundingBoxHit = false;

            #region BoundingBoxMesh DEBUG
            public VertexPositionColor[] boundingBoxMesh
            {
                get { return BoundingBoxMesh; }
            }
            VertexPositionColor[] BoundingBoxMesh;

            #endregion

            BasicEffect lineEffect;
            VertexDeclaration lineVertexDeclaration;

            bool InView = false;

            // Holds heights that determine this tree's bounding box
            float MinHeight = 1000000;
            float MaxHeight = 0;

            int RootWidth;
            #endregion

            #region Initialization
            /// <summary>
            /// Creates a QuadTree Node using the supplied Terrain and VerticesLength
            /// </summary>
            /// <param name="SourceTerrain"></param>
            /// <param name="VerticesLength"></param>
            public QuadTree(RLandscape SourceTerrain, int VerticesLength)
            {
                this.Terrain = SourceTerrain;
                Terrain.Device = REngine.Instance._game.GraphicsDevice;
                // Line effect is used for rendering debug bounding boxes
                lineEffect = new BasicEffect(REngine.Instance._game.GraphicsDevice);
                lineEffect.VertexColorEnabled = true;

                lineVertexDeclaration = VertexPositionColor.VertexDeclaration;

                // This truncation requires all heightmap images to be
                // a power of two in height and width
                Width = (int)Math.Sqrt(VerticesLength);
                Height = Width;
                RootWidth = Width;

                // Vertices are only used for setting up the dimensions of
                // the bounding box. The vertices used in rendering are
                // located in the terrain class.
                SetUpBoundingBoxes();
                Query = new OcclusionQuery(REngine.Instance._game.GraphicsDevice);
                // If this tree is the smallest allowable size, set it as a leaf
                // so that it will not continue branching smaller.
                if (VerticesLength <= Terrain.MinimumLeafSize)
                {
                    Leaf = true;

                    CreateBoundingBoxMesh();
                }

                if (Leaf)
                {
                    SetUpTerrainVertices();

                    if (Terrain.DrawVegetation)
                        SetupVegetation();

                    LeafPatch = new TerrainPatch(Terrain.Device, Terrain, this, Width, Terrain.DetailDefault);
                    //RLandscape.CalculateTangents(LeafPatch.IndexBuffers[4], ref Terrain.TerrainVertexBuffer, ref Terrain.TerrainVertices, true, true, 0); 
                }
                else
                    BranchOffRoot();
            }

            // Use this when creating child trees/branches
            public QuadTree(RLandscape SourceTerrain, int VerticesLength,
                            int OffsetX, int OffsetY, int RootWidth)
            {
                this.Terrain = SourceTerrain;

                lineEffect = new BasicEffect(Terrain.Device);
                lineEffect.VertexColorEnabled = true;

                lineVertexDeclaration = VertexPositionColor.VertexDeclaration;

                this.OffsetX = OffsetX;
                this.OffsetY = OffsetY;

                // This truncation requires all heightmap images to be
                // a power of two in height and width
                Width = ((int)Math.Sqrt(VerticesLength) / 2) + 1;
                Height = Width;
                this.RootWidth = RootWidth;

                SetUpBoundingBoxes();

                // If this tree is the smallest allowable size, set it as a leaf
                // so that it will not continue branching smaller.
                if ((Width - 1) * (Height - 1) <= Terrain.MinimumLeafSize)
                {
                    Leaf = true;

                    CreateBoundingBoxMesh();
                    Query = new OcclusionQuery(REngine.Instance._game.GraphicsDevice);
                }

                if (Leaf)
                {
                    SetUpTerrainVertices();

                    if (Terrain.DrawVegetation)
                        SetupVegetation();

                    LeafPatch = new TerrainPatch(Terrain.Device, Terrain, this, Width, Terrain.DetailDefault);
                    //RLandscape.CalculateTangents(LeafPatch.IndexBuffers[4], ref Terrain.TerrainVertexBuffer, ref VertexTerrain, true, true, 0); 
                }
                else
                    BranchOff();
            }

            private void SetUpBoundingBoxes()
            {
                FirstCorner = new Vector2(OffsetX * Terrain.scale, OffsetY * Terrain.scale);
                LastCorner = new Vector2((Width - 1 + OffsetX) * Terrain.scale, (Height - 1 + OffsetY) * Terrain.scale);
                QuadSize = Width * Height;

                // Determine heights for use with the bounding box
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        if (Terrain.HeightData[x + OffsetX, y + OffsetY] < MinHeight)
                            MinHeight = Terrain.HeightData[x + OffsetX, y + OffsetY] - .1f;
                        else if (Terrain.HeightData[x + OffsetX, y + OffsetY] > MaxHeight)
                            MaxHeight = Terrain.HeightData[x + OffsetX, y + OffsetY];
                    }

                TreeBoundingBox = new BoundingBox(new Vector3(FirstCorner.X + this.Terrain.Position.X, MinHeight, FirstCorner.Y + this.Terrain.Position.Z), new Vector3(LastCorner.X + this.Terrain.Position.X, MaxHeight, LastCorner.Y + this.Terrain.Position.Z));
            }
            VertexTerrain[] verts;
            /// <summary>
            /// This sets up the vertices for all of the triangles in this quad-tree section
            /// passes them to the main terrain component.
            /// </summary>
            private void SetUpTerrainVertices()
            {
                int Offset = Terrain.TerrainVertices.Count;
                verts = new VertexTerrain[(Width)*(Height)];
                // Texture the level
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        VertexTerrain tempVert = new VertexTerrain();
                        tempVert.Position = new Vector3((OffsetX + x) * Terrain.scale,
                                                        Terrain.HeightData[OffsetX + x, OffsetY + y],
                                                        (OffsetY + y) * Terrain.scale);

                        tempVert.Normal = Terrain.TerrainNormals[OffsetX + x, OffsetY + y];
                        verts[x + y * Width] = tempVert;
                        //Terrain.TerrainVertices.Add(tempVert);
                    }
                
                // Calc Tangent and Bi Normals.
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        // Tangent Data.
                        if (x != 0 && x < Width - 1)
                            verts[(x + y * Width)].Tangent = verts[x - 1 + y * Width].Position - verts[x + 1 + y * Width].Position;
                        else
                            if (x == 0)
                                verts[(x + y * Width)].Tangent = verts[x + y * Width].Position - verts[x + 1 + y * Width].Position;
                            else
                                verts[(x + y * Width)].Tangent = verts[x - 1 + y * Width].Position - verts[x + y * Width].Position;

                        // Bi Normal Data.
                        if (y != 0 && y < Height - 1)
                            verts[x + y * Width].Binormal = verts[x + (y - 1) * Width].Position - verts[x + (y + 1) * Width].Position;
                        else
                            if (y == 0)
                                verts[x + y * Width].Binormal = verts[x + y * Width].Position - verts[x + (y + 1) * Width].Position;
                            else
                                verts[x + y * Width].Binormal = verts[x + (y - 1) * Width].Position - verts[x + y * Width].Position;
                    }
                 
                foreach (VertexTerrain v in verts)
                {
                    Terrain.TerrainVertices.Add(v);
                }
                VertexBufferOffset = Offset;
                VertexBufferOffsetEnd = Terrain.TerrainVertices.Count;
            }
            private int GrassIndex(int i, int j)
            {
                return j * (Width) + i;
            }
            List<Vector3> grassPlot = new List<Vector3>();
            BoundingBox GrassBox;
            VertexBuffer grassBuffer;
            IndexBuffer grassIndex;
            int grassCount;
            private void SetupVegetation()
            {
                GrassBillboard.random = new Random();
                List<Vector3> grassPos = new List<Vector3>();
                

                for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Width; j++)
                        {
                            Vector3 pos = new Vector3();
                            int x = GrassBillboard.random.Next(0, Width);
                            int y = GrassBillboard.random.Next(0, Width);
                            float k = (float)(2.0 * GrassBillboard.random.NextDouble()) - (1.0f);
                            float kk = (float)GrassBillboard.random.NextDouble();
                            kk = (float)GrassBillboard.random.NextDouble();
                                pos.X = x;
                                pos.X += k;
                                pos.Z = y;
                                pos.Z += k;
                            
                            pos.Y = Terrain.GetTerrainHeight((OffsetX + (pos.X))*Terrain.scale, (OffsetY + (pos.Z))*Terrain.scale)/Terrain.scale;
                            //if (pos.Y > Terrain.MinGrassHeight)
                            //{
                            //grassPos.Add(new Vector3((pos.X) * Terrain.scale, Terrain.GetTerrainHeight((pos.X) * Terrain.scale, (pos.Z) * Terrain.scale), (pos.Z) * Terrain.scale));
                            grassPos.Add(pos);
                            //}
                        }
                        //i += (int)Terrain.Veg0Size.X;
                    }
                
                
                grassCount = grassPos.Count;
                Vector3[] gpos = grassPos.ToArray();
                for (int i = 0; i < grassCount; i++)
                {
                    gpos[i] = new Vector3(gpos[i].X + OffsetX, gpos[i].Y, gpos[i].Z + OffsetY);
                    gpos[i] *= Terrain.scale;
                }

                    GrassBox = BoundingBox.CreateFromPoints(gpos);
                    gpos = null;
                

                    int numVertices = grassCount * 4;
                    int numIndices = grassCount * 6;
                    int[] index = new int[numIndices];
                    VertexBillboard[] grassverts = new VertexBillboard[numVertices];
                    grassBuffer = new VertexBuffer(Terrain.Device,VertexPositionColorTexture.VertexDeclaration, VertexBillboard.SizeInBytes * numVertices, BufferUsage.None);
                    grassIndex = new IndexBuffer(Terrain.Device, IndexElementSize.ThirtyTwoBits, numIndices * sizeof(int), BufferUsage.None);


                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Width; j++)
                        {
                            int gIndex = GrassIndex(i, j);
                            
                                Vector3 position = grassPos[gIndex];

                                
                                grassverts[4 * gIndex + 0] = new VertexBillboard();
                                grassverts[4 * gIndex + 0].Position = new Vector3(position.X, position.Y, position.Z);
                                grassverts[4 * gIndex + 0].TextureCoordinate = new Vector2(1.0f, 1.0f);
                                grassverts[4 * gIndex + 0].Normal = Vector3.Up;

                                grassverts[4 * gIndex + 1] = new VertexBillboard();
                                grassverts[4 * gIndex + 1].Position = new Vector3(position.X, position.Y, position.Z);
                                grassverts[4 * gIndex + 1].TextureCoordinate = new Vector2(1.0f, 0.0f);
                                grassverts[4 * gIndex + 1].Normal = Vector3.Up;

                                grassverts[4 * gIndex + 2] = new VertexBillboard();
                                grassverts[4 * gIndex + 2].Position = new Vector3(position.X, position.Y, position.Z);
                                grassverts[4 * gIndex + 2].TextureCoordinate = new Vector2(0.0f, 0.0f);
                                grassverts[4 * gIndex + 2].Normal = Vector3.Up;

                                grassverts[4 * gIndex + 3] = new VertexBillboard();
                                grassverts[4 * gIndex + 3].Position = new Vector3(position.X, position.Y, position.Z);
                                grassverts[4 * gIndex + 3].TextureCoordinate = new Vector2(0.0f, 1.0f);
                                grassverts[4 * gIndex + 3].Normal = Vector3.Up;

                                index[6 * gIndex + 0] = 4 * gIndex + 0;
                                index[6 * gIndex + 1] = 4 * gIndex + 1;
                                index[6 * gIndex + 2] = 4 * gIndex + 2;

                                index[6 * gIndex + 3] = 4 * gIndex + 0;
                                index[6 * gIndex + 4] = 4 * gIndex + 2;
                                index[6 * gIndex + 5] = 4 * gIndex + 3;
                            
                            //j += (int)Terrain.Veg0Size.X;
                        }
                        //i += (int)Terrain.Veg0Size.X;
                    }
                    grassBuffer.SetData<VertexBillboard>(grassverts);
                    grassIndex.SetData<int>(index);
                
                GrassBillboard.dec = new VertexDeclaration(VertexBillboard.VertexElements);
                /*GrassBillboard gtype0 = new GrassBillboard(Vector3.One, Vector3.Up, 0, Terrain.GrassDistance);
                gtype0.ParentNode = Terrain;
                gtype0.SetEffect(ref Terrain.vegEffect);
                gtype0.SetTexture(Terrain.TerrainVegetation[0]);
                gtype0.SetSize(Terrain.Veg0Size.X, Terrain.Veg0Size.Y);
                gtype0.enabled = false;
                GrassBillboard gtype1 = new GrassBillboard(Vector3.One, Vector3.Up, 1, Terrain.GrassDistance);
                gtype1.ParentNode = Terrain;
                gtype1.SetEffect(ref Terrain.vegEffect);
                gtype1.SetTexture(Terrain.TerrainVegetation[1]);
                gtype1.SetSize(Terrain.Veg1Size.X, Terrain.Veg1Size.Y);
                gtype1.enabled = false;
                GrassBillboard gtype2 = new GrassBillboard(Vector3.One, Vector3.Up, 2, Terrain.GrassDistance);
                gtype2.ParentNode = Terrain;
                gtype2.SetEffect(ref Terrain.vegEffect);
                gtype2.SetTexture(Terrain.TerrainVegetation[2]);
                gtype2.SetSize(Terrain.Veg2Size.X, Terrain.Veg2Size.Y);
                gtype2.enabled = false;
                GrassList = new List<GrassBillboard>();
                Vector3[] grassPoints;
                for(int i = 0; i < Terrain.MaxGrassCount; i++)
                {
                    
                
                    if (GrassBillboard.random.NextDouble() < Terrain.GrassFactor)
                    {
                    

                        Vector3 pos = new Vector3();
                       
                        int x = GrassBillboard.random.Next(0, Width);
                        int y = GrassBillboard.random.Next(0, Height);
                        if (x > 1 && y > 1)
                        {
                            Vector3 i1 = verts[(x) + (y) * Width].Position;
                            Vector3 i2 = verts[((x) - 1) + ((y)) * Width].Position;
                            Vector3 i3 = verts[((x) - 1) + (y - 1) * Width].Position;
                            PickRandomPoint(i1, i2, i3, out pos);
                        }
                        else
                            pos = verts[(x) + (y) * Width].Position;
                        
                        
                        if (Terrain.BillboardData[x, y] != -1)
                        {
                            if (pos.Y > Terrain.MinGrassHeight)
                            {
                            Vector2 Size = new Vector2(1.0f, 1.0f);
                            int veg = GrassBillboard.random.Next(0, 3);
                            if (veg == 0)
                            {
                                Size = Terrain.Veg0Size;
                                //Terrain.GetNormal(pos.X, pos.Z);
                                GrassBillboard g = (GrassBillboard)RScene.Instance.CloneNode(gtype0);
                                g.Position = pos;
                                //g.SetPosition(pos);
                                g.ParentNode = Terrain;
                                g.SetEffect(ref Terrain.vegEffect);
                                //g.SetTexture(Terrain.TerrainVegetation[veg]);
                                //g.SetSize(Size.X, Size.Y);
                                GrassList.Add(g);
                            }
                            else if (veg == 1)
                            {
                                Size = Terrain.Veg1Size;
                                //Terrain.GetNormal(pos.X, pos.Z);
                                GrassBillboard g = (GrassBillboard)RScene.Instance.CloneNode(gtype1);
                                g.Position = pos;
                                //g.SetPosition(pos);
                                g.ParentNode = Terrain;
                                g.SetEffect(ref Terrain.vegEffect);
                                //g.SetTexture(Terrain.TerrainVegetation[veg]);
                                //g.SetSize(Size.X, Size.Y);
                                GrassList.Add(g);
                            }
                            else if (veg == 2)
                            {
                                Size = Terrain.Veg2Size;
                                //Terrain.GetNormal(pos.X, pos.Z);
                                GrassBillboard g = (GrassBillboard)RScene.Instance.CloneNode(gtype2);
                                g.Position = pos;
                                //g.SetPosition(pos);
                                g.ParentNode = Terrain;
                                g.SetEffect(ref Terrain.vegEffect);
                                //g.SetTexture(Terrain.TerrainVegetation[veg]);
                                //g.SetSize(Size.X, Size.Y);
                                GrassList.Add(g);
                            }
                            
                            
                                
                            }
                        }
                    }
                
                        
                        
                    
                    //GC.Collect();
                }
                
                if (GrassList.Count > 0)
                {
                    grassPoints = new Vector3[GrassList.Count];

                    for (int i = 0; i < GrassList.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            grassPoints[i] = GrassList[i].Position;
                            //grassPoints[i].Y *= 2;
                        }
                        else
                        {
                            grassPoints[i] = GrassList[i].Position;
                        }
                    }
                    GrassBox = BoundingBox.CreateFromPoints(grassPoints);
                    grassPoints = null;
                }
                
                verts = null;
                 */
            }

            private void CreateBoundingBoxMesh()
            {
                List<Vector3> boxList = new List<Vector3>();

                BoundingBoxMesh = new VertexPositionColor[36];
                for (int i = 0; i < 36; i++)
                    BoundingBoxMesh[i].Color = Color.Magenta;

                foreach (Vector3 thisVector in TreeBoundingBox.GetCorners())
                {
                    boxList.Add(thisVector);
                }

                // Front
                BoundingBoxMesh[0].Position = boxList[0];
                BoundingBoxMesh[1].Position = boxList[1];
                BoundingBoxMesh[2].Position = boxList[2];

                BoundingBoxMesh[3].Position = boxList[2];
                BoundingBoxMesh[4].Position = boxList[3];
                BoundingBoxMesh[5].Position = boxList[0];

                // Top
                BoundingBoxMesh[6].Position = boxList[0];
                BoundingBoxMesh[7].Position = boxList[5];
                BoundingBoxMesh[8].Position = boxList[1];

                BoundingBoxMesh[9].Position = boxList[0];
                BoundingBoxMesh[10].Position = boxList[4];
                BoundingBoxMesh[11].Position = boxList[5];

                // Left
                BoundingBoxMesh[12].Position = boxList[0];
                BoundingBoxMesh[13].Position = boxList[3];
                BoundingBoxMesh[14].Position = boxList[7];

                BoundingBoxMesh[15].Position = boxList[7];
                BoundingBoxMesh[16].Position = boxList[4];
                BoundingBoxMesh[17].Position = boxList[0];

                // Right
                BoundingBoxMesh[18].Position = boxList[1];
                BoundingBoxMesh[19].Position = boxList[5];
                BoundingBoxMesh[20].Position = boxList[6];

                BoundingBoxMesh[21].Position = boxList[6];
                BoundingBoxMesh[22].Position = boxList[3];
                BoundingBoxMesh[23].Position = boxList[1];

                // Bottom
                BoundingBoxMesh[24].Position = boxList[3];
                BoundingBoxMesh[25].Position = boxList[7];
                BoundingBoxMesh[26].Position = boxList[6];

                BoundingBoxMesh[27].Position = boxList[6];
                BoundingBoxMesh[28].Position = boxList[2];
                BoundingBoxMesh[29].Position = boxList[3];

                // Back
                BoundingBoxMesh[30].Position = boxList[7];
                BoundingBoxMesh[31].Position = boxList[4];
                BoundingBoxMesh[32].Position = boxList[6];

                BoundingBoxMesh[33].Position = boxList[6];
                BoundingBoxMesh[34].Position = boxList[4];
                BoundingBoxMesh[35].Position = boxList[5];
            }
            #endregion

            #region Methods

            // Only called from the main root node
            private void BranchOffRoot()
            {
                TopLeft = new QuadTree(Terrain, QuadSize, 0, 0, RootWidth);
                BottomLeft = new QuadTree(Terrain, QuadSize, 0, Height / 2 - 1, RootWidth);
                TopRight = new QuadTree(Terrain, QuadSize, Width / 2 - 1, 0, RootWidth);
                BottomRight = new QuadTree(Terrain, QuadSize, Width / 2 - 1, Height / 2 - 1, RootWidth);

                TreeList = new List<QuadTree>();
                TreeList.Add(TopLeft);
                TreeList.Add(TopRight);
                TreeList.Add(BottomLeft);
                TreeList.Add(BottomRight);
            }

            // This is called to branch off of child nodes
            private void BranchOff()
            {
                TopLeft = new QuadTree(Terrain, QuadSize, 0 + OffsetX, 0 + OffsetY, RootWidth);
                BottomLeft = new QuadTree(Terrain, QuadSize, 0 + OffsetX, (Height - 1) / 2 - 1 + (OffsetY + 1), RootWidth);
                TopRight = new QuadTree(Terrain, QuadSize, (Width - 1) / 2 - 1 + (OffsetX + 1), 0 + OffsetY, RootWidth);
                BottomRight = new QuadTree(Terrain, QuadSize, (Width - 1) / 2 - 1 + (OffsetX + 1), (Height - 1) / 2 - 1 + (OffsetY + 1), RootWidth);

                TreeList = new List<QuadTree>();
                TreeList.Add(TopLeft);
                TreeList.Add(TopRight);
                TreeList.Add(BottomLeft);
                TreeList.Add(BottomRight);
            }
            BoundingFrustum bFrus;
            bool Occluded = false;
            public void Draw(ref BoundingFrustum bFrustum, RLANDSCAPE_LOD TerrainDetail)
            {
                bFrus = bFrustum;
                // View is kept track of for later when vegetation is drawn.
                // This keeps the program from having to calculate the frustum intersections
                // again for each node.
                
                int Detail = (int)TerrainDetail;
                //SetUpBoundingBoxes();

                //Camera Frustum Cull, any quads outside the view will not be drawn.
                ContainmentType type = bFrustum.Contains(TreeBoundingBox);
                if (type != ContainmentType.Disjoint && OcclusionRunning == false)
                    InView = true;
                


                //PVS Culling, any quads shadowed by the height of terrain in front will not be drawn
                


                    // Only draw leaves on the tree, never the main tree branches themselves.
                    if (Leaf && InView)
                    {
                        
                        /*BoundingSphere sphere = BoundingSphere.CreateFromBoundingBox(TreeBoundingBox);
                        float dist = Vector3.Distance(sphere.Center, REngine.Instance._camera.Position.vector);
                        float range = (Terrain.scale);
                        if (dist > (500f * (range*1.15f)))
                            Detail = 8;
                        else if (dist > (150f * (range * 1.15f)))
                            Detail = 4;
                        else if (dist > (10f * (range * 1.15f)))
                            Detail = 2;
                        else if (dist > (1f * (range * 1.15f)))
                            Detail = 1;
                        if (Detail > (int)RLANDSCAPE_LOD.Minimum)
                            Detail = (int)RLANDSCAPE_LOD.Minimum;
                        if (Detail < (int)RLANDSCAPE_LOD.High)
                            Detail = (int)RLANDSCAPE_LOD.High;*/
                        //Terrain.Device.Vertices[0].SetSource(LeafVertexBuffer, 0, VertexTerrain.SizeInBytes);
                        //Terrain.Device.VertexDeclaration = new VertexDeclaration(Terrain.Device, VertexTerrain.VertexElements);
                        Terrain.Device.Indices = LeafPatch.IndexBuffers;


                        foreach (EffectPass pass in Terrain.effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            Terrain.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, VertexBufferOffset, 0, Width * Height, 0, LeafPatch.NumTris);


                        }

                        leavesDrawn++;
                    }

                    // If there are branches on this node, move down through them recursively
                    if (TreeList != null)
                    {
                        for(int i = 0; i<TreeList.Count; i++)
                        {
                            TreeList[i].Draw(ref bFrustum, Terrain.terrainDetail);
                        }
                    }
                
            }
            Vector3 AddGrass(Vector3 CamPos, float Distance)
            {
                Redo:
                float radius = Distance + ((Distance/4) * (float)GrassBillboard.random.NextDouble());
                float angle = (float)GrassBillboard.random.NextDouble() * 360;
                //angle += .005f;
                if (angle > 360)
                    angle = 0;
                float cos = radius * (float)Math.Cos(angle);
                float sin = radius * (float)Math.Sin(angle);
                //float cos2 = radius * (float)Math.Cos(angle2);
                //float sin = (float)Math.Pow(radius, 1) * (float)Math.Sin(angle);
                Vector2 flatPos = new Vector2(CamPos.X + cos, CamPos.Z + sin);
                float y = Terrain.GetTerrainHeight(flatPos.X, flatPos.Y);
                Vector3 v = new Vector3();
                
                v = new Vector3(flatPos.X, y, flatPos.Y);
                
                return v;
            }
            void UpdateVegetation(Vector3 CamPos, float Distance)
            {
                
                int numDeleted = 0;
                int type;
                Vector2 size = Vector2.One;
                for(int i = 0; i < GrassList.Count; i++)
                {
                    
                    Vector3 left = new Vector3(GrassList[i].Position.X - GrassList[i].width, GrassList[i].Position.Y, GrassList[i].Position.Z - GrassList[i].width);
                    Vector3 right = new Vector3(GrassList[i].Position.X + GrassList[i].width, GrassList[i].Position.Y, GrassList[i].Position.Z + GrassList[i].width);
                    float dist = Math.Abs(Vector3.Distance(CamPos, GrassList[i].Position));
                    if (dist > Distance + (Distance/4))
                    {
                        numDeleted++;
                        type = GrassList[i].Type;
                        size = new Vector2(GrassList[i].width, GrassList[i].height);
                        Vector3 newpos = AddGrass(CamPos - ((right - left) * 0.001f), Distance);
                        //GrassList[i].Dispose();
                        GrassList[i].Position = newpos;
                        GrassList[i].Normal = Terrain.GetNormal(newpos.X, newpos.Z);
                        GrassList[i].Update();
                        
                    }
                    
                }
                
                //newList = null;
                
            }
            public void Update()
            {
                if (Leaf)
                {
                    //UpdateVegetation(REngine.Instance._camera.Position.vector, Terrain.GrassDistance);

                }
                else
                {
                    if (TreeList != null)
                    {
                        for (int i = 0; i < TreeList.Count; i++)
                        {
                            TreeList[i].Update();
                        }
                    }
                }
            }
            public void DrawVegetation()
            {
                
                    // If this node is a leaf, draw the vegetation
                if (InView && !Occluded)
                {
                    if (Leaf)
                    {
                        
                            //float far = REngine.Instance._camera.zfar;
                            //REngine.Instance._camera.SetClipPlanes(1.0f, Terrain.GrassDistance);
                            Vector3 campos = REngine.Instance._camera.Position.vector;
                            BoundingSphere sphere = new BoundingSphere(REngine.Instance._camera.Position.vector, Terrain.GrassDistance);

                            if (bFrus.Contains(GrassBox) != ContainmentType.Disjoint)
                            {
                                if (sphere.Contains(GrassBox) != ContainmentType.Disjoint)
                                {
                                    //Sort our Vegitation based on distance from camera.  Draw back to front.
                                    //GrassList.Sort(GrassBillboard.DistanceSorter);

                                    Terrain.Device.BlendState = BlendState.AlphaBlend;
                                    
                                    //REngine.Instance._graphics.GraphicsDevice.Indices = indices;

                                    

                                    /*foreach (GrassBillboard g in GrassList)
                                    {
                                        Vector3 Min = new Vector3(g.Position.X - g.width, g.Position.Y, g.Position.Z - g.width);
                                        Vector3 Max = new Vector3(g.Position.X + g.width, g.Position.Y + g.height, g.Position.Z + g.width);
                                        BoundingBox box = new BoundingBox(Min, Max);
                                        ContainmentType type = bFrus.Contains(box);
                                        if (type == ContainmentType.Contains || type == ContainmentType.Intersects)
                                        {
                                            if (Vector3.Distance(campos, g.Position) < Terrain.GrassDistance)
                                            {
                                            
                                                //REngine.Instance._graphics.GraphicsDevice.Vertices[0].SetSource(buffer, 0, VertexBillboard.SizeInBytes);


                                                //((BasicEffect)vegEffect).Projection = REngine.Instance._camera.projMatrix;
                                                //((BasicEffect)vegEffect).View = REngine.Instance._camera.viewMatrix;
                                                //((BasicEffect)vegEffect).World = Matrix.CreateBillboard(position, REngine.Instance._camera.Position.vector, Vector3.Up, REngine.Instance._camera.viewDir);
                                                //((BasicEffect)vegEffect).VertexColorEnabled = true;
                                                //((BasicEffect)vegEffect).Texture = (Texture2D)RTextureFactory.Instance._textureList[textures[0]];

                                            
                                            
                                              g.Render();

                                          
                                            
                                            }
                                        }
                                    }*/

                                    REngine.Instance._game.GraphicsDevice.SetVertexBuffer(grassBuffer);
                                    REngine.Instance._game.GraphicsDevice.Indices = grassIndex;
                                    Effect vegEffect = Terrain.vegEffect;

                                    Terrain.vegEffect.CurrentTechnique = vegEffect.Techniques["Billboards"];
                                    Terrain.vegEffect.Parameters["Distance"].SetValue(Terrain.GrassDistance);
                                    Terrain.vegEffect.Parameters["GrassHeight"].SetValue(Terrain.MinGrassHeight);
                                    Terrain.vegEffect.Parameters["MaxGrassHeight"].SetValue(Terrain.MaxGrassHeight);
                                    if (Terrain._reflected)
                                    {
                                        try
                                        {
                                            Terrain.vegEffect.Parameters["World"].SetValue(Matrix.CreateScale(Terrain.scale) * Terrain._reflection);
                                        }
                                        catch { }
                                    }
                                    else
                                        Terrain.vegEffect.Parameters["World"].SetValue(Matrix.CreateScale(Terrain.scale));
                                    Terrain.vegEffect.Parameters["Heightmap"].SetValue(Terrain.HeightMap);
                                    //vegEffect.Parameters["Normalmap"].SetValue(Terrain.normalRenderTarget.GetTexture());
                                    Terrain.vegEffect.Parameters["textureSize"].SetValue(Terrain.HeightMap.Width);
                                    Terrain.vegEffect.Parameters["elevationStrength"].SetValue(Terrain.ElevationStrength);
                                    Terrain.vegEffect.Parameters["terrainScale"].SetValue(Terrain.scale);
                                    Terrain.vegEffect.Parameters["Grassmap"].SetValue(Terrain.BillboardMap);
                                    Terrain.vegEffect.Parameters["Texture0"].SetValue(Terrain.TerrainVegetation[0]);
                                    Terrain.vegEffect.Parameters["Texture1"].SetValue(Terrain.TerrainVegetation[1]);
                                    Terrain.vegEffect.Parameters["Texture2"].SetValue(Terrain.TerrainVegetation[2]);
                                    Terrain.vegEffect.Parameters["ViewDirection"].SetValue(REngine.Instance._camera.viewDir);
                                    Terrain.vegEffect.Parameters["BillboardWidth"].SetValue(Terrain.Veg0Size.X);
                                    Terrain.vegEffect.Parameters["BillboardHeight"].SetValue(Terrain.Veg0Size.Y);
                                    Terrain.vegEffect.Parameters["View"].SetValue(REngine.Instance._camera.viewMatrix);
                                    Terrain.vegEffect.Parameters["Projection"].SetValue(REngine.Instance._camera.projMatrix);
                                    Terrain.vegEffect.Parameters["Position"].SetValue(new Vector4((OffsetX), 0, (OffsetY), 1.0f));
                                    //vegEffect.Parameters["Position"].SetValue(new Vector4(grassPos,1));
                                    Terrain.vegEffect.Parameters["cameraPosition"].SetValue(campos);
                                    Terrain.vegEffect.Parameters["startFadingInDistance"].SetValue(5000);
                                    Terrain.vegEffect.Parameters["stopFadingInDistance"].SetValue(1);
                                    Terrain.vegEffect.Parameters["Random"].SetValue(1.0f);
                                    Terrain.vegEffect.Parameters["LightDirection"].SetValue(RAtmosphere.Instance.sunDirection);
                                    Terrain.vegEffect.Parameters["LightColor"].SetValue(RAtmosphere.Instance.sunColor);
                                    Terrain.vegEffect.Parameters["WindTime"].SetValue(0.333f*(float)REngine.Instance._gameTime.TotalGameTime.TotalSeconds);
                                    if (RAtmosphere.Instance != null)
                                    {
                                        Terrain.vegEffect.Parameters["FOGENABLE"].SetValue(RAtmosphere.Instance.fogEnabled);
                                        Terrain.vegEffect.Parameters["FOGCOLOR"].SetValue(RAtmosphere.Instance.fogColor);
                                        Terrain.vegEffect.Parameters["FOGDIST"].SetValue(RAtmosphere.Instance.fogDistance);
                                    }
                                    try
                                    {



                                        foreach (EffectPass pass in Terrain.vegEffect.CurrentTechnique.Passes)
                                        {
                                            pass.Apply();
                                            REngine.Instance._graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, grassCount * 4, 0, grassCount * 2);

                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                        REngine.Instance.AddToLog(e.ToString());
                                    }

                                }

                                
                            }
                        //REngine.Instance._camera.SetClipPlanes(1.0f, far);
                    }
                    if (TreeList != null)
                    {
                        for(int i = 0; i<TreeList.Count; i++)
                        {
                            TreeList[i].DrawVegetation();
                        }
                    }
                }
            }
            bool OcclusionRunning = false;
            public void DrawOcclusionQuery()
            {
                InView = false;
                // If this node is a leaf, draw the vegetation
                if (Leaf)
                {
                    try
                    {
                        if (!OcclusionRunning)
                        {
                            //REngine.Instance._game.GraphicsDevice.SetRenderTarget(0, Terrain.renderTarget);
                            OcclusionRunning = true;
                            Query.Begin();

                            lineEffect.View = REngine.Instance._camera.viewMatrix;
                            lineEffect.Projection = REngine.Instance._camera.projMatrix;


                            lineEffect.CurrentTechnique.Passes[0].Apply();

                            // Draw the triangle.

                            Terrain.Device.DrawUserPrimitives(PrimitiveType.TriangleList,
                                                              BoundingBoxMesh, 0, 12);





                            Query.End();
                            //REngine.Instance._game.GraphicsDevice.SetRenderTarget(0, null);
                        }
                        else
                        {
                            if (Query.IsComplete)
                            {
                                OcclusionRunning = false;
                                if (Query.PixelCount > 0)
                                {
                                    InView = true;
                                    Occluded = false;
                                }
                                else
                                {
                                    InView = false;
                                    Occluded = true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    
                }
                else if (TreeList != null)
                {
                    foreach (QuadTree thisTree in TreeList)
                    {
                        thisTree.DrawOcclusionQuery();
                    }
                }
            }
            public void DrawBoundingBox()
            {
                if (InView)
                {
                    // If this node is a leaf, draw the vegetation
                    if (Leaf)
                    {
                        lineEffect.View = REngine.Instance._camera.viewMatrix;
                        lineEffect.Projection = REngine.Instance._camera.projMatrix;


                        lineEffect.CurrentTechnique.Passes[0].Apply();

                        // Draw the triangle.


                        Terrain.Device.DrawUserPrimitives(PrimitiveType.TriangleList,
                                                          BoundingBoxMesh, 0, 12);


                    }
                    else if (TreeList != null)
                    {
                        foreach (QuadTree thisTree in TreeList)
                        {
                            thisTree.DrawBoundingBox();
                        }
                    }
                }
            }

            /// <summary>
            /// This recursive function makes sure that every array of index buffers in each
            /// quad-tree patch have proper pointers. This allows for the user to call any
            /// LOD draw on the terrain and the terrain should draw that LOD or the next highest
            /// LOD possible available.
            /// </summary>
            public void SetupLODs()
            {
                // Only setup LODs for leaves on the tree, never the main tree branches themselves.
                
                if (Leaf)
                {
                    int HighestLODUsed = 0;

                    for (int i = (int)RLANDSCAPE_LOD.NUMOFLODS - 1; i > 0; i--)
                    {
                        if (LeafPatch.IndexBuffers != null)
                            HighestLODUsed = i;
                    }

                    for (int i = 1; i < (int)RLANDSCAPE_LOD.NUMOFLODS; i++)
                    {
                        if (i < HighestLODUsed)
                        {
                            //LeafPatch.IndexBuffers = LeafPatch.IndexBuffers;
                            //LeafPatch.NumTris = LeafPatch.NumTris;
                        }
                        else if (i > HighestLODUsed)
                        {
                            if (LeafPatch.IndexBuffers == null)
                            {
                                //LeafPatch.IndexBuffers = LeafPatch.IndexBuffers;
                                //LeafPatch.NumTris = LeafPatch.NumTris;
                            }
                        }
                    }
                }

                // If there are branches on this node, move down through them recursively
                if (TreeList != null)
                {
                    foreach (QuadTree thisTree in TreeList)
                    {
                        thisTree.SetupLODs();
                    }
                }
            }

            public void AddNewPatchLOD(RLANDSCAPE_LOD DetailLevel)
            {
                // Only setup LODs for leaves on the tree, never the main tree branches themselves.
                if (Leaf)
                {
                    this.LeafPatch.SetupTerrainIndices(Terrain.Device, DetailLevel);

                    SetupLODs();        // Update LOD array to account for new LOD
                }

                // If there are branches on this node, move down through them recursively
                if (TreeList != null)
                {
                    foreach (QuadTree thisTree in TreeList)
                    {
                        thisTree.AddNewPatchLOD(DetailLevel);
                    }
                }

            }

            /// <summary>
            /// Helper function chooses a random location on a triangle.
            /// </summary>
            void PickRandomPoint(Vector3 position1, Vector3 position2, Vector3 position3,
                                 out Vector3 randomPosition)
            {
                Random random = new Random(DateTime.Now.Millisecond);

                float a = (float)random.NextDouble();
                float b = (float)random.NextDouble();

                if (a + b > 1)
                {
                    a = 1 - a;
                    b = 1 - b;
                }

                randomPosition = Vector3.Barycentric(position1, position2, position3, a, b);
            }

            #endregion
        }
    
    public enum RLANDSCAPE_LOD
    {
        NUMOFLODS = 5,
        Low = 8,
        Med = 6,
        High = 4,
        Highest = 2,
        Ultra = 1
    }
    
    internal struct VertexBillboard
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;


        public static int SizeInBytes = (3 + 3 + 2) * 4;
        public static VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
            new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
            new VertexElement( sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0 ),
            
            
        };
    }

    internal class TerrainPatch
    {
        #region Properties
        #endregion

        #region Fields
        RLandscape Terrain;
        QuadTree Parent;

        public IndexBuffer IndexBuffers;

        //public IndexBuffer VegIndexBuffers;

        public int NumTris = 0;
        public int NumVegTris = 0;

        int Width;

        Random random = new Random();
        #endregion

        #region Initialization
        public TerrainPatch(GraphicsDevice Device, RLandscape terrain, QuadTree parent, int Width, RLANDSCAPE_LOD Detail)
        {
            // Create an array index of 4 for each, as 4 is our lowest detail level
            //IndexBuffers = new IndexBuffer(;
            //VegIndexBuffers = new IndexBuffer[(int)RLANDSCAPE_LOD.NUMOFLODS];

            Terrain = terrain;
            Parent = parent;

            

            this.Width = Width;

            // Setup patch with the highest LOD available
            SetupTerrainIndices(Device, Detail);
            /*
            SetupTerrainIndices(Device, (RLANDSCAPE_LOD)((int)Detail * 2)); //2
            SetupTerrainIndices(Device, (RLANDSCAPE_LOD)((int)Detail * 4)); //4
            SetupTerrainIndices(Device, (RLANDSCAPE_LOD)((int)Detail * 8)); //8
             */

            //if (Terrain.DrawVegetation)
                //SetupVegetation(Device, Detail);
        }

        public void SetupTerrainIndices(GraphicsDevice Device)
        {
            // No LOD was chosen in this setup call, so default to High LOD.
            int Detail = (int)RLANDSCAPE_LOD.High;

            int[] Indices = new int[((Width - 1) * (Width - 1) * 6) / (Detail * Detail)];

            for (int x = 0; x < (Width - 1) / Detail; x++)
                for (int y = 0; y < (Width - 1) / Detail; y++)
                {
                    Indices[(x + y * ((Width - 1) / Detail)) * 6] = ((x + 1) + (y + 1) * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 1] = (x + y * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 2] = ((x + 1) + y * Width) * Detail;

                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 3] = ((x + 1) + (y + 1) * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 4] = (x + (y + 1) * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 5] = (x + y * Width) * Detail;
                }
            
            IndexBuffers = new IndexBuffer(Device, typeof(int), (Width - Detail) * (Width - Detail) * 6, BufferUsage.WriteOnly);
            IndexBuffers.SetData(Indices);

            NumTris = Indices.Length / 3;
        }

        public void SetupTerrainIndices(GraphicsDevice Device, RLANDSCAPE_LOD? DetailLevel)
        {
            if (DetailLevel == null)
                DetailLevel = RLANDSCAPE_LOD.High;

            int Detail = (int)DetailLevel;

            // If detail level is smaller than the quad patch, then move up to
            // the next highest detail level.
            if (Detail >= (Width - 1))
                Detail /= 2;

            int[] Indices = new int[((Width - 1) * (Width - 1) * 6) / (Detail * Detail)];

            for (int x = 0; x < (Width - 1) / Detail; x++)
                for (int y = 0; y < (Width - 1) / Detail; y++)
                {
                    Indices[(x + y * ((Width - 1) / Detail)) * 6] = ((x + 1) + (y + 1) * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 1] = (x + y * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 2] = ((x + 1) + y * Width) * Detail;

                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 3] = ((x + 1) + (y + 1) * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 4] = (x + (y + 1) * Width) * Detail;
                    Indices[(x + y * ((Width - 1) / Detail)) * 6 + 5] = (x + y * Width) * Detail;

                }

            IndexBuffers = new IndexBuffer(Device, typeof(int), (Width - Detail) * (Width - Detail) * 6, BufferUsage.WriteOnly);
            IndexBuffers.SetData(Indices);

            NumTris = Indices.Length / 3;
        }

        public void SetupVegetation(GraphicsDevice Device, RLANDSCAPE_LOD? DetailLevel)
        {
            //// Rather than define the vertices here, they need to be defined at the terrain level.
            //// The terrain should only have one set of vegetation.
            //// Then each terrain patch should simply define which vegetation quads it wants to use
            //// which should be dependant upon the LOD. If the LOD is Med (which is 1/2 detail on each
            //// axis), then 1 of every 2^2 (1 in 4) vegetation quads should be added to the vegIndices
            //// for this patch.

            List<int> VegIndices = new List<int>();

            int Detail = (int)DetailLevel;

            /*for (int i = Parent.VegVertBufferOffset; i < Parent.VegVertBufferOffsetEnd; i += (3 * Detail))
            {
                VegIndices.Add(i + 0);
                VegIndices.Add(i + 1);
                VegIndices.Add(i + 2);

                VegIndices.Add(i + 0);
                VegIndices.Add(i + 2);
                VegIndices.Add(i + 3);
            }

            if (VegIndices.Count > 0)
            {
                int[] VegetationIndicesArray = new int[VegIndices.Count];

                for (int i = 0; i < VegIndices.Count; i++)
                {
                    VegetationIndicesArray[i] = VegIndices[i];
                }

                //VegIndexBuffers = new IndexBuffer(Terrain.Device, typeof(int), VegIndices.Count, BufferUsage.WriteOnly);
                //VegIndexBuffers.SetData(VegetationIndicesArray);
            }
            */
            NumVegTris = 0;
        }
        #endregion

        #region Methods

        #endregion
    }

    public class RLandscape : RSceneNode, IDisposable
    {
        int MAX_TERRAIN_SCALE = 2500;
        #region Properties
        #endregion

        #region Fields
#if !XBOX
        //internal Reactor.Physics.PhysicObjects.HeightmapObject _skin;
#endif
        internal GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }
        GraphicsDevice device;

        internal ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }
        ContentManager content;
        ResourceContentManager resource;
        internal VertexBuffer TerrainVertexBuffer;
        internal List<VertexTerrain> TerrainVertices;      // Nullified after terrain patches are initialized.

        // Buffers to hold vegetation info
        internal VertexBuffer VegetationVertexBuffer;
        internal List<VertexBillboard> VegVertices;

        /// <summary>
        /// A camera frustum is used for culling out terrain.
        /// @todo, consider giving this to the camera component instead.
        /// </summary>
        BoundingFrustum CameraFrustum;

        internal int MapWidth;        // Width of the heightmap image
        internal int MapHeight;       // Height of the heightmap image
        internal float ElevationStrength;

        /// <summary>
        /// Holds the height (Z coordinate) of each [x, y] coordinate
        /// </summary>
        internal float[,] HeightData;

        /// <summary>
        /// Holds information about the texture type, which is stored
        /// in each vertex after the vertex buffers are created.
        /// </summary>
        internal Color[,] TerrainTypeData;

        /// <summary>
        /// Holds the normal vectors for each vertex in the terrain.
        /// The normals for lighting are later stored in each vertex, but
        /// we want to store these values permanentally for proper physics
        /// collisions with the ground.
        /// </summary>
        internal Vector3[,] TerrainNormals;

        /// <summary>
        /// Holds information about billboards in on the terrain.
        /// </summary>
        internal int[,] BillboardData;

        internal Texture2D HeightMap;
        Texture2D TerrainMap;
        internal Texture2D BillboardMap;

        internal Texture2D[] TerrainTextures;
        internal Texture2D[] TerrainTextureNormals;

        internal Texture2D[] TerrainVegetation;

        internal Effect effect;
        internal Effect vegEffect;
        internal OcclusionQuery Query;
        internal QuadTree rootQuadTree
        {
            get { return RootQuadTree; }
        }
        QuadTree RootQuadTree;
        internal RenderTarget2D renderTarget;
        // Debug settings
        bool DrawTerrain = true;            // Default: true
        bool DrawBoundingBox = false;       // Default: false
        internal bool DrawVegetation = false;         // Default: true

        // Must be power of two values. ALL Quads in the tree will be set to this size.
        internal int MinimumLeafSize = 64 * 64;
        internal float GrassDistance = 100;
        internal float MinGrassHeight = 0;
        internal float MaxGrassHeight = 0;
        internal int MaxGrassCount = 750;
        /// <summary>
        /// Default terrain level-of-detail
        /// </summary>
        internal RLANDSCAPE_LOD DetailDefault;

        internal RLANDSCAPE_LOD terrainDetail
        {
            get { return Detail; }
        }
        RLANDSCAPE_LOD Detail;

        internal int scale
        {
            get { return Scale; }
        }
        int Scale = 1;
        internal float GrassFactor = 0.1f;
        internal Vector2 Veg0Size = new Vector2(1.0f, 1.0f);
        internal Vector2 Veg1Size = new Vector2(1.0f, 1.0f);
        internal Vector2 Veg2Size = new Vector2(1.0f, 1.0f);

        // Vegetation settings ==================================
        internal bool AutoVegetation = false;

        //internal VegBillboard Grass;
        //internal VegBillboard Tree;
        //internal VegBillboard Brush;
        // ======================================================

        #endregion

        #region Initialization
        internal void CreateLandscape()
        {
            Device = REngine.Instance._graphics.GraphicsDevice;
            Content = new ContentManager(REngine.Instance._game.Services);
            resource = new ResourceContentManager(REngine.Instance._game.Services, Resource1.ResourceManager);
            DetailDefault = RLANDSCAPE_LOD.Ultra;
            Detail = DetailDefault;
#if !XBOX
            effect = resource.Load<Effect>("Landscape");
            vegEffect = resource.Load<Effect>("Grass");
#else
            effect = resource.Load<Effect>("Landscape1");
            vegEffect = resource.Load<Effect>("Grass1");
#endif
            TerrainVertices = new List<VertexTerrain>();
            //VegVertices = new List<VertexBillboard>();
        }
        internal RenderTarget2D normalRenderTarget;
        SpriteBatch normalSpriteBatch;
        private void ComputeNormalMap()
        {
            GraphicsDevice graphicsDevice = Device;

            RenderTarget2D oldRenderTarget = graphicsDevice.GetRenderTargets()[0].RenderTarget as RenderTarget2D;
            

            normalSpriteBatch = new SpriteBatch(Device);

            normalRenderTarget = new RenderTarget2D(graphicsDevice, HeightMap.Width,
                HeightMap.Height);

           

            graphicsDevice.SetRenderTarget(normalRenderTarget);

            graphicsDevice.Clear(Color.White);

            normalSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            effect.Parameters["HeightMap"].SetValue(HeightMap);
            effect.Parameters["textureSize"].SetValue(HeightMap.Width);
            effect.Parameters["normalStrength"].SetValue(ElevationStrength);
            effect.CurrentTechnique = effect.Techniques["ComputeNormals"];

            effect.CurrentTechnique.Passes[0].Apply();

            normalSpriteBatch.Draw(HeightMap, new Rectangle(0, 0, HeightMap.Width, HeightMap.Height),
                                   Color.Black);



            normalSpriteBatch.End();
            graphicsDevice.SetRenderTarget(oldRenderTarget);

            //normalRenderTarget.GetTexture().Save("height.jpg", ImageFileFormat.Jpg);
            

            for (int i = 0; i < 12; i++)
            {
                Device.SamplerStates[i].Filter = TextureFilter.Anisotropic;
                Device.SamplerStates[i].AddressU = TextureAddressMode.Wrap;
                Device.SamplerStates[i].AddressV = TextureAddressMode.Wrap;
                Device.SamplerStates[i].AddressW = TextureAddressMode.Wrap;
            }
        }
        internal void CreateLandscape(RLANDSCAPE_LOD? DetailDefault)
        {
            if (DetailDefault == null)
                DetailDefault = RLANDSCAPE_LOD.Ultra;  // Default to a high detail level for terrain

            Device = REngine.Instance._graphics.GraphicsDevice;
            Content = new ContentManager(REngine.Instance._game.Services);
            resource = new ResourceContentManager(REngine.Instance._game.Services, Resource1.ResourceManager);

            this.DetailDefault = (RLANDSCAPE_LOD)DetailDefault;
            Detail = this.DetailDefault;
#if !XBOX
            effect = resource.Load<Effect>("Landscape");
            vegEffect = resource.Load<Effect>("Grass");
#else
            effect = resource.Load<Effect>("Landscape1");
            vegEffect = resource.Load<Effect>("Grass1");
#endif

            TerrainVertices = new List<VertexTerrain>();
            //VegVertices = new List<VertexBillboard>();
        }

        /// <summary>
        /// Initialize main terrain settings.
        /// </summary>
        /// <param name="HeightImage">TextureID for heightmap image, image must be a power of 2 in height and width</param>
        /// <param name="TerrainTexture">TextureID for terrain texture image, image must be a power of 
        /// 2 in height and width. Texture image defines where multi-texture splatting occurs. You can use
        /// this to draw out paths or sections in the terrain.</param>
        /// <param name="scale">Scale (size) of terrain (1-4)</param>
        /// <param name="SmoothingPasses">Smoothes out the terrain using averages of height. The number of
        /// smoothing passes you choose to make is up to you. If you have sharp
        /// elevations on your map, you have the elevation strength turned up
        /// then you may want a higher value. If your terrain is already smooth
        /// or has very small elevation strength you may not need any passes.
        /// Default value is 5. Use value of 0 to skip smoothing.</param>
        public void Initialize(int HeightImage, int TerrainTexture, int scale, int? SmoothingPasses)
        {
            if (SmoothingPasses < 0)
                SmoothingPasses = 10;

            HeightMap = (Texture2D)RTextureFactory.Instance._textureList[HeightImage];
            TerrainMap = (Texture2D)RTextureFactory.Instance._textureList[TerrainTexture];

            if (scale < 1)
                scale = 1;
            else if (scale > MAX_TERRAIN_SCALE)
                scale = MAX_TERRAIN_SCALE;

            this.Scale = scale;
            _world = Matrix.CreateScale(scale);
            LoadHeightData();
            LoadTerrainTypeData();

            if (DrawVegetation)
                LoadBillboardData(true);

            SmoothTerrain((int)SmoothingPasses);
            SetupTerrainNormals();

            renderTarget = new RenderTarget2D(REngine.Instance._game.GraphicsDevice, 256, 256);
            RootQuadTree = new QuadTree(this, TerrainNormals.Length);
            TerrainTypeData = null;     // Free terrain data to GC now that each quad section has its own.

            SetupTerrainVertexBuffer(); // Quad-tree sections have setup the vertex list, now this creates a buffer.
            if (DrawVegetation)
                SetupTerrainVegVertexBuffer();
            TerrainVertices.Clear();
            Query = new OcclusionQuery(REngine.Instance._game.GraphicsDevice);
            //ComputeNormalMap();
        }
        bool IsPowerOf2(float value)
        {
            if ((value % 2) == 0)
                return true;
            else
                return false;
        }
        
        public void SetMinGrassPlotElevation(float Elevation)
        {
            MinGrassHeight = Elevation;
        }
        public void SetMaxGrassPlotElevation(float Elevation)
        {
            MaxGrassHeight = Elevation;
        }
        
        // <summary>
        /// Initialize main terrain settings. Default smoothing settings will be used.
        /// </summary>
        /// <param name="HeightImage">TextureID for heightmap image, image must be a power of 2 in height and width</param>
        /// <param name="TerrainTexture">TextureID for terrain texture image, image must be a power of 
        /// 2 in height and width. Texture image defines where multi-texture splatting occurs. You can use
        /// this to draw out paths or sections in the terrain.</param>
        public void Initialize(int HeightImage, int TerrainTexture, int scale)
        {
            HeightMap = (Texture2D)RTextureFactory.Instance._textureList[HeightImage];
            TerrainMap = (Texture2D)RTextureFactory.Instance._textureList[TerrainTexture];

            if (!IsPowerOf2(HeightMap.Width) && !IsPowerOf2(HeightMap.Height))
                throw new Exception("Height maps must have a width and height that is a power of two.");

            if (!IsPowerOf2(TerrainMap.Width) && !IsPowerOf2(TerrainMap.Height))
                throw new Exception("Terrain maps must have a width and height that is a power of two.");

            if (scale < 1)
                scale = 1;
            else if (scale > MAX_TERRAIN_SCALE)
                scale = MAX_TERRAIN_SCALE;

            this.Scale = scale;
            _world = Matrix.CreateScale(this.Scale);
            LoadHeightData();
            LoadTerrainTypeData();

            if (DrawVegetation)
                LoadBillboardData(true);

            SmoothTerrain(10);          // Default of 10 smoothing passes
            SetupTerrainNormals();

            RootQuadTree = new QuadTree(this, TerrainNormals.Length);
            TerrainTypeData = null;     // Free terrain data to GC now that each quad section has its own.

            SetupTerrainVertexBuffer(); // Quad-tree sections have setup the vertex list, now this creates a buffer.
            if(DrawVegetation)
                SetupTerrainVegVertexBuffer();
            TerrainVertices.Clear();
            Query = new OcclusionQuery(REngine.Instance._game.GraphicsDevice);
            //ComputeNormalMap();
        }
        public void SetPosition(R3DVECTOR Position)
        {
            this.Position = Position.vector;
            _world.Translation = Position.vector;
        }
        public void SetPosition(float x, float y, float z)
        {
            SetPosition(new R3DVECTOR(x, y, z));
        }
        /// <summary>
        /// Sets up 3 different textures that the terrain can use.
        /// </summary>
        /// <param name="Texture0">Terrain texture #0</param>
        /// <param name="Texture1">Terrain texture #1</param>
        /// <param name="Texture2">Terrain texture #2</param>
        public void InitTerrainTextures(int Texture0, int Texture1, int Texture2)
        {
            TerrainTextures = new Texture2D[3];

            TerrainTextures[0] = (Texture2D)RTextureFactory.Instance._textureList[Texture0];
            TerrainTextures[1] = (Texture2D)RTextureFactory.Instance._textureList[Texture1];
            TerrainTextures[2] = (Texture2D)RTextureFactory.Instance._textureList[Texture2];
        }

        public void InitTerrainNormalsTextures(int Texture0Normal, int Texture1Normal, int Texture2Normal)
        {
            TerrainTextureNormals = new Texture2D[3];

            TerrainTextureNormals[0] = (Texture2D)RTextureFactory.Instance._textureList[Texture0Normal];
            TerrainTextureNormals[1] = (Texture2D)RTextureFactory.Instance._textureList[Texture1Normal];
            TerrainTextureNormals[2] = (Texture2D)RTextureFactory.Instance._textureList[Texture2Normal];
        }
        
        
        public void InitTerrainGrass(int VegTexture0, int GrassMapID, float GrassDrawDistance)
        {
            InitTerrainGrass(VegTexture0, VegTexture0, VegTexture0, GrassMapID, GrassDrawDistance);
        }
        public void InitTerrainGrass(int VegTexture0, int VegTexture1, int GrassMapID, float GrassDrawDistance)
        {
            InitTerrainGrass(VegTexture0, VegTexture1, VegTexture0, GrassMapID, GrassDrawDistance);
        }
        public void InitTerrainGrass(int VegTexture0, int VegTexture1, int VegTexture2, int GrassMapID, float GrassDrawDistance)
        {
            TerrainVegetation = new Texture2D[3];
            TerrainVegetation[0] = (Texture2D)RTextureFactory.Instance._textureList[VegTexture0];
            TerrainVegetation[1] = (Texture2D)RTextureFactory.Instance._textureList[VegTexture1];
            TerrainVegetation[2] = (Texture2D)RTextureFactory.Instance._textureList[VegTexture2];
            GrassDistance = GrassDrawDistance;

            BillboardMap = (Texture2D)RTextureFactory.Instance._textureList[GrassMapID];

            if (!IsPowerOf2(BillboardMap.Width) && !IsPowerOf2(BillboardMap.Height))
                throw new Exception("Grass Maps must have a width and height that is a power of two.");

            DrawVegetation = true;
            //LoadBillboardData(true);
        }
        public void SetTerrainGrassSize(float VegWidth0, float VegHeight0)
        {
            SetTerrainGrassSize(VegWidth0, VegHeight0, VegWidth0, VegHeight0, VegWidth0, VegHeight0);
        }
        public void SetTerrainGrassSize(float VegWidth0, float VegHeight0, float VegWidth1, float VegHeight1)
        {
            SetTerrainGrassSize(VegWidth0, VegHeight0, VegWidth1, VegHeight1, VegWidth1, VegHeight1);
        }
        public void SetTerrainGrassSize(float VegWidth0, float VegHeight0, float VegWidth1, float VegHeight1, float VegWidth2, float VegHeight2)
        {
            Veg0Size = new Vector2(VegWidth0, VegHeight0);
            Veg1Size = new Vector2(VegWidth1, VegHeight1);
            Veg2Size = new Vector2(VegWidth2, VegHeight2);
        }
        /// <summary>
        /// Loads in the height data using a height map image.
        /// </summary>
        private void LoadHeightData()
        {
            float minimumHeight = 1000;             // Set a high number because this will go drop with the math below
            float maximumHeight = 0;                // Opposite of line above

            MapWidth = HeightMap.Width;             // Sets the map width to the same as the heightmap texture.
            MapHeight = HeightMap.Height;           // Same as line above

            // We setup the map for colors so we can use the color to determine elevations of the map
            Color[] heightMapColors = new Color[MapWidth * MapHeight];

            HeightMap.GetData(heightMapColors);     // XNA Built-in feature automatically copies
            // pixel data into the heightmap.

            HeightData = new float[MapWidth, MapHeight];  // Create an array to hold elevations from heightMap

            // If elevation strength was never initialized, use 6 by default.
            if (ElevationStrength < 0.0f)
                ElevationStrength = 6;

            // Find minimum and maximum values for the heightmap file we read in
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                {
                    HeightData[x, y] = heightMapColors[x + y * MapWidth].R;
                    if (HeightData[x, y] < minimumHeight) minimumHeight = HeightData[x, y];
                    if (HeightData[x, y] > maximumHeight) maximumHeight = HeightData[x, y];
                }


            // Set height by color, and then alter height by min and max amounts
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                {
                    HeightData[x, y] = (heightMapColors[y + x * MapHeight].R) + (heightMapColors[y + x * MapHeight].G) + (heightMapColors[y + x * MapHeight].B);
                    HeightData[x, y] = (HeightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * ElevationStrength * Scale;
                }
        }

        private void LoadTerrainTypeData()
        {
            // We setup the map with colors so we can splat terrain in the quad-tree sections.
            Color[] TerrainTypeColors = new Color[MapWidth * MapHeight];

            TerrainMap.GetData(TerrainTypeColors);        // XNA Built-in feature automatically copies
            // pixel data into the heightmap.

            TerrainTypeData = new Color[MapWidth, MapHeight];

            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                {
                    TerrainTypeData[x, y] = new Color(TerrainTypeColors[y + x * MapWidth].R,
                                                      TerrainTypeColors[y + x * MapWidth].G,
                                                      TerrainTypeColors[y + x * MapWidth].B);
                }
        }

        /// <summary>
        /// Loads the billboard types.
        /// </summary>
        /// <param name="AutoVegetation">If true then billboards will automatically occur with a
        /// set frequency in any green area on the billboard map. Otherwise they will occur according
        /// to red, green, and blue areas on the billboard map.</param>
        private void LoadBillboardData(bool AutoVegetation)
        {
            //Grass.Frequency = 50;
            //Grass.Height = 4;
            //Grass.Width = 4;
            //Grass.WindAmount = 5;
            //Grass.IDNum = 0;

            //Tree.Frequency = 6;
            //Tree.Height = 12;
            //Tree.Width = 12;
            //Tree.WindAmount = 0;
            //Tree.IDNum = 1;

            //Brush.Frequency = 10;
            //Brush.Height = 7;
            //Brush.Width = 7;
            //Brush.WindAmount = 3;
            //Brush.IDNum = 2;

            // We setup the map for colors so we can check later for billboard information
            Color[] BillboardColors = new Color[MapWidth * MapHeight];

            BillboardMap.GetData(BillboardColors);        // XNA Built-in feature automatically copies
            // pixel data into the heightmap.

            this.BillboardData = new int[MapWidth, MapHeight];

            
            Random random = new Random();

            // Determine which billboard will be used by the strongest color
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                {
                    float Sum = BillboardColors[y + x * MapHeight].R +
                                BillboardColors[y + x * MapHeight].G +
                                BillboardColors[y + x * MapHeight].B;
                    
                    // This process can be re-done in a better way I am sure
                    // This is temporary just to get random vegatation tested.
                    
                    
                        if (BillboardColors[y + x * MapHeight].R >= BillboardColors[y + x * MapHeight].G)
                        {
                            if (BillboardColors[y + x * MapHeight].R >= BillboardColors[y + x * MapHeight].B)
                            {
                                if (Sum > 10.0f)
                                    this.BillboardData[x, y] = 0;    // Billboard texture0 will be used
                                else
                                    this.BillboardData[x, y] = -1;    // No vegetation will be used in this location
                            }
                        }
                        else if (BillboardColors[y + x * MapHeight].G >= BillboardColors[y + x * MapHeight].B)
                        {
                            if (Sum > 10.0f)
                                this.BillboardData[x, y] = 1;    // Billboard texture1 will be used
                            else
                                this.BillboardData[x, y] = -1;    // No vegetation will be used in this location
                        }
                        else
                        {
                            if (Sum > 10.0f)
                                this.BillboardData[x, y] = 2;    // Billboard texture2 will be used
                            else
                                this.BillboardData[x, y] = -1;    // No vegetation will be used in this location
                        }

                        
                    
                         
                }

            
            
        }

        /// <summary>
        /// This sets up the vertices for all of the triangles.
        /// </summary>
        private void SetupTerrainNormals()
        {
            VertexTerrain[] TerrainVertices = new VertexTerrain[MapWidth * MapHeight];
            TerrainNormals = new Vector3[MapWidth, MapHeight];

            // Determine vertex positions so we can figure out normals in section below.
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                {
                    TerrainVertices[x + y * MapWidth].Position = new Vector3(x * Scale, HeightData[x, y], y * Scale);
                }

            // Setup normals for lighting and physics (Credit: Riemer's method)
            for (int x = 1; x < MapWidth-1; x++)
                for (int y = 1; y < MapHeight-1; y++)
                {
                    Vector3 X = TerrainVertices[y * MapWidth + x].Position - TerrainVertices[y * MapWidth + x + 1].Position;
                    Vector3 Z = TerrainVertices[(y) * MapWidth + x].Position - TerrainVertices[(y + 1) * MapWidth + x].Position;
                    Vector3 normal = Vector3.Cross(Z, X);
                    //Normal.Normalize();
                    Vector3 normX = new Vector3((TerrainVertices[x - 1 + y * MapWidth].Position.Y - TerrainVertices[x + 1 + y * MapWidth].Position.Y) / 2, 1, 0);
                    Vector3 normY = new Vector3(0, 1, (TerrainVertices[x + (y - 1) * MapWidth].Position.Y - TerrainVertices[x + (y + 1) * MapWidth].Position.Y) / 2);
                    //TerrainVertices[x + y * MapWidth].Normal = normX + normY;
                    TerrainVertices[x + y * MapWidth].Normal = normal;
                    TerrainVertices[x + y * MapWidth].Normal.Normalize();
                    
                    TerrainNormals[x, y] = TerrainVertices[x + y * MapWidth].Normal;    // Stored for use in physics and for the
                    // quad-tree component to reference.
                }
        }

        private void SetupTerrainVertexBuffer()
        {


            TerrainVertexBuffer = new VertexBuffer(Device, dec, VertexTerrain.SizeInBytes * (MapWidth + (MapWidth / (int)(Math.Sqrt(MinimumLeafSize)))) * (MapHeight + (MapHeight / (int)(Math.Sqrt(MinimumLeafSize)))), BufferUsage.WriteOnly);
            TerrainVertexBuffer.SetData(TerrainVertices.ToArray());

            
        }
        private void SetupTerrainVegVertexBuffer()
        {

            

        }


        #endregion

        #region Methods
        internal void OcclusionRender()
        {
            REngine.Instance._game.GraphicsDevice.BlendState = BlendState.Opaque;
            REngine.Instance._game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            REngine.Instance._game.GraphicsDevice.SetRenderTarget(renderTarget);
            REngine.Instance._game.GraphicsDevice.Clear(Color.Black);
            RootQuadTree.DrawOcclusionQuery();
            REngine.Instance._game.GraphicsDevice.SetRenderTarget(null);
        }
        public override void Render()
        {


            REngine.Instance._game.GraphicsDevice.BlendState = BlendState.Opaque;
            REngine.Instance._game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            
            try
            {
                
                Draw();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                REngine.Instance.AddToLog(e.ToString());
            }
            
        }
        public void RenderGrass()
        {
            
            DrawVegitation();
        }
        internal Matrix _world = Matrix.Identity;
        public R3DMATRIX GetMatrix()
        {
            return R3DMATRIX.FromMatrix(_world);

        }
        internal Matrix _reflection;
        internal bool _reflected = false;
        public void SetReflectionMatrix(R3DMATRIX Reflection)
        {
            _reflected = true;
            _reflection = Reflection.matrix;
        }
        public void ResetReflectionMatrix()
        {
            _reflected = false;
            
        }
        public void SetMatrix(R3DMATRIX Matrix)
        {
            _world = Matrix.matrix;
        }
        VertexDeclaration dec;
        internal void Draw()
        {
            CameraFrustum = new BoundingFrustum(REngine.Instance._camera.viewMatrix * REngine.Instance._camera.projMatrix);

            if (DrawTerrain)
            {
                if(dec == null)
                    dec = new VertexDeclaration(VertexTerrain.VertexElements);
                Device.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
                Device.SetVertexBuffer(TerrainVertexBuffer);
                
                
                    effect.CurrentTechnique = effect.Techniques["MultiTexturedNormaled"];
                    effect.Parameters["GrassNormal"].SetValue(TerrainTextureNormals[0]);
                    effect.Parameters["SandNormal"].SetValue(TerrainTextureNormals[1]);
                    effect.Parameters["RockNormal"].SetValue(TerrainTextureNormals[2]);
                
                    //effect.CurrentTechnique = effect.Techniques["MultiTextured"];

                effect.Parameters["GrassTexture"].SetValue(TerrainTextures[0]);
                effect.Parameters["SandTexture"].SetValue(TerrainTextures[1]);
                effect.Parameters["RockTexture"].SetValue(TerrainTextures[2]);

                effect.Parameters["TerrainWidth"].SetValue(this.MapWidth);
                effect.Parameters["TerrainScale"].SetValue(this.Scale);
                effect.Parameters["CameraForward"].SetValue(REngine.Instance._camera.viewDir);
                effect.Parameters["TextureMap"].SetValue(TerrainMap);
                //effect.Parameters["NormalMap"].SetValue(normalRenderTarget.GetTexture());
                Vector3 lightDir = RAtmosphere.Instance.sunDirection;
                _world = Matrix.CreateTranslation(this.Position);
                if (_reflected)
                {
                    Device.RasterizerState.CullMode = CullMode.None;
                    if (RWater.water.World.Translation.Y < REngine.Instance._camera.Position.Y)
                        Device.RasterizerState.CullMode = CullMode.None;
                    _world *= _reflection;
                    lightDir.Y *= -1;
                }
                effect.Parameters["xWorld"].SetValue(_world);
                effect.Parameters["xView"].SetValue(REngine.Instance._camera.viewMatrix);
                effect.Parameters["xProjection"].SetValue(REngine.Instance._camera.projMatrix);
                effect.Parameters["WorldViewProj"].SetValue(_world * REngine.Instance._camera.viewMatrix * REngine.Instance._camera.projMatrix);
                effect.Parameters["WorldView"].SetValue(_world * REngine.Instance._camera.viewMatrix);
                effect.Parameters["LightDirection"].SetValue(lightDir);
                effect.Parameters["AmbientColor"].SetValue(RAtmosphere.Instance.ambientColor);
                effect.Parameters["AmbientPower"].SetValue(1f);
                effect.Parameters["SpecularColor"].SetValue(RAtmosphere.Instance.sunColor * 0.2f);
                effect.Parameters["SpecularPower"].SetValue(0.5f);
                effect.Parameters["DiffuseColor"].SetValue(RAtmosphere.Instance.sunColor);
                if (RAtmosphere.Instance != null)
                {
                        effect.Parameters["FOGENABLE"].SetValue(RAtmosphere.Instance.fogEnabled);
                        effect.Parameters["FOGCOLOR"].SetValue(RAtmosphere.Instance.fogColor);
                        effect.Parameters["FOGDIST"].SetValue(RAtmosphere.Instance.fogDistance);
                }
                RootQuadTree.Draw(ref CameraFrustum, Detail);
            }
            
            
            
            if (DrawBoundingBox)
            {
                Device.RasterizerState.FillMode = FillMode.WireFrame;
                Device.RasterizerState.CullMode = CullMode.None;
                REngine.Instance._game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            

                RootQuadTree.DrawBoundingBox();
                Device.RasterizerState.FillMode = FillMode.Solid;
            }
        }
        internal void DrawVegitation()
        {
            if (DrawVegetation)
            {
                RootQuadTree.DrawVegetation();
            }
        }
        public void ToggleTerrainDraw()
        {
            DrawTerrain = !DrawTerrain;
        }

        public void ToggleBoundingBoxDraw()
        {
            DrawBoundingBox = !DrawBoundingBox;
        }

        public void SetElevationStrength(float Strength)
        {
            ElevationStrength = Strength;
        }

        /// <summary>
        /// Smoothes out the terrain using averages of height. The number of
        /// smoothing passes you choose to make is up to you. If you have sharp
        /// elevations on your map, you have the elevation strength turned up
        /// then you may want a higher value. If your terrain is already smooth
        /// or has very small elevation strength you may not need any passes.        
        /// </summary>
        /// <param name="SmoothingPasses">Number of smoothing passes to make</param>
        public void SmoothTerrain(int Passes)
        {
            float[,] newHeightData;

            while (Passes > 0)
            {
                Passes--;
                newHeightData = new float[MapWidth, MapHeight];

                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        int adjacentSections = 0;
                        float sectionsTotal = 0.0f;

                        if ((x - 1) > 0)            // Check to left
                        {
                            sectionsTotal += HeightData[x - 1, y];
                            adjacentSections++;

                            if ((y - 1) > 0)        // Check up and to the left
                            {
                                sectionsTotal += HeightData[x - 1, y - 1];
                                adjacentSections++;
                            }

                            if ((y + 1) < MapHeight)        // Check down and to the left
                            {
                                sectionsTotal += HeightData[x - 1, y + 1];
                                adjacentSections++;
                            }
                        }

                        if ((x + 1) < MapWidth)     // Check to right
                        {
                            sectionsTotal += HeightData[x + 1, y];
                            adjacentSections++;

                            if ((y - 1) > 0)        // Check up and to the right
                            {
                                sectionsTotal += HeightData[x + 1, y - 1];
                                adjacentSections++;
                            }

                            if ((y + 1) < MapHeight)        // Check down and to the right
                            {
                                sectionsTotal += HeightData[x + 1, y + 1];
                                adjacentSections++;
                            }
                        }

                        if ((y - 1) > 0)            // Check above
                        {
                            sectionsTotal += HeightData[x, y - 1];
                            adjacentSections++;
                        }

                        if ((y + 1) < MapHeight)    // Check below
                        {
                            sectionsTotal += HeightData[x, y + 1];
                            adjacentSections++;
                        }

                        newHeightData[x, y] = (HeightData[x, y] + (sectionsTotal / adjacentSections)) * 0.5f;
                    }
                }

                // Overwrite the HeightData info with our new smoothed info
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        HeightData[x, y] = newHeightData[x, y];
                    }
                }
            }
        }

        public float GetTerrainHeight(float XPos, float ZPos)
        {
            // we first get the height of 4 points of the quad underneath the point
            // Check to make sure this point is not off the map at all
            XPos = (XPos - this.Position.X) / Scale;
            ZPos = (ZPos - this.Position.Z) / Scale;
            int x = (int)XPos;
            if (x > MapWidth - 2)
                return -10000;      // Terrain height is considered -10000 (or any really low number will do)
            // if it is outside the heightmap.
            else if (x < 0)
                return -10000;

            int y = (int)ZPos;
            if (y > MapHeight - 2)
                return -10000;
            else if (y < 0)
                return -10000;

            float TriY0 = (HeightData[x, y]);
            float TriY1 = (HeightData[x + 1, y]);
            float TriY2 = (HeightData[x, y + 1]);
            float TriY3 = (HeightData[x + 1, y + 1]);

            float Height;
            float SqX = (XPos) - x;
            float SqY = (ZPos) - y;
            if ((SqX + SqY) < 1)
            {
                Height = TriY0;
                Height += (TriY1 - TriY0) * SqX;
                Height += (TriY2 - TriY0) * SqY;
            }
            else
            {
                Height = TriY3;
                Height += (TriY1 - TriY3) * (1.0f - SqY);
                Height += (TriY2 - TriY3) * (1.0f - SqX);
            }
            return (Height + this.Position.Y);
        }

        public bool IsAboveTerrain(float xPos, float yPos)
        {
            // Keep object from going off the edge of the map
            if ((xPos / Scale) > MapWidth)
                return false;
            else if ((xPos / Scale) < 0)
                return false;

            // Keep object from going off the edge of the map
            if ((yPos / Scale) > MapHeight)
                return false;
            else if ((yPos / Scale) < 0)
                return false;

            return true;
        }

        // Old way of finding terrain normals (less accurate)
        //public Vector3 GetNormal(float fTerX, float fTerY)
        //{
        //    int x = (int)(fTerX / Scale);
        //    if (x > MapWidth - 2)
        //        x = MapWidth - 2;
        //    else if (x < 0)
        //        x = 0;

        //    int y = (int)(fTerY / Scale);
        //    if (y > MapHeight - 2)
        //        y = MapHeight - 2;
        //    else if (y < 0)
        //        y = 0;

        //    return TerrainNormals[x, y];
        //}

        /// <summary>
        /// Gets the normal of a position on the heightmap.
        /// </summary>
        /// <param name="XPos">X position on the map</param>
        /// <param name="YPos">Y position on the map</param>
        /// <returns>Normal vector of this spot on the terrain</returns>
        public Vector3 GetNormal(float XPos, float YPos)
        {
            int x = (int)(XPos / Scale);
            if (x > MapWidth - 2)
                x = MapWidth - 2;
            // if it is outside the heightmap.
            else if (x < 0)
                x = 0;

            int y = (int)(YPos / Scale);
            if (y > MapHeight - 2)
                y = MapHeight - 2;
            else if (y < 0)
                y = 0;

            Vector3 TriY0 = (TerrainNormals[x, y]);
            Vector3 TriY1 = (TerrainNormals[x + 1, y]);
            Vector3 TriY2 = (TerrainNormals[x, y + 1]);
            Vector3 TriY3 = (TerrainNormals[x + 1, y + 1]);

            Vector3 AvgNormal;
            float SqX = (XPos / Scale) - x;
            float SqY = (YPos / Scale) - y;
            if ((SqX + SqY) < 1)
            {
                AvgNormal = TriY0;
                AvgNormal += (TriY1 - TriY0) * SqX;
                AvgNormal += (TriY2 - TriY0) * SqY;
            }
            else
            {
                AvgNormal = TriY3;
                AvgNormal += (TriY1 - TriY3) * (1.0f - SqY);
                AvgNormal += (TriY2 - TriY3) * (1.0f - SqX);
            }
            return AvgNormal;
        }

        /// <summary>
        /// Sets the minimum leaf size for quad-tree patches. Must be a power of two value.
        /// </summary>
        /// <param name="Width">Minimum leaf size width (also sets height to match)</param>
        public void SetLeafSize(int Width)
        {
            if (IsPowerOf2(Width))
                MinimumLeafSize = Width * Width;    // All maps must be square, so only width is needed
            else
                throw new Exception("Leaf size must be set to a power of two");
        }

        public void SetDetailLevel(RLANDSCAPE_LOD DetailLevel)
        {
            Detail = DetailLevel;
        }

        public void SetupLODS()
        {
            RootQuadTree.SetupLODs();
        }

        public void AddNew(RLANDSCAPE_LOD DetailLevel)
        {
            RootQuadTree.AddNewPatchLOD(DetailLevel);
        }
        public void Dispose()
        {
            this.BillboardData = null;
            this.BillboardMap.Dispose();
            this.content.Dispose();
            this.effect.Dispose();
            this.HeightData = null;
            
        }
        #endregion
#if !XBOX
        /*public void SetPhysicsEnable(bool Enable)
        {
            if (Enable)
            {
                _skin = new Reactor.Physics.PhysicObjects.HeightmapObject(this, REngine.Instance._game);
            }
            else
            {
                
                if (_skin != null)
                {
                    _skin.Dispose();
                }
            }
            
        }*/
#endif
        public override void Update()
        {
#if !XBOX
            //if(_skin != null)
            //_skin.Update(REngine.Instance._gameTime);
#endif
            
            rootQuadTree.Update();
            base.Update();
        }
        internal static int GetVertexElementSize(VertexDeclaration decl, int elementIndex)
        {
            int elemSize = 0;

            VertexElement[] elems = decl.GetVertexElements();

            if (elems.Length > elementIndex)
            {
                VertexElement e = elems[elementIndex];

                //get the next element if there is one
                int iNextElem = -1;
                if (elems.Length > elementIndex + 1)
                {
                    VertexElement tmp = elems[elementIndex + 1];

                    if (tmp == e)
                    {
                        iNextElem = elementIndex + 1;
                    }
                }

                if (iNextElem != -1)
                {
                    elemSize = elems[iNextElem].Offset - e.Offset;
                }
                else
                {
                    elemSize =
                       decl.VertexStride - e.Offset;
                }
            }

            return elemSize;
        }

        internal static int GetVertexElementByteOffset(VertexDeclaration decl,
                                        VertexElementUsage usage,
                                        byte usageIndex,
                                        int stream)
        {
            VertexElement[] elems = decl.GetVertexElements();
            for (int x = 0; x < elems.Length; x++)
            {
                if (elems[x].VertexElementUsage == usage &&
                    elems[x].UsageIndex == usageIndex)
                {
                    return elems[x].Offset;
                }
            }
            return -1;
        }
        internal static void ConvertVB(ref VertexBuffer vb,
                          VertexDeclaration fromDecl,
                          VertexDeclaration toDecl)
        {
            ConvertVB(vb, fromDecl, 0, toDecl, 0);
        }

        internal static void ConvertVB(ref VertexBuffer vb,
                              VertexDeclaration fromDecl,
                              int fromStreamIndex,
                              VertexDeclaration toDecl,
                              int toStreamIndex)
        {
            vb = ConvertVB(vb,
                           fromDecl,
                           fromStreamIndex,
                           toDecl,
                           toStreamIndex);
        }

        internal static VertexBuffer ConvertVB(VertexBuffer vb,
                              VertexDeclaration fromDecl,
                              VertexDeclaration toDecl)
        {
            return ConvertVB(vb,
                             fromDecl,
                             0,
                             toDecl,
                             0);
        }

        internal static VertexBuffer ConvertVB(VertexBuffer vb,
                              VertexDeclaration fromDecl,
                              int fromStreamIndex,
                              VertexDeclaration toDecl,
                              int toStreamIndex)
        {
            byte[] fromData = new byte[vb.VertexCount * fromDecl.VertexStride];
            vb.GetData<byte>(fromData);

            int fromNumVertices = vb.VertexCount;

            List<int> vertMap = new List<int>();

            //find mappings
            for (int x = 0; x < fromDecl.GetVertexElements().Length; x++)
            {
                VertexElement thisElem = fromDecl.GetVertexElements()[x];

                bool bFound = false;

                int i = 0;
                for (i = 0; i < toDecl.GetVertexElements().Length; i++)
                {
                    VertexElement elem = toDecl.GetVertexElements()[i];

                    
                        if (thisElem.VertexElementUsage == elem.VertexElementUsage &&
                            thisElem.UsageIndex == elem.UsageIndex &&
                            thisElem.VertexElementFormat == elem.VertexElementFormat)
                        {
                            bFound = true;
                            break;
                        }
                }
                if (bFound)
                {
                    vertMap.Add(i);
                }
                else
                {
                    vertMap.Add(-1);
                }
            }


            int newBufferSize = fromNumVertices *
                                    toDecl.VertexStride;



            byte[] toData = new byte[newBufferSize];

            int toDeclVertexStride = toDecl.VertexStride;
            int fromDeclVertexStride = fromDecl.VertexStride;

            for (int x = 0; x < vertMap.Count; x++)
            {
                int i = vertMap[x];

                if (i != -1)
                {
                    VertexElement fromElem = fromDecl.GetVertexElements()[x];
                    VertexElement toElem = toDecl.GetVertexElements()[i];

                    for (int k = 0; k < fromNumVertices; k++)
                    {
                        for (int j = 0; j < GetVertexElementSize(fromDecl, x); j++)
                        {
                            toData[k * toDeclVertexStride + toElem.Offset + j] =
                                fromData[k * fromDeclVertexStride + fromElem.Offset + j];
                        }
                    }
                }
            }

            VertexBuffer newVB = new VertexBuffer(
                vb.GraphicsDevice,
                toDecl,
                fromNumVertices,
                BufferUsage.None); // in xna 1.0 use vb.ResourceUsage instead

            newVB.SetData<byte>(toData);

            return newVB;
        }
        internal static void CalculateTangents(IndexBuffer IB,
                                     ref VertexBuffer fromVB,
                                     ref VertexDeclaration fromDecl,
                                     bool calcTangent,
                                     bool calcBinormal,
                                     int iStream)
        {



            #region Allocate Tangent and Bitangent
            if ((GetVertexElementByteOffset(fromDecl, VertexElementUsage.Tangent, 0, iStream) == -1 && calcTangent) ||
               (GetVertexElementByteOffset(fromDecl, VertexElementUsage.Binormal, 0, iStream) == -1 && calcBinormal))
            {

                VertexElement[] elems = fromDecl.GetVertexElements();


                VertexElement[] newElems = new VertexElement[elems.Length +
                                                             (calcTangent ? 1 : 0) +
                                                             (calcBinormal ? 1 : 0)];
                elems.CopyTo(newElems, 0);

                if (calcTangent)
                {
                    newElems[elems.Length] =
                        new VertexElement((short)fromDecl.VertexStride,
                                          VertexElementFormat.Vector3,
                                          VertexElementUsage.Tangent,
                                          0);
                }

                if (calcBinormal)
                {
                    newElems[elems.Length + (calcTangent ? 1 : 0)] =
                        new VertexElement((short)(fromDecl.VertexStride +
                                                        (calcTangent ? sizeof(float) * 3 : 0)),
                                          VertexElementFormat.Vector3,
                                          VertexElementUsage.Binormal,
                                          0);
                }

                VertexDeclaration toDecl = new VertexDeclaration(newElems);


                fromVB = ConvertVB(fromVB, fromDecl, iStream, toDecl, iStream);

                fromDecl = toDecl;
            }

            #endregion


            Vector3[] tans = new Vector3[fromVB.VertexCount];
            Vector3[] bitans = new Vector3[fromVB.VertexCount];


            int fromNumVertices = fromVB.VertexCount;

            int newBufferSize = fromNumVertices *
                                    fromDecl.VertexStride;

            byte[] data = new byte[newBufferSize];

            fromVB.GetData<byte>(data);

            int iPosOffset = GetVertexElementByteOffset(fromDecl, VertexElementUsage.Position, 0, iStream);
            int iNormalOffset = GetVertexElementByteOffset(fromDecl, VertexElementUsage.Normal, 0, iStream);
            int iTexOffset = GetVertexElementByteOffset(fromDecl, VertexElementUsage.TextureCoordinate, 0, iStream);
            int iTanOffset = GetVertexElementByteOffset(fromDecl, VertexElementUsage.Tangent, 0, iStream);
            int iBitanOffset = GetVertexElementByteOffset(fromDecl, VertexElementUsage.Binormal, 0, iStream);

            int dwVertSize = fromDecl.VertexStride;

            int numIndices = 0;


            #region 16 bit indices
            if (IB.IndexElementSize == IndexElementSize.SixteenBits)
            {
                numIndices = IB.IndexCount;

                short[] indices = new short[numIndices];
                IB.GetData<short>(indices);

                int count = numIndices / 3;

                for (int i = 0; i < count; i++)
                {
                    float x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iPosOffset + 0 * sizeof(float));
                    float x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iPosOffset + 1 * sizeof(float));
                    float x31 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iPosOffset + 2 * sizeof(float));

                    Vector3 v1 = new Vector3(x11, x21, x31);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iPosOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iPosOffset + 1 * sizeof(float));
                    x31 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iPosOffset + 2 * sizeof(float));

                    Vector3 v2 = new Vector3(x11, x21, x31);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iPosOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iPosOffset + 1 * sizeof(float));
                    x31 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iPosOffset + 2 * sizeof(float));

                    Vector3 v3 = new Vector3(x11, x21, x31);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iTexOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iTexOffset + 1 * sizeof(float));

                    Vector2 w1 = new Vector2(x11, x21);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iTexOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iTexOffset + 1 * sizeof(float));

                    Vector2 w2 = new Vector2(x11, x21);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iTexOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iTexOffset + 1 * sizeof(float));

                    Vector2 w3 = new Vector2(x11, x21);


                    float x1 = v2.X - v1.X;
                    float x2 = v3.X - v1.X;
                    float y1 = v2.Y - v1.Y;
                    float y2 = v3.Y - v1.Y;
                    float z1 = v2.Z - v1.Z;
                    float z2 = v3.Z - v1.Z;

                    float s1 = w2.X - w1.X;
                    float s2 = w3.X - w1.X;
                    float t1 = w2.Y - w1.Y;
                    float t2 = w3.Y - w1.Y;

                    float r = 1.0f / (s1 * t2 - s2 * t1);

                    Vector3 sDir = new Vector3((t2 * x1 - t1 * x2) * r,
                                               (t2 * y1 - t1 * y2) * r,
                                               (t2 * z1 - t1 * z2) * r);

                    Vector3 tDir = new Vector3((s1 * x2 - s2 * x1) * r,
                                               (s1 * y2 - s2 * y1) * r,
                                               (s1 * z2 - s2 * z1) * r);

                    tans[indices[i * 3 + 0]] += sDir;
                    tans[indices[i * 3 + 1]] += sDir;
                    tans[indices[i * 3 + 2]] += sDir;

                    bitans[indices[i * 3 + 0]] += tDir;
                    bitans[indices[i * 3 + 1]] += tDir;
                    bitans[indices[i * 3 + 2]] += tDir;



                }



                count = fromVB.VertexCount;

                for (int i = 0; i < count; i++)
                {

                    float x11 = BitConverter.ToSingle(data,
                                i * dwVertSize + iNormalOffset + sizeof(float) * 0);
                    float x21 = BitConverter.ToSingle(data,
                                i * dwVertSize + iNormalOffset + sizeof(float) * 1);
                    float x31 = BitConverter.ToSingle(data,
                                i * dwVertSize + iNormalOffset + sizeof(float) * 2);

                    Vector3 n = new Vector3(x11, x21, x31);

                    #region Update Tangent
                    if (iTanOffset != -1)
                    {
                        Vector3 t = tans[i];
                        t = Vector3.Normalize(t - n * (Vector3.Dot(n, t)));

                        byte[] b = BitConverter.GetBytes(t.X);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iTanOffset + x + sizeof(float) * 0] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Y);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iTanOffset + x + sizeof(float) * 1] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Z);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iTanOffset + x + sizeof(float) * 2] =
                                b[x];
                        }


                    }
                    #endregion


                    #region UpdateBitangent
                    if (iBitanOffset != -1)
                    {
                        Vector3 t = bitans[i];

                        t = Vector3.Normalize(t - n * (Vector3.Dot(n, t)));

                        byte[] b = BitConverter.GetBytes(t.X);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iBitanOffset + x + sizeof(float) * 0] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Y);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iBitanOffset + x + sizeof(float) * 1] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Z);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iBitanOffset + x + sizeof(float) * 2] =
                                b[x];
                        }

                    }
                    #endregion

                }

                fromVB.SetData<byte>(data);
            }
            #endregion

            #region 32 bit indices
            if (IB.IndexElementSize == IndexElementSize.ThirtyTwoBits)
            {
                numIndices = IB.IndexCount;

                int[] indices = new int[numIndices];
                IB.GetData<int>(indices);

                int count = numIndices / 3;

                for (int i = 0; i < count; i++)
                {
                    float x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iPosOffset + 0 * sizeof(float));
                    float x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iPosOffset + 1 * sizeof(float));
                    float x31 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iPosOffset + 2 * sizeof(float));

                    Vector3 v1 = new Vector3(x11, x21, x31);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iPosOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iPosOffset + 1 * sizeof(float));
                    x31 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iPosOffset + 2 * sizeof(float));

                    Vector3 v2 = new Vector3(x11, x21, x31);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iPosOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iPosOffset + 1 * sizeof(float));
                    x31 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iPosOffset + 2 * sizeof(float));

                    Vector3 v3 = new Vector3(x11, x21, x31);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iTexOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 0] * dwVertSize + iTexOffset + 1 * sizeof(float));

                    Vector2 w1 = new Vector2(x11, x21);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iTexOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 1] * dwVertSize + iTexOffset + 1 * sizeof(float));

                    Vector2 w2 = new Vector2(x11, x21);

                    x11 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iTexOffset + 0 * sizeof(float));
                    x21 = BitConverter.ToSingle(data,
                            indices[i * 3 + 2] * dwVertSize + iTexOffset + 1 * sizeof(float));

                    Vector2 w3 = new Vector2(x11, x21);


                    float x1 = v2.X - v1.X;
                    float x2 = v3.X - v1.X;
                    float y1 = v2.Y - v1.Y;
                    float y2 = v3.Y - v1.Y;
                    float z1 = v2.Z - v1.Z;
                    float z2 = v3.Z - v1.Z;

                    float s1 = w2.X - w1.X;
                    float s2 = w3.X - w1.X;
                    float t1 = w2.Y - w1.Y;
                    float t2 = w3.Y - w1.Y;

                    float r = 1.0f / (s1 * t2 - s2 * t1);

                    Vector3 sDir = new Vector3((t2 * x1 - t1 * x2) * r,
                                               (t2 * y1 - t1 * y2) * r,
                                               (t2 * z1 - t1 * z2) * r);

                    Vector3 tDir = new Vector3((s1 * x2 - s2 * x1) * r,
                                               (s1 * y2 - s2 * y1) * r,
                                               (s1 * z2 - s2 * z1) * r);

                    tans[indices[i * 3 + 0]] += sDir;
                    tans[indices[i * 3 + 1]] += sDir;
                    tans[indices[i * 3 + 2]] += sDir;

                    bitans[indices[i * 3 + 0]] += tDir;
                    bitans[indices[i * 3 + 1]] += tDir;
                    bitans[indices[i * 3 + 2]] += tDir;



                }



                count = fromVB.VertexCount;

                for (int i = 0; i < count; i++)
                {

                    float x11 = BitConverter.ToSingle(data,
                                i * dwVertSize + iNormalOffset + sizeof(float) * 0);
                    float x21 = BitConverter.ToSingle(data,
                                i * dwVertSize + iNormalOffset + sizeof(float) * 1);
                    float x31 = BitConverter.ToSingle(data,
                                i * dwVertSize + iNormalOffset + sizeof(float) * 2);

                    Vector3 n = new Vector3(x11, x21, x31);

                    #region Update Tangent
                    if (iTanOffset != -1)
                    {
                        Vector3 t = tans[i];
                        t = Vector3.Normalize(t - n * (Vector3.Dot(n, t)));

                        byte[] b = BitConverter.GetBytes(t.X);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iTanOffset + x + sizeof(float) * 0] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Y);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iTanOffset + x + sizeof(float) * 1] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Z);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iTanOffset + x + sizeof(float) * 2] =
                                b[x];
                        }


                    }
                    #endregion


                    #region UpdateBitangent
                    if (iBitanOffset != -1)
                    {
                        Vector3 t = bitans[i];

                        t = Vector3.Normalize(t - n * (Vector3.Dot(n, t)));

                        byte[] b = BitConverter.GetBytes(t.X);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iBitanOffset + x + sizeof(float) * 0] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Y);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iBitanOffset + x + sizeof(float) * 1] =
                                b[x];
                        }

                        b = BitConverter.GetBytes(t.Z);

                        for (int x = 0; x < b.Length; x++)
                        {
                            data[i * dwVertSize + iBitanOffset + x + sizeof(float) * 2] =
                                b[x];
                        }

                    }
                    #endregion

                }

                fromVB.SetData<byte>(data);
            }
            #endregion
        }
 
    }
}
