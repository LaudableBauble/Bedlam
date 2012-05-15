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

using Library.Infrastructure;
using Library.Imagery;

namespace Library.GUI.Basic
{
    /// <summary>
    /// Labels are used to display text.
    /// </summary>
    public class Label : Component
    {
        #region Fields
        private string _Text;
        private object _Tag;
        private SpriteFont _Font;
        private int _VisibleTextLength;

        public delegate void TextChangeHandler(object obj, EventArgs e);
        public event TextChangeHandler TextChange;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a label.
        /// </summary>
        /// <param name="gui">The GUI that this label will be a part of.</param>
        /// <param name="position">The position of this label.</param>
        /// <param name="height">The height of this label.</param>
        /// <param name="width">The width of this label.</param>
        public Label(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the label.
        /// </summary>
        /// <param name="gui">The GUI that this label will be a part of.</param>
        /// <param name="position">The position of this label.</param>
        /// <param name="height">The height of this label.</param>
        /// <param name="width">The width of this label.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Text = "";
            _Tag = null;
            _VisibleTextLength = 0;
        }
        /// <summary>
        /// Load the content of this label.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create the label's texture and load the font.
            CreateTexture();
            _Font = GUI.ContentManager.Load<SpriteFont>("GameScreen/Fonts/diagnosticFont");

            //Fit and align the text accordingly, now that the font has finally been created properly.
            FitAndAlignText();
        }
        /// <summary>
        /// Update the label.
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

            //If the item is active.
            if (IsActive)
            {
                //If the item is visible.
                if (IsVisible)
                {
                    //If the item has focus.
                    if (HasFocus) { }
                }
            }
        }
        /// <summary>
        /// Draw the label.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //Draw the text.
            DrawText(CropText());
        }

        /// <summary>
        /// Draw some text on the label this frame.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        private void DrawText(string text)
        {
            //Only draw if the label is active and visible.
            if (!_IsActive || !_IsVisible) { return; }

            //Try (sprite batch may be null) to draw the text.
            try { GUI.SpriteBatch.DrawString(_Font, text, new Vector2((Position.X + 2), Position.Y), Color.White); }
            catch { }
        }

        /// <summary>
        /// Fit and align the text that will be published in the label.
        /// </summary>
        private void FitAndAlignText()
        {
            //Try this.
            try
            {
                //If the full text does not fit in the text box.
                if (_Font.MeasureString(_Text).X > Width)
                {
                    //Loop through all chars in the string and stop when the string fits best.
                    for (int i = 0; i <= _Text.Length; i++)
                    {
                        //If the string does fit the box, save the new InputIndexLength position.
                        if (_Font.MeasureString(_Text.Substring(0, i) + "...").X < Width) { _VisibleTextLength = i; }
                        //Otherwise, break this loop.
                        else { break; }
                    }
                }
                //Otherwise just use the list's length.
                else { _VisibleTextLength = _Text.Length; }
            }
            //Catch.
            catch { }
        }
        /// <summary>
        /// Crop this label's text so that it will fit the given publication area.
        /// </summary>
        /// <returns>The cropped text.</returns>
        private string CropText()
        {
            try
            {
                //Crop the text so that it will fit into the box.
                return (_Text.Substring(0, _VisibleTextLength));
            }
            catch { return ""; }
        }
        /// <summary>
        /// Create a new background texture for the label.
        /// </summary>
        private void CreateTexture()
        {
            try
            {
                //The texture.
                Texture2D texture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, new Color(0, 0, 0, 155));

                //If there already exists a sprite, just switch a new texture to it.
                if (_Sprite.GetLastSprite() != null) { _Sprite.GetLastSprite().Texture = texture; }
                //Otherwise create a new sprite.
                else { AddSprite(texture); }
            }
            catch { }
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Fit and align the text.
            FitAndAlignText();
            //Create the new texture.
            CreateTexture();

            //Direct the call to the base event.
            base.BoundsChangeInvoke(width, height);
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Fit and align the text.
            FitAndAlignText();

            //Direct the call to the base event.
            base.PositionChangeInvoke(position);
        }
        /// <summary>
        /// Tell the world that the text of this item has changed.
        /// </summary>
        private void TextChangeInvoke()
        {
            //Fit and align the text.
            FitAndAlignText();

            //If someone has hooked up a delegate to the event, fire it.
            if (TextChange != null) { TextChange(this, new EventArgs()); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The text that currently is displayed on this label.
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; TextChangeInvoke(); }
        }
        /// <summary>
        /// The label's tag.
        /// </summary>
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }
        /// <summary>
        /// The font that is used by this label.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        /// <summary>
        /// The texture of the label.
        /// </summary>
        public Texture2D Texture
        {
            get { return _Sprite[0].Texture; }
            set { _Sprite[0].Texture = value; }
        }
        /// <summary>
        /// The length of the text to be displayed.
        /// </summary>
        public int VisibleTextLength
        {
            get { return _VisibleTextLength; }
        }
        #endregion
    }
}
