using SimHammer.Core.Models.Simulation;
using SimHammer.Core.Models.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Services.Interfaces
{
    public interface IRangedCombatService
    {
        public CombatRound SimulateRangedCombatRound(Unit attacker, Unit defender, int roundNumber);
    }
}
