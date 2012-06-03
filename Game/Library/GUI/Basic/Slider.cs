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

using Library.Factories;
using Library.Imagery;
using Library.Infrastructure;

namespace Library.GUI.Basic
{
    /// <summary>
    /// Whether the slider operates on a horizontal or vertical basis.
    /// </summary>
    public enum SliderType
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Sliders lets the user set values without exceeding preset boundaries.
    /// </summary>
    public class Slider : Component
    {
        #region Fields
        private float _Value;
        private SliderType _Type;
        private Button _Thumb;
        private bool _IsThumbDown;
        private float _Maximum;
        private float _Minimum;

        public delegate void ValueChangeHandler(object obj, EventArgs e);
        public event ValueChangeHandler ValueChange;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a slider.
        /// </summary>
        /// <param name="gui">The GUI that this slider will be a part of.</param>
        /// <param name="position">The position of this slider.</param>
        /// <param name="type">Whether the item slides horizontally or vertically.</param>
        /// <param name="length">The length of this slider.</param>
        public Slider(GraphicalUserInterface gui, Vector2 position, SliderType type, float length)
        {
            //Initialize some variables.
            Initialize(gui, position, type, length);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the slider.
        /// </summary>
        /// <param name="gui">The GUI that this slider will be a part of.</param>
        /// <param name="position">The position of the slider.</param>
        /// <param name="type">The scroller type, that is whether it slides horizontally or vertically.</param>
        /// <param name="length">The length of this slider. Minimum is 50.</param>
        public void Initialize(GraphicalUserInterface gui, Vector2 position, SliderType type, float length)
        {
            //Call the base class and pass along the appropriate information.
            if (type == SliderType.Horizontal) { base.Initialize(gui, position, Math.Max(length, 50), 10); }
            else { base.Initialize(gui, position, 10, Math.Max(length, 50)); }

            //Initialize some variables.
            _Type = type;
            _Value = 50;
            _Maximum = 100;
            _Minimum = 0;
            _Thumb = new Button(GUI, Vector2.Add(Position, new Vector2(-2.5f, 0)), 15, 15);

            //Add the controls.
            Add(_Thumb);

            //Hook up some events.
            _Thumb.MouseDown += OnThumbDown;
        }
        /// <summary>
        /// Load the content of this slider.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Add a sprite to the slider.
            Factory.Instance.AddSprite(Sprite, "", DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, new Color(0, 0, 0, 155), Color.Black),
                Position, 0, Vector2.One, 0, 0, 0, "Base");

            //Update the buttons' positions and bounds.
            UpdateButtons();
        }
        /// <summary>
        /// Update the slider.
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
                    //If the thumb button is currently held down but the mouse is pointing somewhere else.
                    if ((_IsThumbDown) && (!Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), _Thumb.Position, _Thumb.Width, _Thumb.Height)))
                    {
                        //If the left mouse button is still held down, pretend that the button is still held down.
                        if (input.IsNewLeftMousePress()) { OnThumbDown(this, new MouseClickEventArgs(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), MouseButton.Left)); }
                        //Otherwise the thumb button isn't held down.
                        else { _IsThumbDown = false; }
                    }
                }
            }
        }
        /// <summary>
        /// Draw the slider.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Get the length of the slider.
        /// </summary>
        public float GetLength()
        {
            //Check the scroller type and return the correct value.
            if (_Type == SliderType.Horizontal) { return Width; }
            else { return Height; }
        }
        /// <summary>
        /// Calculate the thumb button's position.
        /// </summary>
        public Vector2 CalculateThumbPosition()
        {
            //Check the slider type and return the correct value.
            if (_Type == SliderType.Horizontal)
            {
                //The position of the thumb button.
                return new Vector2(Math.Min(Math.Max((Position.X + (_Value * ((Width - _Thumb.Width) / _Maximum))), Position.X),
                    (Position.X + (Width - _Thumb.Width))), (Position.Y - 2.5f));
            }
            else
            {
                //The position of the thumb button.
                return new Vector2((Position.X - 2.5f), Math.Min(Math.Max((Position.Y + (_Value * ((Height - _Thumb.Height) / _Maximum))), Position.Y),
                    (Position.Y + (Height - _Thumb.Height))));
            }
        }
        /// <summary>
        /// Calculate the thumb button's length.
        /// </summary>
        public float CalculateThumbLength()
        {
            //Check the slider type and return the correct value.
            if (_Type == SliderType.Horizontal) { return 15; }
            else { return 15; }
        }
        /// <summary>
        /// Update the slider and its buttons.
        /// </summary>
        public void UpdateButtons()
        {
            //Check the slider type and return the correct value.
            if (_Type == SliderType.Horizontal)
            {
                //Update the position and bounds for every button.
                _Thumb.Width = CalculateThumbLength();
                _Thumb.Height = 15;
                _Thumb.Position = CalculateThumbPosition();
            }
            else
            {
                //Update the position and bounds for every button.
                _Thumb.Width = 15;
                _Thumb.Height = CalculateThumbLength();
                _Thumb.Position = CalculateThumbPosition();
            }
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Update the slider's buttons.
            UpdateButtons();

            //Invoke the base method.
            base.BoundsChangeInvoke(width, height);
        }
        /// <summary>
        /// Tell the world that the value of this slider has changed.
        /// </summary>
        /// <param name="value">The new value of the scroller.</param>
        protected virtual void ValueChangeInvoke(float value)
        {
            //Update the value.
            _Value = Math.Max(Math.Min(value, _Maximum), _Minimum);
            //Update the thumb button's position.
            _Thumb.Position = CalculateThumbPosition();

            //If someone has hooked up a delegate to the event, fire it.
            if (ValueChange != null) { ValueChange(this, new EventArgs()); }
        }
        /// <summary>
        /// The thumb button has been held down by the mouse.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnThumbDown(object obj, MouseClickEventArgs e)
        {
            //The thumb is down.
            _IsThumbDown = true;

            //Check the scroller type and set the correct value.
            if (_Type == SliderType.Horizontal) { ValueChangeInvoke(((e.Position.X - Position.X) / Width) * _Maximum); }
            else { ValueChangeInvoke(((e.Position.Y - Position.Y) / Height) * _Maximum); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The value of this scroller.
        /// </summary>
        public float Value
        {
            get { return _Value; }
            set { ValueChangeInvoke(value); }
        }
        /// <summary>
        /// The maximum value that this slider can be.
        /// </summary>
        public float Maximum
        {
            get { return _Maximum; }
            set { _Maximum = value; }
        }
        /// <summary>
        /// The minimum value that this slider can be.
        /// </summary>
        public float Minimum
        {
            get { return _Maximum; }
            set { _Maximum = value; }
        }
        /// <summary>
        /// The length of this scroller.
        /// </summary>
        public float Length
        {
            get { return GetLength(); }
        }
        /// <summary>
        /// Whether this item slides on the horizontal or vertical plane.
        /// </summary>
        public SliderType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        #endregion
    }
}
