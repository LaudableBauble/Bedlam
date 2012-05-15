/*using System;
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

namespace Library.Weaponry
{
    /// <summary>
    /// The projectile of a weapon.
    /// </summary>
    public class Projectile : Object
    {
        #region Fields
        //The height.
        private float _Height;
        //The width.
        private float _Width;
        //The mass.
        private float _Mass;

        //The velocity of the projectile.
        public Vector2 Velocity;
        //The speed.
        public float _Speed;
        //The pixel offset.
        public float _PixelOffset;
        //The Sprite name.
        public string _SpriteName;
        //The position.
        public Vector2 _Position;
        //The Dispose timer.
        public float disposeTimer = -1;
        //The list if intersection points.
        public List<Vector2> lineIntersectInfoList = new List<Vector2>();
        public List<Vector2> lineIntersectVectorList = new List<Vector2>();
        //The direction.
        public Vector2 Direction;

        //The Dispose delegate.
        new public delegate void DisposeHandler(Projectile projectile);
        //The Dispose handler.
        new public DisposeHandler OnDispose;

        //The debug index.
        public int debugIndex;
        #endregion

        #region Constructors
        /// <summary>
        /// The main constructor of the Projectile.
        /// </summary>
        /// <param name="spriteName">The name of the sprite.</param>
        /// <param name="width">The width of the projectile.</param>
        /// <param name="height">The height of the projectile.</param>
        /// <param name="mass">The mass of the projectile.</param>
        /// <param name="speed">The speed of the projectile.</param>
        /// <param name="cannonAngle">The angle of the origin point.</param>
        /// <param name="cannonPosition">The position of the origin point.</param>
        /// <param name="pixelOffset">The pixel offset.</param>
        /// <param name="system">The System this Object is a part of.</param>
        /// <param name="contentManager">The contentmanager that keeps track of all content.</param>
        /// <param name="spriteBatch">The SpriteBatch that helps to draw the sprite of the projectile.</param>
        public Projectile(string spriteName, float width, float height, float mass, float speed,
            float cannonAngle, Vector2 cannonPosition, float pixelOffset, System system,
            ContentManager contentManager, SpriteBatch spriteBatch)
        {
            //The sprite name.
            _SpriteName = spriteName;
            //The width.
            _Width = width;
            //The height.
            _Height = height;
            //The mass.
            _Mass = mass;
            //The pixel offset.
            _PixelOffset = pixelOffset;
            //The position of the projectile. Calculated by adding the offset along the vector of the origin angle and point.
            _Position = Vector2.Add(Vector2.Multiply(CalculateDirection((double)cannonAngle), _PixelOffset), cannonPosition);
            //The speed of the projectile.
            _Speed = speed;

            //Initialize the projectile.
            Initialize(system);
            //Load all Content.
            LoadContent(contentManager);
            //Add a body of the given instructions.
            AddBody(_SpriteName, _Position, 1f, 1f, _Width, _Height, 0, _Mass, 0, "Projectile");

            //Make the projectile ignore gravity.
            Bodies[0].IgnoreGravity = true;
            //Attach the projectile's OnCollide method to the virtual method of the Object.
            Geoms[0].OnCollision += OnCollide;
            //The response to a collision is negative.
            //Geometry[0].CollisionResponseEnabled = false;

            //The direction.
            Direction = CalculateDirection((double)cannonAngle);
            //Initlaize the velocity of the projectile.
            InitializeVelocity(CalculateVelocity(Direction, _Speed));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the Projectile.
        /// </summary>
        /// <param name="system">The System this projectile is a part of.</param>
        public override void Initialize(System system)
        {
            //The inherit the method from the Object.
            base.Initialize(system);

            //Set the dispose timer.
            disposeTimer = 45;
            //Set the debug index.
            //debugIndex = System.debugText.Count;
        }
        /// <summary>
        /// Load the projectiles content.
        /// </summary>
        /// <param name="contentManager">The content manager, keeps track of all content.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            //The inherited method.
            base.LoadContent(contentManager);
        }
        /// <summary>
        /// Updates the projectile.
        /// </summary>
        /// <param name="gameTime">The game time object.</param>
        public override void Update(GameTime gameTime)
        {
            //The inherited method.
            base.Update(gameTime);

            //Start the dispose timer.
            DisposeTimer();
            //Check the effects of the high velocity.
            CheckHighVelocityCollision();
        }
        /// <summary>
        /// Draw the sprites of this projectile.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Check and manage the high velocity of this projectile.
        /// </summary>
        public void CheckHighVelocityCollision()
        {
            //Point two of the ray.
            Vector2 point2 = Vector2.Add(Vector2.Multiply(Direction, 1), PreviousPosition);

            //Using the Ray Helper, get the intersection points in between the "jumps".
            RayHelper.LineSegmentGeomIntersect(Bodies[0].Position, point2,
                System.Spartan2.Geoms[0], false, ref lineIntersectVectorList);
            RayHelper.LineSegmentGeomIntersect(Bodies[0].Position, point2,
                System.Spartan2.Geoms[1], false, ref lineIntersectVectorList);
            RayHelper.LineSegmentGeomIntersect(Bodies[0].Position, point2,
                System.Spartan2.Geoms[2], false, ref lineIntersectVectorList);

            //Add a debug text.
            //System.AddDebugText(debugIndex, lineIntersectInfoList.Count.ToString());

            //If there was a collision between the two stored positions.
            if (lineIntersectInfoList.Count > 0)
            {
                //Tell the Spartan that it has been hit.
                System.Spartan2.OnHighVelocityCollide(Geoms[0],
                    System.Spartan2.Geoms[0], lineIntersectInfoList);
                //Dispose this projectile.
                OnDispose(this);
            }
        }
        /// <summary>
        /// The Dispose Timer.
        /// </summary>
        public void DisposeTimer()
        {
            //Countdown until disposal.
            if (disposeTimer > 0) { disposeTimer -= (100 * System.UpdateSpeed); }
            else if (disposeTimer <= 0) { OnDispose(this); }
        }
        /// <summary>
        /// Dispose this projectile.
        /// </summary>
        public override void Dispose()
        {
            //Inherit the method.
            base.Dispose();
        }
        /// <summary>
        /// Initialize the projectiles velocity.
        /// </summary>
        /// <param name="velocity">The velocity.</param>
        public void InitializeVelocity(Vector2 velocity)
        {
            //Apply the velocity as force to the projectile.
            Bodies[0].ApplyForce(velocity);
        }
        /// <summary>
        /// Calculate the velocity of the projectile.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="speed">The speed.</param>
        /// <returns>The calculated velocity.</returns>
        public Vector2 CalculateVelocity(Vector2 direction, float speed)
        {
            //Return the calculated velocity.
            return (Vector2.Multiply(direction, (speed / System.UpdateSpeed)));
        }
        /// <summary>
        /// A chance to decide some extra effect when collision occurs is given here.
        /// This can vary from the damage from a bullet to picking up objects in the game.
        /// </summary>
        /// <param name="geom1">The first geom that collides.</param>
        /// <param name="geom2">The second geom that collides.</param>
        /// <param name="contactList">The list of contacts that tell the exact tale of the collision.</param>
        /// <returns>True.</returns>
        public override bool OnCollide(Geom geom1, Geom geom2, ContactList contactList)
        {
            //Set the dispose timer.
            disposeTimer = 15;

            //Return true.
            return true;
        }
        #endregion
    }
}
*/