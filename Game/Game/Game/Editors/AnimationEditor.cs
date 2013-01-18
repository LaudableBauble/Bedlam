using System;
using System.Collections.Generic;
using System.IO;
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

using Library;
using Library.Animate;
using Library.GUI;
using Library.Core;
using Library.Factories;
using Library.GUI.Basic;
using Library.Imagery;
using Library.Infrastructure;

using Game.Screens;

namespace Game.Editors
{
    /// <summary>
    /// An animation editor steers and handles all modification of an animation and thus allows the user to edit it as he sees fit.
    /// </summary>
    class AnimationEditor
    {
        #region Fields
        private AnimationEditorScreen _Screen;
        private ContentManager _ContentManager;
        private Character _Character;
        private Animation _SelectedAnimation;
        private GraphicalUserInterface _GUI;

        private List _InformationList;
        private Checkboxlist _AnimationList;
        private Menu _Menu;

        private int _SelectedBoneIndex;
        private List<bool> _ModifiedBone;
        private List<AnimationBar> _AnimationBars;
        private bool _IsGUIClicked;
        #endregion

        #region Constructor
        /// <summary>
        /// Create an animation controller.
        /// </summary>
        public AnimationEditor(AnimationEditorScreen screen)
        {
            //Initialize some variables.
            Initialize(screen, null);
        }
        /// <summary>
        /// Create an animation controller.
        /// </summary>
        public AnimationEditor(AnimationEditorScreen screen, Character character)
        {
            //Initialize some variables.
            Initialize(screen, character);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the animation controller.
        /// </summary>
        public void Initialize(AnimationEditorScreen screen, Character character)
        {
            //Initialize some variables.
            _Screen = screen;
            _Character = character;
            _SelectedAnimation = ((_Character != null) && (_Character.Skeleton.Animations.Count != 0)) ? GetAnimation(0) : null;
            _AnimationBars = new List<AnimationBar>();
            _SelectedBoneIndex = 0;
            _ModifiedBone = new List<bool>();
            _GUI = new GraphicalUserInterface();
            _IsGUIClicked = false;

            //Create the character.
            _Character = new Character(new Level("redundant", null), "deafult", new Vector2(600, 250), 0, Vector2.One, 0, 0);
            _Character.Skeleton.Initialize(screen.ScreenManager.GraphicsDevice);
            _Character.AddAnimation();
            _Character.Skeleton.Animations[0].Name = "default pose";
            _Character.Skeleton.AddKeyframe(0, 0);
            _Character.Skeleton.AddKeyframe(0, 3);
            _Character.Skeleton.AddKeyframe(0, 8);
            _Character.Skeleton.AddKeyframe(0, 12);
            _Character.Skeleton.AddKeyframe(0, 15);
            _Character.Skeleton.AddKeyframe(0, 19);

            //Intialize the list of modified bones.
            ResetModifiedBones();

            //Add items to the GUI.
            _InformationList = new List(_GUI, new Vector2(950, 155), 300, 300);
            _AnimationList = new Checkboxlist(_GUI, new Vector2(950, 50), 300, 100);
            _Menu = new Menu(_GUI, new Vector2(0, 0), 500, 30);

            //Save some components closer to home.
            _GUI.AddItem(_InformationList);
            _GUI.AddItem(_AnimationList);
            _GUI.AddItem(_Menu);

            //Create the animation bars and update the animation lists.
            UpdateAnimationLists();
            UpdateInformationList(true);

            //Tinker with the animation.
            _SelectedAnimation.ResetKeyframe(0);

            //Hook up some events.
            _GUI.ItemClicked += OnGUIClicked;
            _InformationList.ItemSelect += OnInformationSelected;
            _AnimationList.ItemSelect += OnAnimationSelected;

            #region Menu
            //Play with the menu.
            _Menu.AddMenuItem();
            _Menu.AddMenuItem();
            _Menu.AddMenuItem();

            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[0].AddListItem();
            _Menu.MenuItems[0].AddListItem();

            _Menu.MenuItems[1].AddListItem();
            _Menu.MenuItems[1].AddListItem();
            _Menu.MenuItems[1].AddListItem();
            _Menu.MenuItems[1].AddListItem();
            _Menu.MenuItems[1].AddListItem();

            _Menu.MenuItems[2].AddListItem();
            _Menu.MenuItems[2].AddListItem();
            _Menu.MenuItems[2].AddListItem();

            _Menu.MenuItems[0].IsScrollable = false;
            _Menu.MenuItems[1].IsScrollable = false;
            _Menu.MenuItems[2].IsScrollable = false;
            _Menu.MenuItems[0].IsFixed = false;
            _Menu.MenuItems[1].IsFixed = false;
            _Menu.MenuItems[2].IsFixed = false;

            _Menu.MenuItems[0].Text = "File";
            _Menu.MenuItems[1].Text = "Edit";
            _Menu.MenuItems[2].Text = "View";

            _Menu.MenuItems[0][0].Label.Text = "New Animation";
            _Menu.MenuItems[0][1].Label.Text = "New Skeleton";
            _Menu.MenuItems[0][2].Label.Text = "Open Animation";
            _Menu.MenuItems[0][3].Label.Text = "Open Skeleton";
            _Menu.MenuItems[0][4].Label.Text = "Save Animation";
            _Menu.MenuItems[0][5].Label.Text = "Save Skeleton";

            _Menu.MenuItems[1][0].Label.Text = "Create Bone";
            _Menu.MenuItems[1][1].Label.Text = "Edit Bone";
            _Menu.MenuItems[1][2].Label.Text = "Create Keyframe";
            _Menu.MenuItems[1][3].Label.Text = "Delete Keyframe";
            _Menu.MenuItems[1][4].Label.Text = "Remove Animation";

            _Menu.MenuItems[2][0].Label.Text = "Zoom";
            _Menu.MenuItems[2][1].Label.Text = "Scroll";
            _Menu.MenuItems[2][2].Label.Text = "Windows";

            _Menu.MenuOptionSelect += OnMenuOptionSelected;
            #endregion
        }
        /// <summary>
        /// Load the animation controller's content.
        /// </summary>
        /// <param name="contentManager">The manager that handles all graphical content.</param>
        /// <param name="graphicsDevice">The device that controls the graphics.</param>
        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            //Save the content manager for further use.
            _ContentManager = contentManager;

            //Give the content manager to the character prematurely.
            _Character.Skeleton.Sprites.ContentManager = contentManager;

            #region Character
            //Torso.
            Factory.Instance.AddBone(_Character.Skeleton, "Torso", "Animation/Textures/Main Character[Torso]", new Vector2(600, 250), 158, (float)Math.PI,
                new Vector2(35f, 0.5f), (float)Math.PI);

            //Head.
            Factory.Instance.AddBone(_Character.Skeleton, "Head", "Animation/Textures/Main Character[Head]", 0, new Vector2(600, 250), 128, new Vector2(49, 128));

            //Left Upper Arm.
            Factory.Instance.AddBone(_Character.Skeleton, "Left Upper Arm", "Animation/Textures/Main Character[UpperArm]", 0, new Vector2(600, 250), 72,
                (float)Math.PI / 2, new Vector2(9.5f, 0), (float)Math.PI);
            //Left Lower Arm.
            Factory.Instance.AddBone(_Character.Skeleton, "Left Lower Arm", "Animation/Textures/Main Character[LowerArm]", 2, 68, (float)Math.PI, new Vector2(8.5f, 0),
                -(float)Math.PI);

            //Right Upper Arm.
            Factory.Instance.AddBone(_Character.Skeleton, "Right Upper Arm", "Animation/Textures/Main Character[UpperArm]", 0, new Vector2(600, 250), 72,
                -((float)Math.PI / 2), new Vector2(9.5f, 0), (float)Math.PI);
            //Right Lower Arm.
            Factory.Instance.AddBone(_Character.Skeleton, "Right Lower Arm", "Animation/Textures/Main Character[LowerArm]", 4, 68, (float)Math.PI, new Vector2(8.5f, 0),
                (float)Math.PI);

            //Right Upper Leg.
            Factory.Instance.AddBone(_Character.Skeleton, "Right Upper Leg", "Animation/Textures/Main Character[UpperLeg]", 0, new Vector2(600, 408), 69,
                3 * (-(float)Math.PI / 4), new Vector2(9.5f, 0), (float)Math.PI);
            //Right Lower Leg.
            Factory.Instance.AddBone(_Character.Skeleton, "Right Lower Leg", "Animation/Textures/Main Character[LowerLeg]", 6, 65, 3.5f * (-(float)Math.PI / 4),
                new Vector2(6.5f, 0), -(float)Math.PI);
            //Right Foot.
            Factory.Instance.AddBone(_Character.Skeleton, "Right Foot", "Animation/Textures/Main Character[RightFoot]", 7, 65, -(float)Math.PI / 2, new Vector2(11.5f, 0),
                -(float)Math.PI);

            //Left Upper Leg.
            Factory.Instance.AddBone(_Character.Skeleton, "Left Upper Leg", "Animation/Textures/Main Character[UpperLeg]", 0, new Vector2(600, 408), 69,
                3 * ((float)Math.PI / 4), new Vector2(9.5f, 0), (float)Math.PI);
            //Left Lower Leg.
            Factory.Instance.AddBone(_Character.Skeleton, "Left Lower Leg", "Animation/Textures/Main Character[LowerLeg]", 9, 65, 3.5f * ((float)Math.PI / 4),
                new Vector2(6.5f, 0), (float)Math.PI);
            //Left Foot.
            Factory.Instance.AddBone(_Character.Skeleton, "Left Foot", "Animation/Textures/Main Character[LeftFoot]", 10, 65, (float)Math.PI / 2, new Vector2(11.5f, 0),
                (float)Math.PI);
            #endregion

            //Load the character.
            _Character.LoadContent(_ContentManager);

            //Animation animation = contentManager.Load<Animation>("animation01");

            //Load the GUI's content.
            _GUI.LoadContent(graphicsDevice, _ContentManager, spriteBatch);
        }
        /// <summary>
        /// Update the animation controller.
        /// </summary>
        /// <param name="gameTime">The engine time to adhere to.</param>
        public void Update(GameTime gameTime)
        {
            //The GUI hasn't been clicked yet this step.
            _IsGUIClicked = false;
            //Update the GUI.
            _GUI.Update(gameTime);
            //Update the character.
            _Character.Update(gameTime);
            //Update the debug list.
            UpdateInformationList(false);

            //If the animation has been temporarily suspended, cease control over the basic skeleton motoric functions until it is back on track.
            if (!IsAnyAnimationPlaying()) { UpdateSkeleton(); }
        }
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method, this will only be called when the gameplay screen is active.
        /// </summary>
        /// <param name="input">The InputState instance that relays the state of input.</param>
        public void HandleInput(InputState input)
        {
            //Enable the GUI to handle input as well.
            _GUI.HandleInput(input);

            //If the GUI hasn't been clicked.
            if (!_IsGUIClicked)
            {
                #region Mouse Clicks
                //If a left mouse click has occured, see what the user wants to be selected.
                if (input.IsNewLeftMouseClick())
                {
                    //If the click wasn't on any of the animation bars, select the closest bone to the mouse. 
                    //if (!Helper.IsPointWithinBox(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), _AnimationBars.Position, _AnimationBars.Width, _AnimationBars.Height))
                    {
                        //The bone index.
                        int index = -1;
                        //The shortest distance.
                        float shortest = -1;

                        //Loop through all bones in all skeletons and find the closest one.
                        foreach (Bone b in _Character.Skeleton.Bones)
                        {
                            //The average distance to the bone.
                            float distance = Vector2.Distance(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Vector2.Divide(Vector2.Add(b.TransformedPosition,
                                Helper.CalculateOrbitPosition(b.TransformedPosition, b.TransformedRotation, b.Length)), 2));

                            //Determine the shortest distance and the bone.
                            if (shortest == -1) { index = b.Index; shortest = distance; }
                            else if ((shortest != -1) && (distance < shortest))
                            {
                                //The selected index.
                                index = b.Index;
                                //The shortest distance so far.
                                shortest = distance;
                            }
                        }

                        //Pass along the new selected bone index.
                        _SelectedBoneIndex = index;
                    }
                }
                #endregion

                #region Keyboard Presses
                //If the user presses the TAB button, loop through all bones in the skeleton.
                if (input.IsNewKeyPress(Keys.Tab))
                {
                    //Increment the counter.
                    _SelectedBoneIndex++;
                    //Check the bounds of the selected bone index.
                    if (_SelectedBoneIndex >= _Character.Skeleton.Bones.Count) { _SelectedBoneIndex = 0; }
                }

                //If the user holds down CTRL.
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.LeftControl) || input.CurrentKeyboardStates[0].IsKeyDown(Keys.RightControl))
                {
                    //Add a new bone to the skeleton.
                    if (input.IsNewKeyPress(Keys.B)) { DisplayCreateBoneDialog(_SelectedBoneIndex); }
                    //If the user presses the K button, insert a new keyframe after the selected one.
                    if (input.IsNewKeyPress(Keys.K)) { AddKeyframe(GetAnimationBar().SelectedFrameNumber + 1); }
                    //If the user presses the DELETE button, delete the selected keyframe.
                    if (input.IsNewKeyPress(Keys.Delete)) { DeleteKeyframe(GetAnimationBar().SelectedFrameNumber); }
                }

                //If the animation has been paused.
                if (!IsAnyAnimationPlaying())
                {
                    //If the user holds down CTRL.
                    if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.LeftControl) || input.CurrentKeyboardStates[0].IsKeyDown(Keys.RightControl))
                    {
                        //Move the selected bone.
                        if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Right)) { MoveBone(_SelectedBoneIndex, new Vector2(1, 0)); }
                        else if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Left)) { MoveBone(_SelectedBoneIndex, new Vector2(-1, 0)); }
                        else if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Up)) { MoveBone(_SelectedBoneIndex, new Vector2(0, -1)); }
                        else if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Down)) { MoveBone(_SelectedBoneIndex, new Vector2(0, 1)); }
                    }
                    //Otherwise rotate the selected bone.
                    else
                    {
                        //If a bone is selected and the user presses an arrow button, rotate the bone.
                        if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Right))
                        {
                            //Rotate the bone and acknowledge that it has been modified.
                            _Character.Skeleton.Bones[_SelectedBoneIndex].Rotation += .1f;
                            _ModifiedBone[_SelectedBoneIndex] = true;
                        }
                        else if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Left))
                        {
                            //Rotate the bone and acknowledge that it has been modified.
                            _Character.Skeleton.Bones[_SelectedBoneIndex].Rotation -= .1f;
                            _ModifiedBone[_SelectedBoneIndex] = true;
                        }
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// Draw the animation controller's interface.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to be used.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the character.
            _Character.Draw(spriteBatch);
            _Character.Skeleton.DebugDraw(spriteBatch, SelectedBoneIndex);

            //Draw the GUI.
            _GUI.Draw();
        }

        /// <summary>
        /// Update the skeleton's basic motoric functions.
        /// </summary>
        private void UpdateSkeleton()
        {
            //Loop through all bones in the skeleton.
            for (int boneIndex = 0; boneIndex < _Character.Skeleton.BoneUpdateOrder.Count; boneIndex++)
            {
                //Current bone.
                Bone bone = _Character.Skeleton.BoneUpdateOrder[boneIndex];

                //If the bone's not a root bone, continue as usual.
                if (!bone.IsRootBone)
                {
                    //The new absolute position.
                    /*bone.AbsolutePosition = Helper.CalculateOrbitPosition(_Character.Skeleton.Bones[bone.ParentIndex].AbsolutePosition,
                        (bone.RelativeDirection + _Character.Skeleton.Bones[bone.ParentIndex].AbsoluteRotation), Vector2.Distance(Vector2.Zero, bone.RelativePosition));

                    //Update the absolute rotation and the relative direction.
                    bone.UpdateAbsoluteRotation();
                    bone.UpdateRelativeDirection();*/
                }
            }
        }
        /// <summary>
        /// Transform the skeleton according to a state of one of its animations, ie. a certain keyframe.
        /// </summary>
        /// <param name="animation">The animation in focus.</param>
        /// <param name="frameNumber">The index of the frame.</param>
        public void TransformSkeleton(Animation animation, int frameNumber)
        {
            //Loop through all bones in the skeleton.
            for (int boneIndex = 0; boneIndex < _Character.Skeleton.BoneUpdateOrder.Count; boneIndex++)
            {
                //Current bone.
                Bone bone = _Character.Skeleton.BoneUpdateOrder[boneIndex];

                //If the frame happens to be a keyframe.
                if (animation.Keyframes.Exists(kf => (kf.FrameNumber == frameNumber)))
                {
                    //Get the keyframe.
                    Keyframe keyframe = animation.Keyframes.Find(kf => (kf.FrameNumber == frameNumber));

                    //If the bone is involved in any key changes in the not so distant future.
                    if (keyframe.ExistsBone(bone.Index))
                    {
                        //Get the correct bone in the next keyframe's list of bones to update, that is the one that's currently being updated.
                        Bone boneToBe = keyframe.GetBone(bone.Index);

                        //Perform the linear interpolation between the last and next keyframe bone states.
                        bone.Rotation = MathHelper.Lerp(bone.Rotation, boneToBe.Rotation, 1);
                    }
                }
            }
        }
        /// <summary>
        /// Update the list with information about the animation.
        /// </summary>
        /// <param name="complete">Whether a complete update is necessary.</param>
        private void UpdateInformationList(bool complete)
        {
            try
            {
                //If a complete update is necessary.
                if (complete)
                {
                    //Clear the list.
                    _InformationList.Items.Clear();

                    //Add all items again with updated information.
                    for (int i = 0; i <= 18; i++)
                    {
                        //add the items.
                        _InformationList.AddItem();
                        //See if it to load their content.
                        if (_GUI.ContentManager != null) { _InformationList[_InformationList.Items.Count - 1].LoadContent(); }
                    }

                    (_InformationList[0] as LabelListItem).Label.Text = "--- Animation ---";
                    (_InformationList[1] as LabelListItem).Label.Text = "Number of Frames: " + _SelectedAnimation.NumberOfFrames;
                    (_InformationList[2] as LabelListItem).Label.Text = "Frame currently animated: " + GetAnimationBar().AnimatedFrame;
                    (_InformationList[3] as LabelListItem).Label.Text = "Signal Strength: " + _SelectedAnimation.Strength;
                    (_InformationList[4] as LabelListItem).Label.Text = "";
                    (_InformationList[5] as LabelListItem).Label.Text = "--- Keyframes ---";
                    (_InformationList[6] as LabelListItem).Label.Text = "Selected Index: " + GetAnimationBar().SelectedFrameNumber;
                    (_InformationList[7] as LabelListItem).Label.Text = "Frame Time: " + _SelectedAnimation.FrameTime;
                    (_InformationList[8] as LabelListItem).Label.Text = "";
                    (_InformationList[9] as LabelListItem).Label.Text = "--- Skeleton ---";
                    (_InformationList[10] as LabelListItem).Label.Text = "Number of Bones: " + _Character.Skeleton.Bones.Count;
                    (_InformationList[11] as LabelListItem).Label.Text = "";
                    (_InformationList[12] as LabelListItem).Label.Text = "--- Bones ---";
                    (_InformationList[13] as LabelListItem).Label.Text = "Selected Index: " + _SelectedBoneIndex;
                    (_InformationList[14] as LabelListItem).Label.Text = "Parent Index: " + _Character.Skeleton.Bones[_SelectedBoneIndex].ParentIndex;
                    (_InformationList[15] as LabelListItem).Label.Text = "Name: " + _Character.Skeleton.Bones[_SelectedBoneIndex].Name;
                    (_InformationList[16] as LabelListItem).Label.Text = "Position: " + _Character.Skeleton.Bones[_SelectedBoneIndex].StartPosition.ToString();
                    (_InformationList[17] as LabelListItem).Label.Text = "Rotation: " + _Character.Skeleton.Bones[_SelectedBoneIndex].Rotation;
                    (_InformationList[18] as LabelListItem).Label.Text = "Length: " + _Character.Skeleton.Bones[_SelectedBoneIndex].Length;
                }
                //Otherwise just update as needed.
                else
                {
                    (_InformationList[1] as LabelListItem).Label.Text = "Number of Frames: " + _SelectedAnimation.NumberOfFrames;
                    (_InformationList[2] as LabelListItem).Label.Text = "Frame currently animated: " + GetAnimationBar().AnimatedFrame;
                    (_InformationList[3] as LabelListItem).Label.Text = "Signal Strength: " + _SelectedAnimation.Strength;
                    (_InformationList[6] as LabelListItem).Label.Text = "Selected Index: " + GetAnimationBar().SelectedFrameNumber;
                    (_InformationList[7] as LabelListItem).Label.Text = "Frame Time: " + _SelectedAnimation.FrameTime;
                    (_InformationList[10] as LabelListItem).Label.Text = "Number of Bones: " + _Character.Skeleton.Bones.Count;
                    (_InformationList[13] as LabelListItem).Label.Text = "Selected Index: " + _SelectedBoneIndex;
                    (_InformationList[14] as LabelListItem).Label.Text = "Parent Index: " + _Character.Skeleton.Bones[_SelectedBoneIndex].ParentIndex;
                    (_InformationList[15] as LabelListItem).Label.Text = "Name: " + _Character.Skeleton.Bones[_SelectedBoneIndex].Name;
                    (_InformationList[16] as LabelListItem).Label.Text = "Position: " + _Character.Skeleton.Bones[_SelectedBoneIndex].StartPosition.ToString();
                    (_InformationList[17] as LabelListItem).Label.Text = "Rotation: " + _Character.Skeleton.Bones[_SelectedBoneIndex].Rotation;
                    (_InformationList[18] as LabelListItem).Label.Text = "Length: " + _Character.Skeleton.Bones[_SelectedBoneIndex].Length;
                }
            }
            catch { }
        }
        /// <summary>
        /// Update the list of animations to accurately reflect the loaded list of animations.
        /// This also updates the animation bars scattered around the screen.
        /// </summary>
        private void UpdateAnimationLists()
        {
            //Clear the lists.
            _AnimationList.Items.Clear();
            foreach (AnimationBar bar in _AnimationBars) { _GUI.RemoveItem(bar); }
            _AnimationBars.Clear();
            _SelectedAnimation = (_Character.Skeleton.Animations.Count == 0) ? new Animation() : GetAnimation(0);

            //Go through all the skeleton's animations and add them as entries in the list.
            foreach (Animation animation in _Character.Skeleton.Animations)
            {
                //Add an entry.
                _AnimationList.AddItem();
                if (_GUI.ContentManager != null) { _AnimationList[_AnimationList.Items.Count - 1].LoadContent(); }
                _GUI.AddItem(new AnimationBar(_GUI, animation, new Vector2(50, (600 + _AnimationBars.Count * 22)), 800, 20));
                _AnimationBars.Add(_GUI.LastItem as AnimationBar);
                if (_GUI.ContentManager != null) { _AnimationBars[_AnimationBars.Count - 1].LoadContent(); }

                //Hook up some events.
                _AnimationBars[_AnimationBars.Count - 1].MouseClick += OnAnimationBarClick;
                _AnimationBars[_AnimationBars.Count - 1].PlayStateChange += OnPlayStateChange;
                _AnimationBars[_AnimationBars.Count - 1].SelectedFrameChange += OnSelectedFrameChange;

                //Edit the label.
                _AnimationList[_AnimationList.Items.Count - 1].Checkbox.Text = animation.Name;
            }
        }
        /// <summary>
        /// Check if there is any animation currently playing.
        /// </summary>
        /// <returns>Whether an animation is currently playing.</returns>
        private bool IsAnyAnimationPlaying()
        {
            //Go through each animation bar.
            foreach (AnimationBar bar in _AnimationBars)
            {
                //If the bar is currently playing, return true.
                if (bar.IsPlaying) { return true; }
            }

            //No animation is playing, return false.
            return false;
        }
        /// <summary>
        /// Get the animation bar that houses the selected animation.
        /// </summary>
        /// <returns>The animation bar.</returns>
        private AnimationBar GetAnimationBar()
        {
            //Return the animation bar.
            return GetAnimationBar(_SelectedAnimation);
        }
        /// <summary>
        /// Get the animation bar that houses the specified animation.
        /// </summary>
        /// <param name="animation">The animation in question.</param>
        /// <returns>The animation bar.</returns>
        private AnimationBar GetAnimationBar(Animation animation)
        {
            //Go through each animation bar.
            foreach (AnimationBar bar in _AnimationBars)
            {
                //If the bar houses the animation, return it.
                if (bar.Animation == animation) { return bar; }
            }

            //Sorry, no animation bar has any records of that animation.
            return null;
        }
        /// <summary>
        /// Get the index of the specified animation.
        /// </summary>
        /// <param name="animation">The animation in question.</param>
        /// <returns>The index of the animation.</returns>
        private int GetAnimationIndex(Animation animation)
        {
            //Return the animation's index.
            return _Character.Skeleton.Animations.IndexOf(animation);
        }
        /// <summary>
        /// Get the animation at a certain index position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The animation.</returns>
        private Animation GetAnimation(int index)
        {
            //Return the animation.
            return _Character.Skeleton.Animations[index];
        }
        /// <summary>
        /// Display a form that lets the user create a bone and add it to the skeleton.
        /// </summary>
        /// <param name="parent">The parent bone.</param>
        private void DisplayCreateBoneDialog(int parent)
        {
            //The dialog.
            CreateBoneDialog dialog = new CreateBoneDialog(_GUI, new Vector2(350, 150), 200, 200);

            //Display a bone dialog.
            _GUI.AddForegroundItem(dialog);
            dialog.Bone.ParentIndex = parent;
            //Subscribe to some of the bone dialog's events.
            dialog.BoneCreated += OnBoneCreated;
        }
        /// <summary>
        /// Display a form that lets the user edit an already existing bone.
        /// </summary>
        /// <param name="bone">The bone to edit.</param>
        private void DisplayEditBoneDialog(Bone bone)
        {
            //The dialog.
            EditBoneDialog dialog = new EditBoneDialog(_GUI, new Vector2(350, 150), 500, 400);

            //Display the dialog.
            _GUI.AddForegroundItem(dialog);
            dialog.Bone = bone;
            //Subscribe to some of the bone dialog's events.
            //(_GUI.LastItem as EditBoneDialog).BoneEdited += OnBoneCreated;
        }
        /// <summary>
        /// Display a form that lets the user load an animation.
        /// </summary>
        private void DisplayLoadAnimationDialog()
        {
            //The dialog.
            LoadAnimationDialog dialog = new LoadAnimationDialog(_GUI, new Vector2(350, 150), 200, 200);

            //Display a dialog.
            _GUI.AddForegroundItem(dialog);
            //Subscribe to some of the dialog's events.
            dialog.AnimationLoaded += OnAnimationLoaded;
        }
        /// <summary>
        /// Display a form that lets the user save an animation.
        /// </summary>
        private void DisplaySaveAnimationDialog()
        {
            //The dialog.
            SaveAnimationDialog dialog = new SaveAnimationDialog(_GUI, new Vector2(350, 150), 200, 55);

            //Display a dialog.
            _GUI.AddForegroundItem(dialog);
            //Subscribe to some of the dialog's events.
            dialog.AnimationSaved += OnAnimationSaved;
        }
        /// <summary>
        /// Display a form that lets the user save a skeleton.
        /// </summary>
        private void DisplaySaveSkeletonDialog()
        {
            //The dialog.
            SaveSkeletonDialog dialog = new SaveSkeletonDialog(_GUI, new Vector2(350, 150), 200, 55);

            //Display a dialog.
            _GUI.AddForegroundItem(dialog);
            //Subscribe to some of the dialog's events.
            dialog.SkeletonSaved += OnSkeletonSaved;
        }
        /// <summary>
        /// Display a form that lets the user load a skeleton.
        /// </summary>
        private void DisplayLoadSkeletonDialog()
        {
            //The dialog.
            LoadSkeletonDialog dialog = new LoadSkeletonDialog(_GUI, new Vector2(350, 150), 200, 200);

            //Display a dialog.
            _GUI.AddForegroundItem(dialog);
            //Subscribe to some of the dialog's events.
            dialog.SkeletonLoaded += OnSkeletonLoaded;
        }
        /// <summary>
        /// Insert a new keyframe to the animation.
        /// </summary>
        /// <param name="frameNumber">The frame number that this keyframe will have in the animation.</param>
        private void AddKeyframe(int frameNumber)
        {
            //Add a new keyframe to the animation.
            _SelectedAnimation.AddKeyframe(frameNumber);
        }
        /// <summary>
        /// Delete a keyframe from the animation.
        /// </summary>
        /// <param name="index">The index of the keyframe to be deleted.</param>
        private void DeleteKeyframe(int index)
        {
            //Add a new keyframe to the animation.
            _SelectedAnimation.RemoveKeyframe(index);
        }
        /// <summary>
        /// Modify a frame in a given animation.
        /// </summary>
        /// <param name="animation">The animation to modify.</param>
        /// <param name="frameNumber">The index of the frame to modify.</param>
        private void ModifyFrame(Animation animation, int frameNumber)
        {
            //If there is any bone that has been modified.
            if (_ModifiedBone.Exists(flag => (flag)))
            {
                //If the currently selected frame isn't a keyframe, create it.
                if (!animation.Keyframes.Exists(kf => (kf.FrameNumber == frameNumber))) { AddKeyframe(frameNumber); }

                //Add the bones that have had their properties modified since the last keyframe to the selected keyframe, thus altering the animation.
                for (int i = 0; i < animation.Skeleton.Bones.Count; i++)
                {
                    //If the list have enough items, continue.
                    if (_ModifiedBone.Count >= (i + 1))
                    {
                        //If the bone have been modified, add it to the new keyframe.
                        if (_ModifiedBone[i])
                        {
                            //The new bone.
                            Bone bone = animation.Skeleton.Bones[i].DeepClone();

                            //If the copied bone already exists at the keyframe, remove it.
                            animation.GetKeyframe(frameNumber).RemoveBone(bone.Index);
                            //Add the copied bone to the selected keyframe.
                            animation.GetKeyframe(frameNumber).AddBone(bone);
                        }
                    }
                    //Otherwise break this party up.
                    else { break; }
                }
            }

            //Set up the list of modified bones.
            _ModifiedBone = new List<bool>(animation.Skeleton.Bones.Count);
            //Reset the modify bone list.
            for (int i = 0; i < _ModifiedBone.Capacity; i++) { _ModifiedBone.Add(false); }
        }
        /// <summary>
        /// Remove an animation from both the editor and the skeleton.
        /// </summary>
        /// <param name="animation">The animation to delete.</param>
        private void RemoveAnimation(Animation animation)
        {
            //Remove the animation from the skeleton.
            _Character.Skeleton.RemoveAnimation(animation);
            //Update the animaion lists.
            UpdateAnimationLists();
        }
        /// <summary>
        /// Move a bone.
        /// </summary>
        private void MoveBone(int index, Vector2 move)
        {
            //Move the specified bone.
            _Character.Skeleton.Bones[_SelectedBoneIndex].StartPosition += move;
        }
        /// <summary>
        /// Save the animation.
        /// </summary>
        /// <param name="animation">The animation to save.</param>
        /// <param name="name">The name of the animation.</param>
        private void SaveAnimation(Animation animation, string name)
        {
            //Change the name of the animation.
            animation.Name = name;
            //Save the animation.
            Helper.SaveAnimation(animation, (_ContentManager.RootDirectory + @"\Editor\"));
        }
        /// <summary>
        /// Load an animation.
        /// </summary>
        /// <param name="skeleton">The skeleton that houses the animation.</param>
        /// <param name="name">The name of the animaion.</param>
        private void LoadAnimation(Skeleton skeleton, string name)
        {
            //Load the animation.
            Helper.LoadAnimation(skeleton, _ContentManager.RootDirectory + @"\" + name);
            //Update the animation bars.
            UpdateAnimationLists();
            //_AnimationBars.SetAnimation(_Animation);
            //Update the animation list.
            UpdateAnimationLists();
        }
        /// <summary>
        /// Create a new animation.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        private void CreateAnimation(string name)
        {
            //Create a new animation and set its name.
            //_Character.Skeleton.Animations.Clear();
            _Character.Skeleton.AddAnimation();
            UpdateAnimationLists();
            //_Animation = _Character.Skeleton.Animations[0];
            GetAnimation(_Character.Skeleton.Animations.Count - 1).Name = name;
            //Update the animation bar and remove the previous animation from the skeleton.
            //_AnimationBars.SetAnimation(_Animation);
        }
        /// <summary>
        /// Save the skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to save.</param>
        private void SaveSkeleton(Skeleton skeleton)
        {
            //Save the skeleton.
            Helper.SaveSkeleton(skeleton, (_ContentManager.RootDirectory + @"\Editor\"));
        }
        /// <summary>
        /// Load a skeleton.
        /// </summary>
        /// <param name="name">The name of the skeleton.</param>
        private void LoadSkeleton(string name)
        {
            //Save the skeleton and load its content.
            _Character.Skeleton = Helper.LoadSkeleton(_Screen.ScreenManager.GraphicsDevice, _ContentManager.RootDirectory + @"\" + name);
            _Character.Skeleton.LoadContent(_ContentManager);
            UpdateAnimationLists();
            //Update the animation bar and remove the previous animation from the skeleton.
            /*_Animation = _Character.Skeleton.Animations[0] != null ? _Character.Skeleton.Animations[0] : new Animation(_Character.Skeleton);
            _AnimationBars.SetAnimation(_Animation);*/

            //Reset the modified bones flags.
            ResetModifiedBones();
        }
        /// <summary>
        /// Create a new skeleton.
        /// </summary>
        /// <param name="name">The name of the skeleton.</param>
        private void CreateSkeleton(string name)
        {
            //Create a new skeleton, set its name and try to load its content.
            _Character.Skeleton = new Skeleton(_Screen.ScreenManager.GraphicsDevice);
            _Character.Skeleton.Name = name;
            _Character.Skeleton.Position = new Vector2(600, 250);
            _Character.Skeleton.AddAnimation();
            UpdateAnimationLists();
            /*_Animation = _Character.Skeleton.Animations[0];
            _Animation.Name = name;*/
            if (_ContentManager != null) { _Character.Skeleton.LoadContent(_ContentManager); }
            //Update the animation bar and remove the previous animation from the skeleton.
            //_AnimationBars.SetAnimation(_Animation);

            //Reset the modified bones flags.
            ResetModifiedBones();
        }
        /// <summary>
        /// Reset the list of modified bones so that it corresponds to the current skeleton.
        /// </summary>
        private void ResetModifiedBones()
        {
            //Reset the list.
            _ModifiedBone.Clear();
            foreach (Bone b in _Character.Skeleton.Bones) { _ModifiedBone.Add(false); }
        }
        /// <summary>
        /// The animation bar has been clicked.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAnimationBarClick(object obj, EventArgs e)
        {
            //The animation bar.
            AnimationBar bar = (obj as AnimationBar);
            //If the animation has been temporarily suspended, transform the skeleton so as to display its state at the selected keyframe.
            if (!bar.IsPlaying) { TransformSkeleton(GetAnimation(_AnimationBars.IndexOf(bar)), bar.SelectedFrameNumber); }
        }
        /// <summary>
        /// A bone has been created, add it to the selected skeleton.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnBoneCreated(object obj, BoneCreatedEventArgs e)
        {
            //Whether the bone has been fitted with a sprite, add a bone accordingly. TODO: Pass along the name of the bone as well.
            if (String.IsNullOrWhiteSpace(e.SpriteName)) { Factory.Instance.AddBone(_Character.Skeleton, e.Bone.ParentIndex, e.Bone.Length); }
            else { Factory.Instance.AddBone(_Character.Skeleton, e.SpriteName, e.Bone.ParentIndex, e.Bone.Length, e.SpriteOrigin, e.SpriteRotationOffset); }

            //Update the list of modified bones.
            _ModifiedBone.Add(false);

            //Unsubscribe from the bone dialog's events.
            (obj as CreateBoneDialog).BoneCreated -= OnBoneCreated;
        }
        /// <summary>
        /// An animation has been picked for loading.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAnimationLoaded(object obj, AnimationEventArgs e)
        {
            //Load the animation.
            LoadAnimation(_Character.Skeleton, e.FileName);

            //Unsubscribe from the bone dialog's events.
            (obj as LoadAnimationDialog).AnimationLoaded -= OnAnimationLoaded;
        }
        /// <summary>
        /// An animation has been picked for saving.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAnimationSaved(object obj, AnimationEventArgs e)
        {
            //Load the animation.
            SaveAnimation(_Character.Skeleton.Animations[0], e.FileName);

            //Unsubscribe from the bone dialog's events.
            (obj as SaveAnimationDialog).AnimationSaved -= OnAnimationSaved;
        }
        /// <summary>
        /// A skeleton has been picked for loading.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSkeletonLoaded(object obj, SkeletonEventArgs e)
        {
            //Load the animation.
            LoadSkeleton(e.FileName);

            //Unsubscribe from the bone dialog's events.
            (obj as LoadSkeletonDialog).SkeletonLoaded -= OnSkeletonLoaded;
        }
        /// <summary>
        /// A skeleton has been picked for saving.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSkeletonSaved(object obj, SkeletonEventArgs e)
        {
            //Change the skeleton's name.
            _Character.Skeleton.Name = e.FileName;
            //Load the animation.
            SaveSkeleton(_Character.Skeleton);

            //Unsubscribe from the bone dialog's events.
            (obj as SaveSkeletonDialog).SkeletonSaved -= OnSkeletonSaved;
        }
        /// <summary>
        /// The playing state of the animation bar has changed.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlayStateChange(object obj, EventArgs e)
        {
            //The animation bar.
            AnimationBar bar = (obj as AnimationBar);
            //Modify the keyframe.
            ModifyFrame(bar.Animation, bar.SelectedFrameNumber);
        }
        /// <summary>
        /// The selected frame has been changed.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSelectedFrameChange(object obj, SelectedFrameChangeEventArgs e)
        {
            //The selected frame index have changed, modify the past frame.
            ModifyFrame((obj as AnimationBar).Animation, e.PastIndex);
        }
        /// <summary>
        /// An already loaded animation has been selected.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAnimationSelected(object obj, ListItemSelectEventArgs e)
        {
            //Change the selected animation.
            _SelectedAnimation = GetAnimation(_AnimationList.Items.IndexOf(e.Item));
        }
        /// <summary>
        /// An item in the list of information has been selected, see if it can be modified.
        /// </summary>
        /// <param name="obj">The object that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnInformationSelected(object obj, ListItemSelectEventArgs e)
        {
            //Modify the information.
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
                case ("New Animation"): { CreateAnimation("test"); break; }
                case ("New Skeleton"): { CreateSkeleton("test"); break; }
                case ("Open Animation"): { DisplayLoadAnimationDialog(); break; }
                case ("Open Skeleton"): { DisplayLoadSkeletonDialog(); break; }
                case ("Save Animation"): { DisplaySaveAnimationDialog(); break; }
                case ("Save Skeleton"): { DisplaySaveSkeletonDialog(); break; }
                case ("Create Bone"): { DisplayCreateBoneDialog(_SelectedBoneIndex); break; }
                case ("Edit Bone"): { DisplayEditBoneDialog(_Character.Skeleton.Bones[_SelectedBoneIndex]); break; }
                case ("Create Keyframe"): { AddKeyframe(_AnimationBars[GetAnimationIndex(_SelectedAnimation)].SelectedFrameNumber + 1); break; }
                case ("Delete Keyframe"): { DeleteKeyframe(_AnimationBars[GetAnimationIndex(_SelectedAnimation)].SelectedFrameNumber); break; }
                case ("Remove Animation"): { RemoveAnimation(_SelectedAnimation); break; }
            }
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

        #region Properties
        /// <summary>
        /// The selected bone index.
        /// </summary>
        public int SelectedBoneIndex
        {
            get { return _SelectedBoneIndex; }
            set { _SelectedBoneIndex = value; }
        }
        #endregion
    }
}