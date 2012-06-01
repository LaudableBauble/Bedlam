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

using FarseerPhysics.DrawingSystem;

using Library;
using Library.Animate;
using Library.Core;
using Library.Enums;
using Library.GUI;
using Library.GUI.Basic;
using Library.Factories;
using Library.Imagery;
using Library.Infrastructure;

namespace Library.Core
{
    /// <summary>
    /// A texture item displays an image somewhere, confined within the boundaries of levels and layers.
    /// </summary>
    public class TextureItem : Item
    {
        #region Fields
        private SpriteManager _Sprites;
        #endregion

        #region Constructors
        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="level">The level that this item belongs to.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="scale">The scale of the item.</param>
        public TextureItem(Level level, string name, Vector2 position, float rotation, Vector2 scale)
        {
            //Initialize the editor.
            Initialize(level, name, position, rotation, scale, 0, 0);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the item.
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

            //Initialize some variables.
            _Sprites = new SpriteManager();
            _Type = Enums.ItemType.TextureItem;
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="contentManager">The manager that handles all graphical content.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            //Call the base method.
            base.LoadContent(contentManager);

            //Load the sprite.
            _Sprites.LoadContent(contentManager);
        }
        /// <summary>
        /// Update the item.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            //Call the base method.
            base.Update(gameTime);

            //Update the sprite. TODO: Do this in a better way.
            _Sprites.Update(gameTime, Position, Rotation);
            foreach (Sprite sprite in _Sprites.Sprites) { sprite.Scale = (Scale.X + Scale.Y) / 2; }
        }
        /// <summary>
        /// Draw the item.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Call the base method.
            base.Draw(spriteBatch);

            //Draw the sprite.
            _Sprites.Draw(spriteBatch);
        }

        /// <summary>
        /// Add a sprite to the item.
        /// </summary>
        /// <param name="sprite">The asset to add.</param>
        public void AddSprite(Sprite sprite)
        {
            //Subscribe to some events.
            sprite.BoundsChanged += OnSpriteBoundsChange;
        }
        /// <summary>
        /// Add a sprite to the item.
        /// </summary>
        /// <param name="path">The path of the asset to load.</param>
        public void AddSprite(string path)
        {
            //Add a sprite.
            AddSprite(Factory.Instance.AddSprite(Sprites, "Sprite" + _Sprites.Count, path, Position, 0, 1, 0, 0, 0, "Sprite" + _Sprites.Count));
        }
        /// <summary>
        /// Add a sprite to the item.
        /// </summary>
        /// <param name="texture">The texture of the asset to use.</param>
        public void AddSprite(Texture2D texture)
        {
            //Add a sprite.
            AddSprite(Factory.Instance.AddSprite(Sprites, texture.Name, texture, Position, 0, 1, 0, 0, 0, "Sprite" + _Sprites.Count));
        }
        /// <summary>
        /// Change the visibility state of this item.
        /// </summary>
        /// <param name="isVisible">Whether the item will be visible or not.</param>
        public override void ChangeVisibilityState(bool isVisible)
        {
            //Call the base method.
            base.ChangeVisibilityState(isVisible);

            //If the sprite manager has no sprites, stop here.
            if (_Sprites.Count == 0) { return; }

            //Change the state of visibility for the sprite collection.
            if (isVisible) { _Sprites[0].Visibility = Visibility.Visible; }
            else { _Sprites[0].Visibility = Visibility.Invisible; }
        }
        /// <summary>
        /// See if a vector position collides with this item. Because of this is a texture item we will do per-pixel collision.
        /// </summary>
        /// <param name="point">The point of the would-be collision.</param>
        /// <returns>Whether the point collides or not.</returns>
        public override bool IsPixelsIntersecting(Vector2 point)
        {
            //If the point and this item intersects, return true.
            if (Helper.IsPointWithinImage(point, _Sprites.FirstSprite())) { return true; }

            //Return false;
            return false;
        }

        /// <summary>
        /// The bounds of the main sprite has changed, update the item's bounds to reflect this.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        public void OnSpriteBoundsChange(object obj, BoundsChangedEventArgs e)
        {
            //Change the item's bounds.
            Width = e.Width;
            Height = e.Height;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The sprites of the item.
        /// </summary>
        public SpriteManager Sprites
        {
            get { return _Sprites; }
            set { _Sprites = value; }
        }
        #endregion
    }
}