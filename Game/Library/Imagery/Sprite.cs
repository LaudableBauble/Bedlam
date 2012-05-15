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

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Library;

namespace Library.Imagery
{
    /// <summary>
    /// The sprite handles all image displaying.
    /// </summary>
    public class Sprite : IDisposable
    {
        #region Fields
        private SpriteCollection _SpriteCollection;
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
        private float _Scale;
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

        #region Constructor
        /// <summary>
        /// Create a sprite.
        /// </summary>
        /// <param name="spriteCollection">The collection of sprites this sprite is a part of.</param>
        /// <param name="name">The name of the frame.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="timePerFrame">The time each frame gets on the screen.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="offset">The offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        public Sprite(SpriteCollection spriteCollection, string name, Vector2 position, float timePerFrame, float scale, int depth, float rotation, float offset, string tag)
        {
            //Intialize the sprite.
            Initialize(spriteCollection, name, position, timePerFrame, scale, depth, rotation, offset, tag, null);
        }
        /// <summary>
        /// Create a sprite.
        /// </summary>
        /// <param name="spriteCollection">The collection of sprites this sprite is a part of.</param>
        /// <param name="name">The name of the frame.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="timePerFrame">The time each frame gets on the screen.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="offset">The offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <param name="origin">The origin of the sprite.</param>
        public Sprite(SpriteCollection spriteCollection, string name, Vector2 position, float timePerFrame, float scale, int depth, float rotation, float offset, string tag,
            Vector2 origin)
        {
            //Intialize the sprite.
            Initialize(spriteCollection, name, position, timePerFrame, scale, depth, rotation, offset, tag, origin);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the sprite.
        /// </summary>
        /// <param name="spriteCollection">The collection of sprites this sprite is a part of.</param>
        /// <param name="name">The name of the frame.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="timePerFrame">The time each frame gets on the screen.</param>
        /// <param name="scale">The scale of the sprite.</param>
        /// <param name="depth">The depth of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="offset">The offset of the sprite.</param>
        /// <param name="tag">The tag of the sprite, that is something to link it with.</param>
        /// <param name="origin">The origin of the sprite.</param>
        public void Initialize(SpriteCollection spriteCollection, string name, Vector2 position, float timePerFrame, float scale, int depth, float rotation, float offset,
            string tag, Vector2? origin)
        {
            //Initialize some variables.
            _SpriteCollection = spriteCollection;
            _Name = name;
            _Position = position;
            _TimePerFrame = timePerFrame;
            _Scale = scale;
            _Depth = depth;
            _Rotation = rotation;
            _PositionOffset = offset;
            _OrbitOffset = 0;
            _RotationOffset = 0;
            _Tag = tag;
            _Transparence = 1;
            _Visibility = Visibility.Visible;
            _Orientation = Orientation.Right;

            //Initialize some variables.
            if (_Frames == null) { _Frames = new List<Frame>(); }
            //Create the first frame. If an origin wasn't passed along, let the frame come up with one.
            if (!origin.HasValue) { AddFrame(_Name); }
            else { AddFrame(_Name, origin.Value); }
        }
        /// <summary>
        /// Load the texture for the sprite using the Content Pipeline.
        /// </summary>
        public void LoadContent()
        {
            //Update the bounds of all frames in the sprite. They may have been distorted due to the lack of a content manager.
            foreach (Frame frame in _Frames)
            {
                //The width and the height.
                frame.Width = _SpriteCollection.GetTextureBounds(frame.Name).Width;
                frame.Height = _SpriteCollection.GetTextureBounds(frame.Name).Height;
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
        /// Update the sprite and all its frames.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        /// <param name="position">The position of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        public void Update(GameTime gameTime, Vector2 position, float rotation)
        {
            //Update the Frames.
            if (_EnableAnimation) { UpdateFrame(gameTime); }
            //Update the sprite's position.
            _Position = Helper.CalculateOrbitPosition(position, AddAngles(rotation, _OrbitOffset), _PositionOffset);
            //Update the sprite's rotation.
            _Rotation = AddAngles(rotation, _RotationOffset);
        }
        /// <summary>
        /// Draw the sprite and its current frame to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //If the Sprite is visible.
            if (_Visibility == Visibility.Visible)
            {
                //The Sprite Effect, facing right.
                SpriteEffects spriteEffects = SpriteEffects.None;

                //The sprite faces left.
                if (_Orientation == Orientation.Left) { spriteEffects = SpriteEffects.FlipHorizontally; }

                //Draw the sprite.
                spriteBatch.Draw(_Texture, _Position, null, Color.White * _Transparence, AddAngles(_Rotation, _RotationOffset), _Frames[_FrameIndex].Origin,
                    _Scale, spriteEffects, 0);
            }
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
        /// <param name="frameName">The name of the frame.</param>
        public void AddFrame(string frameName)
        {
            //Get the bounds of the frame.
            Rectangle rectangle = _SpriteCollection.GetTextureBounds(frameName);
            //Add the frame to the list of frames.
            _Frames.Add(new Frame(frameName, rectangle.Width, rectangle.Height));
        }
        /// <summary>
        /// Add a frame to the sprite.
        /// </summary>
        /// <param name="texture">The texture of the frame.</param>
        public void AddFrame(Texture2D texture)
        {
            //Get the bounds of the frame.
            Rectangle rectangle = _SpriteCollection.GetTextureBounds(texture);
            //Add the frame to the list of frames.
            _Frames.Add(new Frame(texture, rectangle.Width, rectangle.Height));
        }
        /// <summary>
        /// Add a frame to the sprite.
        /// </summary>
        /// <param name="frameName">The name of the frame.</param>
        /// <param name="origin">The origin of the frame.</param>
        public void AddFrame(string frameName, Vector2 origin)
        {
            //Get the bounds of the frame.
            Rectangle rectangle = _SpriteCollection.GetTextureBounds(frameName);
            //Add the frame to the list of frames.
            _Frames.Add(new Frame(frameName, rectangle.Width, rectangle.Height, origin));
        }
        /// <summary>
        /// Find a frame's index.
        /// </summary>
        /// <param name="frameName">The name of the frame.</param>
        /// <returns>The index of the frame.</returns>
        public int FindFrameIndex(string frameName)
        {
            //The frame to return.
            Frame frame = null;
            //Loop through the list of frames and find the one with the right name.
            foreach (Frame f in _Frames) { if (f.Name == frameName) { frame = f; } }

            //Return it.
            return (_Frames.IndexOf(frame));
        }
        /// <summary>
        /// Delete a frame from the sprite.
        /// </summary>
        /// <param name="frameName">The name of the frame.</param>
        public void DeleteFrame(string frameName)
        {
            //Delete the frame at the correct location.
            _Frames.RemoveAt(FindFrameIndex(frameName));
        }
        /// <summary>
        /// Load a frame's texture.
        /// </summary>
        public void LoadFrame()
        {
            //If a frame has a texture already stored on its premises, load that texture.
            if (_Frames[_FrameIndex].Texture != null) { _Texture = _Frames[_FrameIndex].Texture; }
            //Otherwise load one by using the name of the frame.
            else { _Texture = _SpriteCollection.ContentManager.Load<Texture2D>(_Frames[_FrameIndex].Name); }

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

        /// <summary>
        /// Add two angles.
        /// </summary>
        /// <param name="radian1">The first angle to add.</param>
        /// <param name="radian2">The second angle to add.</param>
        /// <returns>The angle sum.</returns>
        public float AddAngles(float radian1, float radian2)
        {
            //Add the angles together.
            float addResult = radian1 + radian2;
            //Check if the sum of the angles has overreached a full lap, aka two PI, and if so fix it.
            if (addResult > (Math.PI * 2)) { return (addResult - ((float)Math.PI * 2)); }
            else { return addResult; }
        }
        /// <summary>
        /// Subtracts an angle from an angle.
        /// </summary>
        /// <param name="radian1">The angle to subtract from.</param>
        /// <param name="radian2">The angle to subtract.</param>
        /// <returns>The subtracted angle.</returns>
        public float SubtractAngles(float radian1, float radian2)
        {
            //Subtract the angles from eachother.
            float subtractResult = radian1 - radian2;
            //If the difference has exceeded a full lap, aka 0, fix that.
            if (subtractResult < 0) { return (subtractResult + ((float)Math.PI * 2)); }
            else { return subtractResult; }
        }

        #region IDisposable Members
        /// <summary>
        /// Dispose of the sprite instance.
        /// </summary>
        public void Dispose() { }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// The Sprite Collection.
        /// </summary>
        public SpriteCollection SpriteCollection
        {
            get { return (_SpriteCollection); }
            set { _SpriteCollection = value; }
        }
        /// <summary>
        /// The Content Manager.
        /// </summary>
        public ContentManager ContentManager
        {
            get { return (_SpriteCollection.ContentManager); }
            set { _SpriteCollection.ContentManager = value; }
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
        public float Scale
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
