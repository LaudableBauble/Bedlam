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
    /// Menus are used to provide the user with a way to navigate the application.
    /// </summary>
    public class Menu : Component
    {
        #region Fields
        private MenuItem _SelectedItem;
        private float _Border;
        private float _ItemWidth;

        public delegate void MenuItemSelectHandler(object obj, MenuItemSelectEventArgs e);
        public delegate void MenuOptionSelectHandler(object obj, ListItemSelectEventArgs e);
        public event MenuItemSelectHandler MenuItemSelect;
        public event MenuOptionSelectHandler MenuOptionSelect;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an menu item.
        /// </summary>
        /// <param name="index">The index of the menu item.</param>
        /// <returns>The menu item instance.</returns>
        public MenuItem this[int index]
        {
            get { return ((MenuItem)_Items[index]); }
            set { _Items[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a menu.
        /// </summary>
        /// <param name="gui">The GUI that this menu will be a part of.</param>
        /// <param name="position">The position of this menu.</param>
        /// <param name="height">The height of this menu.</param>
        /// <param name="width">The width of this menu.</param>
        public Menu(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the menu.
        /// </summary>
        /// <param name="gui">The GUI that this menu will be a part of.</param>
        /// <param name="position">The position of this menu.</param>
        /// <param name="height">The height of this menu.</param>
        /// <param name="width">The width of this menu.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Border = 2;
            _ItemWidth = 100;
        }
        /// <summary>
        /// Load the content of this menu.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Update the list.
            UpdateList();
        }
        /// <summary>
        /// Update the menu.
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
        /// Draw the menu and all its items.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //Draw the items that fits within the list's bounds.
            //foreach (MenuItem item in _MenuItems) { if (Helper.IsPointWithinBox(item.Position, Position, Width, (Height - (2 * _Border)))) { item.Draw(spriteBatch); } }
        }

        /// <summary>
        /// Add a menu item to the list.
        /// </summary>
        public void AddMenuItem()
        {
            //Add the child node to the list of other nodes.
            Component item = Add(new MenuItem(GUI, CalculateMenuItemPosition(_Items.Count), _ItemWidth, Height));
            //Hook up some events.
            AddItemEvents((MenuItem)item);
        }
        /// <summary>
        /// Insert a menu item.
        /// </summary>
        /// <param name="index">The index of where to insert the menu item.</param>
        /// <param name="item">The menu item to insert.</param>
        public void InsertMenuItem(int index, MenuItem item)
        {
            //Insert the item to the list.
            _Items.Insert(index, item);
            //Hook up some events.
            AddItemEvents((MenuItem)_Items[_Items.Count - 1]);
        }
        /// <summary>
        /// Subscribe to an item's events.
        /// </summary>
        private void AddItemEvents(MenuItem item)
        {
            //Hook up some events.
            item.MouseClick += OnMenuItemClick;
            item.ItemSelect += OnMenuOptionSelect;
        }
        /// <summary>
        /// Update the all items' position in the list.
        /// </summary>
        public void UpdateList()
        {
            //The maximum number of items that fits the list's bounds.
            int count = (int)((Width - (2 * _Border)) / _ItemWidth);
            //The position furthest down the list.
            Vector2 last = CalculateMenuItemPosition(0);

            //Reset all items' positions.
            foreach (Component item in _Items) { item.Position = Vector2.Zero; }

            //Loop through all items and update their positions.
            for (int i = 0; i < Math.Min(_Items.Count, count); i++)
            {
                //Calculate their new position, but only if they are visible.
                if (_Items[i].IsVisible) { _Items[i].Position = last; last = new Vector2((last.X + _ItemWidth), last.Y); }
                //Otherwise skip this item.
                else { if (i < (_Items.Count - 1)) { count++; } }
            }
        }
        /// <summary>
        /// Move an item upwards in the menu.
        /// </summary>
        /// <param name="item">The menu item to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the top, this method will return false.</returns>
        public bool MoveNodeUp(MenuItem item)
        {
            //First see if the item actually exists in this list.
            if (_Items.Exists(n => (n.Equals(item))))
            {
                //Get the index position of the item.
                int index = _Items.FindIndex(i => (i.Equals(item)));

                //If the item can climb in the list.
                if (index != 0)
                {
                    //Remove the item from the list.
                    _Items.Remove(item);
                    //Insert the item at one step up from its past position in the list.
                    _Items.Insert((index - 1), item);

                    //Wrap it all up by returning true.
                    return true;
                }
                //Otherwise return false.
                else { return false; }
            }
            //Otherwise return false.
            else { return false; }
        }
        /// <summary>
        /// Move a menu item downwards in the menu.
        /// </summary>
        /// <param name="item">The menu item to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the bottom, this method will return false.</returns>
        public bool MoveNodeDown(MenuItem item)
        {
            //First see if the item actually exists in this list.
            if (_Items.Exists(n => (n.Equals(item))))
            {
                //Get the index position of the item.
                int index = _Items.FindIndex(i => (i.Equals(item)));

                //If the item can descend in the list.
                if (index != (_Items.Count - 1))
                {
                    //Remove the item from the list.
                    _Items.Remove(item);
                    //Insert the item at one step down from its past position in the list.
                    _Items.Insert((index + 1), item);

                    //Wrap it all up by returning true.
                    return true;
                }
                //Otherwise return false.
                else { return false; }
            }
            //Otherwise return false.
            else { return false; }
        }
        /// <summary>
        /// A menu item has been clicked on by a mouse.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuItemClick(object obj, MouseClickEventArgs e)
        {
            //Save the clicked node as the selected node.
            MenuItemSelectInvoke(obj as MenuItem);
        }
        /// <summary>
        /// A menu item's list item has been selected.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuOptionSelect(object obj, ListItemSelectEventArgs e)
        {
            //Save the clicked node as the selected node.
            MenuOptionSelectInvoke(e.Item);
        }
        /// <summary>
        /// A menu item has been selected.
        /// </summary>
        /// <param name="item">The selected menu item.</param>
        private void MenuItemSelectInvoke(MenuItem item)
        {
            //Save the selected item.
            _SelectedItem = item;

            //If someone has hooked up a delegate to the event, fire it.
            if (MenuItemSelect != null) { MenuItemSelect(this, new MenuItemSelectEventArgs(item)); }
        }
        /// <summary>
        /// A menu item's list item has been selected.
        /// </summary>
        /// <param name="item">The selected list item.</param>
        private void MenuOptionSelectInvoke(ListItem item)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (MenuOptionSelect != null) { MenuOptionSelect(this, new ListItemSelectEventArgs(item)); }
        }
        /// <summary>
        /// Calculate the position of a menu item, ignoring whether the item is visible or not.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The vector position.</returns>
        private Vector2 CalculateMenuItemPosition(int index)
        {
            //Return the calculated vector.
            return new Vector2((Position.X + _Border + (index * _ItemWidth)), (Position.Y + _Border));
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Pass along the call to the base.
            base.PositionChangeInvoke(position);

            //Update the list and all its items' positions.
            UpdateList();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The menu items in the menu.
        /// </summary>
        public List<MenuItem> MenuItems
        {
            get { return _Items.ConvertAll<MenuItem>(delegate(Component item) { return (MenuItem)item; }); }
            set { _Items = value.Cast<Component>().ToList<Component>(); }
        }
        #endregion
    }
}
