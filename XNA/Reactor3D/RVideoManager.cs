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

//how to use  
//Call new instance of the class     VideoManager myVideo;  
//load vid manager-                  myVideo = new VideoManager(arguments,,,,,,,)  
//start video -                      myVideo.Start();  
//updateVideo                        myVideo.Update();
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Reactor
{
    public class RVideoManager
    {
        Video video;
        //VideoPlayer vidPlayer;
        double crop;
        double time;
        bool PlayFullVid;
        public Vector2 position;
        public Vector2 scale;
        bool loop;
        double timer;

        /// <summary>  
        /// Video manager lets you add a video and play and stop and such  
        /// </summary>  
        /// <param name="content">pass in content from game1.cs class</param>  
        /// <param name="videoFileLocation">full string file path</param>  
        /// <param name="Crop">take off time from the end of the vid, if the vid is 13 secs and you put 3 here you get the first 10 seconds of your video</param>  
        /// <param name="Position">position to play video, using 0,0 as origin</param>  
        /// <param name="Scale">scale of the video</param>  
        /// <param name="Loop">bool to tell weather or not to loop the video</param>  
        public RVideoManager(string videoFileLocation, double Crop, R2DVECTOR Position, R2DVECTOR Size, bool Loop)
        {
            video = REngine.Instance._game.Content.Load<Video>(videoFileLocation);
            loop = Loop;
            scale = Size.vector;
            position = Position.vector;
            crop = Crop;
            //vidPlayer = new VideoPlayer();

        }

        /*public bool IsPaused
        {
            //get { return vidPlayer.State == MediaState.Paused; }
        }
        public bool IsPlaying
        {
           // get { return vidPlayer.State == MediaState.Playing; }
        }
        public bool IsStopped
        {
            //get { return vidPlayer.State == MediaState.Stopped; }
        }*/
        public void Start()
        {
            timer = 0;
            //if (vidPlayer.State != MediaState.Playing)
            //    vidPlayer.Play(video);
        }

        public void Stop()
        {
            //vidPlayer.Stop();
        }

        public void Pause()
        {
            //if (vidPlayer.State == MediaState.Paused)
            //    vidPlayer.Resume();
            //else if (vidPlayer.State == MediaState.Playing)
            //    vidPlayer.Pause();
        }

        /// <summary>  
        /// updates the player  
        /// </summary>  
        /// <param name="gameTime"></param>  
        public void Update()
        {
            timer += REngine.Instance._gameTime.ElapsedGameTime.TotalSeconds;

            //if (!loop)
                //if (timer > video.Duration.TotalSeconds - crop)
                //{
                //    vidPlayer.Stop();
                //}
        }
        public void Draw_Video(int X, int Y, int Width, int Height, int scaleX, int scaleY)
        {

            //Texture2D tex = vidPlayer.GetTexture();
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            RScreen2D.Instance._spritebatch.Begin();
            //RScreen2D.Instance._spritebatch.Draw(tex, rect, Color.White);
            RScreen2D.Instance._spritebatch.End();

        }
        public void Draw_Video(int X, int Y, int Width, int Height, int scaleX, int scaleY, R4DVECTOR color)
        {
            Color c = new Color(color.vector);
            //Texture2D tex = vidPlayer.GetTexture();
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            RScreen2D.Instance._spritebatch.Begin();
            //RScreen2D.Instance._spritebatch.Draw(tex, rect, c);
            RScreen2D.Instance._spritebatch.End();

        }
        public void Draw_Video(int X, int Y, int Width, int Height, int SourceX, int SourceY, int SourceWidth, int SourceHeight, int scaleX, int scaleY, R4DVECTOR color)
        {
            Color c = new Color(color.vector);
            //Texture2D tex = vidPlayer.GetTexture();
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            Rectangle sourcerect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);

            RScreen2D.Instance._spritebatch.Begin();
            //RScreen2D.Instance._spritebatch.Draw(tex, rect, sourcerect, c);
            RScreen2D.Instance._spritebatch.End();


        }
        public void Draw_Video(int X, int Y, int Width, int Height, int SourceX, int SourceY, int SourceWidth, int SourceHeight, int scaleX, int scaleY, R4DVECTOR color, float Rotation)
        {
            Color c = new Color(color.vector);
            //Texture2D tex = vidPlayer.GetTexture();
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            Rectangle sourcerect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);

            RScreen2D.Instance._spritebatch.Begin();
            //RScreen2D.Instance._spritebatch.Draw(tex, rect, sourcerect, c, Rotation, new Vector2((rect.Width / 2), (rect.Height / 2)), SpriteEffects.None, 1.0f);
            RScreen2D.Instance._spritebatch.End();

        }
        public void Draw_Video(int X, int Y, int Width, int Height, int SourceX, int SourceY, int SourceWidth, int SourceHeight, int scaleX, int scaleY, R4DVECTOR color, float Rotation, bool FlipHorizontal)
        {
            Color c = new Color(color.vector);
            //Texture2D tex = vidPlayer.GetTexture();
            Rectangle rect = new Rectangle(X, Y, Width * scaleX, Height * scaleY);
            Rectangle sourcerect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);
            SpriteEffects effects = SpriteEffects.None;
            if (FlipHorizontal)
                effects = SpriteEffects.FlipHorizontally;
            RScreen2D.Instance._spritebatch.Begin();
            //RScreen2D.Instance._spritebatch.Draw(tex, rect, sourcerect, c, Rotation, new Vector2((rect.Width / 2), (rect.Height / 2)), effects, 1.0f);
            RScreen2D.Instance._spritebatch.End();
            
        }
    }
}