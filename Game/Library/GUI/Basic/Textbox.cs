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

using Library.Infrastructure;

namespace Library.GUI.Basic
{
    /// <summary>
    /// Textboxes are used to collect written data from the user.
    /// </summary>
    public class Textbox : Component
    {
        #region Fields
        private string _Text;
        private SpriteFont _Font;
        private int _TextStart;
        private int _VisibleTextLength;
        private int _MarkerIndex;
        private char _MarkerCharacter;
        private Vector2 _MarkerPosition;
        private Dictionary<Keys, TimeSpan> _UsedKeys;
        private List<Keys> _FastRepeatKeys;
        private TimeSpan _KeyRepeatTime;
        private TimeSpan _FastKeyRepeatTime;
        private TimeSpan _TotalElapsedTime;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a textbox.
        /// </summary>
        /// <param name="gui">The GUI that this textbox will be a part of.</param>
        /// <param name="position">The position of this textbox.</param>
        /// <param name="height">The height of this textbox.</param>
        /// <param name="width">The width of this textbox.</param>
        public Textbox(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the textbox.
        /// </summary>
        /// <param name="gui">The GUI that this textbox will be a part of.</param>
        /// <param name="position">The position of this textbox.</param>
        /// <param name="height">The height of this textbox.</param>
        /// <param name="width">The width of this textbox.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Text = "";
            _TextStart = 0;
            _VisibleTextLength = 0;
            _MarkerIndex = 0;
            _MarkerCharacter = '_';
            _MarkerPosition = Position;
            _UsedKeys = new Dictionary<Keys, TimeSpan>();
            _FastRepeatKeys = new List<Keys>();
            _KeyRepeatTime = new TimeSpan(0, 0, 0, 0, 500);
            _FastKeyRepeatTime = new TimeSpan(0, 0, 0, 0, 25);
            _TotalElapsedTime = TimeSpan.Zero;
        }
        /// <summary>
        /// Load the content of this textbox.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to use.</param>
        /// <param name="contentManager">The content manager to rely upon.</param>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create the textbox's texture and load the font.
            CreateTexture();
            _Font = GUI.ContentManager.Load<SpriteFont>("GameScreen/Fonts/diagnosticFont");
        }
        /// <summary>
        /// Update the textbox.
        /// </summary>
        /// <param name="gameTime">The time to adhere to.</param>
        public override void Update(GameTime gameTime)
        {
            //The inherited method.
            base.Update(gameTime);

            //Update the list of available keys.
            UpdateAvailableKeys(gameTime);
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public override void HandleInput(InputState input)
        {
            //The inherited method.
            base.HandleInput(input);

            //If the inputbox is active, write the input to the box.
            if (IsActive)
            {
                //If the inputbox is visible.
                if (IsVisible)
                {
                    //If the textbox has focus.
                    if (HasFocus)
                    {
                        //Delete text.
                        if (input.IsKeyDown(Keys.Delete) && !_UsedKeys.ContainsKey(Keys.Delete)) { DeleteText(true); }
                        else if (input.IsKeyDown(Keys.Back) && !_UsedKeys.ContainsKey(Keys.Back)) { DeleteText(false); }
                        //Unfocus the textbox.
                        else if (input.IsKeyDown(Keys.Enter) && !_UsedKeys.ContainsKey(Keys.Enter)) { FocusChangeInvoke(false); }

                        //If the marker should be moved.
                        if (input.IsKeyDown(Keys.Left) && !_UsedKeys.ContainsKey(Keys.Left)) { MoveMarkerLeft(); }
                        if (input.IsKeyDown(Keys.Right) && !_UsedKeys.ContainsKey(Keys.Right)) { MoveMarkerRight(); }

                        //If a key has been pressed, write to the textbox.
                        if (input.IsAnyKeyPress()) { InsertText(input, 0, _MarkerIndex); }
                    }
                }
            }
        }
        /// <summary>
        /// Draw the textbox.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //If the component isn't active or visible, stop here.
            if (!_IsActive || !_IsVisible) { return; }

            //Draw text.
            GUI.SpriteBatch.DrawString(_Font, CropText(), new Vector2((Position.X + 2), Position.Y), Color.White);

            //If the textbox is in focus, draw the selection marker.
            if (HasFocus) { GUI.SpriteBatch.DrawString(_Font, _MarkerCharacter.ToString(), _MarkerPosition, Color.White); }
        }

        /// <summary>
        /// Create a new background texture for the textbox.
        /// </summary>
        private void CreateTexture()
        {
            try
            {
                //The texture.
                Texture2D texture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, new Color(0, 0, 0, 155));

                //If there already exists a sprite, just switch a new texture to it.
                if (_Sprite.LastSprite() != null) { _Sprite.LastSprite().Texture = texture; }
                //Otherwise create a new sprite.
                else { AddSprite(texture); }
            }
            catch { }
        }
        /// <summary>
        /// Insert text into the textbox directly from an InputState buffer.
        /// </summary>
        /// <param name="text">The text to insert.</param>
        public void InsertText(string text)
        {
            //Control the string's legitimacy.
            if (text == null || text.Equals("")) { return; }

            //Insert the text at the correct position.
            _Text = _Text.Insert(_MarkerIndex, text);

            //Finally, move the marker accordingly.
            MoveMarker(text.Length);
        }
        /// <summary>
        /// Insert text into the textbox directly from an InputState buffer.
        /// </summary>
        /// <param name="input">The input state.</param>
        /// <param name="i">The specific input channel.</param>
        /// <param name="index">At which index to insert the text.</param>
        public void InsertText(InputState input, int i, int index)
        {
            //Get the user input.
            string text = GetTextInput(input, i);

            //Control the string's legitimacy.
            if (!text.Equals(""))
            {
                //Insert the text at the correct position.
                _Text = _Text.Insert(index, text);

                //Finally, move the marker accordingly.
                if (_MarkerIndex >= index) { MoveMarker(text.Length); }
            }
        }
        /// <summary>
        /// Get a string containing the writable keyboard input from last update.
        /// </summary>
        /// <param name="input">The input state.</param>
        /// <param name="i">The player index.</param>
        /// <returns>A string of writable keys.</returns>
        private string GetTextInput(InputState input, int i)
        {
            //The list of keys to return.
            string text = "";
            //Make the pressed keys more accessible.
            Keys[] keys = input.CurrentKeyboardStates[i].GetPressedKeys();

            //Loop through the list of recently pressed keys and see if they are eligible for writing.
            for (int a = 0; a < keys.Length; a++)
            {
                //If the key has not been pressed for a while, continue.
                if (!_UsedKeys.ContainsKey(keys[a]))
                {
                    //Add the key to the list of recently pressed keys.
                    _UsedKeys.Add(keys[a], _TotalElapsedTime);

                    //If an alphabetical key has been pressed.
                    if ((keys[a] >= Keys.A) && (keys[a] <= Keys.Z))
                    {
                        //If uppercase.
                        //TODO: No support for Caps Lock.
                        if (input.IsKeyDown(Keys.LeftShift)) { text += keys[a].ToString().ToUpper(); }
                        else { text += keys[a].ToString().ToLower(); }
                    }
                    //If a numerical key has been pressed.
                    else if ((keys[a] >= Keys.D0 && keys[a] <= Keys.D9))
                    {
                        //If left shift is currently down.
                        if (input.IsKeyDown(Keys.LeftShift))
                        {
                            //Shifted numerical keys.
                            switch (keys[a])
                            {
                                case Keys.D1: { text += "!"; break; }
                                case Keys.D2: { text += "\""; break; }
                                case Keys.D3: { text += "#"; break; }
                                case Keys.D4: { text += "¤"; break; }
                                case Keys.D5: { text += "%"; break; }
                                case Keys.D6: { text += "&"; break; }
                                case Keys.D7: { text += "/"; break; }
                                case Keys.D8: { text += "("; break; }
                                case Keys.D9: { text += ")"; break; }
                                case Keys.D0: { text += "="; break; }
                                default: { break; }
                            }
                        }
                        //If right alt is currently down.
                        else if (input.IsKeyDown(Keys.RightAlt))
                        {
                            //Non-shifted numerical symbol keys.
                            switch (keys[a])
                            {
                                case Keys.D1: { text += ""; break; }
                                case Keys.D2: { text += "@"; break; }
                                case Keys.D3: { text += "£"; break; }
                                case Keys.D4: { text += "$"; break; }
                                case Keys.D5: { text += "€"; break; }
                                case Keys.D6: { text += ""; break; }
                                case Keys.D7: { text += "{"; break; }
                                case Keys.D8: { text += "["; break; }
                                case Keys.D9: { text += "]"; break; }
                                case Keys.D0: { text += "}"; break; }
                                default: { break; }
                            }
                        }
                        //Use the regular numerical keys.
                        else { text += keys[a].ToString().Replace("D", string.Empty); }
                    }
                    //If another symbol key has been pressed.
                    else
                    {
                        //Non-shifted symbol keys.
                        switch (keys[a])
                        {
                            case (Keys.OemComma): { text += ","; break; }
                            case (Keys.OemPeriod): { text += "."; break; }
                            case (Keys.OemBackslash): { text += @"\"; break; }
                            case (Keys.Multiply): { text += "*"; break; }
                            case (Keys.OemMinus): { text += "-"; break; }
                            case (Keys.OemPlus): { text += "+"; break; }
                            case (Keys.OemSemicolon): { text += ";"; break; }
                            case (Keys.OemQuotes): { text += "'"; break; }
                            case (Keys.Space): { text += " "; break; }
                            case (Keys.OemQuestion): { text += "?"; break; }
                            default: { break; }
                        }
                    }
                }
            }

            //Return the input in string format.
            return text;
        }
        /// <summary>
        /// Update the list of available keys.
        /// </summary>
        /// <param name="gameTime">The time to adhere to.</param>
        private void UpdateAvailableKeys(GameTime gameTime)
        {
            //Update the total elapsed time.
            _TotalElapsedTime = gameTime.TotalGameTime;

            //Copy the list of used keys in order to modify it while iterating.
            Dictionary<Keys, TimeSpan> used = new Dictionary<Keys, TimeSpan>(_UsedKeys);

            //Update the list of recently used keys.
            foreach (KeyValuePair<Keys, TimeSpan> pair in used)
            {
                //If the key is not held down anymore, remove it from the fast repeat list.
                if (Keyboard.GetState().IsKeyUp(pair.Key)) { _UsedKeys.Remove(pair.Key); _FastRepeatKeys.Remove(pair.Key); }
                else
                {
                    //Remove the key if its time. The time depends on if the key belongs to the fast repeat list or not.
                    if (_FastRepeatKeys.Contains(pair.Key)) { if ((_TotalElapsedTime - pair.Value) > _FastKeyRepeatTime) { _UsedKeys.Remove(pair.Key); } }
                    else { if ((_TotalElapsedTime - pair.Value) > _KeyRepeatTime) { _UsedKeys.Remove(pair.Key); _FastRepeatKeys.Add(pair.Key); } }
                }
            }
        }
        /// <summary>
        /// Delete text from the textbox.
        /// </summary>
        /// <param name="deleteToRight">Which direction, relative to the current marker position, to delete a character.
        /// True tries to delete a character residing to the right of the marker and false will do the opposite.</param>
        public void DeleteText(bool deleteToRight)
        {
            //Delete a character.
            if (deleteToRight) { DeleteText(_MarkerIndex, 1); }
            else { DeleteText(_MarkerIndex - 1, 1); }
        }
        /// <summary>
        /// Delete text from the textbox.
        /// </summary>
        /// <param name="index">The index to start at.</param>
        /// <param name="count">The number of characters to delete.</param>
        public void DeleteText(int index, int count)
        {
            //Quit if the text is shorter than index.
            if ((_Text.Length <= index) && (index >= 0)) { return; }

            //Try and catch possible exceptions.
            try
            {
                //Delete the text specified.
                _Text = _Text.Remove(index, Math.Min(_Text.Length - index, count));
            }
            catch { }

            //Update the marker.
            UpdateMarker();
        }
        /// <summary>
        /// Move the selection marker right one step.
        /// </summary>
        public void MoveMarkerRight()
        {
            //Move the selection marker right.
            MoveMarker(1);
        }
        /// <summary>
        /// Move the selection marker left one step.
        /// </summary>
        public void MoveMarkerLeft()
        {
            //Move the selection marker left.
            MoveMarker(-1);
        }
        /// <summary>
        /// Move the selection marker to the end of the text.
        /// </summary>
        public void MoveMarkerToEnd()
        {
            //Move the selection marker to the end of the text.
            MoveMarker((_Text.Length - 1) - _MarkerIndex);
        }
        /// <summary>
        /// Move the selection marker to the start of the text.
        /// </summary>
        public void MoveMarkerToStart()
        {
            //Move the selection marker to the start of the text.
            MoveMarker(-_MarkerIndex);
        }
        /// <summary>
        /// Move the selection marker across the text and also update the text that is shown.
        /// </summary>
        /// <param name="steps">The increase in index of the selection marker.</param>
        private void MoveMarker(int steps)
        {
            //Move the marker the correct way.
            _MarkerIndex += steps;

            //Update the selection marker.
            UpdateMarker();
        }
        /// <summary>
        /// Update the selection marker and also the text that will be displayed.
        /// </summary>
        private void UpdateMarker()
        {
            //Clamp the marker position between the walls.
            ClampMarkerIndexPosition();

            //Fit and align the text.
            FitAndAlignText();

            //Update the selection marker position.
            _MarkerPosition = CalculateMarkerPosition();
        }
        /// <summary>
        /// Clamp the marker index position so that it stays within bounds.
        /// </summary>
        private void ClampMarkerIndexPosition()
        {
            //Clamp the marker position so that the text selection can't get out of control.
            _MarkerIndex = Math.Min(Math.Max(_MarkerIndex, 0), Math.Max(_Text.Length, 0));
        }
        /// <summary>
        /// Fit and align the text that will be published in the text box.
        /// </summary>
        private void FitAndAlignText()
        {
            //If no font has been created yet, call this off.
            if (_Font == null) { return; }

            //If the full text does not fit in the text box.
            if (_Font.MeasureString(_Text + _MarkerCharacter).X > Width)
            {
                //Calculate the new InputIndexStart position.
                if (_MarkerIndex < _TextStart) { _TextStart = Math.Min(_MarkerIndex, (_Text.Length - 1)); }
                else if (_MarkerIndex > ((_TextStart + (_VisibleTextLength - 1)) + 1))
                {
                    //Calculate the new InputIndexStart position.
                    _TextStart = Math.Max((Math.Min(_MarkerIndex, (_Text.Length - 1)) - (_VisibleTextLength - 1)), 0);
                }

                //Beginning at the InputIndexStart, loop through all chars in the string and stop when the string fits best.
                for (int i = 0; i <= ((_Text.Length - 1) - _TextStart); i++)
                {
                    //If the string does fit the box, save the new InputIndexLength position.
                    if (_Font.MeasureString(_Text.Substring(_TextStart, i) + _MarkerCharacter).X < Width) { _VisibleTextLength = (i + 1); }
                    //Otherwise, break this loop.
                    else { break; }
                }
            }
            //Otherwise use the beginning and end of the string as the InputIndexStart and InputIndexLength positions.
            else { _TextStart = 0; _VisibleTextLength = _Text.Length; }

            //Make certain that TextStart and VisibleTextLength do not overextend themselves outside the boundary of the string.
            _TextStart = Math.Min(Math.Max(_TextStart, 0), Math.Max(_Text.Length - 1, 0));
            _VisibleTextLength = Math.Min(Math.Max(_VisibleTextLength, 0), Math.Max(_Text.Length, 0));
        }
        /// <summary>
        /// Crop this textbox's text so that it will fit the given publication area.
        /// </summary>
        /// <returns>The cropped text.</returns>
        private string CropText()
        {
            return _Text.Substring(_TextStart, _VisibleTextLength);
        }
        /// <summary>
        /// Calculate the text selection marker's position.
        /// </summary>
        /// <returns>The marker's position.</returns>
        private Vector2 CalculateMarkerPosition()
        {
            //If no font has been created yet, call this off.
            if (_Font == null) { return Position; }

            //Calculate the selection marker position.
            return (new Vector2(Position.X + _Font.MeasureString((_Text + _MarkerCharacter).Substring(_TextStart, (_MarkerIndex - _TextStart))).X, Position.Y));
        }
        /// <summary>
        /// Tell the world that the position of this item has changed.
        /// </summary>
        /// <param name="position">The new position of the item.</param>
        protected override void PositionChangeInvoke(Vector2 position)
        {
            //Call the base method.
            base.PositionChangeInvoke(position);

            //Update the marker.
            UpdateMarker();
        }
        /// <summary>
        /// Change the text of this textbox.
        /// </summary>
        /// <param name="text">The new text.</param>
        protected void TextChangeInvoke(string text)
        {
            //If the text is the same, stop here.
            if (_Text.Equals(text)) { return; }

            //Change the text and reset the marker position.
            _Text = text;
            _TextStart = 0;
            FitAndAlignText();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The text that currently is displayed on this textbox.
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set { TextChangeInvoke(value); }
        }
        /// <summary>
        /// The font that is used by this textbox.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        /// <summary>
        /// The texture of the textbox.
        /// </summary>
        public Texture2D Texture
        {
            get { return _Sprite[0].Texture; }
            set { _Sprite[0].Texture = value; }
        }
        /// <summary>
        /// The index position that tells the textbox what part of the text to display in case the entire length of it does not fit its boundaries.
        /// </summary>
        public int TextStart
        {
            get { return _TextStart; }
            set { _TextStart = value; }
        }
        /// <summary>
        /// The length of the text to be displayed.
        /// </summary>
        public int VisibleTextLength
        {
            get { return _VisibleTextLength; }
            set { _VisibleTextLength = value; }
        }
        /// <summary>
        /// The index of the selection marker.
        /// </summary>
        public int MarkerIndex
        {
            get { return _MarkerIndex; }
            set { _MarkerIndex = value; }
        }
        /// <summary>
        /// The character that the selection marker uses.
        /// </summary>
        public char MarkerCharacter
        {
            get { return _MarkerCharacter; }
            set { _MarkerCharacter = value; }
        }
        /// <summary>
        /// The position of the selection marker.
        /// </summary>
        public Vector2 MarkerPosition
        {
            get { return _MarkerPosition; }
            set { _MarkerPosition = value; }
        }
        #endregion
    }
}
