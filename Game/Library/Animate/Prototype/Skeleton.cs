﻿using System;
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

using Library.Factories;
using Library.Imagery;

namespace Library.Animate
{
    /// <summary>
    /// The skeleton uses a number of linked bones and animations to animate a character.
    /// </summary>
    public class Skeleton
    {
        #region Fields
        private List<Bone> _Bones;
        private List<Bone> _BoneUpdateOrder;
        private List<Animation> _Animations;
        private SpriteManager _Sprites;

        private Vector2 _Position;
        private float _Rotation;

        private FarseerPhysics.DrawingSystem.LineBrush _BoneBrush;
        private FarseerPhysics.DrawingSystem.LineBrush _SelectedBoneBrush;
        private FarseerPhysics.DrawingSystem.LineBrush _JointBrush;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for a skeleton.
        /// </summary>
        public Skeleton(GraphicsDevice graphicsDevice)
        {
            Initialize(graphicsDevice);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the skeleton.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to be used.</param>
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            //Initialize variables.
            _Bones = new List<Bone>();
            _BoneUpdateOrder = _Bones;
            _Animations = new List<Animation>();
            _Sprites = new SpriteManager();
            _Position = HasRootBone() ? GetRootBone().StartPosition : Vector2.Zero;
            _Rotation = 0;

            //Initialize the bone brushes.
            _BoneBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Black);
            _SelectedBoneBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Green);
            _JointBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Red);
            try
            {
                _BoneBrush.Load(graphicsDevice);
                _SelectedBoneBrush.Load(graphicsDevice);
                _JointBrush.Load(graphicsDevice);
            }
            catch { }
        }
        /// <summary>
        /// Load the skeleton's content.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager contentManager)
        {
            //Load the Sprite.
            _Sprites.LoadContent(contentManager);
        }
        /// <summary>
        /// Update the skeleton.
        /// </summary>
        /// <param name="gameTime">The engine time to adhere to.</param>
        public void Update(GameTime gameTime)
        {
            //If there's an active animation to update.
            if (ActiveAnimationsExists())
            {
                //Update the animations.
                _Animations.ForEach(animation => animation.Update(gameTime));
                //Transform the skeleton.
                TransformSkeleton();
            }

            //Update the sprites.
            _Sprites.Update(gameTime);

            //Update the sprites attached to the skeleton.
            foreach (Sprite sprite in _Sprites.Sprites)
            {
                //Update the position and rotation.
                sprite.Position = _Bones[Int32.Parse(sprite.Tag)].TransformedPosition;
                sprite.Rotation = _Bones[Int32.Parse(sprite.Tag)].TransformedRotation;
            }
        }
        /// <summary>
        /// Draw the skeleton.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Draw all the entity's sprites.
            _Sprites.Draw(spriteBatch);
        }

        /// <summary>
        /// Add a bone to the skeleton.
        /// </summary>
        /// <param name="bone">The bone to add.</param>
        public void AddBone(Bone bone)
        {
            //Add a bone to the skeleton.
            _Bones.Add(bone);
            //Make sure the bone is aware of this skeleton.
            _Bones[_Bones.Count - 1].Skeleton = this;
            //Set the index of this bone if it has not been already set.
            if (_Bones[_Bones.Count - 1].Index == -1) { _Bones[_Bones.Count - 1].Index = (_Bones.Count - 1); }
        }
        /// <summary>
        /// Add a sprite to the skeleton.
        /// </summary>
        /// <param name="path">The path of the asset to load.</param>
        /// <param name="boneIndex">The index of the bone.</param>
        /// <param name="origin">The origin of the sprite relative to the bone.</param>
        public void AddSprite(string path, int boneIndex, Vector2 origin)
        {
            Factory.Instance.AddSprite(_Sprites, _Bones[boneIndex].Name, path, _Bones[boneIndex].StartPosition, 0, Vector2.One, 0, 0, 0, boneIndex.ToString(), origin);
        }
        /// <summary>
        /// Add a sprite to the skeleton.
        /// </summary>
        /// <param name="texture">The texture of the asset to use.</param>
        /// <param name="boneIndex">The index of the bone.</param>
        /// <param name="origin">The origin of the sprite relative to the bone.</param>
        public void AddSprite(Texture2D texture, int boneIndex, Vector2 origin)
        {
            Factory.Instance.AddSprite(_Sprites, _Bones[boneIndex].Name, texture, _Bones[boneIndex].StartPosition, 0, Vector2.One, 0, 0, 0, boneIndex.ToString(), origin);
        }
        /// <summary>
        /// Transform the skeleton according to the state of its animation.
        /// </summary>
        public void TransformSkeleton()
        {
            //Loop through all bones in the skeleton.
            for (int boneIndex = 0; boneIndex < _BoneUpdateOrder.Count; boneIndex++)
            {
                //Current bone.
                Bone bone = _BoneUpdateOrder[boneIndex];
                //A 'tabula rasa' just waiting to be filled with bone rotational data. The new rotation of the bone.
                float rotation = 0;
                //The sum of all blend factors.
                float blendSum = 0;

                //Sum up all animations' individual blend factors for this bone.
                foreach (Animation animation in _Animations) { if (animation.IsActive) { blendSum += animation.GetBlendFactor(bone); } }

                //Go through each active animation.
                foreach (Animation animation in _Animations)
                {
                    //If the animation is active.
                    if (animation.IsActive)
                    {
                        //Calculate the normalised blend factor and make sure it isn't NaN or 0.
                        float blend = animation.GetBlendFactor(bone) / blendSum;
                        blend = float.IsNaN(blend) || blend == 0 ? 1 : blend;

                        //Transform the bone according to the normalised blend factor of the animation.
                        rotation += animation.TransformBone(boneIndex) * blend;
                    }
                }

                if (bone.Index == 1)
                {
                    Vector2 end = Helper.CalculateOrbitPosition(bone.StartPosition, rotation, bone.Length);
                    float rot = Helper.CalculateAngleFromOrbitPosition(bone.StartPosition, end);
                }

                //Calculate the new end position of the bone.
                bone.Rotation = rotation;

                //Update the bone according to the parent's transformation matrix.
                bone.Update(bone.IsRootBone ? Matrix.Identity : _Bones[bone.ParentIndex].Transform);
            }
        }
        /// <summary>
        /// Draw the skeleton's bones and joints as a debugging feature.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to be used.</param>
        /// <param name="selected">The selected bone.</param>
        public void DebugDraw(SpriteBatch spriteBatch, float selected)
        {
            foreach (Bone b in _Bones)
            {
                //Draw the bone with green.
                if (b.Index == selected)
                {
                    //Draw with green.
                    _SelectedBoneBrush.Draw(spriteBatch, b.TransformedPosition, Helper.CalculateOrbitPosition(b.TransformedPosition, b.TransformedRotation, b.Length));
                }
                //Draw the bone with black.
                else { _BoneBrush.Draw(spriteBatch, b.TransformedPosition, Helper.CalculateOrbitPosition(b.TransformedPosition, b.TransformedRotation, b.Length)); }

                //Draw the joint.
                _JointBrush.Draw(spriteBatch, b.TransformedPosition, Helper.CalculateOrbitPosition(b.TransformedPosition, 0, 1));
            }
        }
        /// <summary>
        /// Add an animation to the skeleton.
        /// </summary>
        public void AddAnimation()
        {
            _Animations.Add(new Animation(this));
        }
        /// <summary>
        /// Remove an animation from the skeleton.
        /// </summary>
        /// <param name="animation">The animation to remove.</param>
        public void RemoveAnimation(Animation animation)
        {
            //Remove the animation.
            _Animations.Remove(animation);
        }
        /// <summary>
        /// Add an animation to the skeleton.
        /// </summary>
        /// <param name="animation">The animation to add.</param>
        public void AddAnimation(Animation animation)
        {
            _Animations.Add(animation);
        }
        /// <summary>
        /// Add a keyframe to a certain animation of the skeleton.
        /// </summary>
        /// <param name="index">The index of the animation to add the keyframe to.</param>
        /// <param name="frameNumber">The number this keyframe has in the animation.</param>
        public void AddKeyframe(int index, int frameNumber)
        {
            _Animations[index].AddKeyframe(frameNumber);
        }
        /// <summary>
        /// Add a bone to a keyframe, thus enabling that bone to be animated.
        /// </summary>
        /// <param name="animationIndex">The index of the particular animation.</param>
        /// <param name="keyframeIndex">The index of the particular keyframe.</param>
        /// <param name="bone">A copy of the bone to animate, only modified to portray the final composition of that bone. This is what the bone will animate into.</param>
        public void AddBoneToBe(int animationIndex, int keyframeIndex, Bone bone)
        {
            _Animations[animationIndex].Keyframes[keyframeIndex].AddBone(bone);
        }
        /// <summary>
        /// Sort the list of bones so that they are stored in a hierarchical fashion, ie. by parent index.
        /// That is the bones with the lowest parent index will show up at the top of the list.
        /// </summary>
        public void SortBones()
        {
            _BoneUpdateOrder.Sort((a, b) => a.ParentIndex.CompareTo(b.ParentIndex));
        }
        /// <summary>
        /// See if an active animation exists.
        /// </summary>
        /// <returns>Whether an active animation exists.</returns>
        public bool ActiveAnimationsExists()
        {
            //Go through each animation and if any of them is active, return true.
            foreach (Animation animation in _Animations) { if (animation.IsActive) { return true; } }

            //No animation was active, return false;
            return false;
        }
        /// <summary>
        /// Find a bone's index in the skeleton's list.
        /// </summary>
        /// <param name="bone">The bone to find.</param>
        /// <returns>The index of the bone.</returns>
        public int FindBone(Bone bone)
        {
            return (_Bones.FindIndex(b => (b.Equals(bone))));
        }
        /// <summary>
        /// Calculate the absolute end position of a bone.
        /// </summary>
        /// <param name="index">The index of the bone in question.</param>
        /// <returns>The absolute end position.</returns>
        public Vector2 CalculateAbsoluteEndPosition(int index)
        {
            //A placeholder for the absolute position.
            Vector2 position = Vector2.Zero;

            //Stack the position and move on to the next parent, until we reach the root bone.
            while (index != -1)
            {
                position += _Bones[index].EndPosition;
                index = _Bones[index].ParentIndex;
            }

            //Return the absolute position.
            return position;
        }
        /*public float CalculateLength(int index, Vector2 childPosition)
        {
            //Calculate the position of the bone by looking at the position, rotation and length of its parent.
            return (Vector2.Distance(Bones[index].RelativePosition, childPosition));
        }*/
        /// <summary>
        /// Whether a root bone exists in the skeleton.
        /// </summary>
        /// <returns>Whether a root bone exists.</returns>
        public bool HasRootBone()
        {
            return (_Bones.Exists(bone => bone.IsRootBone));
        }
        /// <summary>
        /// Get the root bone of this skeleton. If there is none, the method returns null.
        /// </summary>
        /// <returns>The root bone for the skeleton.</returns>
        public Bone GetRootBone()
        {
            return _Bones.Find(bone => bone.IsRootBone);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name of the skeleton.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The position of the skeleton, ie. the root bone.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        /// <summary>
        /// The rotation of the skeleton, ie. the root bone.
        /// </summary>
        public float Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }
        public Vector2 Scale { get; set; }
        public List<Bone> Bones
        {
            get { return _Bones; }
            set { _Bones = value; }
        }
        public List<Bone> BoneUpdateOrder
        {
            get { return _BoneUpdateOrder; }
            set { _BoneUpdateOrder = value; }
        }
        public List<Animation> Animations
        {
            get { return _Animations; }
            set { _Animations = value; }
        }
        /// <summary>
        /// The sprite collection of the skeleton.
        /// </summary>
        public SpriteManager Sprites
        {
            get { return (_Sprites); }
            set { _Sprites = value; }
        }
        #endregion
    }
}
