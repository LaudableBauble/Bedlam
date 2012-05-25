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

using Library.Enums;
using Library.Factories;
using Library.Imagery;
using Library.Infrastructure;

namespace Library.GUI.Basic
{
    /// <summary>
    /// Components are the basic building blocks of a GUI and are thus derived from a plethora of plenty.
    /// </summary>
    public abstract class Component
    {
        #region Fields
        protected GraphicalUserInterface _GUI;
        protected SpriteCollection _Sprite;
        protected Vector2 _Position;
        protected float _Width;
        protected float _Height;
        protected bool _IsActive;
        protected bool _IsVisible;
        protected bool _HasFocus;
        protected bool _IsMouseHovering;
        protected float _Transparence;
        protected int _DrawOrder;
        protected CellStyle _CellStyle;
        protected List<Component> _Items;
        protected Component _Parent;
        protected bool _UpdateDrawOrders;

        public delegate void BoundsChangeHandler(object obj, BoundsChangedEventArgs e);
        public delegate void MouseClickHandler(object obj, MouseClickEventArgs e);
        public delegate void MouseDownHandler(object obj, MouseClickEventArgs e);
        public delegate void MouseHoverHandler(object obj, EventArgs e);
        public delegate void PositionChangeHandler(object obj, EventArgs e);
        public delegate void ItemTypeChangeHandler(object obj, EventArgs e);
        public delegate void DisposeHandler(object obj, DisposeEventArgs e);
        public delegate void FocusChangeHandler(object obj, FocusChangeEventArgs e);
        public delegate void DrawOrderChangeHandler(object obj, EventArgs e);
        public event DrawOrderChangeHandler DrawOrderChange;
        public event FocusChangeHandler FocusChange;
        public event DisposeHandler Dispose;
        public event BoundsChangeHandler BoundsChange;
        public event MouseClickHandler MouseClick;
        public event MouseDownHandler MouseDown;
        public event MouseHoverHandler MouseHover;
        public event PositionChangeHandler PositionChange;
        public event ItemTypeChangeHandler VocalTypeChange;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the component.
        /// </summary>
        /// <param name="gui">The GUI that this component will be a part of.</param>
        /// <param name="position">The position of the component.</param>
        /// <param name="width">The width of this component.</param>
        /// <param name="height">The height of this component.</param>
        protected virtual void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            _GUI = gui;
            _Position = position;
            _Sprite = new SpriteCollection();
            _Width = width;
            _Height = height;
            _IsActive = true;
            _IsVisible = true;
            _HasFocus = false;
            _IsMouseHovering = false;
            _Transparence = .5f;
            _DrawOrder = 0;
            _CellStyle = CellStyle.Dynamic;
            _Items = new List<Component>();
            _Parent = null;

            //Subscribe to events.
            _GUI.FocusNotification += OnFocusNotification;
        }
        /// <summary>
        /// Load the content of this item.
        /// </summary>
        public virtual void LoadContent()
        {
            //Load the sprite's content.
            _Sprite.LoadContent(_GUI.ContentManager);

            //Loop through all items and load their content.
            foreach (Component item in _Items) { item.LoadContent(); }
        }
        /// <summary>
        /// Update the item.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public virtual void Update(GameTime gametime)
        {
            //If the component isn't active or visible, stop here.
            if (!_IsActive || !_IsVisible) { return; }

            //If to update the draw orders.
            if (_UpdateDrawOrders)
            {
                //Sort the list of items by draw order.
                SortItems();
                //Update all draw orders.
                UpdateDrawOrders();
                //Turn off the flag.
                _UpdateDrawOrders = false;
            }

            //Update the sprite.
            _Sprite.Update(gametime);

            //Loop through all items and update them.
            foreach (Component item in _Items) { item.Update(gametime); }
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public virtual void HandleInput(InputState input)
        {
            //If the component is active, write the input to the box.
            if (_IsActive)
            {
                //If the component is visible.
                if (_IsVisible)
                {
                    //If the left mouse button has been pressed.
                    if (input.IsNewLeftMouseClick())
                    {
                        //If the user clicks somewhere on the item, fire the event.
                        if (Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Position, Width, Height))
                        {
                            //Fire the event.
                            MouseClickInvoke(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), MouseButton.Left);
                        }
                        //If not, see if it is appropriate to defocus the component.
                        else { FocusChangeInvoke(false); }
                    }

                    //If the right mouse button has been pressed.
                    if (input.IsNewRightMouseClick())
                    {
                        //If the user clicks somewhere on the item, fire the event.
                        if (Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Position, Width, Height))
                        {
                            //Fire the event.
                            MouseClickInvoke(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), MouseButton.Right);
                        }
                        //If not, see if it is appropriate to defocus the component.
                        else { FocusChangeInvoke(false); }
                    }

                    //If the left mouse button is being held down.
                    if (input.IsNewLeftMousePress())
                    {
                        //If the user clicks somewhere on the item, fire the event.
                        if (Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Position, Width, Height))
                        {
                            //Fire the event.
                            MouseDownInvoke(Helper.GetMousePosition(), MouseButton.Left);
                        }
                    }

                    //If the mouse is currently hovering over the component.
                    if (Helper.IsPointWithinBox(Helper.GetMousePosition(), Position, Width, Height))
                    {
                        //If the mouse has just entered the component's surface, fire the event and enable the flag.
                        if (!_IsMouseHovering) { MouseHoverInvoke(); }
                    }
                    //Else, disable the flag.
                    else { _IsMouseHovering = false; }

                    //Loop through all items and give them access to user input.
                    foreach (Component item in _Items)
                    {
                        //Enable the item to handle input.
                        item.HandleInput(input);
                    }
                }
            }
        }
        /// <summary>
        /// Draw the item.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //If the component isn't active or visible, stop here.
            if (!_IsActive || !_IsVisible) { return; }

            //Draw the sprite.
            _Sprite.Draw(spriteBatch);

            //Draw all items.
            foreach (Component item in _Items) { item.Draw(spriteBatch); }
        }

        /// <summary>
        /// Add a component.
        /// </summary>
        /// <param name="item">The component to add.</param>
        protected virtual Component Add(Component item)
        {
            //Add the component to the list of components.
            _Items.Add(item);
            item.Parent = this;
            item.DrawOrder = _Items.Count;

            //Subscribe to some events.
            item.BoundsChange += OnItemBoundsChange;
            item.FocusChange += OnItemFocusChange;
            item.DrawOrderChange += OnDrawOrderChange;

            //Return the component.
            return item;
        }
        /// <summary>
        /// Add a sprite to the item.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        public Sprite AddSprite(string name)
        {
            //Add a sprite.
            return Factory.Instance.AddSprite(_Sprite, name, _Position, 0, 1, 0, 0, 0, "Sprite" + _Sprite.SpriteCount);
        }
        /// <summary>
        /// Add a sprite to the item.
        /// </summary>
        /// <param name="name">The name of the asset to load.</param>
        /// <param name="position">The position of the sprite.</param>
        public Sprite AddSprite(string name, Vector2 position)
        {
            //Add a sprite.
            return Factory.Instance.AddSprite(_Sprite, name, position, 0, 1, 0, 0, 0, "Sprite" + _Sprite.SpriteCount);
        }
        /// <summary>
        /// Add a sprite to the item.
        /// </summary>
        /// <param name="texture">The texture of the asset to use.</param>
        /// <param name="position">The position of the sprite.</param>
        public Sprite AddSprite(Texture2D texture, Vector2 position)
        {
            //Add a sprite.
            return Factory.Instance.AddSprite(_Sprite, texture.Name, texture, position, 0, 1, 0, 0, 0, "Sprite" + _Sprite.SpriteCount);
        }
        /// <summary>
        /// Add a sprite to the item.
        /// </summary>
        /// <param name="texture">The texture of the asset to use.</param>
        public Sprite AddSprite(Texture2D texture)
        {
            //Add a sprite.
            return Factory.Instance.AddSprite(_Sprite, texture.Name, texture, _Position, 0, 1, 0, 0, 0, "Sprite" + _Sprite.SpriteCount);
        }
        /// <summary>
        /// See if any child items still has focus.
        /// </summary>
        /// <returns>Whether any child items has focus.</returns>
        protected bool HasChildInFocus()
        {
            //For all children.
            foreach (Component item in _Items)
            {
                //If the child is in focus, return true.
                if (item.HasFocus) { return true; }
            }

            //No child in focus, return false.
            return false;
        }
        /// <summary>
        /// Update all tier one draw orders, according to the current state of the list.
        /// </summary>
        private void UpdateDrawOrders()
        {
            //The draw order counter.
            int counter = _Items.Count;

            //Go through all items and reset their draw orders.
            foreach (Component item in _Items) { item.DrawOrder = counter--; }
        }
        /// <summary>
        /// Get the topmost parent.
        /// </summary>
        /// <returns>The component's topmost parent.</returns>
        public Component GetTopmostParent()
        {
            //The parent item.
            Component parent = this;

            //While the parent isn't null, go deeper into the rabbit hole.
            while (parent.Parent != null) { parent = parent.Parent; }

            //Return the item.
            return parent;
        }
        /// <summary>
        /// Sort the item list by draw order.
        /// </summary>
        private void SortItems()
        {
            //Sort the items list by descending draw order.
            _Items.Sort(new ComponentComparer());
        }
        /// <summary>
        /// Update the position and bounds of all components located in the component.
        /// </summary>
        protected virtual void UpdateComponents() { }
        /// <summary>
        /// Change the transparence of this component. The values range between 1 and 0, where 1 means no transparence.
        /// </summary>
        /// <param name="value">The new transparence value.</param>
        protected virtual void ChangeTransparence(float value)
        {
            //Change the transparence.
            _Transparence = Math.Max(Math.Min(value, 1), 0);

            //Change all sprites' transparence as well.
            foreach (Sprite sprite in _Sprite.Sprites) { sprite.Transparence = _Transparence; }
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected virtual void BoundsChangeInvoke(float width, float height)
        {
            //Continue only if the bounds have changed.
            if (width == _Width && height == _Height) { return; }

            //Save the variables.
            _Width = width;
            _Height = height;

            //Update the components.
            UpdateComponents();

            //If someone has hooked up a delegate to the event, fire it.
            if (BoundsChange != null) { BoundsChange(this, new BoundsChangedEventArgs(_Width, _Height)); }
        }
        /// <summary>
        /// Tell the world that the item has been clicked.
        /// </summary>
        /// <param name="position">The position of the mouse at the time of the click.</param>
        /// <param name="button">Which mouse button that has been clicked.</param>
        protected virtual void MouseClickInvoke(Vector2 position, MouseButton button)
        {
            //Ask for focus.
            _GUI.RequestFocus(this);

            //If someone has hooked up a delegate to the event, fire it.
            if (MouseClick != null) { MouseClick(this, new MouseClickEventArgs(position, button)); }
        }
        /// <summary>
        /// Tell the world that the item has been held down upon by the mouse.
        /// </summary>
        /// <param name="position">The position of the mouse.</param>
        /// <param name="button">Which mouse button that has been held down.</param>
        protected virtual void MouseDownInvoke(Vector2 position, MouseButton button)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (MouseDown != null) { MouseDown(this, new MouseClickEventArgs(position, button)); }
        }
        /// <summary>
        /// The mouse is currently hovering over the component.
        /// </summary>
        protected virtual void MouseHoverInvoke()
        {
            //Change the state of the flag.
            _IsMouseHovering = true;

            //If someone has hooked up a delegate to the event, fire it.
            if (MouseHover != null) { MouseHover(this, new EventArgs()); }
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected virtual void PositionChangeInvoke(Vector2 position)
        {
            //Continue only if the position truly has changed.
            if (_Position == position) { return; }

            //Update the variables.
            _Position = position;
            //If the item has any sprites, update their position as well.
            _Sprite.UpdatePositions(position);

            //Update the components.
            UpdateComponents();

            //If someone has hooked up a delegate to the event, fire it.
            if (PositionChange != null) { PositionChange(this, new EventArgs()); }
        }
        /// <summary>
        /// Dispose of this item.
        /// </summary>
        protected virtual void DisposeInvoke()
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (Dispose != null) { Dispose(this, new DisposeEventArgs(this)); }

            //Release some resources. Meh, do it later.
        }
        /// <summary>
        /// Change the focus of this item.
        /// </summary>
        /// <param name="hasFocus">Whether the item has been granted focus or not.</param>
        protected virtual void FocusChangeInvoke(bool hasFocus)
        {
            //If a change in focus isn't about to happen, stop here.
            if (_HasFocus == hasFocus) { return; }

            //Change the focus.
            _HasFocus = hasFocus;

            //If someone has hooked up a delegate to the event, fire it.
            if (FocusChange != null) { FocusChange(this, new FocusChangeEventArgs(this)); }
        }
        /// <summary>
        /// When a child component has changed bounds.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnItemBoundsChange(object obj, BoundsChangedEventArgs e) { }
        /// <summary>
        /// A child component has had its focus change.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        protected virtual void OnItemFocusChange(object obj, FocusChangeEventArgs e) { }
        /// <summary>
        /// If a direct child item has changed its draw order, enable draw order update next turn.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        protected virtual void OnDrawOrderChange(object obj, EventArgs e)
        {
            //There is a pressing need to update the draw orders for direct child items.
            _UpdateDrawOrders = true;
        }
        /// <summary>
        /// Listen to the GUI's focus notification.
        /// <param name="item">The item that has gained focus.</param>
        /// </summary>
        protected virtual void OnFocusNotification(Component item)
        {
            //If you got solitary rights to focus, come and claim them.
            if (item == this) { FocusChangeInvoke(true); }
            //Otherwise relinquish focus.
            else { FocusChangeInvoke(false); }
        }
        /// <summary>
        /// Change the draw order of this item.
        /// </summary>
        /// <param name="drawOrder">The draw order.</param>
        protected virtual void DrawOrderChangeInvoke(int drawOrder)
        {
            //If a change in draw order really happened.
            if (_DrawOrder == Math.Max(drawOrder, 0)) { return; }

            //Change the draw order.
            _DrawOrder = Math.Max(drawOrder, 0);

            //If a change really took place and if someone has hooked up a delegate to the event, fire it.
            if (DrawOrderChange != null) { DrawOrderChange(this, new EventArgs()); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The GUI that houses this item.
        /// </summary>
        public GraphicalUserInterface GUI
        {
            get { return _GUI; }
            set { _GUI = value; }
        }
        /// <summary>
        /// The sprite that will make up the item.
        /// </summary>
        public SpriteCollection Sprite
        {
            get { return _Sprite; }
        }
        /// <summary>
        /// The position of this item.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
            set { PositionChangeInvoke(value); }
        }
        /// <summary>
        /// The width of this item.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { BoundsChangeInvoke(value, _Height); }
        }
        /// <summary>
        /// The height of this item.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { BoundsChangeInvoke(_Width, value); }
        }
        /// <summary>
        /// If the item is active.
        /// </summary>
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }
        /// <summary>
        /// If the item is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }
        /// <summary>
        /// If the item has focus.
        /// </summary>
        public bool HasFocus
        {
            get { return _HasFocus; }
            set { FocusChangeInvoke(value); }
        }
        /// <summary>
        /// If the mouse is currently hovering over the component.
        /// </summary>
        public bool IsMouseHovering
        {
            get { return _IsMouseHovering; }
            set { _IsMouseHovering = value; }
        }
        /// <summary>
        /// The transparence of this item.
        /// </summary>
        public float Transparence
        {
            get { return _Transparence; }
            set { ChangeTransparence(value); }
        }
        /// <summary>
        /// The item's draw order, ie. how early or late the item wants to be drawn in relation to other items.
        /// This value is more of a request than an order due to that the responsibility for drawing components lies elsewhere.
        /// </summary>
        public int DrawOrder
        {
            get { return _DrawOrder; }
            set { DrawOrderChangeInvoke(value); }
        }
        /// <summary>
        /// Whether this component has a fixed position or if it adheres to a layout grid.
        /// </summary>
        public CellStyle CellStyle
        {
            get { return _CellStyle; }
            set { _CellStyle = value; }
        }
        /// <summary>
        /// This component's parent.
        /// </summary>
        public Component Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }
        #endregion
    }
}
