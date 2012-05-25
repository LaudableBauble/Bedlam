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
    /// Lists are used to list data in a single dimensional order.
    /// </summary>
    public class List : Component
    {
        #region Fields
        protected Scroller _Scroller;
        protected SpriteFont _Font;
        protected Texture2D _BackgroundTexture;
        protected ListItem _SelectedItem;
        protected float _Border;
        protected float _ItemHeight;
        protected bool _IsScrollable;
        protected bool _IsFixed;
        protected bool _IsDirty;

        public delegate void ItemSelectHandler(object obj, ListItemSelectEventArgs e);
        public delegate void ItemAddedHandler(object obj, ListItemAddedEventArgs e);
        public event ItemSelectHandler ItemSelect;
        public event ItemAddedHandler ItemAdded;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item instance.</returns>
        public virtual Component this[int index]
        {
            get { return (_Items[index]); }
            set { _Items[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a list.
        /// </summary>
        /// <param name="gui">The GUI that this list will be a part of.</param>
        /// <param name="position">The position of this list.</param>
        /// <param name="height">The height of this list.</param>
        /// <param name="width">The width of this list.</param>
        public List(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the list.
        /// </summary>
        /// <param name="gui">The GUI that this list will be a part of.</param>
        /// <param name="position">The position of this list.</param>
        /// <param name="height">The height of this list.</param>
        /// <param name="width">The width of this list.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Scroller = new Scroller(GUI, CalculateScrollerPosition(), ScrollerType.Vertical, (Height - 10));
            _Border = 2;
            _ItemHeight = 15;
            _IsScrollable = true;
            _IsFixed = true;
            _IsDirty = true;

            //Hook up some events.
            _Scroller.ValueChange += OnScrollerChange;
        }
        /// <summary>
        /// Load the content of this list.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create the list's texture and load the font.
            UpdateBackgroundTexture();
            _Font = GUI.ContentManager.Load<SpriteFont>("GameScreen/Fonts/diagnosticFont");

            //Load the scroller.
            _Scroller.LoadContent();

            //Update the list.
            UpdateList();
        }
        /// <summary>
        /// Update the list.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);

            //If the component isn't active, stop here.
            if (!_IsActive) { return; }

            //If the list has been changed, update it.
            if (_IsDirty) { UpdateList(); _IsDirty = false; }

            //Update the scroller.
            if (_IsScrollable) { _Scroller.Update(gametime); }

            //Update every child node.
            foreach (ListItem item in _Items) { item.Update(gametime); }
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

                    //Let the scroller handle input.
                    _Scroller.HandleInput(input);
                }
            }
        }
        /// <summary>
        /// Draw the list and all its items.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //If the component isn't active or visible, stop here.
            if (!_IsActive || !_IsVisible) { return; }

            //Draw the sprite.
            _Sprite.Draw(spriteBatch);

            //Draw the texture.
            GUI.SpriteBatch.Draw(_BackgroundTexture, Position, Color.White);

            //Draw the scroller.
            if (_IsScrollable) { _Scroller.Draw(spriteBatch); }

            //Draw the items that fits within the list's bounds.
            foreach (ListItem item in _Items) { if (Helper.IsPointWithinBox(item.Position, Position, Width, (Height - (2 * _Border)))) { item.Draw(spriteBatch); } }
        }

        /// <summary>
        /// Add an item to the list.
        /// </summary>
        public virtual void AddItem()
        {
            //Create the item and load its content.
            LabelListItem item = new LabelListItem(GUI, this, CalculateItemPosition(_Items.Count), CalculateItemWidth(), _ItemHeight);
            if (_GUI.ContentManager != null) { item.LoadContent(); }
            //Add the child node to the list of other nodes.
            _Items.Add(item);
            //Hook up some events.
            item.MouseClick += OnItemClick;
            //Call the event.
            ItemAddedInvoke(item);
        }
        /// <summary>
        /// Insert an item.
        /// </summary>
        /// <param name="index">The index of where to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        public void InsertItem(int index, Component item)
        {
            //Insert the item to the list.
            _Items.Insert(index, item);
            if (_GUI.ContentManager != null) { item.LoadContent(); }
            //Hook up some events.
            item.MouseClick += OnItemClick;
            //Call the event.
            ItemAddedInvoke(item);
        }
        /// <summary>
        /// Update all items' position in the list.
        /// </summary>
        public void UpdateList()
        {
            //See if the bounds of the list have to change.
            if (!_IsFixed) { Height = CalculateListHeight(); }

            //The maximum number of items that fits the list's bounds.
            int count = (int)((Height - (2 * _Border)) / _ItemHeight);
            //The top scrolled item.
            int start = 0;
            if (_IsScrollable) { start = (int)(Math.Max(((float)(_Items.Count - (count - 1)) / 100), 0) * _Scroller.Value); }
            //The position furthest down the list.
            Vector2 last = CalculateItemPosition(0);

            //Reset all items' positions.
            foreach (Component item in _Items) { item.Position = Vector2.Zero; }

            //Loop through all items and update their positions.
            for (int i = start; i < Math.Min(_Items.Count, (start + count)); i++)
            {
                //Calculate their new position, but only if they are visible.
                if (_Items[i].IsVisible) { _Items[i].Position = last; last = new Vector2(last.X, (last.Y + _ItemHeight)); }
                //Otherwise skip this item.
                else { if (i < (_Items.Count - 1)) { count++; } }
            }

            //Update the scroller.
            UpdateScroller();
        }
        /// <summary>
        /// Update the texture to fit the current bounds of the list.
        /// </summary>
        protected void UpdateBackgroundTexture()
        {
            //Create a new background texture, if the GUI's graphics device isn't null.
            if (GUI.GraphicsDevice != null)
            {
                _BackgroundTexture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, new Color(0, 0, 0, 155));
            }
        }
        /// <summary>
        /// Move an item upwards in the list.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the top, this method will return false.</returns>
        public bool MoveNodeUp(ListItem item)
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
        /// Move an item downwards in the list.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the bottom, this method will return false.</returns>
        public bool MoveNodeDown(ListItem item)
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
        /// An item has been clicked on by a mouse.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnItemClick(object obj, MouseClickEventArgs e)
        {
            //Save the clicked node as the selected node.
            ItemSelectInvoke(obj as ListItem);
        }
        /// <summary>
        /// The scroller's value has been changed.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        protected void OnScrollerChange(object obj, EventArgs e)
        {
            //Change which list items that will be visible.
            UpdateList();
        }
        /// <summary>
        /// An item has been selected.
        /// </summary>
        /// <param name="item">The selected item.</param>
        protected virtual void ItemSelectInvoke(Component item)
        {
            //Save the selected item.
            _SelectedItem = (ListItem)item;

            //If someone has hooked up a delegate to the event, fire it.
            if (ItemSelect != null) { ItemSelect(this, new ListItemSelectEventArgs((ListItem)item)); }
        }
        /// <summary>
        /// An item has been added.
        /// </summary>
        /// <param name="item">The added item.</param>
        protected void ItemAddedInvoke(Component item)
        {
            //Let the list know it has been changed.
            _IsDirty = true;

            //If someone has hooked up a delegate to the event, fire it.
            if (ItemAdded != null) { ItemAdded(this, new ListItemAddedEventArgs((ListItem)item)); }
        }
        /// <summary>
        /// Calculate the position of a list item, ignoring whether the items are visible or not.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The vector position.</returns>
        protected Vector2 CalculateItemPosition(int index)
        {
            //Return the calculated vector.
            return new Vector2((Position.X + _Border), ((Position.Y + _Border) + (index * _ItemHeight)));
        }
        /// <summary>
        /// Update the scroller to match the list.
        /// </summary>
        protected void UpdateScroller()
        {
            //Update the scroller's position and length.
            _Scroller.Position = CalculateScrollerPosition();
            _Scroller.Length = Height - 10;
        }
        /// <summary>
        /// Calculate the position of the scroller item.
        /// </summary>
        /// <returns>The vector position.</returns>
        protected Vector2 CalculateScrollerPosition()
        {
            //Return the calculated vector.
            return new Vector2((Position.X + (Width - 15)), (Position.Y + _Border));
        }
        /// <summary>
        /// Calculate the width of a list item.
        /// </summary>
        /// <returns>The width.</returns>
        protected float CalculateItemWidth()
        {
            //Return the calculated width.
            if (_IsScrollable) { return (Width - (2 * _Border) - _Scroller.Width); }
            else { return (Width - (2 * _Border)); }
        }
        /// <summary>
        /// Calculate the unfixed height of the list.
        /// </summary>
        /// <returns>The height.</returns>
        protected float CalculateListHeight()
        {
            //Return the calculated width.
            return ((_Items.Count * _ItemHeight) + (2 * _Border));
        }
        /// <summary>
        /// Clear the list.
        /// </summary>
        public void Clear()
        {
            //Clear the list.
            if (_Items != null) { _Items.Clear(); }
        }
        /// <summary>
        /// Change the list's scrolling status.
        /// </summary>
        /// <param name="isScrollable">Whether the list can be scrolled or not.</param>
        protected void ChangeScrollingStatus(bool isScrollable)
        {
            //Set the scrolling state.
            _IsScrollable = isScrollable;
            //Update the scroller accordingly.
            if (_IsScrollable) { _Scroller.IsActive = true; _Scroller.IsVisible = true; }
            else { _Scroller.IsActive = false; _Scroller.IsVisible = false; }
            //Recalculate the width of the list items.
            foreach (ListItem item in _Items) { item.Width = CalculateItemWidth(); }
            //Update the list.
            UpdateList();
        }
        /// <summary>
        /// Change the list's capacity status.
        /// </summary>
        /// <param name="isFixed">Whether the list's capacity should be fixed or not.</param>
        protected void ChangeCapacityStatus(bool isFixed)
        {
            //Set the capacity state and update the list.
            _IsFixed = isFixed;
            UpdateList();
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Invoke the base event method.
            if (_IsFixed) { base.BoundsChangeInvoke(width, height); }
            else { base.BoundsChangeInvoke(width, CalculateListHeight()); }

            //Update the list item's width.
            foreach (ListItem item in _Items) { item.Width = CalculateItemWidth(); }
            //Update the list's background texture.
            UpdateBackgroundTexture();
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
        /// The nodes that are stored in this treeview.
        /// </summary>
        public List<Component> Items
        {
            get { return _Items; }
            set { _Items = value; }
        }
        /// <summary>
        /// The font that is used by this textbox.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        /// <summary>
        /// Whether the list has been outfitted with a scroller or not.
        /// </summary>
        public bool IsScrollable
        {
            get { return _IsScrollable; }
            set { ChangeScrollingStatus(value); }
        }
        /// <summary>
        /// Whether the list's capacity is fixed or not.
        /// </summary>
        public bool IsFixed
        {
            get { return _IsFixed; }
            set { ChangeCapacityStatus(value); }
        }
        /// <summary>
        /// The border of the list.
        /// </summary>
        public float Border
        {
            get { return _Border; }
            set { _Border = value; }
        }
        #endregion
    }
}
