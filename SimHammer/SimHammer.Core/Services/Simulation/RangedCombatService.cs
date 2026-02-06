using Microsoft.Extensions.Logging;
using SimHammer.Core.Models.Simulation;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Services.Simulation
{
    public class RangedCombatService : IRangedCombatService
    {
        // ---Properties---
        private readonly ILogger<IRangedCombatService> _logger;
        private readonly IDiceRoller _diceRoller;

        // ---FIelds---

        // ---Constructors---
        public RangedCombatService(ILogger<IRangedCombatService> logger, IDiceRoller diceRoller)
        {
            _logger = logger;
            _diceRoller = diceRoller;
        }

        // ---Combat Methods---
        /// <summary>
        /// Being a round of Rnaged Combat between two units and return the results
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="roundNumber"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public CombatRound SimulateRangedCombatRound(Unit attacker, Unit defender, int roundNumber)
        {
            _logger.LogInformation($"Simulating ranged combat round {roundNumber}");
            CombatRound round = new CombatRound
            {
                Id = roundNumber,
                SimNumber = roundNumber,
                ModelsKilled = 0,
                MoraleSuccessChance = 1.0 // Placeholder for morale success chance
            };

            // Simulate the combat logic here

            // Conduct combat for each ranged weapon
            foreach (RangedWeapon weapon in attacker.RangedWeapons)
            {
                _logger.LogInformation($"==Simulating attacks with weapon {weapon.Name}==");
                WeaponResult result = new WeaponResult();

                int numAttacks = weapon.Attacks * weapon.Quantity;
                _logger.LogInformation($"Number of attacks with weapon: {numAttacks}");

                // foreach attack, determine if it hits, causes a wound and then does damage
                result.AttacksMade = numAttacks;
                result.Hits = RollForHits(weapon.Attacks * weapon.Quantity, weapon.BallisticSkill);


                _logger.LogInformation($"Hit Rolls completed. Total hits: {result.Hits}");
                if (result.Hits == 0)
                {
                    _logger.LogInformation("No hits were recorded. Ending this weapon attack");
                    continue;
                }

                _logger.LogInformation("Checking for wounds inflicted");
                int resultNeeded = CalculateRollToWound(weapon.Strength, defender.Toughness);

                _logger.LogInformation($"With a weapon strength of {weapon.Strength} and a defender toughness of {defender.Toughness} the wound roll needed: {resultNeeded}");
                result.WoundsInflicted = RollForWounds(result.Hits, resultNeeded);

                if (result.WoundsInflicted == 0)
                {
                    _logger.LogInformation("No wounds were inflicted. Ending this weapon attack");
                    continue;
                }
                _logger.LogInformation($"Wounds inflicted calculation completed. Total Wounds: {result.WoundsInflicted}");

                // Calculate saves for each wound that was inflicted
                _logger.LogInformation("Calculating saves for wounds inflicted");

                result.SavesMade = RollForSaves(result.WoundsInflicted, weapon, defender, out int damageDealt);
                result.DamageDealt = damageDealt;


                round.WeaponResults.Add(result);
                _logger.LogInformation($"==Completed simulation for weapon {weapon.Name}==");
            }

            // Calculate the total number of models killed
            int totalWounds = round.WeaponResults.Sum(wr => wr.DamageDealt);
            if (totalWounds > 0)
                round.ModelsKilled = totalWounds / defender.Wounds;
            _logger.LogInformation($"Total wounds dealt in this round: {totalWounds}");
            _logger.LogInformation($"Total models killed in this round: {round.ModelsKilled}");

            _logger.LogInformation($"==Completed ranged combat round {roundNumber}==");
            return round;
        }

        internal int RollForHits(int numAttacks, int skillLevel)
        {
            _logger.LogInformation($"Roll needed for a hit: {skillLevel}");
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
                    if (hitRoll >= skillLevel)
                    {
                        _logger.LogInformation("Roll is a hit");
                        totalHits++;
                    }
                }
            }

            return totalHits;
        }

        internal int CalculateRollToWound(int weaponStrength, int defenderToughness)
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

        internal int RollForWounds(int numHits, int rollNeeded)
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

        internal int RollForSaves(int woundsInflicted, RangedWeapon weapon, Unit defender, out int damageDealt)
        {
            int savesMade = 0;
            damageDealt = 0;
            for (int i = 0; i < woundsInflicted; i++)
            {
                // If a wound was caused, roll for saves. If the defender has an invulnerable save, determine whether to use
                // the Invuln save or regular save (if Invuln < (Save - weapon.AP))
                int saveRoll = _diceRoller.RollD6(); ;
                _logger.LogInformation($"Save roll: {saveRoll}");

                int apRollResult = saveRoll + weapon.ArmourPiercing;
                _logger.LogInformation($"Save roll after applying Armour Piercing ({weapon.ArmourPiercing}): {apRollResult}");

                if (saveRoll == 1)
                {
                    _logger.LogInformation("Save roll is a 1. Automatic failed save.");
                    damageDealt += weapon.Damage;
                }
                else if (apRollResult < defender.Save)
                {
                    _logger.LogInformation("Save has failed. Applying damage.");
                    damageDealt += weapon.Damage;
                }
                else
                {
                    _logger.LogInformation("Save successful. No damage applied.");
                    savesMade++;
                }
            }

            return savesMade;
        }
    }
}
