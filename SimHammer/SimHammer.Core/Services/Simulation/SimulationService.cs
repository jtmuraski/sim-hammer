using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using SimHammer.Core.Models.Simulation;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace SimHammer.Core.Services.Simulation;

public class SimulationService : ISimulationService
{
    // ---Fields---
    public SimulationResult SimResult { get;set ; }
    bool IsSimulationComplete { get; set; }

    // These functions will manage the flow of the combat simulation

    
    // ---Contructors---
    public SimulationService()
    {
        SimResult = new SimulationResult();
        IsSimulationComplete = false;
    }

    public void BeginSimulation(Unit attacker, Unit defender, int rounds, bool isMelee = false)
    {
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
            WeaponResult result = new WeaponResult();

            int numAttacks = weapon.Attacks * weapon.Quantity;

            // foreach attack, determine if it hits, causes a wound and then does damage
            for (int i = 1; i <= numAttacks; i++)
            {
                result.AttacksMade++;

                // Determine the number of hits and wounds from this weapon
                int hitRoll = DiceRoller.RollD6();
                if (hitRoll == 6)
                {
                    result.Hits++;
                }
                else
                {
                    // Compare the Strength of the weapon against the Toughness of the defender
                    if (hitRoll >= weapon.BallisticSkill)
                    {
                        result.Hits++;
                    }
                }
            }
            if (result.Hits == 0)
                return round;

            // Calculate wounds inflicted for each hit
            for (int i = 0; i < result.Hits; i++)
            {
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

                int woundRoll = DiceRoller.RollD6();
                bool isWounded = false;
                if (woundRoll == 6)
                {
                    result.WoundsInflicted++;
                }
                if (woundRoll >= resultNeeded)
                {
                    result.WoundsInflicted++;
                }
            }
            if (result.WoundsInflicted == 0)
                return round;

            // Calculate saves for each wound that was inflicted
            for (int i = 0; i < result.WoundsInflicted; i++)
            {
                // If a wound was caused, roll for saves. If the defender has an invulnerable save, determine whether to use
                // the Invuln save or regular save (if Invuln < (Save - weapon.AP))
                int saveRoll = DiceRoller.RollD6(); ;
                int apRollResult = saveRoll + weapon.ArmourPiercing;

                if (saveRoll > 1 && apRollResult > defender.Save)
                {
                    // The save has failed, so calculate the damage
                    result.DamageDealt += weapon.Damage;
                }
                else
                {
                    result.SavesMade++;
                }
            }

            round.WeaponResults.Add(result);
        }

        // Calculate the total number of models killed
        int totalWounds = round.WeaponResults.Sum(wr => wr.DamageDealt);
        round.ModelsKilled = totalWounds / defender.Wounds;
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
