using SimHammer.Core.Models.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Services.Interfaces
{
    public interface ICommonCombatRolls
    {
        public int RollForHits(int numAttacks, int weaponSkill);
        public int CalculateRollToWound(int weaponStrength, int defenderToughness);
        public int RollForWounds(int numHits, int rollNeeded);
        public int RollForSaves(int woundsInflicted, int weaponApValue, Unit defender);
        public int CalculateDamage(int woundsInflicted, int weaponDamage);


    }
}
