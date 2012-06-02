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
using Library.Tools;

namespace Library.Imagery
{
    /// <summary>
    /// A sprite manager is used both as a container of sprites and a tool with which to handle them.
    /// </summary>
    public class SpriteManager
    {
        #region Fields
        private ContentManager _ContentManager;
        private RobustList<Sprite> _Sprites;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <returns>The sprite instance.</returns>
        public Sprite this[int index]
        {
            get { return Find(index); }
            set { Set(index, value); }
        }
        /// <summary>
        /// Get or set a sprite.
        /// </summary>
        /// <param name="tag">The tag of the sprite.</param>
        /// <returns>The sprite instance.</returns>
        public Sprite this[string tag]
        {
            get { return Find(tag); }
            set { Set(tag, value); }
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
            //Initialize variables.
            _Sprites = new RobustList<Sprite>();
        }
        /// <summary>
        /// Load the texture for all the sprites using the Content Pipeline.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager contentManager)
        {
            //Save the content manager for future usage.
            _ContentManager = contentManager;

            //Update the robust list so that each added sprite will have its content loaded.
            ManageSprites();

            //If there's any sprites in the list, load their content.
            _Sprites.ForEach(item => item.LoadContent());
        }
        /// <summary>
        /// Update all sprites.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        public void Update(GameTime gameTime)
        {
            //Update the robust list.
            ManageSprites();

            //Update all sprites.
            _Sprites.ForEach(item => item.Update(gameTime));
        }
        /// <summary>
        /// Update all sprites.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        public void Update(GameTime gameTime, Vector2 position, float rotation)
        {
            //Update the robust list.
            ManageSprites();

            //Update all sprites's position.
            _Sprites.ForEach(item => item.UpdateSprite(position, rotation));

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
            _Sprites.ForEach(item => item.Draw(spriteBatch));
        }

        /// <summary>
        /// Add a sprite.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        public Sprite Add(Sprite sprite)
        {
            //Add the sprite to the list.
            _Sprites.Add(sprite);

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
            return _Sprites.IndexOf(Find(tag));
        }
        /// <summary>
        /// Get a sprite.
        /// </summary>
        /// <param name="name">The name of the sprite.</param>
        /// <returns>The sprite.</returns>
        public Sprite Find(string name)
        {
            //Loop through the list of sprites and find the one with the right name.
            foreach (Sprite sprite in _Sprites) { if (sprite.Name.Equals(name)) { return sprite; } }

            //Return null.
            return null;
        }
        /// <summary>
        /// Get a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <returns>The sprite.</returns>
        public Sprite Find(int index)
        {
            //If the index is out of bounds, quit here.
            if (index >= _Sprites.Count) { return null; }

            //Return the sprite.
            return _Sprites[index];
        }
        /// <summary>
        /// Set a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <param name="sprite">The sprite.</param>
        public void Set(int index, Sprite sprite)
        {
            //If the index is out of bounds, quit here.
            if (index >= Count) { return; }

            //Set the sprite.
            _Sprites[index] = sprite;
        }
        /// <summary>
        /// Set a sprite.
        /// </summary>
        /// <param name="tag">The tag of the sprite to set.</param>
        /// <param name="sprite">The sprite.</param>
        public void Set(string tag, Sprite sprite)
        {
            //The index.
            int index = IndexOf(tag);

            //If the index is out of bounds, quit here.
            if (index >= Count) { return; }

            //Set the sprite.
            _Sprites[index] = sprite;
        }
        /// <summary>
        /// Remove a sprite and its frames.
        /// </summary>
        /// <param name="name">The name of the sprite to remove.</param>
        public void Remove(string name)
        {
            Remove(Find(name));
        }
        /// <summary>
        /// Remove a sprite and its frames.
        /// </summary>
        /// <param name="sprite">The sprite to remove.</param>
        public void Remove(Sprite sprite)
        {
            _Sprites.Remove(sprite);

            //Sort the list by depth.
            _Sprites.Sort((x, y) => x.Depth.CompareTo(y.Depth));
        }
        /// <summary>
        /// Remove a sprite and its frames.
        /// </summary>
        /// <param name="index">The index of the sprite to remove.</param>
        public void Remove(int index)
        {
            _Sprites.RemoveAt(index);

            //Sort the list by depth.
            _Sprites.Sort((x, y) => x.Depth.CompareTo(y.Depth));
        }
        /// <summary>
        /// Manage all sprites, ie. add and remove them from the master list.
        /// </summary>
        public void ManageSprites()
        {
            //Update the robust list, and sort it by depth if necessary.
            if (_Sprites.Update()) { _Sprites.Sort((x, y) => x.Depth.CompareTo(y.Depth)); }
        }
        /// <summary>
        /// Clear all sprites.
        /// </summary>
        public void Clear()
        {
            _Sprites.Clear();
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
            return _Sprites.Count != 0 ? _Sprites[0] : null;
        }
        /// <summary>
        /// Shortcut to the last sprite in the collection.
        /// </summary>
        public Sprite LastSprite()
        {
            return _Sprites.Count != 0 ? _Sprites[_Sprites.Count - 1] : null;
        }
        /// <summary>
        /// Reset all sprites to this position.
        /// </summary>
        /// <param name="position">The new base position for all sprites.</param>
        public void SetPosition(Vector2 position)
        {
            _Sprites.ForEach(item => item.UpdateSprite(position, item.Rotation));
        }
        /// <summary>
        /// Change the visibility of all the collection's sprites.
        /// </summary>
        /// <param name="visibility">The state of visibility to adapt.</param>
        private void ChangeVisibility(Visibility visibility)
        {
            //For all sprites in the collection, change their visibility to match the specified one.
            foreach (Sprite sprite in _Sprites) { sprite.Visibility = visibility; }
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
            get { return _Sprites.ToList(); }
        }
        /// <summary>
        /// The number of sprites.
        /// </summary>
        public int Count
        {
            get { return _Sprites.Count; }
        }
        /// <summary>
        /// The complete number of sprites, including those that have yet to be activated.
        /// </summary>
        public int CompleteCount
        {
            get { return _Sprites.CompleteCount; }
        }
        /// <summary>
        /// The collection's state of visibility.
        /// </summary>
        public Visibility Visibility
        {
            get { return (_Sprites.Exists(s => s.Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Invisible); }
            set { ChangeVisibility(value); }
        }
        #endregion
    }
}