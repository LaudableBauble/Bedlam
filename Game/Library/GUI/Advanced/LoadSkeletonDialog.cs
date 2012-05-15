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
    public class LoadSkeletonDialog : Form
    {
        #region Fields
        private List _List;
        private Button _Button;
        private float _Border;
        private List<string> _Skeletons;
        private int _SelectedIndex;

        public delegate void SkeletonLoadedHandler(object obj, SkeletonEventArgs e);
        public event SkeletonLoadedHandler SkeletonLoaded;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a load skeleton dialog.
        /// </summary>
        /// <param name="gui">The GUI that this dialog will be a part of.</param>
        /// <param name="position">The position of this dialog.</param>
        /// <param name="height">The height of this dialog.</param>
        /// <param name="width">The width of this dialog.</param>
        public LoadSkeletonDialog(GraphicalUserInterface gui, Vector2 position, float width, float height)
            : base(gui, position, width, height) { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the load skeleton dialog.
        /// </summary>
        /// <param name="gui">The GUI that this dialog will be a part of.</param>
        /// <param name="position">The position of this dialog.</param>
        /// <param name="height">The height of this dialog.</param>
        /// <param name="width">The width of this dialog.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Layout.IsEnabled = false;
            _Border = 5;
            _SelectedIndex = -1;
            _Skeletons = new List<string>();
            _List = new List(GUI, new Vector2((position.X + _Border), (position.Y + _Border)), (Width - (2 * _Border)), (Height - 35 - (2 * _Border)));
            _List.CellStyle = Enums.CellStyle.Fixed;
            _Button = new Button(GUI, new Vector2((position.X + ((Width / 2) - 25)), (position.Y + (Height - 30 - _Border))), 50, 30);
            _Button.CellStyle = Enums.CellStyle.Fixed;

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

            //Clear the lists to prevent duplicates.
            _Skeletons.Clear();
            _List.Clear();

            //Load the list with items.
            foreach (string a in Directory.GetFiles(GUI.ContentManager.RootDirectory, "*.skel", SearchOption.AllDirectories).ToList<string>())
            {
                //Save all animations' names.
                _Skeletons.Add(a.Replace(@"Content\", "").Replace(".skel", ""));
                //Add the list item.
                _List.AddItem();
                (_List[_List.Items.Count - 1] as LabelListItem).Label.Text = a.Replace(".skel", "").Substring((a.LastIndexOf('\\') + 1));
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
        /// Let the world know that a skeleton has been loaded.
        /// </summary>
        /// <param name="fileName">The file name of the loaded skeleton.</param>
        protected virtual void SkeletonLoadedInvoke(string fileName)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (SkeletonLoaded != null) { SkeletonLoaded(this, new SkeletonEventArgs(fileName)); }
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
            //If a skeleton hasbeen selected for loading, invoke the event.
            if (_SelectedIndex != -1) { SkeletonLoadedInvoke(_Skeletons[_SelectedIndex]); }
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
