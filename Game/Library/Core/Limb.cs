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

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Library.Factories;
using Library.Imagery;

namespace Library.Core
{
    /// <summary>
    /// A limb is a connection between a body and its sprites. A body is only a set of invisible points in space that can collide, whereas a sprite can be seen with the naked eye.
    /// A limb binds these two together into a cohesive unit.
    /// </summary>
    public class Limb
    {
        #region Fields
        private Body _Body;
        private List<Sprite> _Sprites;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a part.
        /// </summary>
        public Limb()
        {
            //Initialize stuff.
            _Sprites = new List<Sprite>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the part.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        public void Update(GameTime gameTime)
        {
            //Loop through all sprites.
            foreach (Sprite sprite in _Sprites)
            {
                //Update the sprite according to the state of the body.
                sprite.Position = ConvertUnits.ToDisplayUnits(_Body.Position);
                sprite.Rotation = _Body.Rotation;
                sprite.Update(gameTime);
            }
        }

        /// <summary>
        /// Set the body.
        /// </summary>
        /// <param name="body">The body to use.</param>
        private void SetBody(Body body)
        {
            //Set the body.
            _Body = body;
        }
        /// <summary>
        /// Add a sprite to the list.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        public void AddSprite(Sprite sprite)
        {
            //Add a sprite to the list.
            _Sprites.Add(sprite);
        }
        /// <summary>
        /// Return the sprite at the given index.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <returns>The sprite.</returns>
        public Sprite GetSprite(int index)
        {
            //Return the sprite at the given index.
            return _Sprites[index];
        }
        /// <summary>
        /// Scale the limb.
        /// </summary>
        /// <param name="scale">The scale to apply.</param>
        /// <param name="oldScale">The old scale.</param>
        public void Scale(Vector2 scale, Vector2 oldScale)
        {
            //Get the relative scale. Needed for the polygon scaling.
            Vector2 rScale = scale / oldScale;

            //For all the body's fixtures, scale them.
            foreach (Fixture fixture in new List<Fixture>(_Body.FixtureList))
            {
                //Depending on the type of the shapes, do different stuff.
                switch (fixture.ShapeType)
                {
                    case ShapeType.Polygon:
                        {
                            //Cast the shape to a polygon shape and scale it.
                            (fixture.Shape as PolygonShape).Vertices.Scale(ref rScale);
                            break;
                        }
                }
            }

            //Scale the sprites as well.
            _Sprites.ForEach(item => item.Scale = scale);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The body of the part.
        /// </summary>
        public Body Body
        {
            get { return _Body; }
            set { SetBody(value); }
        }
        /// <summary>
        /// The list of sprites available to the body.
        /// </summary>
        public List<Sprite> Sprites
        {
            get { return _Sprites; }
        }
        #endregion
    }
}
