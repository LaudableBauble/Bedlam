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
    /// Whether the scroller operates on a horizontal or vertical basis.
    /// </summary>
    public enum ScrollerType
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Scrollers lets the user cycle through a list without exceeding preset boundaries.
    /// </summary>
    public class Scroller : Component
    {
        #region Fields
        private float _Value;
        private ScrollerType _Type;
        private Button _Thumb;
        private Button _Backward;
        private Button _Forward;
        private bool _IsThumbDown;

        public delegate void ValueChangeHandler(object obj, EventArgs e);
        public event ValueChangeHandler ValueChange;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a scroller.
        /// </summary>
        /// <param name="gui">The GUI that this scroller will be a part of.</param>
        /// <param name="position">The position of this scroller.</param>
        /// <param name="type">Whether the item scrolls horizontally or vertically.</param>
        /// <param name="length">The length of this scroller.</param>
        public Scroller(GraphicalUserInterface gui, Vector2 position, ScrollerType type, float length)
        {
            //Initialize some variables.
            Initialize(gui, position, type, length);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the scroller.
        /// </summary>
        /// <param name="gui">The GUI that this scroller will be a part of.</param>
        /// <param name="position">The position of the scroller.</param>
        /// <param name="type">The scroller type, that is whether it scrolls horizontally or vertically.</param>
        /// <param name="length">The length of this scroller. Minimum is 80.</param>
        public void Initialize(GraphicalUserInterface gui, Vector2 position, ScrollerType type, float length)
        {
            //Call the base class and pass along the appropriate information.
            if (type == ScrollerType.Horizontal) { base.Initialize(gui, position, Math.Max(length, 80), 15); }
            else { base.Initialize(gui, position, 15, Math.Max(length, 80)); }

            //Initialize some variables.
            _Type = type;
            _Value = 0;
            _Thumb = new Button(GUI, Position);
            _Backward = new Button(GUI, Position);
            _Forward = new Button(GUI, Position);

            //Add the controls.
            Add(_Thumb);
            Add(_Backward);
            Add(_Forward);

            //Hook up some events.
            _Thumb.MouseDown += OnThumbDown;
            _Backward.MouseDown += OnBackwardDown;
            _Forward.MouseDown += OnForwardDown;
            _Backward.MouseClick += OnBackwardClick;
            _Forward.MouseClick += OnForwardClick;
        }
        /// <summary>
        /// Load the content of this scroller.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Update the buttons' positions and bounds.
            UpdateButtons();
        }
        /// <summary>
        /// Update the scroller.
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

            //If the scroller is active.
            if (IsActive)
            {
                //If the scroller is visible.
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
        /// Draw the scroller.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Get the length of the scroller.
        /// </summary>
        public float GetLength()
        {
            //Check the scroller type and return the correct value.
            if (_Type == ScrollerType.Horizontal) { return Width; }
            else { return Height; }
        }
        /// <summary>
        /// Set the length of the scroller.
        /// </summary>
        /// <param name="length">The new length of the scroller.</param>
        public void SetLength(float length)
        {
            //Check the scroller type and set the correct value.
            if (_Type == ScrollerType.Horizontal) { Width = length; }
            else { Height = length; }
        }
        /// <summary>
        /// Calculate the thumb button's position.
        /// </summary>
        public Vector2 CalculateThumbPosition()
        {
            //Check the scroller type and return the correct value.
            if (_Type == ScrollerType.Horizontal)
            {
                //The position of the thumb button.
                return new Vector2(Math.Min(Math.Max(((Position.X + 25) + (_Value * (((Width - 50) - _Thumb.Width) / 100))), (Position.X + 25)),
                    ((Position.X + 25) + ((Width - 50) - _Thumb.Width))), Position.Y);
            }
            else
            {
                //The position of the thumb button.
                return new Vector2(Position.X, Math.Min(Math.Max(((Position.Y + 25) + (_Value * (((Height - 50) - _Thumb.Height) / 100))), (Position.Y + 25)),
                    ((Position.Y + 25) + ((Height - 50) - _Thumb.Height))));
            }
        }
        /// <summary>
        /// Calculate the thumb button's length.
        /// </summary>
        public float CalculateThumbLength()
        {
            //Check the scroller type and return the correct value.
            if (_Type == ScrollerType.Horizontal) { return 30; }
            else { return 30; }
        }
        /// <summary>
        /// Update the scroller and its buttons.
        /// </summary>
        public void UpdateButtons()
        {
            //Check the scroller type and return the correct value.
            if (_Type == ScrollerType.Horizontal)
            {
                //Update the position and bounds for every button.
                _Backward.Width = 25;
                _Backward.Height = 15;
                _Backward.Position = Position;
                _Thumb.Width = CalculateThumbLength();
                _Thumb.Height = 15;
                _Thumb.Position = CalculateThumbPosition();
                _Forward.Width = 25;
                _Forward.Height = 15;
                _Forward.Position = Vector2.Add(Position, new Vector2((Width - 25), 0));
            }
            else
            {
                //Update the position and bounds for every button.
                _Backward.Width = 15;
                _Backward.Height = 25;
                _Backward.Position = Position;
                _Thumb.Width = 15;
                _Thumb.Height = CalculateThumbLength();
                _Thumb.Position = CalculateThumbPosition();
                _Forward.Width = 15;
                _Forward.Height = 25;
                _Forward.Position = Vector2.Add(Position, new Vector2(0, (Height - 25)));
            }
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Update the scroller's buttons.
            UpdateButtons();

            //Invoke the base method.
            base.BoundsChangeInvoke(width, height);
        }
        /// <summary>
        /// Tell the world that the value of this scroller has changed.
        /// </summary>
        /// <param name="value">The new value of the scroller.</param>
        protected virtual void ValueChangeInvoke(float value)
        {
            //Update the value.
            _Value = Math.Max(Math.Min(value, 100), 0);
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
            if (_Type == ScrollerType.Horizontal) { ValueChangeInvoke(((e.Position.X - (Position.X + 25)) / (Width - 50)) * 100); }
            else { ValueChangeInvoke(((e.Position.Y - (Position.Y + 25)) / (Height - 50)) * 100); }
        }
        /// <summary>
        /// The backward button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnBackwardClick(object obj, MouseClickEventArgs e)
        {
            //Scroll backward a bit.
            ValueChangeInvoke(_Value - 5);
        }
        /// <summary>
        /// The forward button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnForwardClick(object obj, MouseClickEventArgs e)
        {
            //Scroll forward a bit.
            ValueChangeInvoke(_Value + 5);
        }
        /// <summary>
        /// The backward button has been held down by the mouse.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnBackwardDown(object obj, MouseClickEventArgs e)
        {
            //Scroll backward a bit.
            ValueChangeInvoke(_Value - 1);
        }
        /// <summary>
        /// The forward button has been held down by the mouse.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnForwardDown(object obj, MouseClickEventArgs e)
        {
            //Scroll forward a bit.
            ValueChangeInvoke(_Value + 1);
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
        /// The length of this scroller.
        /// </summary>
        public float Length
        {
            get { return GetLength(); }
            set { SetLength(value); }
        }
        /// <summary>
        /// Whether this item scrolls on the horizontal or vertical plane.
        /// </summary>
        public ScrollerType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        #endregion
    }
}
