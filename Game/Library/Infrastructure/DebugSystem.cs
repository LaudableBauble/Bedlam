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
using FarseerPhysics.DebugViews;
using FarseerPhysics.DrawingSystem;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Library.Infrastructure;

namespace Library.Infrastructure
{
    /// <summary>
    /// The system that manages all debugging in the game, preproduction-wise anyway.
    /// </summary>
    public class DebugSystem
    {
        #region Fields
        private World _World;
        private DebugViewXNA _DebugView;

        public bool _DebugViewEnabled;
        //The texture of the performance panel.
        public Texture2D PerformancePanelTexture;
        //The sprite font of the performance panel.
        public SpriteFont PerformancePanelSpriteFont;
        //The list of debug text to display.
        public List<string> debugText = new List<string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Create a debug system.
        /// </summary>
        /// <param name="world">The world simulator that will be debugged.</param>
        public DebugSystem(World world)
        {
            //Initialize the debug system.
            Initialize(world);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the Debug System.
        /// </summary>
        public void Initialize(World world)
        {
            //Save the world reference.
            _World = world;
            _DebugView = new DebugViewXNA(world);
            _DebugViewEnabled = false;
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="contentManager">The Content Manager.</param>
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            //Load the content of the PhysicsSimulatorView.
            _DebugView.LoadContent(graphicsDevice, contentManager);
            //Load all debug content.
            LoadDebug(graphicsDevice, contentManager);
        }
        /// <summary>
        /// Handle all input to the debug system.
        /// </summary>
        /// <param name="input">The input to consider.</param>
        public void HandleInput(InputState input)
        {
            if (input.IsNewKeyPress(Keys.F1)) { Debug(); }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                if (input.IsNewKeyPress(Keys.Q)) { _World.Gravity = new Vector2(0, 0); }
                if (input.IsNewKeyPress(Keys.W)) { _World.Gravity = new Vector2(1, 0); }
                if (input.IsNewKeyPress(Keys.E)) { _World.Gravity = new Vector2(10, 0); }
                if (input.IsNewKeyPress(Keys.R)) { _World.Gravity = new Vector2(100, 0); }
                if (input.IsNewKeyPress(Keys.T)) { _World.Gravity = new Vector2(1000, 0); }
            }
            else
            {
                if (input.IsNewKeyPress(Keys.Q)) { _World.Gravity = new Vector2(0, -10); }
                if (input.IsNewKeyPress(Keys.W)) { _World.Gravity = new Vector2(0, -1); }
                if (input.IsNewKeyPress(Keys.E)) { _World.Gravity = new Vector2(0, 1); }
                if (input.IsNewKeyPress(Keys.R)) { _World.Gravity = new Vector2(0, 10); }
                if (input.IsNewKeyPress(Keys.T)) { _World.Gravity = new Vector2(0, 100); }
            }

            /*if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Z)) { System.UpdateSpeed = 0.1f; }
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.X)) { System.UpdateSpeed = 0.01f; }
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.C)) { System.UpdateSpeed = 0.001f; }
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.V)) { System.UpdateSpeed = 0.0001f; }
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.B)) { System.UpdateSpeed = 0.0f; }*/
        }
        /// <summary>
        /// Draw all debug content.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="view">The camera matrix.</param>
        public void Draw(SpriteBatch spriteBatch, Matrix projection, Matrix view)
        {
            //Draw.
            DrawDebug(spriteBatch, projection, view, debugText);
        }

        /// <summary>
        /// Load the debug panel content.
        /// </summary>
        /// <param name="content">The Content Manager.</param>
        public void LoadDebug(GraphicsDevice graphicsDevice, ContentManager content)
        {
            //The Performance Panel Texture.
            PerformancePanelTexture = DrawingHelper.CreateRectangleTexture(graphicsDevice, 220, 150, new Color(0, 0, 0, 155));
            //The Performance Panel Sprite Font.
            PerformancePanelSpriteFont = content.Load<SpriteFont>("GameScreen/Fonts/diagnosticFont");
        }
        /// <summary>
        /// Draw the debug thingies.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="view">The camera matrix.</param>
        /// <param name="str">The list of text to display.</param>
        private void DrawDebug(SpriteBatch spriteBatch, Matrix projection, Matrix view, List<string> str)
        {
            //If the debugView is enabled.
            if (_DebugViewEnabled)
            {
                //Draw the debug view.
                _DebugView.RenderDebugData(ref projection, ref view);
                //Draw the Performance Panel.
                //spriteBatch.Draw(PerformancePanelTexture, new Vector2(500, 110), Color.White);
                //Draw the Debug Text.
                //DrawDebugText(spriteBatch, str);
            }
        }
        /// <summary>
        /// Draw the debug text.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch.</param>
        /// <param name="str">The list of text.</param>
        private void DrawDebugText(SpriteBatch spriteBatch, List<string> str)
        {
            //Loop through all texts to display.
            for (int i = 0; i < str.Count; i++)
            {
                //Draw the string.
                spriteBatch.DrawString(PerformancePanelSpriteFont, str[i],
                    new Vector2(510, (110 + (10 * (i + 1)))), Color.White);
            }
        }
        /// <summary>
        /// Add debug text.
        /// </summary>
        /// <param name="index">The index of  the text to add.</param>
        /// <param name="str">The text to add.</param>
        /// <returns>The index of the last item in the text list.</returns>
        public int AddDebugText(int index, string str)
        {
            //Insert the text at the given position in the list.
            debugText.Insert(index, str);
            //If the index is less than the max index, remove the item at the next index.
            if (index < (debugText.Count - 1)) { debugText.RemoveAt(index + 1); }

            //Return the highest index.
            return (debugText.Count - 1);
        }
        /// <summary>
        /// Enable or disable the debugging.
        /// </summary>
        /// <param name="enable">Enable or disable?</param>
        public void Debug(bool enable)
        {
            //Save the decision.
            _DebugViewEnabled = enable;

            //Turn everything either on or off.
            Settings.EnableDiagnostics = _DebugViewEnabled;

            //Append debug flags if enabled.
            if (_DebugViewEnabled)
            {
                _DebugView.AppendFlags(DebugViewFlags.DebugPanel);
                _DebugView.AppendFlags(DebugViewFlags.PerformanceGraph);
                _DebugView.AppendFlags(DebugViewFlags.Shape);
                _DebugView.AppendFlags(DebugViewFlags.AABB);
                _DebugView.AppendFlags(DebugViewFlags.PolygonPoints);
            }
            //Otherwise remove them flags.
            else
            {
                _DebugView.RemoveFlags(DebugViewFlags.DebugPanel);
                _DebugView.RemoveFlags(DebugViewFlags.PerformanceGraph);
                _DebugView.RemoveFlags(DebugViewFlags.Shape);
                _DebugView.RemoveFlags(DebugViewFlags.AABB);
                _DebugView.RemoveFlags(DebugViewFlags.PolygonPoints);
            }
            /*_DebugView.EnablePerformancePanelView = DebugViewEnabled;
            PhysicsSimulatorView.EnableAABBView = DebugViewEnabled;
            PhysicsSimulatorView.EnableContactView = DebugViewEnabled;
            PhysicsSimulatorView.EnableCoordinateAxisView = DebugViewEnabled;
            PhysicsSimulatorView.EnableEdgeView = DebugViewEnabled;
            PhysicsSimulatorView.EnablePinJointView = DebugViewEnabled;
            PhysicsSimulatorView.EnableRevoluteJointView = DebugViewEnabled;
            PhysicsSimulatorView.EnableVerticeView = DebugViewEnabled;*/
        }
        /// <summary>
        /// Enable or disable the debugging. If the debug is on, turn it off. If it is off, turn it on.
        /// </summary>
        public void Debug()
        {
            //Enable or disable debugging.
            Debug(!_DebugViewEnabled);
        }
        #endregion
    }
}
