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
    public class SpriteDialog : Form
    {
        #region Fields
        private Picturebox _Picturebox;
        private List _List;
        private Slider _Slider;
        private Button _Button;
        private Vector2 _StartPosition;
        private Vector2 _EndPosition;
        private float _Ratio;
        private float _Border;
        private List<string> _Textures;
        private LineBrush _BoneBrush;
        private LineBrush _OriginBrush;
        private LineBrush _BoneEndBrush;

        public delegate void SpritePickedHandler(object obj, SpritePickedEventArgs e);
        public event SpritePickedHandler SpritePicked;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a sprite dialog.
        /// </summary>
        /// <param name="gui">The GUI that this sprite dialog will be a part of.</param>
        /// <param name="position">The position of this sprite dialog.</param>
        /// <param name="height">The height of this sprite dialog.</param>
        /// <param name="width">The width of this sprite dialog.</param>
        public SpriteDialog(GraphicalUserInterface gui, Vector2 position, float width, float height)
            : base(gui, position, width, height) { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the sprite dialog.
        /// </summary>
        /// <param name="gui">The GUI that this sprite dialog will be a part of.</param>
        /// <param name="position">The position of this sprite dialog.</param>
        /// <param name="height">The height of this sprite dialog.</param>
        /// <param name="width">The width of this sprite dialog.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Layout.IsEnabled = false;
            _Border = 5;
            _Ratio = .4f;
            _Textures = new List<string>();
            _StartPosition = Vector2.Zero;
            _EndPosition = Vector2.Zero;
            _Picturebox = new Picturebox(GUI, new Vector2((position.X + _Border + (Width * _Ratio)), (position.Y + _Border)), ((Width * (1 - _Ratio)) - (2 * _Border) - 15),
                (Height - 35 - (2 * _Border)));
            _Picturebox.Origin = new Vector2((_Picturebox.Width / 2), (_Picturebox.Height / 2));
            _List = new List(GUI, new Vector2((position.X + _Border), (position.Y + _Border)), ((Width * _Ratio) - _Border), (Height - (2 * _Border)));
            _Slider = new Slider(GUI, new Vector2((position.X + Width - _Border - 10), (position.Y + (Height - 85 - _Border))), SliderType.Vertical, 50);
            _Slider.Value = 1;
            _Slider.Maximum = 3;
            _Button = new Button(GUI, new Vector2((position.X + (2 * (Width / 3) - 25)), (position.Y + (Height - 30 - _Border))), 50, 30);

            //Add the controls.
            AddItem(_Picturebox);
            AddItem(_List);
            AddItem(_Slider);
            AddItem(_Button);

            //Hook up some events.
            _Picturebox.MouseClick += OnPictureboxClick;
            _Button.MouseClick += OnDoneButtonClick;
            _List.ItemSelect += OnListSelect;
            _Slider.ValueChange += OnSliderChange;
        }
        /// <summary>
        /// Load the content of this sprite dialog.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create the item's texture and set its position.
            _BoneBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Black);
            _OriginBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Green);
            _BoneEndBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Red);
            _BoneBrush.Load(GUI.GraphicsDevice);
            _OriginBrush.Load(GUI.GraphicsDevice);
            _BoneEndBrush.Load(GUI.GraphicsDevice);

            //Clear the lists to prevent duplicates.
            _Textures.Clear();
            _List.Clear();

            //Load the list with items.
            foreach (string t in Directory.GetFiles(GUI.ContentManager.RootDirectory, "*.xnb", SearchOption.AllDirectories).ToList<string>())
            {
                //Add a new label to the list, but only if the item in question is a texture.
                if (t.Contains("Textures"))
                {
                    //Remove the "Content\" and the ".xnb" from the name.
                    //t = t.Remove(0, 8).Remove(t.Length - 4, 4);
                    //Save all textures' names.
                    _Textures.Add(t.Replace(@"Content\", "").Replace(".xnb", ""));
                    //Add the list item.
                    _List.AddItem();
                    (_List[_List.Items.Count - 1] as LabelListItem).Label.Text = t.Replace(".xnb", "").Substring((t.LastIndexOf('\\') + 1));
                }
            }
        }
        /// <summary>
        /// Update the sprite dialog.
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
        /// Draw the sprite dialog.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //Draw the origin, bone start and end positions, but only if the length is greater than 0.
            if (Vector2.Distance(_StartPosition, _EndPosition) != 0)
            {
                //Draw the origin point.
                _OriginBrush.Draw(GUI.SpriteBatch, _StartPosition, Helper.CalculateOrbitPosition(_StartPosition, 0, 1));

                //If the bone end position isn't at (0,0).
                if (!Vector2.Equals(_EndPosition, Vector2.Zero))
                {
                    _BoneBrush.Draw(GUI.SpriteBatch, _StartPosition, _EndPosition);
                    _BoneEndBrush.Draw(GUI.SpriteBatch, _EndPosition, Helper.CalculateOrbitPosition(_EndPosition, 0, 1));
                }
            }
        }

        /// <summary>
        /// Update the position and bounds of all components located on the form.
        /// </summary>
        protected override void UpdateComponents()
        {
            _Picturebox.Position = new Vector2((_Position.X + _Border + (Width * _Ratio)), (_Position.Y + _Border));
            _Picturebox.Width = (Width * (1 - _Ratio)) - (2 * _Border) - 15;
            _Picturebox.Height = Height - 35 - (2 * _Border);
            _List.Position = new Vector2((_Position.X + _Border), (_Position.Y + _Border));
            _List.Width = ((Width * _Ratio) - _Border);
            _List.Height = Height - (2 * _Border);
            _Slider.Position = new Vector2((_Position.X + Width - _Border - 10), (_Position.Y + (Height - 85 - _Border)));
            _Button.Position = new Vector2((_Position.X + (2 * (Width / 3) - 25)), (_Position.Y + (Height - 30 - _Border)));
        }
        /// <summary>
        /// Calculate the chosen origin of the picturebox's picture.
        /// </summary>
        private Vector2 CalculateSpriteOrigin()
        {
            /* First calculate the position of the picture's top left corner, with the scaling included.
             * Then get the scaled start position and calculate the difference between these two vectors.
             * Finally unscale the difference and voila, the origin is left unscathed and up for grabs.*/

            //Return the origin of the picturebox's picture.
            return (Vector2.Divide(Vector2.Subtract(_StartPosition, Vector2.Subtract(Vector2.Add(_Picturebox.Position, _Picturebox.Origin),
                new Vector2(((_Picturebox.Picture.Width * _Picturebox.Scale) / 2), ((_Picturebox.Picture.Height * _Picturebox.Scale) / 2)))), _Picturebox.Scale));
        }
        /// <summary>
        /// Calculate the chosen end point of the picturebox's picture.
        /// </summary>
        private Vector2 CalculateSpriteEnd()
        {
            /* First calculate the position of the picture's top left corner, with the scaling included.
             * Then get the scaled end position and calculate the difference between these two vectors.
             * Finally unscale the difference and voila, the end point is left unscathed and up for grabs.*/

            //Return the origin of the picturebox's picture.
            return (Vector2.Divide(Vector2.Subtract(_EndPosition, Vector2.Subtract(Vector2.Add(_Picturebox.Position, _Picturebox.Origin),
                new Vector2(((_Picturebox.Picture.Width * _Picturebox.Scale) / 2), ((_Picturebox.Picture.Height * _Picturebox.Scale) / 2)))), _Picturebox.Scale));
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
            //Change the picturebox's scale.
            _Picturebox.Scale = _Slider.Value;
        }
        /// <summary>
        /// The picturebox has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnPictureboxClick(object obj, MouseClickEventArgs e)
        {
            //If the left button was pressed, update the origin.
            if (e.Button == MouseButton.Left) { _StartPosition = e.Position; }
            //If the right button was pressed, update the end position.
            else if (e.Button == MouseButton.Right) { _EndPosition = e.Position; }
        }
        /// <summary>
        /// A list item has been selected.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnListSelect(object obj, ListItemSelectEventArgs e)
        {
            //Try and load the new texture.
            _Picturebox.Picture = GUI.ContentManager.Load<Texture2D>(_Textures.Find(s => (s.Contains((e.Item as LabelListItem).Label.Text))));
            _Picturebox.Picture.Name = _Textures.Find(s => (s.Contains((e.Item as LabelListItem).Label.Text)));
        }
        /// <summary>
        /// The 'done' button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        public virtual void OnDoneButtonClick(object obj, MouseClickEventArgs e)
        {
            //Invoke the sprite picked event.
            SpritePickedInvoke(_Picturebox.Name, CalculateSpriteOrigin(), CalculateSpriteEnd());
            //Invoke the dispose event.
            DisposeInvoke();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The picturebox of this sprite dialog.
        /// </summary>
        public Picturebox Picturebox
        {
            get { return _Picturebox; }
            set { _Picturebox = value; }
        }
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
