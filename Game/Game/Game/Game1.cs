using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.DebugViews;
using Library;
using Library.Core;
using Library.Entities;
using Library.Factories;
using Library.Infrastructure;
using System;

namespace Game
{
    /// <summary>
    /// This is a temporary debug class that tests the Farseer Physics Engine. Masses over ~10 results in weird and springy behaviour.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private SpriteFont _font;

        private Camera2D _Camera;
        private Level _Level;
        private World _World;

        private Body box;

        // Simple camera controls
        private Matrix _view;
        private Matrix _projection;
        private Vector2 _screenCenter;
        private DebugViewXNA _debugViewLevel;
        private DebugViewXNA _debugView;

        // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
        // 1 meters equals 64 pixels here
        // (Objects should be scaled to be between 0.1 and 10 meters in size)
        private const float MeterInPixels = 64f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize camera controls
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2, _graphics.GraphicsDevice.Viewport.Height / 2);
            _Camera = new Camera2D(Window.ClientBounds, new Rectangle(0, 0, 2000, 2000));
            _Camera.Position = _screenCenter;

            _batch = new SpriteBatch(_graphics.GraphicsDevice);
            _font = Content.Load<SpriteFont>(@"GameScreen/Fonts/diagnosticFont");
            _World = new World(new Vector2(0, 20));

            _Level = new Level("Level 1", _Camera);
            _Level.LoadContent(_graphics.GraphicsDevice, Content);
            Layer layer = _Level.AddLayer("Layer 1", Vector2.One);

            Box box1 = Factory.Instance.AddBox(layer, "Ground", @"General/Textures/FrozenMetalGroundV1[1]", new Vector2(800, 700), 937, 32);
            box1.Parts[0].Body.BodyType = FarseerPhysics.Dynamics.BodyType.Static;

            Body ground = BodyFactory.CreateRectangle(_World, 937 / MeterInPixels, 32 / MeterInPixels, 1);
            ground.BodyType = BodyType.Static;
            ground.Position = new Vector2(800, 700) / MeterInPixels;
            ground.Friction = .5f;

            for (int i = 0; i <= 5; i++)
            {
                Box box2 = Factory.Instance.AddBox(layer, "Box", @"General/Textures/BlueBoxV1[1]", new Vector2(500 + 50 * i, 50), 26, 27);
                box2.Parts[0].Body.Restitution = .1f * i;
                box2.Parts[0].Body.Mass = 1 + 10 * i;
            }

            box = BodyFactory.CreateRectangle(_World, 26 / MeterInPixels, 27 / MeterInPixels, 1);
            box.BodyType = BodyType.Dynamic;
            box.Restitution = .5f;
            //box.Mass = 51;
            box.Position = new Vector2(900, 50) / MeterInPixels;
            box.Position = (_screenCenter / MeterInPixels);

            _debugViewLevel = new DebugViewXNA(_Level.World);
            _debugViewLevel.Enabled = true;
            _debugViewLevel.LoadContent(_graphics.GraphicsDevice, Content);

            _debugView = new DebugViewXNA(_World);
            _debugView.Enabled = true;
            _debugView.LoadContent(_graphics.GraphicsDevice, Content);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();

            //The camera.
            _projection = _Camera.Projection;
            _view = _Camera.TransformSimulationMatrix();
            //We update the world
            _Level.Update(gameTime);
            _World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            // We make it possible to rotate the circle body
            if (state.IsKeyDown(Keys.A))
                box.ApplyLinearImpulse(new Vector2(-.01f, 0));

            if (state.IsKeyDown(Keys.D))
                box.ApplyLinearImpulse(new Vector2(.01f, 0));

            if (state.IsKeyDown(Keys.W) && _oldKeyState.IsKeyUp(Keys.W))
                box.ApplyLinearImpulse(new Vector2(0, -.5f));

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            _oldKeyState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix m = Matrix.CreateOrthographicOffCenter(0, _graphics.GraphicsDevice.Viewport.Width / MeterInPixels,
                _graphics.GraphicsDevice.Viewport.Height / MeterInPixels, 0, 0, 1);

            _debugViewLevel.RenderDebugData(ref _projection, ref _view);
            _debugView.RenderDebugData(ref m);

            _Level.Draw(_batch);

            base.Draw(gameTime);
        }
    }
}