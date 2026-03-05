using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Models.Units
{
    public class MeleeWeapon
    {
        // --- Properties ---
        public string Name { get; set; }
        public int Range { get; set; } // in inches
        public int WeaponSkill { get; set; } // Weapon Skill of the unit using the weapon
        public int Attacks { get; set; }
        public int Strength { get; set; }
        public int ArmorPiercing { get; set; } // Armor Penetration
        public int Damage { get; set; }
        public int Quantity { get; set; }

        // --- Constructors ---
        public MeleeWeapon()
        {
            
        }
        public MeleeWeapon(string name, int range, int attacks, int weaponSkill, int strength, int ap, int damage, int quantity)
        {
            Name = name;
            Range = range;
            Attacks = attacks;
            Strength = strength;
            ArmorPiercing = ap;
            Damage = damage;
            Quantity = quantity;
            WeaponSkill = weaponSkill;
        }
    }
}
