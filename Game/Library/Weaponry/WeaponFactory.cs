/*using System;
using System.Collections.Generic;
using System.Text;

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

namespace Library.Weaponry
{
    /// <summary>
    /// The factory that produces weapon at your leisure.
    /// </summary>
    public static class WeaponFactory
    {
        #region Methods
        /// <summary>
        /// Create a Sub machine gun.
        /// </summary>
        /// <param name="position">The position of the weapon.</param>
        /// <returns>The sub machine gun.</returns>
        public static Weapon CreateSMG(Vector2 position)
        {
            //Create the weapon and return it.
            return (new Weapon("SMG", "M7 Sub Machine Gun", 24, 11, 1, Calculator.DegreesToRadians(105),
                 Calculator.DegreesToRadians(20), 15, -12, false, -1, "Weapon/Textures/M7SMGV1", position,
                 Weapon.WeaponWieldType.Dual, Weapon.WeaponNameType.SMG));
        }
        /// <summary>
        /// Create a Battle Rifle.
        /// </summary>
        /// <param name="position">The position of the weapon.</param>
        /// <returns>The Battle Rifle.</returns>
        public static Weapon CreateBattleRifle(Vector2 position)
        {
            /*Weapon weapon = new Weapon("Battle Rifle", "BR55 Battle Rifle", 37, 15, 2,
                Calculator.DegreesToRadians(110), Calculator.DegreesToRadians(25), 12, -10, false, -1,
                "Weapon/BR55BattleRifle[2]", position, Weapon.WeaponWieldType.Single,
                Weapon.WeaponNameType.BattleRifle);*//*

            //Return the created weapon.
            return (new Weapon("Battle Rifle", "BR55 Battle Rifle", 37, 15, 2,
                Calculator.DegreesToRadians(0), Calculator.DegreesToRadians(0), 0, 0, false, -1,
                "Weapon/Textures/BR55BattleRifle[2]", position, Weapon.WeaponWieldType.Single,
                Weapon.WeaponNameType.BattleRifle));
        }
        #endregion
    }
}
*/