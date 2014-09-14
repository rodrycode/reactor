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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Reactor.Content
{
    
    #region Keyframe
    public class Keyframe
    {
        #region Fields

        int boneValue;
        TimeSpan timeValue;
        Matrix transformValue;

        #endregion


        /// <summary>
        /// Constructs a new keyframe object.
        /// </summary>
        public Keyframe(int bone, TimeSpan time, Matrix transform)
        {
            boneValue = bone;
            timeValue = time;
            transformValue = transform;
        }


        /// <summary>
        /// Gets the index of the target bone that is animated by this keyframe.
        /// </summary>
        public int Bone
        {
            get { return boneValue; }
        }


        /// <summary>
        /// Gets the time offset from the start of the animation to this keyframe.
        /// </summary>
        public TimeSpan Time
        {
            get { return timeValue; }
        }


        /// <summary>
        /// Gets the bone transform for this keyframe.
        /// </summary>
        public Matrix Transform
        {
            get { return transformValue; }
        }
    }
    #endregion
    #region AnimationClip

    /// <summary>
    /// An animation clip is the runtime equivalent of the
    /// Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    /// It holds all the keyframes needed to describe a single animation.
    /// </summary>
    public class AnimationClip
    {
        /// <summary>
        /// Constructs a new animation clip object.
        /// </summary>
        public AnimationClip(TimeSpan duration, IList<Keyframe> keyframes)
        {
            durationValue = duration;
            keyframesValue = keyframes;
        }


        /// <summary>
        /// Gets the total length of the animation.
        /// </summary>
        public TimeSpan Duration
        {
            get { return durationValue; }
        }

        TimeSpan durationValue;


        /// <summary>
        /// Gets a combined list containing all the keyframes for all bones,
        /// sorted by time.
        /// </summary>
        public IList<Keyframe> Keyframes
        {
            get { return keyframesValue; }
        }

        IList<Keyframe> keyframesValue;
    }

    #endregion
    #region SkinningData
    public class SkinningData
    {
        #region Fields

        IDictionary<string, AnimationClip> animationClipsValue;
        IList<Matrix> bindPoseValue;
        IList<Matrix> inverseBindPoseValue;
        IList<int> skeletonHierarchyValue;

        #endregion


        /// <summary>
        /// Constructs a new skinning data object.
        /// </summary>
        public SkinningData(IDictionary<string, AnimationClip> animationClips,
                            IList<Matrix> bindPose, IList<Matrix> inverseBindPose,
                            IList<int> skeletonHierarchy)
        {
            animationClipsValue = animationClips;
            bindPoseValue = bindPose;
            inverseBindPoseValue = inverseBindPose;
            skeletonHierarchyValue = skeletonHierarchy;
        }


        /// <summary>
        /// Gets a collection of animation clips. These are stored by name in a
        /// dictionary, so there could for instance be clips for "Walk", "Run",
        /// "JumpReallyHigh", etc.
        /// </summary>
        public IDictionary<string, AnimationClip> AnimationClips
        {
            get { return animationClipsValue; }
        }

        public IList<AnimationClip> AnimationIndexes
        {
            get
            {
                List<AnimationClip> clips = new List<AnimationClip>(animationClipsValue.Values);
                return clips;
            }
        }
        /// <summary>
        /// Bindpose matrices for each bone in the skeleton,
        /// relative to the parent bone.
        /// </summary>
        public IList<Matrix> BindPose
        {
            get { return bindPoseValue; }
        }


        /// <summary>
        /// Vertex to bonespace transforms for each bone in the skeleton.
        /// </summary>
        public IList<Matrix> InverseBindPose
        {
            get { return inverseBindPoseValue; }
        }


        /// <summary>
        /// For each bone in the skeleton, stores the index of the parent bone.
        /// </summary>
        public IList<int> SkeletonHierarchy
        {
            get { return skeletonHierarchyValue; }
        }
    }


    /// <summary>
    /// Loads SkinningData objects from compiled XNB format.
    /// </summary>

    #endregion
    #region TypeReader

    /// <summary>
    /// Loads SkinningData objects from compiled XNB format.
    /// </summary>
    public class SkinningDataReader : ContentTypeReader<SkinningData>
    {
        protected override SkinningData Read(ContentReader input,
                                             SkinningData existingInstance)
        {
            IDictionary<string, AnimationClip> animationClips;
            IList<Matrix> bindPose, inverseBindPose;
            IList<int> skeletonHierarchy;

            animationClips = input.ReadObject<IDictionary<string, AnimationClip>>();
            bindPose = input.ReadObject<IList<Matrix>>();
            inverseBindPose = input.ReadObject<IList<Matrix>>();
            skeletonHierarchy = input.ReadObject<IList<int>>();

            return new SkinningData(animationClips, bindPose,
                                    inverseBindPose, skeletonHierarchy);
        }
    }


    /// <summary>
    /// Loads AnimationClip objects from compiled XNB format.
    /// </summary>
    public class AnimationClipReader : ContentTypeReader<AnimationClip>
    {
        protected override AnimationClip Read(ContentReader input,
                                              AnimationClip existingInstance)
        {
            TimeSpan duration = input.ReadObject<TimeSpan>();
            IList<Keyframe> keyframes = input.ReadObject<IList<Keyframe>>();

            return new AnimationClip(duration, keyframes);
        }
    }


    /// <summary>
    /// Loads Keyframe objects from compiled XNB format.
    /// </summary>
    public class KeyframeReader : ContentTypeReader<Keyframe>
    {
        protected override Keyframe Read(ContentReader input,
                                         Keyframe existingInstance)
        {
            int bone = input.ReadObject<int>();
            TimeSpan time = input.ReadObject<TimeSpan>();
            Matrix transform = input.ReadObject<Matrix>();

            return new Keyframe(bone, time, transform);
        }
    }
    #endregion
    #region AnimationPlayer
    /// <summary>
    /// The animation player is in charge of decoding bone position
    /// matrices from an animation clip.
    /// </summary>
    public class AnimationPlayer
    {
        #region Fields


        // Information about the currently playing animation clip.
        AnimationClip currentClipValue;
        TimeSpan currentTimeValue;
        int currentKeyframe;


        // Current animation transform matrices.
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinTransforms;


        // Backlink to the bind pose and skeleton hierarchy data.
        SkinningData skinningDataValue;


        #endregion


        /// <summary>
        /// Constructs a new animation player.
        /// </summary>
        public AnimationPlayer(SkinningData skinningData)
        {
            if (skinningData == null)
                throw new ArgumentNullException("skinningData");

            skinningDataValue = skinningData;

            boneTransforms = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransforms = new Matrix[skinningData.BindPose.Count];
        }

        internal float blendFactor = 0.0f;
        /// <summary>
        /// Starts decoding the specified animation clip.
        /// </summary>
        public void StartClip(AnimationClip clip, float blendfactor)
        {
            if (clip == null)
                throw new ArgumentNullException("clip");
            blendFactor = blendfactor;
            currentClipValue = clip;
            currentTimeValue = TimeSpan.Zero;
            currentKeyframe = 0;

            // Initialize bone transforms to the bind pose.
            skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
        }


        /// <summary>
        /// Advances the current animation position.
        /// </summary>
        public void Update(TimeSpan time, bool relativeToCurrentTime,
                           Matrix rootTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();
        }


        /// <summary>
        /// Helper used by the Update method to refresh the BoneTransforms data.
        /// </summary>
        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            if (currentClipValue == null)
                throw new InvalidOperationException(
                            "internal RActor.Update was called before StartClip");

            // Update the animation position.
            if (relativeToCurrentTime)
            {
                time += currentTimeValue;

                // If we reached the end, loop back to the start.
                while (time >= currentClipValue.Duration)
                    time -= currentClipValue.Duration;
            }

            if ((time < TimeSpan.Zero) || (time >= currentClipValue.Duration))
                throw new ArgumentOutOfRangeException("time");

            // If the position moved backwards, reset the keyframe index.
            if (time < currentTimeValue)
            {
                currentKeyframe = 0;
                skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
            }

            currentTimeValue = time;

            // Read keyframe matrices.
            IList<Keyframe> keyframes = currentClipValue.Keyframes;

            while (currentKeyframe < keyframes.Count)
            {
                Keyframe keyframe = keyframes[currentKeyframe];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > currentTimeValue)
                    break;

                // Use this keyframe.
                boneTransforms[keyframe.Bone] = keyframe.Transform;

                currentKeyframe++;
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the WorldTransforms data.
        /// </summary>
        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // Child bones.
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                int parentBone = skinningDataValue.SkeletonHierarchy[bone];

                worldTransforms[bone] = boneTransforms[bone] *
                                             worldTransforms[parentBone];
            }
        }


        /// <summary>
        /// Helper used by the Update method to refresh the SkinTransforms data.
        /// </summary>
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningDataValue.InverseBindPose[bone] *
                                            worldTransforms[bone];
            }
        }


        /// <summary>
        /// Gets the current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }


        /// <summary>
        /// Gets the current bone transform matrices,
        /// relative to the skinning bind pose.
        /// </summary>
        public Matrix[] GetSkinTransforms()
        {
            return skinTransforms;
        }


        /// <summary>
        /// Gets the clip currently being decoded.
        /// </summary>
        public AnimationClip CurrentClip
        {
            get { return currentClipValue; }
        }


        /// <summary>
        /// Gets the current play position.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return currentTimeValue; }
        }
    }
    #endregion
    
}
