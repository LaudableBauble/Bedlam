using System;
using System.IO;
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

using Library.GUI.Basic;
using Library.Infrastructure;

namespace Library.GUI
{
    /// <summary>
    /// The sprite dialog enables the user to pick a sprite. Besides the obvious it also lets you point out the origin for the sprite.
    /// </summary>
    public class SaveAnimationDialog : Form
    {
        #region Fields
        private Textbox _Textbox;
        private Button _Button;
        private float _Border;

        public delegate void AnimationSavedHandler(object obj, AnimationEventArgs e);
        public event AnimationSavedHandler AnimationSaved;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a save animation dialog.
        /// </summary>
        /// <param name="gui">The GUI that this dialog will be a part of.</param>
        /// <param name="position">The position of this dialog.</param>
        /// <param name="height">The height of this dialog.</param>
        /// <param name="width">The width of this dialog.</param>
        public SaveAnimationDialog(GraphicalUserInterface gui, Vector2 position, float width, float height)
            : base(gui, position, width, height) { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the load animation dialog.
        /// </summary>
        /// <param name="gui">The GUI that this dialog will be a part of.</param>
        /// <param name="position">The position of this dialog.</param>
        /// <param name="height">The height of this dialog.</param>
        /// <param name="width">The width of this dialog.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Layout.IsEnabled = false;
            _Border = 5;
            _Textbox = new Textbox(GUI, new Vector2((position.X + _Border), (position.Y + _Border)), (Width - (2 * _Border)), 15);
            _Button = new Button(GUI, new Vector2((position.X + ((Width / 2) - 25)), (position.Y + (Height - 30 - _Border))), 50, 30);

            //Add the controls.
            AddItem(_Textbox);
            AddItem(_Button);

            //Hook up some events.
            _Button.MouseClick += OnDoneButtonClick;
        }
        /// <summary>
        /// Load the content of this dialog.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the dialog.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public override void HandleInput(InputState input)
        {
            //The inherited method.
            base.HandleInput(input);

            //If the item is active.
            if (IsActive)
            {
                //If the item is visible.
                if (IsVisible)
                {
                    //If the item has focus.
                    if (HasFocus) { }
                }
            }
        }
        /// <summary>
        /// Draw the dialog.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Let the world know that an animation has been saved.
        /// </summary>
        /// <param name="fileName">The file name of the saved animation.</param>
        protected virtual void AnimationSavedInvoke(string fileName)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (AnimationSaved != null) { AnimationSaved(this, new AnimationEventArgs(fileName)); }
        }
        /// <summary>
        /// The 'done' button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        public virtual void OnDoneButtonClick(object obj, MouseClickEventArgs e)
        {
            //If an animation has been selected for loading, invoke the event.
            if (_Textbox.Text != null) { AnimationSavedInvoke(_Textbox.Text); }
            //Invoke the dispose event.
            DisposeInvoke();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The textbox of this dialog.
        /// </summary>
        public Textbox Textbox
        {
            get { return _Textbox; }
            set { _Textbox = value; }
        }
        /// <summary>
        /// The button of this dialog.
        /// </summary>
        public Button DoneButton
        {
            get { return _Button; }
            set { _Button = value; }
        }
        #endregion
    }
}
