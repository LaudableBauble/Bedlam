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

using Library.GUI.Basic;
using Library.Animate;
using Library.Infrastructure;

namespace Library.GUI
{
    /// <summary>
    /// A GUI (Graphical User Interface) is used to display and collect data from and to the user, often in a graphically appealing way.
    /// </summary>
    public class GraphicalUserInterface
    {
        #region Fields
        private List<Component> _Items;
        private List<Component> _ItemsToBeAdded;
        private List<Component> _ItemsToBeRemoved;
        private List<Component> _FocusQueue;
        private List<Component> _ForegroundItems;
        private List<Component> _ForegroundItemsToBeAdded;
        private bool _IsActive;
        private bool _IsVisible;
        private Texture2D _FadeTexture;
        private List _RightClickList;
        private bool _HasRightClicked;
        private bool _UpdateDrawOrders;

        private ContentManager _ContentManager;
        private GraphicsDevice _GraphicsDevice;
        private SpriteBatch _SpriteBatch;

        public delegate void ItemClickedHandler(object obj, MouseClickEventArgs e);
        public delegate void FocusQueueNotification(Component item);
        public event FocusQueueNotification FocusNotification;
        public event ItemClickedHandler ItemClicked;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item instance.</returns>
        public Component this[int index]
        {
            get { return (_Items[index]); }
            set { _Items[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a GUI.
        /// </summary>
        public GraphicalUserInterface()
        {
            //Initialize some variables.
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the GUI.
        /// </summary>
        public void Initialize()
        {
            //Initialize some variables.
            _Items = new List<Component>();
            _ItemsToBeAdded = new List<Component>();
            _ItemsToBeRemoved = new List<Component>();
            _FocusQueue = new List<Component>();
            _ForegroundItems = new List<Component>();
            _ForegroundItemsToBeAdded = new List<Component>();
            _IsActive = true;
            _IsVisible = true;
            _RightClickList = new List(this, Vector2.Zero, 100, 200);
            _HasRightClicked = false;

            //Play with the right click list.
            _RightClickList.AddItem();
            _RightClickList.AddItem();
            _RightClickList.AddItem();
            (_RightClickList.Items[0] as LabelListItem).Label.Text = "Item1";
            (_RightClickList.Items[1] as LabelListItem).Label.Text = "Item2";
            (_RightClickList.Items[2] as LabelListItem).Label.Text = "Item3";
        }
        /// <summary>
        /// Load all the GUI's content.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spritebatch)
        {
            //Save the graphic devices.
            _ContentManager = contentManager;
            _GraphicsDevice = graphicsDevice;
            _SpriteBatch = spritebatch;

            //Load the background fading texture.
            _FadeTexture = _ContentManager.Load<Texture2D>("GameScreen/Textures/blank");

            //Load the right click list's content.
            _RightClickList.LoadContent();
            //Add and remove items to and from the GUI.
            ManageItems();

            //Loop through all items and load their content.
            _Items.ForEach(item => item.LoadContent());
            _ForegroundItems.ForEach(item => item.LoadContent());
        }
        /// <summary>
        /// Update the GUI and all its items.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public void Update(GameTime gametime)
        {
            //Add and remove items to and from the GUI, also manage the focus queue.
            ManageItems();

            //Update the right click list, if the time is right.
            if (_HasRightClicked) { _RightClickList.Update(gametime); }

            //Loop through all items and update them.
            _Items.ForEach(item => item.Update(gametime));
            _ForegroundItems.ForEach(item => item.Update(gametime));
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public void HandleInput(InputState input)
        {
            //If the GUI is active, continue.
            if (_IsActive)
            {
                //If the GUI is visible.
                if (_IsVisible)
                {
                    //Decide which collection of items to use.
                    if (_ForegroundItems.Count != 0) { _ForegroundItems.ForEach(item => item.HandleInput(input)); }
                    else { _Items.ForEach(item => item.HandleInput(input)); }

                    //If the right click list is enabled and the user has pressed the left mouse button.
                    if (_HasRightClicked && input.IsNewLeftMouseClick())
                    {
                        //If the user clicks somewhere else than on the list, disable it.
                        if (!Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), _RightClickList.Position, _RightClickList.Width, _RightClickList.Height))
                        {
                            //Disable the list.
                            EnableOrDisableRightClickList(false, Vector2.Zero);
                        }
                    }

                    //If the right mouse button has been pressed, enable or disable the right click list.
                    if (input.IsNewRightMouseClick())
                    {
                        //Enable or disable it.
                        EnableOrDisableRightClickList(true, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                    }

                    //Enable the right click list to handle user input, if the time is right.
                    if (_HasRightClicked) { _RightClickList.HandleInput(input); }
                }
            }
        }
        /// <summary>
        /// Draw the GUI.
        /// </summary>
        public void Draw()
        {
            //If the GUI is active, continue.
            if (_IsActive)
            {
                //If the GUI is visible.
                if (_IsVisible)
                {
                    //Loop through all items and draw them.
                    Items.ForEach(item => item.Draw(_SpriteBatch));

                    //If there exists items in the foreground, fade the background and draw them.
                    if (_ForegroundItems.Count != 0) { FadeBackBufferToBlack(150); _ForegroundItems.ForEach(item => item.Draw(_SpriteBatch)); }

                    //Draw the right click list, if the time is right.
                    if (_HasRightClicked) { _RightClickList.Draw(_SpriteBatch); }
                }
            }
        }

        /// <summary>
        /// Add an item to the GUI.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(Component item)
        {
            //Store the item in a list until it is time to add them to the GUI.
            _ItemsToBeAdded.Add(item);
        }
        /// <summary>
        /// Add a foreground item to the GUI.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddForegroundItem(Component item)
        {
            //Store the item in a list until it is time to add it to the GUI.
            _ForegroundItemsToBeAdded.Add(item);
        }
        /// <summary>
        /// Remove an item from the GUI.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(Component item)
        {
            //Remove the item from the list of items.
            _ItemsToBeRemoved.Add(item);
        }
        /// <summary>
        /// Add and remove items to and from the GUI.
        /// </summary>
        public void ManageItems()
        {
            //If there are items to add to the GUI, add them.
            if (_ItemsToBeAdded.Count != 0)
            {
                foreach (Component item in _ItemsToBeAdded)
                {
                    //Add the item to the GUI.
                    _Items.Add(item);
                    //Load its contents.
                    try { item.LoadContent(); }
                    catch { }
                    //Update the draw order.
                    item.DrawOrder = _Items.Count;
                    //Hook up some events.
                    AddItemEvents(_Items[_Items.Count - 1]);
                }

                //Clear the list.
                _ItemsToBeAdded.Clear();
            }
            //If there are foregound items to add to the GUI, add them.
            if (_ForegroundItemsToBeAdded.Count != 0)
            {
                foreach (Component item in _ForegroundItemsToBeAdded)
                {
                    //Add the item to the GUI.
                    _ForegroundItems.Add(item);
                    //Load its contents.
                    try { item.LoadContent(); }
                    catch { }
                    //Update the draw order.
                    item.DrawOrder = _ForegroundItems.Count;
                    //Hook up some events.
                    AddItemEvents(item);
                }

                //Clear the list.
                _ForegroundItemsToBeAdded.Clear();
            }
            //If there are items to remove from the GUI, remove them.
            if (_ItemsToBeRemoved.Count != 0)
            {
                foreach (Component item in _ItemsToBeRemoved)
                {
                    //Unsubscribe from the item.
                    RemoveItemEvents(item);
                    //Remove the item from the list of items.
                    _Items.Remove(item);
                    _ForegroundItems.Remove(item);
                }

                //Clear the list.
                _ItemsToBeRemoved.Clear();
            }

            //If there is any items vying for focus.
            if (_FocusQueue.Count > 0)
            {
                //Sort the list.
                SortFocusQueue();
                //Notify all interested of the focus decision.
                FocusNotificationInvoke(_FocusQueue[_FocusQueue.Count - 1]);
                //Clear the focus queue.
                _FocusQueue.Clear();
            }

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
        }
        /// <summary>
        /// Update all tier one draw orders, according to the current state of the list.
        /// </summary>
        private void UpdateDrawOrders()
        {
            //The draw order counter.
            int counter = _Items.Count;

            //Go through all items and reset their draw orders. Reset the counter inbetween.
            _Items.ForEach(item => item.DrawOrder = counter--);
            counter = _Items.Count;
            _ForegroundItems.ForEach(item => item.DrawOrder = counter--);
        }
        /// <summary>
        /// Request focus to an item in the GUI.
        /// </summary>
        /// <param name="item">The item to focus upon.</param>
        public void RequestFocus(Component item)
        {
            //Add the item to the focus queue.
            _FocusQueue.Add(item);
        }
        /// <summary>
        /// Add events from an item.
        /// </summary>
        /// <param name="item">The item to add events from.</param>
        private void AddItemEvents(Component item)
        {
            //Hook up some events from this item.
            item.DrawOrderChange += OnDrawOrderChange;
            item.MouseClick += OnItemClick;
            item.Dispose += OnItemDispose;
            item.VocalTypeChange += OnItemVocalTypeChange;
        }
        /// <summary>
        /// Remove events from an item.
        /// </summary>
        /// <param name="item">The item to remove events from.</param>
        private void RemoveItemEvents(Component item)
        {
            //Unsubscribe some events from this item.
            item.Dispose -= OnItemDispose;
            item.VocalTypeChange -= OnItemVocalTypeChange;
        }
        /// <summary>
        /// An item of this GUI has been clicked on, deal with it.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnItemClick(object obj, MouseClickEventArgs e)
        {
            //Invoke the appropriate event.
            ItemClickedInvoke(e.Position, e.Button);
        }
        /// <summary>
        /// An item of this GUI has been disposed of, deal with it.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnItemDispose(object obj, DisposeEventArgs e)
        {
            //Remove the item from the list.
            RemoveItem(e.Item);
        }
        /// <summary>
        /// An item of this GUI has changed its vocal state, deal with it.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnItemVocalTypeChange(object obj, EventArgs e)
        {
            //TODO: Obsolete.
            //Update the last loud item in the list.
            /*_LastLoudItem = GetLastLoudComponent();

            //Remove the item from the list.
            RemoveItem(_LastLoudItem);
            //Add the item again, now at the bottom of the list.
            AddItem(_LastLoudItem);*/
        }
        /// <summary>
        /// An item of this GUI has been clicked on.
        /// </summary>
        private void ItemClickedInvoke(Vector2 position, MouseButton button)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (ItemClicked != null) { ItemClicked(this, new MouseClickEventArgs(position, button)); }
        }
        /// <summary>
        /// If a direct item child has changed its draw order, enable draw order update next turn.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnDrawOrderChange(object obj, EventArgs e)
        {
            //There is a pressing need to update the draw orders for direct child items.
            _UpdateDrawOrders = true;
        }
        /// <summary>
        /// Notify all interested of which item claimed solitary right to focus this round.
        /// </summary>
        /// <param name="item">The item who gained focus.</param>
        private void FocusNotificationInvoke(Component item)
        {
            //Change the draw order of the item's family line.
            BringToFront(item);

            //If someone has hooked up a delegate to the event, fire it.
            if (FocusNotification != null) { FocusNotification(item); }
        }
        /// <summary>
        /// Bring an item's particular family line to the front by changing its draw orders.
        /// </summary>
        /// <param name="item">The item to send to front.</param>
        public void BringToFront(Component item)
        {
            //The parent item.
            Component parent = item;

            //While the parent isn't null, go deeper into the rabbit hole and set their draw orders.
            while (parent != null) { parent.DrawOrder = 0; parent = parent.Parent; }
        }
        /// <summary>
        /// Get the last component in the collection, even if the component hasn't technically been added yet.
        /// </summary>
        public Component GetLastComponent()
        {
            //Return the last component in the list.            
            if (_ItemsToBeAdded.Count == 0) { return _Items[_Items.Count - 1]; }
            else { return _ItemsToBeAdded[_ItemsToBeAdded.Count - 1]; }
        }
        /// <summary>
        /// Draws a translucent black fullscreen sprite, used for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(int alpha)
        {
            //Draw the faded backgroubd.
            SpriteBatch.Draw(_FadeTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0, 0, 0, (byte)alpha));
        }
        /// <summary>
        /// Enable or disable the right click list.
        /// </summary>
        /// <param name="enable">Whether the right click list should be enabled or disabled.</param>
        /// <param name="position">The mouse position at the time of the right click.</param>
        private void EnableOrDisableRightClickList(bool enable, Vector2 position)
        {
            //Enable or disable it.
            _HasRightClicked = enable;

            //If it has been enabled, set its position to the mouse's.
            if (_HasRightClicked) { _RightClickList.Position = position; }

        }
        /// <summary>
        /// Sort the focus queue list by draw order.
        /// </summary>
        private void SortFocusQueue()
        {
            //Sort the focus queue by descending draw order.
            _FocusQueue.Sort(new ComponentComparer());
        }
        /// <summary>
        /// Sort the item lists by draw order.
        /// </summary>
        private void SortItems()
        {
            //Sort the item lists by descending draw order.
            _Items.Sort(new ComponentComparer());
            _ForegroundItems.Sort(new ComponentComparer());
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list of items in this GUI.
        /// </summary>
        public List<Component> Items
        {
            get { return _Items; }
            set { _Items = value; }
        }
        /// <summary>
        /// The list of items to be added in this GUI.
        /// </summary>
        public List<Component> ItemsToBeAdded
        {
            get { return _ItemsToBeAdded; }
            set { _ItemsToBeAdded = value; }
        }
        /// <summary>
        /// The list of items to be removed in this GUI.
        /// </summary>
        public List<Component> ItemsToBeRemoved
        {
            get { return _ItemsToBeRemoved; }
            set { _ItemsToBeRemoved = value; }
        }
        /// <summary>
        /// The last item in the list.
        /// </summary>
        public Component LastItem
        {
            get
            {
                if (_ItemsToBeAdded.Count == 0) { return _Items[_Items.Count - 1]; }
                else { return _ItemsToBeAdded[_ItemsToBeAdded.Count - 1]; }
            }
        }
        /// <summary>
        /// If the GUI is active.
        /// </summary>
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }
        /// <summary>
        /// If the GUI is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }
        /// <summary>
        /// The content manager of this GUI.
        /// </summary>
        public ContentManager ContentManager
        {
            get { return _ContentManager; }
            set { _ContentManager = value; }
        }
        /// <summary>
        /// The graphics device of this GUI.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return _GraphicsDevice; }
            set { _GraphicsDevice = value; }
        }
        /// <summary>
        /// The sprite batch of this GUI.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return _SpriteBatch; }
            set { _SpriteBatch = value; }
        }
        #endregion
    }
}
