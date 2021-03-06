﻿using System;
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
        private Form _FrmGeneral;
        private Form _FrmPhysics;
        private Picturebox _PtbThumbnail;
        private Expander _ExpBase;
        private Expander _ExpWorld;
        private Expander _ExpLocal;
        private Field _FldWidth;
        private Field _FldHeight;
        private Field _FldPosX;
        private Field _FldPosY;
        private Field _FldRotation;
        private Field _FldScaleX;
        private Field _FldScaleY;
        private Field _FldOriginX;
        private Field _FldOriginY;
        private Field _FldMass;
        private Field _FldRestitution;
        private Field _FldFriction;
        private Checkbox _CkbIsStatic;
        private Checkbox _CkbIgnoreGravity;
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
            _FrmGeneral = new Form(gui, position + new Vector2(0, 25), width, height);
            _PtbThumbnail = new Picturebox(gui, position, width - 10, 150);
            _ExpBase = new Expander(gui, position, width - 10, 15);
            _FldWidth = new Field(gui, width - 10, 15);
            _FldHeight = new Field(gui, width - 10, 15);
            _ExpWorld = new Expander(gui, position, width - 10, 15);
            _FldPosX = new Field(gui, width - 10, 15);
            _FldPosY = new Field(gui, width - 10, 15);
            _FldRotation = new Field(gui, width - 10, 15);
            _ExpLocal = new Expander(gui, position, width - 10, 15);
            _FldScaleX = new Field(gui, width - 10, 15);
            _FldScaleY = new Field(gui, width - 10, 15);
            _FldOriginX = new Field(gui, width - 10, 15);
            _FldOriginY = new Field(gui, width - 10, 15);

            //Create the physics tab.
            _FrmPhysics = new Form(gui, position + new Vector2(0, 25), width, height);
            _FldMass = new Field(gui, width - 10, 15);
            _FldFriction = new Field(gui, width - 10, 15);
            _FldRestitution = new Field(gui, width - 10, 15);
            _CkbIsStatic = new Checkbox(gui, position, width - 10, 15);
            _CkbIgnoreGravity = new Checkbox(gui, position, width - 10, 15);

            //Set up the components.
            SetUpComponents();

            //Add controls to the expanders.
            _ExpBase.AddItem(_FldWidth);
            _ExpBase.AddItem(_FldHeight);
            _ExpWorld.AddItem(_FldPosX);
            _ExpWorld.AddItem(_FldPosY);
            _ExpWorld.AddItem(_FldRotation);
            _ExpLocal.AddItem(_FldScaleX);
            _ExpLocal.AddItem(_FldScaleY);
            _ExpLocal.AddItem(_FldOriginX);
            _ExpLocal.AddItem(_FldOriginY);

            //Add the controls to the forms.
            _FrmGeneral.AddItem(_PtbThumbnail);
            _FrmGeneral.AddItem(_ExpBase);
            _FrmGeneral.AddItem(_ExpWorld);
            _FrmGeneral.AddItem(_ExpLocal);
            _FrmPhysics.AddItem(_FldMass);
            _FrmPhysics.AddItem(_FldFriction);
            _FrmPhysics.AddItem(_FldRestitution);
            _FrmPhysics.AddItem(_CkbIsStatic);
            _FrmPhysics.AddItem(_CkbIgnoreGravity);

            //Add the controls to their respective tabs.
            _TabControl.AddTab(_FrmGeneral, "General");
            _TabControl.AddTab(_FrmPhysics, "Physics");

            //Add the tab control to this modifier.
            Add(_TabControl);

            //Subscribe to the component's events.
            _FldPosX.Textbox.FocusChange += OnUserModify;
            _FldPosY.Textbox.FocusChange += OnUserModify;
            _FldRotation.Textbox.FocusChange += OnUserModify;
            _FldScaleX.Textbox.FocusChange += OnUserModify;
            _FldScaleY.Textbox.FocusChange += OnUserModify;
            _FldOriginX.Textbox.FocusChange += OnUserModify;
            _FldOriginY.Textbox.FocusChange += OnUserModify;
            _FldMass.Textbox.FocusChange += OnUserModify;
            _FldFriction.Textbox.FocusChange += OnUserModify;
            _FldRestitution.Textbox.FocusChange += OnUserModify;
            _CkbIsStatic.CheckboxTick += OnUserModify;
            _CkbIgnoreGravity.CheckboxTick += OnUserModify;
        }
        /// <summary>
        /// Update the item modifier.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);

            //Make sure to display information that is up to date.
            UpdateInformation();
        }

        /// <summary>
        /// Set up the components.
        /// </summary>
        private void SetUpComponents()
        {
            //A text validator only accepting numbers.
            string validator = @"^[\d,-]+$";

            //Edit the components.
            _ExpBase.Text = "Base";
            _ExpWorld.Text = "World";
            _ExpLocal.Text = "Local";
            _FldWidth.Title = "Width:";
            _FldHeight.Title = "Height:";
            _FldPosX.Title = "PosX:";
            _FldPosY.Title = "PosY:";
            _FldRotation.Title = "Rotation:";
            _FldScaleX.Title = "ScaleX:";
            _FldScaleY.Title = "ScaleY:";
            _FldOriginX.Title = "OriginX:";
            _FldOriginY.Title = "OriginY:";
            _FldMass.Title = "Mass:";
            _FldFriction.Title = "Friction:";
            _FldRestitution.Title = "Restitution:";
            _CkbIsStatic.Text = "Is static?";
            _CkbIgnoreGravity.Text = "Ignores gravity?";
            _FldWidth.Textbox.Validator = validator;
            _FldHeight.Textbox.Validator = validator;
            _FldPosX.Textbox.Validator = validator;
            _FldPosY.Textbox.Validator = validator;
            _FldRotation.Textbox.Validator = validator;
            _FldScaleX.Textbox.Validator = validator;
            _FldScaleY.Textbox.Validator = validator;
            _FldOriginX.Textbox.Validator = validator;
            _FldOriginY.Textbox.Validator = validator;
            _FldWidth.Textbox.IsReadOnly = true;
            _FldHeight.Textbox.IsReadOnly = true;

            //Update the item's information.
            UpdateInformation();

            //Get the item's thumbnail.
            if (_Item is TextureItem) { _PtbThumbnail.SetPicture((_Item as TextureItem).Sprites); }
            else if (_Item is Entity) { _PtbThumbnail.SetPicture((_Item as Entity).Sprites); }
        }
        /// <summary>
        /// Make sure all components display up to date information on the selected item. 
        /// </summary>
        private void UpdateInformation()
        {
            //If no item has been selected, reset the data shown.
            if (_Item == null)
            {
                //Display the item's data.
                _FldWidth.Text = "";
                _FldHeight.Text = "";
                _FldPosX.Text = "";
                _FldPosY.Text = "";
                _FldRotation.Text = "";
                _FldScaleX.Text = "";
                _FldScaleY.Text = "";
                _FldOriginX.Text = "";
                _FldOriginY.Text = "";
                _FldMass.Text = "";
                _FldFriction.Text = "";
                _FldRestitution.Text = "";
                _PtbThumbnail.SetPicture((Texture2D)null);
                return;
            }

            //Display the item's data.
            if (!_FldWidth.Textbox.HasFocus) { _FldWidth.Text = _Item.Width.ToString(); }
            if (!_FldHeight.Textbox.HasFocus) { _FldHeight.Text = _Item.Height.ToString(); }
            if (!_FldPosX.Textbox.HasFocus) { _FldPosX.Text = _Item.Position.X.ToString(); }
            if (!_FldPosY.Textbox.HasFocus) { _FldPosY.Text = _Item.Position.Y.ToString(); }
            if (!_FldRotation.Textbox.HasFocus) { _FldRotation.Text = _Item.Rotation.ToString(); }
            if (!_FldScaleX.Textbox.HasFocus) { _FldScaleX.Text = (_Item.Width * _Item.Scale.X).ToString(); }
            if (!_FldScaleY.Textbox.HasFocus) { _FldScaleY.Text = (_Item.Height * _Item.Scale.Y).ToString(); }
            if (!_FldOriginX.Textbox.HasFocus) { _FldOriginX.Text = _Item.Origin.X.ToString(); }
            if (!_FldOriginY.Textbox.HasFocus) { _FldOriginY.Text = _Item.Origin.Y.ToString(); }

            //Display additional data depending on the type of item.
            switch (_Item.Type)
            {
                case ItemType.Entity:
                    {
                        //Cast the item to an entity.
                        Entity entity = (Entity)_Item;

                        if (!_FldMass.Textbox.HasFocus) { _FldMass.Text = entity.Mass.ToString(); }
                        if (!_FldFriction.Textbox.HasFocus) { _FldFriction.Text = entity.Friction.ToString(); }
                        if (!_FldRestitution.Textbox.HasFocus) { _FldRestitution.Text = entity.Restitution.ToString(); }
                        if (!_CkbIsStatic.HasFocus) { _CkbIsStatic.IsChecked = entity.IsStatic; }
                        if (!_CkbIgnoreGravity.HasFocus) { _CkbIgnoreGravity.IsChecked = entity.IgnoreGravity; }

                        break;
                    }
                case ItemType.Character: { goto case ItemType.Entity; }
            }
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
        /// <summary>
        /// Change the item to be selected.
        /// </summary>
        /// <param name="item">The new item to be selected.</param>
        protected void ItemChangeInvoke(Item item)
        {
            //If the item is already selected, end here.
            if (_Item == item) { return; }

            //Switch to the new item.
            _Item = item;

            //Reset the component data.
            SetUpComponents();
        }
        /// <summary>
        /// When the user has requested a modification of an item, update the selected item to match the data specified by the user.
        /// </summary>
        /// <param name="obj">The component that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUserModify(object obj, EventArgs e)
        {
            //Cast the object to a GUI component.
            Component item = (Component)obj;

            //If the textbox has not lost focus, stop here.
            //if (item.HasFocus) { return; }

            //Decide which property to modify.
            if (item == _FldPosX.Textbox) { _Item.Position = new Vector2(float.Parse(_FldPosX.Textbox.Text), _Item.Position.Y); }
            if (item == _FldPosY.Textbox) { _Item.Position = new Vector2(_Item.Position.X, float.Parse(_FldPosY.Textbox.Text)); }
            if (item == _FldRotation.Textbox) { _Item.Rotation = float.Parse(_FldRotation.Textbox.Text); }
            if (item == _FldScaleX.Textbox) { _Item.Scale = new Vector2(float.Parse(_FldScaleX.Textbox.Text) / _Item.Width, _Item.Scale.Y); }
            if (item == _FldScaleY.Textbox) { _Item.Scale = new Vector2(_Item.Scale.X, float.Parse(_FldScaleY.Textbox.Text) / _Item.Height); }
            if (item == _FldOriginX.Textbox) { _Item.Origin = new Vector2(float.Parse(_FldOriginX.Textbox.Text), _Item.Origin.Y); }
            if (item == _FldOriginY.Textbox) { _Item.Origin = new Vector2(_Item.Origin.X, float.Parse(_FldOriginY.Textbox.Text)); }

            //Modify additional properties depending on the type of item.
            switch (_Item.Type)
            {
                case ItemType.Entity:
                    {
                        //Cast the item to an entity.
                        Entity entity = (Entity)_Item;

                        if (item == _FldMass.Textbox) { entity.Mass = float.Parse(_FldMass.Text); }
                        if (item == _FldFriction.Textbox) { entity.Friction = float.Parse(_FldFriction.Text); }
                        if (item == _FldRestitution.Textbox) { entity.Restitution = float.Parse(_FldRestitution.Text); }
                        if (item == _CkbIsStatic) { entity.IsStatic = _CkbIsStatic.IsChecked; }
                        if (item == _CkbIgnoreGravity) { entity.IgnoreGravity = _CkbIgnoreGravity.IsChecked; }

                        break;
                    }
            }
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
