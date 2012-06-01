/*
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
using FarseerPhysics.Collisions;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Springs;
using FarseerPhysics.Factories;
using FarseerPhysics.Interfaces;
using FarseerPhysics.Mathematics;

using Library.Factories;
using Library.Imagery;
using Library.Weaponry;

namespace Library
{
    /// <summary>
    /// The direction an object is facing.
    /// </summary>
    public enum FacingDirection
    {
        None,
        Left,
        Right
    }

    /// <summary>
    /// The object serves as a base for all other objects that strive to make a physical appearance in the game.
    /// </summary>
    public class Object
    {
        #region Fields
        private System _System;
        private List<Body> _Bodies;
        private List<Geom> _Geoms;
        private SpriteManager _SpriteManager = new SpriteManager();
        public int Id;
        public string FirstTag;
        public string SecondTag;
        private FacingDirection _FacingDirection;
        public Vector2 PreviousPosition = Vector2.Zero;
        public Weapon WeaponChild;

        /// <summary>
        /// The dispose handler delegate.
        /// </summary>
        /// <param name="obj">The Object to dispose.</param>
        public delegate void DisposeHandler(Object obj);
        /// <summary>
        /// The command handler delegate.
        /// </summary>
        /// <param name="position">The position.</param>
        public delegate void CommandHandler(Vector2 position);
        public DisposeHandler OnDispose;
        public event CommandHandler Command;
        #endregion

        #region Methods
        #region Main Methods
        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="system">The System the object is going to be a part of.</param>
        public virtual void Initialize(System system)
        {
            //Initialize a few variables.
            _System = system;
            _Bodies = new List<Body>();
            _Geoms = new List<Geom>();

            //Get this Object's Id.
            Id = _System.GetId(this);

            //Add a Command event.
            Command += new CommandHandler(OnCommand);
        }
        /// <summary>
        /// Load the object's content.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public virtual void LoadContent(ContentManager contentManager)
        {
            //Load the Sprite.
            _SpriteManager.LoadContent(contentManager);
        }
        /// <summary>
        /// Update the object.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        public virtual void Update(GameTime gameTime)
        {
            //Loop through all sprites.
            foreach (Sprite sprite in _SpriteManager.Sprites)
            {
                //Update the sprite frames along with Sprite position.
                sprite.Update(gameTime, _Bodies[0].Position, _Bodies[0].Rotation);
            }
        }
        /// <summary>
        /// Draw the object.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Draw all the object's sprites.
            _SpriteManager.Draw(spriteBatch);
        }
        #endregion

        #region Add Body
        /// <summary>
        /// Add a rectangular body to the Object.
        /// </summary>
        /// <param name="spriteName">The name of the sprite that will display the added body.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="timePerFrame">The time it takes per frame for the sprite to update.</param>
        /// <param name="scale">The scaling factor of the sprite.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <param name="depth">The depth of the body's sprite. Used to manage the order of drawing importance.</param>
        /// <param name="mass">The mass of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="offset">The offset of the sprite relative to the body.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <returns>The index of the body.</returns>
        public int AddBody(string spriteName, Vector2 position, float timePerFrame, float scale, float width, float height, int depth, float mass, float rotation,
            float offset, string tag)
        {
            //Create the body and save it to the list.
            _Bodies.Add(BodyFactory.Instance.CreateRectangleBody(_System.PhysicsSimulator, width, height, mass));
            //Pass along the position.
            _Bodies[BodyCount - 1].Position = position;

            //Create the geom and save it to the list.
            _Geoms.Add(GeomFactory.Instance.CreateRectangleGeom(_System.PhysicsSimulator, _Bodies[BodyCount - 1], width, height));

            //The Sprite.
            Factory.Instance.AddSprite(_SpriteManager, spriteName, _Bodies[BodyCount - 1].Position, timePerFrame, scale, depth, rotation, offset, tag);
            //Pass along a tag to the body.
            _SpriteManager.GetLastSprite().Tag = tag;

            //Return the index of the body.
            return (BodyCount - 1);
        }
        /// <summary>
        /// Add a rectangular body to the Object.
        /// </summary>
        /// <param name="spriteName">The name of the sprite that will display the added body.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="timePerFrame">The time it takes per frame for the sprite to update.</param>
        /// <param name="scale">The scaling factor of the sprite.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <param name="depth">The depth of the body's sprite. Used to manage the order of drawing importance.</param>
        /// <param name="mass">The mass of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <returns>The index of the body.</returns>
        public int AddBody(string spriteName, Vector2 position, float timePerFrame, float scale, float width, float height, int depth, float mass, float rotation, string tag)
        {
            //Create the body and save it to the list.
            _Bodies.Add(BodyFactory.Instance.CreateRectangleBody(_System.PhysicsSimulator, width, height, mass));
            //Pass along the position.
            _Bodies[BodyCount - 1].Position = position;

            //Create the geom and save it to the list.
            _Geoms.Add(GeomFactory.Instance.CreateRectangleGeom(_System.PhysicsSimulator, _Bodies[BodyCount - 1], width, height));

            //The Sprite.
            Factory.Instance.AddSprite(_SpriteManager, spriteName, _Bodies[BodyCount - 1].Position, timePerFrame, scale, depth, rotation, 0, tag);
            //Pass along the index of the body.
            _SpriteManager.GetLastSprite().Tag = tag;

            //Return the index of the body.
            return (BodyCount - 1);
        }
        /// <summary>
        /// Add a rectangular body to the Object.
        /// </summary>
        /// <param name="spriteName">The name of the sprite that will display the added body.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="timePerFrame">The time it takes per frame for the sprite to update.</param>
        /// <param name="scale">The scaling factor of the sprite.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <param name="depth">The depth of the body's sprite. Used to manage the order of drawing importance.</param>
        /// <param name="mass">The mass of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="offset">The offset of the sprite relative to the body.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <param name="origin">The origin of the sprite, normally the center of it.</param>
        /// <returns>The index of the body.</returns>
        public int AddBody(string spriteName, Vector2 position, float timePerFrame, float scale, float width, float height, int depth, float mass, float rotation, string tag,
            float offset, Vector2 origin)
        {
            //Create the body and save it to the list.
            _Bodies.Add(BodyFactory.Instance.CreateRectangleBody(_System.PhysicsSimulator, width, height, mass));
            //Pass along the position.
            _Bodies[BodyCount - 1].Position = position;

            //Create the geom and save it to the list.
            _Geoms.Add(GeomFactory.Instance.CreateRectangleGeom(_System.PhysicsSimulator, _Bodies[BodyCount - 1], width, height));

            //The Sprite.
            Factory.Instance.AddSprite(_SpriteManager, spriteName, _Bodies[BodyCount - 1].Position, timePerFrame, scale, depth, rotation, offset, tag, origin);
            //Pass along the index of the body.
            _SpriteManager.GetLastSprite().Tag = tag;

            //Return the index of the body.
            return (BodyCount - 1);
        }
        /// <summary>
        /// Add an elliptical body to the Object.
        /// </summary>
        /// <param name="spriteName">The name of the sprite that will display the added body.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="timePerFrame">The time it takes per frame for the sprite to update.</param>
        /// <param name="scale">The scaling factor of the sprite.</param>
        /// <param name="width">The width of the body.</param>
        /// <param name="height">The height of the body.</param>
        /// <param name="depth">The depth of the body's sprite. Used to manage the order of drawing importance.</param>
        /// <param name="mass">The mass of the body.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <returns>The index of the body.</returns>
        public int AddBody(string spriteName, Vector2 position, float timePerFrame, float scale, float width, float height, int depth, float mass, string tag)
        {
            //Create the body and save it to the list.
            _Bodies.Add(BodyFactory.Instance.CreateEllipseBody(_System.PhysicsSimulator, width / 2, height / 2, mass));
            //Pass along the position.
            _Bodies[BodyCount - 1].Position = position;

            //Create the geom and save it to the list.
            _Geoms.Add(GeomFactory.Instance.CreateEllipseGeom(_System.PhysicsSimulator, _Bodies[BodyCount - 1], width, height, 32));

            //The Sprite.
            Factory.Instance.AddSprite(_SpriteManager, spriteName, _Bodies[BodyCount - 1].Position, timePerFrame, scale, depth, 0, 0, tag);
            //Pass along the index of the body.
            _SpriteManager.GetLastSprite().Tag = tag;

            //Return the index of the body.
            return (BodyCount - 1);
        }
        /// <summary>
        /// Add a circular body to the Object.
        /// </summary>
        /// <param name="spriteName">The name of the sprite that will display the added body.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="timePerFrame">The time it takes per frame for the sprite to update.</param>
        /// <param name="scale">The scaling factor of the sprite.</param>
        /// <param name="radius">The radius of the circular body.</param>
        /// <param name="depth">The depth of the body's sprite. Used to manage the order of drawing importance.</param>
        /// <param name="mass">The mass of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="offset">The offset of the sprite relative to the body.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <returns>The index of the body.</returns>
        public int AddBody(string spriteName, Vector2 position, float timePerFrame, float scale, float radius, int depth, float mass, float rotation, float offset, string tag)
        {
            //Create the body and save it to the list.
            _Bodies.Add(BodyFactory.Instance.CreateCircleBody(_System.PhysicsSimulator, radius, mass));
            //Pass along the position.
            _Bodies[BodyCount - 1].Position = position;

            //Create the geom and save it to the list.
            _Geoms.Add(GeomFactory.Instance.CreateCircleGeom(_System.PhysicsSimulator, _Bodies[BodyCount - 1], radius, 32));

            //The Sprite.
            Factory.Instance.AddSprite(_SpriteManager, spriteName, _Bodies[BodyCount - 1].Position, timePerFrame, scale, depth, rotation, offset, tag);
            //Pass along the index of the body.
            _SpriteManager.GetLastSprite().Tag = tag;

            //Return the index of the body.
            return (BodyCount - 1);
        }
        #endregion

        /// <summary>
        /// Add a geom to a body of the Object.
        /// </summary>
        /// <param name="bodyIndex">The index of the body the geom will join with.</param>
        /// <param name="_Geom">The geom this new geom will be a clone of.</param>
        /// <param name="offset">The offset of the geom relative to the body.</param>
        /// <param name="angleOffset">The angle offset relative to the body's rotation.</param>
        public int AddGeometry(int bodyIndex, Geom _Geom, Vector2 offset, float angleOffset)
        {
            //Create a geom and add it to the list.
            _Geoms.Add(GeomFactory.Instance.CreateGeom(_System.PhysicsSimulator, _Bodies[bodyIndex], _Geom, offset, angleOffset));

            //Return the index of the geom.
            return (GeomCount - 1);
        }
        /// <summary>
        /// Set the position of the first body.
        /// </summary>
        /// <param name="position">The position.</param>
        public virtual void OnCommand(Vector2 position)
        {
            //Update the data.
            _Bodies[0].Position = position;
        }
        /// <summary>
        /// Calculate the direction from a given angle.
        /// </summary>
        /// <param name="angleRadians">The angle.</param>
        /// <returns>The direction.</returns>
        public Vector2 CalculateDirection(double angleRadians)
        {
            //The direction of a given angle.
            return (new Vector2((float)-Math.Cos(angleRadians), (float)-Math.Sin(angleRadians)));
        }
        /// <summary>
        /// Dispose of the Object.
        /// </summary>
        public virtual void Dispose()
        {
            //The Bodies.
            foreach (Body body in _Bodies) { if (body != null) { body.Dispose(); } }
            //The Geoms.
            foreach (Geom geom in _Geoms) { if (geom != null) { geom.Dispose(); } }
            //The Sprite.
            _SpriteManager = null;
        }
        /// <summary>
        /// The weapon equip method.
        /// </summary>
        /// <param name="equiper">The Object that equips the weapon.</param>
        /// <param name="weaponWieldType">The wield type of the weapon. (Single, Dual or Support Weapon)</param>
        public virtual void EquipedWeapon(Object equiper, Weapon.WeaponWieldMode weaponWieldType) { }
        /// <summary>
        /// Unequip a weapon.
        /// </summary>
        public virtual void UnEquipedWeapon() { }
        /// <summary>
        /// A chance to decide some extra effect when collision occurs is given here.
        /// This can vary from the damage from a bullet to picking up objects in the game.
        /// </summary>
        /// <param name="geom1">The first geom that collides.</param>
        /// <param name="geom2">The second geom that collides.</param>
        /// <param name="contactList">The list of contacts that tell the exact tale of the collision.</param>
        /// <returns>True.</returns>
        public virtual bool OnCollide(Geom geom1, Geom geom2, ContactList contactList)
        {
            return true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The System that this Object is a part of.
        /// </summary>
        public System System
        {
            get { return (_System); }
            set { _System = value; }
        }
        /// <summary>
        /// The list of bodies this Object has.
        /// </summary>
        public List<Body> Bodies
        {
            get { return (_Bodies); }
            set { _Bodies = value; }
        }
        /// <summary>
        /// The list of geoms this Object has.
        /// </summary>
        public List<Geom> Geoms
        {
            get { return (_Geoms); }
            set { _Geoms = value; }
        }
        /// <summary>
        /// The sprite collection of the Object.
        /// </summary>
        public SpriteManager SpriteManager
        {
            get { return (_SpriteManager); }
            set { _SpriteManager = value; }
        }
        /// <summary>
        /// The facing direction of the Object.
        /// </summary>
        public FacingDirection FacingDirection
        {
            get { return (_FacingDirection); }
            set { _FacingDirection = value; }
        }
        /// <summary>
        /// The number of bodies in the list.
        /// </summary>
        public int BodyCount
        {
            get { return (_Bodies.Count); }
        }
        /// <summary>
        /// The number of geoms in the list.
        /// </summary>
        public int GeomCount
        {
            get { return (_Geoms.Count); }
        }
        #endregion
    }
}
*/