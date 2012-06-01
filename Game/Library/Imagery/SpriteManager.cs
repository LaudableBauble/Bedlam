using System;
using System.Linq;
using System.Collections.Generic;
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

namespace Library.Imagery
{
    /// <summary>
    /// A sprite manager is used both as a container of sprites and a tool with which to handle them.
    /// </summary>
    public class SpriteManager
    {
        #region Fields
        private ContentManager _ContentManager;
        private List<Sprite> _SpritesInner;
        private List<Sprite> _SpritesOuter;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <returns>The sprite instance.</returns>
        public Sprite this[int index]
        {
            get { return GetSprite(index); }
            set { SetSprite(index, value); }
        }
        /// <summary>
        /// Get or set a sprite.
        /// </summary>
        /// <param name="tag">The tag of the sprite.</param>
        /// <returns>The sprite instance.</returns>
        public Sprite this[string tag]
        {
            get { return GetSprite(tag); }
            set { SetSprite(tag, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a sprite manager.
        /// </summary>
        public SpriteManager()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the manager.
        /// </summary>
        private void Initialize()
        {
            //Initialize variables. The inner list is not iterated upon but serves as a container, whereas the outer list mimick the inner and is iterated upon.
            _SpritesInner = new List<Sprite>();
            _SpritesOuter = new List<Sprite>();
        }
        /// <summary>
        /// Load the texture for all the sprites using the Content Pipeline.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager contentManager)
        {
            //Save the content manager for future usage.
            _ContentManager = contentManager;

            //Put all the sprites to update and draw in a seperate list.
            _SpritesOuter = new List<Sprite>(_SpritesInner);

            //If there's any sprites in the list, load their content.
            _SpritesOuter.ForEach(item => item.LoadContent());
        }
        /// <summary>
        /// Update all sprites.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        public void Update(GameTime gameTime)
        {
            //Put all the sprites to update and draw in a seperate list.
            ManageSprites();

            //Update all sprites.
            _SpritesOuter.ForEach(item => item.Update(gameTime));
        }
        /// <summary>
        /// Update all sprites.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        public void Update(GameTime gameTime, Vector2 position, float rotation)
        {
            //Put all the sprites to update and draw in a seperate list.
            ManageSprites();

            //Update all sprites's position.
            _SpritesOuter.ForEach(item => item.UpdateSprite(position, rotation));

            //Update the sprites.
            Update(gameTime);
        }
        /// <summary>
        /// Draw all sprites to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw all sprites.
            _SpritesOuter.ForEach(item => item.Draw(spriteBatch));
        }

        /// <summary>
        /// Add a sprite.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        public Sprite AddSprite(Sprite sprite)
        {
            //Add the sprite to the list.
            _SpritesInner.Add(sprite);

            //Sort the list by depth.
            _SpritesInner.Sort((x, y) => x.Depth.CompareTo(y.Depth));

            //If we have a valid content manager, load the sprite's content.
            if (_ContentManager != null) { sprite.LoadContent(); }

            //Return it.
            return sprite;
        }
        /// <summary>
        /// Get a sprite's index.
        /// </summary>
        /// <param name="tag">The tag of the sprite.</param>
        /// <returns>The index of the sprite.</returns>
        public int IndexOf(string tag)
        {
            return _SpritesOuter.IndexOf(GetSprite(tag));
        }
        /// <summary>
        /// Get a sprite.
        /// </summary>
        /// <param name="name">The name of the sprite.</param>
        /// <returns>The sprite.</returns>
        public Sprite GetSprite(string name)
        {
            //Loop through the list of sprites and find the one with the right name.
            foreach (Sprite sprite in _SpritesOuter) { if (sprite.Name.Equals(name)) { return sprite; } }

            //Return null.
            return null;
        }
        /// <summary>
        /// Get a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <returns>The sprite.</returns>
        public Sprite GetSprite(int index)
        {
            //If the index is out of bounds, quit here.
            if (index >= _SpritesOuter.Count) { return null; }

            //Return the sprite.
            return _SpritesInner[index];
        }
        /// <summary>
        /// Set a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <param name="sprite">The sprite.</param>
        public void SetSprite(int index, Sprite sprite)
        {
            //If the index is out of bounds, quit here.
            if (index >= Count) { return; }

            //Set the sprite.
            _SpritesOuter[index] = sprite;
        }
        /// <summary>
        /// Set a sprite.
        /// </summary>
        /// <param name="tag">The tag of the sprite to set.</param>
        /// <param name="sprite">The sprite.</param>
        public void SetSprite(string tag, Sprite sprite)
        {
            //The index.
            int index = IndexOf(tag);

            //If the index is out of bounds, quit here.
            if (index >= Count) { return; }

            //Set the sprite.
            _SpritesInner[index] = sprite;
        }
        /// <summary>
        /// Remove a sprite and its frames.
        /// </summary>
        /// <param name="tag">The tag of the sprite to remove.</param>
        public void RemoveSprite(string tag)
        {
            RemoveSprite(IndexOf(tag));
        }
        /// <summary>
        /// Remove a sprite and its frames.
        /// </summary>
        /// <param name="index">The index of the sprite to remove.</param>
        public void RemoveSprite(int index)
        {
            _SpritesInner.RemoveAt(index);

            //Sort the list by depth.
            _SpritesInner.Sort((x, y) => x.Depth.CompareTo(y.Depth));
        }
        /// <summary>
        /// Manage all sprites, ie. add and remove them from the master list.
        /// </summary>
        public void ManageSprites()
        {
            //Put all the sprites to update and draw in a seperate list.
            _SpritesOuter = new List<Sprite>(_SpritesInner);
        }
        /// <summary>
        /// Clear all sprites.
        /// </summary>
        public void Clear()
        {
            _SpritesInner.Clear();
            _SpritesOuter.Clear();
        }
        /// <summary>
        /// Get the bounds of a certain texture asset.
        /// </summary>
        /// <param name="name">The file name of this frame.</param>
        /// <returns>A rectangle containing the bounds.</returns>
        public Rectangle GetTextureBounds(string name)
        {
            //Return the bounds of this frame.
            try { return (_ContentManager.Load<Texture2D>(name).Bounds); }
            //Else return an empty rectangle.
            catch { return Rectangle.Empty; }
        }
        /// <summary>
        /// Get the bounds of a certain texture asset.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>A rectangle containing the bounds.</returns>
        public Rectangle GetTextureBounds(Texture2D texture)
        {
            //Return the bounds of this frame.
            return (texture.Bounds);
        }
        /// <summary>
        /// Shortcut to the first sprite of the collection.
        /// </summary>
        public Sprite FirstSprite()
        {
            return _SpritesInner.Count != 0 ? _SpritesInner[0] : null;
        }
        /// <summary>
        /// Shortcut to the last sprite in the collection.
        /// </summary>
        public Sprite LastSprite()
        {
            return _SpritesInner.Count != 0 ? _SpritesInner[_SpritesInner.Count - 1] : null;
        }
        /// <summary>
        /// Reset all sprites to this position.
        /// </summary>
        /// <param name="position">The new base position for all sprites.</param>
        public void SetPosition(Vector2 position)
        {
            _SpritesOuter.ForEach(item => item.UpdateSprite(position, item.Rotation));
        }
        /// <summary>
        /// Change the visibility of all the collection's sprites.
        /// </summary>
        /// <param name="visibility">The state of visibility to adapt.</param>
        private void ChangeVisibility(Visibility visibility)
        {
            //For all sprites in the collection, change their visibility to match the specified one.
            foreach (Sprite sprite in _SpritesInner) { sprite.Visibility = visibility; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Content Manager.
        /// </summary>
        public ContentManager ContentManager
        {
            get { return _ContentManager; }
            set { _ContentManager = value; }
        }
        /// <summary>
        /// The list of sprites.
        /// </summary>
        public List<Sprite> Sprites
        {
            get { return new List<Sprite>(_SpritesInner); }
            set { _SpritesInner = value; }
        }
        /// <summary>
        /// The number of sprites.
        /// </summary>
        public int Count
        {
            get { return _SpritesInner.Count; }
        }
        /// <summary>
        /// The collection's state of visibility.
        /// </summary>
        public Visibility Visibility
        {
            get { return (_SpritesInner.Exists(s => s.Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Invisible); }
            set { ChangeVisibility(value); }
        }
        #endregion
    }
}