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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System.Text;

namespace Reactor
{
    #region CONSTANT ENUMERATORS
    public enum CONST_REACTOR_XBOX_RESOLUTION : int
    {
        r480P = 0,
        r720P = 1,

    }
    public enum CONST_REACTOR_TEXTURELAYER : int
    {
        Diffuse = 0,
        Normal = 1,
        Bump = 2,
        Opacity = 3,
        Lighting = 4,
        Detail = 5,
        Relief = 6
    }
    public enum CONST_REACTOR_FILLMODE : int
    {
        Wireframe = 1,
        Solid = 2
    }
    public enum CONST_REACTOR_DEPTHBUFFERFORMAT : int
    {
        Depth16Bit = DepthFormat.Depth16,
        Depth24Bit = DepthFormat.Depth24,
        Depth24Bit8Stencil = DepthFormat.Depth24Stencil8,
        None = DepthFormat.None
    }
    public enum CONST_REACTOR_LIGHTTYPE : int
    {
        Directional = 0,
        Omni = 1,
        Spot = 2,
        Other = 3
    }
    public enum CONST_REACTOR_SURFACEFORMAT : int
    {
        Color = 0,
        Bgr565 = 1,
        Bgra5551 = 2,
        Bgra4444 = 3,
        Dxt1 = 4,
        Dxt3 = 5,
        Dxt5 = 6,
        NormalizedByte2 = 7,
        NormalizedByte4 = 8,
        Rgba1010102 = 9,
        Rg32 = 10,
        Rgba64 = 11,
        Alpha8 = 12,
        Single = 13,
        Vector2 = 14,
        Vector4 = 15,
        HalfSingle = 16,
        HalfVector2 = 17,
        HalfVector4 = 18,
        HdrBlendable = 19,
        Bgr32 = 20,
        Bgra32 = 21,
        RgbPvrtc2Bpp = 50,
        RgbPvrtc4Bpp = 51,
        RgbaPvrtc2Bpp = 52,
        RgbaPvrtc4Bpp = 53,
        RgbEtc1 = 60,
        Dxt1a = 70,

    }
    public enum CONST_REACTOR_PARTICLETYPE : int
    {
        Point = 0,
        Billboard = 1,
        MiniMesh = 2
    }
    public enum CONST_REACTOR_KEY : int
    {
        None = 0,
        //
        // Summary:
        //     BACKSPACE key
        Back = 8,
        //
        // Summary:
        //     TAB key
        Tab = 9,
        //
        // Summary:
        //     ENTER key
        Enter = 13,
        //
        // Summary:
        //     CAPS LOCK key
        CapsLock = 20,
        //
        // Summary:
        //     ESC key
        Escape = 27,
        //
        // Summary:
        //     SPACEBAR
        Space = 32,
        //
        // Summary:
        //     PAGE UP key
        PageUp = 33,
        //
        // Summary:
        //     PAGE DOWN key
        PageDown = 34,
        //
        // Summary:
        //     END key
        End = 35,
        //
        // Summary:
        //     HOME key
        Home = 36,
        //
        // Summary:
        //     LEFT ARROW key
        Left = 37,
        //
        // Summary:
        //     UP ARROW key
        Up = 38,
        //
        // Summary:
        //     RIGHT ARROW key
        Right = 39,
        //
        // Summary:
        //     DOWN ARROW key
        Down = 40,
        //
        // Summary:
        //     SELECT key
        Select = 41,
        //
        // Summary:
        //     PRINT key
        Print = 42,
        //
        // Summary:
        //     EXECUTE key
        Execute = 43,
        //
        // Summary:
        //     PRINT SCREEN key
        PrintScreen = 44,
        //
        // Summary:
        //     INS key
        Insert = 45,
        //
        // Summary:
        //     DEL key
        Delete = 46,
        //
        // Summary:
        //     HELP key
        Help = 47,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D0 = 48,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D1 = 49,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D2 = 50,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D3 = 51,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D4 = 52,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D5 = 53,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D6 = 54,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D7 = 55,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D8 = 56,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        D9 = 57,
        //
        // Summary:
        //     A key
        A = 65,
        //
        // Summary:
        //     B key
        B = 66,
        //
        // Summary:
        //     C key
        C = 67,
        //
        // Summary:
        //     D key
        D = 68,
        //
        // Summary:
        //     E key
        E = 69,
        //
        // Summary:
        //     F key
        F = 70,
        //
        // Summary:
        //     G key
        G = 71,
        //
        // Summary:
        //     H key
        H = 72,
        //
        // Summary:
        //     I key
        I = 73,
        //
        // Summary:
        //     J key
        J = 74,
        //
        // Summary:
        //     K key
        K = 75,
        //
        // Summary:
        //     L key
        L = 76,
        //
        // Summary:
        //     M key
        M = 77,
        //
        // Summary:
        //     N key
        N = 78,
        //
        // Summary:
        //     O key
        O = 79,
        //
        // Summary:
        //     P key
        P = 80,
        //
        // Summary:
        //     Q key
        Q = 81,
        //
        // Summary:
        //     R key
        R = 82,
        //
        // Summary:
        //     S key
        S = 83,
        //
        // Summary:
        //     T key
        T = 84,
        //
        // Summary:
        //     U key
        U = 85,
        //
        // Summary:
        //     V key
        V = 86,
        //
        // Summary:
        //     W key
        W = 87,
        //
        // Summary:
        //     X key
        X = 88,
        //
        // Summary:
        //     Y key
        Y = 89,
        //
        // Summary:
        //     Z key
        Z = 90,
        //
        // Summary:
        //     Left Windows key
        LeftWindows = 91,
        //
        // Summary:
        //     Right Windows key
        RightWindows = 92,
        //
        // Summary:
        //     Applications key
        Apps = 93,
        //
        // Summary:
        //     Computer Sleep key
        Sleep = 95,
        //
        // Summary:
        //     Numeric keypad 0 key
        NumPad0 = 96,
        //
        // Summary:
        //     Numeric keypad 1 key
        NumPad1 = 97,
        //
        // Summary:
        //     Numeric keypad 2 key
        NumPad2 = 98,
        //
        // Summary:
        //     Numeric keypad 3 key
        NumPad3 = 99,
        //
        // Summary:
        //     Numeric keypad 4 key
        NumPad4 = 100,
        //
        // Summary:
        //     Numeric keypad 5 key
        NumPad5 = 101,
        //
        // Summary:
        //     Numeric keypad 6 key
        NumPad6 = 102,
        //
        // Summary:
        //     Numeric keypad 7 key
        NumPad7 = 103,
        //
        // Summary:
        //     Numeric keypad 8 key
        NumPad8 = 104,
        //
        // Summary:
        //     Numeric keypad 9 key
        NumPad9 = 105,
        //
        // Summary:
        //     Multiply key
        Multiply = 106,
        //
        // Summary:
        //     Add key
        Add = 107,
        //
        // Summary:
        //     Separator key
        Separator = 108,
        //
        // Summary:
        //     Subtract key
        Subtract = 109,
        //
        // Summary:
        //     Decimal key
        Decimal = 110,
        //
        // Summary:
        //     Divide key
        Divide = 111,
        //
        // Summary:
        //     F1 key
        F1 = 112,
        //
        // Summary:
        //     F2 key
        F2 = 113,
        //
        // Summary:
        //     F3 key
        F3 = 114,
        //
        // Summary:
        //     F4 key
        F4 = 115,
        //
        // Summary:
        //     F5 key
        F5 = 116,
        //
        // Summary:
        //     F6 key
        F6 = 117,
        //
        // Summary:
        //     F7 key
        F7 = 118,
        //
        // Summary:
        //     F8 key
        F8 = 119,
        //
        // Summary:
        //     F9 key
        F9 = 120,
        //
        // Summary:
        //     F10 key
        F10 = 121,
        //
        // Summary:
        //     F11 key
        F11 = 122,
        //
        // Summary:
        //     F12 key
        F12 = 123,
        //
        // Summary:
        //     F13 key
        F13 = 124,
        //
        // Summary:
        //     F14 key
        F14 = 125,
        //
        // Summary:
        //     F15 key
        F15 = 126,
        //
        // Summary:
        //     F16 key
        F16 = 127,
        //
        // Summary:
        //     F17 key
        F17 = 128,
        //
        // Summary:
        //     F18 key
        F18 = 129,
        //
        // Summary:
        //     F19 key
        F19 = 130,
        //
        // Summary:
        //     F20 key
        F20 = 131,
        //
        // Summary:
        //     F21 key
        F21 = 132,
        //
        // Summary:
        //     F22 key
        F22 = 133,
        //
        // Summary:
        //     F23 key
        F23 = 134,
        //
        // Summary:
        //     F24 key
        F24 = 135,
        //
        // Summary:
        //     NUM LOCK key
        NumLock = 144,
        //
        // Summary:
        //     SCROLL LOCK key
        Scroll = 145,
        //
        // Summary:
        //     Left SHIFT key
        LeftShift = 160,
        //
        // Summary:
        //     Right SHIFT key
        RightShift = 161,
        //
        // Summary:
        //     Left CONTROL key
        LeftControl = 162,
        //
        // Summary:
        //     Right CONTROL key
        RightControl = 163,
        //
        // Summary:
        //     Left ALT key
        LeftAlt = 164,
        //
        // Summary:
        //     Right ALT key
        RightAlt = 165,
        //
        // Summary:
        //     Windows 2000/XP: Browser Back key
        BrowserBack = 166,
        //
        // Summary:
        //     Windows 2000/XP: Browser Forward key
        BrowserForward = 167,
        //
        // Summary:
        //     Windows 2000/XP: Browser Refresh key
        BrowserRefresh = 168,
        //
        // Summary:
        //     Windows 2000/XP: Browser Stop key
        BrowserStop = 169,
        //
        // Summary:
        //     Windows 2000/XP: Browser Search key
        BrowserSearch = 170,
        //
        // Summary:
        //     Windows 2000/XP: Browser Favorites key
        BrowserFavorites = 171,
        //
        // Summary:
        //     Windows 2000/XP: Browser Start and Home key
        BrowserHome = 172,
        //
        // Summary:
        //     Windows 2000/XP: Volume Mute key
        VolumeMute = 173,
        //
        // Summary:
        //     Windows 2000/XP: Volume Down key
        VolumeDown = 174,
        //
        // Summary:
        //     Windows 2000/XP: Volume Up key
        VolumeUp = 175,
        //
        // Summary:
        //     Windows 2000/XP: Next Track key
        MediaNextTrack = 176,
        //
        // Summary:
        //     Windows 2000/XP: Previous Track key
        MediaPreviousTrack = 177,
        //
        // Summary:
        //     Windows 2000/XP: Stop Media key
        MediaStop = 178,
        //
        // Summary:
        //     Windows 2000/XP: Play/Pause Media key
        MediaPlayPause = 179,
        //
        // Summary:
        //     Windows 2000/XP: Start Mail key
        LaunchMail = 180,
        //
        // Summary:
        //     Windows 2000/XP: Select Media key
        SelectMedia = 181,
        //
        // Summary:
        //     Windows 2000/XP: Start Application 1 key
        LaunchApplication1 = 182,
        //
        // Summary:
        //     Windows 2000/XP: Start Application 2 key
        LaunchApplication2 = 183,
        //
        // Summary:
        //     Windows 2000/XP: The OEM Semicolon key on a US standard keyboard
        OemSemicolon = 186,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the '+' key
        OemPlus = 187,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the ',' key
        OemComma = 188,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the '-' key
        OemMinus = 189,
        //
        // Summary:
        //     Windows 2000/XP: For any country/region, the '.' key
        OemPeriod = 190,
        //
        // Summary:
        //     Windows 2000/XP: The OEM question mark key on a US standard keyboard
        OemQuestion = 191,
        //
        // Summary:
        //     Windows 2000/XP: The OEM tilde key on a US standard keyboard
        OemTilde = 192,
        //
        // Summary:
        //     Windows 2000/XP: The OEM open bracket key on a US standard keyboard
        OemOpenBrackets = 219,
        //
        // Summary:
        //     Windows 2000/XP: The OEM pipe key on a US standard keyboard
        OemPipe = 220,
        //
        // Summary:
        //     Windows 2000/XP: The OEM close bracket key on a US standard keyboard
        OemCloseBrackets = 221,
        //
        // Summary:
        //     Windows 2000/XP: The OEM singled/double quote key on a US standard keyboard
        OemQuotes = 222,
        //
        // Summary:
        //     Used for miscellaneous characters; it can vary by keyboard.
        Oem8 = 223,
        //
        // Summary:
        //     Windows 2000/XP: The OEM angle bracket or backslash key on the RT 102 key
        //     keyboard
        OemBackslash = 226,
        //
        // Summary:
        //     Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        ProcessKey = 229,
        //
        // Summary:
        //     Attn key
        Attn = 246,
        //
        // Summary:
        //     CrSel key
        Crsel = 247,
        //
        // Summary:
        //     ExSel key
        Exsel = 248,
        //
        // Summary:
        //     Erase EOF key
        EraseEof = 249,
        //
        // Summary:
        //     Play key
        Play = 250,
        //
        // Summary:
        //     Zoom key
        Zoom = 251,
        //
        // Summary:
        //     PA1 key
        Pa1 = 253,
        //
        // Summary:
        //     CLEAR key
        OemClear = 254,
    }

    public enum CONST_REACTOR_WATERMARK_POSITION : int
    {
        TOP_LEFT = 0,
        TOP_RIGHT = 1,
        BOTTOM_LEFT = 2,
        BOTTOM_RIGHT = 3
    }
    

    public enum CONST_RTEXTURE_TYPE : int
    {
        Texture2D = 0,
        Texture3D = 1,
        TextureCube = 2
    }

    public enum CONST_REACTOR_SHADERPARAM : int
    {
        FLOAT = 0,
        STRING = 1,
        BOOLEAN = 2,
        TEXTURE2D = 3,
        TEXTURE3D = 4,

    }
    #endregion

    #region TYPE CLASSES
    public struct R3DVECTOR
    {
        internal Vector3 vector;
        public R3DVECTOR(float x, float y, float z)
        {
            vector = new Vector3();
            vector.X = x;
            vector.Y = y;
            vector.Z = z;
            return;
        }
		public R3DVECTOR(float value)
		{
			vector = new Vector3(value);
			return;
		}

		public R3DVECTOR(double value)
		{
			vector = new Vector3((float)value);
			return;
		}
        
        public override string ToString()
        {
            return ""+vector.X+","+vector.Y+","+vector.Z+"";
        }
        public float Length()
        {
            return vector.Length();
        }
        public float LengthSquared()
        {
            return vector.LengthSquared();
        }
        public static float Distance(ref R3DVECTOR v1, ref R3DVECTOR v2)
        {
            float distance = 0;
            Vector3.Distance(ref v1.vector, ref v2.vector, out distance);
            return distance;
        }
        public void Normalize()
        {
            vector.Normalize();
            return;
        }
        public float X
        {
            get { return vector.X; }
            set { vector.X = value; }
        }
        public float Y
        {
            get { return vector.Y; }
            set { vector.Y = value; }
        }
        public float Z
        {
            get { return vector.Z; }
            set { vector.Z = value; }
        }
        public Vector3 ToVector3()
        {
            return R3DVECTOR.ToVector3(this);
            //return (Vector3)this;
        }
        
        public static R3DVECTOR FromVector3(Vector3 vector)
        {
            R3DVECTOR v = new R3DVECTOR();
            v.vector = vector;
            return v;
            //return (R3DVECTOR)vector;
        }
        public static Vector3 ToVector3(R3DVECTOR vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
        public static R3DVECTOR operator + (R3DVECTOR value1, R3DVECTOR value2)
        {
            value1.vector.X += value2.vector.X;
            value1.vector.Y += value2.vector.Y;
            value1.vector.Z += value2.vector.Z;
            return value1;
        }
        public static R3DVECTOR operator + (R3DVECTOR value1, float value2)
        {
            value1.vector.X += value2;
            value1.vector.Y += value2;
            value1.vector.Z += value2;
            return value1;
        }
        public static R3DVECTOR operator -(R3DVECTOR value1, R3DVECTOR value2)
        {
            value1.vector.X -= value2.X;
            value1.vector.Y -= value2.Y;
            value1.vector.Z -= value2.Z;
            return value1;
        }
        public static R3DVECTOR operator -(R3DVECTOR value1, float value2)
        {
            value1.vector.X -= value2;
            value1.vector.Y -= value2;
            value1.vector.Z -= value2;
            return value1;
        }
        public static R3DVECTOR operator *(R3DVECTOR value1, float value2)
        {
            value1.vector.X *= value2;
            value1.vector.Y *= value2;
            value1.vector.Z *= value2;
            return value1;
        }
        public static R3DVECTOR operator *(R3DVECTOR value1, R3DVECTOR value2)
        {
            value1.vector.X *= value2.X;
            value1.vector.Y *= value2.Y;
            value1.vector.Z *= value2.Z;
            return value1;
        }
        public static R3DVECTOR operator /(R3DVECTOR value1, float value2)
        {
            value1.vector.X /= value2;
            value1.vector.Y /= value2;
            value1.vector.Z /= value2;
            return value1;
        }
        public static R3DVECTOR operator /(R3DVECTOR value1, R3DVECTOR value2)
        {
            value1.vector.X /= value2.X;
            value1.vector.Y /= value2.Y;
            value1.vector.Z /= value2.Z;
            return value1;
        }
        public static bool operator !=(R3DVECTOR vector1, R3DVECTOR vector2)
        {
            if (vector1.vector != vector2.vector)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator ==(R3DVECTOR vector1, R3DVECTOR vector2)
        {
            if (vector1.vector == vector2.vector)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void Negate(ref R3DVECTOR Vector, out R3DVECTOR result)
        {
            result = new R3DVECTOR();
            result.vector = Vector3.Negate(Vector.vector);
            
        }
        public static R3DVECTOR Negate(R3DVECTOR vector)
        {
            Vector3 r = Vector3.Negate(vector.vector);
            return R3DVECTOR.FromVector3(r);
        }
        public static void Normalize(ref R3DVECTOR vector, out R3DVECTOR result)
        {
            result = R3DVECTOR.Normalize(vector);
        }
        public static R3DVECTOR Normalize(R3DVECTOR vector)
        {
            Vector3 v = Vector3.Normalize(vector.vector);
            R3DVECTOR r = R3DVECTOR.FromVector3(v);
            return r;
        }
        public static float Dot(R3DVECTOR vector1, R3DVECTOR vector2)
        {
            Vector3 v1 = vector1.ToVector3();
            Vector3 v2 = vector2.ToVector3();
            float r = Vector3.Dot(v1, v2);
            return r;
        }
        public static void Dot(ref R3DVECTOR vector1, ref R3DVECTOR vector2, out float result)
        {
            Vector3 v1 = vector1.ToVector3();
            Vector3 v2 = vector2.ToVector3();
            Vector3.Dot(ref v1, ref v2, out result);
        }
        public static R3DVECTOR Transform(R3DVECTOR value, Quaternion value1)
        {
            Vector3 v = R3DVECTOR.ToVector3(value);
            Vector3 r;
            r = Vector3.Transform(v, value1);
            return R3DVECTOR.FromVector3(r);
        }
        public static R3DVECTOR Cross(ref R3DVECTOR vector1, ref R3DVECTOR vector2, out R3DVECTOR result)
        {
            Vector3 v = vector1.ToVector3();
            Vector3 v2 = vector2.ToVector3();
            Vector3 r = Vector3.Zero;
            Vector3.Cross(ref v, ref v2, out r);
            result = R3DVECTOR.FromVector3(r);
            return result;
        }
        public static R3DVECTOR Cross(R3DVECTOR vector, R3DVECTOR vector1)
        {
            Vector3 v = vector.ToVector3();
            Vector3 v1 = vector1.ToVector3();
            Vector3 r = Vector3.Cross(v, v1);
            
            return R3DVECTOR.FromVector3(r);
        }
        public static R3DVECTOR Up
        {
            get { return R3DVECTOR.FromVector3(Vector3.Up); }
        }
        public static R3DVECTOR Down
        {
            get { return R3DVECTOR.FromVector3(Vector3.Down); }
        }
        public static R3DVECTOR Forward
        {
            get { return R3DVECTOR.FromVector3(Vector3.Forward); }
        }
        public static R3DVECTOR Left
        {
            get { return R3DVECTOR.FromVector3(Vector3.Left); }
        }
        public static R3DVECTOR Right
        {
            get { return R3DVECTOR.FromVector3(Vector3.Right); }
        }
        public static R3DVECTOR Backward
        {
            get { return R3DVECTOR.FromVector3(Vector3.Backward); }
        }
        public static R3DVECTOR Zero
        {
            get { return R3DVECTOR.FromVector3(Vector3.Zero); }
        }
        public static R3DVECTOR UnitX
        {
            get { return R3DVECTOR.FromVector3(Vector3.UnitX); }
        }
        public static R3DVECTOR UnitY
        {
            get { return R3DVECTOR.FromVector3(Vector3.UnitY); }
        }
        public static R3DVECTOR UnitZ
        {
            get { return R3DVECTOR.FromVector3(Vector3.UnitZ); }
        }
        
    }

    public struct R3DMATRIX
    {
        internal Matrix matrix;
        public float m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44;
        public R3DMATRIX(float M11, float M12, float M13, float M14, float M21, float M22, float M23, float M24, float M31, float M32, float M33, float M34, float M41, float M42, float M43, float M44)
        {
            
            matrix = new Matrix(M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
            m11 = M11;
            m12 = M12;
            m13 = M13;
            m14 = M14;

            m21 = M21;
            m22 = M22;
            m23 = M23;
            m24 = M24;

            m31 = M31;
            m32 = M32;
            m33 = M33;
            m34 = M34;

            m41 = M41;
            m42 = M42;
            m43 = M43;
            m44 = M44;
            
        }
        
        internal void Populate()
        {
            m11 = matrix.M11; m12 = matrix.M12; m13 = matrix.M13; m14 = matrix.M14;
            m21 = matrix.M21; m22 = matrix.M22; m23 = matrix.M23; m24 = matrix.M24;
            m31 = matrix.M31; m32 = matrix.M32; m33 = matrix.M33; m34 = matrix.M34;
            m41 = matrix.M41; m42 = matrix.M42; m43 = matrix.M43; m44 = matrix.M44;
        }

        public R3DVECTOR Left
        {
            get { return R3DVECTOR.FromVector3(matrix.Left); Populate(); }
            set { matrix.Left = value.ToVector3(); Populate(); }
        }
        public R3DVECTOR Right
        {
            get { return R3DVECTOR.FromVector3(matrix.Right); Populate(); }
            set { matrix.Right = value.ToVector3(); Populate(); }
        }
        public R3DVECTOR Forward
        {
            get { return R3DVECTOR.FromVector3(matrix.Forward); Populate(); }
            set { matrix.Forward = value.ToVector3(); Populate(); }
        }
        public R3DVECTOR Backward
        {
            get { return R3DVECTOR.FromVector3(matrix.Backward); Populate(); }
            set { matrix.Backward = value.ToVector3(); Populate(); }
        }
        public R3DVECTOR Up
        {
            get { return R3DVECTOR.FromVector3(matrix.Up); Populate(); }
            set { matrix.Up = value.ToVector3(); Populate(); }
        }
        public R3DVECTOR Down
        {
            get { return R3DVECTOR.FromVector3(matrix.Down); Populate(); }
            set { matrix.Down = value.ToVector3(); Populate(); }
        }
        public R3DVECTOR Translation
        {
            get { return R3DVECTOR.FromVector3(matrix.Translation); Populate(); }
            set { matrix.Translation = value.ToVector3(); Populate(); }
        }
        public Matrix ToMatrix()
        {
            return matrix;
        }
        public static Matrix ToMatrix(R3DMATRIX matrix)
        {
            return matrix.ToMatrix();
            
        }

        public static R3DMATRIX FromMatrix(Matrix matrix)
        {
            R3DMATRIX m = new R3DMATRIX();
            m.matrix = matrix;
            m.Populate();
            return m;
        }

        public static R3DMATRIX operator -(R3DMATRIX matrix1)
        {
            return R3DMATRIX.FromMatrix(-matrix1.matrix);
        }

        public static R3DMATRIX operator -(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return R3DMATRIX.FromMatrix(matrix1.matrix - matrix2.matrix);
        }

        public static bool operator !=(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            if (matrix1.matrix != matrix2.matrix)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static R3DMATRIX operator *(float scaleFactor, R3DMATRIX matrix)
        {
            return R3DMATRIX.FromMatrix(matrix.matrix * scaleFactor);
        }

        public static R3DMATRIX operator *(R3DMATRIX matrix, float scaleFactor)
        {
            return R3DMATRIX.FromMatrix(matrix.matrix * scaleFactor);
        }

        public static R3DMATRIX operator *(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return R3DMATRIX.FromMatrix(matrix1.matrix * matrix2.matrix);
        }

        public static R3DMATRIX operator /(R3DMATRIX matrix1, float divider)
        {
            return R3DMATRIX.FromMatrix(matrix1.matrix / divider);
        }

        public static R3DMATRIX operator /(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return R3DMATRIX.FromMatrix(matrix1.matrix / matrix2.matrix);
        }

        public static R3DMATRIX operator +(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return R3DMATRIX.FromMatrix(matrix1.matrix + matrix2.matrix);
        }

        public static bool operator ==(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            if (matrix1.matrix == matrix2.matrix)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static R3DMATRIX Add(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return R3DMATRIX.FromMatrix(Matrix.Add(matrix1.matrix, matrix2.matrix));
        }

        public static void Add(ref R3DMATRIX matrix1, ref R3DMATRIX matrix2, out R3DMATRIX result)
        {
            result = R3DMATRIX.FromMatrix(Matrix.Add(matrix1.matrix, matrix2.matrix));
        }

        public static R3DMATRIX Divide(R3DMATRIX matrix1, float divider)
        {
            return R3DMATRIX.FromMatrix(Matrix.Divide(matrix1.matrix, divider));
        }

        public static R3DMATRIX Divide(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return R3DMATRIX.FromMatrix(Matrix.Divide(matrix1.matrix, matrix2.matrix));
        }

        public static void Divide(ref R3DMATRIX matrix1, float divider, out R3DMATRIX result)
        {
            result = R3DMATRIX.Divide(matrix1, divider);
        }

        public static void Divide(ref R3DMATRIX matrix1, ref R3DMATRIX matrix2, out R3DMATRIX result)
        {
            result = R3DMATRIX.Divide(matrix1, matrix2);
        }

        public static R3DMATRIX Invert(R3DMATRIX matrix)
        {
            return R3DMATRIX.FromMatrix(Matrix.Invert(matrix.matrix));
        }

        public static void Invert(ref R3DMATRIX matrix, out R3DMATRIX result)
        {
            result = R3DMATRIX.Invert(matrix);
        }

        public static R3DMATRIX Lerp(R3DMATRIX matrix1, R3DMATRIX matrix2, float amount)
        {
            return R3DMATRIX.FromMatrix(Matrix.Lerp(matrix1.matrix, matrix2.matrix, amount));
        }

        public static void Lerp(ref R3DMATRIX matrix1, ref R3DMATRIX matrix2, float amount, out R3DMATRIX result)
        {
            result = R3DMATRIX.Lerp(matrix1, matrix2, amount);
        }

        public static R3DMATRIX Multiply(R3DMATRIX matrix1, float scaleFactor)
        {
            return R3DMATRIX.FromMatrix(Matrix.Multiply(matrix1.matrix, scaleFactor));
        }

        public static R3DMATRIX Multiply(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return R3DMATRIX.FromMatrix(Matrix.Multiply(matrix1.matrix, matrix2.matrix));
        }

        public static void Multiply(ref R3DMATRIX matrix1, float scaleFactor, out R3DMATRIX result)
        {
            result = R3DMATRIX.Multiply(matrix1, scaleFactor);
        }

        public static void Multiply(ref R3DMATRIX matrix1, ref R3DMATRIX matrix2, out R3DMATRIX result)
        {
            result = R3DMATRIX.Multiply(matrix1, matrix2);
        }

        public static R3DMATRIX Negate(R3DMATRIX matrix)
        {
            return R3DMATRIX.FromMatrix(Matrix.Negate(matrix.matrix));
        }

        public static void Negate(ref R3DMATRIX matrix, out R3DMATRIX result)
        {
            result = R3DMATRIX.Negate(matrix);
        }

        public static R3DMATRIX Subtract(R3DMATRIX matrix1, R3DMATRIX matrix2)
        {
            return matrix1 - matrix2;
        }

        public static void Subtract(ref R3DMATRIX matrix1, ref R3DMATRIX matrix2, out R3DMATRIX result)
        {
            result = matrix1 - matrix2;
        }


        public static R3DMATRIX Transpose(R3DMATRIX matrix)
        {
            return R3DMATRIX.FromMatrix(Matrix.Transpose(matrix.matrix));
        }

        public static void Transpose(ref R3DMATRIX matrix, out R3DMATRIX result)
        {
            result = R3DMATRIX.Transpose(matrix);
        }
    }

    public struct R4DVECTOR
    {
        internal Vector4 vector;
        public R4DVECTOR(float x, float y, float z, float w)
        {
            vector = new Vector4(x, y, z, w);
        }
        public float X
        {
            get { return vector.X; }
            set { vector.X = value; }
        }
        public float Y
        {
            get { return vector.Y; }
            set { vector.Y = value; }
        }
        public float Z
        {
            get { return vector.Z; }
            set { vector.Z = value; }
        }
        public float W
        {
            get { return vector.W; }
            set { vector.W = value; }
        }
    }

    public struct R2DVECTOR
    {
        internal Vector2 vector;
        public R2DVECTOR(float x, float y)
        {
            vector = new Vector2(x, y);
        }
        public R2DVECTOR(float value)
        {
            vector = new Vector2(value);
        }
        public float X
        {
            get { return vector.X; }
            set { vector.X = value; }
        }
        public float Y
        {
            get { return vector.Y; }
            set { vector.Y = value; }
        }
        internal static R2DVECTOR FromVector(Vector2 value)
        {
            R2DVECTOR v = new R2DVECTOR(value.X, value.Y);
            return v;
        }
        internal static Vector2 ToVector(R2DVECTOR vector)
        {
            return vector.vector;
        }
        internal Vector2 ToVector()
        { return vector; }
        public static R2DVECTOR operator -(R2DVECTOR value)
        {
            return -R2DVECTOR.FromVector(value.vector);
        }

        public static R2DVECTOR operator -(R2DVECTOR value1, R2DVECTOR value2)
        {
            R2DVECTOR v = new R2DVECTOR();
            v.vector = value1.vector - value2.vector;
            return v;
        }

        public static bool operator !=(R2DVECTOR value1, R2DVECTOR value2)
        {
            if (value1.vector != value2.vector)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public static R2DVECTOR operator *(float scaleFactor, R2DVECTOR value)
        {
            return R2DVECTOR.FromVector(scaleFactor * value.vector);
        }

        public static R2DVECTOR operator *(R2DVECTOR value, float scaleFactor)
        {
            return R2DVECTOR.FromVector(value.vector * scaleFactor);
        }

        public static R2DVECTOR operator *(R2DVECTOR value1, R2DVECTOR value2)
        {
            return R2DVECTOR.FromVector(value1.vector * value2.vector);
        }

        public static R2DVECTOR operator /(R2DVECTOR value1, float divider)
        {
            return R2DVECTOR.FromVector(value1.vector / divider);
        }

        public static R2DVECTOR operator /(R2DVECTOR value1, R2DVECTOR value2)
        {
            return R2DVECTOR.FromVector(value1.vector / value2.vector);
        }

        public static R2DVECTOR operator +(R2DVECTOR value1, R2DVECTOR value2)
        {
            return R2DVECTOR.FromVector(value1.vector + value2.vector);
        }

        public static bool operator ==(R2DVECTOR value1, R2DVECTOR value2)
        {
            if (value1.vector == value2.vector)
                return true;
            else
                return false;
        }

        public static R2DVECTOR One { get { return R2DVECTOR.FromVector(Vector2.One); } }

        public static R2DVECTOR UnitX { get { return R2DVECTOR.FromVector(Vector2.UnitX); } }

        public static R2DVECTOR UnitY { get { return R2DVECTOR.FromVector(Vector2.UnitY); } }

        public static R2DVECTOR Zero { get { return R2DVECTOR.FromVector(Vector2.Zero); } }

        public static R2DVECTOR Add(R2DVECTOR value1, R2DVECTOR value2)
        {
            return R2DVECTOR.FromVector(value1.vector + value2.vector);
        }

        public static void Add(ref R2DVECTOR value1, ref R2DVECTOR value2, out R2DVECTOR result)
        {
            result = R2DVECTOR.FromVector(value1.vector + value2.vector);
        }

        public static R2DVECTOR Barycentric(R2DVECTOR value1, R2DVECTOR value2, R2DVECTOR value3, float amount1, float amount2)
        {
            return R2DVECTOR.FromVector(Vector2.Barycentric(value1.vector, value2.vector, value3.vector, amount1, amount2));
        }

        public static void Barycentric(ref R2DVECTOR value1, ref R2DVECTOR value2, ref R2DVECTOR value3, float amount1, float amount2, out R2DVECTOR result)
        {
            Vector2 v = Vector2.Barycentric(value1.vector, value2.vector, value3.vector, amount1, amount2);
            result = R2DVECTOR.FromVector(v);
        }

        public static R2DVECTOR CatmullRom(R2DVECTOR value1, R2DVECTOR value2, R2DVECTOR value3, R2DVECTOR value4, float amount)
        {
            return R2DVECTOR.FromVector(Vector2.CatmullRom(value1.vector, value2.vector, value3.vector, value4.vector, amount));
        }

        public static void CatmullRom(ref R2DVECTOR value1, ref R2DVECTOR value2, ref R2DVECTOR value3, ref R2DVECTOR value4, float amount, out R2DVECTOR result)
        {
            Vector2 v = Vector2.CatmullRom(value1.vector, value2.vector, value3.vector, value4.vector, amount);
            result = R2DVECTOR.FromVector(v);
        }

        public static R2DVECTOR Clamp(R2DVECTOR value1, R2DVECTOR min, R2DVECTOR max)
        {
            return R2DVECTOR.FromVector(Vector2.Clamp(value1.vector, min.vector, max.vector));
        }

        public static void Clamp(ref R2DVECTOR value1, ref R2DVECTOR min, ref R2DVECTOR max, out R2DVECTOR result)
        {
            Vector2 v = Vector2.Clamp(value1.vector, min.vector, max.vector);
            result = R2DVECTOR.FromVector(v);
        }

        public static float Distance(R2DVECTOR value1, R2DVECTOR value2)
        {
            return Vector2.Distance(value1.vector, value2.vector);
        }

        public static void Distance(ref R2DVECTOR value1, ref R2DVECTOR value2, out float result)
        {
            result = Vector2.Distance(value1.vector, value2.vector);
        }

        public static float DistanceSquared(R2DVECTOR value1, R2DVECTOR value2)
        {
            return Vector2.DistanceSquared(value1.vector, value2.vector);
        }

        public static void DistanceSquared(ref R2DVECTOR value1, ref R2DVECTOR value2, out float result)
        {
            result = Vector2.DistanceSquared(value1.vector, value2.vector);
        }

        public static R2DVECTOR Divide(R2DVECTOR value1, float divider)
        {
            return R2DVECTOR.FromVector(Vector2.Divide(value1.vector, divider));
        }

        public static R2DVECTOR Divide(R2DVECTOR value1, R2DVECTOR value2)
        {
            return R2DVECTOR.FromVector(Vector2.Divide(value1.vector, value2.vector));
        }

        public static void Divide(ref R2DVECTOR value1, float divider, out R2DVECTOR result)
        {
            result = R2DVECTOR.FromVector(Vector2.Divide(value1.vector, divider));
        }

        public static void Divide(ref R2DVECTOR value1, ref R2DVECTOR value2, out R2DVECTOR result)
        {
            result = R2DVECTOR.FromVector(Vector2.Divide(value1.vector, value2.vector));
        }

        public static float Dot(R2DVECTOR value1, R2DVECTOR value2)
        {
            return Vector2.Dot(value1.vector, value2.vector);
        }

        public static void Dot(ref R2DVECTOR value1, ref R2DVECTOR value2, out float result)
        {
            result = Vector2.Dot(value1.vector, value2.vector);
        }

        public bool Equals(R2DVECTOR other)
        {
            if (vector == other.vector)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static R2DVECTOR Hermite(R2DVECTOR value1, R2DVECTOR tangent1, R2DVECTOR value2, R2DVECTOR tangent2, float amount)
        {
            return R2DVECTOR.FromVector(Vector2.Hermite(value1.vector, tangent1.vector, value2.vector, tangent2.vector, amount));
        }

        public static void Hermite(ref R2DVECTOR value1, ref R2DVECTOR tangent1, ref R2DVECTOR value2, ref R2DVECTOR tangent2, float amount, out R2DVECTOR result)
        {
            result = R2DVECTOR.FromVector(Vector2.Hermite(value1.vector, tangent1.vector, value2.vector, tangent2.vector, amount));
        }

        public float Length()
        {
            return vector.Length();
        }

        public float LengthSquared()
        { return vector.LengthSquared(); }

        public static R2DVECTOR Lerp(R2DVECTOR value1, R2DVECTOR value2, float amount)
        {
            return R2DVECTOR.FromVector(Vector2.Lerp(value1.vector, value2.vector, amount));
        }

        public static void Lerp(ref R2DVECTOR value1, ref R2DVECTOR value2, float amount, out R2DVECTOR result)
        {
            result = R2DVECTOR.FromVector(Vector2.Lerp(value1.vector, value2.vector, amount));
        }

        public static R2DVECTOR Max(R2DVECTOR value1, R2DVECTOR value2)
        {
            return R2DVECTOR.FromVector(Vector2.Max(value1.vector, value2.vector));
        }

        public static void Max(ref R2DVECTOR value1, ref R2DVECTOR value2, out R2DVECTOR result)
        {
            result = R2DVECTOR.FromVector(Vector2.Max(value1.vector, value2.vector));
        }
        /*
        public static R2DVECTOR Min(R2DVECTOR value1, R2DVECTOR value2);

        public static void Min(ref R2DVECTOR value1, ref R2DVECTOR value2, out R2DVECTOR result);

        public static R2DVECTOR Multiply(R2DVECTOR value1, float scaleFactor);

        public static R2DVECTOR Multiply(R2DVECTOR value1, R2DVECTOR value2);

        public static void Multiply(ref R2DVECTOR value1, float scaleFactor, out R2DVECTOR result);

        public static void Multiply(ref R2DVECTOR value1, ref R2DVECTOR value2, out R2DVECTOR result);

        public static R2DVECTOR Negate(R2DVECTOR value);

        public static void Negate(ref R2DVECTOR value, out R2DVECTOR result);
        
        public void Normalize();

        public static R2DVECTOR Normalize(R2DVECTOR value);

        public static void Normalize(ref R2DVECTOR value, out R2DVECTOR result);

        public static R2DVECTOR Reflect(R2DVECTOR vector, R2DVECTOR normal);

        public static void Reflect(ref R2DVECTOR vector, ref R2DVECTOR normal, out R2DVECTOR result);

        public static R2DVECTOR SmoothStep(R2DVECTOR value1, R2DVECTOR value2, float amount);

        public static void SmoothStep(ref R2DVECTOR value1, ref R2DVECTOR value2, float amount, out R2DVECTOR result);

        public static R2DVECTOR Subtract(R2DVECTOR value1, R2DVECTOR value2);

        public static void Subtract(ref R2DVECTOR value1, ref R2DVECTOR value2, out R2DVECTOR result);
        
        public override string ToString();

        public static R2DVECTOR Transform(R2DVECTOR position, R3DMATRIX matrix);

        public static R2DVECTOR Transform(R2DVECTOR value, RQUATERNION rotation);

        public static void Transform(ref R2DVECTOR position, ref Matrix matrix, out R2DVECTOR result);

        public static void Transform(ref R2DVECTOR value, ref Quaternion rotation, out R2DVECTOR result);

        public static void Transform(R2DVECTOR[] sourceArray, ref Matrix matrix, R2DVECTOR[] destinationArray);

        public static void Transform(R2DVECTOR[] sourceArray, ref Quaternion rotation, R2DVECTOR[] destinationArray);

        public static void Transform(R2DVECTOR[] sourceArray, int sourceIndex, ref Matrix matrix, R2DVECTOR[] destinationArray, int destinationIndex, int length);

        public static void Transform(R2DVECTOR[] sourceArray, int sourceIndex, ref Quaternion rotation, R2DVECTOR[] destinationArray, int destinationIndex, int length);

        public static R2DVECTOR TransformNormal(R2DVECTOR normal, Matrix matrix);

        public static void TransformNormal(ref R2DVECTOR normal, ref Matrix matrix, out R2DVECTOR result);

        public static void TransformNormal(R2DVECTOR[] sourceArray, ref Matrix matrix, R2DVECTOR[] destinationArray);

        public static void TransformNormal(R2DVECTOR[] sourceArray, int sourceIndex, ref Matrix matrix, R2DVECTOR[] destinationArray, int destinationIndex, int length); 
        */ 
    }

    public struct RQUATERNION
    {
        internal Quaternion quaternion;
        internal RQUATERNION(Quaternion q)
        {
            quaternion = q;
        }
        public RQUATERNION(float X, float Y, float Z, float W)
        {
            Quaternion q = new Quaternion(X, Y, Z, W);
            quaternion = q;
        }
        public float W
        { 
            get{ return quaternion.W; }
            set { quaternion.W = value; } 
        }
        
        public float X
        {
            get { return quaternion.X; }
            set { quaternion.X = value; }
        }
        
        public float Y
        {
            get { return quaternion.Y; }
            set { quaternion.Y = value; }
        }
        
        public float Z
        {
            get { return quaternion.Z; }
            set { quaternion.Z = value; }
        }

		internal Vector3 ToRotationXNAVector3()
		{
			return new Vector3(X, Y, Z);
		}

		internal Vector4 ToRotationXNAVector4()
		{
			return new Vector4(X, Y, Z, W);
		}

		public R3DVECTOR ToRotationVector()
		{
			return new R3DVECTOR(X, Y, Z);
		}

		public R4DVECTOR ToRotationVector4()
		{
			return new R4DVECTOR(X, Y, Z, W);
		}

		internal static Vector3 ToRotationXNAVector3(RQUATERNION quaternion)
		{
			return quaternion.ToRotationXNAVector3();
		}

		internal static Vector4 ToRotationXNAVector4(RQUATERNION quaternion)
		{
			return quaternion.ToRotationXNAVector4();
		}

		public static R3DVECTOR ToRotationVector(RQUATERNION quaternion)
		{
			return quaternion.ToRotationVector();
		}

		public static R4DVECTOR ToRotationVector4(RQUATERNION quaternion)
		{
			return quaternion.ToRotationVector4();
		}
			
        internal static RQUATERNION FromQuaternion(Quaternion q)
        {
            return new RQUATERNION(q);
        }
        public static RQUATERNION operator -(RQUATERNION quaternion)
        {
            Quaternion q = quaternion.quaternion;
            return RQUATERNION.FromQuaternion(-q);
        }

        public static RQUATERNION operator -(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            Quaternion q = quaternion1.quaternion - quaternion2.quaternion;
            return RQUATERNION.FromQuaternion(q);
        }

        public static bool operator !=(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            if (quaternion1.quaternion != quaternion2.quaternion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static RQUATERNION operator *(RQUATERNION quaternion1, float scaleFactor)
        {
            Quaternion q = quaternion1.quaternion * scaleFactor;
            return RQUATERNION.FromQuaternion(q);
        }

        public static RQUATERNION operator *(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            Quaternion q = quaternion1.quaternion * quaternion2.quaternion;
            return RQUATERNION.FromQuaternion(q);
        }

        public static RQUATERNION operator /(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            Quaternion q = quaternion1.quaternion / quaternion2.quaternion;
            return RQUATERNION.FromQuaternion(q);
        }

        public static RQUATERNION operator +(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            Quaternion q = quaternion1.quaternion + quaternion2.quaternion;
            return RQUATERNION.FromQuaternion(q);
        }

        public static bool operator ==(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            if (quaternion1.quaternion == quaternion2.quaternion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static RQUATERNION Identity { get { return RQUATERNION.FromQuaternion(new Quaternion(0, 0, 0, 1)); } }

        public static RQUATERNION Add(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            return quaternion1 + quaternion2;
        }

        public static void Add(ref RQUATERNION quaternion1, ref RQUATERNION quaternion2, out RQUATERNION result)
        {
            result = quaternion1 + quaternion2;
            return;
        }

        public static RQUATERNION Concatenate(RQUATERNION value1, RQUATERNION value2)
        {
            Quaternion q = Quaternion.Concatenate(value1.quaternion, value2.quaternion);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void Concatenate(ref RQUATERNION value1, ref RQUATERNION value2, out RQUATERNION result)
        {
            result = RQUATERNION.Concatenate(value1, value2);
            return;
        }

        public void Conjugate()
        {
            quaternion.Conjugate();
        }

        public static RQUATERNION Conjugate(RQUATERNION value)
        {
            Quaternion q = Quaternion.Conjugate(value.quaternion);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void Conjugate(ref RQUATERNION value, out RQUATERNION result)
        {
            result = RQUATERNION.Conjugate(value);
            return;
        }

        public static RQUATERNION CreateFromAxisAngle(R3DVECTOR axis, float angle)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(axis.vector, angle);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void CreateFromAxisAngle(ref R3DVECTOR axis, float angle, out RQUATERNION result)
        {
            result = RQUATERNION.CreateFromAxisAngle(axis, angle);
            return;
        }

        public static RQUATERNION CreateFromRotationMatrix(R3DMATRIX matrix)
        {
            Quaternion q = Quaternion.CreateFromRotationMatrix(matrix.matrix);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void CreateFromRotationMatrix(ref R3DMATRIX matrix, out RQUATERNION result)
        {
            result = RQUATERNION.CreateFromRotationMatrix(matrix);
            return;
        }

        public static RQUATERNION CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Quaternion q = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out RQUATERNION result)
        {
            result = RQUATERNION.CreateFromYawPitchRoll(yaw, pitch, roll);
            return;
        }

        public static RQUATERNION Divide(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            return quaternion1 / quaternion2;
        }

        public static void Divide(ref RQUATERNION quaternion1, ref RQUATERNION quaternion2, out RQUATERNION result)
        {
            result = quaternion1 / quaternion2;
            return;
        }

        public static float Dot(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            return Quaternion.Dot(quaternion1.quaternion, quaternion2.quaternion);
        }

        public static void Dot(ref RQUATERNION quaternion1, ref RQUATERNION quaternion2, out float result)
        {
            result = RQUATERNION.Dot(quaternion1, quaternion2);
            return;
        }

        public override bool Equals(object obj)
        {
            if ((RQUATERNION)obj == this)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(RQUATERNION other)
        {
            if (this == other)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public R3DMATRIX ToRotationMatrix()
        {
            return R3DMATRIX.FromMatrix(Matrix.CreateFromQuaternion(quaternion));
        }
        public static void ToRotationMatrix(RQUATERNION quaternion, out R3DMATRIX matrix)
        {
            matrix = quaternion.ToRotationMatrix();
        }
        //public override int GetHashCode();

        public static RQUATERNION Inverse(RQUATERNION quaternion)
        {
            Quaternion q = Quaternion.Inverse(quaternion.quaternion);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void Inverse(ref RQUATERNION quaternion, out RQUATERNION result)
        {
            result = RQUATERNION.Inverse(quaternion);
            return;
        }

        public float Length()
        {
            return quaternion.Length();
        }

        public float LengthSquared()
        {
            return quaternion.LengthSquared();
        }

        public static RQUATERNION Lerp(RQUATERNION quaternion1, RQUATERNION quaternion2, float amount)
        {
            Quaternion q = Quaternion.Lerp(quaternion1.quaternion, quaternion2.quaternion, amount);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void Lerp(ref RQUATERNION quaternion1, ref RQUATERNION quaternion2, float amount, out RQUATERNION result)
        {
            result = RQUATERNION.Lerp(quaternion1, quaternion2, amount);
            return;
        }

        public static RQUATERNION Multiply(RQUATERNION quaternion1, float scaleFactor)
        {
            return quaternion1 * scaleFactor;
        }

        public static RQUATERNION Multiply(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            return quaternion1 * quaternion2;
        }

        public static void Multiply(ref RQUATERNION quaternion1, float scaleFactor, out RQUATERNION result)
        {
            result = quaternion1 * scaleFactor;
            return;
        }

        public static void Multiply(ref RQUATERNION quaternion1, ref RQUATERNION quaternion2, out RQUATERNION result)
        {
            result = quaternion1 * quaternion2;
            return;
        }

        public static RQUATERNION Negate(RQUATERNION quaternion)
        {
            Quaternion q = Quaternion.Negate(quaternion.quaternion);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void Negate(ref RQUATERNION quaternion, out RQUATERNION result)
        {
            result = RQUATERNION.Negate(quaternion);
            return;
        }

        public void Normalize()
        {
            quaternion.Normalize();
        }

        public static RQUATERNION Normalize(RQUATERNION quaternion)
        {
            quaternion.Normalize();
            return quaternion;
        }

        public static void Normalize(ref RQUATERNION quaternion, out RQUATERNION result)
        {
            quaternion.Normalize();
            result = quaternion;
            return;
        }

        public static RQUATERNION Slerp(RQUATERNION quaternion1, RQUATERNION quaternion2, float amount)
        {
            Quaternion q = Quaternion.Slerp(quaternion1.quaternion, quaternion2.quaternion, amount);
            return RQUATERNION.FromQuaternion(q);
        }

        public static void Slerp(ref RQUATERNION quaternion1, ref RQUATERNION quaternion2, float amount, out RQUATERNION result)
        {
            result = RQUATERNION.Slerp(quaternion1, quaternion2, amount);
            return;
        }

        public static RQUATERNION Subtract(RQUATERNION quaternion1, RQUATERNION quaternion2)
        {
            return quaternion1 - quaternion2;
        }

        public static void Subtract(ref RQUATERNION quaternion1, ref RQUATERNION quaternion2, out RQUATERNION result)
        {
            result = quaternion1 - quaternion2;
            return;
        }

        public override string ToString()
        {
            return quaternion.ToString();
        }
    }

    public struct RFONT
    {
        internal SpriteFont _spriteFont;
        internal string _name;
        public RFONT(string name, string filename)
        {
            _spriteFont = REngine.Instance._content.Load<SpriteFont>(filename);
            _name = name;
        }
        public string Name
        {
            get { return _name; }
        }
        public R2DVECTOR Measure(string text)
        {
            try
            {
                return R2DVECTOR.FromVector(_spriteFont.MeasureString(text));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                REngine.Instance.AddToLog(e.ToString());
                return R2DVECTOR.Zero;
            }
        }
        public R2DVECTOR Measure(StringBuilder text)
        {
            try
            {
                return R2DVECTOR.FromVector(_spriteFont.MeasureString(text));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                REngine.Instance.AddToLog(e.ToString());
                return R2DVECTOR.Zero;
            }
        }
        
    }
    internal struct PointParticleVertex
    {
        // Stores the starting position of the particle.
        public Vector3 Position;

        // Stores the starting velocity of the particle.
        public Vector3 Velocity;

        // Four random values, used to make each particle look slightly different.
        public Color Random;

        // The time (in seconds) at which this particle was created.
        public float Time;


        // Describe the layout of this vertex structure.
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, VertexElementFormat.Vector3,
                                    VertexElementUsage.Position, 0),

            new VertexElement(12, VertexElementFormat.Vector3,
                                     VertexElementUsage.Normal, 0),

            new VertexElement(24, VertexElementFormat.Color,
                                     VertexElementUsage.Color, 0),

            new VertexElement(28, VertexElementFormat.Single,
                                     VertexElementUsage.TextureCoordinate, 0),
        };


        // Describe the size of this vertex structure.
        public const int SizeInBytes = 32;
    }

    public struct RLIGHT
    {

        public int LightType;
        public R3DVECTOR Position; // position of the light
        public R3DVECTOR Direction;// direction of the light.
        public float Theta;
        public float Phi;
        public float Radius;	// radius of the light 
        public bool Enabled;


	// shadows.
        

    }
    public enum RSURFACEFORMAT
    {
        
        Unknown = -1,
        
        Color = 1,
        
        Bgr32 = 2,
        
        Bgra1010102 = 3,
        
        Rgba32 = 4,
        
        Rgb32 = 5,
        //
        // Summary:
        //     (Unsigned format) 32-bit RGBA pixel format using 10 bits for each color and
        //     2 bits for alpha.
        Rgba1010102 = 6,
        //
        // Summary:
        //     (Unsigned format) 32-bit pixel format using 16 bits each for red and green.
        Rg32 = 7,
        //
        // Summary:
        //     (Unsigned format) 64-bit RGBA pixel format using 16 bits for each component.
        Rgba64 = 8,
        //
        // Summary:
        //     (Unsigned format) 16-bit BGR pixel format with 5 bits for blue, 6 bits for
        //     green, and 5 bits for red.
        Bgr565 = 9,
        //
        // Summary:
        //     (Unsigned format) 16-bit BGRA pixel format where 5 bits are reserved for
        //     each color and 1 bit is reserved for alpha.
        Bgra5551 = 10,
        //
        // Summary:
        //     (Unsigned format) 16-bit BGR pixel format where 5 bits are reserved for each
        //     color.
        Bgr555 = 11,
        //
        // Summary:
        //     (Unsigned format) 16-bit BGRA pixel format with 4 bits for each channel.
        Bgra4444 = 12,
        //
        // Summary:
        //     (Unsigned format) 16-bit BGR pixel format where 4 bits are reserved for each
        //     color.
        Bgr444 = 13,
        //
        // Summary:
        //     (Unsigned format) 16-bit BGRA format using 2 bits for blue, 3 bits each for
        //     red and green; and 8 bits for alpha.
        Bgra2338 = 14,
        //
        // Summary:
        //     (Unsigned format) 8-bit alpha only.
        Alpha8 = 15,
        //
        // Summary:
        //     (Unsigned format) 8-bit BGR texture format using 2 bits for blue, 3 bits
        //     for green, and 3 bits for red.
        Bgr233 = 16,
        //
        // Summary:
        //     (Unsigned format) 24-bit BGR pixel format with 8 bits per channel.
        Bgr24 = 17,
        //
        // Summary:
        //     (Signed format) 16-bit bump-map format using 8 bits each for u and v data.
        NormalizedByte2 = 18,
        //
        // Summary:
        //     (Signed format) 32-bit bump-map format using 8 bits for each channel.
        NormalizedByte4 = 19,
        //
        // Summary:
        //     (Signed format) 32-bit bump-map format using 16 bits for each channel.
        NormalizedShort2 = 20,
        //
        // Summary:
        //     (Signed format) 64-bit bump-map format using 16 bits for each component.
        NormalizedShort4 = 21,
        //
        // Summary:
        //     (IEEE format) 32-bit float format using 32 bits for the red channel.
        Single = 22,
        //
        // Summary:
        //     (IEEE format) 64-bit float format using 32 bits for the red channel and 32
        //     bits for the green channel.
        Vector2 = 23,
        //
        // Summary:
        //     (IEEE format) 128-bit float format using 32 bits for each channel (alpha,
        //     blue, green, red).
        Vector4 = 24,
        //
        // Summary:
        //     (Floating-point format) 16-bit float format using 16 bits for the red channel.
        HalfSingle = 25,
        //
        // Summary:
        //     (Floating-point format) 32-bit float format using 16 bits for the red channel
        //     and 16 bits for the green channel.
        HalfVector2 = 26,
        //
        // Summary:
        //     (Floating-point format) 64-bit float format using 16 bits for each channel
        //     (alpha, blue, green, red).
        HalfVector4 = 27,
        //
        // Summary:
        //     DXT1 compression texture format. The runtime will not allow an application
        //     to create a surface using a DXTn format unless the surface dimensions are
        //     multiples of 4. This applies to offscreen-plain surfaces, render targets,
        //     2D textures, cube textures, and volume textures.
        Dxt1 = 28,
        //
        // Summary:
        //     DXT2 compression texture format. The runtime will not allow an application
        //     to create a surface using a DXTn format unless the surface dimensions are
        //     multiples of 4. This applies to offscreen-plain surfaces, render targets,
        //     2D textures, cube textures, and volume textures.
        Dxt2 = 29,
        //
        // Summary:
        //     DXT3 compression texture format. The runtime will not allow an application
        //     to create a surface using a DXTn format unless the surface dimensions are
        //     multiples of 4. This applies to offscreen-plain surfaces, render targets,
        //     2D textures, cube textures, and volume textures.
        Dxt3 = 30,
        //
        // Summary:
        //     DXT4 compression texture format. The runtime will not allow an application
        //     to create a surface using a DXTn format unless the surface dimensions are
        //     multiples of 4. This applies to offscreen-plain surfaces, render targets,
        //     2D textures, cube textures, and volume textures.
        Dxt4 = 31,
        //
        // Summary:
        //     DXT5 compression texture format. The runtime will not allow an application
        //     to create a surface using a DXTn format unless the surface dimensions are
        //     multiples of 4. This applies to offscreen-plain surfaces, render targets,
        //     2D textures, cube textures, and volume textures.
        Dxt5 = 32,
        //
        // Summary:
        //     (Unsigned format) 8-bit luminance only.
        Luminance8 = 33,
        //
        // Summary:
        //     (Unsigned format) 16-bit luminance only.
        Luminance16 = 34,
        //
        // Summary:
        //     (Unsigned format) 8-bit using 4 bits each for alpha and luminance.
        LuminanceAlpha8 = 35,
        //
        // Summary:
        //     (Unsigned format) 16-bit using 8 bits each for alpha and luminance.
        LuminanceAlpha16 = 36,
        //
        // Summary:
        //     (Unsigned format) 8-bit color indexed.
        Palette8 = 37,
        //
        // Summary:
        //     (Unsigned format) 8-bit color indexed with 8 bits of alpha.
        PaletteAlpha16 = 38,
        //
        // Summary:
        //     (Mixed format) 16-bit bump-map format with luminance using 6 bits for luminance,
        //     and 5 bits each for v and u.
        NormalizedLuminance16 = 39,
        //
        // Summary:
        //     (Mixed format) 32-bit bump-map format with luminance using 8 bits for each
        //     channel.
        NormalizedLuminance32 = 40,
        //
        // Summary:
        //     (Mixed format) 32-bit bump-map format using 2 bits for alpha and 10 bits
        //     each for w, v, and u.
        NormalizedAlpha1010102 = 41,
        //
        // Summary:
        //     (Signed format) 16-bit normal compression format. The texture sampler computes
        //     the C channel from: C = sqrt(1 − U2− V2).
        NormalizedByte2Computed = 42,
        //
        // Summary:
        //     VideoYuYv format (PC98 compliance)
        VideoYuYv = 43,
        //
        // Summary:
        //     YUY2 format (PC98 compliance)
        VideoUyVy = 44,
        //
        // Summary:
        //     A 16-bit packed RGB format analogous to VideoYuYv (U0Y0, V0Y1, U2Y2, and
        //     so on). It requires a pixel pair to properly represent the color value. The
        //     first pixel in the pair contains 8 bits of green (in the low 8 bits) and
        //     8 bits of red (in the high 8 bits). The second pixel contains 8 bits of green
        //     (in the low 8 bits) and 8 bits of blue (in the high 8 bits). Together, the
        //     two pixels share the red and blue components, while each has a unique green
        //     component (R0G0, B0G1, R2G2, and so on). The texture sampler does not normalize
        //     the colors when looking up into a pixel shader; they remain in the range
        //     of 0.0f to 255.0f. This is true for all programmable pixel shader models.
        //     For the fixed-function pixel shader, the hardware should normalize to the
        //     0.f to 1.f range and essentially treat it as the VideoUyVy texture. Hardware
        //     that exposes this format must have GraphicsDeviceCapabilities.PixelShader1xMaxValue
        //     set to a value capable of handling that range.
        VideoGrGb = 45,
        //
        // Summary:
        //     A 16-bit packed RGB format analogous to VideoUyVy (Y0U0, Y1V0, Y2U2, and
        //     so on). It requires a pixel pair to properly represent the color value. The
        //     first pixel in the pair contains 8 bits of green (in the high 8 bits) and
        //     8 bits of red (in the low 8 bits). The second pixel contains 8 bits of green
        //     (in the high 8 bits) and 8 bits of blue (in the low 8 bits). Together, the
        //     two pixels share the red and blue components, while each has a unique green
        //     component (G0R0, G1B0, G2R2, and so on). The texture sampler does not normalize
        //     the colors when looking up into a pixel shader; they remain in the range
        //     of 0.0f to 255.0f. This is true for all programmable pixel shader models.
        //     For the fixed-function pixel shader, the hardware should normalize to the
        //     0.f to 1.f range and essentially treat it as the VideoUyVy texture. Hardware
        //     that exposes this format must have GraphicsDeviceCapabilities.PixelShader1xMaxValue
        //     set to a value capable of handling that range.
        VideoRgBg = 46,
        //
        // Summary:
        //     MultiElement texture (not compressed)
        Multi2Bgra32 = 47,
        //
        // Summary:
        //     (Buffer format) 32-bit depth-buffer bit depth using 24 bits for the depth
        //     channel and 8 bits for the stencil channel.
        Depth24Stencil8 = 48,
        //
        // Summary:
        //     (Buffer format) A non-lockable format that contains 24 bits of depth (in
        //     a 24-bit floating-point format − 20e4) and 8 bits of stencil.
        Depth24Stencil8Single = 49,
        //
        // Summary:
        //     (Buffer format) 32-bit depth-buffer bit depth using 24 bits for the depth
        //     channel and 4 bits for the stencil channel.
        Depth24Stencil4 = 50,
        //
        // Summary:
        //     (Buffer format) 32-bit depth-buffer bit depth using 24 bits for the depth
        //     channel.
        Depth24 = 51,
        //
        // Summary:
        //     (Buffer format) 32-bit depth-buffer bit depth.
        Depth32 = 52,
        //
        // Summary:
        //     (Buffer format) 16-bit depth-buffer bit depth.
        Depth16 = 54,
        //
        // Summary:
        //     (Buffer format) 16-bit depth-buffer bit depth where 15 bits are reserved
        //     for the depth channel and 1 bit is reserved for the stencil channel.
        Depth15Stencil1 = 56,
    }
    public struct RVERTEXFORMAT
    {
        internal Vector3 position;
        internal Vector2 texture;
        internal Vector3 normal;
        internal Vector3 tangent;
        internal Vector3 binormal;
        public static int SizeInBytes
        {
            get { return 4 * (3 + 2 + 3 + 3 + 3); }
        }
        public RVERTEXFORMAT(Vector3 Position, Vector2 Texture, Vector3 Normal, Vector3 Tangent, Vector3 Binormal)
        {
            position = Position;
            texture = Texture;
            normal = Normal;
            tangent = Tangent;
            binormal = Binormal;
        }
        /// <summary>
		/// Vertex elements for Mesh.Clone
		/// </summary>
		public static readonly VertexElement[] VertexElements =
			GenerateVertexElements();
		/// <summary>
		/// Vertex declaration for vertex buffers.
		/// </summary>
		public static readonly VertexDeclaration VertexDeclaration =
			new VertexDeclaration(VertexElements);

		/// <summary>
		/// Generate vertex declaration
		/// </summary>
		private static VertexElement[] GenerateVertexElements()
		{
			VertexElement[] decl = new VertexElement[]
				{
					// Construct new vertex declaration with tangent info
					// First the normal stuff (we should already have that)
					new VertexElement(0, VertexElementFormat.Vector3,
						VertexElementUsage.Position, 0),
					new VertexElement(12, VertexElementFormat.Vector2,
						VertexElementUsage.TextureCoordinate, 0),
					new VertexElement(20, VertexElementFormat.Vector3,
						VertexElementUsage.Normal, 0),
					new VertexElement(32, VertexElementFormat.Vector3,
						VertexElementUsage.Tangent, 0),
                    new VertexElement(44, VertexElementFormat.Vector3,
						VertexElementUsage.Binormal, 0),
					
				};
			return decl;
		} // GenerateVertexElements()
		

		#region Is decl tangent vertex declaration
		/// <summary>
		/// Returns true if decl is tangent vertex declaration.
		/// </summary>
		public static bool IsSkinnedTangentVertexDeclaration(
			VertexElement[] declaration)
		{
            return
                declaration.Length == 5 &&
                declaration[0].VertexElementUsage == VertexElementUsage.Position &&
                declaration[1].VertexElementUsage == VertexElementUsage.TextureCoordinate &&
                declaration[2].VertexElementUsage == VertexElementUsage.Normal &&
                declaration[3].VertexElementUsage == VertexElementUsage.Tangent &&
                declaration[4].VertexElementUsage == VertexElementUsage.Binormal;
				
				
		} // IsSkinnedTangentVertexDeclaration(declaration)
        #endregion

    }

	public struct RBOUNDINGBOX
	{
		public const int CornerCount = 8;

		public R3DVECTOR Min = RBOUNDINGBOX.WorldMin;

		public R3DVECTOR Max = RBOUNDINGBOX.WorldMax;

		public static R3DVECTOR WorldMin;
		public static R3DVECTOR WorldMax;

		public R3DVECTOR Center 
		{
			get { return Min + (Max - Min) / 2; }
			set { R3DVECTOR diff = value - Center;
				Min += diff;
				Max += diff;
			}
		}
		//
		// Constructors
		//
		static RBOUNDINGBOX ()
		{
			// Note: this type is marked as 'beforefieldinit'.
			RBOUNDINGBOX.WorldMax = new R3DVECTOR (3.402823E+38);
			RBOUNDINGBOX.WorldMin = new R3DVECTOR (-3.402823E+38);
		}

		public RBOUNDINGBOX (R3DVECTOR min, R3DVECTOR max)
		{
			this.Min = min;
			this.Max = max;
		}

		//
		// Static Methods
		//
		public static RBOUNDINGBOX CreateFromPoints (IEnumerable<R3DVECTOR> points)
		{
			if (points == null) {
				throw new ArgumentNullException ();
			}
			bool flag = true;
			R3DVECTOR maxVector = RBOUNDINGBOX.WorldMax;
			R3DVECTOR minVector = RBOUNDINGBOX.WorldMin;
			foreach (R3DVECTOR current in points) {
				maxVector.X = ((maxVector.X >= current.X) ? current.X : maxVector.X);
				maxVector.Y = ((maxVector.Y >= current.Y) ? current.Y : maxVector.Y);
				maxVector.Z = ((maxVector.Z >= current.Z) ? current.Z : maxVector.Z);
				minVector.X = ((minVector.X <= current.X) ? current.X : minVector.X);
				minVector.Y = ((minVector.Y <= current.Y) ? current.Y : minVector.Y);
				minVector.Z = ((minVector.Z <= current.Z) ? current.Z : minVector.Z);
				flag = false;
			}
			if (flag) {
				throw new ArgumentException ();
			}
			return new RBOUNDINGBOX (maxVector, minVector);
		}

		public static void CreateFromSphere (ref BoundingSphere sphere, out RBOUNDINGBOX result)
		{
			R3DVECTOR value = new R3DVECTOR (sphere.Radius);
			result.Min = R3DVECTOR.FromVector3(sphere.Center) - value;
			result.Max = R3DVECTOR.FromVector3(sphere.Center) + value;
		}

		public static RBOUNDINGBOX CreateFromSphere (BoundingSphere sphere)
		{
			RBOUNDINGBOX result;
			RBOUNDINGBOX.CreateFromSphere (ref sphere, out result);
			return result;
		}

		public static void CreateMerged (ref RBOUNDINGBOX original, ref RBOUNDINGBOX additional, out RBOUNDINGBOX result)
		{
			float rmX = Math.Min (original.Min.X, additional.Min.X);
			float rmY = Math.Min (original.Min.Y, additional.Min.Y);
			float rmZ = Math.Min (original.Min.Z, additional.Min.Z);
			float rxX = Math.Max (original.Max.X, additional.Max.X);
			float rxY = Math.Max (original.Max.Y, additional.Max.Y);
			float rxZ = Math.Max (original.Max.Z, additional.Max.Z);
			result.Min = new R3DVECTOR(rmX, rmY, rmZ);
			result.Max = new R3DVECTOR(rxX, rxY, rxZ);
		}

		public static RBOUNDINGBOX CreateMerged (RBOUNDINGBOX original, RBOUNDINGBOX additional)
		{
			RBOUNDINGBOX result;
			RBOUNDINGBOX.CreateMerged (ref original, ref additional, out result);
			return result;
		}

		//
		// Methods
		//
		public ContainmentType Contains (RBOUNDINGBOX box)
		{
			if (box.Max.X < this.Min.X || box.Min.X > this.Max.X || box.Max.Y < this.Min.Y || box.Min.Y > this.Max.Y || box.Max.Z < this.Min.Z || box.Min.Z > this.Max.Z) {
				return ContainmentType.Disjoint;
			}
			if (box.Min.X >= this.Min.X && box.Max.X <= this.Max.X && box.Min.Y >= this.Min.Y && box.Max.Y <= this.Max.Y && box.Min.Z >= this.Min.Z && box.Max.Z <= this.Max.Z) {
				return ContainmentType.Contains;
			}
			return ContainmentType.Intersects;
		}

		public void Contains (ref RBOUNDINGBOX box, out ContainmentType result)
		{
			result = this.Contains (box);
		}

		public ContainmentType Contains (BoundingFrustum frustum)
		{
			Vector3[] corners = frustum.GetCorners ();
			int i;
			for (i = 0; i < corners.Length; i++) {
				ContainmentType containmentType;
				this.Contains (ref corners [i], out containmentType);
				if (containmentType == ContainmentType.Disjoint) {
					break;
				}
			}
			if (i == corners.Length) {
				return ContainmentType.Contains;
			}
			if (i != 0) {
				return ContainmentType.Intersects;
			}
			for (i++; i < corners.Length; i++) {
				ContainmentType containmentType;
				this.Contains (ref corners [i], out containmentType);
				if (containmentType != ContainmentType.Contains) {
					return ContainmentType.Intersects;
				}
			}
			return ContainmentType.Contains;
		}

		public ContainmentType Contains (BoundingSphere sphere)
		{
			if (sphere.Center.X - this.Min.X >= sphere.Radius && sphere.Center.Y - this.Min.Y >= sphere.Radius && sphere.Center.Z - this.Min.Z >= sphere.Radius && this.Max.X - sphere.Center.X >= sphere.Radius && this.Max.Y - sphere.Center.Y >= sphere.Radius && this.Max.Z - sphere.Center.Z >= sphere.Radius) {
				return ContainmentType.Contains;
			}
			double num = 0;
			double num2 = (double)(sphere.Center.X - this.Min.X);
			if (num2 < 0) {
				if (num2 < (double)(-(double)sphere.Radius)) {
					return ContainmentType.Disjoint;
				}
				num += num2 * num2;
			}
			else {
				num2 = (double)(sphere.Center.X - this.Max.X);
				if (num2 > 0) {
					if (num2 > (double)sphere.Radius) {
						return ContainmentType.Disjoint;
					}
					num += num2 * num2;
				}
			}
			num2 = (double)(sphere.Center.Y - this.Min.Y);
			if (num2 < 0) {
				if (num2 < (double)(-(double)sphere.Radius)) {
					return ContainmentType.Disjoint;
				}
				num += num2 * num2;
			}
			else {
				num2 = (double)(sphere.Center.Y - this.Max.Y);
				if (num2 > 0) {
					if (num2 > (double)sphere.Radius) {
						return ContainmentType.Disjoint;
					}
					num += num2 * num2;
				}
			}
			num2 = (double)(sphere.Center.Z - this.Min.Z);
			if (num2 < 0) {
				if (num2 < (double)(-(double)sphere.Radius)) {
					return ContainmentType.Disjoint;
				}
				num += num2 * num2;
			}
			else {
				num2 = (double)(sphere.Center.Z - this.Max.Z);
				if (num2 > 0) {
					if (num2 > (double)sphere.Radius) {
						return ContainmentType.Disjoint;
					}
					num += num2 * num2;
				}
			}
			if (num <= (double)(sphere.Radius * sphere.Radius)) {
				return ContainmentType.Intersects;
			}
			return ContainmentType.Disjoint;
		}

		public void Contains (ref BoundingSphere sphere, out ContainmentType result)
		{
			result = this.Contains (sphere);
		}

		public ContainmentType Contains (R3DVECTOR point)
		{
			ContainmentType result;
			this.Contains (ref point, out result);
			return result;
		}

		public void Contains (ref R3DVECTOR point, out ContainmentType result)
		{
			if (point.X < this.Min.X || point.X > this.Max.X || point.Y < this.Min.Y || point.Y > this.Max.Y || point.Z < this.Min.Z || point.Z > this.Max.Z) {
				result = ContainmentType.Disjoint;
			}
			else {
				if (point.X == this.Min.X || point.X == this.Max.X || point.Y == this.Min.Y || point.Y == this.Max.Y || point.Z == this.Min.Z || point.Z == this.Max.Z) {
					result = ContainmentType.Intersects;
				}
				else {
					result = ContainmentType.Contains;
				}
			}
		}
		internal void Contains (ref Vector3 point, out ContainmentType result)
		{
			if (point.X < this.Min.X || point.X > this.Max.X || point.Y < this.Min.Y || point.Y > this.Max.Y || point.Z < this.Min.Z || point.Z > this.Max.Z) {
				result = ContainmentType.Disjoint;
			}
			else {
				if (point.X == this.Min.X || point.X == this.Max.X || point.Y == this.Min.Y || point.Y == this.Max.Y || point.Z == this.Min.Z || point.Z == this.Max.Z) {
					result = ContainmentType.Intersects;
				}
				else {
					result = ContainmentType.Contains;
				}
			}
		}

		public bool Equals (RBOUNDINGBOX other)
		{
			return this.Min == other.Min && this.Max == other.Max;
		}

		public override bool Equals (object obj)
		{
			return obj is RBOUNDINGBOX && this.Equals ((RBOUNDINGBOX)obj);
		}

		public R3DVECTOR[] GetCorners ()
		{
			return new R3DVECTOR[] {
				new R3DVECTOR (this.Min.X, this.Max.Y, this.Max.Z),
				new R3DVECTOR (this.Max.X, this.Max.Y, this.Max.Z),
				new R3DVECTOR (this.Max.X, this.Min.Y, this.Max.Z),
				new R3DVECTOR (this.Min.X, this.Min.Y, this.Max.Z),
				new R3DVECTOR (this.Min.X, this.Max.Y, this.Min.Z),
				new R3DVECTOR (this.Max.X, this.Max.Y, this.Min.Z),
				new R3DVECTOR (this.Max.X, this.Min.Y, this.Min.Z),
				new R3DVECTOR (this.Min.X, this.Min.Y, this.Min.Z)
			};
		}

		public void GetCorners (R3DVECTOR[] corners)
		{
			if (corners == null) {
				throw new ArgumentNullException ("corners");
			}
			if (corners.Length < 8) {
				throw new ArgumentOutOfRangeException ("corners", "Not Enought Corners");
			}
			corners [0].X = this.Min.X;
			corners [0].Y = this.Max.Y;
			corners [0].Z = this.Max.Z;
			corners [1].X = this.Max.X;
			corners [1].Y = this.Max.Y;
			corners [1].Z = this.Max.Z;
			corners [2].X = this.Max.X;
			corners [2].Y = this.Min.Y;
			corners [2].Z = this.Max.Z;
			corners [3].X = this.Min.X;
			corners [3].Y = this.Min.Y;
			corners [3].Z = this.Max.Z;
			corners [4].X = this.Min.X;
			corners [4].Y = this.Max.Y;
			corners [4].Z = this.Min.Z;
			corners [5].X = this.Max.X;
			corners [5].Y = this.Max.Y;
			corners [5].Z = this.Min.Z;
			corners [6].X = this.Max.X;
			corners [6].Y = this.Min.Y;
			corners [6].Z = this.Min.Z;
			corners [7].X = this.Min.X;
			corners [7].Y = this.Min.Y;
			corners [7].Z = this.Min.Z;
		}

		public override int GetHashCode ()
		{
			return this.Min.GetHashCode () + this.Max.GetHashCode ();
		}

		public void Intersects (ref Ray ray, out float? result)
		{
			result = this.Intersects (ray);
		}

		public float? Intersects (Ray ray)
		{
			return ray.Intersects (new BoundingBox(this.Min.ToVector3(), this.Max.ToVector3()));
		}

		public void Intersects (ref Plane plane, out PlaneIntersectionType result)
		{
			R3DVECTOR vector = new R3DVECTOR();
			R3DVECTOR vector2 = new R3DVECTOR();
			if (plane.Normal.X >= 0) {
				vector.X = this.Max.X;
				vector2.X = this.Min.X;
			}
			else {
				vector.X = this.Min.X;
				vector2.X = this.Max.X;
			}
			if (plane.Normal.Y >= 0) {
				vector.Y = this.Max.Y;
				vector2.Y = this.Min.Y;
			}
			else {
				vector.Y = this.Min.Y;
				vector2.Y = this.Max.Y;
			}
			if (plane.Normal.Z >= 0) {
				vector.Z = this.Max.Z;
				vector2.Z = this.Min.Z;
			}
			else {
				vector.Z = this.Min.Z;
				vector2.Z = this.Max.Z;
			}
			float num = R3DVECTOR.Dot (R3DVECTOR.FromVector3(plane.Normal), vector2) + plane.D;
			if (num > 0) {
				result = PlaneIntersectionType.Front;
				return;
			}
			num = R3DVECTOR.Dot (R3DVECTOR.FromVector3(plane.Normal), vector) + plane.D;
			if (num < 0) {
				result = PlaneIntersectionType.Back;
				return;
			}
			result = PlaneIntersectionType.Intersecting;
		}

		public PlaneIntersectionType Intersects (Plane plane)
		{
			PlaneIntersectionType result;
			this.Intersects (ref plane, out result);
			return result;
		}

		public bool Intersects (RBOUNDINGBOX box)
		{
			bool result;
			this.Intersects (ref box, out result);
			return result;
		}

		public void Intersects (ref RBOUNDINGBOX box, out bool result)
		{
			if (this.Max.X < box.Min.X || this.Min.X > box.Max.X) {
				result = false;
				return;
			}
			if (this.Max.Y < box.Min.Y || this.Min.Y > box.Max.Y) {
				result = false;
				return;
			}
			result = (this.Max.Z >= box.Min.Z && this.Min.Z <= box.Max.Z);
		}

		public bool Intersects (BoundingFrustum frustum)
		{
			return frustum.Intersects (new BoundingBox(this.Min.ToVector3(), this.Max.ToVector3()));
		}

		public bool Intersects (BoundingSphere sphere)
		{
			if (sphere.Center.X - this.Min.X > sphere.Radius && sphere.Center.Y - this.Min.Y > sphere.Radius && sphere.Center.Z - this.Min.Z > sphere.Radius && this.Max.X - sphere.Center.X > sphere.Radius && this.Max.Y - sphere.Center.Y > sphere.Radius && this.Max.Z - sphere.Center.Z > sphere.Radius) {
				return true;
			}
			double num = 0;
			if (sphere.Center.X - this.Min.X <= sphere.Radius) {
				num += (double)((sphere.Center.X - this.Min.X) * (sphere.Center.X - this.Min.X));
			}
			else {
				if (this.Max.X - sphere.Center.X <= sphere.Radius) {
					num += (double)((sphere.Center.X - this.Max.X) * (sphere.Center.X - this.Max.X));
				}
			}
			if (sphere.Center.Y - this.Min.Y <= sphere.Radius) {
				num += (double)((sphere.Center.Y - this.Min.Y) * (sphere.Center.Y - this.Min.Y));
			}
			else {
				if (this.Max.Y - sphere.Center.Y <= sphere.Radius) {
					num += (double)((sphere.Center.Y - this.Max.Y) * (sphere.Center.Y - this.Max.Y));
				}
			}
			if (sphere.Center.Z - this.Min.Z <= sphere.Radius) {
				num += (double)((sphere.Center.Z - this.Min.Z) * (sphere.Center.Z - this.Min.Z));
			}
			else {
				if (this.Max.Z - sphere.Center.Z <= sphere.Radius) {
					num += (double)((sphere.Center.Z - this.Max.Z) * (sphere.Center.Z - this.Max.Z));
				}
			}
			return num <= (double)(sphere.Radius * sphere.Radius);
		}

		public void Intersects (ref BoundingSphere sphere, out bool result)
		{
			result = this.Intersects (sphere);
		}

		public override string ToString ()
		{
			return string.Format ("{{Min:{0} Max:{1}}}", this.Min.ToString (), this.Max.ToString ());
		}

		//
		// Operators
		//
		public static bool operator == (RBOUNDINGBOX a, RBOUNDINGBOX b) {
			return a.Equals (b);
		}

		public static bool operator != (RBOUNDINGBOX a, RBOUNDINGBOX b) {
			return !a.Equals (b);
		}

		internal BoundingBox ToBoundingBox()
		{
			return new BoundingBox(Min.ToVector3(), Max.ToVector3());
		}
	}
    #endregion
}