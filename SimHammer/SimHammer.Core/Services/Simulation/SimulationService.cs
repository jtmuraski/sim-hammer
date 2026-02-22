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
    private IRangedCombatService _rangedCombatService;
    private IMeleeCombatService _meleeCombatService;
    
    // ---Contructors---
    public SimulationService(ILogger<SimulationService> logger, IRangedCombatService rangedCombatService, IMeleeCombatService meleeCombatService)
    {
        _logger = logger;
        _rangedCombatService = rangedCombatService;
        _meleeCombatService = meleeCombatService;
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
            SimResult.CombatRounds.Add(SimResult.IsMeleeCombat ? _meleeCombatService.SimulateMeleeCombatRound(attacker, defender, i) : 
                                                                _rangedCombatService.SimulateRangedCombatRound(attacker, defender, i));
            roundsCompleted++;
        }

        SimResult.TotalRounds = roundsCompleted;
        SimResult.EndTime = DateTime.Now;

        return;
    }
}
