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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for a field.
        /// </summary>
        /// <param name="gui">The GUI that this field will be a part of.</param>
        /// <param name="position">The position of this field.</param>
        /// <param name="height">The height of this field.</param>
        /// <param name="width">The width of this field.</param>
        public Field(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            Initialize(gui, position, width, height);
        }
        /// <summary>
        /// Constructor for a field.
        /// </summary>
        /// <param name="gui">The GUI that this field will be a part of.</param>
        /// <param name="height">The height of this field.</param>
        /// <param name="width">The width of this field.</param>
        public Field(GraphicalUserInterface gui, float width, float height)
        {
            Initialize(gui, Vector2.Zero, width, height);
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
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Ratio = .4f;
            _Label = new Label(GUI, Position, Width * _Ratio, Height);
            _Textbox = new Textbox(GUI, new Vector2(Position.X + Width * _Ratio, Position.Y), Width * (1 - _Ratio), Height);
            _IsFixed = false;
            _Label.Text = "";
            _Textbox.Text = "";

            //Add the items.
            Add(_Label);
            Add(_Textbox);
        }

        /// <summary>
        /// Update the components' position and bounds when the bounds or position of this item changes.
        /// </summary>
        protected override void UpdateComponents()
        {
            //If the item has been recently modified update the components' position, width and height.
            _Label.Position = Position;
            _Label.Width = Width * _Ratio;
            _Label.Height = Height;
            _Textbox.Position = new Vector2(Position.X + Width * _Ratio, Position.Y);
            _Textbox.Width = Width * (1 - _Ratio);
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
