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

namespace Library.Animate
{
    /// <summary>
    /// An animation is basically a list of keyframes that together lay the key steps of the animation
    /// and trust some sort of interpolation to do the fine tuning in between.
    /// </summary>
    public class Animation
    {
        #region Fields
        private Skeleton _Skeleton;
        private List<Keyframe> _Keyframes;
        private string _Name;
        private float _FrameTime;
        private int _NumberOfFrames;
        private int _CurrentFrameIndex;
        private int _NextFrameIndex;
        private bool _IsActive;
        private float _TotalElapsedTime;
        private float _Strength;
        #endregion

        #region Events
        public delegate void KeyframeHandler(object obj, EventArgs e);

        /// <summary>
        /// An event fired when a keyframe has been added to the animation.
        /// </summary>
        public event KeyframeHandler KeyframeAdded;
        /// <summary>
        /// An event fired when a keyframe has been removed to the animation.
        /// </summary>
        public event KeyframeHandler KeyframeRemoved;
        #endregion

        #region Constructor
        /// <summary>
        /// Create an animation.
        /// </summary>
        public Animation()
        {
            //Initialize stuff.
            Initialize(null);
        }
        /// <summary>
        /// Create an animation.
        /// </summary>
        /// <param name="skeleton">The skeleton this animation belongs to.</param>
        public Animation(Skeleton skeleton)
        {
            //Initialize stuff.
            Initialize(skeleton);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the animation.
        /// <param name="skeleton">The skeleton this animation belongs to.</param>
        /// </summary>
        public void Initialize(Skeleton skeleton)
        {
            //Initialize a few variables.
            _Skeleton = skeleton;
            _Keyframes = new List<Keyframe>();
            _FrameTime = .2f;
            _NumberOfFrames = 20;
            _CurrentFrameIndex = 0;
            _NextFrameIndex = 0;
            _IsActive = false;
            _TotalElapsedTime = 0;
            _Strength = 1;

            //Create the first keyframe and reset it to the state of the skeleton.
            AddKeyframe();
            ResetKeyframe(0);
        }
        /// <summary>
        /// Update the skeleton's animation.
        /// </summary>
        /// <param name="deltaSeconds">The engine time to adhere to.</param>
        public void Update(GameTime gameTime)
        {
            //If the animation is active.
            if (_IsActive)
            {
                //Add the time since the last update.
                _TotalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //If it's time to change frame.
                if (_TotalElapsedTime > _FrameTime)
                {
                    //Substract the time per frame, to be certain the next frame is drawn in time.
                    _TotalElapsedTime -= _FrameTime;

                    //Increment the frame counter.
                    _CurrentFrameIndex = _NextFrameIndex;

                    //Check whether the animation has reached its end. If it has, reset the next frame counter.
                    if (_CurrentFrameIndex == (_Keyframes[_Keyframes.Count - 1].FrameNumber)) { _NextFrameIndex = 0; }
                    //Otherwise, the next frame is the one in front of the current one.
                    else { _NextFrameIndex = (_CurrentFrameIndex + 1); }
                }

                //Transform the skeleton. Explanation: The skeleton has seized control over its own transformation.
                //TransformSkeleton();
            }
        }

        /// <summary>
        /// Transform a specified bone in the skeleton according to the state of the animation.
        /// The method tries to return an interpolation value that is calculated between the bone's rotation in the previous keyframe and the next.
        /// No blend factors or strength come into play here. That is up to the skeleton to utilize correctly.
        /// </summary>
        /// <param name="boneIndex">The index of the bone transform.</param>
        /// <returns>The interpolated rotation of the specified bone after the transformation.</returns>
        public float TransformBone(int boneIndex)
        {
            //Current bone.
            Bone bone = _Skeleton.BoneUpdateOrder[boneIndex];

            //If the bone is involved in any key changes in the not so distant future.
            if (_Keyframes.Exists(kf => (kf.ExistsBone(bone.Index))))
            {
                //Get the next keyframe that wants to modify the bone and the previous one who have already done that.
                Keyframe nextKeyframe = GetNextKeyframe(bone);
                Keyframe previousKeyframe = GetPreviousKeyframe(bone);

                //Get the updated bone from the next keyframe that will try and modify this bone, and the state of the bone at its last important keyframe.
                Bone boneToBe = nextKeyframe.GetBone(bone.Index);
                Bone boneThatWas = previousKeyframe.GetBone(bone.Index);

                //Calculate the interpolation amount.
                float interpolation = CalculateInterpolation(previousKeyframe.FrameNumber, nextKeyframe.FrameNumber);

                //Perform the linear interpolation between the last and next keyframe bone states and return the result.
                return MathHelper.Lerp(boneThatWas.Rotation, boneToBe.Rotation, interpolation);
            }

            //As the transformation has not occured, simply return the current and unmodified rotation of the bone.
            return bone.Rotation;
        }
        /// <summary>
        /// Transform the skeleton according to the state of its animation.
        /// </summary>
        [Obsolete("Use any of the other transform methods instead.")]
        public void TransformSkeleton()
        {
            //Loop through all bones in the skeleton.
            /*for (int boneIndex = 0; boneIndex < _Skeleton.BoneUpdateOrder.Count; boneIndex++)
            {
                //Current bone.
                Bone bone = _Skeleton.BoneUpdateOrder[boneIndex];

                //If the bone's a root bone, take that into account.
                if (bone.IsRootBone)
                {
                    //If the bone is involved in any key changes in the not so distant future.
                    if (_Keyframes.Exists(kf => (kf.ExistsBone(bone.Index))))
                    {
                        //Get the next keyframe that wants to modify the bone and the previous one who have already done that.
                        Keyframe nextKeyframe = GetNextKeyframe(bone);
                        Keyframe previousKeyframe = GetPreviousKeyframe(bone);

                        //Get the updated bone from the next keyframe that will try and modify this bone, and the state of the bone at its last important keyframe.
                        Bone boneToBe = nextKeyframe.GetBone(bone.Index);
                        Bone boneThatWas = previousKeyframe.GetBone(bone.Index);

                        //Calculate the interpolation amount.
                        float interpolation = CalculateInterpolation(previousKeyframe.FrameNumber, nextKeyframe.FrameNumber);

                        //Perform the linear interpolation between the last and next keyframe bone states.
                        bone.AbsoluteRotation = MathHelper.Lerp(boneThatWas.AbsoluteRotation, boneToBe.AbsoluteRotation, interpolation);
                    }
                }
                //Otherwise continue as normal.
                else
                {
                    //If the bone is involved in any key changes in the not so distant future.
                    if (_Keyframes.Exists(kf => (kf.ExistsBone(bone.Index))))
                    {
                        //Get the next keyframe that wants to modify the bone and the previous one who have already done that.
                        Keyframe nextKeyframe = GetNextKeyframe(bone);
                        Keyframe previousKeyframe = GetPreviousKeyframe(bone);

                        //Get the updated bone from the next keyframe that will try and modify this bone, and the state of the bone at its last important keyframe.
                        Bone boneToBe = nextKeyframe.GetBone(bone.Index);
                        Bone boneThatWas = previousKeyframe.GetBone(bone.Index);

                        //Calculate the interpolation amount.
                        float interpolation = CalculateInterpolation(previousKeyframe.FrameNumber, nextKeyframe.FrameNumber);

                        //Perform the linear interpolation between the last and next keyframe bone states.
                        bone.RelativeRotation = MathHelper.Lerp(boneThatWas.RelativeRotation, boneToBe.RelativeRotation, interpolation);
                    }

                    //The new absolute position and relative rotation.
                    bone.AbsolutePosition = Helper.CalculateOrbitPosition(_Skeleton.Bones[bone.ParentIndex].AbsolutePosition,
                        (bone.RelativeDirection + _Skeleton.Bones[bone.ParentIndex].AbsoluteRotation), Vector2.Distance(Vector2.Zero, bone.RelativePosition));

                    //Update the absolute rotation.
                    bone.UpdateAbsoluteRotation();
                    //Update the relative direction.
                    bone.UpdateRelativeDirection();
                }
            }*/
        }
        /// <summary>
        /// Transform the skeleton according to the state of its animation.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        [Obsolete("Use any of the other transform methods instead.")]
        public void TransformSkeleton(int nextKeyframeIndex, float time)
        {
            #region Matrix
            //Loop through all bones in the skeleton.
            /*for (int boneIndex = 0; boneIndex < _Skeleton.BoneUpdateOrder.Count; boneIndex++)
            {
                //Make the bone's matrix influential properties more exposed and thus easier to reach.
                Vector2 position = _Skeleton.BoneUpdateOrder[boneIndex].RelativePosition;
                Vector2 scale = _Skeleton.BoneUpdateOrder[boneIndex].Scale;
                float rotation = _Skeleton.BoneUpdateOrder[boneIndex].AbsoluteRotation;

                //If the bone's a root bone, use the absolute position and absolute rotation instead.
                if (_Skeleton.BoneUpdateOrder[boneIndex].RootBone)
                {
                    //Make the bone's matrix influential properties more exposed and thus easier to reach.
                    position = _Skeleton.BoneUpdateOrder[boneIndex].AbsolutePosition;
                    rotation = _Skeleton.BoneUpdateOrder[boneIndex].AbsoluteRotation;
                }

                //Create the variables that'll handle the matrix decomposing.
                Vector3 position3, scale3;
                Vector2 direction;
                Quaternion rotationQ;

                //If the bone is involved in any key changes in the not so distant future.
                if (_Keyframes[nextKeyframeIndex].BonesToBe.Exists(bone => (_Skeleton.BoneUpdateOrder[boneIndex].Index == bone.Index)))
                {
                    //Get the correct bone in the next keyframe's list of bones to update, that is the one that's currently being updated.
                    Bone boneToBe = _Keyframes[nextKeyframeIndex].BonesToBe[_Keyframes[nextKeyframeIndex].BonesToBe.FindIndex(b => (_Skeleton.BoneUpdateOrder[boneIndex].Index == b.Index))];

                    //Decompose the bone's last keyframe matrix transformation and extract the position, scale and rotation.
                    _Skeleton.BoneUpdateOrder[boneIndex].LastKeyframeTransform.Decompose(out scale3, out rotationQ, out position3);
                    //Get the direction from the quaternion.
                    direction = Vector2.Transform(Vector2.UnitX, rotationQ);

                    //Perform the linear interpolation between the last and next keyframe bone states.
                    position = Vector2.Lerp(new Vector2(position3.X, position3.Y), boneToBe.RelativePosition, time);
                    scale = Vector2.Lerp(new Vector2(scale3.X, scale3.Y), boneToBe.Scale, time);
                    rotation = MathHelper.Lerp((float)Math.Atan2(direction.Y, direction.X), boneToBe.RelativeRotation, time);
                }

                //Get the bone's parent's matrix transformation.
                Matrix parentTransform = _Skeleton.BoneUpdateOrder[boneIndex].ParentIndex == -1 ? Matrix.Identity : _Skeleton.Bones[_Skeleton.BoneUpdateOrder[boneIndex].ParentIndex].LastFrameTransform;

                //Transform the bone with both its own properties and its parent's in mind.
                _Skeleton.BoneUpdateOrder[boneIndex].LastFrameTransform =
                    Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position.X, position.Y, 0) * parentTransform;

                //Decompose the bone's matrix transformation and extract the new position, scale and rotation.
                _Skeleton.BoneUpdateOrder[boneIndex].LastFrameTransform.Decompose(out scale3, out rotationQ, out position3);
                //Get the direction from the quaternion.
                direction = Vector2.Transform(Vector2.UnitX, rotationQ);

                //Finally change the bone's position, scale and rotation so that the animation can go forward.
                _Skeleton.BoneUpdateOrder[boneIndex].AbsolutePosition = new Vector2(position3.X, position3.Y);
                _Skeleton.BoneUpdateOrder[boneIndex].Scale = new Vector2(scale3.X, scale3.Y);
                _Skeleton.BoneUpdateOrder[boneIndex].AbsoluteRotation = (float)Math.Atan2(direction.Y, direction.X);
            }*/
            #endregion

            #region Non-Matrix
            /*
             //Loop through all bones in the skeleton.
            for (int boneIndex = 0; boneIndex < _Skeleton.BoneUpdateOrder.Count; boneIndex++)
            {
                //Current bone.
                Bone bone = _Skeleton.BoneUpdateOrder[boneIndex];

                //Expose some of the bone's matrix properties.
                Vector2 position = bone.RelativePosition;
                Vector2 scale = bone.Scale;
                float rotation = bone.RelativeRotation;

                //Some additional variables that may come in handy.
                Vector2 parentPosition = Vector2.Zero;
                float parentRotation = 0;

                //If the bone's a root bone, use the absolute position and absolute rotation instead.
                if (bone.RootBone) { parentPosition = bone.AbsolutePosition; }
                else
                {
                    //Get the parent bone's position and rotation.
                    parentPosition = _Skeleton.Bones[bone.ParentIndex].AbsolutePosition;
                    parentRotation = _Skeleton.Bones[bone.ParentIndex].AbsoluteRotation;
                }

                //If the bone is involved in any key changes in the not so distant future.
                if (_Keyframes.Exists(kf => (kf.BonesToBe.Exists(b => (bone.Index == b.Index)))))
                {
                    //Get the next keyframe that wants to modify the bone and the previous one who have already done that.
                    Keyframe nextKeyframe = GetNextKeyframe(bone);
                    Keyframe previousKeyframe = GetPreviousKeyframe(bone);

                    //Get the updated bone from the next keyframe that will try and modify this bone, and the state of the bone at its last important keyframe.
                    Bone boneToBe = nextKeyframe.BonesToBe.Find(b => (bone.Index == b.Index));
                    Bone boneThatWas = previousKeyframe.BonesToBe.Find(b => (bone.Index == b.Index));

                    //Calculate the interpolation amount.
                    float interpolation = CalculateInterpolation(previousKeyframe.FrameNumber, nextKeyframe.FrameNumber);

                    //If the bone is a root bone, use the absolute rotation instead.
                    if (bone.RootBone)
                    {
                        //Perform the linear interpolation between the last and next keyframe bone states.
                        scale = Vector2.Lerp(boneThatWas.Scale, boneToBe.Scale, interpolation);
                        rotation = MathHelper.Lerp(boneThatWas.AbsoluteRotation, boneToBe.AbsoluteRotation, interpolation);
                    }
                    else
                    {
                        //Perform the linear interpolation between the last and next keyframe bone states.
                        scale = Vector2.Lerp(boneThatWas.Scale, boneToBe.Scale, interpolation);
                        rotation = MathHelper.Lerp(boneThatWas.RelativeRotation, boneToBe.RelativeRotation, interpolation);
                    }
                }

                //The distance to the parent bone. CHECK THE INTERPOLATION OF RELATIVE POSITIONS!!!
                float distance = Vector2.Distance(Vector2.Zero, position);
                //The new absolute position and relative rotation.
                bone.AbsolutePosition = CalculateOrbitPosition(parentPosition, ((bone.RelativeDirection + parentRotation) - ((float)Math.PI / 2)), distance);
                bone.RelativeRotation = rotation;
                //Update the relative direction.
                bone.UpdateRelativeDirection();
            }
            */
            #endregion
        }
        /// <summary>
        /// Reset the specified keyframe to factory settings, i.e the natural/current pose of the skeleton.
        /// </summary>
        /// <param name="frameNumber">The keyframe's frame number.</param>
        public void ResetKeyframe(int frameNumber)
        {
            GetKeyframe(frameNumber).SetBones(_Skeleton);
        }
        /// <summary>
        /// Add a keyframe to the animation.
        /// </summary>
        public void AddKeyframe()
        {
            //Add a keyframe.
            if (_Keyframes.Count != 0) { AddKeyframe(new Keyframe(_Keyframes[_Keyframes.Count - 1].FrameNumber)); }
            else { AddKeyframe(0); }
        }
        /// <summary>
        /// Add a new keyframe to the animation.
        /// </summary>
        /// <param name="frameNumber">The frame number of the keyframe to be added.</param>
        public void AddKeyframe(int frameNumber)
        {
            //Add a new keyframe to the animation.
            AddKeyframe(new Keyframe(frameNumber));
        }
        /// <summary>
        /// Add a new keyframe to the animation.
        /// </summary>
        /// <param name="keyframe">The keyframe that will be added.</param>
        public void AddKeyframe(Keyframe keyframe)
        {
            //If the keyframe will fit in the animation and if there is no other keyframe at the same position.
            if ((_NumberOfFrames > keyframe.FrameNumber) && (!_Keyframes.Exists(kf => (kf.FrameNumber == keyframe.FrameNumber))))
            {
                //Add a new keyframe to the animation.
                _Keyframes.Add(keyframe);
                //Fire the event.
                KeyframeAddedInvoke();
            }
        }
        /// <summary>
        /// Remove a keyframe from the animation.
        /// </summary>
        /// <param name="index">The index of the keyframe to be deleted.</param>
        public void RemoveKeyframe(int index)
        {
            //Delete a keyframe from the animation.
            _Keyframes.RemoveAt(index);

            //Fire the event.
            KeyframeRemovedInvoke();
        }
        /// <summary>
        /// Get a certain keyframe from the animation.
        /// </summary>
        /// <param name="frameNumber">The frame number of the keyframe.</param>
        /// <returns>The keyframe carrying the given frame number.</returns>
        public Keyframe GetKeyframe(int frameNumber)
        {
            //Find and return the specified keyframe.
            return (_Keyframes.Find(kf => (kf.FrameNumber == frameNumber)));
        }
        /// <summary>
        /// Is there a keyframe with a certain frame number, I wonder? Do not fret, this method will not deny you the answer you are craving.
        /// </summary>
        /// <param name="frameNumber">The frame number of the keyframe.</param>
        /// <returns>Whether there's a keyframe carrying the given frame number.</returns>
        public bool IsKeyframe(int frameNumber)
        {
            //Find and return the specified keyframe.
            return (_Keyframes.Exists(kf => (kf.FrameNumber == frameNumber)));
        }
        /// <summary>
        /// Update the index of which keyframe that's next in the animation.
        /// </summary>
        public void UpdateNextKeyframeIndex()
        {
            //Set the nextkeyframe index to be either the index of the next keyframe in the list or the last in the list, whichever is lowest.
            _NextFrameIndex = Math.Min(_CurrentFrameIndex + 1, _Keyframes.Count - 1);
        }
        /// <summary>
        /// Sort the animation's list of keyframes by looking at their frame numbers.
        /// </summary>
        public void SortKeyframes()
        {
            //Sort the keyframes by frame number in a descending order.
            _Keyframes.Sort((a, b) => a.FrameNumber.CompareTo(b.FrameNumber));
        }
        /// <summary>
        /// Get the time between two keyframes.
        /// </summary>
        /// <param name="start">The index of the keyframe to start with.</param>
        /// <param name="end">The index of the keyframe to end with.</param>
        public float GetTimeBetweenFrames(int start, int end)
        {
            //Calculate the time between two keyframes.
            if (start > end) { return (_FrameTime * ((_NumberOfFrames - (start + 1)) + (end + 1))); }
            else if (start < end) { return (_FrameTime * ((end + 1) - (start + 1))); }
            else { return (_FrameTime * _NumberOfFrames); }
        }
        /// <summary>
        /// Calculate the interpolation between two keyframes.
        /// </summary>
        /// <param name="previous">The index of the previous keyframe.</param>
        /// <param name="next">The index of the next keyframe.</param>
        public float CalculateInterpolation(int previous, int next)
        {
            //If the previous keyframe is the one we just passed, take that into account.
            if (previous == _CurrentFrameIndex) { return (_TotalElapsedTime / GetTimeBetweenFrames(previous, next)); }
            else { return ((GetTimeBetweenFrames(previous, _CurrentFrameIndex) + _TotalElapsedTime) / GetTimeBetweenFrames(previous, next)); }
        }
        /// <summary>
        /// Get the blend factor for a specified bone at the current phase of the animation.
        /// This includes both the bone's unique blending factor in the upcoming keyframe and the overall strength of the animation.
        /// </summary>
        /// <param name="bone">The bone in question.</param>
        /// <returns>The blend factor of this bone.</returns>
        public float GetBlendFactor(Bone bone)
        {
            //If the bone has found home in any keyframe, return the next keyframe's blend factor for it.
            if (_Keyframes.Exists(kf => (kf.ExistsBone(bone.Index)))) { return (GetNextKeyframe(bone).GetBlendFactor(bone) * _Strength); }

            //If not, return zero.
            return 0;
        }
        /// <summary>
        /// Find the next keyframe that will modify a certain bone.
        /// </summary>
        /// <param name="bone">The bone that the next keyframe will modify.</param>
        public Keyframe GetNextKeyframe(Bone bone)
        {
            //Return the keyframe.
            return GetNextKeyframe(bone.Index, _CurrentFrameIndex);
        }
        /// <summary>
        /// Find the next keyframe that will modify a certain bone.
        /// </summary>
        /// <param name="boneIndex">The index of the bone that the next keyframe will modify.</param>
        /// <param name="frameNumber">The current frame number.</param>
        public Keyframe GetNextKeyframe(int boneIndex, int frameNumber)
        {
            //The index and the number of frames between the current keyframe and the next.
            int index = -1;
            int space = _NumberOfFrames;

            //Find and return a keyframe that fulfills two conditions: being the closest keyframe in front of the current one and one that will modify the bone.
            foreach (Keyframe keyframe in _Keyframes)
            {
                //If the keyframe wants to modify the bone.
                if (keyframe.BonesToBe.Exists(b => (boneIndex == b.Index)))
                {
                    //If the index of the keyframe is greater than the current keyframe and the space between them is less than the previous.
                    if ((keyframe.FrameNumber > frameNumber) && (space > ((keyframe.FrameNumber + 1) - (frameNumber + 1))))
                    {
                        //Save the new index and space.
                        index = _Keyframes.IndexOf(keyframe);
                        space = ((keyframe.FrameNumber + 1) - (frameNumber + 1));
                    }
                    //If the index of the keyframe is less than the current keyframe and the space between them is less than the previous.
                    else if (space >= ((keyframe.FrameNumber + 1) + (_Keyframes[_Keyframes.Count - 1].FrameNumber - (frameNumber + 1))))
                    {
                        //Save the new index and space.
                        index = _Keyframes.IndexOf(keyframe);
                        space = ((keyframe.FrameNumber + 1) + (_Keyframes[_Keyframes.Count - 1].FrameNumber - (frameNumber + 1)));
                    }
                }
            }

            //Return the keyframe.
            return _Keyframes[index];
        }
        /// <summary>
        /// Find the previous keyframe that modified a certain bone.
        /// </summary>
        /// <param name="bone">The bone that the previous keyframe modified.</param>
        public Keyframe GetPreviousKeyframe(Bone bone)
        {
            //Return the keyframe.
            return GetPreviousKeyframe(bone.Index, _CurrentFrameIndex);
        }
        /// <summary>
        /// Find the previous keyframe that modified a certain bone.
        /// </summary>
        /// <param name="boneIndex">The index of the bone that the previous keyframe modified.</param>
        /// <param name="frameNumber">The starting frame number.</param>
        public Keyframe GetPreviousKeyframe(int boneIndex, int frameNumber)
        {
            //The index and the number of keyframes between the current keyframe and the previous one.
            int index = -1;
            int space = _NumberOfFrames;

            //Find and return a keyframe that fulfills two conditions: being the closest keyframe behind the current one and has modified the bone.
            foreach (Keyframe keyframe in _Keyframes)
            {
                //If the keyframe wants to modify the bone.
                if (keyframe.BonesToBe.Exists(b => (boneIndex == b.Index)))
                {
                    //If the index of the keyframe is less than the current keyframe and the space between them is less than the previous.
                    if ((keyframe.FrameNumber <= frameNumber) && (space > ((frameNumber + 1) - (keyframe.FrameNumber + 1))))
                    {
                        //Save the new index and space.
                        index = _Keyframes.IndexOf(keyframe);
                        space = ((frameNumber + 1) - (keyframe.FrameNumber + 1));
                    }
                    //If the index of the keyframe is greater than the current keyframe and the space between them is less than the previous.
                    else if ((keyframe.FrameNumber > frameNumber) &&
                        (space > ((frameNumber + 1) + (_Keyframes[_Keyframes.Count - 1].FrameNumber - (keyframe.FrameNumber + 1)))))
                    {
                        //Save the new index and space.
                        index = _Keyframes.IndexOf(keyframe);
                        space = ((frameNumber + 1) + (_Keyframes.Count - (keyframe.FrameNumber + 1)));
                    }
                }
            }

            //Return the keyframe.
            return _Keyframes[index];
        }
        /// <summary>
        /// Calculate the difference in angle between two vectors.
        /// </summary>
        /// <param name="parentPosition">The parent position.</param>
        /// <param name="parentRotation">The parent rotation.</param>
        /// <param name="childPosition">The child position.</param>
        /// <returns></returns>
        float CalculateAngleBetweenVectors(Vector2 parentPosition, float parentRotation, Vector2 childPosition)
        {
            //The distance between vectors.
            float distance = Vector2.Distance(parentPosition, childPosition);

            //If the values are acceptable, proceed.
            if (distance != 0)
            {
                //Return the difference in angles.
                float rotation = (parentRotation - Helper.CalculateAngleFromOrbitPosition(parentPosition, childPosition));
                return rotation;
            }
            //Otherwise return 0.
            else { return 0; }
        }
        /// <summary>
        /// Tell the world that a keyframe has been added.
        /// </summary>
        private void KeyframeAddedInvoke()
        {
            //Sort the keyframes and update the nextkeyframeindex counter.
            SortKeyframes();
            UpdateNextKeyframeIndex();

            //If someone has hooked up a delegate to the event, fire it.
            if (KeyframeAdded != null) { KeyframeAdded(this, new EventArgs()); }
        }
        /// <summary>
        /// Tell the world that a keyframe has been removed.
        /// </summary>
        private void KeyframeRemovedInvoke()
        {
            //Sort the keyframes and update the nextkeyframeindex counter.
            SortKeyframes();
            UpdateNextKeyframeIndex();

            //If someone has hooked up a delegate to the event, fire it.
            if (KeyframeRemoved != null) { KeyframeRemoved(this, new EventArgs()); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The skeleton that this animation belongs to.
        /// </summary>
        public Skeleton Skeleton
        {
            get { return _Skeleton; }
            set { _Skeleton = value; }
        }
        /// <summary>
        /// The keyframes of this animation.
        /// </summary>
        public List<Keyframe> Keyframes
        {
            get { return _Keyframes; }
            set { _Keyframes = value; }
        }
        /// <summary>
        /// The name of the animation.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// The time a frame has onscreen.
        /// </summary>
        public float FrameTime
        {
            get { return _FrameTime; }
            set { _FrameTime = value; }
        }
        /// <summary>
        /// The total number of frames in this animation.
        /// </summary>
        public int NumberOfFrames
        {
            get { return _NumberOfFrames; }
            set { _NumberOfFrames = value; }
        }
        /// <summary>
        /// The index of the current frame in the animation.
        /// </summary>
        public int CurrentFrameIndex
        {
            get { return _CurrentFrameIndex; }
            set { _CurrentFrameIndex = value; }
        }
        /// <summary>
        /// If the animation is currently active.
        /// </summary>
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }
        /// <summary>
        /// The total amount of time that has elapsed since the last frame update.
        /// </summary>
        public float TotalElapsedTime
        {
            get { return _TotalElapsedTime; }
            set { _TotalElapsedTime = value; }
        }
        /// <summary>
        /// The signal strength of the animation.
        /// A value of one translates to full strength while a value of zero means that the animation is not active.
        /// </summary>
        public float Strength
        {
            get { return _Strength; }
            set { _Strength = value; }
        }
        #endregion
    }
}
