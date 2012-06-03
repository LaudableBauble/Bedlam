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

using Library.Enums;

namespace Library.Imagery
{
    /// <summary>
    /// A sprite is a collection of frames grouped together to form an animation.
    /// </summary>
    public class Sprite
    {
        #region Fields
        private SpriteManager _Manager;
        private Texture2D _Texture;
        private Vector2 _Position;
        private string _Name;
        private string _Tag;
        private List<Frame> _Frames;
        private int _FrameIndex;
        private float _TimePerFrame;
        private int _FrameStartIndex;
        private int _FrameEndIndex;
        private bool _EnableAnimation;
        private bool _AnimationDirection;
        private float _TotalElapsedTime;
        private float _Rotation;
        private Vector2 _Scale;
        private int _Depth;
        private float _PositionOffset;
        private float _OrbitOffset;
        private float _RotationOffset;
        private float _Transparence;
        private Visibility _Visibility;
        private Orientation _Orientation;

        public delegate void BoundsChangedHandler(object obj, BoundsChangedEventArgs e);
        public delegate void FrameChangedHandler(object obj, EventArgs e);
        public event BoundsChangedHandler BoundsChanged;
        public event FrameChangedHandler FrameChanged;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set a frame.
        /// </summary>
        /// <param name="index">The index of the frame.</param>
        /// <returns>The frame instance.</returns>
        public Frame this[int index]
        {
            get { return (_Frames[index]); }
            set { _Frames[index] = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor a sprite.
        /// </summary>
        /// <param name="manager">The manager this sprite is a part of.</param>
        /// <param name="name">The name of the sprite. Has nothing to do with the path of any sprite.</param>
        public Sprite(SpriteManager manager, string name)
        {
            Initialize(manager, name);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the sprite.
        /// </summary>
        /// <param name="manager">The manager this sprite is a part of.</param>
        /// <param name="name">The name of the frame.</param>
        private void Initialize(SpriteManager manager, string name)
        {
            //Initialize some variables.
            _Manager = manager;
            _Name = name;
            _Position = Vector2.Zero;
            _TimePerFrame = 1;
            _Scale = Vector2.One;
            _Depth = 0;
            _Rotation = 0;
            _PositionOffset = 0;
            _OrbitOffset = 0;
            _RotationOffset = 0;
            _Tag = "";
            _Transparence = 1;
            _Visibility = Visibility.Visible;
            _Orientation = Orientation.Right;
            _Frames = new List<Frame>();
        }
        /// <summary>
        /// Load the texture for the sprite using the Content Pipeline.
        /// </summary>
        public void LoadContent()
        {
            //Update the bounds of all frames in the sprite. They may have been distorted due to the lack of a valid content manager.
            foreach (Frame frame in _Frames)
            {
                //The width and the height.
                frame.Width = _Manager.GetTextureBounds(frame.Path).Width;
                frame.Height = _Manager.GetTextureBounds(frame.Path).Height;
            }

            //Load the first frame.
            LoadFrame();
        }
        /// <summary>
        /// Update the sprite and all its frames.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        public void Update(GameTime gameTime)
        {
            //Update the Frames.
            if (_EnableAnimation) { UpdateFrame(gameTime); }
        }
        /// <summary>
        /// Draw the sprite and its current frame to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //If there is no texture loaded or if the sprite is invisible, stop here.
            if (_Texture == null || _Visibility == Visibility.Invisible) { return; }

            //The Sprite Effect, facing right.
            SpriteEffects spriteEffects = SpriteEffects.None;

            //The sprite faces left.
            if (_Orientation == Orientation.Left) { spriteEffects = SpriteEffects.FlipHorizontally; }

            //Draw the sprite.
            spriteBatch.Draw(_Texture, _Position, null, Color.White * _Transparence, Helper.AddAngles(_Rotation, _RotationOffset), _Frames[_FrameIndex].Origin,
                _Scale, spriteEffects, 0);
        }

        /// <summary>
        /// Load a frame's texture.
        /// </summary>
        public void LoadFrame()
        {
            //If ths sprite has no frames, stop here.
            if (_Frames.Count == 0) { return; }

            //If a frame has a texture already stored on its premises, load that texture.
            if (_Frames[_FrameIndex].Texture != null) { _Texture = _Frames[_FrameIndex].Texture; }
            //Otherwise load one by using the name of the frame.
            else { _Texture = _Manager.ContentManager.Load<Texture2D>(_Frames[_FrameIndex].Path); }

            //The bounds of the sprite has changed, invoke the appropriate event.
            BoundsChangedInvoke();
        }
        /// <summary>
        /// Update the sprite's frames.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void UpdateFrame(GameTime gameTime)
        {
            //Get the Time since the last Update.
            _TotalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //If it's time to change SpriteFrame.
            if (_TotalElapsedTime > _TimePerFrame)
            {
                //If the animation is going forward.
                if (_AnimationDirection)
                {
                    //If the number of drawn frames are less than the total, change to the next.
                    if (_FrameIndex < _FrameEndIndex) { IncrementFrameIndex(); }
                    //Make the animation start over.
                    else { FrameChangedInvoke(_FrameStartIndex); }
                }
                //If the animation is going backwards.
                else
                {
                    //If the number of drawn frames are less than the total, change to the next.
                    if (_FrameIndex > _FrameStartIndex) { DecrementFrameIndex(); }
                    //Make the animation start over.
                    else { FrameChangedInvoke(_FrameEndIndex); }
                }

                //Substract the time per frame, to be certain the next frame is drawn in time.
                _TotalElapsedTime -= _TimePerFrame;
            }
        }
        /// <summary>
        /// Update the sprite's position and rotation.
        /// </summary>
        /// <param name="position">The new position of the sprite.</param>
        /// <param name="rotation">The new rotation of the sprite.</param>
        public void UpdateSprite(Vector2 position, float rotation)
        {
            //Update the sprite's position and rotation
            _Position = Helper.CalculateOrbitPosition(position, Helper.AddAngles(rotation, _OrbitOffset), _PositionOffset);
            _Rotation = Helper.AddAngles(rotation, _RotationOffset);
        }

        /// <summary>
        /// Increment the selected frame index and change the currently drawn frame.
        /// </summary>
        public void IncrementFrameIndex()
        {
            //Increment the frame index.
            FrameChangedInvoke(_FrameIndex + 1);
        }
        /// <summary>
        /// Decrement the selected frame index and change the currently drawn frame.
        /// </summary>
        public void DecrementFrameIndex()
        {
            //Increment the frame index.
            FrameChangedInvoke(_FrameIndex - 1);
        }
        /// <summary>
        /// Add a frame to the sprite.
        /// </summary>
        /// <param name="frame">The frame to add.</param>
        public void AddFrame(Frame frame)
        {
            //Add the frame to the list of frames.
            _Frames.Add(frame);

            //Increment the animation end index.
            _FrameEndIndex++;
        }
        /// <summary>
        /// Add a frame to the sprite.
        /// </summary>
        /// <param name="path">The path of the frame.</param>
        public void AddFrame(string path)
        {
            //Get the bounds of the frame.
            Rectangle rectangle = _Manager.GetTextureBounds(path);

            //Add the frame.
            AddFrame(new Frame(path, rectangle.Width, rectangle.Height));
        }
        /// <summary>
        /// Add a frame to the sprite.
        /// </summary>
        /// <param name="path">The path of the frame.</param>
        /// <param name="origin">The origin of the frame.</param>
        public void AddFrame(string path, Vector2 origin)
        {
            //Get the bounds of the frame.
            Rectangle rectangle = _Manager.GetTextureBounds(path);

            //Add the frame.
            AddFrame(new Frame(path, rectangle.Width, rectangle.Height, origin));
        }
        /// <summary>
        /// Add a frame to the sprite.
        /// </summary>
        /// <param name="texture">The texture of the frame.</param>
        public void AddFrame(Texture2D texture)
        {
            //Get the bounds of the frame.
            Rectangle rectangle = _Manager.GetTextureBounds(texture);

            //Add the frame.
            AddFrame(new Frame(texture, rectangle.Width, rectangle.Height));
        }
        /// <summary>
        /// Add a frame to the sprite.
        /// </summary>
        /// <param name="texture">The texture of the frame.</param>
        /// <param name="origin">The origin of the frame.</param>
        public void AddFrame(Texture2D texture, Vector2 origin)
        {
            //Get the bounds of the frame.
            Rectangle rectangle = _Manager.GetTextureBounds(texture);

            //Add the frame.
            AddFrame(new Frame(texture, rectangle.Width, rectangle.Height, origin));
        }
        /// <summary>
        /// Find a frame's index.
        /// </summary>
        /// <param name="frameName">The name of the frame.</param>
        /// <returns>The index of the frame.</returns>
        public int IndexOf(string frameName)
        {
            //The frame to return.
            Frame frame = null;

            //Loop through the list of frames and find the one with the right name.
            _Frames.ForEach(item => { if (item.Path == frameName) { frame = item; } });

            //Return it.
            return _Frames.IndexOf(frame);
        }
        /// <summary>
        /// Remove a frame from the sprite.
        /// </summary>
        /// <param name="frameName">The name of the frame to remove.</param>
        public void RemoveFrame(string frameName)
        {
            _Frames.RemoveAt(IndexOf(frameName));

            //Decrement the animation end index.
            _FrameEndIndex--;
        }
        /// <summary>
        /// Remove a frame from the sprite.
        /// </summary>
        /// <param name="index">The index of the frame to remove.</param>
        public void RemoveFrame(int index)
        {
            _Frames.RemoveAt(index);

            //Decrement the animation end index.
            _FrameEndIndex--;
        }
        /// <summary>
        /// The bounds of sprite has changed.
        /// </summary>
        private void BoundsChangedInvoke()
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (BoundsChanged != null) { BoundsChanged(this, new BoundsChangedEventArgs(_Frames[_FrameIndex].Width, _Frames[_FrameIndex].Height)); }
        }
        /// <summary>
        /// Change the current frame and invoke the event in one fellow swoop.
        /// </summary>
        /// <param name="frameIndex">The index of the new frame to be in front.</param>
        private void FrameChangedInvoke(int frameIndex)
        {
            //Change the frame.
            _FrameIndex = Math.Min(Math.Max(frameIndex, 0), Math.Max((_Frames.Count - 1), 0));
            //Load the new frame.
            LoadFrame();

            //If someone has hooked up a delegate to the event, fire it.
            if (FrameChanged != null) { FrameChanged(this, new EventArgs()); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The sprite's manager.
        /// </summary>
        public SpriteManager Manager
        {
            get { return _Manager; }
            set { _Manager = value; }
        }
        /// <summary>
        /// The sprite's content manager.
        /// </summary>
        public ContentManager ContentManager
        {
            get { return _Manager.ContentManager; }
            set { _Manager.ContentManager = value; }
        }
        /// <summary>
        /// The sprite texture.
        /// </summary>
        public Texture2D Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }
        /// <summary>
        /// The sprite position.
        /// </summary>
        public Vector2 Position
        {
            get { return (_Position); }
            set { _Position = value; }
        }
        /// <summary>
        /// The sprite's name.
        /// </summary>
        public string Name
        {
            get { return (_Name); }
            set { _Name = value; }
        }
        /// <summary>
        /// The tag of the body, used to get a specific sprite.
        /// </summary>
        public string Tag
        {
            get { return (_Tag); }
            set { _Tag = value; }
        }
        /// <summary>
        /// The List of frames.
        /// </summary>
        public List<Frame> Frames
        {
            get { return (_Frames); }
            set { _Frames = value; }
        }
        /// <summary>
        /// The number of frames.
        /// </summary>
        public int FrameCount
        {
            get { return (_Frames.Count); }
        }
        /// <summary>
        /// The current frame index.
        /// </summary>
        public int CurrentFrameIndex
        {
            get { return (_FrameIndex); }
            set { FrameChangedInvoke(value); }
        }
        /// <summary>
        /// The time every frame has on screen before the next appears.
        /// </summary>
        public float TimePerFrame
        {
            get { return (_TimePerFrame); }
            set { _TimePerFrame = value; }
        }
        /// <summary>
        /// The index that tells which frame the animation should start with.
        /// </summary>
        public int FrameStartIndex
        {
            get { return (_FrameStartIndex); }
            set { _FrameStartIndex = value; }
        }
        /// <summary>
        /// The index that tells which frame the animation should end with.
        /// </summary>
        public int FrameEndIndex
        {
            get { return (_FrameEndIndex); }
            set { _FrameEndIndex = value; }
        }
        /// <summary>
        /// If the sprite uses animations or not.
        /// </summary>
        public bool EnableAnimation
        {
            get { return (_EnableAnimation); }
            set { _EnableAnimation = value; }
        }
        /// <summary>
        /// The direction the animation plays, that is forward or backwards.
        /// </summary>
        public bool AnimationDirection
        {
            get { return (_AnimationDirection); }
            set { _AnimationDirection = value; }
        }
        /// <summary>
        /// The time that has gone past since the last frame update.
        /// </summary>
        public float TotalElapsedTime
        {
            get { return (_TotalElapsedTime); }
            set { _TotalElapsedTime = value; }
        }
        /// <summary>
        /// The sprite rotation.
        /// </summary>
        public float Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }
        /// <summary>
        /// The sprite scaling.
        /// </summary>
        public Vector2 Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }
        /// <summary>
        /// The depth the sprite is being drawn at.
        /// </summary>
        public int Depth
        {
            get { return _Depth; }
            set { _Depth = value; }
        }
        /// <summary>
        /// The sprite offset.
        /// </summary>
        public float PositionOffset
        {
            get { return (_PositionOffset); }
            set { _PositionOffset = value; }
        }
        /// <summary>
        /// The sprite orbit offset.
        /// </summary>
        public float OrbitOffset
        {
            get { return (_OrbitOffset); }
            set { _OrbitOffset = value; }
        }
        /// <summary>
        /// The offset at the sprite's rotation.
        /// </summary>
        public float RotationOffset
        {
            get { return (_RotationOffset); }
            set { _RotationOffset = value; }
        }
        /// <summary>
        /// The transparence of the sprite. The values lies between 0 and 1.
        /// </summary>
        public float Transparence
        {
            get { return _Transparence; }
            set { _Transparence = value; }
        }
        /// <summary>
        /// The sprite's state of visibility.
        /// </summary>
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { _Visibility = value; }
        }
        /// <summary>
        /// The sprite's state of orientation.
        /// </summary>
        public Orientation Orientation
        {
            get { return (_Orientation); }
            set { _Orientation = value; }
        }
        #endregion
    }
}