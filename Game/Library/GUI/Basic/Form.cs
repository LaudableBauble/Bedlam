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
    /// Forms are blank windows that can be populated by different combinations of items.
    /// They can also be either silent (which means that they are simply acting as a canvas to items) or loud (which means that it lies ontop and prevents any other item
    /// from accessing user input as long as it is alive.
    /// </summary>
    public class Form : Component
    {
        #region Fields
        protected SpriteFont _Font;
        protected Layout _Layout;
        protected bool _IsDirty;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item instance.</returns>
        public Component this[int index]
        {
            get { return (_Items[index]); }
            set { _Items[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a form.
        /// </summary>
        /// <param name="gui">The GUI that this form will be a part of.</param>
        /// <param name="position">The position of this form.</param>
        /// <param name="height">The height of this form.</param>
        /// <param name="width">The width of this form.</param>
        public Form(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <param name="gui">The GUI that this form will be a part of.</param>
        /// <param name="position">The position of this form.</param>
        /// <param name="height">The height of this form.</param>
        /// <param name="width">The width of this form.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Items = new List<Component>();
            _Layout = new Layout(gui, position, width, height);
            _IsDirty = false;
        }
        /// <summary>
        /// Load the content of this form.
        /// </summary>
        public override void LoadContent()
        {
            //Reset the sprites.
            _Sprite.Clear();

            //Create the item's texture and load the font.
            AddSprite(DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, new Color(0, 0, 0, 155))).Position = Position;
            _Font = GUI.ContentManager.Load<SpriteFont>("GameScreen/Fonts/diagnosticFont");

            //The inherited method.
            base.LoadContent();
        }
        /// <summary>
        /// Update the form.
        /// </summary>
        /// <param name="gametime">The time to adhere to.</param>
        public override void Update(GameTime gametime)
        {
            //The inherited method.
            base.Update(gametime);

            //Manage the list of items.
            ManageItems();
            //Update the layout.
            _Layout.Update();

            //Fade the component appropriately.
            Fade(gametime);
        }

        /// <summary>
        /// Add a component to the form.
        /// </summary>
        /// <param name="item">The component to add.</param>
        public Component AddItem(Component item)
        {
            //Call the base method.
            base.Add(item);

            //Add the item to the layout.
            _Layout.Add(item);

            //Return the component.
            return item;
        }
        /// <summary>
        /// Manage the list of items.
        /// </summary>
        private void ManageItems()
        {
            //If to update the list.
            if (_IsDirty) { SortItems(); _IsDirty = false; }
        }
        /// <summary>
        /// Sort the list of items based on drawing order.
        /// </summary>
        private void SortItems()
        {
            //Sort the list.
            _Items = _Items.OrderByDescending(i => i.DrawOrder).ToList();
        }
        /// <summary>
        /// Change the transparence of this component.
        /// </summary>
        /// <param name="value">The new transparence value.</param>
        protected override void ChangeTransparence(float value)
        {
            //Call the base method.
            base.ChangeTransparence(value);

            //Change all inner components' transparence as well.
            foreach (Component component in _Items) { component.Transparence = Transparence; }
        }
        /// <summary>
        /// Fade the form in and out depending on whether the mouse is hovering over the form or not.
        /// </summary>
        /// <param name="gametime">The elapsed time.</param>
        private void Fade(GameTime gametime)
        {
            //If the form has focus, use no transparence.
            if (HasFocus || HasChildInFocus()) { if (Transparence != 1) { ChangeTransparence(Transparence + (float)gametime.ElapsedGameTime.TotalSeconds); } }
            else
            {
                //Fade the form in our out.
                if (IsMouseHovering) { ChangeTransparence(Transparence + (float)gametime.ElapsedGameTime.TotalSeconds); }
                else { ChangeTransparence(Math.Max(Transparence - (float)gametime.ElapsedGameTime.TotalSeconds, .2f)); }
            }
        }
        /// <summary>
        /// When a child component has changed bounds.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        protected override void OnItemBoundsChange(object obj, BoundsChangedEventArgs e)
        {
            //Request a layout readjustment.
            _Layout.RequestUpdate();
        }
        /// <summary>
        /// If an item has either been granted focus or lost it.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        protected override void OnItemFocusChange(object obj, FocusChangeEventArgs e)
        {
            //Call the base method.
            base.OnItemFocusChange(obj, e);

            //The component.
            Component item = (Component)obj;

            //First, see if the component has gained focus.
            if (!item.HasFocus) { return; }

            //See to it that the items' drawing orders are evenly spaced.
            foreach (Component i in _Items) { i.DrawOrder = 1; }
            //Make sure that the item in focus gets drawn last.
            item.DrawOrder = 0;
        }
        /// <summary>
        /// If a direct child item has changed its draw order, enable draw order update next turn.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        protected override void OnDrawOrderChange(object obj, EventArgs e)
        {
            //Call the base method.
            base.OnDrawOrderChange(obj, e);

            //Prepare to sort the list of items based on drawing order.
            _IsDirty = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The font that is used by this item.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        /// <summary>
        /// The texture of the item.
        /// </summary>
        public Texture2D Texture
        {
            get { return Sprite[0].Texture; }
            set { Sprite[0].Texture = value; }
        }
        /// <summary>
        /// The items this form displays.
        /// </summary>
        public List<Component> Items
        {
            get { return _Items; }
            set { _Items = value; }
        }
        #endregion
    }
}
