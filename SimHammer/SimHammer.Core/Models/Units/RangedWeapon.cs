using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Models.Units
{
    public class RangedWeapon
    {
        public string Name { get; set; }
        public int Range { get; set; }
        public int BallisticSkill { get; set; }
        public int Attacks { get; set; }
        public int Strength { get; set; }
        public int ArmourPiercing { get; set; }
        public int Damage { get; set; }
        public RangedWeapon(string name, int range, int attacks, int strength, int armourPiercing, int damage)
        {
            Name = name;
            Range = range;
            Attacks = attacks;
            Strength = strength;
            ArmourPiercing = armourPiercing;
            Damage = damage;
        }
    }
}
