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

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Library.Animate;
using Library.Imagery;

namespace Library.Core
{
    /// <summary>
    /// A character extends the functionality of the entity and allows it to be animated with a skeleton.
    /// </summary>
    public class Character : Entity
    {
        #region Fields
        private Skeleton _Skeleton;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a character.
        /// </summary>
        /// <param name="level">The level that this item belongs to.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="scale">The scale of the item.</param>
        /// <param name="width">The width of the item.</param>
        /// <param name="height">The height of the item.</param>
        public Character(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            Initialize(level, name, position, rotation, scale, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the character.
        /// </summary>
        /// <param name="level">The level that this item belongs to.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="scale">The scale of the item.</param>
        /// <param name="width">The width of the item.</param>
        /// <param name="height">The height of the item.</param>
        protected override void Initialize(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            //Call the base method.
            base.Initialize(level, name, position, rotation, scale, width, height);

            //Initialize a few variables.
            _Skeleton = new Skeleton(level.GraphicsDevice);
            _Type = Enums.ItemType.Character;
        }
        /// <summary>
        /// Load the character's content.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            //Call the base method.
            base.LoadContent(contentManager);

            //Load the skeleton's content.
            _Skeleton.LoadContent(contentManager);
        }
        /// <summary>
        /// Update the character.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        public override void Update(GameTime gameTime)
        {
            //Call the base method.
            base.Update(gameTime);

            //Update the skeleton.
            Position = (Parts.Count != 0) ? ConvertUnits.ToDisplayUnits(Parts[0].Body.Position) : Position;
            Rotation = (Parts.Count != 0) ? Parts[0].Body.Rotation : Rotation;
            _Skeleton.Position = Position;
            _Skeleton.Rotation = Rotation;
            _Skeleton.Bones[0].RelativeRotation = Rotation;
            _Skeleton.Update(gameTime);

            //Update the sprites attached to the skeleton.
            foreach (Sprite sprite in Sprites.Sprites)
            {
                //Update the position and rotation.
                //sprite.Position = _Skeleton.Bones[Int32.Parse(sprite.Tag)].AbsolutePosition;
                //sprite.Rotation = (_Skeleton.Bones[Int32.Parse(sprite.Tag)].AbsoluteRotation - (float)Math.PI);
            }
        }
        /// <summary>
        /// Draw the character.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Call the base method.
            base.Draw(spriteBatch);

            //Draw the skeleton.
            _Skeleton.Draw(spriteBatch);
        }

        /// <summary>
        /// Add an animation to the character.
        /// </summary>
        public void AddAnimation()
        {
            _Skeleton.AddAnimation();
        }
        /// <summary>
        /// Add an animation to the character.
        /// </summary>
        public void AddAnimation(Animation animation)
        {
            _Skeleton.AddAnimation(animation);
        }
        /// <summary>
        /// Add a sprite to the character.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <param name="boneIndex">The index of the bone.</param>
        /// <param name="origin">The origin of the sprite relative to the bone.</param>
        public virtual void AddSprite(string name, int boneIndex, Vector2 origin)
        {
            _Skeleton.AddSprite(name, boneIndex, origin);
        }
        /// <summary>
        /// Add a sprite to the character.
        /// </summary>
        /// <param name="texture">The texture of the asset to use.</param>
        /// <param name="boneIndex">The index of the bone.</param>
        /// <param name="origin">The origin of the sprite relative to the bone.</param>
        public virtual void AddSprites(Texture2D texture, int boneIndex, Vector2 origin)
        {
            _Skeleton.AddSprite(texture, boneIndex, origin);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The skeleton that this character uses.
        /// </summary>
        public Skeleton Skeleton
        {
            get { return _Skeleton; }
            set { _Skeleton = value; }
        }
        #endregion
    }
}