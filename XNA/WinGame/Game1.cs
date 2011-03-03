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
using System.Text;
using Reactor;


namespace WinGame
{
    public class Game : RGame
    {
        RScene scene;
        RScreen2D screen;
        RCamera camera;
        RAtmosphere atmosphere;
        RActor mesh;
        RInput input;
        RParticleEmitter emitter;
        RParticleEmitter emitter2;
        RLandscape landscape;
        RWater water;
        RTextureFactory textures;
        RLightingFactory lighting;
        RMaterialFactory materials;
        RFONT font;
        bool fog = true;
        public Game()
            : base()
        {

        }

        public override void Init()
        {
#if !XBOX
            this.Reactor.Init3DGame(1280, 720, false);
#else
            this.Reactor.Init3DGame(180, 720, true);
            this.Reactor.SetXBox360Resolution(CONST_REACTOR_XBOX_RESOLUTION.r720P);
#endif
            this.Reactor.SetWatermarkPosition(CONST_REACTOR_WATERMARK_POSITION.BOTTOM_RIGHT);
            Reactor.ShowFPS(true);
            Reactor.AllowEscapeQuit(true);
            Reactor.ShowMouse(false);
            //Reactor.SetDebugFile("debug.txt");
            //Reactor.SetDebugMode(true);
            scene = new RScene();
            screen = new RScreen2D();
            textures = new RTextureFactory();
            //lighting = new RLightingFactory();
            //materials = new RMaterialFactory();
            
            
            Reactor.Resized += new ViewportResized(Reactor_Resized);
            camera = this.Reactor.GetCamera();
            camera.SetClipPlanes(1f, 180000);
            atmosphere = new RAtmosphere();
            atmosphere.Initialize();
            input = new RInput();
            camera.Position = R3DVECTOR.Zero;
            camera.LookAt(new R3DVECTOR(0.5f,0f,0.5f));
            font = screen.Create_TextureFont("Font1", "Font1");
            //emitter = (RParticleEmitter)scene.CreateNode<RParticleEmitter>("myemitter");
            //emitter2 = scene.CreateParticleEmitter(CONST_REACTOR_PARTICLETYPE.Billboard, "myemitter2");
            landscape = (RLandscape)scene.CreateNode<RLandscape>("myland");
            water = (RWater)scene.CreateNode<RWater>("mywater");
            //mesh = (RActor)scene.CreateNode<RActor>("mymesh");
            
        }

        void Reactor_Resized()
        {
            Reactor.Resize(Reactor.GetViewport().Width, Reactor.GetViewport().Height);
        }

        public override void Dispose()
        {
            //Reactor.ReleaseAll();
        }

        public override void Load()
        {
            int islands_down = textures.LoadTexture("Textures\\islands_down", "islands_down", CONST_RTEXTURE_TYPE.Texture2D);
            int islands_up = textures.LoadTexture("Textures\\islands_up", "islands_up", CONST_RTEXTURE_TYPE.Texture2D);
            int islands_east = textures.LoadTexture("Textures\\islands_east", "islands_east", CONST_RTEXTURE_TYPE.Texture2D);
            int islands_west = textures.LoadTexture("Textures\\islands_west", "islands_west", CONST_RTEXTURE_TYPE.Texture2D);
            int islands_north = textures.LoadTexture("Textures\\islands_north", "islands_north", CONST_RTEXTURE_TYPE.Texture2D);
            int islands_south = textures.LoadTexture("Textures\\islands_south", "islands_south", CONST_RTEXTURE_TYPE.Texture2D);

            //int starstexture = textures.LoadTexture("stars", "starbg", CONST_RTEXTURE_TYPE.Texture2D);
            int skycube = textures.BuildCubeMap(1024, "bgcube", islands_west, islands_up, islands_north, islands_east, islands_down, islands_south, RSURFACEFORMAT.Bgr32);
            int skytexture = textures.LoadTexture("skybox001", "sky", CONST_RTEXTURE_TYPE.TextureCube);


            int pointtexture = textures.LoadTexture("point", "point", CONST_RTEXTURE_TYPE.Texture2D);


            int terraintexture = textures.LoadTexture("gcanyonTerrain", "terrainData", CONST_RTEXTURE_TYPE.Texture2D);
            int terrainheight = textures.LoadTexture("gcanyon", "terrain", CONST_RTEXTURE_TYPE.Texture2D);
            
            int rock = textures.LoadTexture("Textures\\rock", "rock", CONST_RTEXTURE_TYPE.Texture2D);
            int rocknormal = textures.LoadTexture("Textures\\rockNormal", "rockNormal", CONST_RTEXTURE_TYPE.Texture2D);

            int grass = textures.LoadTexture("Textures\\grass", "grass", CONST_RTEXTURE_TYPE.Texture2D);
            int grassnormal = textures.LoadTexture("Textures\\grassNormal", "grassNormal", CONST_RTEXTURE_TYPE.Texture2D);

            int sand = textures.LoadTexture("Textures\\sand", "sand", CONST_RTEXTURE_TYPE.Texture2D);
            int sandnormal = textures.LoadTexture("Textures\\sandNormal", "sandNormal", CONST_RTEXTURE_TYPE.Texture2D);

            int snow = textures.LoadTexture("Textures\\snow", "snow", CONST_RTEXTURE_TYPE.Texture2D);
            int snownormal = textures.LoadTexture("Textures\\snowNormal", "snowNormal", CONST_RTEXTURE_TYPE.Texture2D);
            
            //int billboardgrass = textures.LoadTexture("billboardgrass", "billboardgrass", CONST_RTEXTURE_TYPE.Texture2D);

            int tree = textures.LoadTexture("Textures\\Pine", "Pine", CONST_RTEXTURE_TYPE.Texture2D);
            //int grassbillboard = textures.LoadTexture("Textures\\billboardgrass", "GrassBillboard", CONST_RTEXTURE_TYPE.Texture2D);
            int wave0 = textures.LoadTexture("Textures\\wave0", "wave0", CONST_RTEXTURE_TYPE.Texture2D);
            int wave1 = textures.LoadTexture("Textures\\wave1", "wave1", CONST_RTEXTURE_TYPE.Texture2D);
            //mesh.Load("dude");
            
            //mesh.SetPosition(0, 0, 0);
            
            //mesh.PlayAnimation("Take 001", 1.0f);
            //mesh.SetScale(0.1f, 0.1f, 0.1f);
            //mesh.SetLookAt(new R3DVECTOR(-1, 0, -1));
            //R3DVECTOR scale = mesh.GetScale();
            //emitter.SetTexture(pointtexture);
            //emitter.BuildEmitter(135000);
            
            //emitter2.BuildEmitter(500);
            //emitter2.SetTexture(pointtexture);
            //emitter.SetPointSettings(pointtexture, 135000, 10000, -1.0f,1.0f,0.0f,0.0f, R3DVECTOR.Down * 500f, 100.0f, 0.0f, 0.0f, 150, 150, 150,150);
            landscape.SetDetailLevel(RLANDSCAPE_LOD.Ultra);
            landscape.SetElevationStrength(10);
            landscape.SetMinGrassPlotElevation(1f);
            landscape.SetMaxGrassPlotElevation(700f);
            
            landscape.InitTerrainTextures(snow, grass, sand);
            landscape.InitTerrainNormalsTextures(snownormal, grassnormal, rocknormal);
            landscape.InitTerrainGrass(tree, terraintexture, 1750);
            landscape.SetTerrainGrassSize(18f, 40f);

            landscape.SetPosition(new R3DVECTOR((256f * 100f) - (512f * 100f), -300f, (256f * 100f) - (512f * 100f)));
            landscape.Initialize(terrainheight, terraintexture, 50, 12);
            landscape.SetupLODS();
            

            // Put camera somewhere on the landscape.  Rule is HeightMap Width x Scale for exact position.  I'm dividing by 3 to place somewhere
            // in the first third area so we have landscape around us.
            //camera.Position = new R3DVECTOR((512 * 40) / 3, landscape.GetTerrainHeight((512*40)/3, (512*40)/3)+5f, (512 * 40) / 3);
            camera.Position = new R3DVECTOR(1.0f, 1.0f, 1.0f);
            camera.SetFieldOfView(45f);
            camera.SetClipPlanes(1f, 180000f);
            camera.LookAt(camera.Position * R3DVECTOR.Backward);
            //input.SetMousePosition(1280 / 2, 800 / 2);
            camera.CurrentBehavior = RCamera.Behavior.Flight;
            //mesh.SetPosition(camera.Position + (camera.ViewDirection*1.0001f));
            //mesh.SetLookAt(new R3DVECTOR(mesh.GetPosition().X-1, mesh.GetPosition().Y, mesh.GetPosition().Z-1));
            atmosphere.SkyBox_Enable(true);
            atmosphere.SkyBox_Initialize();
            atmosphere.SkyBox_SetTextureCube(skycube);
            atmosphere.Fog_Enable(fog);
            atmosphere.Fog_SetDistance(9000f);
            atmosphere.Fog_SetColor(0.7f, 0.7f, 0.7f, 0.5f);
            atmosphere.Sky_SetSunlightColor(new R4DVECTOR(1f, 1f, 1f, 1.0f));
            atmosphere.Sky_SetSunlightDirection(new R3DVECTOR(0.1f, -1.5f, 0.1f));
            atmosphere.Sky_SetGlobalAmbientColor(new R4DVECTOR(0.1f, 0.1f, 0.1f, 1.0f));
            //emitter.SetPosition(v);
            //emitter2.SetPosition(v);
            //emitter2.SetScale(1f);
            //emitter2.SetBillboardSize(5f);
            RWaterOptions options = new RWaterOptions();
            options.WaveMapAsset0ID = wave0;
            options.WaveMapAsset1ID = wave1;
            options.WaveMapScale = 100.0f;
            options.WaterColor = new R4DVECTOR(0.5f, 0.6f, 0.7f, 0.5f);
            options.WaveMapVelocity0 = new R2DVECTOR(0.005f, 0.003f);
            options.WaveMapVelocity1 = new R2DVECTOR(-0.001f, 0.01f);
            options.SunFactor = 1.0f;
            options.SunPower = 350f;
            options.Width = 256;
            options.Height = 256;
            options.CellSpacing = 1.0f;
            options.RenderTargetSize = 256;
            water.SetOptions(options);
            water.SetReflectionRenderDelegate(Reflection);
            water.Init(64, 12f, new R2DVECTOR(0.00003f,-0.00001f));
            water.SetScale(new R3DVECTOR(10000f,5f,10000f));
            water.SetWaveNormalTexture(wave0);
            water.SetPosition(new R3DVECTOR(256*10000, 300f, 256*10000));
            //water.SetWaveParameters(0.008f, 0.0003f, 0.38f, new R4DVECTOR(0.2f, 0.2f, 0.1f, 1f), 0.29f, 0.3f, new R4DVECTOR(0.5f, 0.5f, 0.5f, 0.5f), new R4DVECTOR(0.2f, 0.4f, 0.7f, 1f), false);
            //landscape.ToggleTerrainDraw();
        }
        //This is where we render our objects for the water reflections!
        public void Reflection(R3DMATRIX ReflectionMatrix)
        {

            try
            {
                // Set our reflection matrix for the atmosphere to draw itself flipped
                atmosphere.SkyBox_SetReflectionMatrix(ReflectionMatrix);
                // Render our Sky
                atmosphere.SkyBox_Render();
                // Reset the sky's matrix back to it's original
                atmosphere.SkyBox_ResetReflectionMatrix();

                // Landscape is different as scaling is baked in, instead we call SetReflectionMatrix and it will do the rest.
                //landscape.SetReflectionMatrix(ReflectionMatrix);
                //landscape.Render();
                //landscape.RenderGrass();
                //landscape.ResetReflectionMatrix();
                // Reset reflection matrix on the Landscape to draw as normal.
                

                // With any game objects, you want to get it's world matrix first, multiply the reflection, and set the matrix back to the game object.
                // The next time this is called, it will flip it back.
                //R3DMATRIX meshMatrix = mesh.GetMatrix();
                //mesh.SetMatrix(meshMatrix * ReflectionMatrix);
                //mesh.Render();
                //mesh.SetMatrix(meshMatrix);

                
            }
            catch (Exception e)
            {
            }

            
        }
        
        public override void Render2D()
        {
            //long mem = GC.GetTotalMemory(false)/1024;
            //screen.Action_Begin2D();
            //screen.Draw_TextureFont(font, 40, 65, Reactor.GetCamera().ViewDirection.ToString());
            //screen.Draw_TextureFont(font, 40, 80, "Garbage Memory: " +  mem.ToString()+"kb");
            //screen.Action_End2D();
        }
        public override void Render()
        {
            try
            {
                atmosphere.SkyBox_Render();


                landscape.Render();

                water.Render();
                
                //mesh.Render();
                //landscape.RenderGrass();


                //emitter.Render();
                //emitter2.Render();
                
                //screen.Draw_TextureFont(font, 40, 50, emitter.GetPosition().ToString());
                
                //screen.Draw_Line3D(new R3DVECTOR(0,0,0), new R3DVECTOR(1500f, 150f, 1500f), new R4DVECTOR(1f,1f,1f,1f));
                //screen.Draw_Rect2D(40, 75, 100, 2, new R4DVECTOR(1f, 1f, 1f, 1f));
                //screen.Draw_Texture2D(0, 10, 20, 128, 128, 1, 1);
                
            }
            catch (Exception e)
            {
            }

        }
        R2DVECTOR lastMouseLocation;
        public override void Update()
        {
            
            int X,Y,Wheel;
            bool b1,b2,b3;
#if !XBOX
            input.GetCenteredMouse(out X, out Y, out Wheel, out b1, out b2, out b3);
            R2DVECTOR mouseMoved = new R2DVECTOR(lastMouseLocation.X - X, lastMouseLocation.Y - Y);
            lastMouseLocation = new R2DVECTOR(X, Y);
            //camera.RotateFirstPerson(X * 0.1f, Y * 0.1f);
            camera.RotateFlight(X*0.1f, Y*0.1f, 0);
            //camera.LevelRoll();

            if (input.IsKeyDown(CONST_REACTOR_KEY.A) && !input.IsKeyDown(CONST_REACTOR_KEY.LeftShift))
            {
                camera.Move(-0.001f * Reactor.AccurateTimeElapsed(), 0f, 0f);
                //camera.RotateY(0.005f * Reactor.AccurateTimeElapsed());
            }
            if (input.IsKeyDown(CONST_REACTOR_KEY.D) && !input.IsKeyDown(CONST_REACTOR_KEY.LeftShift))
            {
                camera.Move(0.001f * Reactor.AccurateTimeElapsed(), 0f, 0f);
                //camera.RotateY(-0.005f * Reactor.AccurateTimeElapsed());
            }
            if (input.IsKeyDown(CONST_REACTOR_KEY.W) && !input.IsKeyDown(CONST_REACTOR_KEY.LeftShift))
            {
                camera.Move(0f, 0f, 0.001f * Reactor.AccurateTimeElapsed());
                //camera.Position = new R3DVECTOR(camera.Position.X, landscape.GetTerrainHeight(camera.Position.X, camera.Position.Z)+2f, camera.Position.Z);

            }
            if (input.IsKeyDown(CONST_REACTOR_KEY.S) && !input.IsKeyDown(CONST_REACTOR_KEY.LeftShift))
            {
                camera.Move(0f, 0f, -0.001f * Reactor.AccurateTimeElapsed());
                //camera.Position = new R3DVECTOR(camera.Position.X, landscape.GetTerrainHeight(camera.Position.X, camera.Position.Z) + 2f, camera.Position.Z);
            }
            if (input.IsKeyDown(CONST_REACTOR_KEY.LeftShift) && input.IsKeyDown(CONST_REACTOR_KEY.W))
                camera.Move(0f, 0f, 0.1f * Reactor.AccurateTimeElapsed());
            if (input.IsKeyDown(CONST_REACTOR_KEY.LeftShift) && input.IsKeyDown(CONST_REACTOR_KEY.S))
                camera.Move(0f, 0f, -0.1f * Reactor.AccurateTimeElapsed());
            if (input.IsKeyDown(CONST_REACTOR_KEY.LeftShift) && input.IsKeyDown(CONST_REACTOR_KEY.A))
                camera.Move(-0.1f * Reactor.AccurateTimeElapsed(), 0f, 0f);
            if (input.IsKeyDown(CONST_REACTOR_KEY.LeftShift) && input.IsKeyDown(CONST_REACTOR_KEY.D))
                camera.Move(0.1f * Reactor.AccurateTimeElapsed(), 0f, 0f);
            if (input.IsKeyDown(CONST_REACTOR_KEY.T))
                landscape.ToggleBoundingBoxDraw();
            if (input.IsKeyDown(CONST_REACTOR_KEY.F))
            {
                if (fog) { fog = false; } else { fog = true; }
                atmosphere.Fog_Enable(fog);
            }

            if (input.IsKeyDown(CONST_REACTOR_KEY.F1))
                scene.SetShadeMode(CONST_REACTOR_FILLMODE.Point);
            if (input.IsKeyDown(CONST_REACTOR_KEY.F2))
                scene.SetShadeMode(CONST_REACTOR_FILLMODE.Wireframe);
            if (input.IsKeyDown(CONST_REACTOR_KEY.F3))
                scene.SetShadeMode(CONST_REACTOR_FILLMODE.Solid);

            if (input.IsKeyDown(CONST_REACTOR_KEY.PrintScreen))
                this.Reactor.TakeScreenshot("sshot"+Reactor.GetTicks().ToString());


            if (input.IsKeyDown(CONST_REACTOR_KEY.Left))
                camera.RotateX(0.1f);
            if (input.IsKeyDown(CONST_REACTOR_KEY.Right))
                camera.RotateX(-0.1f);

            if (input.IsKeyDown(CONST_REACTOR_KEY.Space))
                camera.LevelRoll();
#endif
#if XBOX
            if (input.GetControllerState(0).IsConnected)
            {
                Microsoft.Xna.Framework.Input.GamePadState state = input.GetControllerState(0);
                camera.Move(state.ThumbSticks.Left.X*5.5f, 0, state.ThumbSticks.Left.Y*5.5f);
                camera.RotateFlight(state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y, 0.0f);
                //camera.RotateFirstPerson(state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);
                if(input.GetControllerState(0).IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftShoulder))
                    camera.LevelRoll();
                if (state.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Back))
                    Exit();
            }
#endif
            atmosphere.Update();
            landscape.Update();
            //float landheight = landscape.GetTerrainHeight(camera.Position.X, camera.Position.Z);

            //if (landheight < water.GetPosition().Y)
            //{
                //landheight = water.GetPosition().Y;
            //}
            //R3DVECTOR ch = new R3DVECTOR(camera.Position.X, landheight + 30, camera.Position.Z);
            //camera.Position = ch;
            //camera.Update();
            //mesh.Move(0f, 0, 0.03995f * Reactor.AccurateTimeElapsed());
            //R3DVECTOR v = mesh.GetPosition();
            //mesh.SetPosition(v.X, landscape.GetTerrainHeight(v.X, v.Z), v.Z);
            //mesh.SetLookAt(new R3DVECTOR(mesh.GetPosition().X - 1, mesh.GetPosition().Y, mesh.GetPosition().Z - 1));
            //R3DVECTOR v = camera.Position;
            //v.Y = landscape.GetTerrainHeight(v.X, v.Z) + 50f;
            //if(fog)
            //camera.Position = v;
            //mesh.Update();
            //camera.Position = v;
            //camera.Update();
            //emitter.SetDirection(R3DVECTOR.Down * 50f);
            //emitter.SetPosition(emitter.GetPosition() * new Random().Next());
            //r = new Random((int)Reactor.GetTicks());
            //float x = (float)r.NextDouble() * (1000f) - 500f;
            //float z = (float)r.NextDouble() * (1000f) - 500f;
            //emitter.SetPosition(new R3DVECTOR(camera.Position.X + x, camera.Position.Y + 200f, camera.Position.Z + z));
            //emitter.Update();
            //R3DVECTOR v = Ball();
            //emitter2.SetPosition(v);
            //emitter2.Update();
            water.Update();
            //landscape.Update();
            
        }
        Random r = new Random();
        float angle = 0f;
        float angle2 = 0f;
        R3DVECTOR Ball()
        {
            float radius = 10;
            angle += .005f;
            if (angle > 360)
            angle = 0;
            angle2 += .005f;
            if (angle2 > 360)
            angle2 = 0;
            float cos = radius * (float)Math.Cos(angle);
            float sin = radius * (float)Math.Sin(angle);
            float cos2 = radius * (float)Math.Cos(angle2);
            float sin2 = (float)Math.Pow(radius, 1) * (float)Math.Sin(angle2);
            return new R3DVECTOR(cos * cos2, sin * cos2, sin2);
            
        }


    }
}
