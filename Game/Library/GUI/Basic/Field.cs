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
    /// A field is in essence a label and a textbox positioned after each other.
    /// </summary>
    public class Field : Component
    {
        #region Fields
        private Label _Label;
        private Textbox _Textbox;
        private float _Ratio;
        private bool _IsFixed;

        public delegate void CheckboxTickHandler(object obj, TickEventArgs e);
        public event CheckboxTickHandler CheckboxTick;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a field.
        /// </summary>
        /// <param name="gui">The GUI that this field will be a part of.</param>
        /// <param name="position">The position of this field.</param>
        /// <param name="height">The height of this field.</param>
        /// <param name="width">The width of this field.</param>
        public Field(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the field.
        /// </summary>
        /// <param name="gui">The GUI that this field will be a part of.</param>
        /// <param name="position">The position of this field.</param>
        /// <param name="height">The height of this field.</param>
        /// <param name="width">The width of this field.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Ratio = .5f;
            _Label = new Label(GUI, Position, (Width * _Ratio), Height);
            _Textbox = new Textbox(GUI, new Vector2(Position.X + (Width * _Ratio), Position.Y), (Width * (1 - _Ratio)), Height);
            _IsFixed = false;
            _Label.Text = "";
            _Textbox.Text = "";

            //Hook up some events.
        }
        /// <summary>
        /// Load the content of this field.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Load the label's and button's content.
            _Label.LoadContent();
            _Textbox.LoadContent();
        }
        /// <summary>
        /// Update the field.
        /// </summary>
        /// <param name="gameTime">The time to adhere to.</param>
        public override void Update(GameTime gameTime)
        {
            //The inherited method.
            base.Update(gameTime);

            //Update the components.
            _Label.Update(gameTime);
            _Textbox.Update(gameTime);
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
                    if (HasFocus)
                    {
                        //If the left mouse button has been pressed.
                        if (input.IsNewLeftMouseClick())
                        {
                            //If the user clicks somewhere else, defocus the item.
                            if (!Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Position, Width, Height))
                            {
                                //Defocus this item.
                                HasFocus = false;
                            }
                        }
                    }

                    //Handle the components' input.
                    _Label.HandleInput(input);
                    _Textbox.HandleInput(input);
                }
            }
        }
        /// <summary>
        /// Draw the field.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //Draw the components.
            _Label.Draw(spriteBatch);
            _Textbox.Draw(spriteBatch);
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
        /// Update the components' position and bounds when the bounds or position of this item changes.
        /// </summary>
        private void UpdateComponents()
        {
            //If the item has been recently modified update the components' position, width and height.
            _Label.Position = Position;
            _Label.Width = (Width * _Ratio);
            _Label.Height = Height;
            _Textbox.Position = new Vector2(Position.X + (Width * _Ratio), Position.Y);
            _Textbox.Width = (Width * (1 - _Ratio));
            _Textbox.Height = Height;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The title that currently is displayed.
        /// </summary>
        public string Title
        {
            get { return _Label.Text; }
            set { _Label.Text = value; }
        }
        /// <summary>
        /// The text that currently is displayed.
        /// </summary>
        public string Text
        {
            get { return _Textbox.Text; }
            set { _Textbox.Text = value; }
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
        public Textbox Textbox
        {
            get { return _Textbox; }
            set { _Textbox = value; }
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
        /// Whether the field's label has a dynamic or fixed width.
        /// </summary>
        public bool IsFixed
        {
            get { return _IsFixed; }
            set { _IsFixed = value; }
        }
        #endregion
    }
}
