using System;
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

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace Library.Imagery
{
    /// <summary>
    /// The orientation of the sprite, namely none, left or right.
    /// </summary>
    public enum Orientation
    {
        None,
        Left,
        Right
    }
    /// <summary>
    /// The visibility of the sprite.
    /// </summary>
    public enum Visibility
    {
        Invisible,
        Visible
    }

    /// <summary>
    /// This is a collection of sprites.
    /// </summary>
    public class SpriteCollection : IDisposable
    {
        #region Fields
        private ContentManager _ContentManager;
        private List<Sprite> _Sprites;
        private List<Sprite> _SpritesToAdd;
        private List<Sprite> _SpritesToRemove;
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

        #region Constructor
        /// <summary>
        /// Create a sprite collection.
        /// </summary>
        public SpriteCollection()
        {
            //Initialize the sprite collection.
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the SpriteCollection.
        /// </summary>
        public void Initialize()
        {
            //Initialize variables.
            _Sprites = new List<Sprite>();
            _SpritesToAdd = new List<Sprite>();
            _SpritesToRemove = new List<Sprite>();
        }
        /// <summary>
        /// Load the texture for all the sprites using the Content Pipeline.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager contentManager)
        {
            //Save the content manager and sprite batch for future usage.
            _ContentManager = contentManager;

            //If there's any sprites in the list, load their content.
            if (SpriteCount != 0) { foreach (Sprite sprite in _Sprites) { sprite.LoadContent(); } }
        }
        /// <summary>
        /// Update all sprites.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        public void Update(GameTime gameTime)
        {
            //Manage all sprites, ie. add and remove them from the collection.
            //TODO: Launch these methods from an event invoker instead.
            ManageSprites();

            //Update all sprites.
            foreach (Sprite sprite in _Sprites) { sprite.Update(gameTime); }
        }
        /// <summary>
        /// Update all sprites.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        public void Update(GameTime gameTime, Vector2 position, float rotation)
        {
            //Manage all sprites, ie. add and remove them from the collection.
            //TODO: Launch these methods from an event invoker instead.
            ManageSprites();

            //Update all sprites.
            foreach (Sprite sprite in _Sprites) { sprite.Update(gameTime, position, rotation); }
        }
        /// <summary>
        /// Draw all sprites to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Loop through the depth values.
            for (int depth = 0; depth <= 10; depth++)
            {
                //Loop through all the sprites.
                foreach (Sprite sprite in _Sprites)
                {
                    //Check if the sprite's depth equals the looped depth value.
                    if (sprite.Depth == depth)
                    {
                        //If the Sprite is visible, draw it.
                        if (sprite.Visibility == Visibility.Visible) { sprite.Draw(spriteBatch); }
                    }
                }
            }
        }

        /// <summary>
        /// Add a sprite.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        public Sprite AddSprite(Sprite sprite)
        {
            //Add the sprite to the list.
            _SpritesToAdd.Add(sprite);
            //Return it.
            return sprite;
        }
        /// <summary>
        /// Get a sprite's index.
        /// </summary>
        /// <param name="tag">The tag of the sprite.</param>
        /// <returns>The index of the sprite.</returns>
        public int GetSpriteIndex(string tag)
        {
            //Return it.
            return (_Sprites.IndexOf(GetSprite(tag)));
        }
        /// <summary>
        /// Get a sprite.
        /// </summary>
        /// <param name="tag">The tag of the sprite.</param>
        /// <returns>The sprite.</returns>
        public Sprite GetSprite(string tag)
        {
            //Loop through the list of sprites and find the one with the right name.
            foreach (Sprite sprite in _Sprites) { if (sprite.Tag.Equals(tag)) { return sprite; } }

            //Return null.
            return (null);
        }
        /// <summary>
        /// Get a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <returns>The sprite.</returns>
        public Sprite GetSprite(int index)
        {
            //If the index is out of bounds, quit here.
            if (index >= _Sprites.Count) { return null; }

            //Return the sprite.
            return (_Sprites[index]);
        }
        /// <summary>
        /// Set a sprite.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        /// <param name="sprite">The sprite.</param>
        public void SetSprite(int index, Sprite sprite)
        {
            //If the index is out of bounds, quit here.
            if (index >= _Sprites.Count) { return; }

            //Set the sprite.
            _Sprites[index] = sprite;
        }
        /// <summary>
        /// Set a sprite.
        /// </summary>
        /// <param name="tag">The tag of the sprite.</param>
        /// <param name="sprite">The sprite.</param>
        public void SetSprite(string tag, Sprite sprite)
        {
            //The index.
            int index = GetSpriteIndex(tag);

            //If the index is out of bounds, quit here.
            if (index >= _Sprites.Count) { return; }

            //Set the sprite.
            _Sprites[index] = sprite;
        }
        /// <summary>
        /// Delete a sprite and its frames.
        /// </summary>
        /// <param name="tag">The tag of the sprite.</param>
        public void DeleteSprite(string tag)
        {
            //Delete the sprite at the correct position.
            _Sprites.RemoveAt(GetSpriteIndex(tag));
        }
        /// <summary>
        /// Delete a sprite and its frames.
        /// </summary>
        /// <param name="index">The index of the sprite.</param>
        public void DeleteSprite(int index)
        {
            //Delete the sprite at the correct position.
            _Sprites.RemoveAt(index);
        }
        /// <summary>
        /// Add and remove sprites to and from the collection.
        /// </summary>
        public void ManageSprites()
        {
            //If the collection has been initialized and has a content manager reference, continue.
            if (_ContentManager != null)
            {
                //If there is sprites to add to the collection, add them.
                if (_SpritesToAdd.Count != 0) { foreach (Sprite sprite in _SpritesToAdd) { _Sprites.Add(sprite); sprite.LoadContent(); } }
                //If there is sprites to remove from the collection, remove them.
                if (_SpritesToRemove.Count != 0) { foreach (Sprite sprite in _SpritesToRemove) { /*RemoveSprite(sprite);*/ } }

                //Clear the lists.
                _SpritesToAdd.Clear();
                _SpritesToRemove.Clear();
            }
        }
        /// <summary>
        /// Clear all sprites.
        /// </summary>
        public void Clear()
        {
            _Sprites.Clear();
            _SpritesToAdd.Clear();
            _SpritesToRemove.Clear();
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
        /// Get the first sprite in the collection, even if the sprite hasn't technically been added yet.
        /// </summary>
        public Sprite GetFirstSprite()
        {
            //Return the first sprite in the list.            
            if (_Sprites.Count != 0) { return _Sprites[0]; }
            else if (_SpritesToAdd.Count != 0) { return _SpritesToAdd[0]; }

            //Return null.
            return null;
        }
        /// <summary>
        /// Get the last sprite in the collection, even if the sprite hasn't technically been added yet.
        /// </summary>
        public Sprite GetLastSprite()
        {
            //Return the last sprite in the list.            
            if (_SpritesToAdd.Count == 0) { return (_Sprites.Count == 0) ? null : _Sprites[_Sprites.Count - 1]; }
            else { return _SpritesToAdd[_SpritesToAdd.Count - 1]; }
        }
        /// <summary>
        /// Update all sprites' positions, including those that still waits to be added.
        /// </summary>
        /// <param name="position">The updated position.</param>
        public void UpdatePositions(Vector2 position)
        {
            //Update all sprites' positions.
            foreach (Sprite sprite in _Sprites) { sprite.Position = position; }
            foreach (Sprite sprite in _SpritesToAdd) { sprite.Position = position; }
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

        #region Math
        /// <summary>
        /// Add two angles.
        /// </summary>
        /// <param name="radian1">The first angle to add.</param>
        /// <param name="radian2">The second angle to add.</param>
        /// <returns>The angle sum.</returns>
        public float AddAngles(float radian1, float radian2)
        {
            //Add the angles together.
            float addResult = radian1 + radian2;
            //Check if the sum of the angles has overreached a full lap, aka two PI, and if so fix it.
            if (addResult > (Math.PI * 2)) { return (addResult - ((float)Math.PI * 2)); }
            else { return addResult; }
        }
        /// <summary>
        /// Subtracts an angle from an angle.
        /// </summary>
        /// <param name="radian1">The angle to subtract from.</param>
        /// <param name="radian2">The angle to subtract.</param>
        /// <returns>The subtracted angle.</returns>
        public float SubtractAngles(float radian1, float radian2)
        {
            //Subtract the angles from eachother.
            float subtractResult = radian1 - radian2;
            //If the difference has exceeded a full lap, aka 0, fix that.
            if (subtractResult < 0) { return (subtractResult + ((float)Math.PI * 2)); }
            else { return subtractResult; }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose of the Sprite instance.
        /// </summary>
        public void Dispose() { }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// The Content Manager.
        /// </summary>
        public ContentManager ContentManager
        {
            get { return (_ContentManager); }
            set { _ContentManager = value; }
        }
        /// <summary>
        /// The list of sprites.
        /// </summary>
        public List<Sprite> Sprites
        {
            get { return _Sprites; }
            set { _Sprites = value; }
        }
        /// <summary>
        /// The number of sprites.
        /// </summary>
        public int SpriteCount
        {
            get { return (_Sprites.Count); }
        }
        /// <summary>
        /// The collection's state of visibility.
        /// </summary>
        public Visibility Visibility
        {
            get { return (_Sprites.Exists(s => s.Visibility == Imagery.Visibility.Visible) ? Visibility.Visible : Visibility.Invisible); }
            set { ChangeVisibility(value); }
        }
        #endregion
    }
}
