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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
namespace Reactor
{
    public class RSoundEffect
    {
        internal SoundEffect _effect;
        internal SoundEffectInstance _instance;
        float _pitch = 1.0f;
        float _volume = 1.0f;
        bool _looping = false;
        bool _playing = false;
		private string _name;

		public string Name 
		{
			get { return _name; }
			internal set { _name = value; }
		}
        public RSoundEffect()
        {

        }
        public float Pitch
        {
            get { return _pitch; }
            set { _pitch = value; if (_instance != null) { if (value > 1.0f) value = 1.0f; if (value < -1.0f) value = -1.0f; _instance.Pitch = value; } }
        }
        public float Volume
        {
            get { return _volume; }
            set { _volume = value; if (_instance != null) { if (value > 1.0f) value = 1.0f; if (value < 0f) value = 0f; _instance.Volume = value; } }
        }
        public bool Playing
        {
            get
            {
                if (_instance != null)
                    if (_instance.State == SoundState.Playing)
                        _playing = true;
                    else
                        _playing = false;

                return _playing;
            }
            
        }
        public bool Loop
        {
            get { return _looping; }
            set { _looping = value; }
        }
        internal void CreateSoundEffect(string Name)
        {
            this.Name = Name;
        }
        
        public void Play()
        {
            if (_instance == null)
            {
                _instance = _effect.CreateInstance();
                _playing = true;
                _instance.Play();
            }
            else
            {
                if (!_playing)
                {
                    _instance.Play();
                    _playing = true;
                }
                
            }
            
        }
        public void Play(float Volume)
        {
            Volume = Volume > 1 ? 1 : Volume;
            Volume = Volume < 0 ? 0 : Volume;
            if (_instance == null)
            {
                _instance = _effect.CreateInstance();
                _instance.Volume = Volume;
                _playing = true;
                _instance.Play();
            }
            else
            {
                if (!_playing)
                {
                    _instance.Volume = Volume;
                    _instance.Play();
                }
                else
                {
                    _instance.Stop();
                    _instance.Volume = Volume;
                    _instance.Play();
                }
            }
            
        }
        public void Play(float Volume, float Pitch)
        {

            Volume = Volume > 1 ? 1 : Volume;
            Volume = Volume < 0 ? 0 : Volume;

            Pitch = Pitch > 1 ? 1 : Pitch;
            Pitch = Pitch < -1 ? -1 : Pitch;
            if (_instance == null)
            {
                _instance = _effect.CreateInstance();
                _instance.Volume = Volume;
                _instance.Pitch = Pitch;
                _playing = true;
                _instance.Play();
            }
            else
            {
                if (!_playing)
                {
                    _instance.Volume = Volume;
                    _instance.Pitch = Pitch;
                    _instance.Play();
                }
                else
                {
                    _instance.Stop();
                    _instance.Volume = Volume;
                    _instance.Pitch = Pitch;
                    _instance.Play();
                }
            }

        }
        public void Stop()
        {
            _playing = false;
            if (_instance != null)
            {
                _instance.Stop();
            }
        }
    }
    public class RSoundFactory
    {
        static RSoundFactory _instance;
        ContentManager _content;
        AudioEngine _engine;
        List<RSoundEffect> _effects;
        MediaLibrary _library;
        Random _random;
        public static RSoundFactory Instance
        {
            get { return _instance; }
            set { Instance = value; }
        }

        public RSoundFactory()
        {
            if (_instance == null)
                _instance = this;
            
        }

        public void Initialize()
        {
            _instance._content = new ContentManager(REngine.Instance._game.Services);
            _instance._content.RootDirectory = _content.RootDirectory + "\\Content";
            _instance._effects = new List<RSoundEffect>();
            _instance._library = new MediaLibrary();
            _instance._random = new Random();
            
        }
        
        public int CreateSoundEffect(string Name, string Filename)
        {

            RSoundEffect effect = new RSoundEffect();
            effect._effect = _content.Load<SoundEffect>(Filename);
            _instance._effects.Add(effect);
            effect.CreateSoundEffect(Name);
            return _instance._effects.IndexOf(effect);
        }
        public bool DeleteSoundEffect(int EffectID)
        {
            if (_instance._effects[EffectID] != null)
            {
                _instance._effects[EffectID]._effect.Dispose();
                _instance._effects[EffectID] = null;
                return true;
            }
            else
                return false;
        }
        public void PlayMediaLibraryMusic()
        {
            SongCollection s = _library.Songs;
            MediaPlayer.Play(s);
            
            
        }
        public void SetMediaLibraryRepeat(bool Repeat)
        {
            MediaPlayer.IsRepeating = Repeat;
        }
        public void PlaySong(string Filename)
        {
            Song s = _instance._content.Load<Song>(Filename);
            
            MediaPlayer.Play(s);
            
            
        }
        public void PlaySong(string Filename, float volume)
        {
            Song s = _instance._content.Load<Song>(Filename);
            MediaPlayer.Volume = volume;
            MediaPlayer.Play(s);


        }
        public RSoundEffect GetRSoundEffect(int EffectID)
        {
            return _instance._effects[EffectID];
        }
        public void PlayEffect(int EffectID)
        {
            if (_instance._effects[EffectID] != null)
            {
                _instance._effects[EffectID].Play();
            }
            
        }
        public void PlayEffect(int EffectID, float Volume)
        {
            if (_instance._effects[EffectID] != null)
            {
                _instance._effects[EffectID].Play(Volume);
            }

        }
        public void PlayEffect(int EffectID, float Volume, float Pitch)
        {
            if (_instance._effects[EffectID] != null)
            {
                _instance._effects[EffectID].Play(Volume, Pitch);
            }

        }
    }
}
