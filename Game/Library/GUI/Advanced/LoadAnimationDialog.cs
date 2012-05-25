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
    public class LoadAnimationDialog : Form
    {
        #region Fields
        private List _List;
        private Button _Button;
        private float _Border;
        private List<string> _Animations;
        private int _SelectedIndex;

        public delegate void AnimationLoadedHandler(object obj, AnimationEventArgs e);
        public event AnimationLoadedHandler AnimationLoaded;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a load animation dialog.
        /// </summary>
        /// <param name="gui">The GUI that this dialog will be a part of.</param>
        /// <param name="position">The position of this dialog.</param>
        /// <param name="height">The height of this dialog.</param>
        /// <param name="width">The width of this dialog.</param>
        public LoadAnimationDialog(GraphicalUserInterface gui, Vector2 position, float width, float height)
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
            _SelectedIndex = -1;
            _Animations = new List<string>();
            _List = new List(GUI, new Vector2((position.X + _Border), (position.Y + _Border)), (Width - (2 * _Border)), (Height - 35 - (2 * _Border)));
            _Button = new Button(GUI, new Vector2((position.X + ((Width / 2) - 25)), (position.Y + (Height - 30 - _Border))), 50, 30);

            //Add the controls.
            AddItem(_List);
            AddItem(_Button);

            //Hook up some events.
            _Button.MouseClick += OnDoneButtonClick;
            _List.ItemSelect += OnListSelect;
        }
        /// <summary>
        /// Load the content of this dialog.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Clear the lists.
            _Animations.Clear();
            _List.Clear();

            //Load the list with items.
            foreach (string a in Directory.GetFiles(GUI.ContentManager.RootDirectory, "*.anim", SearchOption.AllDirectories).ToList<string>())
            {
                //Save all animations' names.
                _Animations.Add(a.Replace(@"Content\", "").Replace(".anim", ""));
                //Add the list item.
                _List.AddItem();
                (_List[_List.Items.Count - 1] as LabelListItem).Label.Text = a.Replace(".anim", "").Substring((a.LastIndexOf('\\') + 1));
            }
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
        /// Let the world know that an animation has been loaded.
        /// </summary>
        /// <param name="fileName">The file name of the loaded animation.</param>
        protected virtual void AnimationLoadedInvoke(string fileName)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (AnimationLoaded != null) { AnimationLoaded(this, new AnimationEventArgs(fileName)); }
        }
        /// <summary>
        /// A list item has been selected.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnListSelect(object obj, ListItemSelectEventArgs e)
        {
            //Pass along the selected item's index.
            _SelectedIndex = _List.Items.FindIndex(item => (item.Equals(e.Item)));
        }
        /// <summary>
        /// The 'done' button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        public virtual void OnDoneButtonClick(object obj, MouseClickEventArgs e)
        {
            //If an animation hasbeen selected for loading, invoke the event.
            if (_SelectedIndex != -1) { AnimationLoadedInvoke(_Animations[_SelectedIndex]); }
            //Invoke the dispose event.
            DisposeInvoke();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The label of this sprite dialog.
        /// </summary>
        public List List
        {
            get { return _List; }
            set { _List = value; }
        }
        /// <summary>
        /// The button of this sprite dialog.
        /// </summary>
        public Button DoneButton
        {
            get { return _Button; }
            set { _Button = value; }
        }
        #endregion
    }
}
