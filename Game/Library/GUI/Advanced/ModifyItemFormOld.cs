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
    public class ModifyItemFormOld : Form
    {
        #region Fields
        private Combobox _ItemTypeCombobox;
        private Combobox _ItemCombobox;
        private Item _Item;

        public delegate void ItemAddedHandler(object obj, BoneCreatedEventArgs e);
        public event ItemAddedHandler ItemAdded;
        #endregion

        #region Constructor
        /// <summary>
        /// Create an item form.
        /// </summary>
        /// <param name="gui">The GUI that this form will be a part of.</param>
        /// <param name="position">The position of this form.</param>
        /// <param name="height">The height of this form.</param>
        /// <param name="width">The width of this form.</param>
        public ModifyItemFormOld(GraphicalUserInterface gui, Vector2 position, float width, float height)
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
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Item = null;
            _ItemTypeCombobox = new Combobox(GUI, Position, Width - 10, 15);
            _ItemCombobox = new Combobox(GUI, Position, Width - 10, 15);
            Textbox textbox = new Textbox(GUI, Position, Width - 10, 15);

            //Initialize the item type combobox.
            foreach (ItemType item in Enum.GetValues(typeof(ItemType)))
            {
                //Add a item to the combobox.
                _ItemTypeCombobox.List.AddItem();
                //Get the added item and change its text.
                (_ItemTypeCombobox.List.Items[_ItemTypeCombobox.List.Items.Count - 1] as LabelListItem).Label.Tag = item;
                (_ItemTypeCombobox.List.Items[_ItemTypeCombobox.List.Items.Count - 1] as LabelListItem).Label.Text = item.ToString();
            }

            //Add the controls.
            AddItem(_ItemTypeCombobox);
            AddItem(_ItemCombobox);
            AddItem(textbox);

            //Hook up to some events.
            _ItemTypeCombobox.ItemSelect += OnItemTypeSelect;
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
            //Clear the list.
            _ItemCombobox.List.Clear();
            //The deciding enum.
            ItemType source = (_ItemTypeCombobox.Label.Tag != null) ? (ItemType)_ItemTypeCombobox.Label.Tag : ItemType.Character;
            Type derivative = null;

            //Get the desired enum.
            switch (source)
            {
                case (ItemType.Character): { derivative = typeof(Characters); break; }
                case (ItemType.Entity): { derivative = typeof(Enums.Entities); break; }
                case (ItemType.Item): { derivative = typeof(Items); break; }
                case (ItemType.TextureItem): { derivative = typeof(Items); break; }
            }

            //Try the operation below.
            try
            {
                //Initialize the item combobox.
                foreach (Enum e in Enum.GetValues(derivative))
                {
                    //Add a item to the combobox.
                    _ItemCombobox.List.AddItem();
                    //Get the added item and change its text.
                    (_ItemCombobox.List.Items[_ItemCombobox.List.Items.Count - 1] as LabelListItem).Label.Tag = e;
                    (_ItemCombobox.List.Items[_ItemCombobox.List.Items.Count - 1] as LabelListItem).Label.Text = e.ToString();
                    (_ItemCombobox.List.Items[_ItemCombobox.List.Items.Count - 1] as LabelListItem).LoadContent();
                }
            }
            catch { }
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
