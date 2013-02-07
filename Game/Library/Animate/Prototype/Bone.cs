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
        private Vector2 _StartPosition;
        private Vector2 _EndPosition;
        private Matrix _Transform;
        private Vector2 _TransformedPosition;
        private float _TransformedRotation;
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
            _Transform = Matrix.Identity;
            _TransformedPosition = Vector2.Zero;
            _TransformedRotation = 0;
        }
        /// <summary>
        /// Update the bone's transformation matrix, as well as its transformed position and rotation.
        /// Will not actually change the position or rotation of the bone, just its equivalent in the 'real' world.
        /// </summary>
        /// <param name="parent">The parent's transformation matrix. It needs to be stacked, ie. it needs to contain parent * grandparent etc. all the way to the root.</param>
        public void Update(Matrix parent)
        {
            //Create the variables that'll handle the matrix decomposing.
            Vector3 position;
            Vector3 scale;
            Vector2 direction;
            Quaternion rotation;

            //Get the local transformation matrix and multiply it with the parent transformation.
            _Transform = Matrix.CreateScale(1, 1, 1) * Matrix.CreateRotationZ(Rotation) * Matrix.CreateTranslation(StartPosition.X, StartPosition.Y, 0) * parent;
            //_Transform = Matrix.CreateScale(1, 1, 1) * Matrix.CreateRotationZ(0) * Matrix.CreateTranslation(EndPosition.X, EndPosition.Y, 0) * parent;

            //Decompose the transformation matrix and extract the new position, scale and rotation.
            _Transform.Decompose(out scale, out rotation, out position);
            //Get the direction from the quaternion.
            direction = Vector2.Transform(Vector2.UnitX, rotation);

            if (new Vector2(position.X, position.Y) != _TransformedPosition)
            { }

            //Update the bone's position, scale and rotation.
            _TransformedPosition = new Vector2(position.X - _EndPosition.X + _StartPosition.X, position.Y - _EndPosition.Y + _StartPosition.Y);
            _TransformedRotation = (float)Math.Atan2(direction.Y, direction.X);
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
        /// The starting position of the bone, ie. the origin, relative to the starting position of the parent. NOTE: Decide what the positions are relative to, this is KEY!
        /// </summary>
        public Vector2 StartPosition
        {
            get { return _StartPosition; }
            set { _StartPosition = value; }
        }
        /// <summary>
        /// The ending position of the bone, relative to the starting position of the parent.
        /// </summary>
        public Vector2 EndPosition
        {
            get { return _EndPosition; }
            set { _EndPosition = value; }
        }
        /// <summary>
        /// The rotation of the bone. This updates the end position.
        /// </summary>
        public float Rotation
        {
            get { return Helper.CalculateAngleFromOrbitPosition(_StartPosition, _EndPosition); }
            set { _EndPosition = Helper.CalculateOrbitPosition(_StartPosition, value, Length); }
        }
        /// <summary>
        /// The transformed position of the bone, ie. its absolute position according to the applied transformation matrix.
        /// </summary>
        public Vector2 TransformedPosition
        {
            get { return _TransformedPosition; }
        }
        /// <summary>
        /// The transformed rotation of the bone, ie. the absolute rotation according to the applied transformation matrix.
        /// </summary>
        public float TransformedRotation
        {
            get { return _TransformedRotation; }
        }
        /// <summary>
        /// The length of the bone.
        /// </summary>
        public float Length
        {
            get { return (float)Math.Abs((_StartPosition - _EndPosition).Length()); }
            set { }
        }
        /// <summary>
        /// The bone's current transformation matrix. The matrix is updated every Update().
        /// </summary>
        public Matrix Transform
        {
            get { return _Transform; }
        }
        #endregion
    }
}
