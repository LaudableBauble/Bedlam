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
    /// A box is simply a rectangular object operating under the laws of physics. If you want it to, that is.
    /// </summary>
    public class Box : Entity
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Create a box.
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
        private override void Initialize(Level level, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            //Call the base method.
            base.Initialize(level, name, position, rotation, scale, width, height);

            //Create a body.
            Body body = BodyFactory.CreateRectangle(level.World, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 1, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
            //Create a part to hold the body and sprite.
            Factory.Instance.AddPart(this, body);
        }
        public override void Update(GameTime gameTime)
        {
            //Call the base method.
            base.Update(gameTime);

            //Update the position and rotation of the box.
            Position = ConvertUnits.ToDisplayUnits(Parts[0].Body.Position);
            Rotation = Parts[0].Body.Rotation;
        }

        /// <summary>
        /// Set the sprite at the first position in the first part of the box.
        /// </summary>
        /// <param name="sprite">The sprite to set.</param>
        public void SetSprite(Sprite sprite)
        {
            //As this box only is supposed to have one part, the index of the part to set the sprite for is zero.
            Parts[0].AddSprite(sprite);
        }
        #endregion
    }
}
