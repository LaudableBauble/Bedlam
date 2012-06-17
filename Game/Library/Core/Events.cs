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

using Library.GUI;
using Library.GUI.Basic;
using Library.Animate;
using Library.Infrastructure;

namespace Library
{
    /// <summary>
    /// An event used when something has been either ticked or unticked.
    /// </summary>
    public class TickEventArgs : EventArgs
    {
        #region Fields
        private bool _IsChecked;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when something is either ticked or unticked.
        /// </summary>
        /// <param name="isChecked">Whether it has been checked or not.</param>
        public TickEventArgs(bool isChecked)
        {
            //Pass along the data.
            _IsChecked = isChecked;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Whether something is ticked or not.
        /// </summary>
        public bool IsChecked
        {
            get { return _IsChecked; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when an item has been clicked on by a mouse.
    /// </summary>
    public class MouseClickEventArgs : EventArgs
    {
        #region Fields
        private Vector2 _Position;
        private MouseButton _Button;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when the user clicks at something with the mouse.
        /// </summary>
        /// <param name="position">The position of the mouse at the time of the click.</param>
        /// <param name="button">Which mouse button that has been clicked.</param>
        public MouseClickEventArgs(Vector2 position, MouseButton button)
        {
            //Pass along the data.
            _Position = position;
            _Button = button;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The position of the mouse at the time of the click.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
        }
        /// <summary>
        /// Which mouse button that has been clicked.
        /// </summary>
        public MouseButton Button
        {
            get { return _Button; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when a new child node has been added.
    /// </summary>
    public class ChildNodeAddedEventArgs : EventArgs
    {
        #region Fields
        private TreeNode _Parent;
        private TreeNode _Child;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when a new node has been added.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="_Child">The child node that has just been added.</param>
        public ChildNodeAddedEventArgs(TreeNode parent, TreeNode child)
        {
            //Pass along the data.
            _Parent = parent;
            _Child = child;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The parent node.
        /// </summary>
        public TreeNode Parent
        {
            get { return _Parent; }
        }
        /// <summary>
        /// The child node that has just been added.
        /// </summary>
        public TreeNode Child
        {
            get { return _Child; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when an item is about to get disposed of.
    /// </summary>
    public class DisposeEventArgs : EventArgs
    {
        #region Fields
        private Component _Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when an item is about to get disposed of.
        /// </summary>
        /// <param name="item">The item to be disposed of.</param>
        public DisposeEventArgs(Component item)
        {
            //Pass along the data.
            _Item = item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The item to be disposed.
        /// </summary>
        public Component Item
        {
            get { return _Item; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when a component has been selected for some reason.
    /// </summary>
    public class ItemSelectEventArgs : EventArgs
    {
        #region Fields
        private Component _Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when an item has been selected.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public ItemSelectEventArgs(Component item)
        {
            //Pass along the data.
            _Item = item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The selected item.
        /// </summary>
        public Component Item
        {
            get { return _Item; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when an item has been selected for some reason.
    /// </summary>
    public class ListItemSelectEventArgs : EventArgs
    {
        #region Fields
        private ListItem _Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when an list item has been selected.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public ListItemSelectEventArgs(ListItem item)
        {
            //Pass along the data.
            _Item = item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The selected list item.
        /// </summary>
        public ListItem Item
        {
            get { return _Item; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when an item has been selected for some reason.
    /// </summary>
    public class MenuItemSelectEventArgs : EventArgs
    {
        #region Fields
        private MenuItem _Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when a menu item has been selected.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public MenuItemSelectEventArgs(MenuItem item)
        {
            //Pass along the data.
            _Item = item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The selected menu item.
        /// </summary>
        public MenuItem Item
        {
            get { return _Item; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when an item has had a change in focus for some reason.
    /// </summary>
    public class FocusChangeEventArgs : EventArgs
    {
        #region Fields
        private Component _Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when an item has had a change in focus for some reason.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public FocusChangeEventArgs(Component item)
        {
            //Pass along the data.
            _Item = item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The item whose focus has changed.
        /// </summary>
        public Component Item
        {
            get { return _Item; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when an animation bar has changed its selected frame one to another.
    /// </summary>
    public class SelectedFrameChangeEventArgs : EventArgs
    {
        #region Fields
        private int _PastIndex;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when an animation bar has changed its selected frame one to another.
        /// </summary>
        /// <param name="pastIndex">The previously selected frame index.</param>
        public SelectedFrameChangeEventArgs(int pastIndex)
        {
            //Pass along the data.
            _PastIndex = pastIndex;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The previously selected frame index.
        /// </summary>
        public int PastIndex
        {
            get { return _PastIndex; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when a sprite has been picked or chosen by an item.
    /// </summary>
    public class SpritePickedEventArgs : EventArgs
    {
        #region Fields
        private string _Name;
        private Vector2 _Origin;
        private Vector2 _EndPosition;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when a sprite has been picked or chosen by an item.
        /// </summary>
        /// <param name="name">The name of the sprite.</param>
        /// <param name="origin">The origin of the sprite.</param>
        /// <param name="end">The end position. Used to calculate the length and rotation offset in regard to a bone.</param>
        public SpritePickedEventArgs(string name, Vector2 origin, Vector2 end)
        {
            //Pass along the data.
            _Name = name;
            _Origin = origin;
            _EndPosition = end;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name of the sprite.
        /// </summary>
        public string Name
        {
            get { return _Name; }
        }
        /// <summary>
        /// The origin of the sprite.
        /// </summary>
        public Vector2 Origin
        {
            get { return _Origin; }
        }
        /// <summary>
        /// The end position. Used to calculate the length and rotation offset in regard to a bone.
        /// </summary>
        public Vector2 EndPosition
        {
            get { return _EndPosition; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when a bone has been created by an item.
    /// </summary>
    public class BoneCreatedEventArgs : EventArgs
    {
        #region Fields
        private Bone _Bone;
        private string _SpriteName;
        private Vector2 _SpriteOrigin;
        private float _SpriteRotationOffset;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when a bone has been created by an item.
        /// </summary>
        /// <param name="bone">The created bone.</param>
        public BoneCreatedEventArgs(Bone bone)
        {
            //Pass along the data.
            _Bone = bone;
            _SpriteName = null;
            _SpriteOrigin = Vector2.Zero;
            _SpriteRotationOffset = 0;
        }
        /// <summary>
        /// Create the event used when a bone has been created by an item.
        /// </summary>
        /// <param name="bone">The created bone.</param>
        /// <param name="spriteName">The name of the bone's sprite.</param>
        /// <param name="origin">The origin of the bone's sprite.</param>
        public BoneCreatedEventArgs(Bone bone, string spriteName, Vector2 origin, float rotationOffset)
        {
            //Pass along the data.
            _Bone = bone;
            _SpriteName = spriteName;
            _SpriteOrigin = origin;
            _SpriteRotationOffset = rotationOffset;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The bone that has been created.
        /// </summary>
        public Bone Bone
        {
            get { return _Bone; }
        }
        /// <summary>
        /// The name of the bone's sprite.
        /// </summary>
        public string SpriteName
        {
            get { return _SpriteName; }
        }
        /// <summary>
        /// The origin of the bone's sprite.
        public Vector2 SpriteOrigin
        {
            get { return _SpriteOrigin; }
        }
        /// <summary>
        /// The sprite's rotation offset.
        public float SpriteRotationOffset
        {
            get { return _SpriteRotationOffset; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when a bone has been linked by an item.
    /// </summary>
    public class BoneEventArgs : EventArgs
    {
        #region Fields
        private Bone _Bone;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when a bone has been linked by an item.
        /// </summary>
        /// <param name="bone">The linked bone.</param>
        public BoneEventArgs(Bone bone)
        {
            //Pass along the data.
            _Bone = bone;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The bone that has been linked.
        /// </summary>
        public Bone Bone
        {
            get { return _Bone; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when a list item has been added to a list.
    /// </summary>
    public class ListItemAddedEventArgs : EventArgs
    {
        #region Fields
        private ListItem _Item;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when a bone has been created by an item.
        /// </summary>
        /// <param name="item">The created bone.</param>
        public ListItemAddedEventArgs(ListItem item)
        {
            //Pass along the data.
            _Item = item;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The list item that has been added to the list.
        /// </summary>
        public ListItem Item
        {
            get { return _Item; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when an animation has been wrapped up in something.
    /// </summary>
    public class AnimationEventArgs : EventArgs
    {
        #region Fields
        private Animation _Animation;
        private string _FileName;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used an animation has been wrapped up in something.
        /// </summary>
        /// <param name="animation">The animation.</param>
        public AnimationEventArgs(Animation animation)
        {
            //Pass along the data.
            _Animation = animation;
        }
        /// <summary>
        /// Create the event used an animation has been wrapped up in something.
        /// </summary>
        /// <param name="fileName">The animation.</param>
        public AnimationEventArgs(string fileName)
        {
            //Pass along the data.
            _FileName = fileName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The animation.
        /// </summary>
        public Animation Animation
        {
            get { return _Animation; }
        }
        /// <summary>
        /// The animation's file name.
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when a skeleton has been wrapped up in something.
    /// </summary>
    public class SkeletonEventArgs : EventArgs
    {
        #region Fields
        private Skeleton _Skeleton;
        private string _FileName;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when a skeleton has been wrapped up in something.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        public SkeletonEventArgs(Skeleton skeleton)
        {
            //Pass along the data.
            _Skeleton = skeleton;
        }
        /// <summary>
        /// Create the event used when a skeleton has been wrapped up in something.
        /// </summary>
        /// <param name="fileName">The skeleton's file name.</param>
        public SkeletonEventArgs(string fileName)
        {
            //Pass along the data.
            _FileName = fileName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The skeleton.
        /// </summary>
        public Skeleton Skeleton
        {
            get { return _Skeleton; }
        }
        /// <summary>
        /// The skeleton's file name.
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
        }
        #endregion

    }
    /// <summary>
    /// An event used when the bounds of something has changed.
    /// </summary>
    public class BoundsChangedEventArgs : EventArgs
    {
        #region Fields
        private float _Width;
        private float _Height;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when the bounds of something has changed.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public BoundsChangedEventArgs(float width, float height)
        {
            //Pass along the data.
            _Width = width;
            _Height = height;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The new width.
        /// </summary>
        public float Width
        {
            get { return _Width; }
        }
        /// <summary>
        /// The new height.
        /// </summary>
        public float Height
        {
            get { return _Height; }
        }
        #endregion

    }
}
