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

namespace Library.GUI.Basic
{
    /// <summary>
    /// An expander control supplies the user with a drop down array of items when clicked.
    /// </summary>
    public class Expander : Component
    {
        #region Fields
        private Button _Button;
        private Label _Header;
        private bool _IsExpanded;
        private Layout _Layout;
        private List<Component> _ItemContent;
        #endregion

        #region Constructor
        /// <summary>
        /// Create an expander.
        /// </summary>
        /// <param name="gui">The GUI that this expander will be a part of.</param>
        /// <param name="position">The position of this expander.</param>
        /// <param name="height">The height of this expander.</param>
        /// <param name="width">The width of this expander.</param>
        public Expander(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the expander.
        /// </summary>
        /// <param name="gui">The GUI that this expander will be a part of.</param>
        /// <param name="position">The position of this expander.</param>
        /// <param name="height">The height of this expander.</param>
        /// <param name="width">The width of this expander.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Button = new Button(GUI, Position, 15, 15);
            _Header = new Label(GUI, Position + new Vector2(20, 0), 75, Height);
            _IsExpanded = true;
            _Layout = new Layout(GUI, Position + new Vector2(0, 15), _Width, _Height);
            _ItemContent = new List<Component>();
            _Header.Text = "Header";

            //Add the items.
            Add(_Header);
            Add(_Button);

            //Hook up some events.
            _Button.MouseClick += OnHeaderClick;
            _Header.MouseClick += OnHeaderClick;
        }
        /// <summary>
        /// Load the content of this combobox.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the combobox.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);

            //Update the layout.
            _Layout.Update();
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
        /// Draw the combobox.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Add an item to the expander control.
        /// </summary>
        /// <param name="item">The item to be displayed on the expander control.</param>
        public void AddItem(Component item)
        {
            //Add the item to this control.
            _ItemContent.Add(item);
            _Layout.Add(item);
            Add(item);

            //Change the size of the layout to accomodate for the new item.
            _Layout.SetToDesiredSize();
            //Calculate the complete size of the expander.
            UpdateTrueSize();

            //Perform the additional event subscribing here.
        }
        /// <summary>
        /// Update the position and bounds of all components located in the component.
        /// </summary>
        protected override void UpdateComponents()
        {
            //If the item has been recently modified update the label's and button's position, width and height.
            _Button.Position = Position;
            _Header.Position = Position + new Vector2(20, 0);
            _Layout.Position = Position + new Vector2(0, 15);
        }
        /// <summary>
        /// Calculate the true size of this expander, one including all the child items.
        /// </summary>
        private void UpdateTrueSize()
        {
            //Update the extender's size.
            Width = _IsExpanded ? _Layout.Width : _Button.Width + 20 + _Header.Width;
            Height = _IsExpanded ? _Layout.Height + Math.Max(_Button.Height, _Header.Height) : Math.Max(_Button.Height, _Header.Height);
        }
        /// <summary>
        /// The header has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnHeaderClick(object obj, MouseClickEventArgs e)
        {
            //Either expand or contract the control.
            SwitchState();
        }
        /// <summary>
        /// Switch this combobox's state.
        /// </summary>
        private void SwitchState()
        {
            //Expand or contract the control.
            _IsExpanded = !_IsExpanded;
            _ItemContent.ForEach(item => item.IsActive = _IsExpanded);

            //Update the size of the expander control.
            UpdateTrueSize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The text that currently is displayed on this checkbox.
        /// </summary>
        public string Text
        {
            get { return _Header.Text; }
            set { _Header.Text = value; }
        }
        /// <summary>
        /// The font that is used by this checkbox.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Header.Font; }
            set { _Header.Font = value; }
        }
        /// <summary>
        /// The button that this control uses.
        /// </summary>
        public Button Button
        {
            get { return _Button; }
            set { _Button = value; }
        }
        /// <summary>
        /// The header of this control.
        /// </summary>
        public Label Header
        {
            get { return _Header; }
            set { _Header = value; }
        }
        /// <summary>
        /// Whether the control is expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; }
        }
        #endregion
    }
}
