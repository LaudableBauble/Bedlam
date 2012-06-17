using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Enums
{
    #region GUI
    /// <summary>
    /// The layout cell size style of a form.
    /// This determines how the layout grid will distribute the space available for a row or column to its cells.
    /// </summary>
    public enum LayoutSizeStyle { Evenly, FirstInLine }
    /// <summary>
    /// The layout flow style of a form.
    /// That is if the form will try to position components primarily vertically or horizontally.
    /// </summary>
    public enum LayoutFlowStyle { Horizontally, Vertically }
    /// <summary>
    /// The style of a grid cell.
    /// A fixed cell does not care whether there are gaps between components whereas a dynamic one does.
    /// Apart from this, a single component with a fixed cell style is left alone by a layout.
    /// </summary>
    public enum CellStyle { Fixed, Dynamic }
    /// <summary>
    /// The vertical layout alignment of a form.
    /// Whether to build from top to down or from bottom to up.
    /// </summary>
    public enum LayoutVerticalAlignment { TopDown, BottomUp }
    /// <summary>
    /// The horizontal layout alignment of a form.
    /// Whether to build from left to right or from right to left.
    /// </summary>
    public enum LayoutHorizontalAlignment { LeftToRight, RightToLeft }
    /// <summary>
    /// The state of the node in the tree view; that is if it's collapsed, expanded or neither.
    /// </summary>
    public enum TreeNodeState { None, Collapsed, Expanded }
    #endregion

    /// <summary>
    /// The orientation of a sprite; namely none, left or right.
    /// </summary>
    public enum Orientation
    {
        None, Left, Right
    }
    /// <summary>
    /// The visibility of the sprite.
    /// </summary>
    public enum Visibility
    {
        Invisible, Visible
    }
    /// <summary>
    /// The direction an object is facing.
    /// </summary>
    public enum FacingDirection
    {
        None, Left, Right
    }
    /// <summary>
    /// The different types of items available to the game.
    /// This enum do not represent any specific or unique item but only the different base classes to choose from.
    /// </summary>
    public enum ItemType
    {
        Item, TextureItem, Entity, Character
    }
    /// <summary>
    /// A list of available items in the game.
    /// </summary>
    public enum Items
    {

    }
    /// <summary>
    /// A list of available entities in the game.
    /// </summary>
    public enum Entities
    {
        Box
    }
    /// <summary>
    /// A list of available characters in the game.
    /// </summary>
    public enum Characters
    {

    }
    /// <summary>
    /// The structure elements of a layout grid.
    /// </summary>
    public enum LayoutStructure
    {
        Row, Column
    }
    /// <summary>
    /// The level state describes what the level is currently up to.
    /// </summary>
    public enum LevelState
    {
        Pause, Play
    }
}
