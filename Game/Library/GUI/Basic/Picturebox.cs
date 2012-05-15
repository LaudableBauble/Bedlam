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

using Library.Imagery;
using Library.Infrastructure;

namespace Library.GUI.Basic
{
    /// <summary>
    /// A picturebox displays a picture and crops it to fit within the given area.
    /// </summary>
    public class Picturebox : Component
    {
        #region Fields
        private float _Scale;
        private Texture2D _Background;
        private Texture2D _Picture;
        private Vector2 _Origin;
        private Vector2 _PictureOrigin;
        private Rectangle _DrawArea;

        public delegate void PictureChangeHandler(object obj, EventArgs e);
        public delegate void ScaleChangeHandler(object obj, EventArgs e);
        public event PictureChangeHandler PictureChange;
        public event ScaleChangeHandler ScaleChange;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a picturebox.
        /// </summary>
        /// <param name="gui">The GUI that this node will be a part of.</param>
        /// <param name="parent">The node's parent node.</param>
        /// <param name="position">The position of this node.</param>
        /// <param name="height">The height of this node.</param>
        /// <param name="width">The width of this node.</param>
        public Picturebox(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the picturebox.
        /// </summary>
        /// <param name="gui">The GUI that this picturebox will be a part of.</param>
        /// <param name="position">The position of the picturebox.</param>
        /// <param name="width">The width of this picturebox.</param>
        /// <param name="height">The height of this picturebox.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Initialize some variables.
            _Scale = 1;
            _Origin = new Vector2(Width / 2, Height / 2);
            _PictureOrigin = Vector2.Zero;
            _DrawArea = new Rectangle(0, 0, (int)Width, (int)Height);
        }
        /// <summary>
        /// Load the content of this picturebox.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create the background texture.
            ChangeBackgroundColor(Color.CornflowerBlue);
        }
        /// <summary>
        /// Update the picturebox.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public override void HandleInput(InputState input)
        {
            //The inherited method.
            base.HandleInput(input);

            //If the inputbox is active, write the input to the box.
            if (IsActive)
            {
                //If the inputbox is visible.
                if (IsVisible)
                {
                    //If the left mouse button has been pressed.
                    if (input.IsNewLeftMouseClick())
                    {
                        //If the user clicks somewhere on the item, fire the event.
                        if (Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Position, Width, Height))
                        {

                        }
                    }
                }
            }
        }
        /// <summary>
        /// Draw the picturebox.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw the background image.
            GUI.SpriteBatch.Draw(_Background, Position, Color.White);
            //Draw the sprite, but only if it exists.
            if (_Picture != null)
            {
                //Draw the texture within the picturebox.
                GUI.SpriteBatch.Draw(_Picture, Vector2.Add(Position, _Origin), _DrawArea, Color.White, 0, _PictureOrigin, _Scale, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Change the background's color.
        /// </summary>
        /// <param name="color">The new color of the background.</param>
        public void ChangeBackgroundColor(Color color)
        {
            //Create the background texture.
            _Background = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, color, Color.Black);
        }
        /// <summary>
        /// Set the picturebox's picture.
        /// </summary>
        /// <param name="name">The name of the asset to set.</param>
        public void SetPicture(string name)
        {
            //Set the picture.
            SetPicture(GUI.ContentManager.Load<Texture2D>(name));
        }
        /// <summary>
        /// Set the picturebox's picture.
        /// </summary>
        /// <param name="texture">The texture of the asset to set.</param>
        public void SetPicture(Texture2D texture)
        {
            //Set the picture.
            PictureChangeInvoke(texture);
        }
        /// <summary>
        /// Change the visible area of the picture.
        /// </summary>
        public void ChangePictureDrawArea()
        {
            //If a is picture loaded.
            if (_Picture != null)
            {
                //Change the picture's draw area and origin.
                _DrawArea.Width = (int)(Math.Min((_Picture.Width * _Scale), Width) / _Scale);
                _DrawArea.Height = (int)(Math.Min((_Picture.Height * _Scale), Height) / _Scale);
                _DrawArea.X = (int)Math.Max((_Picture.Width / 2) - (_DrawArea.Width / 2), 0);
                _DrawArea.Y = (int)Math.Max((_Picture.Height / 2) - (_DrawArea.Height / 2), 0);
                _PictureOrigin = new Vector2((_DrawArea.Width / 2), (_DrawArea.Height / 2));
                _Origin = new Vector2(Width / 2, Height / 2);
            }
        }
        /// <summary>
        /// Change the scale of this picturebox's picture.
        /// </summary>
        /// <param name="scale">The amount of scaling.</param>
        protected virtual void ScaleChangeInvoke(float scale)
        {
            //Change the scale.
            _Scale = scale;
            //Change the picture's draw area.
            ChangePictureDrawArea();

            //If someone has hooked up a delegate to the event, fire it.
            if (ScaleChange != null) { ScaleChange(this, new EventArgs()); }
        }
        /// <summary>
        /// Change the picture of this picturebox.
        /// </summary>
        /// <param name="picture">The new picture.</param>
        protected virtual void PictureChangeInvoke(Texture2D picture)
        {
            //Change the scale.
            _Picture = picture;
            //Change the picture's draw area.
            ChangePictureDrawArea();

            //If someone has hooked up a delegate to the event, fire it.
            if (PictureChange != null) { PictureChange(this, new EventArgs()); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The background texture.
        /// </summary>
        public Texture2D Background
        {
            get { return _Background; }
            set { _Background = value; }
        }
        /// <summary>
        /// The picture displayed in this picturebox.
        /// </summary>
        public Texture2D Picture
        {
            get { return _Picture; }
            set { PictureChangeInvoke(value); }
        }
        /// <summary>
        /// The amount the picture will be scaled.
        /// </summary>
        public float Scale
        {
            get { return _Scale; }
            set { ScaleChangeInvoke(value); }
        }
        /// <summary>
        /// The origin of the picturebox's picture.
        /// </summary>
        public Vector2 Origin
        {
            get { return _Origin; }
            set { _Origin = value; }
        }
        /// <summary>
        /// The name of the picturebox's picture.
        /// </summary>
        public string Name
        {
            get { return (_Picture != null) ? _Picture.Name : ""; }
        }
        #endregion
    }
}
