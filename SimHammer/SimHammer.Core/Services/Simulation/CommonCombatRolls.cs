using Microsoft.Extensions.Logging;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Services.Simulation
{
    public class CommonCombatRolls : ICommonCombatRolls
    {
        // ---Properties---
        private readonly ILogger<ICommonCombatRolls> _logger;
        private readonly IDiceRoller _diceRoller;

        // ---Constructors---
        public CommonCombatRolls(ILogger<ICommonCombatRolls> logger, IDiceRoller diceRoller)
        {
            _logger = logger;
            _diceRoller = diceRoller;
        }

        // ---Combat Methods---
        /// <summary>
        /// Roll all of the Hit die for a weapon. Returns the number of successful hits based on the weapon skill. Hit = roll > weaponsSkill. 6 always hits, 1 always misses.
        /// </summary>
        /// <param name="numAttacks"></param>
        /// <param name="weaponSkill"></param>
        /// <returns></returns>
        public int RollForHits(int numAttacks, int weaponSkill)
        {
            _logger.LogInformation($"Roll needed for a hit: {weaponSkill}");
            int totalHits = 0;
            for (int i = 1; i <= numAttacks; i++)
            {
                // Determine the number of hits and wounds from this weapon
                int hitRoll = _diceRoller.RollD6();
                _logger.LogInformation($"Hit roll: {hitRoll}");
                if (hitRoll == 6)
                {
                    _logger.LogInformation("Roll is an automatic hit");
                    totalHits++;
                }
                else if (hitRoll == 1)
                {
                    _logger.LogInformation("Roll is an automatic miss");
                }
                else
                {
                    // Compare the Strength of the weapon against the Toughness of the defender
                    if (hitRoll >= weaponSkill)
                    {
                        _logger.LogInformation("Roll is a hit");
                        totalHits++;
                    }
                }
            }

            return totalHits;
        }

        /// <summary>
        /// Calculate the roll needed for a hit to cause a wound
        /// </summary>
        /// <param name="weaponStrength"></param>
        /// <param name="defenderToughness"></param>
        /// <returns></returns>
        public int CalculateRollToWound(int weaponStrength, int defenderToughness)
        {
            if (weaponStrength >= (defenderToughness * 2.00))
            {
                return 2; // Wound on 2+ if Strength is double Toughness
            }
            else if (weaponStrength > defenderToughness)
            {
                return 3; // Wound on 3+ if Strength is greater than Toughness
            }
            else if (weaponStrength == defenderToughness)
            {
                return 4; // Wound on 4+ if Strength equals Toughness
            }
            else if (weaponStrength < (defenderToughness / 2.00))
            {
                // Wound on 6+ if Strength is less than half the Toughness
                return 6;
            }
            else
            {
                // Wound on a 5+ if strength is less than Toughness
                return 5;
            }
        }

        /// <summary>
        /// For each hit, roll a die to see if a wound was caused. Wound = roll > rollNeeded. 6 always wounds. 
        /// </summary>
        /// <param name="numHits"></param>
        /// <param name="rollNeeded"></param>
        /// <returns></returns>
        public int RollForWounds(int numHits, int rollNeeded)
        {

            int woundsInflicted = 0;
            for (int i = 0; i < numHits; i++)
            {
                int woundRoll = _diceRoller.RollD6();
                _logger.LogInformation($"Wound roll: {woundRoll}");

                if (woundRoll == 6)
                {
                    _logger.LogInformation("Roll is an automatic wound");
                    woundsInflicted++;
                }
                else if (woundRoll >= rollNeeded)
                {
                    _logger.LogInformation("Roll causes a wound");
                    woundsInflicted++;
                }
            }

            return woundsInflicted;
        }

        /// <summary>
        /// Roll a save die for each wound that was caused. Save = roll + AP >= save needed. 1 always fails, 6 always saves. If the defender has an invulnerable save, determine whether to use the Invuln save or regular save (if Invuln < (Save - weapon.AP))
        /// </summary>
        /// <param name="woundsInflicted"></param>
        /// <param name="weaponApValue"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        public int RollForSaves(int woundsInflicted, int weaponApValue, Unit defender)
        {
            int savesMade = 0;
            for (int i = 0; i < woundsInflicted; i++)
            {
                // Roll for the save and then add the weapon's AP value
                // Then compare the value to the save and Invuln sace
                int saveRoll  = _diceRoller.RollD6();
                int modifiedSave = saveRoll + weaponApValue;

                _logger.LogDebug($"Save Roll: {saveRoll}");
                _logger.LogDebug($"Modified Save: {modifiedSave}");
                _logger.LogDebug($"Armor Save: {defender.Save}, Invuln Save: {defender.InvulnSave}");
                if (saveRoll > 1)
                {
                    // Check against regular armor save
                    if(modifiedSave >= defender.Save)
                    {
                        
                        savesMade++;
                    }
                    else if(defender.HasInvulnSave && saveRoll >= defender.InvulnSave)
                    {
                        savesMade++;
                    }
                }
                else
                {
                    _logger.LogDebug("Save roll = 1. Auto fail.");
                }
            }

            return savesMade;
        }

        /// <summary>
        /// Foreach failed save, calculate the damage casued by the weapon.
        /// </summary>
        /// <param name="woundsInflicted"></param>
        /// <param name="weaponDamage"></param>
        /// <returns></returns>
        public int CalculateDamage(int woundsInflicted, int weaponDamage)
        {
            return weaponDamage * woundsInflicted;
        }
    }
}
