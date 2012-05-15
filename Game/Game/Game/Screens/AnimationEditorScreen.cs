#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Library;
using Library.Animate;
using Library.Infrastructure;
using Library.Core;

using Game.Editors;
#endregion

namespace Game.Screens
{
    /// <summary>
    /// This screen implements the actual game logic by reusing components from before.
    /// </summary>
    class AnimationEditorScreen : GameScreen
    {
        #region Fields
        ContentManager content;
        AnimationEditor _Editor;
        #endregion

        #region Constructors
        /// <summary>
        /// The main constructor.
        /// </summary>
        public AnimationEditorScreen()
        {
            //Set the time it takes for the Screen to transition on and off.
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            //If the ContentManager isn't created, create one.
            if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");

            //Set the Mouse to visible.
            ScreenManager.Game.IsMouseVisible = true;

            //Intialize the animation controller.
            _Editor = new AnimationEditor(this);
            _Editor.LoadContent(content, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);

            // Once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }
        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            //Unload all content that has been used, to free up memory.
            content.Unload();
        }
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        /// <param name="otherScreenHasFocus">If an other screen has focus.</param>
        /// <param name="coveredByOtherScreen">If this screen is covered by another screen.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //Update the game by updating this screen.
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            //If this screen is active, update the System instance as well.
            if (IsActive) { _Editor.Update(gameTime); }
        }
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        /// <param name="input">The InputState instance that relays the state of input.</param>
        public override void HandleInput(InputState input)
        {
            //If the InputState is null, throw an exception.
            if (input == null) { throw new ArgumentNullException("input"); }

            //If the game should be paused, bring up the pause game screen.
            if (input.PauseGame) { ScreenManager.AddScreen(new PauseMenuScreen()); }
            //Otherwise let the System instance handle input as usual.
            else { _Editor.HandleInput(input); }
        }
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        /// <param name="gameTime">The Gametime instance.</param>
        public override void Draw(GameTime gameTime)
        {
            //Set the background color to blue.
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            //Begin the drawing.
            ScreenManager.SpriteBatch.Begin();
            //Draw the animation controller.
            _Editor.Draw(ScreenManager.SpriteBatch);
            //End the drawing.
            ScreenManager.SpriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) { ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha); }
        }
        #endregion
    }
}
