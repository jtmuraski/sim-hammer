using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Models.Units
{
    public class MeleeWeapon
    {
        public string Name { get; set; }
        public int Range { get; set; } // in inches
        public int WeaponSkill { get; set; } // Weapon Skill of the unit using the weapon
        public int Attacks { get; set; }
        public int Strength { get; set; }
        public int ArmorPiercing { get; set; } // Armor Penetration
        public int Damage { get; set; }
        public MeleeWeapon(string name, int range, int attacks, int strength, int ap, int damage)
        {
            Name = name;
            Range = range;
            Attacks = attacks;
            Strength = strength;
            ArmorPiercing = ap;
            Damage = damage;
        }
    }
}
