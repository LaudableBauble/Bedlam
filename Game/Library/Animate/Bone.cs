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
    /// A bone can be linked together with other bones to become a skeleton and choreographed to perform various animations.
    /// </summary>
    public class Bone
    {
        #region Fields
        private Skeleton _Skeleton;
        private string _Name;
        private int _Index;
        private int _ParentIndex;
        private bool _RootBone;

        private Vector2 _AbsolutePosition;
        private float _AbsoluteRotation;
        private Vector2 _RelativePosition;
        private float _RelativeRotation;
        private float _RelativeDirection;
        private Vector2 _Scale;
        private float _Length;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a bone.
        /// </summary>
        public Bone()
        {
            Initialize(null, "", -1, -1, Vector2.Zero, Vector2.One, 0, 1);
        }
        /// <summary>
        /// Create a bone.
        /// </summary>
        /// <param name="skeleton">The skeleton this bone is a part of.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The absolute position of the bone.</param>
        /// <param name="scale">The scale of the bone.</param>
        /// <param name="rotation">The absolute rotation of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        public Bone(Skeleton skeleton, string name, int index, int parentIndex, Vector2 position, Vector2 scale, float rotation, float length)
        {
            Initialize(skeleton, name, index, parentIndex, position, scale, rotation, length);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Intialize the bone.
        /// </summary>
        /// <param name="skeleton">The skeleton this bone is a part of.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The absolute position of the bone.</param>
        /// <param name="scale">The scale of the bone.</param>
        /// <param name="rotation">The absolute rotation of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        private void Initialize(Skeleton skeleton, string name, int index, int parentIndex, Vector2 position, Vector2 scale, float rotation, float length)
        {
            //Save the given index for future use, along with initializing a few things.
            _Skeleton = skeleton;
            _Name = name;
            _Index = index;
            _ParentIndex = parentIndex;
            _RootBone = false;
            _Scale = scale;
            _AbsolutePosition = position;
            _AbsoluteRotation = rotation;
            _RelativePosition = Vector2.Zero;
            _RelativeRotation = rotation;
            _Length = length;
            _RelativeDirection = 0;

            //Check if the bone is the skeleton's root bone.
            if (parentIndex == -1) { _RootBone = true; }
            else
            {
                //Calculate the position relative to the bone's parent.
                UpdateRelativePosition();
                UpdateRelativeRotation();
                UpdateRelativeDirection();
            }
        }
        /// <summary>
        /// Update the bone.
        /// </summary>
        public void Update()
        {
            //Try to keep the rotation within reasonable limits. Commentary: Scrap that last part, will ya'? Let the rotation run wild.
            //_AbsoluteRotation = Helper.WrapAngle(_AbsoluteRotation);
            UpdateRelativeRotation();
        }

        /// <summary>
        /// Update the absolute position to accommodate for a change in its relative position.
        /// </summary>
        public void UpdateAbsolutePosition()
        {
            //Update the absolute position to accommodate for a change in its relative position.
            if (!_RootBone) { _AbsolutePosition = _RelativePosition + _Skeleton.Bones[_ParentIndex].AbsolutePosition; }
        }
        /// <summary>
        /// Update the relative position to accommodate for a change in its absolute position.
        /// </summary>
        public void UpdateRelativePosition()
        {
            //Update the relative position to accommodate for a change in its absolute position.
            if (!_RootBone) { _RelativePosition = _AbsolutePosition - _Skeleton.Bones[_ParentIndex].AbsolutePosition; }
        }
        /// <summary>
        /// Update the absolute rotation to accommodate for a change in its relative rotation.
        /// </summary>
        public void UpdateAbsoluteRotation()
        {
            //Update the absolute rotation to accommodate for a change in its relative rotation.
            if (!_RootBone) { _AbsoluteRotation = _Skeleton.Bones[_ParentIndex].AbsoluteRotation + _RelativeRotation; }
        }
        /// <summary>
        /// Update the relative rotation to accommodate for a change in its absolute rotation.
        /// </summary>
        public void UpdateRelativeRotation()
        {
            //Update the relative rotation to accommodate for a change in its absolute rotation.
            if (!_RootBone) { UpdateRelativeRotation(_Skeleton.Bones[_ParentIndex].AbsoluteRotation); }
        }
        /// <summary>
        /// Update the relative rotation to accommodate for a change in its absolute rotation.
        /// </summary>
        /// <param name="parentRotation">The absolute rotation of the parent.</param>
        public void UpdateRelativeRotation(float parentRotation)
        {
            //Update the relative rotation to accommodate for a change in its absolute rotation.
            if (!_RootBone) { _RelativeRotation = _AbsoluteRotation + ((-1) * parentRotation); }
        }
        /// <summary>
        /// Update the relative direction, according to the rotation and movement of the parent.
        /// </summary>
        public void UpdateRelativeDirection()
        {
            //Check if the bone has a parent.
            if (!_RootBone)
            {
                //Update the relative direction, according to the rotation and movement of the parent.
                _RelativeDirection = /*Helper.WrapAngle(*/Helper.DifferenceInDirection(_Skeleton.Bones[_ParentIndex].AbsolutePosition,
                    _Skeleton.Bones[_ParentIndex].AbsoluteRotation, _AbsolutePosition);
            }
        }
        /// <summary>
        /// Deep clone this bone.
        /// </summary>
        public Bone DeepClone()
        {
            //Create a blank bone.
            Bone bone = new Bone();

            //Add all the data to it.
            bone.Skeleton = _Skeleton;
            bone.Name = _Name;
            bone.Index = _Index;
            bone.ParentIndex = _ParentIndex;
            bone.RootBone = _RootBone;
            bone.AbsolutePosition = _AbsolutePosition;
            bone.AbsoluteRotation = _AbsoluteRotation;
            bone.RelativeDirection = _RelativeDirection;
            bone.Scale = _Scale;
            bone.Length = _Length;

            //Return the deep cloned bone.
            return bone;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The skeleton this bone is a part of.
        /// </summary>
        public Skeleton Skeleton
        {
            get { return _Skeleton; }
            set { _Skeleton = value; }
        }
        /// <summary>
        /// The name of the bone.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// The index of this bone's parent.
        /// </summary>
        public int ParentIndex
        {
            get { return _ParentIndex; }
            set { _ParentIndex = value; }
        }
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        public bool RootBone
        {
            get { return _RootBone; }
            set { _RootBone = value; }
        }
        /// <summary>
        /// The absolute position of the bone.
        /// </summary>
        public Vector2 AbsolutePosition
        {
            get { return _AbsolutePosition; }
            set { _AbsolutePosition = value; UpdateRelativePosition(); }
        }
        /// <summary>
        /// The absolute rotation of the bone.
        /// </summary>
        public float AbsoluteRotation
        {
            get { return _AbsoluteRotation; }
            set { _AbsoluteRotation = value; UpdateRelativeRotation(); }
        }
        /// <summary>
        /// The position of the bone, relative to its parent.
        /// </summary>
        public Vector2 RelativePosition
        {
            get { return _RelativePosition; }
            set { _RelativePosition = value; UpdateAbsolutePosition(); }
        }
        /// <summary>
        /// The rotation of the bone, relative to its parent.
        /// </summary>
        public float RelativeRotation
        {
            get { return _RelativeRotation; }
            set { _RelativeRotation = value; UpdateAbsoluteRotation(); }
        }
        /// <summary>
        /// The direction of the bone, relative to its parent.
        /// </summary>
        public float RelativeDirection
        {
            get { return _RelativeDirection; }
            set { _RelativeDirection = value; }
        }
        public Vector2 Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }
        public float Length
        {
            get { return _Length; }
            set { _Length = value; }
        }
        #endregion
    }
}
