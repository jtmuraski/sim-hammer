using System;
using SimHammer.Core.Models.Simulation;
using SimHammer.Core.Models.Units;

namespace SimHammer.Core.Services.Interfaces;

public interface ISimulationService
{
    void BeginSimulation(Unit attacker, Unit defender, int rounds, bool isMelee = false);
    CombatRound SimulateRangedCombatRound(Unit attacker, Unit defender, int roundNumber);
    CombatRound SimulateMeleeCombatRound(Unit attacker, Unit defender, int roundNumber);
}
