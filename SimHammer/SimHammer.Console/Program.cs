using Spectre.Console;
using SimHammer.Core.Models;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Simulation;

// See https://aka.ms/new-console-template for more information

Console.WriteLine("Building units...");
// Create the units for the console test
Unit attacker = new Unit()
{ 
    Name = "Intercessor Squad",
    Faction = "Space Marines",
    Subfaction = "Ultramraines",
    Movement = 12,
    Strength = 3,
    Toughness = 4,
    Wounds = 2,
    Leadership = 8,
    Save = 4,
    InvulnSave = 0,
    ModelCount = 10,
    MeleeWeapons = new List<MeleeWeapon>(),
    RangedWeapons = new List<RangedWeapon>(),
};
attacker.MeleeWeapons.Add(new MeleeWeapon("Knife", 0, 2, 2, 0, 1));
attacker.RangedWeapons.Add(new RangedWeapon("Bolter", 2, 5, 1, 2, 2,2, 10));

Unit defender = new Unit()
{
    Name = "Infantry Platoon",
    Faction  = "Imperial Guard",
    Subfaction = "Cadian",
    Movement = 12,
    Strength = 3,
    Toughness = 4,
    Wounds = 2,
    Leadership = 8,
    Save = 4,
    InvulnSave = 0,
    ModelCount = 20,
    MeleeWeapons = new List<MeleeWeapon>(),
    RangedWeapons = new List<RangedWeapon>(),
};
defender.MeleeWeapons.Add(new MeleeWeapon("Bayonet", 0, 1, 2, 0, 1));
defender.RangedWeapons.Add(new RangedWeapon("Lasgun", 24, 1, 3, 0, 1,1, 20));

Console.WriteLine("Attacking and Defending unit has been built");

Console.Write("Beginning Simulation...");

SimulationService sim = new SimulationService();
sim.BeginSimulation(attacker, defender, 5, false);

Console.WriteLine("Simulation has been completed");
foreach(var result in sim.SimResult.CombatRounds)
{
    Console.WriteLine($"=======Round {result.SimNumber}========");
    Console.WriteLine($"Total Attacks Made: {result.TotalAttacksMade}");
    Console.WriteLine($"Total Hits SCored: {result.TotalHits}");
    Console.WriteLine($"Total Saves Made: {result.TotalSavesMade}");
    Console.WriteLine($"Total InvulnSaves Made: {result.TotalInvulnSavesMade}");
    Console.WriteLine($"Total Damage Dealt: {result.TotalDamageDealt}");
    Console.WriteLine($"Models Kiled: {result.ModelsKilled}");
    Console.WriteLine("========================================");
}
Console.ReadLine();

