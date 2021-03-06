﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Library.Imagery
{
    /// <summary>
    /// The frame of a sprite.
    /// </summary>
    public class Frame
    {
        #region Fields
        private string _Path;
        private float _Width;
        private float _Height;
        private Vector2 _Origin;
        private Texture2D _Texture;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="path">The path of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        public Frame(string path, float width, float height)
        {
            Intialize(path, null, width, height, Vector2.Zero);
        }
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="path">The path of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        /// <param name="origin">The origin of the frame.</param>
        public Frame(string path, float width, float height, Vector2 origin)
        {
            Intialize(path, null, width, height, origin);
        }
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="texture">The texture of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        public Frame(Texture2D texture, float width, float height)
        {
            Intialize("", texture, width, height, Vector2.Zero);
        }
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="texture">The texture of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        /// <param name="origin">The origin of the frame.</param>
        public Frame(Texture2D texture, float width, float height, Vector2 origin)
        {
            Intialize("", texture, width, height, origin);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Intialize the frame.
        /// </summary>
        /// <param name="path">The path of the frame.</param>
        /// <param name="texture">The texture of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        /// <param name="origin">The origin of the frame texture.</param>
        public void Intialize(string path, Texture2D texture, float width, float height, Vector2 origin)
        {
            //Intialize a few variables.
            _Path = path;
            _Texture = texture;
            _Height = height;
            _Width = width;
            _Origin = origin;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The path of the frame.
        /// </summary>
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }
        /// <summary>
        /// The texture of the frame.
        /// </summary>
        public Texture2D Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }
        /// <summary>
        /// The origin of the frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return _Origin; }
            set { _Origin = value; }
        }
        /// <summary>
        /// The x-coordinate of the origin of the frame.
        /// </summary>
        public float OriginX
        {
            get { return _Origin.X; }
            set { _Origin.X = value; }
        }
        /// <summary>
        /// The y-coordinate of the origin of the frame.
        /// </summary>
        public float OriginY
        {
            get { return _Origin.Y; }
            set { _Origin.Y = value; }
        }
        /// <summary>
        /// The frame height.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        /// <summary>
        /// The frame width.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        #endregion
    }
}