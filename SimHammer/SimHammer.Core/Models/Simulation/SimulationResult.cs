using System;
using SimHammer.Core.Models.Units;

namespace SimHammer.Core.Models.Simulation;

public class SimulationResult
{
    // Properties
    public int Id { get; set; } // Unique identifier for the simulation result
    public int TotalRounds { get; set; } // Total number of combat rounds simulated
    public Unit Attacker { get; set; }
    public Unit Defender { get; set; }
    public bool IsMeleeCombat { get; set; } // Indicates if the combat is melee or ranged
    public List<CombatRound> CombatRounds { get; set; } // List of combat rounds in the simulation
    public DateTime StartTime { get; set; } // Start time of the simulation
    public DateTime EndTime { get; set; } // End time of the simulation


    // Constructors
    public SimulationResult()
    {
        CombatRounds = new List<CombatRound>();
    }
}
