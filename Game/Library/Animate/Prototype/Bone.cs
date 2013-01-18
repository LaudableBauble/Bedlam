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
    /// A bone can be linked together with other bones to become a skeleton and choreographed to perform various animations.
    /// </summary>
    public class Bone
    {
        #region Fields
        private Skeleton _Skeleton;
        private string _Name;
        private int _Index;
        private int _ParentIndex;
        private Vector2 _StartPosition;
        private Vector2 _EndPosition;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a bone.
        /// </summary>
        public Bone()
        {
            Initialize(null, "bone", -1, -1, Vector2.Zero, Vector2.One);
        }
        /// <summary>
        /// Create a bone.
        /// </summary>
        /// <param name="skeleton">The skeleton this bone is a part of.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="startPosition">The starting position of the bone.</param>
        /// <param name="endPosition">The ending position of the bone.</param>
        public Bone(Skeleton skeleton, string name, int index, int parentIndex, Vector2 startPosition, Vector2 endPosition)
        {
            Initialize(skeleton, name, index, parentIndex, startPosition, endPosition);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the bone.
        /// </summary>
        /// <param name="skeleton">The skeleton this bone is a part of.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="startPosition">The starting position of the bone.</param>
        /// <param name="endPosition">The ending position of the bone.</param>
        private void Initialize(Skeleton skeleton, string name, int index, int parentIndex, Vector2 startPosition, Vector2 endPosition)
        {
            //Save the given index for future use, along with initializing a few things.
            _Skeleton = skeleton;
            _Name = name;
            _Index = index;
            _ParentIndex = parentIndex;
            _StartPosition = startPosition;
            _EndPosition = endPosition;
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
            bone.StartPosition = _StartPosition;
            bone.EndPosition = _EndPosition;

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
        /// The index of the bone.
        /// </summary>
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        /// <summary>
        /// The index of this bone's parent.
        /// </summary>
        public int ParentIndex
        {
            get { return _ParentIndex; }
            set { _ParentIndex = value; }
        }
        /// <summary>
        /// Whether the bone is a root bone.
        /// </summary>
        public bool IsRootBone
        {
            get { return _ParentIndex == -1; }
        }
        /// <summary>
        /// The starting position of the bone, ie. the origin.
        /// </summary>
        public Vector2 StartPosition
        {
            get { return _StartPosition; }
            set { _StartPosition = value; }
        }
        /// <summary>
        /// The ending position of the bone.
        /// </summary>
        public Vector2 EndPosition
        {
            get { return _EndPosition; }
            set { _EndPosition = value; }
        }
        /// <summary>
        /// The rotation of the bone.
        /// </summary>
        public float Rotation
        {
            get { return Helper.CalculateAngleFromOrbitPositionBone(_StartPosition, _EndPosition); }
        }
        public Matrix Transform
        {
            get { return Matrix.}
        }
        #endregion
    }
}
