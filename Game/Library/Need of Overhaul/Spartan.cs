/*using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using FarseerPhysics;
using FarseerPhysics.Collisions;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Springs;
using FarseerPhysics.Factories;
using FarseerPhysics.Interfaces;
using FarseerPhysics.Mathematics;

using Library.Factories;
using Library.Imagery;
using Library.Weaponry;
using Library.Infrastructure;

namespace Library
{
    /// <summary>
    /// The Spartan character.
    /// </summary>
    public class Spartan : Object
    {
        #region Fields
        //The Angle Joints, connectging the hip and the neck to the rest of the body.
        AngleJoint _AngleNeckJoint;
        AngleLimitJoint _AngleLimitHipJoint;
        //The Revolute Joint.
        RevoluteJoint _RevoluteNeckJoint;
        RevoluteJoint _RevoluteHipJoint;

        //The body part indexes.
        int Head = new int();
        int Torso = new int();
        int Legs = new int();

        //The weapon indexes, main and secondary.
        int MainWeapon = new int();
        int SecondaryWeapon = new int();

        //Enables the Spartan to move and to jump.
        private bool _CanMove;
        private bool _CanJump;
        //If the charcter is moving horizontally and/or vertically.
        private bool _IsMovingHorizontally;
        private bool _IsMovingVertically;
        //The charcter's max speed.
        private float _MaxSpeed;
        //The base speed.
        private float _BaseSpeed;
        //The base jump speed.
        private float _BaseJumpSpeed;
        //The character's health.
        private float _Health;

        //The HorizontalMovingDirections enums.
        /// <summary>
        /// The horizontal moving direction, namely none, left or right.
        /// </summary>
        public enum HorizontalMovingDirections
        {
            None,
            Left,
            Right
        }
        //The HorizontalMovingDirections for this character.
        private HorizontalMovingDirections _HorizontalMovingDirection;

        //The weapon the character wields.
        private Weapon _WeaponWield1;
        private Weapon _WeaponWield2;

        //A lot of debug information.
        public int debugIndex_Damage;
        public int debugIndex_NeckJoint;
        public int debugIndex_Head;
        public int debugIndexBody;
        public int debugIndex_HeadRotation;
        public int debugIndex_Arms;

        //The base position of the body, that will say the center of the whole body.
        private Vector2 _BasePosition;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the Spartan character.
        /// </summary>
        /// <param name="system">The System this character is a part of.</param>
        public override void Initialize(System system)
        {
            //The inherited method.
            base.Initialize(system);

            //Enable movement and jumping.
            _CanMove = true;
            _CanJump = true;
            //Set the horizontal and vertical movement to false.
            _IsMovingHorizontally = false;
            _IsMovingVertically = false;
            //The Speed, base speed and the base jump speed.
            _MaxSpeed = 10;
            _BaseSpeed = 100;
            _BaseJumpSpeed = 500;
            //The health.
            _Health = 100;
            //The base position.
            _BasePosition = new Vector2(400, 400);
            //Make the Spartan weaponless.
            _WeaponWield1 = null;
            _WeaponWield2 = null;
            //Set the horizontal moving direction to none.
            _HorizontalMovingDirection = HorizontalMovingDirections.None;
            //Set the facing direction to right.
            FacingDirection = FacingDirection.Right;

            #region Debug
            //Damage.
            debugIndex_Damage = System.GetDebugIndex();
            System.AddDebugText(debugIndex_Damage, "");
            //Neck Joint.
            debugIndex_NeckJoint = System.GetDebugIndex();
            System.AddDebugText(debugIndex_NeckJoint, "");
            //Head.
            debugIndex_Head = System.GetDebugIndex();
            System.AddDebugText(debugIndex_Head, "");
            //Body.
            debugIndexBody = System.GetDebugIndex();
            System.AddDebugText(debugIndexBody, "");
            //Head rotation.
            debugIndex_HeadRotation = System.GetDebugIndex();
            System.AddDebugText(debugIndex_HeadRotation, "");
            //Arms.
            debugIndex_Arms = System.GetDebugIndex();
            System.AddDebugText(debugIndex_Arms, "");
            #endregion
        }
        /// <summary>
        /// Load the Spartans content.
        /// </summary>
        /// <param name="contentManager">The Content Manager.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            //The inherited method.
            base.LoadContent(contentManager);

            //The Head. Collision Group and visibility.
            Head = AddBody("Spartan/Textures/SpartanHeadV2", new Vector2(_BasePosition.X + 2, _BasePosition.Y - 10), 1f, 1f, 10f, 2, 1f, 0f, 0f, "Head");
            Geoms[Head].CollisionGroup = 1;
            //Sprite.SpriteVisibility[Head, 0] = false;

            //Torso. Collision Group, Moi and visibility.
            Torso = AddBody("Spartan/Textures/SpartanTorsoV1", _BasePosition, 1, 1, 12, 8, 1, 5, 0, 0, "Torso");
            Geoms[Torso].CollisionGroup = 1;
            Bodies[Torso].MomentOfInertia = float.PositiveInfinity;
            SpriteManager[Torso].Visibility = Visibility.Invisible;

            //Arms. Sprite Origin and visibility.
            Factory.Instance.AddSprite(SpriteManager, "Spartan/Textures/SpartanRightArmV2", Bodies[Torso].Position, Calculator.DegreesToRadians(60), 1, 1, 4, 0,
                "Front Arm", 8);
            Factory.Instance.AddSprite(SpriteManager, "Spartan/Textures/SpartanBackArmV1", Bodies[Torso].Position, Calculator.DegreesToRadians(290), 1, 1, 0, 0,
                "Back Arm", 3);
            SpriteManager["Front Arm"][0].Origin = new Vector2(7, 0);
            SpriteManager["Back Arm"][0].Origin = new Vector2(0, 0);
            //Sprite.SpriteVisibility[Torso, 1] = false;
            //Sprite.SpriteVisibility[Torso, 2] = false;

            //Legs. Visibility and Sprite animation.
            Legs = AddBody("Spartan/Textures/Legs/Running/Right/SpartanRightLegsIdleV2", new Vector2(_BasePosition.X, _BasePosition.Y + 15), 0.5f, 1, 20, 25, 2, 5, "Legs");
            SpriteManager["Legs"].Visibility = Visibility.Invisible;
            SpriteManager["Legs"].EnableAnimation = false;

            #region Minimize
            //Running.
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[1]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[2]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[3]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[4]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[5]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[6]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[7]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[8]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[9]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[10]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Running/Right/SpartanRightLegsV1[11]");

            //Jumping.
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Jumping/Right/SpartanRightLegsJumpV1[1]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Jumping/Right/SpartanRightLegsJumpV1[2]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Jumping/Right/SpartanRightLegsJumpV1[3]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Jumping/Right/SpartanRightLegsJumpV1[4]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Jumping/Right/SpartanRightLegsJumpV1[5]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Jumping/Right/SpartanRightLegsJumpV1[6]");
            SpriteManager["Legs"].AddFrame("Spartan/Textures/Legs/Jumping/Right/SpartanRightLegsJumpV1[7]");

            //Correct the Origin.
            SpriteManager["Legs"][0].Width = 41;
            SpriteManager["Legs"][0].OriginX = (SpriteManager["Legs"][0].Width / 2);
            SpriteManager["Legs"][1].OriginX = (SpriteManager["Legs"][0].OriginX + 3);
            SpriteManager["Legs"][1].OriginY = SpriteManager["Legs"][0].OriginY;
            SpriteManager["Legs"][2].OriginX = (SpriteManager["Legs"][2].OriginX - 1);
            SpriteManager["Legs"][2].OriginY = SpriteManager["Legs"][0].OriginY;
            SpriteManager["Legs"][3].OriginY = (SpriteManager["Legs"][0].OriginY + 3);
            SpriteManager["Legs"][4].OriginY = SpriteManager["Legs"][0].OriginY;
            SpriteManager["Legs"][5].OriginX = (SpriteManager["Legs"][5].OriginX - 2);
            SpriteManager["Legs"][5].OriginY = SpriteManager["Legs"][0].OriginY;
            SpriteManager["Legs"][6].OriginY = (SpriteManager["Legs"][0].OriginY + 3);
            SpriteManager["Legs"][7].OriginY = (SpriteManager["Legs"][0].OriginY + 2);
            SpriteManager["Legs"][8].OriginY = (SpriteManager["Legs"][0].Origin.Y + 2);
            SpriteManager["Legs"][9].OriginY = SpriteManager["Legs"][0].OriginY;
            SpriteManager["Legs"][10].OriginY = SpriteManager["Legs"][0].OriginY;
            SpriteManager["Legs"][11].OriginY = SpriteManager["Legs"][0].OriginY;

            //Legs. Collision Group, friction coefficient and OnCollision event.
            Geoms[Legs].CollisionGroup = 1;
            Geoms[Legs].FrictionCoefficient = 0.5f;
            Geoms[Legs].OnCollision += OnLegsCollide;
            //Head. OnCollision event.
            Geoms[Head].OnCollision += OnCollide;
            //Torso. OnCollision event.
            Geoms[Torso].OnCollision += OnCollide;
            //Legs. OnCollision event.
            Geoms[Legs].OnCollision += OnCollide;

            //The Angle Neck Joint.
            _AngleNeckJoint = JointFactory.Instance.CreateAngleJoint(System.PhysicsSimulator, Bodies[Head], Bodies[Torso]);
            //The Angle Limit Hip Joint.
            _AngleLimitHipJoint = JointFactory.Instance.CreateAngleLimitJoint(System.PhysicsSimulator, Bodies[Legs], Bodies[Torso], 0, 0);
            //The Revolute Neck Joint.
            _RevoluteNeckJoint = JointFactory.Instance.CreateRevoluteJoint(System.PhysicsSimulator, Bodies[Head], Bodies[Torso],
                new Vector2(_BasePosition.X, _BasePosition.Y - 5));
            //The Revolute Hip Joint.
            _RevoluteHipJoint = JointFactory.Instance.CreateRevoluteJoint(System.PhysicsSimulator, Bodies[Legs], Bodies[Torso],
                new Vector2(_BasePosition.X, _BasePosition.Y + 5));
            #endregion
        }
        /// <summary>
        /// Update the Spartan.
        /// </summary>
        /// <param name="gameTime">The Game Time instance.</param>
        public override void Update(GameTime gameTime)
        {
            //If the Spartan is able to move.
            if (_CanMove == true)
            {
                //The TimePerFrame.
                SpriteManager["Legs"].TimePerFrame =
                    ((100 * System.UpdateSpeed) / Math.Abs(Bodies[Legs].LinearVelocity.X));

                //Change the facing direction, the horizontal moving direction and the sprite.
                ChangeFacingDirection();
                ChangeHorizontalMovingDirection();
                ChoosingSprite();
            }

            //The inherited method.
            base.Update(gameTime);

            //The Head and Arm Rotation.
            if (_CanMove == true) { FaceMouse(FacingDirection); }

            //The weapon.
            UpdateWeapon();

            //Debug information.
            System.AddDebugText(debugIndex_NeckJoint, "Neck: " + _RevoluteNeckJoint.CurrentAnchor.ToString());
            System.AddDebugText(debugIndex_Head, "Head: " + Bodies[Head].Position.ToString());
            System.AddDebugText(debugIndexBody, "Body: " + Bodies[Torso].Position.ToString());
            System.AddDebugText(debugIndex_HeadRotation, "Head Rotation: " + Bodies[Head].Rotation);
            //System.AddDebugText(debugIndex_Arms, "Neck Rotation: " + AngleNeckJoint.TargetAngle);
            System.AddDebugText(debugIndex_Arms, "Arm: " + SpriteManager["Front Arm"].Position.ToString() + " | " +
                SpriteManager["Back Arm"].Position.ToString());
        }
        /// <summary>
        /// Draw the Spartan.
        /// </summary>
        /// <param name="spriteBatch">The Sprite Batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Inherited Draw method.
            base.Draw(spriteBatch);
        }

        #region Weapon Equip
        /// <summary>
        /// Equip a weapon.
        /// </summary>
        /// <param name="mainWeapon">Whether the weapon is a main or secondary weapon.</param>
        public void EquipWeapon(bool mainWeapon)
        {
            #region Old
            //If the character wants to equip a weapon as its main weapon.
            /*if (mainWeapon)
            {
                //If the character isn't wielding any weapon.
                if (_WeaponWield1 == null)
                {
                    //Create a temp object to store the weapon.
                    Object tempObject = new Object();

                    //Loop through all the objects in the game until you find a weapon.
                    foreach (Object obj in System.ObjectList)
                    {
                        //If the object is in fact a weapon.
                        if (obj.FirstTag == "Weapon")
                        {
                            //Check if the weapon isn't equiped.
                            if (obj.SecondTag == "False")
                            {
                                //If the temporary object isn't empty.
                                if (tempObject != null)
                                {
                                    //If the first body in the temp weapon is not empty.
                                    if (tempObject.Body[0] != null)
                                    {
                                        //Save this character's position.
                                        Vector2 equiperPosition = this.Body[Torso].Position;
                                        //Save the looped weapon's position.
                                        Vector2 weapon1Position = obj.Body[0].Position;
                                        //Save the saved weapon's position.
                                        Vector2 weapon2Position = tempObject.Body[0].Position;

                                        //If the distance between the looped weapon is less great, save the weapon.
                                        if (Vector2.Distance(equiperPosition, weapon1Position) < Vector2.Distance(equiperPosition, weapon2Position))
                                        {
                                            //Save the weapon.
                                            tempObject = obj;
                                        }
                                        else { tempObject = obj; }
                                    }
                                    else { tempObject = obj; }
                                }
                            }
                        }
                    }

                    //The character main wield is the found weapon.
                    _WeaponWield1 = tempObject;

                    Sprite.AddSprite("Weapon/M7SMGV1", Torso, Body[Torso].Position, 1, 1,
                        _WeaponWield1.Sprite.SpriteWidth[0, 0, 0], _WeaponWield1.Sprite.SpriteHeight[0, 0, 0], 0, 0, 0);
                    Sprite.SpriteOrientation[Torso, 3] = Sprite.SpriteOrientation[Torso, 0];

                    //Alert the chosen weapon of its mission.
                    _WeaponWield1.EquipedWeapon(this, Weapon.WeaponWieldType.Main);
                }
                //Otherwise... unequip the current weapon.
                else
                {
                    _WeaponWield1.UnEquipedWeapon();

                    _WeaponWield1 = null;

                    Sprite.DeleteSprite(Torso, 3);
                }
            }
            else
            {
                //If the character isn't wielding any secondary weapon.
                if (_WeaponWield2 == null)
                {
                    //Create a temp object to store the weapon.
                    Object tempObject = new Object();

                    //Loop through all the objects in the game until you find a weapon.
                    foreach (Object obj in System.ObjectList)
                    {
                        //If the object is in fact a weapon.
                        if (obj.FirstTag == "Weapon")
                        {
                            //Check if the weapon isn't equiped.
                            if (obj.SecondTag == "False")
                            {
                                //If the weapon isn't equiped.

                                //If the temporary object isn't empty.
                                if (tempObject != null)
                                {
                                    //If the first body in the temp weapon is not empty.
                                    if (tempObject.Body[0] != null)
                                    {
                                        //Save this character's position.
                                        Vector2 equiperPosition = this.Body[Torso].Position;
                                        //Save the looped weapon's position.
                                        Vector2 weapon1Position = obj.Body[0].Position;
                                        //Save the saved weapon's position.
                                        Vector2 weapon2Position = tempObject.Body[0].Position;

                                        //If the distance between the looped weapon is less great, save the weapon.
                                        if (Vector2.Distance(equiperPosition, weapon1Position) < Vector2.Distance(equiperPosition, weapon2Position))
                                        {
                                            //Save the weapon.
                                            tempObject = obj;
                                        }
                                        else { tempObject = obj; }
                                    }
                                    else { tempObject = obj; }
                                }
                            }
                        }
                    }

                    //The character secondary wield is the found weapon.
                    _WeaponWield2 = tempObject;

                    Sprite.AddSprite("Weapon/M7SMGV1", Torso, Body[Torso].Position, 1, 1,
                        _WeaponWield2.Sprite.SpriteWidth[0, 0, 0], _WeaponWield2.Sprite.SpriteHeight[0, 0, 0], 3, 0, 0);
                    Sprite.SpriteOrientation[Torso, 4] = Sprite.SpriteOrientation[Torso, 0];

                    //Alert the chosen weapon of its mission.
                    _WeaponWield2.EquipedWeapon(this, Weapon.WeaponWieldType.Secondary);
                }
                //Otherwise... unequip the current weapon.
                else
                {
                    _WeaponWield2.UnEquipedWeapon();

                    _WeaponWield2 = null;

                    Sprite.DeleteSprite(Torso, 4);
                }
            }*/
/*
            #endregion

            /*
             * - Check if it's the main weapon that needs to change.
             *      - Find the closest weapon and see if it's close enough.
             *          - Equip/Switch the weapon.
             * - Find the closest weapon and see if it's close enough.
             *      - Equip/Switch the weapon.
             */
/*
            switch (mainWeapon)
            {
                //Main Weapon.
                case (true):
                    {
                        //Create a temp weapon.
                        Weapon weapon = FindClosestWeapon();

                        //Check if the weapon isn't empty.
                        if (weapon != null)
                        {
                            //Find the closest weapon and see if it's close enough.
                            if (Vector2.Distance(weapon.Bodies[0].Position, Bodies[Torso].Position) < 100)
                            {
                                //Equip or Switch the weapon.
                                EquipMainWeapon(weapon);
                            }
                            //Drop the weapon.
                            else { UnEquipWeapon(true); }
                        }
                        //Drop the weapon.
                        else { UnEquipWeapon(true); }

                        break;
                    }
                //Secondary Weapon.
                case (false):
                    {
                        //Create a temp weapon.
                        Weapon weapon = FindClosestWeapon();

                        //Check if the weapon isn't empty.
                        if (weapon != null)
                        {
                            //Find the closest weapon and see if it's close enough.
                            if (Vector2.Distance(weapon.Bodies[0].Position, Bodies[Torso].Position) < 100)
                            {
                                //Equip or Switch the weapon.
                                EquipSecondaryWeapon(weapon);
                            }
                            //Drop the weapon.
                            else { UnEquipWeapon(false); }
                        }
                        //Drop the weapon.
                        else { UnEquipWeapon(false); }

                        break;
                    }
            }
        }
        /// <summary>
        /// Find the closest unequiped weapon.
        /// </summary>
        /// <returns>The closest unequipped weapon.</returns>
        Weapon FindClosestWeapon()
        {
            /*
             * - Loop through every weapon.
             *      - Check if it's not equipped.
             *          - Check if it's closer to the character than the previous one.
             *          - Overwrite the previous weapon.
             *              
             * - Return the chosen weapon.
             */
/*
            //Create a temp weapon.
            Weapon weapon = null;

            //Loop through all the weapons.
            foreach (Weapon w in System.WeaponList)
            {
                //Check if the weapon isn't equiped.
                if (w.SecondTag == "False")
                {
                    //If the temporary weapon isn't empty.
                    if ((weapon != null) && (weapon.Bodies[0] != null))
                    {
                        //Save this character's position.
                        Vector2 equiperPosition = this.Bodies[Torso].Position;
                        //Save the looped weapon's position.
                        Vector2 weapon1Position = w.Bodies[0].Position;
                        //Save the stored weapon's position.
                        Vector2 weapon2Position = weapon.Bodies[0].Position;

                        //Find out the weapon that is closest to the character and save/keep it.
                        if (Vector2.Distance(equiperPosition, weapon1Position) < Vector2.Distance(equiperPosition, weapon2Position))
                        {
                            //Save the weapon.
                            weapon = w;
                        }
                    }
                    else { weapon = w; }
                }
            }

            //Return the nearest weapon.
            return (weapon);
        }
        /// <summary>
        /// Equip a Main Weapon.
        /// </summary>
        /// <param name="weapon">The weapon to equip.</param>
        void EquipMainWeapon(Weapon weapon)
        {
            /*
             * - Check if the character wields any weapon at all.
             *      - Check if the character wields a main weapon.
             *          - Switch the weapons with eachother.
             *          - Equip the weapon.
             *      - Equip the weapon.
             */
/*
            //Check if the character wields any weapon at all.
            if ((_WeaponWield1 != null) || (_WeaponWield2 != null))
            {
                //Check the weapon wield type.
                switch (weapon.WieldType)
                {
                    case (Weapon.WeaponWieldType.Single):
                        {
                            //Check if there are a weapon in slot 2.
                            if (_WeaponWield2 != null)
                            {
                                //Unequip it.
                                UnEquipWeapon(false);
                            }

                            break;
                        }
                    case (Weapon.WeaponWieldType.Dual):
                        {
                            break;
                        }
                    case (Weapon.WeaponWieldType.Support):
                        {
                            break;
                        }
                }

                //Check if the character wields a main weapon.
                if (_WeaponWield1 != null)
                {
                    //Switch the weapons with eachother.
                    UnEquipWeapon(true);

                    //Equip the weapon.
                    _WeaponWield1 = weapon;

                    //Alert the chosen weapon of its mission.
                    _WeaponWield1.EquipedWeapon(this, Weapon.WeaponWieldMode.Main);

                    //Add the Weapon Sprite.
                    AddWeaponSprite(Weapon.WeaponWieldMode.Main);
                }
                else
                {
                    //Equip the weapon.
                    _WeaponWield1 = weapon;

                    //Alert the chosen weapon of its mission.
                    _WeaponWield1.EquipedWeapon(this, Weapon.WeaponWieldMode.Main);

                    //Add the Weapon Sprite.
                    AddWeaponSprite(Weapon.WeaponWieldMode.Main);
                }
            }
            else
            {
                //Equip the weapon.
                _WeaponWield1 = weapon;

                //Alert the chosen weapon of its mission.
                _WeaponWield1.EquipedWeapon(this, Weapon.WeaponWieldMode.Main);

                //Add the Weapon Sprite.
                AddWeaponSprite(Weapon.WeaponWieldMode.Main);
            }
        }
        /// <summary>
        /// Equip a Secondary Weapon.
        /// </summary>
        /// <param name="weapon">The weapon to equip.</param>
        void EquipSecondaryWeapon(Weapon weapon)
        {
            /*
             * - Check if the character wields any weapon at all.
             *      - Check if the character wields a secondary weapon.
             *          - Switch the weapons with eachother.
             *          - Equip the weapon.
             *      - Equip the weapon as a main weapon.
             */
/*
            //Check if the character wields any weapon at all.
            if ((_WeaponWield1 != null) || (_WeaponWield2 != null))
            {
                //Check if the weapon is able to fill the secondary weapon slot and the current weapon as well.
                if ((weapon.WieldType == Weapon.WeaponWieldType.Dual) &&
                    (_WeaponWield1.WieldType == Weapon.WeaponWieldType.Dual))
                {
                    //Check if the character wields a secondary weapon.
                    if (_WeaponWield2 != null)
                    {
                        //Switch the weapons with eachother.
                        UnEquipWeapon(false);

                        //Equip the weapon.
                        _WeaponWield2 = weapon;

                        //Alert the chosen weapon of its mission.
                        _WeaponWield2.EquipedWeapon(this, Weapon.WeaponWieldMode.Secondary);

                        //Add the Weapon Sprite.
                        AddWeaponSprite(Weapon.WeaponWieldMode.Secondary);
                    }
                    else
                    {
                        //Equip the weapon.
                        _WeaponWield2 = weapon;

                        //Alert the chosen weapon of its mission.
                        _WeaponWield2.EquipedWeapon(this, Weapon.WeaponWieldMode.Secondary);

                        //Add the Weapon Sprite.
                        AddWeaponSprite(Weapon.WeaponWieldMode.Secondary);
                    }
                }
                else
                {
                    //Equip the weapon as a main weapon.
                    EquipMainWeapon(weapon);
                }
            }
            else
            {
                //Equip the weapon as a main weapon.
                EquipMainWeapon(weapon);
            }
        }
        /// <summary>
        /// Unequip a weapon.
        /// </summary>
        /// <param name="mainWeapon">If the weapon to drop is the main or secondary weapon.</param>
        void UnEquipWeapon(bool mainWeapon)
        {
            //Check what kind of wield type the weapon is.
            switch (mainWeapon)
            {
                case (true):
                    {
                        //Check if there's a main weapon.
                        if (_WeaponWield1 != null)
                        {
                            //Unequip it.
                            _WeaponWield1.UnEquipedWeapon();
                            _WeaponWield1 = null;
                            SpriteManager.DeleteSprite("Main Weapon");

                            //Check if there's a secondary weapon.
                            if (_WeaponWield2 != null)
                            {
                                ///Switch the secondary weapon to the main weapon slot.

                                //Unequip it.
                                _WeaponWield2.UnEquipedWeapon();
                                Weapon weapon = _WeaponWield2;
                                _WeaponWield2 = null;
                                SpriteManager.DeleteSprite("Secondary Weapon");
                                //Equip the weapon as a main weapon.
                                EquipMainWeapon(weapon);

                            }
                            else { _WeaponWield1 = null; }
                        }

                        break;
                    }
                case (false):
                    {
                        //Check if there's a secondary weapon.
                        if (_WeaponWield2 != null)
                        {
                            //Unequip it.
                            _WeaponWield2.UnEquipedWeapon();
                            _WeaponWield2 = null;
                            SpriteManager.DeleteSprite("Secondary Weapon");
                        }

                        break;
                    }
            }
        }
        /// <summary>
        /// Add the equipped weapon sprite.
        /// </summary>
        /// <param name="wieldMode">The wield mode. Single, Dual or Support.</param>
        void AddWeaponSprite(Weapon.WeaponWieldMode wieldMode)
        {
            //Create a temporary depth variable.
            int depth = 0;

            //Check the weapon wield type.
            switch (_WeaponWield1.WieldType)
            {
                case (Weapon.WeaponWieldType.Single): { depth = 5; break; }
                case (Weapon.WeaponWieldType.Dual): { depth = 0; break; }
                case (Weapon.WeaponWieldType.Support): { depth = 5; break; }
            }

            //Check the weapon's wield mode.
            switch (wieldMode)
            {
                case (Weapon.WeaponWieldMode.Main):
                    {
                        //Add the weapon Sprite.
                        Factory.Instance.AddSprite(SpriteManager, _WeaponWield1.SpritePath, Bodies[Torso].Position, 1, 1, depth, 0, 0, "Main Weapon");
                        //Get the Sprite id.
                        MainWeapon = (SpriteManager.SpriteCount - 1);
                        //Set the Sprite orientation.
                        SpriteManager["Main Weapon"].Orientation = SpriteManager["Torso"].Orientation;

                        break;
                    }
                case (Weapon.WeaponWieldMode.Secondary):
                    {
                        //Add the weapon Sprite.
                        Factory.Instance.AddSprite(SpriteManager, _WeaponWield2.SpritePath, Bodies[Torso].Position, 1, 1, 3, 0, 0, "Secondary Weapon");
                        //Get the Sprite id.
                        SecondaryWeapon = (SpriteManager.SpriteCount - 1);
                        //Set the Sprite orientation.
                        SpriteManager["Secondary Weapon"].Orientation = SpriteManager["Torso"].Orientation;

                        break;
                    }
            }
        }
        #endregion

        /// <summary>
        /// Update the weapon.
        /// </summary>
        public void UpdateWeapon()
        {
            //The Main Weapon.
            if (_WeaponWield1 != null)
            {
                //Update the position of the weapon.
                SpriteManager["Main Weapon"].Position = _WeaponWield1.Bodies[0].Position;
                //Update the rotation of the weapon.
                SpriteManager["Main Weapon"].Rotation = _WeaponWield1.Bodies[0].Rotation;
            }

            //The Secondary Weapon.
            if (_WeaponWield2 != null)
            {
                //Update the position of the weapon.
                SpriteManager["Secondary Weapon"].Position = _WeaponWield2.Bodies[0].Position;
                //Update the rotation of the weapon.
                SpriteManager["Secondary Weapon"].Rotation = _WeaponWield2.Bodies[0].Rotation;
            }
        }
        /// <summary>
        /// Calculate the damage of a collision.
        /// </summary>
        /// <param name="geom1">The first geom in the collision.</param>
        /// <param name="geom2">The second geom in the collision.</param>
        /// <param name="contactList">The list of contacts in the collision.</param>
        public void CalculateDamage(Geom geom1, Geom geom2, ContactList contactList)
        {
            //The seperation value.
            float seperationValue = 0;

            //Create the vectors that'll be used later on.
            Vector2 impactForce = Vector2.Zero;
            Vector2 interpolatedContactPosition = Vector2.Zero;
            Vector2 velocity1 = Vector2.Zero;
            Vector2 velocity2 = Vector2.Zero;

            //Get interpolated contact position.
            for (int i = 0; i < contactList.Count; i++)
            {
                seperationValue += contactList[i].Separation;
            }

            //Divide the seperation value with the number of contacts in the collision.
            seperationValue /= contactList.Count;

            //If the seperation value is low, go on.
            if (seperationValue < -2)
            {

                //Get interpolated contact position.
                for (int i = 0; i < contactList.Count; i++)
                {
                    //Get the interpolated contact position.
                    interpolatedContactPosition += contactList[i].Position;
                }

                //Divide the interpolated contact position with the number of contacts in the list.
                interpolatedContactPosition /= contactList.Count;

                //Calculate impact.
                geom1.Body.GetVelocityAtWorldPoint(ref interpolatedContactPosition, out velocity1);
                geom2.Body.GetVelocityAtWorldPoint(ref interpolatedContactPosition, out velocity2);

                //If the first geom in the collision is this Spartan.
                if (geom1.Tag == this) { impactForce = (velocity1 - velocity2) * geom2.Body.Mass; }
                else { impactForce = (velocity1 - velocity2) * geom1.Body.Mass; }

                //Lose energy.
                _Health -= impactForce.Length();

                //The Debug Text.
                System.AddDebugText(debugIndex_Damage, seperationValue.ToString() + " " +
                    interpolatedContactPosition.Length().ToString() + " "
                + impactForce.Length().ToString());
            }
        }
        /// <summary>
        /// Calculate the damage of a collision.
        /// </summary>
        /// <param name="geom1">The first geom in the collision.</param>
        /// <param name="geom2">The second geom in the collision.</param>
        /// <param name="lineIntersectInfoList">The list of line intersections in the collision.</param>
        public void CalculateDamage(Geom geom1, Geom geom2, List<Vector2> lineIntersectInfoList)
        {
            /*Vector2 impactForce = Vector2.Zero;
            Vector2 interpolatedContactPosition = Vector2.Zero;
            Vector2 velocity1 = Vector2.Zero;
            Vector2 velocity2 = Vector2.Zero;

            float Counter = 0;

            //Get interpolated contact position.
            for (int i = 0; i < lineIntersectInfoList.Count; i++)
            {
                for (int ii = 0; ii < lineIntersectInfoList[i].Points.Count; ii++)
                {
                    interpolatedContactPosition += lineIntersectInfoList[i].Points[ii];
                    Counter++;
                }
            }

            interpolatedContactPosition /= Counter;

            //Calculate impact.
            geom1.Body.GetVelocityAtWorldPoint(ref interpolatedContactPosition, out velocity1);
            geom2.Body.GetVelocityAtWorldPoint(ref interpolatedContactPosition, out velocity2);

            if (geom1.Tag == this)
            {
                impactForce = (velocity1 - velocity2) * geom2.Body.Mass;
            }
            else
            {
                impactForce = (velocity1 - velocity2) * geom1.Body.Mass;
            }

            //Lose energy.
            _Health -= impactForce.Length();

            System.AddDebugText(debugIndex_Damage, interpolatedContactPosition.Length().ToString() + " "
            + impactForce.Length().ToString());*//*
        }
        /// <summary>
        /// The collision event.
        /// </summary>
        /// <param name="geom1">The first geom in the collision.</param>
        /// <param name="geom2">The second geom in the collision.</param>
        /// <param name="contactList">The list of contacts in the collision.</param>
        /// <returns>True.</returns>
        public override bool OnCollide(Geom geom1, Geom geom2, ContactList contactList)
        {
            //Calculate the damage of the collision.
            CalculateDamage(geom1, geom2, contactList);

            //Return true.
            return true;
        }
        /// <summary>
        /// The high velocity collision event.
        /// </summary>
        /// <param name="geom1">The first geom in the collision.</param>
        /// <param name="geom2">The second geom in the collision.</param>
        /// <param name="lineIntersectInfoList">The list of line intersections.</param>
        /// <returns>True.</returns>
        public bool OnHighVelocityCollide(Geom geom1, Geom geom2, List<Vector2> lineIntersectInfoList)
        {
            //Calculate the damage from the collision.
            CalculateDamage(geom1, geom2, lineIntersectInfoList);

            //Return true.
            return true;
        }
        /// <summary>
        /// When the legs of the Spartan collides with the ground.
        /// </summary>
        /// <param name="geom1">The first geom in the collision.</param>
        /// <param name="geom2">The second geom in the collision.</param>
        /// <param name="contactList">The list of contacts in the collision.</param>
        /// <returns>True.</returns>
        public bool OnLegsCollide(Geom geom1, Geom geom2, ContactList contactList)
        {
            //Loop through all the contacts.
            foreach (Contact contact in contactList)
            {
                //If the contact is beneath the body.
                if ((contact.Position.Y <= geom2.Body.Position.Y) &&
                    //If the contact is above the ground.
                    (contact.Position.Y > geom1.Body.Position.Y) &&
                    //If the contact is to the left of the legs rightest point.
                    (contact.Position.X < Bodies[Legs].Position.X + (20 / 2) - 6) &&
                    //If the contact is to the right of the legs leftest point.
                    (contact.Position.X > Bodies[Legs].Position.X - (20 / 2) + 6))
                {
                    //The Spartan is not moving vertically.
                    _IsMovingVertically = false;
                    break;
                }
            }

            //Return true.
            return true;
        }
        /// <summary>
        /// Handle the player Input and do things according to it.
        /// </summary>
        /// <param name="input">The input state.</param>
        /// <param name="i">The index of the inputstate to consider.</param>
        public void HandleInput(InputState input, int i)
        {
            #region Movement && Weapon
            //If the character can move.
            if (_CanMove == true)
            {
                //Jump.
                if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Up)) { Jump(_BaseJumpSpeed); }
                //Move Left.
                if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Left)) { Move(_BaseSpeed, 0); }
                //Move Right.
                if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Right)) { Move(_BaseSpeed, 1); }
                //Equip weapon.
                if ((input.CurrentKeyboardStates[0].IsKeyDown(Keys.A))
                    && !(input.LastKeyboardStates[0].IsKeyDown(Keys.A))) { EquipWeapon(true); }
                //Unequip weapon.
                if ((input.CurrentKeyboardStates[0].IsKeyDown(Keys.H))
                    && !(input.LastKeyboardStates[0].IsKeyDown(Keys.H))) { EquipWeapon(false); }
            }
            #endregion

            #region Debug
            //Sprite Invisibility.
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.S))
            {
                SpriteManager["Head"].Visibility = Visibility.Invisible;
                SpriteManager["Torso"].Visibility = Visibility.Invisible;
                SpriteManager["Front Arm"].Visibility = Visibility.Invisible;
                SpriteManager["Back Arm"].Visibility = Visibility.Invisible;
                SpriteManager["Legs"].Visibility = Visibility.Invisible;

                //If the Spartan is wielding a main weapon.
                if (_WeaponWield1 != null) { SpriteManager["Main Weapon"].Visibility = Visibility.Invisible; }
            }
            //Sprite Visibility.
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.D))
            {
                SpriteManager["Head"].Visibility = Visibility.Visible;
                SpriteManager["Torso"].Visibility = Visibility.Visible;
                SpriteManager["Front Arm"].Visibility = Visibility.Visible;
                SpriteManager["Back Arm"].Visibility = Visibility.Visible;
                SpriteManager["Legs"].Visibility = Visibility.Visible;

                //If the Spartan is wielding a main weapon.
                if (_WeaponWield1 != null) { SpriteManager["Main Weapon"].Visibility = Visibility.Visible; }
            }
            //Rotate the Neck Joint.
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.NumPad1))
            {
                _AngleNeckJoint.TargetAngle += .2f;
            }
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.NumPad2))
            {
                _AngleNeckJoint.TargetAngle -= .2f;
            }
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.NumPad4))
            {
                //FacingDirection = 0;
                FlipSprite(FacingDirection);
            }
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.NumPad5))
            {
                //FacingDirection = 1;
                FlipSprite(FacingDirection);
            }
            #endregion
        }
        /// <summary>
        /// Make the character jump.
        /// </summary>
        /// <param name="speed">The speed of the jump.</param>
        public void Jump(float speed)
        {
            //If the character isn't moving vertically, etc falling or jumping.
            if (_IsMovingVertically == false)
            {
                //Apply some upwards force.
                Bodies[Legs].ApplyForce(new Vector2(0, -speed));
                //Set the IsMovingVertically to true.
                _IsMovingVertically = true;
            }
        }
        /// <summary>
        /// Make the character move.
        /// </summary>
        /// <param name="speed">The moving speed.</param>
        /// <param name="direction">The direction of the movement.</param>
        public void Move(float speed, int direction)
        {
            //The moving speed.
            float _speed = speed;

            //If the Spartan is moving vertically.
            if (_IsMovingVertically)
            {
                //Limit the acceleration.
                _speed /= 8;
                //Limit the speed.
                Bodies[Legs].LinearVelocity.X =
                    MathHelper.Clamp(Bodies[Legs].LinearVelocity.X, -(_MaxSpeed / 1), (_MaxSpeed / 1));
            }
            else
            {
                //Limit the speed.
                Bodies[Legs].LinearVelocity.X =
                    MathHelper.Clamp(Bodies[Legs].LinearVelocity.X, -_MaxSpeed, _MaxSpeed);
            }
            //Consider the direction.
            switch (direction)
            {
                case (0):
                    {
                        //Add the speed.
                        Bodies[Legs].ApplyForce(new Vector2(-_speed, 0));
                        break;
                    }
                case (1):
                    {
                        //Add the speed.
                        Bodies[Legs].ApplyForce(new Vector2(_speed, 0));
                        break;
                    }
            }
        }
        /// <summary>
        /// Make the character's head and arms face the mouse correctly.
        /// </summary>
        /// <param name="facingDirection">The facing direction of the character.</param>
        public void FaceMouse(FacingDirection facingDirection)
        {
            //The head rotation. Erratic head rotation might be solved by using another currentRotation factor.
            float headRotation = Helper.TurnToFace(Bodies[Head].Position,
                new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                    SpriteManager["Back Arm"].Rotation, 1, facingDirection);
            //Limit the Head angle.
            _AngleNeckJoint.TargetAngle = LimitHeadAngle(headRotation);

            //The Arm rotation.
            float armRotation = Helper.TurnToFace(SpriteManager["Torso"].Position,
                new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                SpriteManager["Back Arm"].Rotation, 1, facingDirection);
            //Rotate the arms.
            RotateArms(armRotation);
        }
        /// <summary>
        /// Limit the head angle.
        /// </summary>
        /// <param name="rotation">The current head rotation.</param>
        /// <returns></returns>
        float LimitHeadAngle(float rotation)
        {
            //Return the limited head angle.
            return (Helper.LimitAngle(rotation, -.5f, .5f));
        }
        /// <summary>
        /// Rotate the arms of the character.
        /// </summary>
        /// <param name="rotation">The rotation parameter.</param>
        public void RotateArms(float rotation)
        {
            //Sprite.SpriteRotation[Torso, 1] = Sprite.AddAngles(rotation, -Calculator.DegreesToRadians(20));
            SpriteManager["Front Arm"].Rotation = ((rotation - 0.1f) / 2);
            SpriteManager["Back Arm"].Rotation = rotation;

            //Check if the character is wielding a weapon.
            if (_WeaponWield1 != null)
            {
                switch (_WeaponWield1.WieldType)
                {
                    case (Weapon.WeaponWieldType.Single):
                        {
                            //The back of the weapon.
                            Vector2 weaponBack = Helper.CalculateOrbitPosition(_WeaponWield1.Bodies[0].Position,
                                SpriteManager.SubtractAngles(_WeaponWield1.Bodies[0].Rotation, (float)Math.PI), 15);

                            /*Sprite.SpriteRotation[Torso, 1] =
                                -Sprite.CalculateAngleFromOrbitPosition(Sprite.SpritePosition[Torso, 1], 25,
                                weaponBack);*/

                            /*Sprite.SpriteRotation[Torso, 1] = Helper.TurnToFace(Sprite.SpritePosition[Torso, 1],
                                weaponBack, Sprite.AddAngles(Sprite.SpriteRotation[Torso, 1],
                                -Calculator.DegreesToRadians(80)), 1, FacingDirection);*/
/*
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// Face an angle.
        /// </summary>
        /// <param name="position">The current position.</param>
        /// <param name="facePosition">The position of the point to face.</param>
        /// <param name="facingRight">If the charcter is facing right.</param>
        /// <returns>The desired angle.</returns>
        public float FaceAngle(Vector2 position, Vector2 facePosition, bool facingRight)
        {
            //Calculate the desired angle.
            float desiredAngle = (float)Math.Atan2(facePosition.Y - position.Y, facePosition.X - position.X);
            //If the charcter is facing left add PI to the desired angle.
            if (facingRight == false) { desiredAngle += (float)Math.PI; }

            //Return the desired angle.
            return desiredAngle;
        }
        /// <summary>
        /// Change the Sprite animation direction.
        /// </summary>
        /// <param name="facingDirection">The facing direction of the character.</param>
        /// <param name="horizontalMovingDirection">The horizontal moving direction.</param>
        public void ChangeSpriteAnimationDirection(FacingDirection facingDirection,
            HorizontalMovingDirections horizontalMovingDirection)
        {
            //Moving Left.
            if (horizontalMovingDirection == HorizontalMovingDirections.Left)
            {
                //Facing Left.
                if (facingDirection == FacingDirection.Left)
                {
                    SpriteManager["Legs"].AnimationDirection = true;
                }
                else { SpriteManager["Legs"].AnimationDirection = false; }
            }
            //Moving Right.
            else if (horizontalMovingDirection == HorizontalMovingDirections.Right)
            {
                //Facing Left.
                if (facingDirection == FacingDirection.Left)
                {
                    SpriteManager["Legs"].AnimationDirection = false;
                }
                else { SpriteManager["Legs"].AnimationDirection = true; }
            }
        }
        /// <summary>
        /// Change the facing direction.
        /// </summary>
        public void ChangeFacingDirection()
        {
            //The Direction.
            if (Bodies[Torso].Position.X > Mouse.GetState().X)
            {
                //If the character is facing right, flip it.
                if (FacingDirection == FacingDirection.Right) { FlipSprite(FacingDirection); }
            }
            //If the character is facing left, flip it.
            else { if (FacingDirection == FacingDirection.Left) { FlipSprite(FacingDirection); } }
        }
        /// <summary>
        /// Change the horizontal moving direction.
        /// </summary>
        public void ChangeHorizontalMovingDirection()
        {
            //Moving Right.
            if (Bodies[Legs].LinearVelocity.X > 1)
            {
                //Moving horizontally.
                _IsMovingHorizontally = true;
                //Moving right.
                _HorizontalMovingDirection = HorizontalMovingDirections.Right;
                //Change the sprite animation direction.
                ChangeSpriteAnimationDirection(FacingDirection, _HorizontalMovingDirection);
            }
            //Moving Left.
            else if (Bodies[Legs].LinearVelocity.X < -1)
            {
                //Moving horizontally.
                _IsMovingHorizontally = true;
                //Moving left.
                _HorizontalMovingDirection = HorizontalMovingDirections.Left;
                //Change the sprite animation direction.
                ChangeSpriteAnimationDirection(FacingDirection, _HorizontalMovingDirection);
            }
            //Standing Still.
            else { _IsMovingHorizontally = false; }
        }
        /// <summary>
        /// Choose the correct sprite for the Spartan.
        /// </summary>
        public void ChoosingSprite()
        {
            //If not moving vertical.
            if (_IsMovingVertically == false)
            {
                //If moving horizontal but not vertical.
                if (_IsMovingHorizontally == true)
                {
                    SpriteManager["Legs"].EnableAnimation = true;
                    SpriteManager["Legs"].FrameStartIndex = 1;
                    SpriteManager["Legs"].FrameEndIndex = 11;

                    //Change the sprite to running if another sprite is loaded.
                    if (SpriteManager["Legs"].CurrentFrameIndex > SpriteManager["Legs"].FrameEndIndex)
                    {
                        SpriteManager["Legs"].CurrentFrameIndex = 1;
                        SpriteManager["Legs"].LoadFrame();
                    }
                }
                //If not moving at all.
                else
                {
                    SpriteManager["Legs"].EnableAnimation = false;
                    SpriteManager["Legs"].CurrentFrameIndex = 0;
                    SpriteManager["Legs"].LoadFrame();
                }
            }
            //If moving vertical.
            else
            {
                SpriteManager["Legs"].CurrentFrameIndex = 16;
                SpriteManager["Legs"].LoadFrame();
                SpriteManager["Legs"].EnableAnimation = false;
                //Sprite.SpriteFrameStartIndex[Legs, 0] = 12;
                //Sprite.SpriteFrameEndIndex[Legs, 0] = 18;               
            }
        }
        /// <summary>
        /// Wielding a Weapon.
        /// </summary>
        public void WeaponWielding()
        {
            //If the Spartan wields a weapon.
            if (_WeaponWield1 != null) { _WeaponWield1.EquipedWeapon(this, Weapon.WeaponWieldMode.Main); }
        }
        /// <summary>
        /// Flip the Sprite Horizontally.
        /// </summary>
        /// <param name="direction">The current facing direction.</param>
        public void FlipSprite(FacingDirection direction)
        {
            //If facing right.
            if (direction == FacingDirection.Right)
            {
                //Loop through all sprites.
                for (int sprite = 0; sprite < SpriteManager.SpriteCount; sprite++)
                {
                    //Flip the sprite.
                    SpriteManager[sprite].Orientation = Orientation.Left;
                }

                //Flip to the Left.

                //Dispose of the neck joints.
                _AngleNeckJoint.Dispose();
                _RevoluteNeckJoint.Dispose();
                //Alter the head rotation.
                if (Bodies[Head].Rotation > 5) { Bodies[Head].Rotation = (float)(2 * Math.PI); }
                else if (Bodies[Head].Rotation < 1) { Bodies[Head].Rotation = 0; }
                //Determine the new head position.
                Bodies[Head].Position = new Vector2(Bodies[Torso].Position.X - 3, Bodies[Torso].Position.Y - 10);
                //Create the new Neck Joints.
                _AngleNeckJoint = JointFactory.Instance.CreateAngleJoint(System.PhysicsSimulator,
                    Bodies[Head], Bodies[Torso]);
                _RevoluteNeckJoint = JointFactory.Instance.CreateRevoluteJoint(System.PhysicsSimulator,
                    Bodies[Head], Bodies[Torso], new Vector2(Bodies[Torso].Position.X, Bodies[Torso].Position.Y - 5));

                //Set the new rotation offset and sprite origin points.
                SpriteManager["Front Arm"].RotationOffset = SpriteManager.AddAngles(Bodies[Torso].Rotation,
                    Calculator.DegreesToRadians(60));
                SpriteManager["Back Arm"].RotationOffset = SpriteManager.AddAngles(Bodies[Torso].Rotation,
                    Calculator.DegreesToRadians(290));
                SpriteManager["Front Arm"][0].OriginX = (SpriteManager["Front Arm"][0].Width - 7);
                SpriteManager["Back Arm"][0].OriginX = SpriteManager["Back Arm"][0].Width;

                //Change the facing direction.
                FacingDirection = FacingDirection.Left;
            }
            else
            {
                //Loop through all sprites.
                for (int sprite = 0; sprite < SpriteManager.SpriteCount; sprite++)
                {
                    //Flip the sprite.
                    SpriteManager[sprite].Orientation = Orientation.Right;
                }

                //Flip to the Right.

                //Dispose of the Neck Joints.
                _AngleNeckJoint.Dispose();
                _RevoluteNeckJoint.Dispose();

                //Alter the head rotation.
                if (Bodies[Head].Rotation > 5) { Bodies[Head].Rotation = (float)(2 * Math.PI); }
                else if (Bodies[Head].Rotation < 1) { Bodies[Head].Rotation = 0; }
                //Set the new head position.
                Bodies[Head].Position = new Vector2(Bodies[Torso].Position.X + 2, Bodies[Torso].Position.Y - 10);
                //Create the new Neck Joints.
                _AngleNeckJoint = JointFactory.Instance.CreateAngleJoint(System.PhysicsSimulator,
                    Bodies[Head], Bodies[Torso]);
                _RevoluteNeckJoint = JointFactory.Instance.CreateRevoluteJoint(System.PhysicsSimulator,
                    Bodies[Head], Bodies[Torso], new Vector2(Bodies[Torso].Position.X, Bodies[Torso].Position.Y - 5));
                //Set the Sprite rotation offset and sprite origin.
                SpriteManager["Front Arm"].RotationOffset =
                    SpriteManager.SubtractAngles(Bodies[Torso].Rotation, Calculator.DegreesToRadians(60));
                SpriteManager["Back Arm"].RotationOffset =
                    SpriteManager.SubtractAngles(Bodies[Torso].Rotation, Calculator.DegreesToRadians(290));
                SpriteManager["Front Arm"][0].OriginX = 7;
                SpriteManager["Back Arm"][0].OriginX = 0;

                //Change the facing direction.
                FacingDirection = FacingDirection.Right;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The Angle Neck Joint.
        /// </summary>
        public AngleJoint AngleNeckJoint
        {
            get { return (_AngleNeckJoint); }
            set { _AngleNeckJoint = value; }
        }
        /// <summary>
        /// The Angle Limit Hip Joint.
        /// </summary>
        public AngleLimitJoint AngleLimitHipJoint
        {
            get { return (_AngleLimitHipJoint); }
            set { _AngleLimitHipJoint = value; }
        }
        /// <summary>
        /// The Revolute Neck Joint.
        /// </summary>
        public RevoluteJoint RevoluteNeckJoint
        {
            get { return (_RevoluteNeckJoint); }
            set { _RevoluteNeckJoint = value; }
        }
        /// <summary>
        /// The Revolute Hip Joint.
        /// </summary>
        public RevoluteJoint RevoluteHipJoint
        {
            get { return (_RevoluteHipJoint); }
            set { _RevoluteHipJoint = value; }
        }
        /// <summary>
        /// If the Spartan can move.
        /// </summary>
        public bool CanMove
        {
            get { return (_CanMove); }
            set { _CanMove = value; }
        }
        /// <summary>
        /// If the Spartan can jump.
        /// </summary>
        public bool CanJump
        {
            get { return (_CanJump); }
            set { _CanJump = value; }
        }
        /// <summary>
        /// If the Spartan is moving horizontally.
        /// </summary>
        public bool IsMovingHorizontally
        {
            get { return (_IsMovingHorizontally); }
            set { _IsMovingHorizontally = value; }
        }
        /// <summary>
        /// If the Spartan is moving vertically.
        /// </summary>
        public bool IsMovingVertically
        {
            get { return (_IsMovingVertically); }
            set { _IsMovingVertically = value; }
        }
        /// <summary>
        /// The maximum movement speed.
        /// </summary>
        public float MaxSpeed
        {
            get { return (_MaxSpeed); }
            set { _MaxSpeed = value; }
        }
        /// <summary>
        /// The base movement speed.
        /// </summary>
        public float BaseSpeed
        {
            get { return (_BaseSpeed); }
            set { _BaseSpeed = value; }
        }
        /// <summary>
        /// The base jump speed.
        /// </summary>
        public float BaseJumpSpeed
        {
            get { return (_BaseJumpSpeed); }
            set { _BaseJumpSpeed = value; }
        }
        /// <summary>
        /// The Spartan's health.
        /// </summary>
        public float Health
        {
            get { return (_Health); }
            set { _Health = value; }
        }
        /// <summary>
        /// The Horizontal Moving Direction.
        /// </summary>
        public HorizontalMovingDirections HorizontalMovingDirection
        {
            get { return (_HorizontalMovingDirection); }
            set { _HorizontalMovingDirection = value; }
        }
        /// <summary>
        /// The main weapon wield.
        /// </summary>
        public Weapon WeaponWield1
        {
            get { return (_WeaponWield1); }
            set { _WeaponWield1 = value; }
        }
        /// <summary>
        /// The secondary weapon wield.
        /// </summary>
        public Weapon WeaponWield2
        {
            get { return (_WeaponWield2); }
            set { _WeaponWield2 = value; }
        }
        /// <summary>
        /// The base position of the Spartan body, that will say the center of the whole body.
        /// </summary>
        public Vector2 BasePosition
        {
            get { return (_BasePosition); }
            set { _BasePosition = value; }
        }
        #endregion
    }
}
*/