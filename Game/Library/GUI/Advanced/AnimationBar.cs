using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using FarseerPhysics.DrawingSystem;

using Library.Animate;
using Library.Infrastructure;

namespace Library.GUI.Basic
{
    /// <summary>
    /// Animation bars are used to visualize the playing of animations while at the same time giving the user access to basic animation controls.
    /// </summary>
    public class AnimationBar : Component
    {
        #region Fields
        private Animation _Animation;
        private Texture2D _BackgroundTexture;
        private LineBrush _FrameDelimiter;
        private LineBrush _ProgressMarker;
        private Texture2D _SelectedFrameTexture;
        private Texture2D _KeyframeTexture;
        private int _CurrentFrame;
        private Vector2 _MarkerPosition;
        private int _SelectedFrameNumber;
        private int _FrameMarkerNumber;
        private float _FrameWidth;
        private Button _PlayButton;
        private bool _IsPlaying;
        private float _FramePositionStart;

        public delegate void PlayStateChangeHandler(object obj, EventArgs e);
        public delegate void SelectedFrameChangeHandler(object obj, SelectedFrameChangeEventArgs e);
        public event SelectedFrameChangeHandler SelectedFrameChange;
        public event PlayStateChangeHandler PlayStateChange;
        #endregion

        #region Constructor
        /// <summary>
        /// Create an animation bar.
        /// </summary>
        /// <param name="gui">The GUI that this animation bar will be a part of.</param>
        /// <param name="animation">The animation that this item will play through.</param>
        /// <param name="position">The position of this animation bar.</param>
        /// <param name="height">The height of this animation bar.</param>
        /// <param name="width">The width of this animation bar.</param>
        public AnimationBar(GraphicalUserInterface gui, Animation animation, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, animation, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the animation bar.
        /// </summary>
        /// <param name="gui">The GUI that this animation bar will be a part of.</param>
        /// <param name="animation">The animation that this item will play through.</param>
        /// <param name="position">The position of this animation bar.</param>
        /// <param name="height">The height of this animation bar.</param>
        /// <param name="width">The width of this animation bar.</param>
        public void Initialize(GraphicalUserInterface gui, Animation animation, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Animation = animation;
            _CurrentFrame = 0;
            _MarkerPosition = Vector2.Add(Position, new Vector2(25, 0));
            _SelectedFrameNumber = 0;
            _FrameMarkerNumber = 0;
            _FrameWidth = (100 * _Animation.FrameTime);
            _PlayButton = new Button(GUI, position, 20, 20);
            _IsPlaying = false;
            _FramePositionStart = (Position.X + 25);

            //Hook up some events.
            _PlayButton.MouseClick += OnPlayButtonClick;
            _Animation.KeyframeAdded += OnKeyframesChange;
            _Animation.KeyframeRemoved += OnKeyframesChange;
        }
        /// <summary>
        /// Load the content of this animation bar.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Load the brushes.
            _FrameDelimiter = new LineBrush(2, Color.Black);
            _ProgressMarker = new LineBrush(2, Color.Red);
            _FrameDelimiter.Load(GUI.GraphicsDevice);
            _ProgressMarker.Load(GUI.GraphicsDevice);

            //Create the textures and sprite fonts needed.
            _BackgroundTexture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, new Color(0, 0, 0, 155), Color.Black);
            _SelectedFrameTexture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)_FrameWidth, (int)Height, Color.DarkSeaGreen, Color.Black);
            _KeyframeTexture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)_FrameWidth, (int)Height, Color.CadetBlue, Color.Black);

            //Load the play button's content.
            _PlayButton.LoadContent();
        }
        /// <summary>
        /// Update the animation bar.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);

            //If the animation has changed frame, calculate the new frame start position.
            if (_Animation.CurrentFrameIndex != _FrameMarkerNumber)
            {
                //If the animation has started over, restart the counter.
                if (_Animation.CurrentFrameIndex == 0) { _FramePositionStart = (Position.X + 25); }
                //Else, add the frame time of the last frame to the counter.
                else { _FramePositionStart += _FrameWidth; }

                //Update the frame marker index counter.
                _FrameMarkerNumber = _Animation.CurrentFrameIndex;
            }

            //Calculate the animation marker position.
            _MarkerPosition.X = (_FramePositionStart + (100 * _Animation.TotalElapsedTime));

            //Update the play button.
            _PlayButton.Update(gametime);
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public override void HandleInput(InputState input)
        {
            //The inherited method.
            base.HandleInput(input);

            //If the animation bar is active, write the input to the box.
            if (IsActive)
            {
                //If the animation bar is visible.
                if (IsVisible)
                {
                    //If the animation bar has focus.
                    if (HasFocus)
                    {
                        //If the left mouse button has been pressed.
                        if (input.IsNewLeftMouseClick())
                        {
                            //If the user clicks somewhere else, defocus the animation bar.
                            if (!Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Position, Width, Height))
                            {
                                //Defocus this animation bar.
                                HasFocus = false;
                            }
                        }
                    }

                    //Let the play button handle input.
                    _PlayButton.HandleInput(input);
                }
            }
        }
        /// <summary>
        /// Draw the animation bar.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //Draw the animation strip.
            GUI.SpriteBatch.Draw(_BackgroundTexture, Vector2.Add(Position, new Vector2(25, 0)), Color.White);
            //Draw the play button.
            _PlayButton.Draw(spriteBatch);

            //Loop through all frames and divide them by drawn lines.
            for (int f = 0; f < _Animation.NumberOfFrames; f++)
            {
                //Calculate the x-coordinate position of each new frame cell.
                float x = ((Position.X + 25) + (f * _FrameWidth));

                //If the current frame is a selected, make it visible with a color alteration.
                if (f == _SelectedFrameNumber) { GUI.SpriteBatch.Draw(_SelectedFrameTexture, new Vector2(x, Position.Y), Color.White); }
                //If the current frame is a keyframe, make it visible with a color alteration.
                else if (_Animation.IsKeyframe(f)) { GUI.SpriteBatch.Draw(_KeyframeTexture, new Vector2(x, Position.Y), Color.White); }

                //Redo the calculations.
                x = ((Position.X + 25) + ((f + 1) * _FrameWidth));
                //Draw a frame delimiter.
                _FrameDelimiter.Draw(GUI.SpriteBatch, new Vector2(x, Position.Y), new Vector2(x, (Position.Y + Height)));
            }

            //Draw the animation marker.
            _ProgressMarker.Draw(GUI.SpriteBatch, _MarkerPosition, new Vector2(_MarkerPosition.X, _MarkerPosition.Y + Height));
        }

        /// <summary>
        /// Update the texture of the selected frame because chances are it has changed.
        /// </summary>
        private void UpdateSelectedFrame()
        {
            //Decide the width of the selected frame and create an appropriate texture.
            _SelectedFrameTexture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)_FrameWidth, (int)Height, Color.ForestGreen, Color.Black);
        }
        /// <summary>
        /// Set the animation to be played.
        /// </summary>
        /// <param name="animation">The animation to be played.</param>
        public void SetAnimation(Animation animation)
        {
            //First, unsubscribe from the past animation.
            _Animation.KeyframeAdded -= OnKeyframesChange;
            _Animation.KeyframeRemoved -= OnKeyframesChange;

            //Intialize some variables.
            _Animation = animation != null ? animation : new Animation();
            _CurrentFrame = 0;
            _SelectedFrameNumber = 0;
            _MarkerPosition = Vector2.Add(Position, new Vector2(25, 0));
            _FrameMarkerNumber = 0;
            _FrameWidth = (100 * _Animation.FrameTime);
            _IsPlaying = false;

            //Hook up some events.
            _Animation.KeyframeAdded += OnKeyframesChange;
            _Animation.KeyframeRemoved += OnKeyframesChange;
        }
        /// <summary>
        /// Tell the world that the bounds of this item has changed.
        /// </summary>
        /// <param name="width">The new width of the item.</param>
        /// <param name="height">The new height of the item.</param>
        protected override void BoundsChangeInvoke(float width, float height)
        {
            //Direct the call to the base event.
            base.BoundsChangeInvoke(width, height);
        }
        /// <summary>
        /// Tell the world that the item has been clicked.
        /// </summary>
        /// <param name="position">The position of the mouse at the time of the click.</param>
        /// <param name="button">Which mouse button that has been clicked.</param>
        protected override void MouseClickInvoke(Vector2 position, MouseButton button)
        {
            //Loop through all frames and determine which frame that has been pressed.
            for (int i = 0; i < _Animation.NumberOfFrames; i++)
            {
                //If it's the right one, pass along the frame number and update the selected frame texture. Finally break this operation.
                if (((i + 1) * _FrameWidth) > (position.X - (Position.X + 25))) { SelectedFrameChangeInvoke(i); break; }
            }

            //Direct the call to the base event.
            base.MouseClickInvoke(position, button);
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Direct the call to the base event.
            base.PositionChangeInvoke(position);
        }
        /// <summary>
        /// Tell the world that the selected frame has been changed.
        /// </summary>
        /// <param name="number">The new selected frame number.</param>
        protected virtual void SelectedFrameChangeInvoke(int number)
        {
            //The previously selected frame index.
            int past = _SelectedFrameNumber;
            //The new selected frame number.
            _SelectedFrameNumber = number;

            //If someone has hooked up a delegate to the event, fire it.
            if (SelectedFrameChange != null) { SelectedFrameChange(this, new SelectedFrameChangeEventArgs(past)); }
        }
        /// <summary>
        /// Tell the world that the playing state of this animation bar has changed.
        /// </summary>
        /// <param name="isPlaying">Whether the animation bar should be playing on not.</param>
        protected virtual void PlayStateChangeInvoke(bool isPlaying)
        {
            //The new playing state.
            _IsPlaying = isPlaying;
            _Animation.IsActive = _IsPlaying;

            //If someone has hooked up a delegate to the event, fire it.
            if (PlayStateChange != null) { PlayStateChange(this, new EventArgs()); }
        }
        /// <summary>
        /// The play button has been clicked on by a mouse.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlayButtonClick(object obj, MouseClickEventArgs e)
        {
            //Either turn on or off the animation.
            PlayStateChangeInvoke(!_IsPlaying);
        }
        /// <summary>
        /// When the number of keyframes in the animation has changed.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnKeyframesChange(object obj, EventArgs e)
        {
            //If the frame to be deleted is the last one, update the selected frame index.
            if (_Animation.NumberOfFrames <= _SelectedFrameNumber) { SelectedFrameChangeInvoke(_Animation.NumberOfFrames - 1); UpdateSelectedFrame(); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The animation.
        /// </summary>
        public Animation Animation
        {
            get { return _Animation; }
            set { SetAnimation(value); }
        }
        /// <summary>
        /// The texture of the label.
        /// </summary>
        public Texture2D Texture
        {
            get { return _BackgroundTexture; }
            set { _BackgroundTexture = value; }
        }
        /// <summary>
        /// Whether the animation is playing or not.
        /// </summary>
        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set { PlayStateChangeInvoke(value); }
        }
        /// <summary>
        /// The selected frame number.
        /// </summary>
        public int SelectedFrameNumber
        {
            get { return _SelectedFrameNumber; }
            set { SelectedFrameChangeInvoke(value); }
        }
        /// <summary>
        /// The frame currently animated.
        /// </summary>
        public int AnimatedFrame
        {
            get { return _FrameMarkerNumber; }
        }
        /// <summary>
        /// The current frame that is chosen.
        /// </summary>
        public int CurrentFrame
        {
            get { return _CurrentFrame; }
            set { _CurrentFrame = value; }
        }
        #endregion
    }
}
