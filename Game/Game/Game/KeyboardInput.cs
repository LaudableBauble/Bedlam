#region License
/*
 * File: KeyboardInput.cs
 * 
 * Copyright (C)2007 Nathan Levesque (http://rhysyngsun.spaces.live.com)
 * 
 * This file is supplied freely for use in commercial or non-commercial software
 * but without any warranty as to its use. By using this software you hold the
 * copyright owner free of any damages results from such use.
 * 
 * You are free to modify and distribute this file as you see fit, provided
 * this license is not removed.
 * 
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AuroraEngine.Input
{
    public delegate void KeyChange();
    public class KeyEvents
    {
        #region Events
        public event KeyChange OnKeyDown;
        public event KeyChange OnKeyUp;
        #endregion

        #region Methods
        public void KeyDown()
        {
            if ( OnKeyDown != null )
                OnKeyDown();
        }
        public void KeyUp()
        {
            if ( OnKeyUp != null )
                OnKeyUp();
        }
        #endregion
    }
    public static class KeyboardInput
    {
        #region Members
        private static Dictionary<Keys, KeyEvents> keyEventDictionary = new Dictionary<Keys, KeyEvents>();
        private static KeyboardState currentState = new KeyboardState();
        #endregion

        #region Methods
        public static void ConfirmKey( Keys key )
        {
            if ( !keyEventDictionary.ContainsKey( key ) )
                keyEventDictionary.Add( key, new KeyEvents() );
        }
        public static void Update()
        {
            KeyboardState previousState = currentState;
            currentState = Keyboard.GetState();
            if ( currentState == previousState )
                return;
            foreach(KeyValuePair<Keys, KeyEvents> key in keyEventDictionary)
            {
                if ( previousState.IsKeyDown( key.Key ) && currentState.IsKeyUp( key.Key ) )
                    key.Value.KeyUp();
                else if ( previousState.IsKeyUp( key.Key ) && currentState.IsKeyDown( key.Key ) )
                    key.Value.KeyDown();
            }
        }
        #endregion

        #region Properties
        public static Dictionary<Keys, KeyEvents> KeyEventDictionary
        {
            get { return keyEventDictionary; }
        }
        public static KeyboardState CurrentState
        {
            get { return currentState; }
        }
        #endregion
    }
}