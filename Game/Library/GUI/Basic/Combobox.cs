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
    /// Whether the combobox is open or closed.
    /// </summary>
    public enum ComboboxState
    {
        Closed,
        Open
    }

    /// <summary>
    /// A combobox supplies the user with a drop down list in which he can choose from several items.
    /// </summary>
    public class Combobox : Component
    {
        #region Fields
        private Label _Label;
        private Button _Button;
        private List _List;
        private float _Ratio;
        private ListItem _SelectedItem;
        private ComboboxState _State;

        public delegate void ItemSelectHandler(object obj, ItemSelectEventArgs e);
        public event ItemSelectHandler ItemSelect;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a combobox.
        /// </summary>
        /// <param name="gui">The GUI that this combobox will be a part of.</param>
        /// <param name="position">The position of this combobox.</param>
        /// <param name="height">The height of this combobox.</param>
        /// <param name="width">The width of this combobox.</param>
        public Combobox(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the combobox.
        /// </summary>
        /// <param name="gui">The GUI that this combobox will be a part of.</param>
        /// <param name="position">The position of this combobox.</param>
        /// <param name="height">The height of this combobox.</param>
        /// <param name="width">The width of this combobox.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Ratio = .1f;
            _Label = new Label(GUI, Position, (Width * (1 - _Ratio)), Height);
            _Button = new Button(GUI, new Vector2((Position.X + (Width * (1 - _Ratio))), Position.Y), (Width * _Ratio), Height);
            _List = new List(GUI, new Vector2(Position.X, (Position.Y + (_Label.Height + 2))), Width, 118);
            _List.IsActive = false;
            _SelectedItem = null;
            _State = ComboboxState.Closed;
            _Label.Text = "";

            //Add the items.
            Add(_Label);
            Add(_Button);
            Add(_List);

            //Hook up some events.
            _List.ItemSelect += OnItemSelect;
            _Button.MouseClick += OnButtonClick;
        }
        /// <summary>
        /// Load the content of this combobox.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the combobox.
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
        /// Draw the combobox.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }
        /// <summary>
        /// Update the label's and button's position and bounds when the bounds or position of this item changes.
        /// </summary>
        private void UpdateComponents()
        {
            //If the item has been recently modified update the label's and button's position, width and height.
            _Label.Position = Position;
            _Label.Width = (Width * (1 - _Ratio));
            _Label.Height = Height;
            _Button.Position = new Vector2((Position.X + (Width * (1 - _Ratio))), Position.Y);
            _Button.Width = (Width * _Ratio);
            _Button.Height = Height;
            _List.Position = new Vector2(Position.X, (Position.Y + (_Label.Height + 2)));
            _List.Width = Width;
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
            if (!(_HasFocus == hasFocus) && !HasFocus && (_State == ComboboxState.Open)) { SwitchState(); }
        }
        /// <summary>
        /// Tell the world that an item has been selected.
        /// </summary>
        /// <param name="item">The selected component.</param>
        protected void ItemSelectInvoke(Component item)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (ItemSelect != null) { ItemSelect(this, new ItemSelectEventArgs(item)); }
        }
        /// <summary>
        /// A child component has had its focus change.
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
                if (!((List)e.Item).HasFocus && (_State == ComboboxState.Open)) { SwitchState(); }
                //If the list has gained focus but isn't open, turn it on.
                else if (((List)e.Item).HasFocus && (_State == ComboboxState.Closed)) { SwitchState(); }
            }
            catch { }
        }
        /// <summary>
        /// The button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnButtonClick(object obj, MouseClickEventArgs e)
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
            //Change the displayed tag and text.
            _Label.Tag = (_SelectedItem as LabelListItem).Label.Tag;
            _Label.Text = (_SelectedItem as LabelListItem).Label.Text;

            //Fire the item select event.
            ItemSelectInvoke(e.Item);
        }
        /// <summary>
        /// Switch this combobox's state.
        /// </summary>
        private void SwitchState()
        {
            //Switch state.
            if (_State == ComboboxState.Open)
            {
                //Close the list.
                _State = ComboboxState.Closed;
                _List.IsActive = false;
            }
            else
            {
                //Open the list.
                _State = ComboboxState.Open;
                _List.IsActive = true;
            }
        }
        /// <summary>
        /// Clear the combobox of items.
        /// </summary>
        public void Clear()
        {
            //Clear the list.
            if (_List != null) { _List.Clear(); }
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
        /// The button that this checkbox uses.
        /// </summary>
        public Button Button
        {
            get { return _Button; }
            set { _Button = value; }
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
        /// The ratio between the label's width and the checkbox's width.
        /// </summary>
        public float Ratio
        {
            get { return _Ratio; }
            set { _Ratio = value; }
        }
        /// <summary>
        /// Whether the list is down or not.
        /// </summary>
        public ComboboxState State
        {
            get { return _State; }
            set { _State = value; }
        }
        #endregion
    }
}
