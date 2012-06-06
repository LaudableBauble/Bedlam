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

using Library.Core;
using Library.Factories;
using Library.Imagery;

namespace Library.Entities
{
    /// <summary>
    /// A box is simply a rectangular object operating under the laws of physics.
    /// </summary>
    [Serializable]
    public class Box : Entity
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor for a box.
        /// </summary>
        public Box() { }
        /// <summary>
        /// Constructor for a box.
        /// </summary>
        /// <param name="level">The level that this box will belong to.</param>
        /// <param name="name">The name of the box.</param>
        /// <param name="position">The position of the box.</param>
        /// <param name="rotation">The rotation of the box.</param>
        /// <param name="scale">The scale of the box.</param>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        public Box(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            Initialize(level, name, position, rotation, scale, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the box.
        /// </summary>
        /// <param name="level">The level that this box belongs to.</param>
        /// <param name="name">The name of the box.</param>
        /// <param name="position">The position of the box.</param>
        /// <param name="rotation">The rotation of the box.</param>
        /// <param name="scale">The scale of the box.</param>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        protected override void Initialize(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            //Call the base method.
            base.Initialize(level, name, position, rotation, scale, width, height);

            //Create a body.
            Body body = BodyFactory.CreateRectangle(level.World, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
            //Create a part to hold the body and sprite.
            Factory.Instance.AddLimb(this, body);
        }
        /// <summary>
        /// Update the box.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public override void Update(GameTime gameTime)
        {
            //Call the base method.
            base.Update(gameTime);

            //Update the position and rotation of the box.
            _Position = ConvertUnits.ToDisplayUnits(Limbs[0].Body.Position);
            _Rotation = Limbs[0].Body.Rotation;
        }

        /// <summary>
        /// Set the sprite at the first position in the first part of the box.
        /// </summary>
        /// <param name="sprite">The sprite to set.</param>
        public void SetSprite(Sprite sprite)
        {
            //As this box only is supposed to have one part, the index of the part to set the sprite for is zero.
            Limbs[0].AddSprite(sprite);
        }
        /// <summary>
        /// Clone the box.
        /// </summary>
        /// <returns>A clone of this box.</returns>
        public override Item Clone()
        {
            //Create the clone.
            Box clone = new Box();

            //Clone the properties.
            clone.Sprites = _Sprites.Clone();
            clone.Limbs = new List<Limb>();
            clone.Level = _Level;
            clone.Name = _Name;
            clone.Position = _Position;
            clone.Rotation = _Rotation;
            clone.Scale = _Scale;
            clone.Width = _Width;
            clone.Height = _Height;
            clone.IsVisible = _IsVisible;
            clone.Origin = _Origin;
            clone._Type = _Type;

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
        /// <summary>
        /// Change the position of this item.
        /// </summary>
        /// <param name="position">The new scale to change into.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Call the base method.
            base.PositionChangeInvoke(position);

            //If there is exists no limbs, stop here.
            if (_Limbs.Count == 0) { return; }

            //Update the body's position.
            Limbs[0].Body.Position = ConvertUnits.ToSimUnits(_Position);
        }
        #endregion
    }
}
