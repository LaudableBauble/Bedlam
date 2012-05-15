using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FarseerGames.FarseerPhysics.Dynamics;

namespace Halo
{
    public class InputHelper
    {
        private GamePadState _lastState;
        private GamePadState _currentState;
        private PlayerIndex _index = PlayerIndex.One;

        public void Update()
        {
            if (_lastState == null && _currentState == null)
            {
                _lastState = _currentState = GamePad.GetState(_index);
            }
            else
            {
                _lastState = _currentState;
                _currentState = GamePad.GetState(_index);
            }
        }

        public GamePadState LastState
        {
            get { return _lastState; }
            set { _lastState = value; }
        }

        public GamePadState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }

        public Vector2 LeftStick
        {
            get { return _currentState.ThumbSticks.Left; }
        }

        public Vector2 RightStick
        {
            get { return _currentState.ThumbSticks.Right; }
        }

        public Vector2 LeftStickVelocity
        {
            get { return _currentState.ThumbSticks.Left - _lastState.ThumbSticks.Left; }
        }

        public Vector2 RightStickVelocity
        {
            get { return _currentState.ThumbSticks.Right - _lastState.ThumbSticks.Right; }
        }

        public float LeftTrigger
        {
            get { return _currentState.Triggers.Left; }
        }

        public float RightTrigger
        {
            get { return _currentState.Triggers.Right; }
        }

        public float LeftTriggerVelocity
        {
            get { return _currentState.Triggers.Left - _lastState.Triggers.Left; }
        }

        public float RightTriggerVelocity
        {
            get { return _currentState.Triggers.Right - _lastState.Triggers.Right; }
        }

        public bool ExitRequested
        {
            get { return (IsCurPress(Buttons.Start) && IsCurPress(Buttons.Back)); }
        }

        public bool IsNewPress(Buttons button)
        {
            return (_lastState.IsButtonUp(button) && _currentState.IsButtonDown(button));
        }

        public bool IsCurPress(Buttons button)
        {
            return (_lastState.IsButtonDown(button) && _currentState.IsButtonDown(button));
        }

        public bool IsOldPress(Buttons button)
        {
            return (_lastState.IsButtonDown(button) && _currentState.IsButtonUp(button));
        }
    }
}