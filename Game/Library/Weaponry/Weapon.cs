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

using Library.Imagery;
using Library.Infrastructure;

namespace Library.Weaponry
{
    /// <summary>
    /// The base class of each weapon in the game.
    /// </summary>
    public class Weapon : Object
    {
        #region Fields
        //The name of the weapon.
        public string Name;
        //The full name of the weapon.
        public string FullName;
        //The height.
        private float _Height;
        //The width.
        private float _Width;
        //The mass.
        private float _Mass;
        //The Id.
        public int Id;
        //Whether the weapon is equiped or not.
        public bool Equiped;
        //The Id of the owner.
        public int OwnerId;
        //The owner of this weapon.
        public Object Owner;
        //The path of the weapon's sprite.
        public string SpritePath;
        //The list of projectiles.
        public List<Projectile> ProjectileList = new List<Projectile>();
        //The list of projectiles to remove.
        public List<Projectile> RemoveProjectileList = new List<Projectile>();
        //The fire delay.
        public float FireDelay = 0;
        //If the weapon is a main weapon.
        public bool MainWeapon;
        //The base position of the weapon.
        public Vector2 BasePosition;
        //The rotation add when equiped.
        public float[] EquipRotationAdd = new float[2];
        //The offset when equiped.
        public float[] EquipOffset = new float[2];

        /// <summary>
        /// The type of the weapon.
        /// </summary>
        public enum WeaponNameType
        {
            AssaultRifle,
            BattleRifle,
            BeamRifle,
            BruteShot,
            CovenantCarbine,
            EnergySword,
            FirebombGrenade,
            Flamethrower,
            FragGrenade,
            FuelRodGun,
            GravityHammer,
            MachineGunTurret,
            Magnum,
            Mauler,
            MissilePod,
            Needler,
            PlasmaCannon,
            PlasmaGrenade,
            PlasmaPistol,
            PlasmaRifle,
            RocketLauncher,
            SentinelBeam,
            Shotgun,
            SMG,
            SniperRifle,
            SpartanLaser,
            Spiker,
            Ball,
            Bomb,
            Flag,
            Melee
        }
        /// <summary>
        /// The wieled type. Single, dual or support.
        /// </summary>
        public enum WeaponWieldType
        {
            Single,
            Dual,
            Support
        }
        /// <summary>
        /// The wield mode. None, main or secondary.
        /// </summary>
        public enum WeaponWieldMode
        {
            None,
            Main,
            Secondary
        }

        //The weapon type.
        public WeaponNameType WeaponType;
        //The wield type.
        public WeaponWieldType WieldType;
        //The wield mode.
        public WeaponWieldMode WieldMode;

        //The angle joint, active when the weapon is equiped.
        public AngleJoint AngleJoint;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a weapon.
        /// </summary>
        /// <param name="name">The name of the weapon.</param>
        /// <param name="fullName">The full name of the weapon.</param>
        /// <param name="width">The width of the weapon.</param>
        /// <param name="height">The height of the weapon.</param>
        /// <param name="mass">The mass of the weapon.</param>
        /// <param name="rotationAddMain">The rotation add for the main weapon when equiped.</param>
        /// <param name="rotationAddSecondary">The rotation add for the secondary weapon when equiped.</param>
        /// <param name="offsetMain">The offset for the main weapon when equiped.</param>
        /// <param name="offsetSecondary">The offset for the secondary weapon when equiped.</param>
        /// <param name="equiped">If the weapon has been equiped.</param>
        /// <param name="ownerId">The Id of the owner if the weapon has been equiped.</param>
        /// <param name="weaponPath">The path of the weapon sprite.</param>
        /// <param name="position">The position of the weapon.</param>
        /// <param name="wieldType">The type of wielding this weapon uses.</param>
        /// <param name="weaponType">The type of weapon this is.</param>
        public Weapon(string name, string fullName, float width, float height, float mass,
            float rotationAddMain, float rotationAddSecondary, float offsetMain, float offsetSecondary,
            bool equiped, int ownerId, string weaponPath, Vector2 position, WeaponWieldType wieldType,
            WeaponNameType weaponType)
        {
            //The name.
            Name = name;
            //The full name.
            FullName = fullName;
            //The width.
            _Width = width;
            //The height.
            _Height = height;
            //The mass.
            _Mass = mass;
            //The rotation add when main equiped.
            EquipRotationAdd[0] = rotationAddMain;
            //The rotation add when secondary equiped.
            EquipRotationAdd[1] = rotationAddSecondary;
            //The main equip offset.
            EquipOffset[0] = offsetMain;
            //The secondary equip offset.
            EquipOffset[1] = offsetSecondary;
            //If the weapon has been equiped.
            Equiped = equiped;
            //The owner id.
            OwnerId = ownerId;
            //The first tag.
            FirstTag = "Weapon";
            //The second tag.
            SecondTag = "False";
            //The sprite path.
            SpritePath = weaponPath;
            //The base position.
            BasePosition = position;
            //The wield type.
            WieldType = wieldType;
            //The weapon type.
            WeaponType = weaponType;
        }
        #endregion

        #region Methods
        #region Main Methods
        /// <summary>
        /// Initialize the weapon.
        /// </summary>
        /// <param name="system">The System instance this weapon is a part of.</param>
        public override void Initialize(System system)
        {
            //The inherited method.
            base.Initialize(system);

            //Tell the System that you're a weapon.
            system.GetWeaponId(this);
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="contentManager">The Content Manager.</param>
        public override void LoadContent(ContentManager contentManager)
        {
            //The inherited method.
            base.LoadContent(contentManager);

            //Add a body.
            AddBody(SpritePath, BasePosition, 1f, 1f, _Width, _Height, 0, _Mass, 0f, "Weapon");
            //Set the collision group.
            Geoms[0].CollisionGroup = 1;
        }
        /// <summary>
        /// Update the weapon.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            //The inherited method.
            base.Update(gameTime);

            //If the weapon is equiped.
            if (Equiped) { UpdateEquiped(); }

            //List of projectiles that should be removed.
            if (RemoveProjectileList.Count > 0)
            {
                //Loop through each projectile in the removal list and remove them.
                foreach (Projectile p in RemoveProjectileList) { ProjectileList.Remove(p); p.Dispose(); }
                //Clear the list.
                RemoveProjectileList.Clear();
            }

            //If there's projectiles left in the list, loop through them and update them.
            if (ProjectileList.Count > 0) { foreach (Projectile p in ProjectileList) { p.Update(gameTime); } }
            //If a fire delay is still in effect, subtract a giving amount.
            if (FireDelay > 0) { FireDelay -= (100 * System.UpdateSpeed); }
        }
        /// <summary>
        /// Draw the weapon.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //The inherited method.
            base.Draw(spriteBatch);

            //If the projectile list isn't empty, draw each projectile.
            if (ProjectileList.Count > 0) { foreach (Projectile projectile in ProjectileList) { projectile.Draw(spriteBatch); } }
        }
        /// <summary>
        /// Handle all input.
        /// </summary>
        /// <param name="input">The input states.</param>
        /// <param name="i">The input state to consider.</param>
        public void HandleInput(InputState input, int i)
        {
            //If equiped.
            if (Equiped == true)
            {
                //If the fire delay has ceased to be and the right button is pressed.
                if ((FireDelay < 1) && (Mouse.GetState().LeftButton == ButtonState.Pressed))
                {
                    //Fire the weapon.
                    FireWeapon();
                }
            }

            #region Debug
            //The F key.
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.F))
            {
                //Make the weapon invisible.
                SpriteManager["Weapon"].Visibility = Visibility.Invisible;
            }
            //The G key.
            if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.G))
            {
                //Make the weapon visible.
                SpriteManager["Weapon"].Visibility = Visibility.Visible;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Check the owner's, if equiped, orientation and 
        /// </summary>
        void CheckFacingDirection()
        {
            //Check the owner's orientation.
            FacingDirection = Owner.FacingDirection;
        }
        /// <summary>
        /// Equip the weapon.
        /// </summary>
        /// <param name="equiper">The Object that equips the weapon.</param>
        /// <param name="wieldMode">The wield mode of the weapon. None, main or secondary.</param>
        public override void EquipedWeapon(Object equiper, WeaponWieldMode wieldMode)
        {
            //Tell the game that the weapon has been equiped.
            Equiped = true;
            //The owner.
            Owner = equiper;
            //The wield mode, none, main or secondary.
            WieldMode = wieldMode;
            //The second tag.
            SecondTag = "True";
            //Make the sprite invisible.
            SpriteManager["Weapon"].Visibility = Visibility.Invisible;
            //Make the weapon ignore gravity.
            Bodies[0].IgnoreGravity = true;

            //Change the Roation Center.
            AddGeometry(0, Geoms[0], new Vector2(-30, 0), 0);
            //Geometry[0].IsSensor = true;
            //Body[0] = BodyFactory.Instance.CreateBody(System.PhysicsSimulator, Body[0]);
            //Geometry[0] = GeomFactory.Instance.CreateGeom(System.PhysicsSimulator, Body[0], Geometry[0], new Vector2(-10, 0), 0);

            //Create the angle joint.
            AngleJoint = JointFactory.Instance.CreateAngleJoint(System.PhysicsSimulator, Bodies[0], equiper.Bodies[1]);
            //Update the equiped weapon.
            UpdateEquiped();
        }
        /// <summary>
        /// Unequip the weapon.
        /// </summary>
        public override void UnEquipedWeapon()
        {
            //Tell the game that the weapon has been dropped.
            Equiped = false;
            //Nullify the owner.
            Owner = null;
            //The second tag.
            SecondTag = "False";

            //Make the weapon visible.
            SpriteManager["Weapon"].Visibility = Visibility.Visible;
            //Make the weapon adhere to the common laws of gravity.
            Bodies[0].IgnoreGravity = false;
            //Dispose of the angle joint.
            AngleJoint.Dispose();
        }
        /// <summary>
        /// Update the equiped weapon.
        /// </summary>
        public void UpdateEquiped()
        {
            //Find out the orientation.
            CheckFacingDirection();

            //The orientation.
            switch (FacingDirection)
            {
                case (FacingDirection.Left):
                    {
                        //Set the orientation of the weapon sprite to left.
                        SpriteManager["Weapon"].Orientation = Orientation.Left;

                        //Set the rotation to that of the weapon's owner's arm rotation.
                        AngleJoint.TargetAngle = -Owner.SpriteManager["Back Arm"].Rotation;

                        //The wield type.
                        switch (WieldMode)
                        {
                            case (WeaponWieldMode.Main):
                                {
                                    //The weapon position.
                                    Bodies[0].Position = WeaponEquipPosition(WeaponWieldMode.Main, -EquipRotationAdd[0], EquipOffset[0]);
                                    break;
                                }
                            case (WeaponWieldMode.Secondary):
                                {
                                    //The weapon position.
                                    Bodies[0].Position = WeaponEquipPosition(WeaponWieldMode.Secondary, EquipRotationAdd[1], EquipOffset[1]);
                                    break;
                                }
                        }

                        break;
                    }
                case (FacingDirection.Right):
                    {
                        //Set the orientation of the weapon sprite to right.
                        SpriteManager["Weapon"].Orientation = Orientation.Right;

                        //Set the rotation to that of the weapon's owner's arm rotation.
                        AngleJoint.TargetAngle = -Owner.SpriteManager["Back Arm"].Rotation;

                        //The wield type.
                        switch (WieldMode)
                        {
                            case (WeaponWieldMode.Main):
                                {
                                    //The weapon position.
                                    Bodies[0].Position = WeaponEquipPosition(WeaponWieldMode.Main, EquipRotationAdd[0], EquipOffset[0]);
                                    break;
                                }
                            case (WeaponWieldMode.Secondary):
                                {
                                    //The weapon position.
                                    Bodies[0].Position = WeaponEquipPosition(WeaponWieldMode.Secondary, -EquipRotationAdd[1], EquipOffset[1]);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }
        /// <summary>
        /// Get the equiped weapon's position.
        /// </summary>
        /// <param name="wieldMode">The wielding mode.</param>
        /// <param name="addRotation">The rotation add.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The position.</returns>
        Vector2 WeaponEquipPosition(WeaponWieldMode wieldMode, float addRotation, float offset)
        {
            //Create the index.
            string tag = "";

            //Check the wield mode and set the index accordingly.
            switch (wieldMode)
            {
                case (WeaponWieldMode.Main): { tag = "Back Arm"; break; }
                case (WeaponWieldMode.Secondary): { tag = "Front Arm"; break; }
            }

            //Alter the position.
            Vector2 v = Vector2.Add(Owner.SpriteManager[tag].Position, new Vector2(8, 1));
            //The weapon position, an orbiting one.
            return (Helper.CalculateOrbitPosition(v,
                SpriteManager.AddAngles(Owner.SpriteManager[tag].Rotation, addRotation), offset));
        }
        /// <summary>
        /// Fire the weapon.
        /// </summary>
        public void FireWeapon()
        {
            //The temporary rotation.
            float tempRotation = 0;
            //Check the facing direction and either add just the weapon's rotation or also PI radians.
            if (FacingDirection == FacingDirection.Left) { tempRotation = Bodies[0].Rotation; }
            else if (FacingDirection == FacingDirection.Right)
            {
                tempRotation = Bodies[0].Rotation + (float)Math.PI;
            }

            //Create the projectile.
            Projectile tempProjectile = new Projectile("Weapon/Textures/M90ShotgunBulletV1", 1f, 1f, 0.1f, 0.5f,
                tempRotation, Bodies[0].Position, 30f, System, SpriteManager.ContentManager, null/*SpriteManager.SpriteBatch*//*);
            //Add the projectile's OnDispose event.
            tempProjectile.OnDispose += new Projectile.DisposeHandler(OnDispose);
            //Add the projectile to the list.
            ProjectileList.Add(tempProjectile);
            //Set the fire delay.
            FireDelay = 10;
        }
        /// <summary>
        /// Dispose of the projectile.
        /// </summary>
        /// <param name="projectile">The projectile object.</param>
        new public void OnDispose(Projectile projectile)
        {
            //Dispose of the projectile.
            RemoveProjectileList.Add(projectile);
        }
        #endregion
    }
}
*/