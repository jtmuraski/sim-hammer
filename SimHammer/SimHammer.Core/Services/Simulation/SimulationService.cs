using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using SimHammer.Core.Models.Simulation;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace SimHammer.Core.Services.Simulation;

public class SimulationService : ISimulationService
{
    // ---Fields---
    public SimulationResult SimResult { get;set ; }
    bool IsSimulationComplete { get; set; }
    private ILogger<SimulationService> _logger;
    
    // ---Contructors---
    public SimulationService(ILogger<SimulationService> logger)
    {
        _logger = logger;
        SimResult = new SimulationResult();
        IsSimulationComplete = false;
    }

    // These functions will manage the flow of the combat simulation
    public void BeginSimulation(Unit attacker, Unit defender, int rounds, bool isMelee = false)
    {
        _logger.LogInformation("========BEGINNING SIMULATION========");
        SimResult.Attacker = attacker;
        SimResult.Defender = defender;
        SimResult.TotalRounds = 0;
        SimResult.CombatRounds = new List<CombatRound>();
        SimResult.StartTime = DateTime.Now;
        SimResult.IsMeleeCombat = isMelee;

        int roundsCompleted = 0;

        for (int i = 1; i <= rounds; i++)
        {
            SimResult.CombatRounds.Add(SimResult.IsMeleeCombat ? SimulateMeleeCombatRound(attacker, defender, i) :
                                                           SimulateRangedCombatRound(attacker, defender, i));
            roundsCompleted++;
        }

        SimResult.TotalRounds = roundsCompleted;
        SimResult.EndTime = DateTime.Now;

        return;
    }

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
            _logger.LogInformation($"Roll needed for a hit: {weapon.BallisticSkill}");
            for (int i = 1; i <= numAttacks; i++)
            {
                _logger.LogInformation($"--Simulating attack {i} with weapon {weapon.Name}--");
                result.AttacksMade++;

                // Determine the number of hits and wounds from this weapon
                int hitRoll = DiceRoller.RollD6();
                _logger.LogInformation($"Hit roll: {hitRoll}");
                if (hitRoll == 6)
                {
                    _logger.LogInformation("Roll is an automatic hit");
                    result.Hits++;
                }
                else
                {
                    // Compare the Strength of the weapon against the Toughness of the defender
                    if (hitRoll >= weapon.BallisticSkill)
                    {
                        _logger.LogInformation("Roll is a hit");
                        result.Hits++;
                    }
                }
            }

            _logger.LogInformation($"Hit Rolls completed. Total hits: {result.Hits}");
            if (result.Hits == 0)
            {
                _logger.LogInformation("No hits were recorded. Ending this weapon attack");
                return round;
            }

            _logger.LogInformation("Checking for wounds inflicted");
            // Calculate what roll is needed to cause a wound
            int resultNeeded = 0;
            if (weapon.Strength >= (defender.Toughness * 2))
            {
                resultNeeded = 2; // Wound on 2+ if Strength is double Toughness
            }
            else if (weapon.Strength > defender.Toughness)
            {
                resultNeeded = 3; // Wound on 3+ if Strength is greater than Toughness
            }
            else if (weapon.Strength == defender.Toughness)
            {
                resultNeeded = 4; // Wound on 4+ if Strength equals Toughness
            }
            else if (weapon.Strength < defender.Toughness)
            {
                // Wound on 5+ if Strength is less than Toughness
                resultNeeded = 5;
            }
            else
            {
                resultNeeded = 6; // Wound on 5+ if Strength is less than Toughness
            }
            _logger.LogInformation($"With a weapon strength of {weapon.Strength} and a defender toughness of {defender.Toughness} the wound roll needed: {resultNeeded}");

            // For each hit, roll to see if it causes a wound
            for (int i = 0; i < result.Hits; i++)
            {
                int woundRoll = DiceRoller.RollD6();
                _logger.LogInformation($"Wound roll: {woundRoll}");
                if (woundRoll == 6)
                {
                    _logger.LogInformation("Roll is an automatic wound");
                    result.WoundsInflicted++;
                }
                else if (woundRoll >= resultNeeded)
                {
                    _logger.LogInformation("Roll causes a wound");
                    result.WoundsInflicted++;
                }
            }
            if (result.WoundsInflicted == 0)
            {
                _logger.LogInformation("No wounds were inflicted. Ending this weapon attack");
                return round;
            }
            _logger.LogInformation($"Wounds inflicted calculation completed. Total Wounds: {result.WoundsInflicted}");

            // Calculate saves for each wound that was inflicted
            _logger.LogInformation("Calculating saves for wounds inflicted");
            for (int i = 0; i < result.WoundsInflicted; i++)
            {
                // If a wound was caused, roll for saves. If the defender has an invulnerable save, determine whether to use
                // the Invuln save or regular save (if Invuln < (Save - weapon.AP))
                int saveRoll = DiceRoller.RollD6(); ;
                _logger.LogInformation($"Save roll: {saveRoll}");

                int apRollResult = saveRoll + weapon.ArmourPiercing;
                _logger.LogInformation($"Save roll after applying Armour Piercing ({weapon.ArmourPiercing}): {apRollResult}");

                if(saveRoll == 1)
                {
                    _logger.LogInformation("Save roll is a 1. Automatic failed save.");
                    result.DamageDealt += weapon.Damage;
                }
                else if(apRollResult < defender.Save)
                {
                    _logger.LogInformation("Save has failed. Applying damage.");
                    result.DamageDealt += weapon.Damage;
                }
                else
                {
                    _logger.LogInformation("Save successful. No damage applied.");
                    result.SavesMade++;
                }
            }

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
    
    public CombatRound SimulateMeleeCombatRound(Unit attacker, Unit defender, int roundNumber)
    {
        CombatRound round = new CombatRound
        {
            Id = roundNumber,
            SimNumber = roundNumber,
            ModelsKilled = 0,
            MoraleSuccessChance = 1.0 // Placeholder for morale success chance
        };

        // Simulate the combat logic here

        // For each attack, calculate hits, wounds, saves, etc.

        return round;
    }
}
