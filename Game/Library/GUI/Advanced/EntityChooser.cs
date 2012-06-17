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
using Library.Enums;
using Library.Infrastructure;

namespace Library.GUI
{
    /// <summary>
    /// The item chooser lets the user choose an item from a list of item blueprints.
    /// </summary>
    public class ItemChooser : Form
    {
        #region Fields
        private Picturebox _PctbDisplay;
        private TreeView _TrvItems;
        private Slider _SldScale;
        private Button _BtnFinish;
        private float _Ratio;
        private float _Border;
        private ItemType _Type;

        public delegate void SpritePickedHandler(object obj, SpritePickedEventArgs e);
        public event SpritePickedHandler SpritePicked;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for an item chooser.
        /// </summary>
        /// <param name="gui">The GUI that this chooser will be a part of.</param>
        /// <param name="position">The position of this chooser.</param>
        /// <param name="height">The height of this chooser.</param>
        /// <param name="width">The width of this chooser.</param>
        public ItemChooser(GraphicalUserInterface gui, Vector2 position, float width, float height)
            : base(gui, position, width, height) { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the item chooser.
        /// </summary>
        /// <param name="gui">The GUI that this chooser will be a part of.</param>
        /// <param name="position">The position of this chooser.</param>
        /// <param name="height">The height of this chooser.</param>
        /// <param name="width">The width of this chooser.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Layout.IsEnabled = false;
            _Border = 5;
            _Ratio = .4f;
            _Type = ItemType.Item;
            _PctbDisplay = new Picturebox(GUI, new Vector2(position.X + _Border + (Width * _Ratio), position.Y + _Border), (Width * (1 - _Ratio)) - (2 * _Border) - 15,
                Height - 35 - (2 * _Border));
            _PctbDisplay.Origin = new Vector2(_PctbDisplay.Width / 2, _PctbDisplay.Height / 2);
            _TrvItems = new TreeView(GUI, new Vector2(position.X + _Border, position.Y + _Border), (Width * _Ratio) - _Border, Height - (2 * _Border));
            _SldScale = new Slider(GUI, new Vector2(position.X + Width - _Border - 10, position.Y + (Height - 85 - _Border)), SliderType.Vertical, 50);
            _SldScale.Value = 1;
            _SldScale.Maximum = 3;
            _BtnFinish = new Button(GUI, new Vector2(position.X + (2 * (Width / 3) - 25), position.Y + (Height - 30 - _Border)), 50, 30);

            //Add the controls.
            AddItem(_PctbDisplay);
            AddItem(_TrvItems);
            AddItem(_SldScale);
            AddItem(_BtnFinish);

            //Hook up some events.
            _BtnFinish.MouseClick += OnDoneButtonClick;
            //_TrvItems.ItemSelect += OnListSelect;
            _SldScale.ValueChange += OnSliderChange;
        }
        /// <summary>
        /// Load the content of this sprite dialog.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Clear the lists to prevent duplicates.
            _TrvItems.Clear();

            //Create a root node.
            TreeNode root = _TrvItems.AddNode();

            //Populate the list with all items.
            Helper.PopulateTree(GUI.ContentManager.RootDirectory + "\\Items", root);
        }

        /// <summary>
        /// Update the position and bounds of all components located on the form.
        /// </summary>
        protected override void UpdateComponents()
        {
            _PctbDisplay.Position = new Vector2(_Position.X + _Border + (Width * _Ratio), _Position.Y + _Border);
            _PctbDisplay.Width = (Width * (1 - _Ratio)) - (2 * _Border) - 15;
            _PctbDisplay.Height = Height - 35 - (2 * _Border);
            _TrvItems.Position = new Vector2(_Position.X + _Border, _Position.Y + _Border);
            _TrvItems.Width = (Width * _Ratio) - _Border;
            _TrvItems.Height = Height - (2 * _Border);
            _SldScale.Position = new Vector2(_Position.X + Width - _Border - 10, _Position.Y + (Height - 85 - _Border));
            _BtnFinish.Position = new Vector2(_Position.X + (2 * (Width / 3) - 25), _Position.Y + (Height - 30 - _Border));
        }
        /// <summary>
        /// Let the world know that a sprite has been picked.
        /// </summary>
        /// <param name="name">The new sprite name.</param>
        /// <param name="origin">The origin of the sprite.</param>
        /// <param name="end">The end position. Used to calculate the length and rotation offset in regard to a bone.</param>
        protected virtual void SpritePickedInvoke(string name, Vector2 origin, Vector2 end)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (SpritePicked != null) { SpritePicked(this, new SpritePickedEventArgs(name, origin, end)); }
        }
        /// <summary>
        /// The slider's value has changed.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnSliderChange(object obj, EventArgs e)
        {
            _PctbDisplay.Scale = _SldScale.Value;
        }
        /// <summary>
        /// A list item has been selected.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnListSelect(object obj, ListItemSelectEventArgs e)
        {
            //Try and load the new texture.
            _PctbDisplay.Picture = GUI.ContentManager.Load<Texture2D>(_Blueprints.Find(s => (s.Contains((e.Item as LabelListItem).Label.Text))));
            _PctbDisplay.Picture.Name = _Blueprints.Find(s => (s.Contains((e.Item as LabelListItem).Label.Text)));
        }
        /// <summary>
        /// The 'done' button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        public virtual void OnDoneButtonClick(object obj, MouseClickEventArgs e)
        {
            //Invoke the sprite picked event.
            //SpritePickedInvoke(_PctbDisplay.Name, CalculateSpriteOrigin(), CalculateSpriteEnd());
            //Invoke the dispose event.
            DisposeInvoke();
        }
        #endregion

        #region Properties
        #endregion
    }
}
