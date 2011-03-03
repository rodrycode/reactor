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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Audio;
// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;
using Reactor.Content;
namespace Reactor.Content.Importer
{
    
    
    #region TypeWriter
    /// <summary>
    /// Writes ModelAnimation objects into compiled XNB format.
    /// </summary>
    [ContentTypeWriter]
    internal class SkinningDataWriter : ContentTypeWriter<SkinningData>
    {
        protected override void Write(ContentWriter output, SkinningData value)
        {
            output.WriteObject(value.AnimationClips);
            output.WriteObject(value.BindPose);
            output.WriteObject(value.InverseBindPose);
            output.WriteObject(value.SkeletonHierarchy);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(SkinningDataReader).AssemblyQualifiedName;
        }
    }


    /// <summary>
    /// Writes AnimationClip objects into compiled XNB format
    /// </summary>
    [ContentTypeWriter]
    internal class AnimationClipWriter : ContentTypeWriter<AnimationClip>
    {
        protected override void Write(ContentWriter output, AnimationClip value)
        {
            output.WriteObject(value.Duration);
            output.WriteObject(value.Keyframes);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimationClipReader).AssemblyQualifiedName;
        }
    }


    /// <summary>
    /// Writes Keyframe objects into compiled XNB format
    /// </summary>
    [ContentTypeWriter]
    internal class KeyframeWriter : ContentTypeWriter<Keyframe>
    {
        protected override void Write(ContentWriter output, Keyframe value)
        {
            output.WriteObject(value.Bone);
            output.WriteObject(value.Time);
            output.WriteObject(value.Transform);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(KeyframeReader).AssemblyQualifiedName;
        }
    }
    #endregion
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Reactor 3D Effect")]
    public class REffectProcessor : EffectProcessor
    {
        public override CompiledEffect Process(EffectContent input, ContentProcessorContext context)
        {
            string effect = input.EffectCode;
            effect = "shared Light lights[8]; \r\n" + effect;
            if (effect.Contains("RPointLight"))
            {

            }
            return base.Process(input, context);
        }
    }
    [ContentImporter(".fbx", DisplayName = "Reactor 3D Actor Importer")]
    public class RActorImporter : FbxImporter
    {
        public override NodeContent Import(string filename, ContentImporterContext context)
        {
            return base.Import(filename, context);
        }
    }
    
    [ContentImporter(".fx", DisplayName = "Reactor 3D Effect Importer")]
    public class REffectImporter : EffectImporter
    {

        public override EffectContent Import(string filename, ContentImporterContext context)
        {
            return base.Import(filename, context);
        }
    }
}