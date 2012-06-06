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
using FarseerPhysics.Collision.Shapes;
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
    public class Entity : Item
    {
        #region Fields
        protected List<Limb> _Limbs;
        protected SpriteManager _Sprites;
        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor for an entity. Does not intialize the entity.
        /// </summary>
        public Entity() { }
        /// <summary>
        /// Constructor for an entity.
        /// <param name="level">The level that this item belongs to.</param>
        /// </summary>
        public Entity(Level level)
        {
            Initialize(level, "", Vector2.Zero, 0, Vector2.Zero, 1, 1);
        }
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
        protected override void Initialize(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            //Call the base method.
            base.Initialize(level, name, position, rotation, scale, width, height);

            //Initialize a few variables.
            _Limbs = new List<Limb>();
            _Sprites = new SpriteManager();
            _Type = Enums.ItemType.Entity;
        }
        /// <summary>
        /// Load the entity's content.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            _Sprites.LoadContent(contentManager);
        }
        /// <summary>
        /// Update the entity.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        public override void Update(GameTime gameTime)
        {
            //Update the sprite collection.
            _Sprites.Update(gameTime);
            //Loop through all parts and update them.
            foreach (Limb limb in _Limbs) { limb.Update(gameTime); }
        }
        /// <summary>
        /// Draw the entity.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            _Sprites.Draw(spriteBatch);
        }

        /// <summary>
        /// Add a limb to the entity.
        /// </summary>
        /// <param name="limb">The limb to add.</param>
        /// <returns>The added limb.</returns>
        public Limb AddLimb(Limb limb)
        {
            //Add the limb to the list.
            _Limbs.Add(limb);

            //Return the limb.
            return limb;
        }
        /// <summary>
        /// Change the visibility state of this item.
        /// </summary>
        /// <param name="isVisible">Whether the item will be visible or not.</param>
        protected override void ChangeVisibilityState(bool isVisible)
        {
            //Call the base method.
            base.ChangeVisibilityState(isVisible);

            //Change the visibility state of the sprite collection.
            _Sprites.Visibility = IsVisible ? Visibility.Visible : Visibility.Invisible;
        }
        /// <summary>
        /// Change the scale of this entity.
        /// </summary>
        /// <param name="scale">The new scale to change into.</param>
        protected override void ScaleChangeInvoke(Vector2 scale)
        {
            //If the scale is the same as before, stop here.
            if (_Scale == scale) { return; }

            //Change the scale of the limbs.
            _Limbs.ForEach(item => item.Scale(scale, _Scale));

            //Call the base method.
            base.ScaleChangeInvoke(scale);
        }
        /// <summary>
        /// Clone the entity.
        /// </summary>
        /// <returns>A clone of this entity.</returns>
        public override Item Clone()
        {
            //Create the clone.
            Entity clone = new Entity(_Level);

            //Clone the properties.
            clone.Level = _Level;
            clone.Name = _Name;
            clone.Position = _Position;
            clone.Rotation = _Rotation;
            clone.Scale = _Scale;
            clone.Width = _Width;
            clone.Height = _Height;
            clone.IsVisible = _IsVisible;
            clone.Origin = _Origin;
            clone.Sprites = _Sprites.Clone();

            //Clone the limbs.
            foreach (Limb limb in _Limbs)
            {
                //Create the cloned limb.
                Limb lClone = new Limb();
                lClone.Body = limb.Body.DeepClone();

                //Match the limb's sprites to the cloned ones.
                foreach (Sprite sprite in limb.Sprites) { lClone.AddSprite(clone.Sprites[_Sprites.IndexOf(sprite)]); }

                //Add the cloned limb.
                clone.AddLimb(lClone);
            }

            //Return the clone.
            return clone;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of limbs this entity has.
        /// </summary>
        public List<Limb> Limbs
        {
            get { return _Limbs; }
            set { _Limbs = value; }
        }
        /// <summary>
        /// The sprites of the entity.
        /// </summary>
        public SpriteManager Sprites
        {
            get { return _Sprites; }
            set { _Sprites = value; }
        }
        /// <summary>
        /// The mass of the entity.
        /// </summary>
        public float Mass
        {
            get
            {
                //The mass.
                float mass = 0;

                //Get the sum of all the limbs' masses.
                _Limbs.ForEach(item => mass += item.Body.Mass);

                //Return the mass.
                return mass;
            }
            set { _Limbs.ForEach(item => item.Body.Mass = value); }
        }
        /// <summary>
        /// The friction of the entity.
        /// </summary>
        public float Friction
        {
            get
            {
                //The friction.
                float friction = 0;

                //Get the sum of all the limbs' frictions.
                _Limbs.ForEach(item => friction += item.Body.Friction);

                //Return the average friction.
                return friction / _Limbs.Count;
            }
            set { _Limbs.ForEach(item => item.Body.Friction = value); }
        }
        /// <summary>
        /// The restitution of the entity.
        /// </summary>
        public float Restitution
        {
            get
            {
                //The restitution.
                float restitution = 0;

                //Get the sum of all the limbs' restitution.
                _Limbs.ForEach(item => restitution += item.Body.Restitution);

                //Return the average restitution.
                return restitution / _Limbs.Count;
            }
            set { _Limbs.ForEach(item => item.Body.Restitution = value); }
        }
        /// <summary>
        /// Whether the entity is static.
        /// </summary>
        public bool IsStatic
        {
            get { return _Limbs.Exists(item => item.Body.IsStatic); }
            set { _Limbs.ForEach(item => item.Body.IsStatic = value); }
        }
        /// <summary>
        /// Whether the entity is ignoring gravity.
        /// </summary>
        public bool IgnoreGravity
        {
            get { return _Limbs.Exists(item => item.Body.IgnoreGravity); }
            set { _Limbs.ForEach(item => item.Body.IgnoreGravity = value); }
        }
        #endregion
    }
}