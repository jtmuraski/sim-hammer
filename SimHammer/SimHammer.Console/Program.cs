using Spectre.Console;
using SimHammer.Core.Models;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Simulation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SimHammer.Core.Services.Interfaces;

// -------Set up Logging for console app-------
string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
string logFilePath = $"Logs/SimHammer_{timeStamp}.txt";
var serviceProvider = new ServiceCollection()    
    .AddLogging(builder =>
       {
           builder.AddFile(logFilePath)
                  .SetMinimumLevel(LogLevel.Information);
       })
    .AddSingleton<IRangedCombatService, RangedCombatService>()
    .AddSingleton<SimulationService>()
    .BuildServiceProvider();

var logger = serviceProvider.GetService<ILogger<Program>>();

Console.WriteLine("Building units...");
// Create the units for the console test
Unit attacker = new Unit()
{ 
    Name = "Intercessor Squad",
    Faction = "Space Marines",
    Subfaction = "Ultramraines",
    Movement = 6,
    Toughness = 4,
    Wounds = 2,
    Leadership = 6,
    Save = 3,
    InvulnSave = 0,
    ModelCount = 10,
    MeleeWeapons = new List<MeleeWeapon>(),
    RangedWeapons = new List<RangedWeapon>(),
};
attacker.MeleeWeapons.Add(new MeleeWeapon("Knife", 0, 2, 2, 0, 1));
attacker.RangedWeapons.Add(new RangedWeapon("Bolter",24,2,3,4,-1,1,10));

Unit defender = new Unit()
{
    Name = "Boyz",
    Faction  = "Orks",
    Subfaction = "None",
    Movement = 6,
    Toughness = 5,
    Wounds = 1,
    Leadership = 7,
    Save = 5,
    InvulnSave = 0,
    ModelCount = 19,
    MeleeWeapons = new List<MeleeWeapon>(),
    RangedWeapons = new List<RangedWeapon>(),
};
defender.MeleeWeapons.Add(new MeleeWeapon("Bayonet", 0, 1, 2, 0, 1));
defender.RangedWeapons.Add(new RangedWeapon("Lasgun", 24, 1, 3, 0, 1,1, 20));

Console.WriteLine("Attacking and Defending unit has been built");

Console.Write("Beginning Simulation...");

var simLogger = serviceProvider.GetService<ILogger<SimulationService>>();
var sim = serviceProvider.GetRequiredService<SimulationService>();

sim.BeginSimulation(attacker, defender, 1000, false);

Console.WriteLine("Simulation has been completed");
foreach(var result in sim.SimResult.CombatRounds)
{
    Console.WriteLine($"=======Round {result.SimNumber}========");
    Console.WriteLine($"Total Attacks Made: {result.TotalAttacksMade}");
    Console.WriteLine($"Total Hits Scored: {result.TotalHits}");
    Console.WriteLine($"Total Wounds Inflicted: {result.TotalWoundsInflicted}");
    Console.WriteLine($"Total Saves Made: {result.TotalSavesMade}");
    Console.WriteLine($"Total InvulnSaves Made: {result.TotalInvulnSavesMade}");
    Console.WriteLine($"Total Damage Dealt: {result.TotalDamageDealt}");
    Console.WriteLine($"Models Kiled: {result.ModelsKilled}");
    Console.WriteLine("========================================");
}

Console.WriteLine();
Console.WriteLine("======Overall Results======");
Console.WriteLine($"Total Rounds Simulated: {sim.SimResult.TotalRounds}");
Console.WriteLine($"Average Hits Scored: {sim.SimResult.CombatRounds.Average(r => r.TotalHits)}");
Console.WriteLine($"Average Wounds Inflicted: {sim.SimResult.CombatRounds.Average(r => r.TotalWoundsInflicted)}");
Console.WriteLine($"Average Saves Made: {sim.SimResult.CombatRounds.Average(r => r.TotalSavesMade)}");
Console.WriteLine($"Average Damage Dealt: {sim.SimResult.CombatRounds.Average(r => r.TotalDamageDealt)}");
Console.WriteLine($"Average Models Killed: {sim.SimResult.CombatRounds.Average(r => r.ModelsKilled)}");


Console.ReadLine();

