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
    /// A tabpage is an item that populates a tab controller.
    /// </summary>
    public class TabPage : Component
    {
        #region Fields
        private Button _Button;
        private bool _IsHiding;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a tabpage.
        /// </summary>
        /// <param name="gui">The GUI that this tabpage will be a part of.</param>
        /// <param name="position">The position of this tabpage.</param>
        /// <param name="height">The height of this tabpage.</param>
        /// <param name="width">The width of this tabpage.</param>
        public TabPage(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the list item.
        /// </summary>
        /// <param name="gui">The GUI that this tabpage will be a part of.</param>
        /// <param name="position">The position of this tabpage.</param>
        /// <param name="height">The height of this tabpage.</param>
        /// <param name="width">The width of this tabpage.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Button = new Button(gui, Position, Width, Height);
            _IsHiding = true;

            //Add the controls.
            Add(_Button);

            //Subscribe to some events.
            _Button.MouseClick += OnButtonClick;
        }
        /// <summary>
        /// Load the content of this tabpage.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the tabpage.
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
                if (IsVisible) { }
            }
        }
        /// <summary>
        /// Draw the tabpage.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //If the item is active, continue.
            if (IsActive)
            {
                //If the item is visible.
                if (IsVisible)
                {
                    //The inherited method.
                    base.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Add an item to the tabpage.
        /// </summary>
        /// <param name="item">The items to be displayed on the tab page.</param>
        public void AddItem(Component item)
        {
            //Call the base method.
            Add(item);

            //Make sure that the item follows the tab page's visibility demands.
            if (_IsHiding) { item.IsActive = false; }
        }
        /// <summary>
        /// Change the state of hiding for this tabpage.
        /// </summary>
        /// <param name="isHiding">Whether the tabpage should be in hiding or not.</param>
        public void HidingChangeInvoke(bool isHiding)
        {
            //If the new value is not different from the previous, stop here.
            if (_IsHiding == isHiding) { return; }

            //Pass along the value.
            _IsHiding = isHiding;

            //Change hiding state.
            switch (_IsHiding)
            {
                case true: { _Items.FindAll(item => item != _Button).ForEach(item => item.IsActive = false); break; }
                case false: { _Items.FindAll(item => item != _Button).ForEach(item => item.IsActive = true); break; }
            }
        }
        /// <summary>
        /// This tab page's button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        protected void OnButtonClick(object obj, MouseClickEventArgs e)
        {
            //Invoke the tab page's mouse click method.
            base.MouseClickInvoke(e.Position, e.Button);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The caption button of this tabpage.
        /// </summary>
        public Button Button
        {
            get { return _Button; }
            set { _Button = value; }
        }
        /// <summary>
        /// Whether the tabpage is in hiding.
        /// </summary>
        public bool IsHiding
        {
            get { return _IsHiding; }
            set { _IsHiding = value; }
        }
        #endregion
    }
}
