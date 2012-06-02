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
    /// An item modifier is a GUI component capable of altering the behaviour and properties of an item.
    /// </summary>
    public class ItemModifier : Component
    {
        #region Fields
        private Item _Item;
        private TabControl _TabControl;
        private Field fldWidth;
        private Field fldHeight;
        #endregion

        #region Constructors
        /// <summary>
        /// Create an item form.
        /// </summary>
        /// <param name="gui">The GUI that this form will be a part of.</param>
        /// <param name="position">The position of this form.</param>
        /// <param name="height">The height of this form.</param>
        /// <param name="width">The width of this form.</param>
        public ItemModifier(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            Initialize(gui, position, width, height);
        }
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

            //Create the tab controller.
            _TabControl = new TabControl(gui, position, width, height);

            //Create the general tab.
            Form frmGeneral = new Form(gui, position + new Vector2(0, 25), width, height);
            Expander expBounds = new Expander(gui, position, width - 10, 15);
            fldWidth = new Field(gui, width - 10, 15);
            fldHeight = new Field(gui, width - 10, 15);
            Expander expPosition = new Expander(gui, Position, width - 10, 15);
            Field fldX = new Field(gui, width - 10, 15);
            Field fldY = new Field(gui, width - 10, 15);

            //Set up the components.
            SetUpComponents();

            //Add controls to the expander.
            expBounds.AddItem(fldWidth);
            expBounds.AddItem(fldHeight);
            expPosition.AddItem(fldX);
            expPosition.AddItem(fldY);

            //Add the controls to the form.
            frmGeneral.AddItem(expBounds);
            frmGeneral.AddItem(expPosition);

            //Add the controls to their respective tabs.
            _TabControl.AddTab(frmGeneral);

            //Add the tab control to this modifier.
            Add(_TabControl);
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
