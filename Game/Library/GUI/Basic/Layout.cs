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
    /// A grid layout for a GUI form.
    /// </summary>
    public class Layout
    {
        #region Fields
        private GraphicalUserInterface _GUI;

        private bool _IsEnabled;
        private LayoutSizeStyle _SizeStyle;
        private LayoutFlowStyle _LayoutFlow;
        private LayoutVerticalAlignment _VerticalAlignment;
        private LayoutHorizontalAlignment _HorizontalAlignment;

        private Vector2 _Position;
        private float _Width;
        private float _Height;
        private int _MaxColumns;
        private int _MaxRows;
        private float _Padding;
        private float _Margin;

        private LayoutCell[,] _Grid;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a layout grid.
        /// </summary>
        /// <param name="gui">The GUI that this layout will be a part of.</param>
        /// <param name="position">The position of this layout.</param>
        /// <param name="height">The height of this field.</param>
        /// <param name="width">The width of this field.</param>
        public Layout(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the layout grid.
        /// </summary>
        /// <param name="gui">The GUI that this layout will be a part of.</param>
        /// <param name="position">The position of this layout.</param>
        /// <param name="height">The height of this layout.</param>
        /// <param name="width">The width of this layout.</param>
        public void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            _GUI = gui;
            _IsEnabled = true;
            _Position = position;
            _Width = width;
            _Height = height;
            _MaxColumns = 3;
            _MaxRows = 12;
            _Padding = 5;
            _Margin = 5;
            _Grid = new LayoutCell[_MaxColumns, _MaxRows];
            _SizeStyle = LayoutSizeStyle.Evenly;
            _LayoutFlow = LayoutFlowStyle.Vertically;
            _VerticalAlignment = LayoutVerticalAlignment.TopDown;
            _HorizontalAlignment = LayoutHorizontalAlignment.LeftToRight;
        }

        /// <summary>
        /// Add a component to the layout and automatically choose an appropriate cell for it.
        /// </summary>
        /// <param name="component">The component to add.</param>
        public void Add(Component component)
        {
            //The cell position.
            int xCell;
            int yCell;

            //Find the 'best' empty cell and add the component to it.
            FindFirstEmptyCell(out xCell, out yCell);
            AddToCell(xCell, yCell, component);

            //Subscribe to some of the item's events.
            //component.BoundsChange += delegate(object sender, BoundsChangedEventArgs e) { UpdateLayout(); };

            //Finally update the grid layout to accomodate for the addition.
            UpdateLayout();
        }
        /// <summary>
        /// Add a GUI component to a specified cell in the layout grid.
        /// Beware that you might overwrite a cell without warning.
        /// </summary>
        /// <param name="x">The horizontal position in the grid.</param>
        /// <param name="y">The vertical position in the grid.</param>
        /// <param name="component">The GUI component to add.</param>
        private void AddToCell(int x, int y, Component component)
        {
            //Add the component to the layout grid.
            _Grid[x, y] = new LayoutCell(this, component);
        }
        /// <summary>
        /// Update the full layout grid.
        /// </summary>
        public void UpdateLayout()
        {
            //Foreach row and column, update it.
            for (int row = 0; row < _Grid.GetLength(1); row++) { UpdateRow(row); }
            for (int column = 0; column < _Grid.GetLength(0); column++) { UpdateColumn(column); }
        }
        /// <summary>
        /// Update a row in the layout grid.
        /// </summary>
        /// <param name="row">The row to update.</param>
        private void UpdateRow(int row)
        {
            //TODO: If the width runs out, move the component somewhere else.

            //The direction flows to match the layout's alignments.
            int dir = (_HorizontalAlignment == LayoutHorizontalAlignment.LeftToRight) ? 1 : -1;

            //The number of occupied cells in the row.
            int cellCount = RowCount(row);

            //The current cell's position coordinates, width and margin.
            float xPos = (dir == 1) ? _Position.X + _Padding : _Position.X + _Width - _Padding;
            float width = 0;

            //How to distribute space between cells in the row.
            switch (_SizeStyle)
            {
                case (LayoutSizeStyle.Evenly):
                    {
                        //The average width.
                        float avgWidth = GetAvailableSpace(row, LayoutStructure.Row) / cellCount;

                        //For each cell in the row.
                        for (int xCell = (dir == 1) ? 0 : cellCount - 1; xCell >= 0 && xCell < cellCount; xCell += dir)
                        {
                            //If the cell does not have a valid item, stop here.
                            if (_Grid[xCell, row] == null) { continue; }

                            //The width available for the cell and its new position.
                            _Grid[xCell, row].ProposeWidth(avgWidth);
                            _Grid[xCell, row].X = xPos + (dir * (width + ((cellCount - 1 > xCell) ? _Margin : 0)));

                            //Update the reference values.
                            xPos = _Grid[xCell, row].X;
                            width += _Grid[xCell, row].Width;
                        }
                        break;
                    }
                case (LayoutSizeStyle.FirstInLine):
                    {
                        //Get the available space for this row.
                        float availableWidth = GetAvailableSpace(row, LayoutStructure.Row);

                        //For each cell in the row.
                        for (int xCell = (dir == 1) ? 0 : cellCount - 1; xCell >= 0 && xCell < cellCount; xCell += dir)
                        {
                            //If the cell does not have a valid item, stop here.
                            if (_Grid[xCell, row] == null) { continue; }

                            //The width available for the cell and its new position. ??? DOES NOT WORK!
                            _Grid[xCell, row].ProposeWidth((dir == 1) ? availableWidth - (_Position.X - xPos - width) : xPos);
                            _Grid[xCell, row].X = xPos + (dir * (width + ((cellCount - 1 > xCell) ? _Margin : 0)));

                            //Update the reference values.
                            xPos = _Grid[xCell, row].X;
                            width = _Grid[xCell, row].Width;
                        }
                        break;
                    }
            }
        }
        /// <summary>
        /// Update a column in the layout grid.
        /// </summary>
        /// <param name="column">The column to update.</param>
        private void UpdateColumn(int column)
        {
            //TODO: If the height runs out, move the component somewhere else.

            //The direction flows to match the layout's alignments.
            int dir = (_VerticalAlignment == LayoutVerticalAlignment.TopDown) ? 1 : -1;

            //The number of occupied cells in the column.
            int cellCount = ColumnCount(column);

            //The current cell's position coordinates and height.
            float yPos = (dir == 1) ? _Position.Y : _Position.Y + _Height;
            float height = 0;

            //How to distribute space between cells in the column.
            switch (_SizeStyle)
            {
                case (LayoutSizeStyle.Evenly):
                    {
                        //The average height.
                        float avgHeight = GetAvailableSpace(column, LayoutStructure.Column) / cellCount;

                        //For each cell in the column.
                        for (int yCell = (dir == 1) ? 0 : cellCount - 1; yCell >= 0 && yCell < cellCount; yCell += dir)
                        {
                            //If the cell does not have a valid item, stop here.
                            if (_Grid[column, yCell] == null) { continue; }

                            //The width available for the cell and its new position.
                            _Grid[column, yCell].ProposeHeight(avgHeight);
                            _Grid[column, yCell].Y = yPos + (dir * (height + ((cellCount - 1 > yCell) ? _Margin : 0)));

                            //Update the reference values.
                            yPos = _Grid[column, yCell].Y;
                            height = _Grid[column, yCell].Height;
                        }
                        break;
                    }
                case (LayoutSizeStyle.FirstInLine):
                    {
                        //For each cell in the column.
                        for (int yCell = (dir == 1) ? 0 : cellCount - 1; yCell >= 0 && yCell < cellCount; yCell += dir)
                        {
                            //If the cell does not have a valid item, stop here.
                            if (_Grid[column, yCell] == null) { continue; }

                            //The width available for the cell and its new position. ??? DOES NOT WORK!
                            _Grid[column, yCell].ProposeHeight((dir == 1) ? height - (_Position.Y - yPos - height) : yPos);
                            _Grid[column, yCell].Y = yPos + (dir * height);

                            //Update the reference values.
                            yPos = _Grid[column, yCell].Y;
                            height = _Grid[column, yCell].Height;
                        }
                        break;
                    }
            }
        }
        /// <summary>
        /// Listen to all cells and set the layout's size to the one desired by them.
        /// </summary>
        public void SetToDesiredSize()
        {
            //Update the width and height.
            _Width = DesiredWidth;
            _Height = DesiredHeight;

            //Update the layout.
            UpdateLayout();
        }
        /// <summary>
        /// Add a component to the layout and automatically choose an appropriate cell for it.
        /// </summary>
        /// <param name="xCell">The x-coordinate of the first empty cell.</param>
        /// <param name="yCell">The y-coordinate of the first empty cell.</param>
        private void FindFirstEmptyCell(out int xCell, out int yCell)
        {
            //The direction flows to match the layout's alignments.
            int hDir = (_HorizontalAlignment == LayoutHorizontalAlignment.LeftToRight) ? 1 : -1;
            int vDir = (_VerticalAlignment == LayoutVerticalAlignment.TopDown) ? 1 : -1;

            //The cells.
            xCell = (hDir == 1) ? 0 : _MaxColumns;
            yCell = (vDir == 1) ? 0 : _MaxRows;

            //The type of layout flow.
            if (_LayoutFlow == LayoutFlowStyle.Horizontally)
            {
                //Iterate through the layout grid in a y, x fashion.
                for (; yCell >= 0 && yCell < _Grid.GetLength(1); yCell += vDir)
                {
                    for (; xCell >= 0 && xCell < _Grid.GetLength(0); xCell += hDir)
                    {
                        //If the cell is empty, return it.
                        if (_Grid[xCell, yCell] == null) { return; }
                    }
                }
            }
            else if (_LayoutFlow == LayoutFlowStyle.Vertically)
            {
                //Iterate through the layout grid in a x, y fashion.
                for (; xCell >= 0 && xCell < _Grid.GetLength(0); xCell += hDir)
                {
                    for (; yCell >= 0 && yCell < _Grid.GetLength(1); yCell += vDir)
                    {
                        //If the cell is empty, return it.
                        if (_Grid[xCell, yCell] == null) { return; }
                    }
                }
            }
        }
        /// <summary>
        /// Get the number of occupied cells in a certain row.
        /// </summary>
        /// <param name="row">The row in question.</param>
        /// <returns>The number of items in the row.</returns>
        private int RowCount(int row)
        {
            //Count the number of items at the specified row..
            int count = 0;
            for (int x = 0; x < _Grid.GetLength(0); x++) { if (_Grid[x, row] != null) { count++; } }
            return count;
        }
        /// <summary>
        /// Get the number of occupied cells in a certain column.
        /// </summary>
        /// <param name="row">The column in question.</param>
        /// <returns>The number of items in the column.</returns>
        private int ColumnCount(int column)
        {
            //Count the number of items at the specified column..
            int count = 0;
            for (int y = 0; y < _Grid.GetLength(1); y++) { if (_Grid[column, y] != null) { count++; } }
            return count;
        }
        /// <summary>
        /// Get the available space for a row or column based upon padding, margin and the number of elements.
        /// </summary>
        /// <param name="index">The index of the row or column.</param>
        /// <param name="structure">Whether it is a row or column we are checking.</param>
        /// <returns>The amount of 'free' space available for the given row or column.</returns>
        private float GetAvailableSpace(int index, LayoutStructure structure)
        {
            //Whether to check a row or column.
            switch (structure)
            {
                case (LayoutStructure.Row): { return (_Width - (2 * _Padding) - ((RowCount(index) - 1) * _Margin)); }
                case (LayoutStructure.Column): { return (_Height - (2 * _Padding) - ((ColumnCount(index) - 1) * _Margin)); }
                default: { return 0; }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The GUI that this layout is part of.
        /// </summary>
        public GraphicalUserInterface GUI
        {
            get { return _GUI; }
            set { _GUI = value; }
        }
        /// <summary>
        /// Whether the layout is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; }
        }
        /// <summary>
        /// The layout size style.
        /// </summary>
        public LayoutSizeStyle SizeStyle
        {
            get { return _SizeStyle; }
            set { _SizeStyle = value; }
        }
        /// <summary>
        /// The layout flow style.
        /// </summary>
        public LayoutFlowStyle LayoutStyle
        {
            get { return _LayoutFlow; }
            set { _LayoutFlow = value; }
        }
        /// <summary>
        /// The layout's vertical alignment.
        /// </summary>
        public LayoutVerticalAlignment VerticalAlignment
        {
            get { return _VerticalAlignment; }
            set { _VerticalAlignment = value; }
        }
        /// <summary>
        /// The layout's horizontal alignment.
        /// </summary>
        public LayoutHorizontalAlignment HorizontalAlignment
        {
            get { return _HorizontalAlignment; }
            set { _HorizontalAlignment = value; }
        }
        /// <summary>
        /// The position of the layout.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        /// <summary>
        /// The width of the layout grid.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        /// <summary>
        /// The height of the layout grid.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        /// <summary>
        /// The desired width of the layout grid, if a man can dream. This value is based off of each cell's goal width.
        /// </summary>
        public float DesiredWidth
        {
            get
            {
                //The desired width.
                float width = 0;

                //For each cell in the grid.
                for (int row = 0; row < _Grid.GetLength(1); row++)
                {
                    //The desired width of this row.
                    float rowWidth = 0;
                    for (int xCell = 0; xCell < _Grid.GetLength(0); xCell++)
                    {
                        //If the cell does not have a valid item, stop here.
                        if (_Grid[xCell, row] == null) { continue; }

                        //Add the cell's goal width to the row's width.
                        rowWidth += (xCell > 0) ? _Grid[xCell, row].GoalWidth + _Margin : _Grid[xCell, row].GoalWidth;
                    }

                    //If the row's goal width is greater than the layout's desired width, change them.
                    if (rowWidth > width) { width = rowWidth; }
                }

                //Return the desired width.
                return width + (2 * _Padding);
            }
        }
        /// <summary>
        /// The desired height of the layout grid, if a man can dream. This value is based off of each cell's goal height.
        /// </summary>
        public float DesiredHeight
        {
            get
            {
                //The desired height.
                float height = 0;

                //For each cell in the grid.
                for (int row = 0; row < _Grid.GetLength(1); row++)
                {
                    //The desired height of this row.
                    float rowHeight = 0;
                    for (int xCell = 0; xCell < _Grid.GetLength(0); xCell++)
                    {
                        //If the cell does not have a valid item, stop here.
                        if (_Grid[xCell, row] == null) { continue; }

                        //Add the cell's goal width to the row's height.
                        rowHeight += (xCell > 0) ? _Grid[xCell, row].GoalHeight + _Margin : _Grid[xCell, row].GoalHeight;
                    }

                    //If the row's goal height is greater than the layout's desired height, change them.
                    if (rowHeight > height) { height = rowHeight; }
                }

                //Return the desired width.
                return height + (2 * _Padding);
            }
        }
        /// <summary>
        /// The number of available grid columns.
        /// </summary>
        public int MaxColumns
        {
            get { return _MaxColumns; }
            set { _MaxColumns = value; }
        }
        /// <summary>
        /// The number of available grid rows.
        /// </summary>
        public int MaxRows
        {
            get { return _MaxRows; }
            set { _MaxRows = value; }
        }
        /// <summary>
        /// The padding of the layout, ie. the amount of space left free just inside the layout area.
        /// </summary>
        public float Padding
        {
            get { return _Padding; }
            set { _Padding = value; }
        }
        /// <summary>
        /// The margin of the layout, ie. the amount of space left free around the layout area.
        /// </summary>
        public float Margin
        {
            get { return _Margin; }
            set { _Margin = value; }
        }
        #endregion
    }
}

/* Old Code
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
    /// A grid layout for a GUI form.
    /// </summary>
    public class Layout
    {
        #region Fields
        private GraphicalUserInterface _GUI;

        private bool _IsEnabled;
        private LayoutFlowStyle _LayoutFlow;
        private CellStyle _CellStyle;
        private LayoutVerticalAlignment _VerticalAlignment;
        private LayoutHorizontalAlignment _HorizontalAlignment;

        private Vector2 _Position;
        private float _Width;
        private float _Height;
        private int _MaxColumns;
        private int _MaxRows;
        private float _CellWidth;
        private float _CellHeight;
        private float _CellPadding;

        private Component[,] _Grid;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a layout grid.
        /// </summary>
        /// <param name="gui">The GUI that this layout will be a part of.</param>
        /// <param name="position">The position of this layout.</param>
        /// <param name="height">The height of this field.</param>
        /// <param name="width">The width of this field.</param>
        public Layout(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the layout grid.
        /// </summary>
        /// <param name="gui">The GUI that this layout will be a part of.</param>
        /// <param name="position">The position of this layout.</param>
        /// <param name="height">The height of this layout.</param>
        /// <param name="width">The width of this layout.</param>
        public void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            _GUI = gui;
            _IsEnabled = true;
            _Position = position;
            _Width = width;
            _Height = height;
            _MaxColumns = 3;
            _MaxRows = 12;
            _CellWidth = 140;
            _CellHeight = 40;
            _CellPadding = 20;
            _Grid = new Component[_MaxColumns, _MaxRows];
            _LayoutFlow = LayoutFlowStyle.Vertically;
            _CellStyle = CellStyle.Dynamic;
            _VerticalAlignment = LayoutVerticalAlignment.TopDown;
            _HorizontalAlignment = LayoutHorizontalAlignment.LeftToRight;

            //Hook up some events.
        }
        /// <summary>
        /// Update the layout grid.
        /// </summary>
        /// <param name="gameTime">The time to adhere to.</param>
        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Add a component to the layout and automatically choose an appropriate cell for it.
        /// </summary>
        /// <param name="component">The component to add.</param>
        public void Add(Component component)
        {
            //The cell position.
            int xCell;
            int yCell;

            //Find the 'best' empty cell and add the component to it.
            FindFirstEmptyCell(out xCell, out yCell);
            AddToCell(xCell, yCell, component);

            //Subscribe to some of the item's events.
            component.BoundsChange += delegate(object sender, BoundsChangedEventArgs e) { UpdateLayout(); };

            //Finally update the grid layout to accomodate for the addition.
            UpdateLayout();
        }
        /// <summary>
        /// Add a GUI component to a specified cell in the layout grid.
        /// Beware that you might overwrite a cell without warning.
        /// </summary>
        /// <param name="x">The horizontal position in the grid.</param>
        /// <param name="y">The vertical position in the grid.</param>
        /// <param name="component">The GUI component to add.</param>
        public void AddToCell(int x, int y, Component component)
        {
            //Add the component to the layout grid.
            _Grid[x, y] = component;
        }
        /// <summary>
        /// Update the layout by going through each cell in the grid and reposition them accordingly.
        /// </summary>
        private void UpdateLayout()
        {
            //If the layout isn't enabled, close the shop here.
            if (!_IsEnabled) { return; }

            //The cell's position.
            float xPos = _Position.X;
            float yPos = _Position.Y;

            //The direction flows to match the layout's alignments.
            int hDir = (_HorizontalAlignment == LayoutHorizontalAlignment.LeftToRight) ? 1 : -1;
            int vDir = (_VerticalAlignment == LayoutVerticalAlignment.TopDown) ? 1 : -1;

            //The cells.
            int xCell = (hDir == 1) ? 0 : _MaxColumns;
            int yCell = (vDir == 1) ? 0 : _MaxRows;

            //For each cell in the grid.
            for (; xCell >= 0 && xCell < _Grid.GetLength(0); xCell += hDir)
            {
                for (; yCell >= 0 && yCell < _Grid.GetLength(1); yCell += vDir)
                {
                    //If the grid is fixed or dynamic.
                    if (_CellStyle == CellStyle.Fixed)
                    {
                        //Calculate the raw position of the cell.
                        xPos = _Position.X + (xCell * (_CellWidth + _CellPadding));
                        yPos = _Position.Y + (yCell * (_CellHeight + _CellPadding));
                    }
                    else if (_CellStyle == CellStyle.Dynamic)
                    {
                        //Go through the current grid row until you find an occupied cell.
                        for (int x = xCell; x >= 0 && x < _MaxRows; x -= hDir)
                        {
                            //Calculate the dynamic position.
                            if (_Grid[x, yCell] != null)
                            {
                                //If this is the first cell on the row, seen from a right to left perspective.
                                //NOTE: The other way has already been covered above.
                                if ((x - hDir) >= _MaxRows)
                                {
                                    xPos = _Position.X + ((_MaxRows - 1) * (_CellWidth + _CellPadding));
                                }
                                else if ((x - hDir) >= 0 && (x - hDir) < _MaxRows)
                                {
                                    xPos = _Grid[x - hDir, yCell].Position.X + (hDir * (_Grid[x - hDir, yCell].Width + _CellPadding));
                                }

                                break;
                            }
                        }

                        //Go through the current grid row until you find an occupied cell.
                        for (int y = yCell; y >= 0 && y < _MaxColumns; y -= vDir)
                        {
                            //Calculate the dynamic position.
                            if (_Grid[xCell, y] != null)
                            {
                                //If this is the first cell on the column, seen from a bottom to top perspective.
                                //NOTE: The other way has already been covered above.
                                if ((y - vDir) >= _MaxColumns)
                                {
                                    yPos = _Position.Y + ((_MaxColumns - 1) * (_CellHeight + _CellPadding));
                                }
                                else if ((y - vDir) >= 0 && (y - vDir) < _MaxColumns)
                                {
                                    yPos = _Grid[xCell, y - vDir].Position.Y + (vDir * (_Grid[xCell, y - vDir].Height + _CellPadding));
                                }

                                break;
                            }
                        }
                    }

                    //Set the position of the cell, ie. the top left corner.
                    if (_Grid[xCell, yCell] != null && _Grid[xCell, yCell].CellStyle == Enums.CellStyle.Dynamic) { _Grid[xCell, yCell].Position = new Vector2(xPos, yPos); }
                }
            }
        }
        /// <summary>
        /// Add a component to the layout and automatically choose an appropriate cell for it.
        /// </summary>
        /// <param name="xCell">The x-coordinate of the first empty cell.</param>
        /// <param name="yCell">The y-coordinate of the first empty cell.</param>
        private void FindFirstEmptyCell(out int xCell, out int yCell)
        {
            //The direction flows to match the layout's alignments.
            int hDir = (_HorizontalAlignment == LayoutHorizontalAlignment.LeftToRight) ? 1 : -1;
            int vDir = (_VerticalAlignment == LayoutVerticalAlignment.TopDown) ? 1 : -1;

            //The cells.
            xCell = (hDir == 1) ? 0 : _MaxColumns;
            yCell = (vDir == 1) ? 0 : _MaxRows;

            //The type of layout flow.
            if (_LayoutFlow == LayoutFlowStyle.Horizontally)
            {
                //Iterate through the layout grid in a y, x fashion.
                for (; yCell >= 0 && yCell < _Grid.GetLength(1); yCell += vDir)
                {
                    for (; xCell >= 0 && xCell < _Grid.GetLength(0); xCell += hDir)
                    {
                        //If the cell is empty, return it.
                        if (_Grid[xCell, yCell] == null) { return; }
                    }
                }
            }
            else if (_LayoutFlow == LayoutFlowStyle.Vertically)
            {
                //Iterate through the layout grid in a x, y fashion.
                for (; xCell >= 0 && xCell < _Grid.GetLength(0); xCell += hDir)
                {
                    for (; yCell >= 0 && yCell < _Grid.GetLength(1); yCell += vDir)
                    {
                        //If the cell is empty, return it.
                        if (_Grid[xCell, yCell] == null) { return; }
                    }
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The GUI that this layout is part of.
        /// </summary>
        public GraphicalUserInterface GUI
        {
            get { return _GUI; }
            set { _GUI = value; }
        }
        /// <summary>
        /// Whether the layout is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; }
        }
        /// <summary>
        /// The layout flow style.
        /// </summary>
        public LayoutFlowStyle LayoutStyle
        {
            get { return _LayoutFlow; }
            set { _LayoutFlow = value; }
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
        /// The layout's vertical alignment.
        /// </summary>
        public LayoutVerticalAlignment VerticalAlignment
        {
            get { return _VerticalAlignment; }
            set { _VerticalAlignment = value; }
        }
        /// <summary>
        /// The layout's horizontal alignment.
        /// </summary>
        public LayoutHorizontalAlignment HorizontalAlignment
        {
            get { return _HorizontalAlignment; }
            set { _HorizontalAlignment = value; }
        }
        /// <summary>
        /// The position of the layout.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        /// <summary>
        /// The width of the layout grid.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        /// <summary>
        /// The height of the layout grid.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        /// <summary>
        /// The number of available grid columns.
        /// </summary>
        public int MaxColumns
        {
            get { return _MaxColumns; }
            set { _MaxColumns = value; }
        }
        /// <summary>
        /// The number of available grid rows.
        /// </summary>
        public int MaxRows
        {
            get { return _MaxRows; }
            set { _MaxRows = value; }
        }
        /// <summary>
        /// The width of a grid cell.
        /// </summary>
        public float CellWidth
        {
            get { return _CellWidth; }
            set { _CellWidth = value; }
        }
        /// <summary>
        /// The height of a grid cell.
        /// </summary>
        public float CellHeight
        {
            get { return _CellHeight; }
            set { _CellHeight = value; }
        }
        /// <summary>
        /// The padding inbetween grid cells.
        /// </summary>
        public float CellPadding
        {
            get { return _CellPadding; }
            set { _CellPadding = value; }
        }
        #endregion
    }
}
*/