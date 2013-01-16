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

namespace Library.GUI.Basic
{
    /// <summary>
    /// Tree nodes are used to populate a tree view with data.
    /// </summary>
    public class TreeNode : Component
    {
        #region Fields
        private TreeNode _ParentNode;
        private List<TreeNode> _Nodes;
        private Button _Button;
        private Checkbox _Checkbox;
        private TreeNodeState _NodeState;
        #endregion

        #region Events
        public delegate void ChildNodeAddedHandler(object obj, ChildNodeAddedEventArgs e);
        public delegate void NodeStateChangedHandler(object obj, EventArgs e);
        public delegate void TickHandler(object obj, TickEventArgs e);

        /// <summary>
        /// An event fired when the node has been ticked.
        /// </summary>
        public event TickHandler Ticked;
        /// <summary>
        /// An event fired when a child node has been added.
        /// </summary>
        public event ChildNodeAddedHandler ChildNodeAdded;
        /// <summary>
        /// An event fired when the state of this node has changed; ie. either collapsed or expanded.
        /// </summary>
        public event NodeStateChangedHandler NodeStateChanged;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an node.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <returns>The node instance.</returns>
        public TreeNode this[int index]
        {
            get { return (_Nodes[index]); }
            set { _Nodes[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a treeview node.
        /// </summary>
        /// <param name="gui">The GUI that this node will be a part of.</param>
        /// <param name="parent">The node's parent node.</param>
        /// <param name="position">The position of this node.</param>
        /// <param name="height">The height of this node.</param>
        /// <param name="width">The width of this node.</param>
        public TreeNode(GraphicalUserInterface gui, TreeNode parent, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            _ParentNode = parent;
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the treeview node.
        /// </summary>
        /// <param name="gui">The GUI that this node will be a part of.</param>
        /// <param name="position">The position of this trenodeeview.</param>
        /// <param name="height">The height of this treenodeview.</param>
        /// <param name="width">The width of this treevnodeiew.</param>
        protected override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Nodes = new List<TreeNode>();
            _Button = new Button(gui, new Vector2(Position.X, Position.Y + (Height / 3)));
            _Checkbox = new Checkbox(gui, new Vector2(Position.X + _Button.Width + 2, Position.Y), width, height);
            _NodeState = TreeNodeState.None;

            //Add the items to the list.
            Add(_Button);
            Add(_Checkbox);

            //Hook up some events.
            _Checkbox.CheckboxTick += OnCheckboxTicked;
            _Button.MouseClick += OnButtonMouseClick;
        }
        /// <summary>
        /// Load the content of this treeview node.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Add textures to the button.
            Sprite button = _Button.AddSprite("UI/Textures/arrow_expand");
            button.AddFrame("UI/Textures/arrow_collapse");
            button.EnableAnimation = false;

            //Hook up some events.
            button.FrameChanged += new Sprite.FrameChangedHandler(OnButtonFrameChange);

            //Update the button's bounds.
            _Button.Width = button[0].Width;
            _Button.Height = button[0].Height;

            //Update the positions of all the checkbox because of the button's changed bounds.
            _Checkbox.Position = new Vector2(Position.X + _Button.Width + 2, Position.Y);
        }

        /// <summary>
        /// Add a child node.
        /// </summary>
        /// <returns>The added node.</returns>
        public TreeNode AddNode()
        {
            return AddNode(Width, Height);
        }
        /// <summary>
        /// Add a child node.
        /// </summary>
        /// <param name="width">The width of the child node.</param>
        /// <param name="height">The height of the child node.</param>
        /// <returns>The added node.</returns>
        public TreeNode AddNode(float width, float height)
        {
            //Add the child node to the list of other nodes.
            TreeNode node = new TreeNode(GUI, this, new Vector2(Position.X + 15, Position.Y + 15), width, height);
            Add(node);
            _Nodes.Add(node);

            //Let the world now you just added a child node.
            ChildNodeAddedInvoke(node);

            //Return the node.
            return node;
        }
        /// <summary>
        /// Insert a child node.
        /// </summary>
        /// <param name="index">The index of where to insert the child node.</param>
        /// <param name="childNode">The child node to insert.</param>
        public void InsertNode(int index, TreeNode childNode)
        {
            //Insert the child node to the list of other nodes.
            _Nodes.Insert(index, childNode);
            Add(childNode);

            //Let the world now you just added a child node.
            ChildNodeAddedInvoke(_Nodes[index]);
        }
        /// <summary>
        /// Update all nodes' positions in the list.
        /// </summary>
        /// <param name="position">The updated position of this node.</param>
        /// <param name="indent">The amount of pixels the child nodes will be moved to the right.</param>
        /// <returns>The position of the last node.</returns>
        public Vector2 UpdateTree(Vector2 position, float indent)
        {
            //Update the node's and checkbox's position.
            Position = position;
            _Checkbox.Position = new Vector2((Position.X + _Button.Width + 2), Position.Y);
            _Button.Position = new Vector2(Position.X, (Position.Y + (Height / 3)));

            //The position furthest down the list.
            Vector2 last = new Vector2((position.X + indent), position.Y);

            //Loop through all nodes and update their positions.
            foreach (TreeNode node in _Nodes)
            {
                //Calculate their new position, but only if they are visible.
                if (node.IsVisible) { last = node.UpdateTree(new Vector2(last.X, (last.Y + 15)), indent); }
            }

            //Return the last node's position, but do not forget to remove the indent used on the child nodes.
            return new Vector2(position.X, last.Y);
        }
        /// <summary>
        /// Get the index of a node.
        /// </summary>
        /// <param name="node">The node in question.</param>
        /// <returns>The index of the node.</returns>
        public int GetNodeIndex(TreeNode node)
        {
            return _Nodes.IndexOf(node);
        }
        /// <summary>
        /// See if a certain node exists underneath this one.
        /// </summary>
        /// <param name="node">The node in question.</param>
        /// <param name="surfaceScratchcOnly">Whether to only look at the nodes directly under this one or all the way down.</param>
        /// <returns>The index of the node.</returns>
        public bool Contains(TreeNode node, bool surfaceScratchcOnly)
        {
            //If the specified node exists directly underneath this one.
            if (_Nodes.Contains(node)) { return true; }

            //If allowed to go deep.
            if (!surfaceScratchcOnly) { foreach (TreeNode n in _Nodes) { if (n.Contains(node, false)) { return true; } } }

            //Return false, no one was found.
            return false;
        }
        /// <summary>
        /// Check all child node's checkboxes.
        /// </summary>
        /// <param name="isChecked">If the child nodes should be ticked or not.</param>
        private void NodeTickedInvoke(bool isChecked)
        {
            //Loop through all nodes and tick their checkboxes.
            foreach (TreeNode node in _Nodes) { node.IsTicked = isChecked; }

            //If someone has hooked up a delegate to the event, fire it.
            if (Ticked != null) { Ticked(this, new TickEventArgs(isChecked)); }
        }
        /// <summary>
        /// If the node's checkbox has been either ticked or not, update all child nodes' checkboxes as well.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCheckboxTicked(object obj, TickEventArgs e)
        {
            NodeTickedInvoke(e.IsChecked);
        }
        /// <summary>
        /// If the node has had a child node added to it, see if it has to expand itself and then fire the event.
        /// </summary>
        private void ChildNodeAddedInvoke(TreeNode child)
        {
            //If this was the first child node added, expand the node and then fire the event.
            if (_Nodes.Count == 1) { _NodeState = TreeNodeState.Expanded; }

            //If someone has hooked up a delegate to the event, fire it.
            if (ChildNodeAdded != null) { ChildNodeAdded(this, new ChildNodeAddedEventArgs(this, child)); }
        }
        /// <summary>
        /// If the node has been either collapsed or expanded, fire an event.
        /// </summary>
        /// <param name="state">The state of this node in the tree view; that is whether it is collapsed, expanded or neither.</param>
        private void NodeStateChangedInvoke(TreeNodeState state)
        {
            //Change the state.
            _NodeState = state;
            //Update the childrens' states.
            CollapseOrExpand();

            //If someone has hooked up a delegate to the event, fire it.
            if (NodeStateChanged != null) { NodeStateChanged(this, new EventArgs()); }
        }
        /// <summary>
        /// As the button's sprite has changed frontal frame it is time to update the button's bounds to accomodate.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonFrameChange(object obj, EventArgs e)
        {
            //Update the button's bounds.
            _Button.Width = _Button.Sprite[0][_Button.Sprite[0].CurrentFrameIndex].Width;
            _Button.Height = _Button.Sprite[0][_Button.Sprite[0].CurrentFrameIndex].Height;
        }
        /// <summary>
        /// If the node's button has been clicked, either expand or collapse its list of children.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonMouseClick(object obj, MouseClickEventArgs e)
        {
            //Change the sprite of the button.
            if (_Button.Sprite[0].CurrentFrameIndex < (_Button.Sprite[0].Frames.Count - 1)) { _Button.Sprite[0].IncrementFrameIndex(); }
            else { _Button.Sprite[0].CurrentFrameIndex = 0; }

            //Change the NodeState.
            if (_NodeState == TreeNodeState.Collapsed) { NodeStateChangedInvoke(TreeNodeState.Expanded); }
            else { NodeStateChangedInvoke(TreeNodeState.Collapsed); }
        }
        /// <summary>
        /// Either collapse or expand the child nodes.
        /// </summary>
        public void CollapseOrExpand()
        {
            //Now either hide or show the list of child nodes.
            switch (_NodeState)
            {
                case (TreeNodeState.Collapsed): { foreach (TreeNode node in _Nodes) { node.IsVisible = false; } _Button.IsVisible = true; break; }
                case (TreeNodeState.Expanded): { foreach (TreeNode node in _Nodes) { node.IsVisible = true; } _Button.IsVisible = true; break; }
                case (TreeNodeState.None): { _Button.IsVisible = false; break; }
            }
        }
        /// <summary>
        /// Move a child node upwards in the list.
        /// </summary>
        /// <param name="childNode">The child node to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the top, this method will return false.</returns>
        public bool MoveChildNodeUp(TreeNode childNode)
        {
            //First see if the child node actually exists directly beneath this node.
            if (_Nodes.Exists(node => (node.Equals(childNode))))
            {
                //Get the index position of the child node.
                int childIndex = _Nodes.FindIndex(node => (node.Equals(childNode)));

                //If the child node can climb in the list.
                if (childIndex != 0)
                {
                    //Remove the node from the list.
                    _Nodes.Remove(childNode);
                    //Insert the node at one step up from its past position in the list.
                    _Nodes.Insert((childIndex - 1), childNode);

                    //Wrap it all up by returning true.
                    return true;
                }
                //Otherwise return false.
                else { return false; }
            }
            //Otherwise return false.
            else { return false; }
        }
        /// <summary>
        /// Move a child node downwards in the list.
        /// </summary>
        /// <param name="childNode">The child node to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the bottom, this method will return false.</returns>
        public bool MoveChildNodeDown(TreeNode childNode)
        {
            //First see if the child node actually exists directly beneath this node.
            if (_Nodes.Exists(node => (node.Equals(childNode))))
            {
                //Get the index position of the child node.
                int childIndex = _Nodes.FindIndex(node => (node.Equals(childNode)));

                //If the child node can descend in the list.
                if (childIndex != (_Nodes.Count - 1))
                {
                    //Remove the node from the list.
                    _Nodes.Remove(childNode);
                    //Insert the node at one step down from its past position in the list.
                    _Nodes.Insert((childIndex + 1), childNode);

                    //Wrap it all up by returning true.
                    return true;
                }
                //Otherwise return false.
                else { return false; }
            }
            //Otherwise return false.
            else { return false; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The node's parent node.
        /// </summary>
        public TreeNode ParentNode
        {
            get { return _ParentNode; }
            set { _ParentNode = value; }
        }
        /// <summary>
        /// The nodes that are stored in this treeview.
        /// </summary>
        public List<TreeNode> ChildNodes
        {
            get { return _Nodes; }
            set { _Nodes = value; }
        }
        /// <summary>
        /// The checkbox item that constitute this treeview node.
        /// </summary>
        public Checkbox Checkbox
        {
            get { return _Checkbox; }
            set { _Checkbox = value; }
        }
        public bool IsTicked
        {
            get { return _Checkbox.IsChecked; }
            set { _Checkbox.IsChecked = value; }
        }
        /// <summary>
        /// The state of this node in the tree view; that is whether it is collapsed, expanded or neither.
        /// </summary>
        public TreeNodeState NodeState
        {
            get { return _NodeState; }
            set { NodeStateChangedInvoke(value); }
        }
        #endregion
    }
}
