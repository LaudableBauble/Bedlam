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

using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;
using FarseerPhysics.DrawingSystem;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Library;
using Library.Animate;
using Library.Core;
using Library.Entities;
using Library.Enums;
using Library.Factories;
using Library.GUI;
using Library.GUI.Basic;
using Library.Imagery;
using Library.Infrastructure;

namespace Game.Editors
{
    /// <summary>
    /// The editor state describes what the editor is currently doing.
    /// </summary>
    enum EditorState
    {
        Idle,
        brush,          //"stamp mode": user double clicked on an item to add multiple instances of it
        cameramoving,   //user is moving the camera
        moving,         //user is moving an item
        rotating,       //user is rotating an item
        scaling,        //user is scaling an item
        selecting,      //user has opened a select box by dragging the mouse (windows style)
    }

    /// <summary>
    /// A level editor edits and manipulates everything that composes a level, including all its layers and items.
    /// </summary>
    public class LevelEditor
    {
        #region Fields
        private DebugSystem _DebugSystem;
        private EditorState _State;
        private Layer _SelectedLayer;
        private Item _SelectedItem;
        private Level _Level;
        private Camera2D _Camera;
        private GraphicalUserInterface _GUI;

        private TreeView _TreeView;
        private List _PropertyList;
        private ItemModifier _ItemModifier;
        private Menu _Menu;

        private LineBrush _SelectionBrush;
        private bool _IsGUIClicked;
        private Vector2 _ItemGrapplePoint;
        private FixedMouseJoint _MouseGrappleJoint;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a level editor.
        /// </summary>
        /// <param name="viewport">The window's viewport.</param>
        public LevelEditor(Rectangle viewport)
        {
            //Initialize the editor.
            Initialize(viewport);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the level editor.
        /// </summary>
        /// <param name="viewport">The window's viewport.</param>
        public void Initialize(Rectangle viewport)
        {
            //Initialize some variables.
            _State = EditorState.Idle;
            _GUI = new GraphicalUserInterface();
            _SelectedItem = null;
            _SelectedLayer = null;
            _Camera = new Camera2D(viewport, new Rectangle(0, 0, 10000, 5000));
            _Level = new Level("Level 1", _Camera);
            _DebugSystem = new DebugSystem(_Level.World);

            //Enable the debug view from start.
            _DebugSystem.Debug(true);

            //Hook up to some of the level's events.
            _Level.LayerChanged += OnLayerChanged;

            //Create the GUI items.
            _TreeView = new TreeView(_GUI, new Vector2(0, 50), 275, 450);
            _PropertyList = new List(_GUI, new Vector2(0, 505), 275, 200);
            _ItemModifier = new ItemModifier(_GUI, new Vector2(viewport.Width - 300, 50), 300, 500);
            _Menu = new Menu(_GUI, new Vector2(0, 0), 500, 30);

            //Add items to the GUI.
            _GUI.AddItem(_TreeView);
            _GUI.AddItem(_PropertyList);
            _GUI.AddItem(_ItemModifier);
            _GUI.AddItem(_Menu);

            //Brushes.
            _SelectionBrush = new LineBrush(1, Color.Gainsboro);

            #region Menu
            //Play with the menu.
            _Menu.AddMenuItem();
            _Menu.AddMenuItem();
            _Menu.AddMenuItem();
            _Menu.AddMenuItem();
            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[1].AddListItem();
            _Menu.MenuItems[1].AddListItem();
            _Menu.MenuItems[1].AddListItem();
            _Menu.MenuItems[2].AddListItem();
            _Menu.MenuItems[2].AddListItem();
            _Menu.MenuItems[2].AddListItem();
            _Menu.MenuItems[3].AddListItem();
            _Menu.MenuItems[3].AddListItem();
            _Menu.MenuItems[3].AddListItem();
            _Menu.MenuItems[0].IsScrollable = false;
            _Menu.MenuItems[1].IsScrollable = false;
            _Menu.MenuItems[2].IsScrollable = false;
            _Menu.MenuItems[3].IsScrollable = false;
            _Menu.MenuItems[0].IsFixed = false;
            _Menu.MenuItems[1].IsFixed = false;
            _Menu.MenuItems[2].IsFixed = false;
            _Menu.MenuItems[3].IsFixed = false;
            _Menu.MenuItems[0].Text = "File";
            _Menu.MenuItems[1].Text = "Edit";
            _Menu.MenuItems[2].Text = "View";
            _Menu.MenuItems[3].Text = "Physics";
            _Menu.MenuItems[0][0].Label.Text = "New Level";
            _Menu.MenuItems[0][1].Label.Text = "Open Level";
            _Menu.MenuItems[0][2].Label.Text = "Save Level";
            _Menu.MenuItems[1][0].Label.Text = "Add Item";
            _Menu.MenuItems[1][1].Label.Text = "Add Layer";
            _Menu.MenuItems[1][2].Label.Text = "Delete Layer";
            _Menu.MenuItems[2][0].Label.Text = "Zoom";
            _Menu.MenuItems[2][1].Label.Text = "Scroll";
            _Menu.MenuItems[2][2].Label.Text = "Windows";
            _Menu.MenuItems[3][0].Label.Text = "Play";
            _Menu.MenuItems[3][1].Label.Text = "Pause";
            _Menu.MenuItems[3][2].Label.Text = "Stop";
            #endregion

            //Hook up some events.
            _GUI.ItemClicked += OnGUIClicked;
            _TreeView.Ticked += OnTreeViewNodeTicked;
            _Menu.MenuOptionSelect += OnMenuOptionSelected;

            #region Test
            //Add layers.
            Layer layer1 = AddLayer("Layer 1", new Vector2(.6f, .6f));
            Layer layer2 = AddLayer("Layer 2", new Vector2(.5f, .5f));
            Layer layer3 = AddLayer("Layer 3", new Vector2(.7f, .7f));
            Layer layer4 = AddLayer("Layer 4", new Vector2(.63f, .63f));
            Layer layer5 = AddLayer("Layer 5", Vector2.One);
            Layer layer6 = AddLayer("Layer 6", new Vector2(.99f, 0.99f));
            Layer layer7 = AddLayer("Layer 7", new Vector2(1.5f, 1));

            //Add items.
            AddTextureItem(layer1, @"General/Textures/Hazy_Field[1]", "Background Hazy", new Vector2(0, 50), 0, Vector2.One);
            AddTextureItem(layer1, @"General/Textures/Hazy_Field[2]", "Background Hazy", new Vector2(0, 1000), 0, Vector2.One);
            AddTextureItem(layer1, @"General/Textures/Ruins[1]", "Background Ruins", new Vector2(2000, 50), 0, Vector2.One);
            AddTextureItem(layer2, @"General/Textures/FrozenMetalGroundV1[1]", "Ground", new Vector2(0, 700), 0, Vector2.One);
            AddTextureItem(layer2, @"General/Textures/Backdrop_Guy[1]", "Backdrop Guy", new Vector2(1900, 200), 0, Vector2.One);
            AddTextureItem(layer3, @"General/Textures/Hazy_Field_Tree[1]", "Hazy Field Tree", new Vector2(0, 1200), 0, Vector2.One);
            AddTextureItem(layer4, @"General/Textures/Hazy_Field_Pillar[1]", "Hazy Field Pillar", new Vector2(1200, 1500), 0, Vector2.One);
            AddTextureItem(layer5, @"General/Textures/Fern", "Fern 1", new Vector2(500, 500), 0, Vector2.One);
            AddTextureItem(layer5, @"General/Textures/Rock[1]", "Rock 1", new Vector2(400, 500), 0, Vector2.One);
            AddTextureItem(layer6, @"General/Textures/Fern", "Fern 2", new Vector2(515, 515), 0, Vector2.One);
            AddTextureItem(layer7, @"General/Textures/Pine_Tree", "Pine Tree", new Vector2(600, 0), 0, Vector2.One);
            AddTextureItem(layer7, @"General/Textures/Rock[2]", "Rock 2", new Vector2(300, 550), 0, Vector2.One);

            Box box1 = Factory.Instance.AddBox(layer5, "Ground", @"General/Textures/FrozenMetalGroundV1[1]", new Vector2(800, 700), 937, 32);
            box1.Limbs[0].Body.BodyType = FarseerPhysics.Dynamics.BodyType.Static;

            for (int i = 0; i <= 5; i++)
            {
                Box box2 = Factory.Instance.AddBox(layer5, "Box", @"General/Textures/BlueBoxV1[1]", new Vector2(500 + 50 * i, 50), 26, 27);
                box2.Limbs[0].Body.Restitution = .1f * i;
                //box2.Parts[0].Body.Mass = 1 + 1 * i;
            }
            #endregion
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="contentManager">The manager that handles all graphical content.</param>
        /// <param name="graphicsDevice">The device that controls the graphics.</param>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            //Load the level's and GUI's content.
            _GUI.LoadContent(graphicsDevice, contentManager, spriteBatch);
            _Level.LoadContent(graphicsDevice, contentManager);
            _DebugSystem.LoadContent(graphicsDevice, contentManager);

            //Set-up the tree view.
            SetUpTreeView();

            //Load some brushes.
            _SelectionBrush.Load(graphicsDevice);

            //Character test.
            _Level.ManageLayers();
            Character character = Factory.Instance.AddCharacter(_Level.Layers[0], "test", new Vector2(1000, 500), 100, 100);
            Limb part = Factory.Instance.AddLimb(character, BodyFactory.CreateRectangle(_Level.World, ConvertUnits.ToSimUnits(character.Width),
                ConvertUnits.ToSimUnits(character.Height), 1, ConvertUnits.ToSimUnits(character.Position)));
            part.Body.BodyType = BodyType.Dynamic;
            character.Skeleton = Helper.LoadSkeleton(graphicsDevice, contentManager.RootDirectory + @"\Editor\default");
            character.LoadContent(contentManager);
            character.Skeleton.Position = character.Position;
            Helper.LoadAnimation(character.Skeleton, contentManager.RootDirectory + @"\Editor\test3");
            character.Skeleton.Animations.RemoveAt(0);
            character.Skeleton.Animations[0].IsActive = true;
        }
        /// <summary>
        /// Update the level editor.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            #region Chaos
            //if (_Level == null) return;

            /*int mwheeldelta = mstate.ScrollWheelValue - oldmstate.ScrollWheelValue;
            if (mwheeldelta > 0)
            {
                float zoom = (float)Math.Round(camera.Scale * 10) * 10.0f + 10.0f;
                MainForm.Instance.zoomcombo.Text = zoom.ToString() + "%";
                camera.Scale = zoom / 100.0f;
            }
            if (mwheeldelta < 0)
            {
                float zoom = (float)Math.Round(camera.Scale * 10) * 10.0f - 10.0f;
                if (zoom <= 0.0f) return;
                MainForm.Instance.zoomcombo.Text = zoom.ToString() + "%";
                camera.Scale = zoom / 100.0f;
            }

            //get mouse world position considering the ScrollSpeed of the current layer
            Vector2 maincameraposition = camera.Position;
            if (SelectedLayer != null) camera.Position *= SelectedLayer.ScrollSpeed;
            mouseworldpos = Vector2.Transform(new Vector2(mstate.X, mstate.Y), Matrix.Invert(camera.matrix));
            mouseworldpos = mouseworldpos.Round();
            MainForm.Instance.toolStripStatusLabel3.Text = "Mouse: (" + mouseworldpos.X + ", " + mouseworldpos.Y + ")";
            camera.Position = maincameraposition;*/

            #region Idle
            if (_State == EditorState.Idle)
            {
                //get item under mouse cursor
                /*LevelItem item = getItemAtPos(mouseworldpos);
                if (item != null)
                {
                    MainForm.Instance.toolStripStatusLabel1.Text = item.Name;
                    item.onMouseOver(mouseworldpos);
                    if (kstate.IsKeyDown(Keys.LeftControl)) MainForm.Instance.pictureBox1.Cursor = cursorDup;
                }
                else
                {
                    MainForm.Instance.toolStripStatusLabel1.Text = "";
                }
                if (item != lastitem && lastitem != null) lastitem.onMouseOut();
                lastitem = item;

                //LEFT MOUSE BUTTON CLICK
                if ((mstate.LeftButton == ButtonState.Pressed && oldmstate.LeftButton == ButtonState.Released) ||
                    (kstate.IsKeyDown(Keys.D1) && oldkstate.IsKeyUp(Keys.D1)))
                {
                    if (item != null) item.onMouseButtonDown(mouseworldpos);
                    if (kstate.IsKeyDown(Keys.LeftControl) && item != null)
                    {
                        if (!SelectedItems.Contains(item)) selectitem(item);

                        beginCommand("Add Item(s)");

                        List<LevelItem> selecteditemscopy = new List<LevelItem>();
                        foreach (LevelItem selitem in SelectedItems)
                        {
                            LevelItem i2 = (LevelItem)selitem.clone();
                            selecteditemscopy.Add(i2);
                        }
                        foreach (LevelItem selitem in selecteditemscopy)
                        {
                            selitem.Name = selitem.getNamePrefix() + level.getNextItemNumber();
                            addItem(selitem);
                        }
                        selectitem(selecteditemscopy[0]);
                        updatetreeview();
                        for (int i = 1; i < selecteditemscopy.Count; i++) SelectedItems.Add(selecteditemscopy[i]);
                        startMoving();
                    }
                    else if (kstate.IsKeyDown(Keys.LeftShift) && item != null)
                    {
                        if (SelectedItems.Contains(item)) SelectedItems.Remove(item);
                        else SelectedItems.Add(item);
                    }
                    else if (SelectedItems.Contains(item))
                    {
                        beginCommand("Change Item(s)");
                        startMoving();
                    }
                    else if (!SelectedItems.Contains(item))
                    {
                        selectitem(item);
                        if (item != null)
                        {
                            beginCommand("Change Item(s)");
                            startMoving();
                        }
                        else
                        {
                            grabbedpoint = mouseworldpos;
                            selectionrectangle = Rectangle.Empty;
                            state = EditorState.selecting;
                        }

                    }
                }

                //MIDDLE MOUSE BUTTON CLICK
                if ((mstate.MiddleButton == ButtonState.Pressed && oldmstate.MiddleButton == ButtonState.Released) ||
                    (kstate.IsKeyDown(Keys.D2) && oldkstate.IsKeyUp(Keys.D2)))
                {
                    if (item != null) item.onMouseOut();
                    if (kstate.IsKeyDown(Keys.LeftControl))
                    {
                        grabbedpoint = new Vector2(mstate.X, mstate.Y);
                        initialcampos = camera.Position;
                        state = EditorState.cameramoving;
                        MainForm.Instance.pictureBox1.Cursor = Forms.Cursors.SizeAll;
                    }
                    else
                    {
                        if (SelectedItems.Count > 0)
                        {
                            grabbedpoint = mouseworldpos - SelectedItems[0].pPosition;

                            //save the initial rotation for each item
                            initialrot.Clear();
                            foreach (LevelItem selitem in SelectedItems)
                            {
                                if (selitem.CanRotate())
                                {
                                    initialrot.Add(selitem.getRotation());
                                }
                            }

                            state = EditorState.rotating;
                            MainForm.Instance.pictureBox1.Cursor = cursorRot;

                            beginCommand("Rotate Item(s)");
                        }
                    }
                }

                //RIGHT MOUSE BUTTON CLICK
                if ((mstate.RightButton == ButtonState.Pressed && oldmstate.RightButton == ButtonState.Released) ||
                    (kstate.IsKeyDown(Keys.D3) && oldkstate.IsKeyUp(Keys.D3)))
                {
                    if (item != null) item.onMouseOut();
                    if (SelectedItems.Count > 0)
                    {
                        grabbedpoint = mouseworldpos - SelectedItems[0].pPosition;

                        //save the initial scale for each item
                        initialscale.Clear();
                        foreach (LevelItem selitem in SelectedItems)
                        {
                            if (selitem.CanScale())
                            {
                                initialscale.Add(selitem.getScale());
                            }
                        }

                        state = EditorState.scaling;
                        MainForm.Instance.pictureBox1.Cursor = cursorScale;

                        beginCommand("Scale Item(s)");
                    }
                }

                if (kstate.IsKeyDown(Keys.H) && oldkstate.GetPressedKeys().Length == 0 && SelectedItems.Count > 0)
                {
                    beginCommand("Flip Item(s) Horizontally");
                    foreach (LevelItem selitem in SelectedItems)
                    {
                        if (selitem is TextureItem)
                        {
                            TextureItem ti = (TextureItem)selitem;
                            ti.FlipHorizontally = !ti.FlipHorizontally;
                        }
                    }
                    MainForm.Instance.propertyGrid1.Refresh();
                    endCommand();
                }
                if (kstate.IsKeyDown(Keys.V) && oldkstate.GetPressedKeys().Length == 0 && SelectedItems.Count > 0)
                {
                    beginCommand("Flip Item(s) Vertically");
                    foreach (LevelItem selitem in SelectedItems)
                    {
                        if (selitem is TextureItem)
                        {
                            TextureItem ti = (TextureItem)selitem;
                            ti.FlipVertically = !ti.FlipVertically;
                        }
                    }
                    MainForm.Instance.propertyGrid1.Refresh();
                    endCommand();
                }*/
            }
            #endregion

            #region Moving
            if (_State == EditorState.moving)
            {
                /*int i = 0;
                foreach (LevelItem selitem in SelectedItems)
                {
                    newPosition = initialpos[i] + mouseworldpos - grabbedpoint;
                    if (Constants.Instance.SnapToGrid || kstate.IsKeyDown(Keys.G)) newPosition = snapToGrid(newPosition);
                    drawSnappedPoint = false;
                    selitem.setPosition(newPosition);
                    i++;
                }
                MainForm.Instance.propertyGrid1.Refresh();
                if ((mstate.LeftButton == ButtonState.Released && oldmstate.LeftButton == ButtonState.Pressed) ||
                    (kstate.IsKeyUp(Keys.D1) && oldkstate.IsKeyDown(Keys.D1)))
                {

                    foreach (LevelItem selitem in SelectedItems) selitem.onMouseButtonUp(mouseworldpos);

                    state = EditorState.idle;
                    MainForm.Instance.pictureBox1.Cursor = Forms.Cursors.Default;
                    if (mouseworldpos != grabbedpoint) endCommand(); else abortCommand();
                }*/
            }
            #endregion

            #region Rotating
            if (_State == EditorState.rotating)
            {
                /*Vector2 newpos = mouseworldpos - SelectedItems[0].pPosition;
                float deltatheta = (float)Math.Atan2(grabbedpoint.Y, grabbedpoint.X) - (float)Math.Atan2(newpos.Y, newpos.X);
                int i = 0;
                foreach (LevelItem selitem in SelectedItems)
                {
                    if (selitem.CanRotate())
                    {
                        selitem.setRotation(initialrot[i] - deltatheta);
                        if (kstate.IsKeyDown(Keys.LeftControl))
                        {
                            selitem.setRotation((float)Math.Round(selitem.getRotation() / MathHelper.PiOver4) * MathHelper.PiOver4);
                        }
                        i++;
                    }
                }
                MainForm.Instance.propertyGrid1.Refresh();
                if ((mstate.MiddleButton == ButtonState.Released && oldmstate.MiddleButton == ButtonState.Pressed) ||
                    (kstate.IsKeyUp(Keys.D2) && oldkstate.IsKeyDown(Keys.D2)))
                {
                    state = EditorState.idle;
                    MainForm.Instance.pictureBox1.Cursor = Forms.Cursors.Default;
                    endCommand();
                }*/
            }
            #endregion

            #region Scaling
            if (_State == EditorState.scaling)
            {
                /*Vector2 newdistance = mouseworldpos - SelectedItems[0].pPosition;
                float factor = newdistance.Length() / grabbedpoint.Length();
                int i = 0;
                foreach (LevelItem selitem in SelectedItems)
                {
                    if (selitem.CanScale())
                    {
                        if (selitem is TextureItem)
                        {
                            MainForm.Instance.toolStripStatusLabel1.Text = "Hold down [X] or [Y] to limit scaling to the according dimension.";
                        }

                        Vector2 newscale = initialscale[i];
                        if (!kstate.IsKeyDown(Keys.Y)) newscale.X = initialscale[i].X * (((factor - 1.0f) * 0.5f) + 1.0f);
                        if (!kstate.IsKeyDown(Keys.X)) newscale.Y = initialscale[i].Y * (((factor - 1.0f) * 0.5f) + 1.0f);
                        selitem.setScale(newscale);

                        if (kstate.IsKeyDown(Keys.LeftControl))
                        {
                            Vector2 scale;
                            scale.X = (float)Math.Round(selitem.getScale().X * 10) / 10;
                            scale.Y = (float)Math.Round(selitem.getScale().Y * 10) / 10;
                            selitem.setScale(scale);
                        }
                        i++;
                    }
                }
                MainForm.Instance.propertyGrid1.Refresh();
                if ((mstate.RightButton == ButtonState.Released && oldmstate.RightButton == ButtonState.Pressed) ||
                    (kstate.IsKeyUp(Keys.D3) && oldkstate.IsKeyDown(Keys.D3)))
                {
                    state = EditorState.idle;
                    MainForm.Instance.pictureBox1.Cursor = Forms.Cursors.Default;
                    endCommand();
                }*/
            }
            #endregion

            #region Camera Moving
            if (_State == EditorState.cameramoving)
            {
                /*Vector2 newpos = new Vector2(mstate.X, mstate.Y);
                Vector2 distance = (newpos - grabbedpoint) / camera.Scale;
                if (distance.Length() > 0)
                {
                    camera.Position = initialcampos - distance;
                }
                if (mstate.MiddleButton == ButtonState.Released)
                {
                    state = EditorState.idle;
                    MainForm.Instance.pictureBox1.Cursor = Forms.Cursors.Default;
                }*/
            }
            #endregion
            #endregion

            //The GUI hasn't been clicked yet this step.
            _IsGUIClicked = false;
            //Update the property list.
            UpdatePropertyList();

            //Update the level.
            _Level.Update(gameTime);
            //Update the GUI.
            _GUI.Update(gameTime);
        }
        /// <summary>
        /// Handle user input.
        /// </summary>
        /// <param name="input">The helper for reading input from the user.</param>
        public void HandleInput(InputState input)
        {
            //Let the GUI handle user input.
            _GUI.HandleInput(input);
            //Enable the DebugSystem to handle input.
            _DebugSystem.HandleInput(input);

            #region Camera
            //If the CTRL button is not held down.
            if (!input.IsKeyDown(Keys.LeftControl))
            {
                //Manage the camera movement.
                if (input.IsKeyDown(Keys.W)) { MoveCamera(new Vector2(0, -1)); }
                if (input.IsKeyDown(Keys.S)) { MoveCamera(new Vector2(0, 1)); }
                if (input.IsKeyDown(Keys.A)) { MoveCamera(new Vector2(-1, 0)); }
                if (input.IsKeyDown(Keys.D)) { MoveCamera(new Vector2(1, 0)); }

                //Let the user zoom in and out.
                if (input.IsKeyDown(Keys.Z)) { _Camera.Zoom(-.05f); }
                if (input.IsKeyDown(Keys.X)) { _Camera.Zoom(.05f); }
            }
            #endregion

            #region Editing
            //If the CTRL button is down.
            if (input.IsKeyDown(Keys.LeftControl))
            {
                //Manage the item movement.
                if (input.IsKeyDown(Keys.W)) { MoveItem(new Vector2(0, -5)); }
                if (input.IsKeyDown(Keys.S)) { MoveItem(new Vector2(0, 5)); }
                if (input.IsKeyDown(Keys.A)) { MoveItem(new Vector2(-5, 0)); }
                if (input.IsKeyDown(Keys.D)) { MoveItem(new Vector2(5, 0)); }

                //Let the user rotate the item.
                if (input.IsKeyDown(Keys.Q)) { RotateItem(-.01f); }
                if (input.IsKeyDown(Keys.E)) { RotateItem(.01f); }

                //Let the user scale the item.
                if (input.IsKeyDown(Keys.Z)) { ScaleItem(new Vector2(-.05f, -.05f)); }
                if (input.IsKeyDown(Keys.X)) { ScaleItem(new Vector2(.05f, .05f)); }

                //Let the user copy the item.
                if (input.IsNewKeyPress(Keys.V)) { CopyItem(Helper.GetMousePosition()); }
            }

            //If the GUI hasn't been clicked.
            if (!_IsGUIClicked)
            {
                //Select an item.
                if (input.IsNewLeftMouseClick()) { SelectItem(GetItemAtPosition(Helper.GetMousePosition())); }

                //If the user wants to seize and move an item with the mouse.
                if (input.IsNewLeftMousePress() && _SelectedItem != null)
                {
                    //Handle the item grapple.
                    GrappleItem();
                }
            }

            //If the user disengages the grapple function.
            if (input.IsNewLeftMouseReleased())
            {
                //If the grapple joint exists.
                if (_MouseGrappleJoint != null)
                {
                    //Remove the grapple joint from the world simulation.
                    _Level.World.RemoveJoint(_MouseGrappleJoint);
                    _MouseGrappleJoint = null;
                }

                //Reset the grapple point.
                _ItemGrapplePoint = Vector2.Zero;
            }
            #endregion

            //Quickie.
            if (input.IsKeyDown(Keys.N)) { SetUpTreeView(); }
            if (input.IsNewKeyPress(Keys.M)) { ToggleGUI(); }

            //Let the level handle user input.
            _Level.HandleInput(input);
        }
        /// <summary>
        /// Draw the level editor and all its items.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the level.
            _Level.Draw(spriteBatch);

            //If an item has been selected, highlight it.
            if (_SelectedItem != null)
            {
                //Highlight the selected item.
                spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, _SelectedLayer.CameraMatrix);
                DrawSelectionForm(spriteBatch);
                spriteBatch.End();
            }

            //Begin drawing.
            spriteBatch.Begin();

            //Draw the DebugSystem.
            _DebugSystem.Draw(spriteBatch, _Camera.Projection, _Camera.TransformSimulationMatrix());
            //Draw the GUI.
            _GUI.Draw();

            //End drawing.
            spriteBatch.End();
        }

        /// <summary>
        /// Add a layer to the level.
        /// </summary>
        /// <param name="layer">The level to add.</param>
        public Layer AddLayer(Layer layer)
        {
            //Add the layer and return it.
            return Factory.Instance.AddLayer(_Level, layer);
        }
        /// <summary>
        /// Add a layer to the level.
        /// </summary>
        /// <param name="name">The name of the layer.</param>
        /// <param name="scrollSpeed">The scrolling speed of this layer. Used for parallex scrolling.</param>
        public Layer AddLayer(string name, Vector2 scrollSpeed)
        {
            //Add the layer and return it.
            return Factory.Instance.AddLayer(_Level, name, scrollSpeed);
        }
        /// <summary>
        /// Remove a layer from the level.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        public void RemoveLayer(Layer layer)
        {
            //Remove the layer.
            _Level.RemoveLayer(layer);
        }
        /// <summary>
        /// Select a layer.
        /// </summary>
        /// <param name="layer">The layer to select.</param>
        public void Selectlayer(Layer layer)
        {
            //Select the specified layer.
            _SelectedLayer = layer;
        }
        /// <summary>
        /// Add an item to a layer.
        /// </summary>
        /// <param name="layer">The destination layer.</param>
        /// <param name="item">The item to add.</param>
        public Item AddItem(Layer layer, Item item)
        {
            return Factory.Instance.AddItem(layer, item);
        }
        /// <summary>
        /// Add a texture item to the layer.
        /// </summary>
        /// <param name="layer">The layer to add the item to.</param>
        /// <param name="spriteName">The name of the item's sprite.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="scale">The scale of the item.</param>
        public TextureItem AddTextureItem(Layer layer, string spriteName, string name, Vector2 position, float rotation, Vector2 scale)
        {
            //Add the item and return it.
            return Factory.Instance.AddTextureItem(layer, spriteName, name, position, rotation, scale);
        }
        /// <summary>
        /// Remove an item from a layer.
        /// </summary>
        /// <param name="layer">The source layer.</param>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(Layer layer, Item item)
        {
            //Remove the item from the specified layer.
            layer.RemoveItem(item);
        }
        /// <summary>
        /// Select an item.
        /// </summary>
        /// <param name="item">The item to select.</param>
        public void SelectItem(Item item)
        {
            //Select the specified item.
            _SelectedItem = item;
            Selectlayer((item == null) ? null : _Level.Layers[GetLayerIndex(_SelectedItem)]);
            //Update the property list.
            UpdatePropertyList();

            //Let the modification form know of the selection.
            _ItemModifier.Item = _SelectedItem;
        }
        /// <summary>
        /// Move the selected item.
        /// </summary>
        /// <param name="move">The amount to move.</param>
        public void MoveItem(Vector2 move)
        {
            if (_SelectedItem != null) { _SelectedItem.Position += move; }
        }
        /// <summary>
        /// Rotate the selected item.
        /// </summary>
        /// <param name="amount">The amount to rotate.</param>
        public void RotateItem(float amount)
        {
            if (_SelectedItem != null) { _SelectedItem.Rotation += amount; }
        }
        /// <summary>
        /// Scale the selected item.
        /// </summary>
        /// <param name="scale">The amount of change in scale.</param>
        public void ScaleItem(Vector2 scale)
        {
            if (_SelectedItem != null) { _SelectedItem.Scale += scale; }
        }
        /// <summary>
        /// Copy the selected item to the specified position.
        /// </summary>
        /// <param name="position">The local screen position to copy the selected item to.</param>
        public void CopyItem(Vector2 position)
        {
            //Get a clone of the item and change its position.
            Item item = _SelectedItem.Clone();
            item.Position = _SelectedLayer.GetWorldPosition(position);

            //Add the item to the level.
            AddItem(_SelectedLayer, item);
        }
        /// <summary>
        /// Grapple an item by the mouse and force it to submit to the user's every whim and fancy.
        /// </summary>
        public void GrappleItem()
        {
            //Check what kind of item we are dealing with.
            switch (_SelectedItem.Type)
            {
                case ItemType.Item:
                    {
                        //If the grapple point has not been set, set it.
                        if (_ItemGrapplePoint == Vector2.Zero) { _ItemGrapplePoint = Helper.GetMousePosition() - _SelectedItem.Position; break; }

                        //Make the item follow the mouse.
                        _SelectedItem.Position = Helper.GetMousePosition() - _ItemGrapplePoint;
                        break;
                    }
                case (ItemType.TextureItem): { goto case ItemType.Item; }
                case ItemType.Entity:
                    {
                        //The mouse's position.
                        Vector2 position = ConvertUnits.ToSimUnits(_Camera.ConvertScreenToWorld(Helper.GetMousePosition()));

                        //If a grapple joint already exist, update its position and then call this off.
                        if (_MouseGrappleJoint != null) { _MouseGrappleJoint.WorldAnchorB = position; break; }
                        //If the mouse's position is not within the bounds of the selected item, stop here.
                        else if (!_SelectedItem.IsPixelsIntersecting(_Camera.ConvertScreenToWorld(Helper.GetMousePosition()))) { break; }

                        //Create a new fixture for the grapple joint.
                        Fixture fixture = _Level.World.TestPoint(position);

                        //If the location is accepted and a fixture has successfully been created.
                        if (fixture != null)
                        {
                            //Create the unseen body and the grapple joint.
                            Body body = fixture.Body;
                            _MouseGrappleJoint = new FixedMouseJoint(body, position);

                            //Tweak some numbers and add the joint to the world simulation.
                            _MouseGrappleJoint.MaxForce = 1000.0f * body.Mass;
                            _Level.World.AddJoint(_MouseGrappleJoint);
                            body.Awake = true;
                        }

                        break;
                    }
                case ItemType.Character: { goto case ItemType.Entity; }
            }
        }
        /// <summary>
        /// Move an item upwards in the list.
        /// </summary>
        /// <param name="item">The item to move.</param>
        public void MoveItemUp(Item item)
        {
            //Get the layer where the item resides.
            Layer layer = _Level.Layers[GetLayerIndex(item)];
            //Get the item's index.
            int index = GetItemIndex(item);

            //If the item isn't already at the top of the list, move it upwards one step.
            if ((layer != null) && (index > 0)) { layer.Items[index] = layer.Items[index - 1]; layer.Items[index - 1] = item; }
        }
        /// <summary>
        /// Move an item downwards in the list.
        /// </summary>
        /// <param name="item">The item to move.</param>
        public void MoveItemDown(Item item)
        {
            //Get the layer where the item resides.
            Layer layer = _Level.Layers[GetLayerIndex(item)];
            //Get the item's index.
            int index = GetItemIndex(item);

            //If the item isn't already at the bottom of the list, move it downwards one step.
            if ((layer != null) && (index < (layer.Items.Count - 1))) { layer.Items[index] = layer.Items[index + 1]; layer.Items[index + 1] = item; }
        }
        /// <summary>
        /// Move a layer and its items upwards in the list.
        /// </summary>
        /// <param name="layer">The layer to move.</param>
        public void MoveLayerUp(Layer layer)
        {
            //Get the layer's index.
            int index = GetLayerIndex(layer);

            //If the item isn't already at the top of the list, move it upwards one step.
            if (index > 0) { _Level.Layers[index] = _Level.Layers[index - 1]; _Level.Layers[index - 1] = layer; }
        }
        /// <summary>
        /// Move a layer and all its item downwards in the list.
        /// </summary>
        /// <param name="layer">The layer to move.</param>
        public void MoveLayerDown(Layer layer)
        {
            //Get the layer's index.
            int index = GetLayerIndex(layer);

            //If the item isn't already at the bottom of the list, move it downwards one step.
            if (index < (_Level.Layers.Count - 1)) { _Level.Layers[index] = _Level.Layers[index + 1]; _Level.Layers[index + 1] = layer; }
        }
        /// <summary>
        /// Move the camera.
        /// </summary>
        /// <param name="direction">The direction in which to move it.</param>
        public void MoveCamera(Vector2 direction)
        {
            //Move the camera.
            _Camera.Move(direction);
        }
        /// <summary>
        /// Draw a selection form around the selected item, thus highlighting it.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void DrawSelectionForm(SpriteBatch spriteBatch)
        {
            //If an item hasn't been selected, quit.
            if (_SelectedItem == null) { return; }

            //Get the selected item's bounding box.
            Rectangle rect = Helper.GetBoundingBox(_SelectedItem);

            //Get the corner positions.
            Vector2 topLeft = new Vector2(rect.Left, rect.Top);
            Vector2 topRight = new Vector2(rect.Right, rect.Top);
            Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);
            Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);

            //Draw a bounding box around the selected item.
            _SelectionBrush.Draw(spriteBatch, topLeft, topRight);
            _SelectionBrush.Draw(spriteBatch, topRight, bottomRight);
            _SelectionBrush.Draw(spriteBatch, bottomRight, bottomLeft);
            _SelectionBrush.Draw(spriteBatch, bottomLeft, topLeft);
        }
        /// <summary>
        /// Toggle the GUI either on or off.
        /// </summary>
        public void ToggleGUI()
        {
            //Toggle the GUI on or off.
            _GUI.IsActive = !_GUI.IsActive;
        }
        /// <summary>
        /// Set up the tree view according to the current state of the level.
        /// </summary>
        private void SetUpTreeView()
        {
            //Clear the tree view.
            _TreeView.Clear();

            //Begin with a root node.
            _TreeView.AddNode();
            TreeViewNode root = _TreeView[0];
            root.Checkbox.Text = _Level.Name;

            //Go through each layer and add them as nodes in the tree.
            foreach (Layer layer in _Level.Layers)
            {
                //Add a node to the tree view.
                root.AddNode();
                root[root.ChildNodes.Count - 1].Checkbox.Text = layer.Name;

                //Go through all items in the layer and add them as well.
                foreach (Item item in layer.Items)
                {
                    //Add a node to the tree view, depicting the item.
                    root[root.ChildNodes.Count - 1].AddNode();
                    root[root.ChildNodes.Count - 1][root[root.ChildNodes.Count - 1].ChildNodes.Count - 1].Checkbox.Text = item.Name;
                }
            }

            //Update the tree view.
            _TreeView.UpdateTree();
            //Tick all the nodes in the tree.
            _TreeView.Nodes[0].IsTicked = true;
        }
        /// <summary>
        /// Update the list with information about the current item.
        /// </summary>
        private void UpdatePropertyList()
        {
            try
            {
                //The number of fields in the property list.
                int fields = 7;

                //If the list is empty, set it up from scratch.
                if (_PropertyList.Items.Count < 7)
                {
                    //Clear the list.
                    _PropertyList.Items.Clear();

                    //Add all items again with updated information.
                    for (int i = 0; i <= fields; i++) { _PropertyList.AddItem(); _PropertyList[_PropertyList.Items.Count - 1].LoadContent(); }
                }

                //Go through each field and update its information.
                for (int i = 0; i <= fields; i++)
                {
                    switch (i)
                    {
                        case (0): { (_PropertyList[i] as LabelListItem).Label.Text = "--- Basic ---"; break; }
                        case (1): { (_PropertyList[i] as LabelListItem).Label.Text = "Name: " + _SelectedItem.Name; break; }
                        case (2): { (_PropertyList[i] as LabelListItem).Label.Text = "Width: " + _SelectedItem.Width; break; }
                        case (3): { (_PropertyList[i] as LabelListItem).Label.Text = "Height: " + _SelectedItem.Height; break; }
                        case (4): { (_PropertyList[i] as LabelListItem).Label.Text = "Position: " + _SelectedItem.Position; break; }
                        case (5): { (_PropertyList[i] as LabelListItem).Label.Text = "Rotation: " + _SelectedItem.Rotation; break; }
                        case (6): { (_PropertyList[i] as LabelListItem).Label.Text = "Scale: " + _SelectedItem.Scale; break; }
                        case (7): { (_PropertyList[i] as LabelListItem).Label.Text = "IsVisible: " + _SelectedItem.IsVisible; break; }
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// Get the index of an item in a layer.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <returns>The index of the item.</returns>
        private int GetItemIndex(Item item)
        {
            return _Level.GetItemIndex(item);
        }
        /// <summary>
        /// Get the index of the layer an item resides in.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <returns>The index of the layer.</returns>
        private int GetLayerIndex(Item item)
        {
            return _Level.GetLayerIndex(item);
        }
        /// <summary>
        /// Get the index of a layer.
        /// </summary>
        /// <param name="layer">The layer in question.</param>
        /// <returns>The index of the layer.</returns>
        private int GetLayerIndex(Layer layer)
        {
            return _Level.GetLayerIndex(layer);
        }
        /// <summary>
        /// Get the item closest to the given position.
        /// </summary>
        /// <param name="position">The given position.</param>
        /// <returns>The closest item.</returns>
        public Item GetItemAtPosition(Vector2 position)
        {
            return _Level.GetItemAtPosition(position);
        }
        /// <summary>
        /// Change the visibility state of the specified level, layer or item.
        /// </summary>
        /// <param name="layer">The index of the layer. (-1 for null)</param>
        /// <param name="item">The index of the item. (-1 for null)</param>
        /// <param name="isVisible">Whether the object will be visible or not.</param>
        public void ChangeVisibilityState(int layer, int item, bool isVisible)
        {
            //If the layer exists. Otherwise change the level's state of visibility.
            if ((layer >= 0) && (layer < _Level.Layers.Count))
            {
                //If the item exists, change its state of visibility. Otherwise change the layer's state of visibility.
                if ((item >= 0) && (item < _Level[layer].Items.Count)) { _Level[layer][item].IsVisible = isVisible; }
                else { _Level[layer].ChangeVisibilityState(isVisible); }
            }
            else { _Level.ChangeVisibilityState(isVisible); }
        }
        /// <summary>
        /// Update the state of visibility of each layer and item in the level.
        /// </summary>
        public void UpdateVisibilityState()
        {
            #region Old
            /*
            //The level.
            if (_TreeView[0].Checkbox.IsChecked)
            {
                //Make the level visible.
                _Level.IsVisible = true;

                //The layers.
                for (int layer = 0; layer < (_TreeView[0].ChildNodes.Count - 1); layer++)
                {
                    //If the layer should be visible.
                    if (_TreeView[0].ChildNodes[layer].Checkbox.IsChecked)
                    {
                        //Make the layer visible.
                        _Level[layer].IsVisible = true;

                        //The items.
                        for (int item = 0; item < (_TreeView[0].ChildNodes[layer].ChildNodes.Count - 1); item++)
                        {
                            //Either make the item visible or invisible.
                            if (_TreeView[0].ChildNodes[layer].ChildNodes[item].Checkbox.IsChecked) { _Level[layer][item].IsVisible = true; }
                            else { _Level[layer][item].IsVisible = true; }
                        }
                    }
                    else { _Level[layer].IsVisible = false; }
                }
            }
            else { _Level.ChangeVisibilityState(false); }
            */
            #endregion
        }
        /// <summary>
        /// Change the state of the level.
        /// </summary>
        /// <param name="state">The state to change it into.</param>
        public void ChangeLevelState(LevelState state)
        {
            //Change the state of the level.
            _Level.State = state;
        }
        /// <summary>
        /// The menu has had an option selected.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuOptionSelected(object obj, ListItemSelectEventArgs e)
        {
            //See what kind of menu option that has been selected.
            switch ((e.Item as LabelListItem).Label.Text)
            {
                case ("New Level"): { break; }
                case ("Open Level"): { break; }
                case ("Save Level"): { break; }
                case ("Add Item"): { break; }
                case ("Add Layer"): { break; }
                case ("Delete Layer"): { break; }
                case ("Play"): { ChangeLevelState(LevelState.Play); break; }
                case ("Pause"): { ChangeLevelState(LevelState.Pause); break; }
                case ("Stop"): { break; }
            }
        }
        /// <summary>
        /// A tree view node has been ticked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTreeViewNodeTicked(object obj, TickEventArgs e)
        {
            //Create the variables needed.
            int layer = -1;
            int item = -1;

            //Shortcut to the node.
            TreeViewNode node = (obj as TreeViewNode);

            try
            {
                //Find the layer in which the item that was ticked exists in.
                for (int l = 0; l < _Level.Layers.Count; l++)
                {
                    if (_TreeView[0][l].Equals(node)) { layer = l; break; }
                    else if (_TreeView[0][l].Contains(node, false)) { layer = l; break; }
                }
            }
            catch { }

            try
            {
                //If the layer was found, get the item's index.
                if (layer != -1) { item = _TreeView[0][layer].GetNodeIndex(node); }
            }
            catch { }

            //Update the state of visiblity of each layer and item in the level.
            ChangeVisibilityState(layer, item, e.IsChecked);
        }
        /// <summary>
        /// The layers of the level has changed.
        /// </summary>
        /// <param name="obj">The object that fired this event.</param>
        /// <param name="e">The event's arguments.</param>
        private void OnLayerChanged(object obj, EventArgs e)
        {
            //Update the tree view.
            //SetUpTreeView();
        }
        /// <summary>
        /// The GUI has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGUIClicked(object obj, MouseClickEventArgs e)
        {
            //The GUI has been clicked.
            _IsGUIClicked = true;
        }
        #endregion
    }
}