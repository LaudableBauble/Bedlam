using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Library.Infrastructure;
using Game.Screens;

namespace Game
{
    /// <summary>
    /// This is the main type for your game. This is where the main game loop is performed.
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Fields
        //The GraphicsDevice Manager.
        GraphicsDeviceManager graphics;
        //The SpriteBatch.
        SpriteBatch spriteBatch;
        //The Screen Manager.
        ScreenManager screenManager;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the main constructor to the game.
        /// </summary>
        public Main()
        {
            //Create a new GraphicsDeviceManager to this game.
            graphics = new GraphicsDeviceManager(this);
            //Set the GraphicsProfile to Reach.
            graphics.GraphicsProfile = GraphicsProfile.Reach;
            //Set the Content root directory to the chosen one.
            Content.RootDirectory = "Content";

            //Lots of safety display and graphics controlling.
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Window.AllowUserResizing = false;
            IsFixedTimeStep = true;

            //Check if the graphics card can handle the game, oterhwise use the reference device.
            if (!GraphicsAdapter.DefaultAdapter.IsProfileSupported(graphics.GraphicsProfile))
            {
                //Use the reference device.
                GraphicsAdapter.UseReferenceDevice = true;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Create the screen manager component.
            screenManager = new ScreenManager(this);
            //Add the component.
            Components.Add(screenManager);
            //Add a frame counter.
            Components.Add(new FrameRateCounter(this));

            //Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new MainMenuScreen());

            //Jump to the a screen. NOTE: For simplicity's sake.
            LoadingScreen.Load(screenManager, true, new LevelEditorScreen());

            base.Initialize();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() { }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Update the game.
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Set the background color to CornFlower Blue.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw the game.
            base.Draw(gameTime);
        }
        #endregion
    }
}
