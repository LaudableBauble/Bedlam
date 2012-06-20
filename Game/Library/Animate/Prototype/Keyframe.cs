using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Library.Animate.Prototype
{
    /// <summary>
    /// Keyframes are used to accurately determine the bone rotations at a specified time so that a skeleton can be made to animate according to wishes.
    /// </summary>
    public class Keyframe
    {
        #region Fields
        private int _FrameNumber;
        private List<Bone> _BonesToBe;
        private List<float> _BlendFactors;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a keyframe.
        /// </summary>
        /// <param name="frameNumber">The index of the frame.</param>
        public Keyframe(int frameNumber)
        {
            //Initialize some variables.
            Initialize(frameNumber, new List<Bone>());
        }
        /// <summary>
        /// Create a keyframe.
        /// </summary>
        /// <param name="frameNumber">The index of the frame.</param>
        /// <param name="bonesToBe">A list of bones that will be animated in some way.</param>
        public Keyframe(int frameNumber, List<Bone> bonesToBe)
        {
            //Initialize some variables.
            Initialize(frameNumber, bonesToBe);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the keyframe.
        /// </summary>
        /// <param name="frameNumber">The index of the frame.</param>
        /// <param name="bonesToBe">A list of bones that will be animated in some way.</param>
        public void Initialize(int frameNumber, List<Bone> bonesToBe)
        {
            //Initialize some variables.
            _FrameNumber = frameNumber;
            _BonesToBe = bonesToBe;
            _BlendFactors = new List<float>();
        }

        /// <summary>
        /// Add a bone to the keyframe, thus enabling an animation to occur with that bone.
        /// </summary>
        /// <param name="bone">A copy of a bone modified to act as an animation goal for the source bone.</param>
        public void AddBone(Bone bone)
        {
            //If the bone has been added before, swap that bone with this one.
            if (ExistsBone(bone.Index)) { SwapBone(bone); }
            //Otherwise just add a bone to be into the list and create a default blend factor.
            else { _BonesToBe.Add(bone); _BlendFactors.Add(1f); }
        }
        /// <summary>
        /// Remove a bone from the keyframe.
        /// </summary>
        /// <param name="bone">The bone to remove from the keyframe.</param>
        public void RemoveBone(Bone bone)
        {
            //Try and remove a bone to be from the list.
            try { _BonesToBe.Remove(bone); }
            catch { }
        }
        /// <summary>
        /// Remove a bone from the keyframe.
        /// </summary>
        /// <param name="index">The index of the bone, that is the index it has in its skeleton's list.</param>
        public void RemoveBone(int index)
        {
            //Remove a bone to be from the list.
            _BonesToBe.Remove(_BonesToBe.Find(bone => (bone.Index == index)));
        }
        /// <summary>
        /// Whether a bone exists in this keyframe or not.
        /// </summary>
        /// <param name="index">The index of the bone.</param>
        /// <returns>Whether the bone exists.</returns>
        public bool ExistsBone(int index)
        {
            //Whether the bone exists in this keyframe or not.
            return (_BonesToBe.Exists(bone => (bone.Index == index)));
        }
        /// <summary>
        /// Get a bone.
        /// </summary>
        /// <param name="index">The index of the bone.</param>
        /// <returns>The bone.</returns>
        public Bone GetBone(int index)
        {
            //Whether the bone exists in this keyframe or not.
            return (_BonesToBe.Find(bone => (bone.Index == index)));
        }
        /// <summary>
        /// Get a bone's position in list.
        /// </summary>
        /// <param name="index">The index of the bone.</param>
        /// <returns>The bone's position in the list.<returns>
        public int GetBonePosition(int index)
        {
            //Return the position of the bone in the list.
            return (_BonesToBe.FindIndex(bone => (bone.Index == index)));
        }
        /// <summary>
        /// Swap a bone with another bone.
        /// </summary>
        /// <param name="bone">The bone to swap.</param>
        public void SwapBone(Bone bone)
        {
            //Swap the bones.
            if (ExistsBone(bone.Index)) { _BonesToBe[GetBonePosition(bone.Index)] = bone; }
        }
        /// <summary>
        /// Set this keyframe to the state of a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to mimick.</param>
        public void SetBones(Skeleton skeleton)
        {
            //Clear the list of bones.
            _BonesToBe.Clear();
            //Add all bones in the skeleton to the keyframe.
            foreach (Bone bone in skeleton.Bones) { AddBone(bone.DeepClone()); }
        }
        /// <summary>
        /// Set the blend factor for a certain bone.
        /// </summary>
        /// <param name="index">The index of the bone.</param>
        /// <param name="factor">The blend factor.</param>
        public void SetBlendFactor(int index, float factor)
        {
            //If the bone exists in this keyframe, set its blend factor to the new value.
            if (ExistsBone(index)) { _BlendFactors[GetBonePosition(index)] = MathHelper.Clamp(factor, 0, 1); }
        }
        /// <summary>
        /// Get the blend factor for a certain bone.
        /// </summary>
        /// <param name="bone">The bone.</param>
        /// <returns>The blend factor.</returns>
        public float GetBlendFactor(Bone bone)
        {
            //If the bone exists in the keyframe, return its blend factor.
            if (ExistsBone(bone.Index)) { return _BlendFactors[GetBonePosition(bone.Index)]; }

            //If no bone was found, return zero.
            return 0;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The frame number that this keyframe has.
        /// </summary>
        public int FrameNumber
        {
            get { return _FrameNumber; }
            set { _FrameNumber = value; }
        }
        /// <summary>
        /// The list of bones that are going to rotate at this keyframe.
        /// </summary>
        public List<Bone> BonesToBe
        {
            get { return _BonesToBe; }
            set { _BonesToBe = value; }
        }
        /// <summary>
        /// The list of blend factors that govern the influence this keyframe has to alter a bone.
        /// </summary>
        public List<float> BlendFactors
        {
            get { return _BlendFactors; }
            set { _BlendFactors = value; }
        }
        #endregion
    }
}
