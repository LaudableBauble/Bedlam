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
using Library.Animate;
using Library.Infrastructure;
using Library.Imagery;

namespace Library.GUI
{
    /// <summary>
    /// The edit bone dialog enables the user to create a bone with the desired properties and attach it to a skeleton.
    /// </summary>
    public class EditBoneDialog : Form
    {
        #region Fields
        private Picturebox _Picturebox;
        private Slider _Slider;
        private Label _NameLabel;
        private Label _ParentLabel;
        private Textbox _NameTextbox;
        private Textbox _ParentTextbox;
        private Button _AddSpriteButton;
        private Button _CloseButton;
        private Bone _Bone;
        private Sprite _BoneSprite;
        private float _Border;
        private float _Ratio;
        private Vector2 _StartPosition;
        private Vector2 _EndPosition;
        private LineBrush _BoneBrush;
        private LineBrush _OriginBrush;
        private LineBrush _BoneEndBrush;

        public delegate void BoneEditedHandler(object obj, BoneEventArgs e);
        public delegate void BoneSelectedHandler(object obj, BoneEventArgs e);
        public event BoneSelectedHandler BoneSelected;
        public event BoneEditedHandler BoneEdited;
        #endregion

        #region Constructor
        /// <summary>
        /// Create an edit bone dialog.
        /// </summary>
        /// <param name="gui">The GUI that this bone dialog will be a part of.</param>
        /// <param name="position">The position of this bone dialog.</param>
        /// <param name="height">The height of this bone dialog.</param>
        /// <param name="width">The width of this bone dialog.</param>
        public EditBoneDialog(GraphicalUserInterface gui, Vector2 position, float width, float height)
            : base(gui, position, width, height) { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the bone dialog.
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
            _Bone = new Bone();
            _BoneSprite = null;
            _Border = 5;
            _Ratio = .4f;
            _StartPosition = Vector2.Zero;
            _EndPosition = Vector2.Zero;

            //Create the necessary components.
            _Picturebox = new Picturebox(GUI, new Vector2((position.X + _Border + (Width * _Ratio)), (position.Y + _Border)),
                ((Width * (1 - _Ratio)) - (2 * _Border) - 15), (Height - 35 - (2 * _Border)));
            _Slider = new Slider(GUI, new Vector2((position.X + Width - _Border - 10), (position.Y + (Height - 85 - _Border))), SliderType.Vertical, 50);
            _NameLabel = new Label(GUI, Vector2.Add(Position, new Vector2(_Border, _Border)), 50, 15);
            _NameTextbox = new Textbox(GUI, Vector2.Add(_NameLabel.Position, new Vector2(_NameLabel.Width, 0)),
                ((Width * _Ratio) - (2 * _Border) - _NameLabel.Width), 15);
            _ParentLabel = new Label(GUI, Vector2.Add(_NameLabel.Position, new Vector2(0, 15)), 50, 15);
            _ParentTextbox = new Textbox(GUI, Vector2.Add(_ParentLabel.Position, new Vector2(_ParentLabel.Width, 0)),
                ((Width * _Ratio) - (2 * _Border) - _ParentLabel.Width), 15);
            _AddSpriteButton = new Button(GUI, Vector2.Add(_NameLabel.Position, new Vector2(10, 40)), 75, 30);
            _CloseButton = new Button(GUI, new Vector2((Position.X + ((Width / 2) - 25)), (Position.Y + (Height - 30 - _Border))), 50, 30);

            //Modify the components.
            _Picturebox.Origin = new Vector2((_Picturebox.Width / 2), (_Picturebox.Height / 2));
            _NameLabel.Text = "Name:";
            _ParentLabel.Text = "Parent ID:";
            _AddSpriteButton.Text = "Sprite";
            _Slider.Value = 1;
            _Slider.Maximum = 3;
            _CloseButton.Text = "Done";

            //Add the controls.
            AddItem(_Picturebox);
            AddItem(_Slider);
            AddItem(_NameLabel);
            AddItem(_NameTextbox);
            AddItem(_ParentLabel);
            AddItem(_ParentTextbox);
            AddItem(_AddSpriteButton);
            AddItem(_CloseButton);

            //Hook up to some events.
            _AddSpriteButton.MouseClick += OnAddSpriteButtonClick;
            _CloseButton.MouseClick += OnCloseButtonClick;
            _Picturebox.MouseClick += OnPictureboxClick;
            _Slider.ValueChange += OnSliderChange;
        }
        /// <summary>
        /// Load the content of this bone dialog.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create and load the item's brushes.
            _BoneBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Black);
            _OriginBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Green);
            _BoneEndBrush = new FarseerPhysics.DrawingSystem.LineBrush(1, Color.Red);
            _BoneBrush.Load(GUI.GraphicsDevice);
            _OriginBrush.Load(GUI.GraphicsDevice);
            _BoneEndBrush.Load(GUI.GraphicsDevice);
        }
        /// <summary>
        /// Update the bone dialog.
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
        /// Draw the bone dialog.
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
        /// Update all components in this dialog and their data.
        /// </summary>
        private void UpdateComponents()
        {
            //Update the components' data to reflect the current bone.
            _NameTextbox.InsertText(_Bone.Name);
            _ParentTextbox.InsertText(_Bone.ParentIndex.ToString());

            //Try and load the new texture.
            _Picturebox.Picture = _BoneSprite.Texture;
            _StartPosition = CalculatePictureOrigin();
            _EndPosition = CalculatePictureEnd();
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
        /// Calculate the point in the picturebox where the origin of the bone's sprite translates to.
        /// </summary>
        private Vector2 CalculatePictureOrigin()
        {
            /* First scale the origin to get the difference between the scaled start position and the
             * position of the picture's top left corner, with the scaling included. Add the position
             * of the picture's top left corner to the difference and the point in the picturebox has been found.*/

            //Return the point in the picturebox where the bone's sprite's origin is located.
            return ((BoneSprite[0].Origin * _Picturebox.Scale) + ((_Picturebox.Position + _Picturebox.Origin) -
                new Vector2(((_Picturebox.Picture.Width * _Picturebox.Scale) / 2), ((_Picturebox.Picture.Height * _Picturebox.Scale) / 2))));
        }
        /// <summary>
        /// Calculate the point in the picturebox where the end of the bone translates to.
        /// </summary>
        private Vector2 CalculatePictureEnd()
        {
            /* First scale the origin to get the difference between the scaled end position and the
             * position of the picture's top left corner, with the scaling included. Add the position
             * of the picture's top left corner to the difference and the point in the picturebox has been found.*/

            //Return the point in the picturebox where the bone's end position is located.
            return (Helper.CalculateOrbitPosition(_StartPosition, -_BoneSprite.RotationOffset, _Bone.Length) * _Picturebox.Scale);
        }
        /// <summary>
        /// A bone has been created.
        /// </summary>
        protected virtual void BoneEditedInvoke()
        {
            //Save the edited data to the bone.
            _Bone.Name = _NameTextbox.Text;
            _Bone.ParentIndex = int.Parse(_ParentTextbox.Text);

            //If someone has hooked up a delegate to the event, fire it.
            if (BoneEdited != null) { BoneEdited(this, new BoneEventArgs(_Bone)); }
        }
        /// <summary>
        /// A new bone has been selected.
        /// </summary>
        /// <param name="bone">The bone to select.</param>
        protected virtual void BoneSelectedInvoke(Bone bone)
        {
            //Select the bone.
            _Bone = bone;
            _BoneSprite = _Bone.Skeleton.Sprites.GetSprite(_Bone.Index.ToString());
            //Update all components to reflect this change.
            UpdateComponents();

            //If someone has hooked up a delegate to the event, fire it.
            if (BoneSelected != null) { BoneSelected(this, new BoneEventArgs(bone)); }
        }
        /// <summary>
        /// The 'add sprite' button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnAddSpriteButtonClick(object obj, MouseClickEventArgs e)
        {
            //Display a sprite dialog.
            GUI.AddItem(new SpriteDialog(GUI, new Vector2(350, 150), 500, 400));
            (GUI.LastItem as SpriteDialog).LoadContent();
            //Subscribe to some events.
            (GUI.LastItem as SpriteDialog).SpritePicked += OnSpritePicked;
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
            if (e.Button == MouseButton.Left)
            {
                _StartPosition = e.Position;
                _BoneSprite[0].Origin = CalculateSpriteOrigin();
            }
            //If the right button was pressed, update the end position.
            else if (e.Button == MouseButton.Right)
            {
                _EndPosition = e.Position;
                _BoneSprite.RotationOffset = Helper.CalculateRotationOffset(CalculateSpriteOrigin(), CalculateSpriteEnd());
            }

            //Update the bone length.
            _Bone.Length = Vector2.Distance(CalculateSpriteOrigin(), CalculateSpriteEnd());
        }
        /// <summary>
        /// A sprite has been picked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnSpritePicked(object obj, SpritePickedEventArgs e)
        {
            //Write down the sprite information.
            _BoneSprite[0].Name = e.Name;
            _BoneSprite[0].Origin = e.Origin;
            _BoneSprite.RotationOffset = Helper.CalculateAngleFromOrbitPositionBone(e.Origin, e.EndPosition);
            _Bone.Length = Vector2.Distance(e.Origin, e.EndPosition);

            //Unsubscribe from the sprite dialog's events.
            (GUI.LastItem as SpriteDialog).SpritePicked -= OnSpritePicked;
        }
        /// <summary>
        /// The 'close' button has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnCloseButtonClick(object obj, MouseClickEventArgs e)
        {
            //Invoke the bone created event.
            BoneEditedInvoke();
            //Invoke the dispose event.
            DisposeInvoke();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The bone currently undergoing editing.
        /// </summary>
        public Bone Bone
        {
            get { return _Bone; }
            set { BoneSelectedInvoke(value); }
        }
        /// <summary>
        /// The sprite of the bone that is currently undergoing editing.
        /// </summary>
        public Sprite BoneSprite
        {
            get { return _BoneSprite; }
        }
        #endregion
    }
}
