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
using FarseerPhysics.DrawingSystem;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Mathematics;

using Library.Infrastructure;
using Library.Weaponry;

namespace Library
{
    /// <summary>
    /// The System object that controls everything happening in the game.
    /// Courtesy of the System object, the game loop finally gets some meat on its bones.
    /// </summary>
    public class System
    {
        #region Fields
        //The Screenmanager.
        public ScreenManager screenManager;
        //The PhysicsSimulator.
        public PhysicsSimulator PhysicsSimulator;
        //The ContentManager.
        public ContentManager contentManager;
        //The SpriteBatch.
        public SpriteBatch _SpriteBatch;
        //The DebugSystem.
        public DebugSystem debugSystem;
        /// <summary>
        /// The Camera.
        /// </summary>
        public Camera2D _Camera;

        //The Gravity.
        Vector2 Gravity;
        //UpdateSpeed.
        public float UpdateSpeed = 0.0f;
        //The number of Objects in the game.
        int ObjectCount;

        //The List of Objects.
        public List<Object> ObjectList = new List<Object>();
        //The List of Weapons.
        public List<Weapon> WeaponList = new List<Weapon>();
        //A debug counter.
        public int debugIndexCount = 0;

        //The Ground.
        public Ground Ground1 = new Ground("General/Textures/FrozenMetalGroundV1[1]", 937, 32, new Vector2(468.5f, 585));

        //The Spartans.
        public Spartan Spartan1 = new Spartan();
        public Spartan Spartan2 = new Spartan();

        //The weapons.
        public Weapon Weapon1 = WeaponFactory.CreateSMG(new Vector2(100, 300));
        public Weapon Weapon2 = WeaponFactory.CreateSMG(new Vector2(200, 300));
        public Weapon Weapon3 = WeaponFactory.CreateBattleRifle(new Vector2(300, 300));
        #endregion

        #region Constructors
        /// <summary>
        /// The main constructor for the System object.
        /// </summary>
        /// <param name="gravity">The gravity of the world the System will keep track of.</param>
        public System(Vector2 gravity)
        {
            //Store the gravity value.
            Gravity = gravity;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the System object.
        /// </summary>
        /// <param name="screen">Tells the System which ScreenManager to use.</param>
        public void Initialize(ScreenManager screen)
        {
            //Set the screenmanager.
            screenManager = screen;

            //Create the PhysicsSimulator.
            PhysicsSimulator = new PhysicsSimulator(Gravity);
            //Create and initialize the debugSystem.
            debugSystem = new DebugSystem(PhysicsSimulator);

            //Initialize the ground object.
            Ground1.Initialize(this);

            //Initialize the Spartans.
            Spartan1.Initialize(this);
            Spartan2.Initialize(this);
            Spartan2.BasePosition = new Vector2(500, 400);
            //Initialize the Weapons.
            Weapon1.Initialize(this);
            Weapon2.Initialize(this);
            Weapon3.Initialize(this);

            //Initialize the camera instance.
            _Camera = new Camera2D(screen.Game.Window.ClientBounds, new Rectangle(0, 0, 5000, 5000));
        }
        /// <summary>
        /// Load the Content that the System object will use.
        /// </summary>
        /// <param name="graphicsDevice">The Graphics Device.</param>
        /// <param name="content">The ContentManager.</param>
        /// <param name="spriteBatch">The SpriteBatch.</param>
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
        {
            contentManager = content;
            _SpriteBatch = spriteBatch;
            //Load the Content of the Debug System.
            debugSystem.LoadContent(graphicsDevice, content);
            //Set the Mouse to visible.
            screenManager.Game.IsMouseVisible = true;

            //The Ground.
            Ground1.LoadContent(content);

            //The Second Spartan.
            Spartan2.LoadContent(content);
            Spartan2.CanMove = false;
            Spartan2.Bodies[1].ApplyForce(new Vector2(0, -100));
            Spartan2.Geoms[0].CollisionGroup = 2;
            Spartan2.Geoms[1].CollisionGroup = 2;
            Spartan2.Geoms[2].CollisionGroup = 2;

            //The First Spartan.
            Spartan1.LoadContent(content);

            //The Weapons.
            Weapon1.LoadContent(content);
            Weapon2.LoadContent(content);
            Weapon3.LoadContent(content);
        }
        /// <summary>
        /// The Update function of the System object.
        /// </summary>
        /// <param name="gameTime">The GameTime object.</param>
        public void Update(GameTime gameTime)
        {
            //The Ground.
            Ground1.Update(gameTime);

            //The Spartans.
            Spartan1.Update(gameTime);
            Spartan2.Update(gameTime);
            //The Weapons.
            Weapon1.Update(gameTime);
            Weapon2.Update(gameTime);
            Weapon3.Update(gameTime);

            //Update the PhysicsSimulator.
            PhysicsSimulator.Update((gameTime.ElapsedGameTime.Milliseconds * UpdateSpeed));
            //Update the camera.
            _Camera.Position = Helper.TransformCameraPosition(Spartan1.Bodies[0].Position, _Camera.Viewport);
            _Camera.Update(gameTime);
            //Set the Game Title.
            screenManager.Game.Window.Title = (Spartan1.Health.ToString() + ", " + Spartan2.Health.ToString());
        }
        /// <summary>
        /// The method that handles all Input to the System object.
        /// </summary>
        /// <param name="input">The InputState object. Keeps track of all inputs.</param>
        public void HandleInput(InputState input)
        {
            //The Debug System.
            debugSystem.HandleInput(input);

            //The Spartans.
            Spartan1.HandleInput(input, 0);
            Spartan2.HandleInput(input, 0);
            //The Weapons.
            Weapon1.HandleInput(input, 0);
            Weapon2.HandleInput(input, 0);
            Weapon3.HandleInput(input, 0);
        }
        /// <summary>
        /// The Draw method of the System object.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //The DebugSystem.
            debugSystem.Draw(spriteBatch);

            //The Ground.
            Ground1.Draw(spriteBatch);

            //The Spartans.
            Spartan1.Draw(spriteBatch);
            Spartan2.Draw(spriteBatch);
            //The Weapons.
            Weapon1.Draw(spriteBatch);
            Weapon2.Draw(spriteBatch);
            Weapon3.Draw(spriteBatch);
        }
        /// <summary>
        /// Add some debug text.
        /// </summary>
        /// <param name="index">The index number.</param>
        /// <param name="str">The text.</param>
        /// <returns>The result.</returns>
        public int AddDebugText(int index, string str)
        {
            //Return the result.
            return (debugSystem.AddDebugText(index, str));
        }
        /// <summary>
        /// Get the Id of an Object.
        /// </summary>
        /// <param name="Object">The Object.</param>
        /// <returns>The Id.</returns>
        public int GetId(Object Object)
        {
            //Add the Object to the ObjectList.
            ObjectList.Add(Object);
            //Return the Index of it in the List.
            return (ObjectList.IndexOf(Object));
        }
        /// <summary>
        /// Get the Weapon Id.
        /// </summary>
        /// <param name="weapon">The Weapon Object.</param>
        /// <returns>The Weapon Id.</returns>
        public int GetWeaponId(Weapon weapon)
        {
            //Add the Weapon.
            WeaponList.Add(weapon);
            //Return the Id.
            return (WeaponList.IndexOf(weapon));
        }
        /// <summary>
        /// Get the debug index.
        /// </summary>
        /// <returns>The Index Count.</returns>
        public int GetDebugIndex()
        {
            //Increment the debugCounter.
            debugIndexCount++;
            //Return the index.
            return (debugIndexCount - 1);
        }
        #endregion

        #region Properties

        public PhysicsSimulator GetPhysicsEngine
        {
            get { return PhysicsSimulator; }
        }

        #endregion
    }
}
*/