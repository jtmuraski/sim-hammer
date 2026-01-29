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
        public int RollForHits(int numAttacks, int skillLevel);
        public int CalculateRollToWound(int weaponStrength, int defenderToughness);
        public int RollForWOunds(int numHits, int rollNeeded);
        public int RollForSaves(int woundsInflicted, RangedWeapon weapon, Unit defender, out int damageDealt);
    }
}
