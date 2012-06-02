using System;
using System.Collections.Generic;

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

using Library.Animate;
using Library.Core;
using Library.Entities;
using Library.Imagery;

namespace Library.Factories
{
    /// <summary>
    /// The factory makes it easy to add and create all kinds of things.
    /// </summary>
    public class Factory
    {
        #region Fields
        private static Factory _Instance;
        #endregion

        #region Constructors
        private Factory() { }
        #endregion

        #region Methods
        #region Layers
        /// <summary>
        /// Add a layer to a level.
        /// </summary>
        /// <param name="level">The level to add the layer to.</param>
        /// <param name="layer">The level to add.</param>
        public Layer AddLayer(Level level, Layer layer)
        {
            //Add the layer and return it.
            return level.AddLayer(layer);
        }
        /// <summary>
        /// Add a layer to a level.
        /// </summary>
        /// <param name="level">The level to add the layer to.</param>
        /// <param name="name">The name of the layer.</param>
        /// <param name="scrollSpeed">The scrolling speed of this layer. Used for parallex scrolling.</param>
        public Layer AddLayer(Level level, string name, Vector2 scrollSpeed)
        {
            //Add the layer and return it.
            return level.AddLayer(name, scrollSpeed);
        }
        #endregion

        #region Items
        /// <summary>
        /// Add an item to a layer.
        /// </summary>
        /// <param name="layer">The destination layer.</param>
        /// <param name="item">The item to add.</param>
        public Item AddItem(Layer layer, Item item)
        {
            //Add the item to the specified layer and return it.
            return layer.AddItem(item);
        }
        /// <summary>
        /// Add a texture item to a layer.
        /// </summary>
        /// <param name="layer">The layer to add the item to.</param>
        /// <param name="spritePath">The name of the item's sprite.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="scale">The scale of the item.</param>
        public TextureItem AddTextureItem(Layer layer, string spritePath, string name, Vector2 position, float rotation, Vector2 scale)
        {
            //The item.
            TextureItem item = new TextureItem(layer.Level, name, position, rotation, scale);
            item.AddSprite(spritePath);

            //Add the item and return it.
            return (layer.AddItem(item) as TextureItem);
        }
        #endregion

        #region Parts
        /// <summary>
        /// Add a part to an entity.
        /// </summary>
        /// <param name="entity">The destination entity.</param>
        /// <returns>The recently added part.</returns>
        public Part AddPart(Entity entity)
        {
            //Add the part to the specified entity and return it.
            return entity.AddPart(new Part());
        }
        /// <summary>
        /// Add a part to an entity.
        /// </summary>
        /// <param name="entity">The destination entity.</param>
        /// <param name="part">The part to add.</param>
        /// <returns>The recently added part.</returns>
        public Part AddPart(Entity entity, Part part)
        {
            //Add the part to the specified entity and return it.
            return entity.AddPart(part);
        }
        /// <summary>
        /// Add a part to an entity.
        /// </summary>
        /// <param name="entity">The destination entity.</param>
        /// <param name="body">The body to be in the part.</param>
        /// <returns>The recently added part.</returns>
        public Part AddPart(Entity entity, Body body)
        {
            //Add the part to the specified entity and return it.
            Part part = AddPart(entity);
            part.Body = body;

            //Return the part.
            return part;
        }
        /// <summary>
        /// Add a part to an entity.
        /// </summary>
        /// <param name="entity">The destination entity.</param>
        /// <param name="body">The body to be in the part.</param>
        /// <param name="sprite">The sprite to be in the part.</param>
        /// <returns>The recently added part.</returns>
        public Part AddPart(Entity entity, Body body, Sprite sprite)
        {
            //Add the part to the specified entity and return it.
            Part part = AddPart(entity, body);
            part.AddSprite(sprite);

            //Return the part.
            return part;
        }
        #endregion

        #region Characters
        /// <summary>
        /// Add a character to the game.
        /// </summary>
        /// <param name="layer">The layer that this character will belong to.</param>
        /// <param name="name">The name of the character.</param>
        /// <param name="position">The position of the character.</param>
        /// <param name="width">The width of the character.</param>
        /// <param name="height">The height of the character.</param>
        /// <returns>The character.</returns>
        public Character AddCharacter(Layer layer, string name, Vector2 position, float width, float height)
        {
            //Add the character to a layer.
            Character character = new Character(layer.Level, name, position, 0, Vector2.One, width, height);
            layer.AddItem(character);

            //Return the character.
            return character;
        }
        #endregion

        #region Entities
        /// <summary>
        /// Add a box to the game.
        /// </summary>
        /// <param name="layer">The layer that this box will belong to.</param>
        /// <param name="name">The name of the box.</param>
        /// <param name="spritePath">The name of the sprite to attach to it.</param>
        /// <param name="position">The position of the box.</param>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        /// <returns>The recently added part.</returns>
        public Box AddBox(Layer layer, string name, string spritePath, Vector2 position, float width, float height)
        {
            //Add the part to the specified entity and return it.
            Box box = AddBox(layer, name, position, 0, Vector2.One, width, height);
            box.SetSprite(AddSprite(box.Sprites, name, spritePath, new Vector2(width / 2, height / 2)));

            //Return the box.
            return box;
        }
        /// <summary>
        /// Add a box to the game.
        /// </summary>
        /// <param name="layer">The layer that this box will belong to.</param>
        /// <param name="name">The name of the box.</param>
        /// <param name="position">The position of the box.</param>
        /// <param name="rotation">The rotation of the box.</param>
        /// <param name="scale">The scale of the box.</param>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        /// <returns>The recently added part.</returns>
        public Box AddBox(Layer layer, string name, Vector2 position, float rotation, Vector2 scale, float width, float height)
        {
            //Add the part to the specified entity and return it.
            return (AddItem(layer, new Box(layer.Level, name, position, rotation, scale, width, height)) as Box);
        }
        #endregion

        #region Sprites
        /// <summary>
        /// Add a sprite to a manager.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="path">The path of the texture.</param>
        /// <returns>The recently added sprite.</returns>
        public Sprite AddSprite(SpriteManager manager, string name, string path)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Add a frame to the sprite.
            sprite.AddFrame(path);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        /// <summary>
        /// Add a sprite to a manager.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="path">The path of the texture.</param>
        /// <param name="origin">The origin of the sprite.</param>
        /// <returns>The recently added sprite.</returns>
        public Sprite AddSprite(SpriteManager manager, string name, string path, Vector2 origin)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Add a frame to the sprite.
            sprite.AddFrame(path, origin);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        /// <summary>
        /// Add a sprite to a manager.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="path">The path of the texture.</param>
        /// <param name="rotationOffset">The rotation offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        public Sprite AddSprite(SpriteManager manager, string name, string path, float rotationOffset, string tag)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Set the properties.
            sprite.Tag = tag;
            sprite.RotationOffset = rotationOffset;

            //Add a frame to the sprite.
            sprite.AddFrame(path);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        /// <summary>
        /// Add a sprite to a manager.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="path">The path of the texture.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="timePerFrame">The time per frame.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="offset">The offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        public Sprite AddSprite(SpriteManager manager, string name, string path, Vector2 position, float timePerFrame, float scale, int depth, float rotation,
            float offset, string tag)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Set the properties.
            sprite.Position = position;
            sprite.TimePerFrame = timePerFrame;
            sprite.Scale = scale;
            sprite.Depth = depth;
            sprite.Rotation = rotation;
            sprite.PositionOffset = offset;

            //Add a frame to the sprite.
            sprite.AddFrame(path);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        /// <summary>
        /// Add a sprite to a manager.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="path">The path of the texture.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="orbitRotation">The orbit rotation of the sprite.</param>
        /// <param name="timePerFrame">The time per frame.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <param name="offset">The offset of the sprite.</param>
        public Sprite AddSprite(SpriteManager manager, string name, string path, Vector2 position, float orbitRotation, float timePerFrame, float scale, int depth,
            float rotation, string tag, float offset)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Set the properties.
            sprite.Position = Helper.CalculateOrbitPosition(position, orbitRotation, offset);
            sprite.TimePerFrame = timePerFrame;
            sprite.Scale = scale;
            sprite.Depth = depth;
            sprite.Rotation = rotation;
            sprite.PositionOffset = offset;
            sprite.RotationOffset = Helper.SubtractAngles(rotation, orbitRotation);

            //Add a frame to the sprite.
            sprite.AddFrame(path);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        /// <summary>
        /// Add a sprite.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="path">The path of the texture.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="timePerFrame">The time per frame.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="offset">The offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <param name="origin">The origin of the sprite.</param>
        public Sprite AddSprite(SpriteManager manager, string name, string path, Vector2 position, float timePerFrame, float scale, int depth, float rotation, float offset,
            string tag, Vector2 origin)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Set the properties.
            sprite.Position = position;
            sprite.TimePerFrame = timePerFrame;
            sprite.Scale = scale;
            sprite.Depth = depth;
            sprite.Rotation = rotation;
            sprite.PositionOffset = offset;
            sprite.Tag = tag;

            //Add a frame to the sprite.
            sprite.AddFrame(path, origin);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        /// <summary>
        /// Add a sprite.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="texture">The texture of the sprite.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="timePerFrame">The time per frame.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="offset">The positional offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        public Sprite AddSprite(SpriteManager manager, string name, Texture2D texture, Vector2 position, float timePerFrame, float scale, int depth, float rotation,
            float offset, string tag)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Set the properties.
            sprite.Position = position;
            sprite.TimePerFrame = timePerFrame;
            sprite.Scale = scale;
            sprite.Depth = depth;
            sprite.Rotation = rotation;
            sprite.PositionOffset = offset;
            sprite.Tag = tag;

            //Add a frame to the sprite.
            sprite.AddFrame(texture);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        /// <summary>
        /// Add a sprite.
        /// </summary>
        /// <param name="manager">The manager to add the sprite to.</param>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="texture">The texture of the sprite.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="timePerFrame">The time per frame.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="offset">The offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <param name="origin">The origin of the sprite.</param>
        public Sprite AddSprite(SpriteManager manager, string name, Texture2D texture, Vector2 position, float timePerFrame, float scale, int depth, float rotation,
            float offset, string tag, Vector2 origin)
        {
            //Create the sprite and add it to the manager.
            Sprite sprite = manager.AddSprite(new Sprite(manager, name));

            //Set the properties.
            sprite.Position = position;
            sprite.TimePerFrame = timePerFrame;
            sprite.Scale = scale;
            sprite.Depth = depth;
            sprite.Rotation = rotation;
            sprite.PositionOffset = offset;
            sprite.Tag = tag;

            //Add a frame to the sprite.
            sprite.AddFrame(texture, origin);
            manager.ManageSprites();

            //Return the sprite.
            return sprite;
        }
        #endregion

        #region Bones
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="position">The position of the bone.</param>
        public void AddBone(Skeleton skeleton, Vector2 position)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, position, 1);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        public void AddBone(Skeleton skeleton, Vector2 position, float length)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, -1, position, length);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="parentIndex">The index of the parent bone.</param>
        public void AddBone(Skeleton skeleton, int parentIndex)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, parentIndex, 1);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="length">The length of the bone.</param>
        public void AddBone(Skeleton skeleton, int parentIndex, float length)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, parentIndex, skeleton.CalculatePosition(parentIndex), length);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        public void AddBone(Skeleton skeleton, int parentIndex, Vector2 position)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, parentIndex, position, 1);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        public void AddBone(Skeleton skeleton, int parentIndex, Vector2 position, float length)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, "", parentIndex, position, length, Vector2.Zero);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton that the bone will belong to.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="scale">The scale of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        public void AddBone(Skeleton skeleton, int index, int parentIndex, Vector2 position, Vector2 scale, float rotation, float length)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, "", index, parentIndex, position, scale, rotation, length);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton that the bone will belong to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="scale">The scale of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        public void AddBone(Skeleton skeleton, string name, int index, int parentIndex, Vector2 position, Vector2 scale, float rotation, float length)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, name, "", index, parentIndex, position, length, rotation, scale, Vector2.Zero, 0);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        public void AddBone(Skeleton skeleton, string spritePath, Vector2 position, float length, Vector2 origin)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, spritePath, -1, position, length, origin);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        public void AddBone(Skeleton skeleton, string spritePath, int parentIndex, float length, Vector2 origin)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, spritePath, parentIndex, length, origin, 0);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string spritePath, int parentIndex, float length, Vector2 origin, float rotationOffset)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, spritePath, parentIndex, skeleton.CalculatePosition(parentIndex), length, origin, rotationOffset);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        public void AddBone(Skeleton skeleton, string spritePath, int parentIndex, Vector2 position, float length, Vector2 origin)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, spritePath, parentIndex, position, length, origin, 0);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, int parentIndex, Vector2 position, float length, Vector2 origin)
        {
            AddBone(skeleton, name, spritePath, -1, parentIndex, position, length, 0, Vector2.One, origin, 0);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string spritePath, int parentIndex, Vector2 position, float length, Vector2 origin, float rotationOffset)
        {
            //Add a bone to a skeleton.
            AddBone(skeleton, "", spritePath, -1, parentIndex, position, length, 0, Vector2.One, origin, rotationOffset);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, Vector2 position, float length, Vector2 origin)
        {
            //Add the bone.
            AddBone(skeleton, name, spritePath, position, length, 0, origin);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, Vector2 position, float length, float rotation, Vector2 origin)
        {
            //Add the bone.
            AddBone(skeleton, name, spritePath, position, length, rotation, origin, 0);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, Vector2 position, float length, float rotation, Vector2 origin, float rotationOffset)
        {
            AddBone(skeleton, name, spritePath, -1, position, length, rotation, Vector2.One, origin, rotationOffset);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="parentIndex">The index of the parent.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, int parentIndex, Vector2 position, float length, float rotation, Vector2 origin,
            float rotationOffset)
        {
            AddBone(skeleton, name, spritePath, -1, parentIndex, position, length, rotation, Vector2.One, origin, rotationOffset);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="parentIndex">The index of the parent.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, int parentIndex, float length, float rotation, Vector2 origin,
            float rotationOffset)
        {
            //Add the bone.
            AddBone(skeleton, name, spritePath, -1, parentIndex, skeleton.CalculatePosition(parentIndex), length, rotation, Vector2.One, origin, rotationOffset);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        /// <param name="scale">The scale of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, Vector2 position, float length, float rotation, Vector2 scale, Vector2 origin,
            float rotationOffset)
        {
            AddBone(skeleton, name, spritePath, -1, position, length, rotation, scale, origin, rotationOffset);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        /// <param name="scale">The scale of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, int index, Vector2 position, float length, float rotation, Vector2 scale, Vector2 origin,
            float rotationOffset)
        {
            AddBone(skeleton, name, spritePath, index, -1, position, length, rotation, scale, origin, rotationOffset);
        }
        /// <summary>
        /// Add a bone to a skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to add the bone to.</param>
        /// <param name="name">The name of the bone.</param>
        /// <param name="spritePath">The path of the sprite to attach to the bone.</param>
        /// <param name="index">The index of the bone.</param>
        /// <param name="parentIndex">The index of the parent bone.</param>
        /// <param name="position">The position of the bone.</param>
        /// <param name="length">The length of the bone.</param>
        /// <param name="rotation">The rotation of the bone.</param>
        /// <param name="scale">The scale of the bone.</param>
        /// <param name="origin">The origin of the sprite to attach to the bone.</param>
        /// <param name="rotationOffset">The bone sprite's rotation offset.</param>
        public void AddBone(Skeleton skeleton, string name, string spritePath, int index, int parentIndex, Vector2 position, float length, float rotation,
            Vector2 scale, Vector2 origin, float rotationOffset)
        {
            //Add the bone.
            skeleton.AddBone(new Bone(skeleton, name, index, parentIndex, position, scale, rotation, length));

            //If a sprite name has been received, attach a sprite to the bone.
            if (!spritePath.Equals(""))
            {
                Factory.Instance.AddSprite(skeleton.Sprites, name, spritePath, position, 0, 1, 0, 0, 0, (skeleton.Bones.Count - 1).ToString(), origin);
                skeleton.Sprites.GetSprite(name).RotationOffset = rotationOffset;
            }
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// The factory instance.
        /// </summary>
        public static Factory Instance
        {
            get
            {
                if (_Instance == null) { _Instance = new Factory(); }
                return _Instance;
            }
        }
        #endregion
    }
}