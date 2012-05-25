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

namespace Library.GUI
{
    /// <summary>
    /// The create bone dialog enables the user to create a bone with the desired properties and attach it to a skeleton.
    /// </summary>
    public class CreateBoneDialog : Form
    {
        #region Fields
        private Label _NameLabel;
        private Textbox _NameTextbox;
        private Button _AddSpriteButton;
        private Button _CloseButton;
        private Bone _Bone;
        private string _SpriteName;
        private Vector2 _SpriteOrigin;
        private float _SpriteRotationOffset;
        private float _Border;

        public delegate void BoneCreatedHandler(object obj, BoneCreatedEventArgs e);
        public event BoneCreatedHandler BoneCreated;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a bone dialog.
        /// </summary>
        /// <param name="gui">The GUI that this bone dialog will be a part of.</param>
        /// <param name="position">The position of this bone dialog.</param>
        /// <param name="height">The height of this bone dialog.</param>
        /// <param name="width">The width of this bone dialog.</param>
        public CreateBoneDialog(GraphicalUserInterface gui, Vector2 position, float width, float height)
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
            _Bone = new Bone();
            _Border = 5;
            _SpriteName = null;
            _SpriteOrigin = Vector2.Zero;
            _SpriteRotationOffset = 0;
            _NameLabel = new Label(GUI, Vector2.Add(Position, new Vector2(_Border, _Border)), 50, 15);
            _NameLabel.Text = "Name:";
            _NameTextbox = new Textbox(GUI, Vector2.Add(_NameLabel.Position, new Vector2(_NameLabel.Width, 0)), (Width - (2 * _Border) - _NameLabel.Width), 15);
            _AddSpriteButton = new Button(GUI, Vector2.Add(_NameLabel.Position, new Vector2(10, 40)), 75, 30);
            _AddSpriteButton.Text = "Sprite";
            _CloseButton = new Button(GUI, new Vector2((Position.X + ((Width / 2) - 25)), (Position.Y + (Height - 30 - _Border))), 50, 30);
            _CloseButton.Text = "Done";

            //Add the controls.
            AddItem(_NameLabel);
            AddItem(_NameTextbox);
            AddItem(_AddSpriteButton);
            AddItem(_CloseButton);

            //Hook up to some events.
            _AddSpriteButton.MouseClick += OnAddSpriteButtonClick;
            _CloseButton.MouseClick += OnCloseButtonClick;
        }
        /// <summary>
        /// Load the content of this bone dialog.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();
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
        }

        /// <summary>
        /// A bone has been created.
        /// </summary>
        protected virtual void BoneCreatedInvoke()
        {
            //Save the edited data to the bone.
            _Bone.Name = _NameTextbox.Text;

            //If someone has hooked up a delegate to the event, fire it.
            if (BoneCreated != null) { BoneCreated(this, new BoneCreatedEventArgs(_Bone, _SpriteName, _SpriteOrigin, _SpriteRotationOffset)); }
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
        /// A sprite has been picked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnSpritePicked(object obj, SpritePickedEventArgs e)
        {
            //Write down the sprite information.
            _SpriteName = e.Name;
            _SpriteOrigin = e.Origin;
            _SpriteRotationOffset = Helper.CalculateRotationOffset(e.Origin, e.EndPosition);
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
            BoneCreatedInvoke();
            //Invoke the dispose event.
            DisposeInvoke();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The bone currently undergoing creation.
        /// </summary>
        public Bone Bone
        {
            get { return _Bone; }
            set { _Bone = value; }
        }
        #endregion
    }
}
