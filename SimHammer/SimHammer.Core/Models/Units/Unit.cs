using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimHammer.Core.Models.Units
{
    public class Unit
    {
        // ---Properties---
        public string Name { get; set; }
        public int Movement { get; set; } // in inches
        public int Strength { get; set; }
        public int Toughness { get; set; }
        public int Wounds { get; set; }
        public int Leadership { get; set; }
        public int Save { get; set; } // Armor Save
        public int InvulnSave { get; set; }
        public int ModelCount { get; set; } = 1; // Default to 1 model, can be adjusted for units with multiple models
        public List<MeleeWeapon> MeleeWeapons { get; set; } = new List<MeleeWeapon>();
        public List<RangedWeapon> RangedWeapons { get; set; } = new List<RangedWeapon>();

        // ---Constructors---
        public Unit() 
        { 
        }
        public Unit(string name, int movement, int weaponSkill, int ballisticSkill, int strength, int toughness, int wounds, int leadership, int save, int invuln, int modelCount)
        {
            Name = name;
            Movement = movement;
            Strength = strength;
            Toughness = toughness;
            Wounds = wounds;
            Leadership = leadership;
            Save = save;
            InvulnSave = invuln;
            ModelCount = modelCount;
        }
    }
}
