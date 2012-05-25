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
    /// Buttons enable users to make a decision by clicking on something.
    /// </summary>
    public class Button : Component
    {
        #region Fields
        private string _Text;
        private SpriteFont _Font;
        private int _VisibleTextLength;
        private Sprite _DefaultSprite;

        public delegate void TextChangeHandler(object obj, EventArgs e);
        public event TextChangeHandler TextChange;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a button.
        /// </summary>
        /// <param name="gui">The GUI that this item will be a part of.</param>
        /// <param name="position">The position of this item.</param>
        /// <param name="height">The height of this item.</param>
        /// <param name="width">The width of this item.</param>
        public Button(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        /// <summary>
        /// Create a button.
        /// </summary>
        /// <param name="gui">The GUI that this item will be a part of.</param>
        /// <param name="position">The position of this item.</param>
        public Button(GraphicalUserInterface gui, Vector2 position)
        {
            //Initialize some variables.
            Initialize(gui, position, 15, 15);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the button.
        /// </summary>
        /// <param name="gui">The GUI that this item will be a part of.</param>
        /// <param name="position">The position of this item.</param>
        /// <param name="height">The height of this item.</param>
        /// <param name="width">The width of this item.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Text = "";
            _VisibleTextLength = 0;
        }
        /// <summary>
        /// Load the content of this button.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create the button's texture and load the font.
            _Font = GUI.ContentManager.Load<SpriteFont>("GameScreen/Fonts/diagnosticFont");
            //Create the default button texture.
            _DefaultSprite = AddSprite(DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, Color.SlateGray, Color.Black));

            //Fit and align the text accordingly, now that the font has finally been created properly.
            FitAndAlignText();
        }
        /// <summary>
        /// Update the button.
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
        /// Draw the button.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //Draw the text.
            GUI.SpriteBatch.DrawString(_Font, CropText(), new Vector2((Position.X + 2), (Position.Y + 2)), Color.White);
        }

        /// <summary>
        /// Fit and align the text that will be published in the button.
        /// </summary>
        private void FitAndAlignText()
        {
            //Try this.
            try
            {
                //If the full text does not fit in the item.
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
                //Otherwise just use the string's length.
                else { _VisibleTextLength = _Text.Length; }
            }
            //Catch.
            catch { }
        }
        /// <summary>
        /// Crop this button's text so that it will fit the given publication area.
        /// </summary>
        /// <returns>The cropped text.</returns>
        private string CropText()
        {
            //Crop the text so that it will fit into the box.
            return (_Text.Substring(0, _VisibleTextLength));
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
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Fit and align the text.
            FitAndAlignText();
            //Update the default button texture.
            if (GUI.ContentManager != null) { _DefaultSprite[0].Texture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)width, (int)height, Color.SlateGray, Color.Black); }

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
        #endregion

        #region Properties
        /// <summary>
        /// The text that currently is displayed on this item.
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set { _Text = value; TextChangeInvoke(); }
        }
        /// <summary>
        /// The font that is used by this item.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        /// <summary>
        /// The length of the text to be displayed.
        /// </summary>
        public int VisibleTextLength
        {
            get { return _VisibleTextLength; }
        }
        /// <summary>
        /// The default sprite.
        /// </summary>
        public Sprite DefaultSprite
        {
            get { return _DefaultSprite; }
        }
        #endregion
    }
}