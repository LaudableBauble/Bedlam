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
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Library.Imagery;

namespace Library
{
    /// <summary>
    /// This is the camera used in the game.
    /// </summary>
    public class Camera2D
    {
        #region Fields
        /// <summary>
        /// The camera viewport's position.
        /// </summary>
        private Vector2 _Position;
        /// <summary>
        /// The camera's zoom value.
        /// </summary>
        private float _ZoomValue;
        /// <summary>
        /// The camera's rotation.
        /// </summary>
        private float _Rotation;
        private Vector2 _Origin;
        /// <summary>
        /// The speed of the camera.
        /// </summary>
        private float _CameraSpeed;
        /// <summary>
        /// The maximum camera zoom.
        /// </summary>
        private float _MaxZoom;
        /// <summary>
        /// The minimum camera zoom.
        /// </summary>
        private float _MinZoom;
        /// <summary>
        /// The camera viewport's bounds.
        /// </summary>
        private Rectangle _Viewport;
        /// <summary>
        /// The world's bounds.
        /// </summary>
        private Rectangle _WorldRect;
        /// <summary>
        /// The camera transformation matrix.
        /// </summary>
        private Matrix _Transform;
        private Matrix _Projection;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the camera from which eyes the player will look upon the game world.
        /// </summary>
        /// <param name="viewport">The viewport that the camera will cover.</param>
        /// <param name="world">The game world's outer bounds.</param>
        public Camera2D(Rectangle viewport, Rectangle world)
        {
            //Initialize the camera.
            Initialize(viewport, world);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the camera.
        /// </summary>
        /// <param name="viewport">The viewport that the camera will cover.</param>
        /// <param name="world">The game world's outer bounds.</param>
        public virtual void Initialize(Rectangle viewport, Rectangle world)
        {
            //Initialize stuff.
            _Viewport = viewport;
            _WorldRect = world;
            _Transform = Matrix.Identity;
            _CameraSpeed = 10f;
            _Rotation = 0.0f;
            _Position = new Vector2(0, 0);
            _Origin = new Vector2(_Viewport.Width / 2, _Viewport.Height / 2);
            _MaxZoom = .6f;
            _MinZoom = 2;
            _Projection = Matrix.CreateOrthographicOffCenter(0, ConvertUnits.ToSimUnits(_Viewport.Width),
                ConvertUnits.ToSimUnits(_Viewport.Height), 0, 0, 1);

            //Set the camera's zoom.
            Zoom(1f);
        }
        /// <summary>
        /// Update the camera.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        public virtual void Update(GameTime gameTime)
        {
            //Update the camera's position, so that it keeps within bounds.
            Clamp();

            //Transform the matrix accordingly.
            UpdateCamera();
        }

        /// <summary>
        /// Move the camera.
        /// </summary>
        /// <param name="amount">The amount of movement.</param>
        public void MoveAmount(Vector2 amount)
        {
            //Set the position.
            _Position = Helper.ConstrainMovement(_Position, amount, new Vector2(_CameraSpeed, _CameraSpeed));
            //Clamp the position.
            Clamp();
            //Update the matrix.
            UpdateCamera();
        }
        /// <summary>
        /// Move the camera.
        /// </summary>
        /// <param name="direction">The direction of the movement.</param>
        public void Move(Vector2 direction)
        {
            //Set the position.
            MoveAmount(direction * _CameraSpeed);
        }
        /// <summary>
        /// Zoom the camera.
        /// </summary>
        /// <param name="amount">The amount that the camera will zoom in/out with.</param>
        public void Zoom(float amount)
        {
            //Zoom in accordingly.
            _ZoomValue += amount;
            //Make sure that the zoom will be within set range.
            _ZoomValue = MathHelper.Clamp(_ZoomValue, _MaxZoom, _MinZoom);
        }
        /// <summary>
        /// Return the camera's matrix transformation.
        /// </summary>
        /// <returns>The transformation matrix.</returns>
        public Matrix TransformMatrix()
        {
            //Create the transformation matrix.
            _Transform = Matrix.CreateTranslation(new Vector3(-_Position, 0)) *
                Matrix.CreateRotationZ(_Rotation) *
                Matrix.CreateScale(new Vector3(_ZoomValue, _ZoomValue, 1)) *
                Matrix.CreateTranslation(new Vector3(_Origin, 0));

            //Return the matrix.
            return _Transform;
        }
        /// <summary>
        /// Return the camera's simulation matrix transformation.
        /// </summary>
        /// <returns>The simulation transformation matrix.</returns>
        public Matrix TransformSimulationMatrix()
        {
            //Create the simulation transformation matrix. Perhaps this'll work? Not tested yet!
            /*return Matrix.CreateRotationZ(_Rotation) *
                Matrix.CreateScale(new Vector3(_ZoomValue, _ZoomValue, 1)) *
                Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(-_Position), 0)) *
                Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(_Origin / _ZoomValue), 0));*/

            //Create the simulation transformation matrix.
            return Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(-_Position), 0)) *
                Matrix.CreateRotationZ(_Rotation) *
                Matrix.CreateScale(new Vector3(_ZoomValue, _ZoomValue, 1)) *
                Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(_Origin), 0));
        }
        /// <summary>
        /// Update the camera's position, zoom and rotation.
        /// </summary>
        /// <param name="position">The new position of the camera.</param>
        /// <param name="zoomValue">The new zoom value of the camera.</param>
        /// <param name="rotation">The new rotation of the camera.</param>
        private void UpdateCamera(Vector2 position, float zoomValue, float rotation)
        {
            //Set the camera's position, zoom and rotation.
            _Position = position;
            _ZoomValue = zoomValue;
            _Rotation = rotation;

            //Update the transformation matrix.
            UpdateCamera();
        }
        /// <summary>
        /// Update the camera's matrix.
        /// </summary>
        private void UpdateCamera()
        {
            //Update the transformation matrix.
            TransformMatrix();
        }
        /// <summary>
        /// Clamp the camera within the viewport.
        /// </summary>
        private void Clamp()
        {
            //Update the camera's position, so that it keeps within bounds.
            if (_Position.X < ((_Viewport.Left + _Origin.X) / _ZoomValue)) { _Position.X = (_Viewport.Left + _Origin.X) / _ZoomValue; }
            if (_Position.Y < ((_Viewport.Top + _Origin.Y) / _ZoomValue)) { _Position.Y = (_Viewport.Top + _Origin.Y) / _ZoomValue; }
            if (_Position.X > (_WorldRect.Width - ((_Viewport.Right - _Origin.X) / _ZoomValue)))
            {
                _Position.X = _WorldRect.Width - ((_Viewport.Right - _Origin.X) / _ZoomValue);
            }
            if (_Position.Y > (_WorldRect.Height - ((_Viewport.Bottom - _Origin.Y) / _ZoomValue)))
            {
                _Position.Y = _WorldRect.Height - ((_Viewport.Bottom - _Origin.Y) / _ZoomValue);
            }
        }
        /// <summary>
        /// Convert screen coordinates to world coordinates.
        /// </summary>
        /// <param name="location">The location on screen.</param>
        /// <returns>The location in world coordinates.</returns>
        public Vector2 ConvertScreenToWorld(Vector2 location)
        {
            Vector3 t = new Vector3(location, 0);

            //t = _graphics.Viewport.Unproject(t, _projection, _view, Matrix.Identity);

            //return new Vector2(t.X, t.Y);

            //Transform the camera vector and return the converted coordinates.
            return Vector2.Transform(location, Matrix.Invert(_Transform));
        }
        /// <summary>
        /// Convert world coordinates to screen coordinates.
        /// </summary>
        /// <param name="location">The location in the world.</param>
        /// <returns>The location in screen coordinates.</returns>
        public Vector2 ConvertWorldToScreen(Vector2 location)
        {
            Vector3 t = new Vector3(location, 0);

            //t = _graphics.Viewport.Project(t, _projection, _view, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The camera viewport's position.
        /// </summary>
        public Vector2 Position
        {
            get { return (_Position); }
            set { UpdateCamera(value, _ZoomValue, _Rotation); }
        }
        /// <summary>
        /// The camera's zoom value.
        /// </summary>
        public float ZoomValue
        {
            get { return (_ZoomValue); }
            set { UpdateCamera(_Position, value, _Rotation); }
        }
        /// <summary>
        /// The camera's rotation.
        /// </summary>
        public float Rotation
        {
            get { return (_Rotation); }
            set { UpdateCamera(_Position, _ZoomValue, value); }
        }
        /// <summary>
        /// The camera viewport's origin.
        /// </summary>
        public Vector2 Origin
        {
            get { return (_Origin); }
            set { _Origin = value; ; }
        }
        /// <summary>
        /// The speed of the camera.
        /// </summary>
        public float CameraSpeed
        {
            get { return (_CameraSpeed); }
            set { _CameraSpeed = value; }
        }
        /// <summary>
        /// The maximum camera zoom.
        /// </summary>
        public float MaxZoom
        {
            get { return (_MaxZoom); }
            set { _MaxZoom = value; }
        }
        /// <summary>
        /// The minimum camera zoom.
        /// </summary>
        public float MinZoom
        {
            get { return (_MinZoom); }
            set { _MinZoom = value; }
        }
        /// <summary>
        /// The camera viewport's bounds.
        /// </summary>
        public Rectangle Viewport
        {
            get { return (_Viewport); }
            set { _Viewport = value; }
        }
        /// <summary>
        /// The world's bounds.
        /// </summary>
        public Rectangle WorldRect
        {
            get { return (_WorldRect); }
            set { _WorldRect = value; }
        }
        /// <summary>
        /// The camera transformation matrix.
        /// </summary>
        public Matrix Transform
        {
            get { return (_Transform); }
            set { _Transform = value; }
        }
        /// <summary>
        /// The camera's projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return (_Projection); }
            set { _Projection = value; }
        }
        #endregion
    }
}