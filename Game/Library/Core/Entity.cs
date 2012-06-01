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

using Library.Enums;
using Library.Factories;
using Library.Imagery;

namespace Library.Core
{
    /// <summary>
    /// The entity serves as a base for all who strive to make a physical appearance in the game.
    /// </summary>
    public abstract class Entity : Item
    {
        #region Fields
        private List<Part> _Parts;
        private SpriteManager _Sprites;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the entity.
        /// </summary>
        /// <param name="level">The level that this item belongs to.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="scale">The scale of the item.</param>
        /// <param name="width">The width of the item.</param>
        /// <param name="height">The height of the item.</param>
        public override void Initialize(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            //Call the base method.
            base.Initialize(level, name, position, rotation, scale, width, height);

            //Initialize a few variables.
            _Parts = new List<Part>();
            _Sprites = new SpriteManager();
            _Type = Enums.ItemType.Entity;
        }
        /// <summary>
        /// Load the entity's content.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            //Call the base method.
            base.LoadContent(contentManager);

            //Load the Sprite.
            _Sprites.LoadContent(contentManager);
        }
        /// <summary>
        /// Update the entity.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        public override void Update(GameTime gameTime)
        {
            //Call the base method.
            base.Update(gameTime);

            //Update the sprite collection.
            _Sprites.Update(gameTime);
            //Loop through all parts and update them.
            foreach (Part part in _Parts) { part.Update(gameTime); }
        }
        /// <summary>
        /// Draw the entity.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Call the base method.
            base.Draw(spriteBatch);

            //Draw all the entity's sprites.
            _Sprites.Draw(spriteBatch);
        }

        /// <summary>
        /// Add a part to the entity.
        /// </summary>
        /// <param name="part">The part to add.</param>
        /// <returns>The part just added.</returns>
        public Part AddPart(Part part)
        {
            //Add the part to the list.
            _Parts.Add(part);

            //Return the part.
            return part;
        }
        /// <summary>
        /// Change the visibility state of this item.
        /// </summary>
        /// <param name="isVisible">Whether the item will be visible or not.</param>
        public override void ChangeVisibilityState(bool isVisible)
        {
            //Call the base method.
            base.ChangeVisibilityState(isVisible);

            //Change the visibility state of the sprite collection.
            if (isVisible) { _Sprites.Visibility = Visibility.Visible; }
            else { _Sprites.Visibility = Visibility.Invisible; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of parts this entity has.
        /// </summary>
        public List<Part> Parts
        {
            get { return (_Parts); }
            set { _Parts = value; }
        }
        /// <summary>
        /// The sprites of the entity.
        /// </summary>
        public SpriteManager Sprites
        {
            get { return _Sprites; }
            set { _Sprites = value; }
        }
        #endregion
    }
}