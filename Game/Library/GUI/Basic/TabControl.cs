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
    /// A tab control is a container that allows you to display multiple dialogs on a single form by switching between its tabs.
    /// </summary>
    public class TabControl : Component
    {
        #region Fields
        private List<TabPage> _Tabs;
        private TabPage _SelectedTab;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a tab control.
        /// </summary>
        /// <param name="gui">The GUI that this tab control will be a part of.</param>
        /// <param name="position">The position of this tab control.</param>
        /// <param name="height">The height of this tab control.</param>
        /// <param name="width">The width of this tab control.</param>
        public TabControl(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the tab control.
        /// </summary>
        /// <param name="gui">The GUI that this tab control will be a part of.</param>
        /// <param name="position">The position of this tab control.</param>
        /// <param name="height">The height of this tab control.</param>
        /// <param name="width">The width of this tab control.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Tabs = new List<TabPage>();
            _SelectedTab = null;
        }
        /// <summary>
        /// Load the content of this combobox.
        /// </summary>
        public override void LoadContent()
        {
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
        /// Add a tab page to the tab control.
        /// </summary>
        /// <param name="item">The items to be displayed on the new tab page.</param>
        public void AddTab(Component item)
        {
            //Create the new tabpage.
            TabPage tab = new TabPage(_GUI, _Position + new Vector2(_Tabs.Count * 55, 0), 50, 25);

            //Add the item to the tabpage.
            tab.AddItem(item);
            //Add the tabpage to this tab control.
            _Tabs.Add(tab);
            Add(tab);

            //Change the selected tab to this.
            TabSelectInvoke(tab);

            //Perform the additional event subscribing here.
            tab.MouseClick += OnTabClick;
        }
        /// <summary>
        /// Update the state of the tab control.
        /// </summary>
        private void UpdateState()
        {
            //Make sure that only the selected tab is visible.
            _Items.FindAll(a => (a.GetType() == typeof(TabPage) && a != _SelectedTab)).ForEach(item => (item as TabPage).HidingChangeInvoke(true));
            _SelectedTab.HidingChangeInvoke(false);
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
            //UpdateComponents();
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
            //UpdateComponents();
        }
        /// <summary>
        /// Change the selected tab.
        /// </summary>
        /// <param name="item">The selected tab component.</param>
        protected void TabSelectInvoke(TabPage item)
        {
            //See if the specified item really is a tab and if isn't already selected.
            if (!_Items.Contains(item) || (item == _SelectedTab)) { return; }

            //Select the tab.
            _SelectedTab = item;
            //Bring the tab to front.
            _SelectedTab.DrawOrder = 0;
            //Update the state of the tab control.
            UpdateState();

            //Ask for focus.
            _GUI.RequestFocus(item);

            //If someone has hooked up a delegate to the event, fire it.
            //if (TabSelect != null) { TabSelect(this, new ItemSelectEventArgs(item)); }
        }
        /// <summary>
        /// A tab has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnTabClick(object obj, MouseClickEventArgs e)
        {
            //Select the tab's item.
            TabSelectInvoke((TabPage)obj);
        }
        #endregion

        #region Properties
        #endregion
    }
}
