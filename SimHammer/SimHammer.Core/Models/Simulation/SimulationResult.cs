using System;
using SimHammer.Core.Models.Units;

namespace SimHammer.Core.Models.Simulation;

public class SimulationResult
{
    // Properties
    public int Id { get; set; } // Unique identifier for the simulation result
    public int TotalRounds { get; set; } // Total number of combat rounds simulated
    public Unit Attacker {get; set;}
    public Unit Defender {get; set;}
    public List<CombatRound> CombatRounds { get; set; } // List of combat rounds in the simulation
}
