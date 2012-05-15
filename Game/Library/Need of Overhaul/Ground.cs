/*
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

namespace Library
{
    /// <summary>
    /// This ground resembles, naturally, the ground in the game.
    /// </summary>
    public class Ground : Object
    {
        #region Fields
        //The Id.
        private int _BodyId;

        //The Sprite name.
        private string _SpriteName;
        //The position.
        private Vector2 _Position;
        //The height.
        float _Height;
        //The width.
        private float _Width;
        #endregion

        #region Constructors
        /// <summary>
        /// The main Constructor.
        /// </summary>
        /// <param name="spriteName">The sprite name.</param>
        /// <param name="width">The width of the ground.</param>
        /// <param name="height">The height of the ground.</param>
        /// <param name="position">The position of the ground.</param>
        public Ground(string spriteName, float width, float height, Vector2 position)
        {
            //The sprite name.
            _SpriteName = spriteName;
            //The width.
            _Width = width;
            //The height.
            _Height = height;
            //The position.
            _Position = position;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="contentManager">The Content Manager.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            //Inherit the base method.
            base.LoadContent(contentManager);

            //Get the id and add a body.
            _BodyId = AddBody(_SpriteName, _Position, 1, 1, _Width, _Height, 0, 1, 0, 0, "Ground");
            //Make the body static.
            Bodies[0].IsStatic = true;
            //Set the friction coefficient.
            Geoms[0].FrictionCoefficient = 0.5f;
        }
        #endregion
    }
}
*/