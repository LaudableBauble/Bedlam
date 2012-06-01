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
        private Field fldWidth;
        private Field fldHeight;
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
            Expander expGeneral = new Expander(GUI, Position + new Vector2(5, 5), Width - 10, 15);
            fldWidth = new Field(GUI, Width - 10, 15);
            fldHeight = new Field(GUI, Width - 10, 15);
            Expander expander2 = new Expander(GUI, Position + new Vector2(5, 65), Width - 10, 15);
            Textbox textbox2 = new Textbox(GUI, Position + new Vector2(5, 85), Width - 10, 15);
            Label label2 = new Label(GUI, Position + new Vector2(5, 105), Width - 10, 15);

            //Set up the components.
            SetUpComponents();

            //Add controls to the expander.
            expGeneral.AddItem(fldWidth);
            expGeneral.AddItem(fldHeight);
            expander2.AddItem(textbox2);
            expander2.AddItem(label2);

            //Add the controls.
            AddItem(expGeneral);
            AddItem(expander2);
        }

        /// <summary>
        /// Set up the components.
        /// </summary>
        private void SetUpComponents()
        {
            //Edit the components.
            fldWidth.Title = "Width:";
            fldHeight.Title = "Height:";

            //If no item has been selected, stop here.
            if (_Item == null) { return; }

            //Display the item's data.
            fldWidth.Text = _Item.Width.ToString();
            fldHeight.Text = _Item.Height.ToString();
        }
        /// <summary>
        /// Update the inner components when an item type has been selected.
        /// </summary>
        /// <param name="o">The object to fire the event.</param>
        /// <param name="e">The event arguments.</param>
        protected void OnItemTypeSelect(object o, ItemSelectEventArgs e)
        {
            UpdateComponents();
        }
        protected void ItemChangeInvoke(Item item)
        {
            //If the item is already selected, end here.
            if (_Item == item) { return; }

            //Switch to the new item.
            _Item = item;

            //Reset the component data.
            SetUpComponents();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The item this form will be modifying.
        /// </summary>
        public Item Item
        {
            get { return _Item; }
            set { ItemChangeInvoke(value); }
        }
        #endregion
    }
}
