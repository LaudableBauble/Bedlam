using System;
using System.IO;
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

using Library.GUI.Basic;
using Library.Animate;
using Library.Infrastructure;
using Library.Core;
using Library.Enums;

namespace Library.GUI
{
    /// <summary>
    /// This form enables the user to add and modify items in a level.
    /// </summary>
    public class ModifyItemForm : Form
    {
        #region Fields
        private Item _Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Create an item form.
        /// </summary>
        /// <param name="gui">The GUI that this form will be a part of.</param>
        /// <param name="position">The position of this form.</param>
        /// <param name="height">The height of this form.</param>
        /// <param name="width">The width of this form.</param>
        public ModifyItemForm(GraphicalUserInterface gui, Vector2 position, float width, float height)
            : base(gui, position, width, height) { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the item form.
        /// </summary>
        /// <param name="gui">The GUI that this form will be a part of.</param>
        /// <param name="position">The position of this form.</param>
        /// <param name="height">The height of this form.</param>
        /// <param name="width">The width of this form.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Item = null;
            Expander expander1 = new Expander(GUI, Position + new Vector2(5, 5), Width - 10, 15);
            Textbox textbox1 = new Textbox(GUI, Position + new Vector2(5, 25), Width - 10, 15);
            Label label1 = new Label(GUI, Position + new Vector2(5, 45), Width - 10, 15);
            Expander expander2 = new Expander(GUI, Position + new Vector2(5, 65), Width - 10, 15);
            Textbox textbox2 = new Textbox(GUI, Position + new Vector2(5, 85), Width - 10, 15);
            Label label2 = new Label(GUI, Position + new Vector2(5, 105), Width - 10, 15);

            //Edit the components.
            label1.Text = "Label Test 1";
            label2.Text = "Label Test 2";

            //Add controls to the expander.
            expander1.AddItem(textbox1);
            expander1.AddItem(label1);
            expander2.AddItem(textbox2);
            expander2.AddItem(label2);

            //Add the controls.
            AddItem(expander1);
            AddItem(expander2);

            //Hook up to some events.
        }
        /// <summary>
        /// Load the content of this item form.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the item form.
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
                    if (HasFocus) { }
                }
            }
        }
        /// <summary>
        /// Draw the item form.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Update all inner components.
        /// </summary>
        private void UpdateComponents()
        {

        }
        /// <summary>
        /// Update the inner components when an item type has been selected.
        /// </summary>
        /// <param name="o">The object to fire the event.</param>
        /// <param name="e">The evebt arguments.</param>
        protected void OnItemTypeSelect(object o, ItemSelectEventArgs e)
        {
            //Update the inner components.
            UpdateComponents();
        }
        #endregion

        #region Properties
        #endregion
    }
}
