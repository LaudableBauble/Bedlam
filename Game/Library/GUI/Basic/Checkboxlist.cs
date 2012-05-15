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
    /// A checkbox list lists a number of checkbox components in a single dimensional order.
    /// </summary>
    public class Checkboxlist : List
    {
        #region Fields
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item instance.</returns>
        public new CheckboxListItem this[int index]
        {
            get { return (_Items[index] as CheckboxListItem); }
            set { _Items[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a checkbox list.
        /// </summary>
        /// <param name="gui">The GUI that this checkbox list will be a part of.</param>
        /// <param name="position">The position of this checkbox list.</param>
        /// <param name="height">The height of this checkbox list.</param>
        /// <param name="width">The width of this checkbox list.</param>
        public Checkboxlist(GraphicalUserInterface gui, Vector2 position, float width, float height)
            : base(gui, position, width, height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the checkbox list.
        /// </summary>
        /// <param name="gui">The GUI that this checkbox list will be a part of.</param>
        /// <param name="position">The position of this checkbox list.</param>
        /// <param name="height">The height of this checkbox list.</param>
        /// <param name="width">The width of this checkbox list.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);
        }
        /// <summary>
        /// Load the content of this checkbox list.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the checkbox list.
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

                    }
                }
            }
        }
        /// <summary>
        /// Draw the checkbox list and all its items.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Add a field item to the list.
        /// </summary>
        public override void AddItem()
        {
            //Add the child node to the list of other nodes.
            Items.Add(new CheckboxListItem(GUI, this, CalculateItemPosition(Items.Count), CalculateItemWidth(), _ItemHeight));
            //Hook up some events.
            Items[Items.Count - 1].MouseClick += OnItemClick;
            //Call the event.
            ItemAddedInvoke(_Items[_Items.Count - 1]);
        }
        #endregion

        #region Properties
        #endregion
    }
}
