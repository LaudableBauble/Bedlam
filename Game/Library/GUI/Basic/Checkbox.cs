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
    /// A checkbox enables the user to make a 'yes or no' decision regarding something.
    /// </summary>
    public class Checkbox : Component
    {
        #region Fields
        private Label _Label;
        private Button _Button;
        private float _Ratio;
        private bool _IsChecked;

        public delegate void CheckboxTickHandler(object obj, TickEventArgs e);
        public event CheckboxTickHandler CheckboxTick;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a checkbox.
        /// </summary>
        /// <param name="gui">The GUI that this checkbox will be a part of.</param>
        /// <param name="position">The position of this checkbox.</param>
        /// <param name="height">The height of this checkbox.</param>
        /// <param name="width">The width of this checkbox.</param>
        public Checkbox(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the checkbox.
        /// </summary>
        /// <param name="gui">The GUI that this checkbox will be a part of.</param>
        /// <param name="position">The position of this checkbox.</param>
        /// <param name="height">The height of this checkbox.</param>
        /// <param name="width">The width of this checkbox.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Ratio = .1f;
            _Label = new Label(GUI, new Vector2(Position.X + (Width * _Ratio), Position.Y), (Width * (1 - _Ratio)), Height);
            _Button = new Button(GUI, Position, (Width * _Ratio), Height);
            _IsChecked = false;
            _Label.Text = "";

            //Add the controls.
            Add(_Label);
            Add(_Button);

            //Hook up some events.
            _Button.MouseClick += OnButtonClicked;
        }
        /// <summary>
        /// Load the content of this checkbox.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Add texture to the button.
            CreateButtonTexture();
        }
        /// <summary>
        /// Update the checkbox.
        /// </summary>
        /// <param name="gameTime">The time to adhere to.</param>
        public override void Update(GameTime gameTime)
        {
            //The inherited method.
            base.Update(gameTime);
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
        /// Draw the checkbox.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Create the button textures.
        /// </summary>
        private void CreateButtonTexture()
        {
            //Get the button's default texture and add a frame to that.
            _Button.DefaultSprite.AddFrame(DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)(Width * _Ratio), (int)Height, Color.Red, Color.Black));
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Call the base method.
            base.BoundsChangeInvoke(width, height);

            //Update the components' positions and bounds.
            UpdateComponents();
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Call the base method.
            base.PositionChangeInvoke(position);

            //Update the components' positions and bounds.
            UpdateComponents();
        }
        /// <summary>
        /// Update the label's and button's position and bounds when the bounds or position of this item changes.
        /// </summary>
        private void UpdateComponents()
        {
            //If the item has been recently modified update the label's and button's position, width and height.
            _Label.Position = new Vector2(Position.X + (Width * _Ratio), Position.Y);
            _Label.Width = (Width * (1 - _Ratio));
            _Label.Height = Height;
            _Button.Position = Position;
            _Button.Width = (Width * _Ratio);
            _Button.Height = Height;
        }
        /// <summary>
        /// When the button has been pressed, check the checkbox.
        /// </summary>
        /// <param name="obj">The object whose bounds changed.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonClicked(object obj, MouseClickEventArgs e)
        {
            //Tick the checkbox.
            CheckboxTickedInvoke(!_IsChecked);
        }
        /// <summary>
        /// Tell the world that the checkbox has either been unticked or ticked.
        /// </summary>
        /// <param name="isChecked">Whether the checkbox just got checked or not.</param>
        private void CheckboxTickedInvoke(bool isChecked)
        {
            //Update the variable.
            _IsChecked = isChecked;
            //Change the button's sprite, if possible.
            if (_Button.Sprite.SpriteCount != 0)
            {
                if (_IsChecked) { _Button.Sprite[0].CurrentFrameIndex = 1; }
                else { _Button.Sprite[0].CurrentFrameIndex = 0; }
            }

            //If someone has hooked up a delegate to the event, fire it.
            if (CheckboxTick != null) { CheckboxTick(this, new TickEventArgs(_IsChecked)); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The text that currently is displayed on this checkbox.
        /// </summary>
        public string Text
        {
            get { return _Label.Text; }
            set { _Label.Text = value; }
        }
        /// <summary>
        /// The font that is used by this checkbox.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Label.Font; }
            set { _Label.Font = value; }
        }
        /// <summary>
        /// The label that this checkbox uses.
        /// </summary>
        public Label Label
        {
            get { return _Label; }
            set { _Label = value; }
        }
        /// <summary>
        /// The button that this checkbox uses.
        /// </summary>
        public Button Button
        {
            get { return _Button; }
            set { _Button = value; }
        }
        /// <summary>
        /// The ratio between the label's width and the checkbox's width.
        /// </summary>
        public float Ratio
        {
            get { return _Ratio; }
            set { _Ratio = value; }
        }
        /// <summary>
        /// Whether the checkbox is checked or not.
        /// </summary>
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { CheckboxTickedInvoke(value); }
        }
        #endregion
    }
}
