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

using Library.Enums;
using Library.Imagery;
using Library.Infrastructure;
using Library.GUI;

namespace Library.GUI.Basic
{
    /// <summary>
    /// A cell used in a grid layout.
    /// </summary>
    public class LayoutCell
    {
        #region Fields
        private Layout _Layout;
        private CellStyle _CellStyle;
        private Vector2 _Position;
        private float _Width;
        private float _MinWidth;
        private float _MaxWidth;
        private float _GoalWidth;
        private float _Height;
        private float _MinHeight;
        private float _MaxHeight;
        private float _GoalHeight;
        private Component _Component;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a layout grid cell.
        /// </summary>
        /// <param name="layout">The layout grid that this cell will be a part of.</param>
        /// <param name="component">The component this cell will contain.</param>
        public LayoutCell(Layout layout, Component component)
        {
            //Initialize some variables.
            Initialize(layout, component);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the layout grid.
        /// </summary>
        /// <param name="layout">The layout grid that this cell will be a part of.</param>
        /// <param name="component">The component this cell will contain.</param>
        public void Initialize(Layout layout, Component component)
        {
            //Initialize some variables.
            _Layout = layout;
            _Component = component;
            _Position = component.Position;
            _Width = component.Width;
            _Height = component.Height;
            _GoalWidth = _Width;
            _GoalHeight = _Height;
            _CellStyle = CellStyle.Dynamic;

            //Set some boundaries.
            _MinWidth = 0;
            _MaxWidth = 500;
            _MinHeight = 0;
            _MaxHeight = 500;

            //Subscribe to events.
            component.BoundsChange += OnItemBoundsChange;
        }
        /// <summary>
        /// Update the cell and its bounds.
        /// </summary>
        public void Update()
        {
            //If the component has changed its size voluntarily, do the same with the cell. Also change the goal size.
            if (_Component.Width != _Width) { _Width = _Component.Width; _GoalWidth = _Component.Width; }
            if (_Component.Height != _Height) { _Height = _Component.Height; _GoalHeight = _Component.Height; }
        }

        /// <summary>
        /// Propose a new width for the cell. The cell will only upgrade, ie. increase, if it is beneficial.
        /// </summary>
        /// <param name="height">The new width.</param>
        public void ProposeWidth(float width)
        {
            //See if it is beneficial to change width.
            if (width < _Width) { SetWidth(width); }
            else if (Math.Abs(_GoalWidth - _Width) > Math.Abs(_GoalWidth - width)) { SetWidth(width); }
        }
        /// <summary>
        /// Propose a new height for the cell. The cell will only upgrade, ie. increase, if it is beneficial.
        /// </summary>
        /// <param name="width">The new height.</param>
        public void ProposeHeight(float height)
        {
            //See if it is beneficial to change height.
            if (height < _Height) { SetHeight(height); }
            else if (Math.Abs(_GoalHeight - _Height) > Math.Abs(_GoalHeight - height)) { SetHeight(height); }
        }
        /// <summary>
        /// Set the width of the cell. Beware that it is still constrained between a min and max value.
        /// </summary>
        /// <param name="height">The new width.</param>
        private void SetWidth(float width)
        {
            //Set the new width and resize the component.
            _Width = MathHelper.Clamp(width, _MinWidth, _MaxWidth);
            _Component.Width = _Width;
        }
        /// <summary>
        /// Set the height of the cell. Beware that it is still constrained between a min and max value.
        /// </summary>
        /// <param name="width">The new height.</param>
        private void SetHeight(float height)
        {
            //Set the new height and resize the component.
            _Height = MathHelper.Clamp(height, _MinHeight, _MaxHeight);
            _Component.Height = _Height;
        }
        /// <summary>
        /// Set the position of the cell.
        /// </summary>
        /// <param name="width">The new position.</param>
        private void SetPosition(Vector2 position)
        {
            //Set the new position and move the component.
            _Position = position;
            _Component.Position = position;
        }
        /// <summary>
        /// If the item has changed its bounds.
        /// </summary>
        /// <param name="obj">The item that changed bounds.</param>
        /// <param name="e">The event arguments.</param>
        private void OnItemBoundsChange(object obj, BoundsChangedEventArgs e)
        {
            //Update the cell with the new data.
            Update();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The layout grid this cell is part of.
        /// </summary>
        public Layout Layout
        {
            get { return _Layout; }
            set { _Layout = value; }
        }
        /// <summary>
        /// The component this cell contains.
        /// </summary>
        public Component Component
        {
            get { return _Component; }
            set { _Component = value; }
        }
        /// <summary>
        /// The cell style.
        /// </summary>
        public CellStyle CellStyle
        {
            get { return _CellStyle; }
            set { _CellStyle = value; }
        }
        /// <summary>
        /// The position of the cell.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
            set { SetPosition(value); }
        }
        /// <summary>
        /// The x-coordinate of the cell.
        /// </summary>
        public float X
        {
            get { return _Position.X; }
            set { SetPosition(new Vector2(value, _Position.Y)); }
        }
        /// <summary>
        /// The y-coordinate of the cell.
        /// </summary>
        public float Y
        {
            get { return _Position.Y; }
            set { SetPosition(new Vector2(_Position.X, value)); }
        }
        /// <summary>
        /// The width of the layout cell.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { SetWidth(value); }
        }
        /// <summary>
        /// The maximum width of the layout cell.
        /// </summary>
        public float MaxWidth
        {
            get { return _MaxWidth; }
            set { _MaxWidth = value; }
        }
        /// <summary>
        /// The minimum width of the layout cell.
        /// </summary>
        public float MinWidth
        {
            get { return _MinWidth; }
            set { _MinWidth = value; }
        }
        /// <summary>
        /// The goal width of the layout cell.
        /// </summary>
        public float GoalWidth
        {
            get { return _GoalWidth; }
            set { _GoalWidth = value; }
        }
        /// <summary>
        /// The height of the layout cell.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { SetHeight(value); }
        }
        /// <summary>
        /// The maximum height of the layout cell.
        /// </summary>
        public float MaxHeight
        {
            get { return _MaxHeight; }
            set { _MaxHeight = value; }
        }
        /// <summary>
        /// The minimum height of the layout cell.
        /// </summary>
        public float MinHeight
        {
            get { return _MinHeight; }
            set { _MinHeight = value; }
        }
        /// <summary>
        /// The goal height of the layout cell.
        /// </summary>
        public float GoalHeight
        {
            get { return _GoalHeight; }
            set { _GoalHeight = value; }
        }
        #endregion
    }
}
