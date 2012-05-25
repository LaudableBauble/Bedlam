using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
    /// Whether the menu item is open or closed.
    /// </summary>
    public enum MenuState
    {
        Closed,
        Open
    }
    /// <summary>
    /// The type of the menu item.
    /// </summary>
    public enum MenuType
    {
        NewAnimation,
        OpenAnimation,
        SaveAnimation,
        CreateBone,
        CreateKeyframe,
        DeleteKeyframe
    }

    /// <summary>
    /// A menu item supplies the user with a drop down list in which he can choose from several options.
    /// </summary>
    public class MenuItem : Component
    {
        #region Fields
        private Label _Label;
        private List _List;
        private ListItem _SelectedItem;
        private MenuState _State;

        public delegate void MenuItemSelectHandler(object obj, ListItemSelectEventArgs e);
        public event MenuItemSelectHandler ItemSelect;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set a list item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item instance.</returns>
        public LabelListItem this[int index]
        {
            get { return (_List.Items[index] as LabelListItem); }
            set { _List.Items[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a menu item.
        /// </summary>
        /// <param name="gui">The GUI that this menu item will be a part of.</param>
        /// <param name="position">The position of this menu item.</param>
        /// <param name="height">The height of this menu item.</param>
        /// <param name="width">The width of this menu item.</param>
        public MenuItem(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the menu item.
        /// </summary>
        /// <param name="gui">The GUI that this menu item will be a part of.</param>
        /// <param name="position">The position of this menu item.</param>
        /// <param name="height">The height of this menu item.</param>
        /// <param name="width">The width of this menu item.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Label = new Label(GUI, Position, Width, 30);
            _List = new List(GUI, new Vector2(Position.X, (Position.Y + (_Label.Height + 2))), Width, 118);
            _List.IsActive = false;
            _SelectedItem = null;
            _State = MenuState.Closed;
            _Label.Text = "";

            //Add the controls.
            Add(_Label);
            Add(_List);

            //Hook up some events.
            _Label.MouseClick += OnLabelClick;
            _List.ItemSelect += OnItemSelect;
        }
        /// <summary>
        /// Load the content of this menu item.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the menu item.
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
        /// Draw the menu item.
        /// </summary>
        /// <param name="spritebatch">The spritebatch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Add an item to the list.
        /// </summary>
        public void AddListItem()
        {
            //Add a new item to the list.
            _List.AddItem();
        }
        /// <summary>
        /// Update the label's position and bounds when the bounds or position of this item changes.
        /// </summary>
        private void UpdateComponents()
        {
            //If the menu item has been recently modified update the label's and list's position, width and height.
            _Label.Position = Position;
            _Label.Width = Width;
            _Label.Height = Height;
            _List.Position = new Vector2(Position.X, (Position.Y + (_Label.Height + 2)));
            _List.Width = (GetDesiredWidth() + (2 * _List.Border) + 2);
            _List.Height = Height;
        }
        /// <summary>
        /// Get the longest desired width of an item in the list.
        /// </summary>
        /// <returns>The longest desired width.</returns>
        private float GetDesiredWidth()
        {
            //The longest desired width. Start with default.
            float width = Width;

            //Loop through all list item's text.
            foreach (ListItem item in _List.Items)
            {
                //Measure the current item's text.
                float w = (item as LabelListItem).Label.Font.MeasureString((item as LabelListItem).Label.Text).X;
                //If the current text is longer than the previous, overwrite it with the old one.
                if (w > width) { width = w; }
            }

            //Return the longest desired width.
            return width;
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Call the base method.
            base.BoundsChangeInvoke(width, height);

            //Update the components' positions and bounds.
            UpdateComponents();
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Call the base method.
            base.PositionChangeInvoke(position);

            //Update the components' positions and bounds.
            UpdateComponents();
        }
        /// <summary>
        /// Change the focus of this item.
        /// </summary>
        /// <param name="hasFocus">Whether the item has been granted focus or not.</param>
        protected override void FocusChangeInvoke(bool hasFocus)
        {
            //Call the base method.
            base.FocusChangeInvoke(hasFocus);

            //If the combobox has lost focus and still is open, close it.
            if ((_HasFocus != hasFocus) && !HasFocus && (_State == MenuState.Open)) { SwitchState(); }
        }
        /// <summary>
        /// Tell the world that a child item has been pressed.
        /// </summary>
        /// <param name="item">The particular item.</param>
        protected virtual void ItemSelectInvoke(ListItem item)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (ItemSelect != null) { ItemSelect(this, new ListItemSelectEventArgs(item)); }
        }

        /// <summary>
        /// The label has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnLabelClick(object obj, MouseClickEventArgs e)
        {
            //Bring the list to front.
            _List.DrawOrder = 0;

            //Request a change in focus.
            _GUI.RequestFocus(_List);
        }
        /// <summary>
        /// An item in the list has been selected.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnItemSelect(object obj, ListItemSelectEventArgs e)
        {
            //Change the selected item.
            _SelectedItem = e.Item;
            //Invoke the event.
            ItemSelectInvoke(e.Item);
        }
        /// <summary>
        /// An item has had its focus change.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        protected override void OnItemFocusChange(object obj, FocusChangeEventArgs e)
        {
            //Call the base method.
            base.OnItemFocusChange(obj, e);

            //Try.
            try
            {
                //If the list has lost focus but still is open, turn it off.
                if (!((List)e.Item).HasFocus && (_State == MenuState.Open)) { SwitchState(); }
                //If the list has gained focus but isn't open, turn it on.
                else if (((List)e.Item).HasFocus && (_State == MenuState.Closed)) { SwitchState(); }
            }
            catch { }
        }
        /// <summary>
        /// Switch this menu item's state.
        /// </summary>
        private void SwitchState()
        {
            //Switch state.
            if (_State == MenuState.Open)
            {
                //Close the list.
                _State = MenuState.Closed;
                _List.IsActive = false;
            }
            else
            {
                //Open the list.
                _State = MenuState.Open;
                _List.IsActive = true;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The text that currently is displayed on this checkbox.
        /// </summary>
        public string Text
        {
            get { return _Label.Text; }
            set { _Label.Text = value; }
        }
        /// <summary>
        /// The font that is used by this checkbox.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Label.Font; }
            set { _Label.Font = value; }
        }
        /// <summary>
        /// The label that this checkbox uses.
        /// </summary>
        public Label Label
        {
            get { return _Label; }
            set { _Label = value; }
        }
        /// <summary>
        /// The list that this checkbox uses.
        /// </summary>
        public List List
        {
            get { return _List; }
            set { _List = value; }
        }
        /// <summary>
        /// Whether the list is down or not.
        /// </summary>
        public MenuState State
        {
            get { return _State; }
            set { _State = value; }
        }
        /// <summary>
        /// Whether the list has been outfitted with a scroller or not.
        /// </summary>
        public bool IsScrollable
        {
            get { return _List.IsScrollable; }
            set { _List.IsScrollable = value; }
        }
        /// <summary>
        /// Whether the list's capacity is fixed or not.
        /// </summary>
        public bool IsFixed
        {
            get { return _List.IsFixed; }
            set { _List.IsFixed = value; }
        }
        #endregion
    }
}
