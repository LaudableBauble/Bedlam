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

using Library;
using Library.Animate;
using Library.Core;
using Library.GUI;
using Library.GUI.Basic;
using Library.Imagery;
using Library.Infrastructure;
using Library.Tools;

namespace Library.Core
{
    /// <summary>
    /// A layer is a part of a level instance and carries information about what items are displayed where.
    /// </summary>
    public class Layer
    {
        #region Fields
        private Level _Level;
        private string _Name;
        private RobustList<Item> _Items;
        private bool _IsVisible;
        private Vector2 _ScrollSpeed;
        private Matrix _CameraMatrix;

        public delegate void ItemChangedHandler(object obj, EventArgs e);
        public event ItemChangedHandler ItemChanged;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item instance.</returns>
        public Item this[int index]
        {
            get { return (_Items[index]); }
            set { _Items[index] = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a layer.
        /// </summary>
        /// <param name="level">The level in which this layer resides.</param>
        /// <param name="name">The name of the layer.</param>
        /// <param name="scrollSpeed">The scrolling speed of this layer. Used for parallex scrolling.</param>
        public Layer(Level level, string name, Vector2 scrollSpeed)
        {
            //Initialize the editor.
            Initialize(level, name, scrollSpeed);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the layer.
        /// </summary>
        /// <param name="level">The level in which this layer resides.</param>
        /// <param name="name">The name of this layer.</param>
        /// <param name="scrollSpeed">The scrolling speed of this layer. Used for parallex scrolling.</param>
        public void Initialize(Level level, string name, Vector2 scrollSpeed)
        {
            //Initialize some variables.
            _Level = level;
            _Name = name;
            _Items = new RobustList<Item>();
            _IsVisible = true;
            _ScrollSpeed = scrollSpeed;
            _CameraMatrix = Matrix.Identity;

            //Manage all items, ie. add and remove them from the layer.
            ManageItems();
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="contentManager">The manager that handles all graphical content.</param>
        public void LoadContent(ContentManager contentManager)
        {
            //Manage all items, ie. add and remove them from the layer.
            ManageItems();

            //Load all layer's content.
            foreach (Item item in _Items) { item.LoadContent(contentManager); }
        }
        /// <summary>
        /// Update the layer.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            //Manage all items, ie. add and remove them from the layer.
            ManageItems();

            //Update the layers.
            foreach (Item item in _Items) { item.Update(gameTime); }

            //Update the layer's camera matrix.
            _CameraMatrix = Helper.TransformCameraMatrix((_Level.Camera.Position * _ScrollSpeed), _Level.Camera.Rotation, _Level.Camera.ZoomValue, _Level.Camera.Origin);
        }
        /// <summary>
        /// Draw the layer and its items.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Begin the drawing.
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _CameraMatrix);

            //Draw each layer.
            foreach (Item item in _Items) { item.Draw(spriteBatch); }

            //End the drawing.
            spriteBatch.End();
        }

        /// <summary>
        /// Add an item to the layer.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public Item AddItem(Item item)
        {
            //Add the item.
            _Items.Add(item);
            //Return the item.
            return item;
        }
        /// <summary>
        /// Remove an item from the layer.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(Item item)
        {
            _Items.Remove(item);
        }
        /// <summary>
        /// Add and remove items to and from the layer.
        /// </summary>
        public void ManageItems()
        {
            _Items.Update();
        }
        /// <summary>
        /// Get the index of an item.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <returns>The index of the item.</returns>
        public int GetItemIndex(Item item)
        {
            //Return the item's index.
            return (_Items.IndexOf(item));
        }
        /// <summary>
        /// Get the last item in the list, even if it has not technically been added yet.
        /// </summary>
        public Item GetLastItem()
        {
            return _Items.GetLastItem();
        }
        /// <summary>
        /// Get the item closest to the given position.
        /// </summary>
        /// <param name="position">The given position.</param>
        /// <returns>The closest item.</returns>
        public Item GetItemAtPosition(Vector2 position)
        {
            //If the layer is visible.
            if (!_IsVisible) { return null; }
            //If there's any items at all.
            if (_Items.Count == 0) { return null; }

            //The item to return.
            Item item = null;

            //Go through each item in the list and find the item closest to the given position.
            foreach (Item i in _Items) { if (i.IsVisible && Helper.IsPointWithinBox(position, Helper.GetBoundingBox(i))) { item = i; } }

            //Return the item.
            return item;
        }
        /// <summary>
        /// Change the visibility state of this layer and each item under it.
        /// </summary>
        /// <param name="isVisible">Whether the layer will be visible or not.</param>
        public void ChangeVisibilityState(bool isVisible)
        {
            //Make the layer visible or not.
            _IsVisible = isVisible;
            //Go through each item and either make them visible or invisible.
            foreach (Item item in _Items) { item.IsVisible = isVisible; }
        }
        /// <summary>
        /// Get the world coordinates of a position given the layer's scrolling speed.
        /// </summary>
        /// <param name="position">The local position.</param>
        /// <returns>The transformed world position.</returns>
        public Vector2 GetWorldPosition(Vector2 position)
        {
            return Vector2.Transform(position, Matrix.Invert(_CameraMatrix));
        }
        /// <summary>
        /// The items of this layer has seen some change in their numbers.
        /// </summary>
        private void ItemChangedInvoke()
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (ItemChanged != null) { ItemChanged(this, new EventArgs()); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The level that this layer belongs to.
        /// </summary>
        public Level Level
        {
            get { return _Level; }
            set { _Level = value; }
        }
        /// <summary>
        /// The layers that make up the level.
        /// </summary>
        public List<Item> Items
        {
            get { return _Items.ToList(); }
        }
        /// <summary>
        /// Whether the level is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { ChangeVisibilityState(value); }
        }
        /// <summary>
        /// The scroll speed is relative to the main camera and the X and Y components are 
        /// interpreted as factors, so (1;1) means the same scrolling speed as the main camera.
        /// Enables parallax scrolling.
        /// </summary>
        public Vector2 ScrollSpeed
        {
            get { return _ScrollSpeed; }
            set { _ScrollSpeed = value; }
        }
        /// <summary>
        /// The name of the layer.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// The layer's camera matrix.
        /// </summary>
        public Matrix CameraMatrix
        {
            get { return _CameraMatrix; }
            set { _CameraMatrix = value; }
        }
        #endregion
    }
}
