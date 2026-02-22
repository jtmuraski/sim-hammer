using Microsoft.Extensions.Logging;
using SimHammer.Core.Models.Simulation;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Services.Simulation
{
    public class MeleeCombatService : IMeleeCombatService
    {
        // ---Properties---
        private readonly ILogger<MeleeCombatService> _logger;
        private readonly IDiceRoller _diceRoller;
        private readonly ICommonCombatRolls _commonCombatRolls;

        // ---Fields---

        // ---Constructors---
        public MeleeCombatService(ILogger<MeleeCombatService> logger, IDiceRoller diceRoller, ICommonCombatRolls commonCombatRolls)
        {
            _logger = logger;
            _diceRoller = diceRoller;
            _commonCombatRolls = commonCombatRolls;
        }
        // ---Combat Methods---
        /// <summary>
        /// Being a round of Melee Combat between two units and return the results
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="roundNumber"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public CombatRound SimulateMeleeCombatRound(Unit attacker, Unit defender, int roundNumber)
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

            // Conduct combat for each melee weapon
            foreach (MeleeWeapon weapon in attacker.MeleeWeapons)
            {
                _logger.LogInformation($"==Simulating attacks with weapon {weapon.Name}==");
                WeaponResult result = new WeaponResult();

                int numAttacks = weapon.Attacks * weapon.Quantity;
                _logger.LogInformation($"Number of attacks with weapon: {numAttacks}");

                // foreach attack, determine if it hits, causes a wound and then does damage
                result.AttacksMade = numAttacks;
                result.Hits = _commonCombatRolls.RollForHits(weapon.Attacks * weapon.Quantity, weapon.WeaponSkill);


                _logger.LogInformation($"Hit Rolls completed. Total hits: {result.Hits}");
                if (result.Hits == 0)
                {
                    _logger.LogInformation("No hits were recorded. Ending this weapon attack");
                    continue;
                }

                _logger.LogInformation("Checking for wounds inflicted");
                int resultNeeded = _commonCombatRolls.CalculateRollToWound(weapon.Strength, defender.Toughness);

                _logger.LogInformation($"With a weapon strength of {weapon.Strength} and a defender toughness of {defender.Toughness} the wound roll needed: {resultNeeded}");
                result.WoundsInflicted = _commonCombatRolls.RollForWounds(result.Hits, resultNeeded);

                if (result.WoundsInflicted == 0)
                {
                    _logger.LogInformation("No wounds were inflicted. Ending this weapon attack");
                    continue;
                }
                _logger.LogInformation($"Wounds inflicted calculation completed. Total Wounds: {result.WoundsInflicted}");

                // Calculate saves for each wound that was inflicted
                _logger.LogInformation("Calculating saves for wounds inflicted");

                result.SavesMade = _commonCombatRolls.RollForSaves(result.WoundsInflicted, weapon.ArmorPiercing, defender);
                result.DamageDealt = _commonCombatRolls.CalculateDamage((result.WoundsInflicted - result.SavesMade), weapon.Damage);


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
    }
}
