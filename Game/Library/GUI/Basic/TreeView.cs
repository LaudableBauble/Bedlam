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

using Library.Imagery;
using Library.Infrastructure;

namespace Library.GUI.Basic
{
    /// <summary>
    /// TreeViews are used to list data in a hierarchical order.
    /// </summary>
    public class TreeView : Component
    {
        #region Fields
        private List<TreeViewNode> _Nodes;
        private SpriteFont _Font;
        private float _Indent;
        private Vector2 _ChildPosition;
        private float _ChildWidth;
        private float _ChildHeight;
        private Button _MoveNodeUp;
        private Button _MoveNodeDown;
        private TreeViewNode _SelectedNode;

        public delegate void TickHandler(object obj, TickEventArgs e);
        public event TickHandler Ticked;
        #endregion

        #region Indexers
        /// <summary>
        /// Get or set an node.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <returns>The node instance.</returns>
        public TreeViewNode this[int index]
        {
            get { return (_Nodes[index]); }
            set { _Nodes[index] = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a treeview.
        /// </summary>
        /// <param name="gui">The GUI that this treeview will be a part of.</param>
        /// <param name="position">The position of this treeview.</param>
        /// <param name="height">The height of this treeview.</param>
        /// <param name="width">The width of this treeview.</param>
        public TreeView(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //Initialize some variables.
            Initialize(gui, position, width, height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the tree view.
        /// </summary>
        /// <param name="gui">The GUI that this treeview will be a part of.</param>
        /// <param name="position">The position of this treeview.</param>
        /// <param name="height">The height of this treeview.</param>
        /// <param name="width">The width of this treeview.</param>
        public override void Initialize(GraphicalUserInterface gui, Vector2 position, float width, float height)
        {
            //The inherited method.
            base.Initialize(gui, position, width, height);

            //Intialize some variables.
            _Nodes = new List<TreeViewNode>();
            _Indent = 15;
            _ChildPosition = new Vector2((Position.X + _Indent), (Position.Y + 10));
            _ChildWidth = 200;
            _ChildHeight = 15;
            _MoveNodeUp = new Button(gui, new Vector2((position.X + (width / 4)), (position.Y + 5)));
            _MoveNodeDown = new Button(gui, new Vector2((position.X + (width / 4) + 20), (position.Y + 5)));

            //Add the controls.
            Add(_MoveNodeDown);
            Add(_MoveNodeUp);

            //Hook up some events.
            _MoveNodeUp.MouseClick += OnMoveNodeUpClick;
            _MoveNodeDown.MouseClick += OnMoveNodeDownClick;
        }
        /// <summary>
        /// Load the content of this treeview.
        /// </summary>
        public override void LoadContent()
        {
            //The inherited method.
            base.LoadContent();

            //Create the treeview's texture and load the font.
            CreateTexture();
            _Font = GUI.ContentManager.Load<SpriteFont>("GameScreen/Fonts/diagnosticFont");

            //Add the sprites while we're at it.
            Sprite moveUp = _MoveNodeUp.AddSprite("UI/Textures/arrow_fat_up");
            Sprite moveDown = _MoveNodeDown.AddSprite("UI/Textures/arrow_fat_down");

            //Update the button's bounds.
            _MoveNodeUp.Width = moveUp[0].Width;
            _MoveNodeUp.Height = moveUp[0].Height;
            _MoveNodeDown.Width = moveDown[0].Width;
            _MoveNodeDown.Height = moveDown[0].Height;
        }
        /// <summary>
        /// Update the treeview.
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
        /// Draw the treeview and all its nodes.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Add a treeview node to the list.
        /// </summary>
        public void AddNode()
        {
            //The new tree view node.
            TreeViewNode node = new TreeViewNode(GUI, null, _ChildPosition, _ChildWidth, _ChildHeight);

            //Add the child node to the list of other nodes, but also the list of other items.
            Add(node);
            _Nodes.Add(node);

            //Load the node's content.
            if (GUI.ContentManager != null) { node.LoadContent(); }

            //Hook up some events.
            node.ChildNodeAdded += OnChildNodeAdded;
            node.NodeStateChanged += OnChildNodeStateChanged;
            node.MouseClick += OnChildNodeMouseClick;
            node.Ticked += OnChildNodeTicked;
        }
        /// <summary>
        /// Insert a node.
        /// </summary>
        /// <param name="index">The index of where to insert the node.</param>
        /// <param name="node">The node to insert.</param>
        public void InsertNode(int index, TreeViewNode node)
        {
            //Add the node to the list of other items.
            Add(node);
            //Insert the child node to the list of other nodes.
            _Nodes.Insert(index, node);

            //Hook up some events.
            node.ChildNodeAdded += OnChildNodeAdded;
            node.NodeStateChanged += OnChildNodeStateChanged;
            node.MouseClick += OnChildNodeMouseClick;
            node.Ticked += OnChildNodeTicked;
        }
        /// <summary>
        /// Update the all nodes' position in the list.
        /// </summary>
        public void UpdateTree()
        {
            //The position furthest down the list.
            Vector2 last = _ChildPosition;

            //Loop through all nodes and update their positions.
            foreach (TreeViewNode node in _Nodes)
            {
                //Calculate their new position, but only if they are visible.
                if (node.IsVisible) { last = node.UpdateTree(new Vector2(last.X, (last.Y + 15)), _Indent); }
            }
        }
        /// <summary>
        /// Create a new background texture for the item.
        /// </summary>
        private void CreateTexture()
        {
            try
            {
                //The texture.
                Texture2D texture = DrawingHelper.CreateRectangleTexture(GUI.GraphicsDevice, (int)Width, (int)Height, new Color(0, 0, 0, 155));

                //If there already exists a sprite, just switch a new texture to it.
                if (_Sprite.GetLastSprite() != null) { _Sprite.GetLastSprite().Texture = texture; }
                //Otherwise create a new sprite.
                else { AddSprite(texture); }
            }
            catch { }
        }
        /// <summary>
        /// Move a node upwards in the list.
        /// </summary>
        /// <param name="node">The node to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the top, this method will return false.</returns>
        public bool MoveNodeUp(TreeViewNode node)
        {
            //First see if the child node actually exists directly beneath this node.
            if (_Nodes.Exists(n => (n.Equals(node))))
            {
                //Get the index position of the child node.
                int index = _Nodes.FindIndex(n => (n.Equals(node)));

                //If the child node can climb in the list.
                if (index != 0)
                {
                    //Remove the node from the list.
                    _Nodes.Remove(node);
                    //Insert the node at one step up from its past position in the list.
                    _Nodes.Insert((index - 1), node);

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
        /// Move a node downwards in the list.
        /// </summary>
        /// <param name="node">The node to move.</param>
        /// <returns>Whether the operation was succesful or not. For instance if the node already is at the bottom, this method will return false.</returns>
        public bool MoveNodeDown(TreeViewNode node)
        {
            //First see if the child node actually exists directly beneath this node.
            if (_Nodes.Exists(n => (n.Equals(node))))
            {
                //Get the index position of the child node.
                int index = _Nodes.FindIndex(n => (n.Equals(node)));

                //If the child node can descend in the list.
                if (index != (_Nodes.Count - 1))
                {
                    //Remove the node from the list.
                    _Nodes.Remove(node);
                    //Insert the node at one step down from its past position in the list.
                    _Nodes.Insert((index + 1), node);

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
        /// Clear the tree view of all nodes.
        /// </summary>
        public void Clear()
        {
            //Clear the tree view.
            //TODO: Delete them properly.
            for (int i = 0; i < _Items.Count; i++) { if (_Items[i] is TreeViewNode) { _Items.RemoveAt(i); } }
            _Nodes.Clear();
        }
        /// <summary>
        /// Get the index of a node.
        /// </summary>
        /// <param name="node">The node in question.</param>
        /// <returns>The index of the node.</returns>
        public int GetNodeIndex(TreeViewNode node)
        {
            //Return the index.
            return (_Nodes.IndexOf(node));
        }
        /// <summary>
        /// See if a certain node exists in the tree.
        /// </summary>
        /// <param name="node">The node in question.</param>
        /// <param name="surfaceScratchOnly">Whether to only look at the first layer of nodes or all the way down.</param>
        /// <returns>The index of the node.</returns>
        public bool Contains(TreeViewNode node, bool surfaceScratchOnly)
        {
            //If the specified node exists directly underneath this one.
            if (_Nodes.Contains(node)) { return true; }

            //If allowed to go deep.
            if (!surfaceScratchOnly) { foreach (TreeViewNode n in _Nodes) { if (n.Contains(node, false)) { return true; } } }

            //Return false, no one was found. I bet your face looks appropriately sunken and puffy at this time, complete with eyes red from crying.
            return false;
        }
        /// <summary>
        /// If any node has had a child node added to it, update the tree view.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnChildNodeAdded(object obj, ChildNodeAddedEventArgs e)
        {
            //Hook up all kinds of events to it.
            e.Child.ChildNodeAdded += OnChildNodeAdded;
            e.Child.NodeStateChanged += OnChildNodeStateChanged;
            e.Child.MouseClick += OnChildNodeMouseClick;
            e.Child.Ticked += OnChildNodeTicked;

            //Update all node's positions.
            UpdateTree();
        }
        /// <summary>
        /// A node has been clicked on by a mouse.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnChildNodeMouseClick(object obj, MouseClickEventArgs e)
        {
            //Save the clicked node as the selected node.
            _SelectedNode = (obj as TreeViewNode);
        }
        /// <summary>
        /// If any node has been either collapsed or expanded, update the tree view.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnChildNodeStateChanged(object obj, EventArgs e)
        {
            //Update all node's positions.
            UpdateTree();
        }
        /// <summary>
        /// If any node has been ticked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnChildNodeTicked(object obj, TickEventArgs e)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (Ticked != null) { Ticked(obj, e); }
        }
        /// <summary>
        /// When the user clicks on the move node up button.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMoveNodeUpClick(object obj, EventArgs e)
        {
            //If there even is a node selected.
            if (_SelectedNode != null)
            {
                //If the node has a parent.
                if (_SelectedNode.ParentNode != null)
                {
                    //If the node's parent fails in moving its child up the list.
                    if (!_SelectedNode.ParentNode.MoveChildNodeUp(_SelectedNode))
                    {
                        //Remove the node from its parent.
                        _SelectedNode.ParentNode.ChildNodes.Remove(_SelectedNode);
                        //If the parent node ends up with an empty list, change its node state.
                        if (_SelectedNode.ParentNode.ChildNodes.Count == 0) { _SelectedNode.ParentNode.NodeState = TreeViewNodeState.None; }

                        //If the node has a grandparent.
                        if (_SelectedNode.ParentNode.ParentNode != null)
                        {
                            //Insert the node at the node's parent's parent.
                            _SelectedNode.ParentNode.ParentNode.InsertNode(
                                Math.Max(_SelectedNode.ParentNode.ParentNode.ChildNodes.FindIndex(parent => (parent.Equals(_SelectedNode.ParentNode))), 0), _SelectedNode);
                            //Update the node's parent.
                            _SelectedNode.ParentNode = _SelectedNode.ParentNode.ParentNode;
                        }
                        //If the node doesn't have a grandparent, position it directly under this item.
                        else
                        {
                            //Insert the node at the this item.
                            _Nodes.Insert(Math.Max(_Nodes.FindIndex(parent => (parent.Equals(_SelectedNode.ParentNode))), 0), _SelectedNode);
                            //Update the node's parent.
                            _SelectedNode.ParentNode = null;
                        }
                    }
                }
                //If the node doesn't have a parent it is positioned directly under this item, which means that we move it up in the tree from here.
                else { MoveNodeUp(_SelectedNode); }
            }

            //Finally update the tree and all nodes' positions.
            UpdateTree();
        }
        /// <summary>
        /// When the user clicks on the move node down button.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMoveNodeDownClick(object obj, EventArgs e)
        {
            //If there even is a node selected.
            if (_SelectedNode != null)
            {
                //If the node has a parent.
                if (_SelectedNode.ParentNode != null)
                {
                    //If the node's parent fails in moving its child down the list.
                    if (!_SelectedNode.ParentNode.MoveChildNodeDown(_SelectedNode))
                    {
                        //Remove the node from its parent.
                        _SelectedNode.ParentNode.ChildNodes.Remove(_SelectedNode);
                        //If the parent node ends up with an empty list, change its node state.
                        if (_SelectedNode.ParentNode.ChildNodes.Count == 0) { _SelectedNode.ParentNode.NodeState = TreeViewNodeState.None; }

                        //If the node has a grandparent.
                        if (_SelectedNode.ParentNode.ParentNode != null)
                        {
                            //Insert the node at the node's parent's parent.
                            _SelectedNode.ParentNode.ParentNode.InsertNode(
                                Math.Min((_SelectedNode.ParentNode.ParentNode.ChildNodes.FindIndex(parent => (parent.Equals(_SelectedNode.ParentNode))) + 1),
                                (_SelectedNode.ParentNode.ParentNode.ChildNodes.Count - 1)), _SelectedNode);
                            //Update the node's parent.
                            _SelectedNode.ParentNode = _SelectedNode.ParentNode.ParentNode;
                        }
                        //If the node doesn't have a grandparent, position it directly under this item.
                        else
                        {
                            //Insert the node at the this item.
                            _Nodes.Insert(Math.Max((_Nodes.FindIndex(parent => (parent.Equals(_SelectedNode.ParentNode))) + 1), (_Nodes.Count - 1)), _SelectedNode);
                            //Update the node's parent.
                            _SelectedNode.ParentNode = null;
                        }
                    }
                }
                //If the node doesn't have a parent it is positioned directly under this item, which means that we move it down in the tree from here.
                else { MoveNodeDown(_SelectedNode); }
            }

            //Finally update the tree and all nodes' positions.
            UpdateTree();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The nodes that are stored in this treeview.
        /// </summary>
        public List<TreeViewNode> Nodes
        {
            get { return _Nodes; }
            set { _Nodes = value; }
        }
        /// <summary>
        /// The font that is used by this textbox.
        /// </summary>
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        #endregion
    }
}
