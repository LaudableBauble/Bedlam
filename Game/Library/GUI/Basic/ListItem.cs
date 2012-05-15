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
    /// List items are items used to populate a list with data.
    /// </summary>
    public class ListItem : Component
    {
        #region Fields
        private List _List;
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
        public ListItem(GraphicalUserInterface gui, List list, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            _List = list;
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
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);
        }
        /// <summary>
        /// Load the content of this list item.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the list item.
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
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list this list item is a part of.
        /// </summary>
        public List List
        {
            get { return _List; }
            set { _List = value; }
        }
        #endregion
    }
}
