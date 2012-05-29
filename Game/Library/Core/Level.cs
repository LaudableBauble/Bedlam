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

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.DrawingSystem;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Library;
using Library.Animate;
using Library.Core;
using Library.Enums;
using Library.GUI;
using Library.GUI.Basic;
using Library.Imagery;
using Library.Infrastructure;
using Library.Tools;

namespace Library.Core
{
    /// <summary>
    /// A level holds all the necessary information of a segmented piece of the game world and can be launched by the engine at the appropriate
    /// time.
    /// </summary>
    public class Level
    {
        #region Fields
        private string _Name;
        private RobustList<Layer> _Layers;
        private bool _IsVisible;
        private Camera2D _Camera;
        private GraphicsDevice _GraphicsDevice;
        private ContentManager _ContentManager;
        private SpriteBatch _SpriteBatch;
        private World _World;
        private LevelState _State;

        public delegate void LayerChangedHandler(object obj, EventArgs e);
        public event LayerChangedHandler LayerChanged;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set a layer.
        /// </summary>
        /// <param name="index">The index of the layer.</param>
        /// <returns>The layer instance.</returns>
        public Layer this[int index]
        {
            get { return (_Layers[index]); }
            set { _Layers[index] = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a level.
        /// </summary>
        /// <param name="name">The name of the level.</param>
        /// <param name="camera">The camera to use.</param>
        public Level(string name, Camera2D camera)
        {
            //Initialize the editor.
            Initialize(name, camera);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the level.
        /// </summary>
        /// <param name="name">The name of the level.</param>
        /// <param name="camera">The camera that will display the level.</param>
        public void Initialize(string name, Camera2D camera)
        {
            //Initialize some variables.
            _Name = name;
            _Layers = new RobustList<Layer>();
            _IsVisible = true;
            _Camera = camera;
            _GraphicsDevice = null;
            _ContentManager = null;
            _SpriteBatch = null;
            _World = new World(new Vector2(0, 10));
            _State = LevelState.Play;
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to use.</param>
        /// <param name="contentManager">The manager that handles all graphical content.</param>
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            //Save the content manager for future use.
            _GraphicsDevice = graphicsDevice;
            _ContentManager = contentManager;

            //Load all layer's content.
            foreach (Layer layer in _Layers) { layer.LoadContent(contentManager); }
        }
        /// <summary>
        /// Update the level.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            //Manage layers.
            ManageLayers();

            //Update the world simulator. Explanation: The update frequency will never fall below 30Hz.
            if (_State == LevelState.Play) { _World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * .001f, (1f / 30f))); }

            //Update the layers.
            foreach (Layer layer in _Layers) { layer.Update(gameTime); }

            //Update the camera.
            _Camera.Update(gameTime);
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public void HandleInput(InputState input)
        {

        }
        /// <summary>
        /// Draw the level and its layers.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw each layer.
            foreach (Layer layer in _Layers) { layer.Draw(spriteBatch); }
        }

        /// <summary>
        /// Add a layer to the level.
        /// </summary>
        /// <param name="layer">The layer to add.</param>
        public Layer AddLayer(Layer layer)
        {
            //Add the layer and load its content..
            _Layers.Add(layer);
            if (_ContentManager != null) { layer.LoadContent(_ContentManager); }
            LayerChangedInvoke();

            //Hook up to some events.
            GetLastLayer().ItemChanged += OnItemChanged;
            //Return the layer.
            return GetLastLayer();
        }
        /// <summary>
        /// Add a layer to the level.
        /// </summary>
        /// <param name="name">The name of the layer.</param>
        /// <param name="scrollSpeed">The scrolling speed of this layer. Used for parallex scrolling.</param>
        public Layer AddLayer(string name, Vector2 scrollSpeed)
        {
            //Add the layer.
            AddLayer(new Layer(this, name, scrollSpeed));
            //Return the layer.
            return GetLastLayer();
        }
        /// <summary>
        /// Remove a layer from the level.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        public void RemoveLayer(Layer layer)
        {
            _Layers.Remove(layer);
            LayerChangedInvoke();
        }
        /// <summary>
        /// Get the index of an item in a layer.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <returns>The index of the item.</returns>
        public int GetItemIndex(Item item)
        {
            //Return the index of the item.
            return (_Layers[GetLayerIndex(item)].GetItemIndex(item));
        }
        /// <summary>
        /// Get the layer in which an item resides.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <returns>The index of the layer.</returns>
        public int GetLayerIndex(Item item)
        {
            //Return the index.
            for (int l = 0; l < _Layers.Count; l++)
            {
                //If the item exists in this layer, return its index.
                if (_Layers[l].Items.Exists(i => (i.Equals(item)))) { return l; }
            }

            //Return -1 if the item has not been found.
            return -1;
        }
        /// <summary>
        /// Get the index of a layer.
        /// </summary>
        /// <param name="layer">The layer in question.</param>
        /// <returns>The index of the layer.</returns>
        public int GetLayerIndex(Layer layer)
        {
            //Return the index of the layer.
            return (_Layers.IndexOf(layer));
        }
        /// <summary>
        /// Get the last layer in the list.
        /// </summary>
        public Layer GetLastLayer()
        {
            //Return the last layer.
            return _Layers.GetLastItem();
        }
        /// <summary>
        /// Get the item closest to the given position.
        /// </summary>
        /// <param name="position">The given position.</param>
        /// <returns>The closest item.</returns>
        public Item GetItemAtPosition(Vector2 position)
        {
            //The closest item.
            Item item = null;

            //Go through each layer and find the item closest to the given position.
            foreach (Layer layer in _Layers)
            {
                //This layer's closest item.
                Item match = layer.GetItemAtPosition(layer.GetWorldPosition(position));
                //If an item has been found to be a match, return it.
                if (match != null) { item = match; }
            }

            //Return the item.
            return item;
        }
        /// <summary>
        /// Manage the level's list of layers, ie. add, remove and sort them.
        /// </summary>
        public void ManageLayers()
        {
            //Update the list of layers and sort them in necessary.
            if (_Layers.Update()) { SortLayers(); }
        }
        /// <summary>
        /// Sort layers after descending scroll speed.
        /// </summary>
        public void SortLayers()
        {
            //Sort the list of layers.
            _Layers.Sort(delegate(Layer a, Layer b)
            {
                return (int)(a.ScrollSpeed - b.ScrollSpeed).Length();
            });
        }
        /// <summary>
        /// Change the visibility state of this level and each layer and item under it.
        /// </summary>
        /// <param name="isVisible">Whether the level will be visible or not.</param>
        public void ChangeVisibilityState(bool isVisible)
        {
            //Make the level visible or not.
            _IsVisible = isVisible;
            //Go through each layer and either make them visible or invisible.
            foreach (Layer layer in _Layers) { layer.IsVisible = isVisible; }
        }
        /// <summary>
        /// The items of a layer has changed.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnItemChanged(object obj, EventArgs e)
        {
            //Invoke the layer change event.
            LayerChangedInvoke();
        }
        /// <summary>
        /// The layers of this level has seen some change in their numbers.
        /// </summary>
        private void LayerChangedInvoke()
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (LayerChanged != null) { LayerChanged(this, new EventArgs()); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The layers that make up the level.
        /// </summary>
        public List<Layer> Layers
        {
            get { return _Layers.Items; }
        }
        /// <summary>
        /// The name of the level.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
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
        /// The camera that will display the level.
        /// </summary>
        public Camera2D Camera
        {
            get { return _Camera; }
            set { _Camera = value; }
        }
        /// <summary>
        /// The level's graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return _GraphicsDevice; }
            set { _GraphicsDevice = value; }
        }
        /// <summary>
        /// The content manager of the level.
        /// </summary>
        public ContentManager ContentManager
        {
            get { return _ContentManager; }
            set { _ContentManager = value; }
        }
        /// <summary>
        /// The state of the level.
        /// </summary>
        public LevelState State
        {
            get { return _State; }
            set { _State = value; }
        }
        /// <summary>
        /// The level's world simulator.
        /// </summary>
        public World World
        {
            get { return _World; }
            set { _World = value; }
        }
        #endregion
    }
}
