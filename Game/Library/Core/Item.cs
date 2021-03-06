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

using FarseerPhysics.DrawingSystem;

using Library;
using Library.Animate;
using Library.Core;
using Library.Enums;
using Library.GUI;
using Library.GUI.Basic;
using Library.Imagery;
using Library.Infrastructure;

namespace Library.Core
{
    /// <summary>
    /// An item is the most basic instance of the game world and populates layers and levels in infinite waves of sugar-coated delirium.
    /// </summary>
    public abstract class Item
    {
        #region Fields
        protected Level _Level;
        protected string _Name;
        protected bool _IsVisible;
        protected Vector2 _Position;
        protected float _Rotation;
        protected Vector2 _Scale;
        protected float _Width;
        protected float _Height;
        protected Vector2 _Origin;
        protected ItemType _Type;
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
        protected virtual void Initialize(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            //Initialize some variables.
            _Level = level;
            _Name = name;
            _Position = position;
            _Rotation = rotation;
            _Scale = scale;
            _Width = width;
            _Height = height;
            _IsVisible = true;
            _Origin = new Vector2(width / 2, height / 2);
            _Type = ItemType.Item;
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="contentManager">The manager that handles all graphical content.</param>
        public abstract void LoadContent(ContentManager contentManager);
        /// <summary>
        /// Update the item.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public abstract void Update(GameTime gameTime);
        /// <summary>
        /// Draw the item.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public abstract void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Change the visibility state of this item.
        /// </summary>
        /// <param name="isVisible">Whether the item will be visible or not.</param>
        protected virtual void ChangeVisibilityState(bool isVisible)
        {
            _IsVisible = isVisible;
        }
        /// <summary>
        /// Change the position of this item.
        /// </summary>
        /// <param name="position">The new scale to change into.</param>
        protected virtual void PositionChangeInvoke(Vector2 position)
        {
            //If the position is the same as before, stop here.
            if (_Position == position) { return; }

            //Change the position.
            _Position = position;
        }
        /// <summary>
        /// Change the scale of this item.
        /// </summary>
        /// <param name="scale">The new scale to change into.</param>
        protected virtual void ScaleChangeInvoke(Vector2 scale)
        {
            //If the scale is the same as before, stop here.
            if (_Scale == scale) { return; }

            //Change the scale.
            _Scale = scale;
        }
        /// <summary>
        /// See if a vector position collides with this item.
        /// </summary>
        /// <param name="point">The point of the would-be collision.</param>
        /// <returns>Whether the point collides or not.</returns>
        public virtual bool IsPixelsIntersecting(Vector2 point)
        {
            //If the point and this item intersects, return true.
            return Helper.IsPointWithinBox(point, Helper.GetBoundingBox(this));
        }
        /// <summary>
        /// Clones the item.
        /// </summary>
        /// <returns>A clone of this item.</returns>
        public abstract Item Clone();
        #endregion

        #region Properties
        /// <summary>
        /// The level that this item belongs to.
        /// </summary>
        public Level Level
        {
            get { return _Level; }
            set { _Level = value; }
        }
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// The position of the item.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
            set { PositionChangeInvoke(value); }
        }
        /// <summary>
        /// The rotation of the item.
        /// </summary>
        public float Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }
        /// <summary>
        /// The scale of the item.
        /// </summary>
        public Vector2 Scale
        {
            get { return _Scale; }
            set { ScaleChangeInvoke(value); }
        }
        /// <summary>
        /// The width of the item.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        /// <summary>
        /// The height of the item.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        /// <summary>
        /// The origin of the item.
        /// </summary>
        public Vector2 Origin
        {
            get { return _Origin; }
            set { _Origin = value; }
        }
        /// <summary>
        /// Whether the level is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { ChangeVisibilityState(value); }
        }
        /// <summary>
        /// The type that this item is part of.
        /// </summary>
        public ItemType Type
        {
            get { return _Type; }
        }
        #endregion
    }
}
