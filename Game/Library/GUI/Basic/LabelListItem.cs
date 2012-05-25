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
    /// Label list items are items used to populate a list with label data.
    /// </summary>
    public class LabelListItem : ListItem
    {
        #region Fields
        private Label _Label;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a list item.
        /// </summary>
        /// <param name="gui">The GUI that this list item will be a part of.</param>
        /// <param name="list">The list this list item is a part of.</param>
        /// <param name="position">The position of this list item.</param>
        /// <param name="height">The height of this list item.</param>
        /// <param name="width">The width of this list item.</param>
        public LabelListItem(GraphicalUserInterface gui, List list, Vector2 position, float width, float height)
            : base(gui, list, position, width, height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the list item.
        /// </summary>
        /// <param name="gui">The GUI that this list item will be a part of.</param>
        /// <param name="position">The position of this list item.</param>
        /// <param name="height">The height of this list item.</param>
        /// <param name="width">The width of this list item.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Label = new Label(gui, Position, Width, Height);
        }
        /// <summary>
        /// Load the content of this list item.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Load the label's content.
            _Label.LoadContent();
        }
        /// <summary>
        /// Update the list item.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);

            //Update the label.
            _Label.Update(gametime);
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
                    //Let the label handle user input.
                    _Label.HandleInput(input);
                }
            }
        }
        /// <summary>
        /// Draw the list item.
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

                    //Draw the label.
                    _Label.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Update the label's bounds.
            _Label.Width = width;
            _Label.Height = height;

            //Invoke the base event method.
            base.BoundsChangeInvoke(width, height);
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Pass along the call to the base.
            base.PositionChangeInvoke(position);

            //Update the label's position.
            _Label.Position = Position;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The label of this list item.
        /// </summary>
        public Label Label
        {
            get { return _Label; }
            set { _Label = value; }
        }
        #endregion
    }
}
