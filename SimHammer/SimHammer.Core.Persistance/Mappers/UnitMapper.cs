using SimHammer.Core.Persistance.Documents;
using SimHammer.Core.Models.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimHammer.Core.Persistance.Mappers
{
    public static class UnitMapper
    {
        public static UnitDocument ToDocument(this Models.Units.Unit unit)
        {
            string partitionKey = $"faction-{unit.Faction}";

            return new UnitDocument
            {
                Id = unit.Id,
                Name = unit.Name,
                Faction = unit.Faction,
                PartitionKey = partitionKey,
                Type = "unit",
                Movement = unit.Movement,
                Toughness = unit.Toughness,
                Wounds = unit.Wounds,
                Leadership = unit.Leadership,
                Save = unit.Save,
                HasInvulnSave = unit.HasInvulnSave,
                ModelCount = unit.ModelCount,
                MeleeWeapons = unit.MeleeWeapons,
                RangedWeapons = unit.RangedWeapons,
                SchemaVersion = 1
            };
        }

        public static Unit ToDomain(UnitDocument unit)
        {
            return new Unit
            {
                Id = unit.Id,
                Name = unit.Name,
                Faction = unit.Faction,
                Movement= unit.Movement,
                Toughness = unit.Toughness,
                Wounds = unit.Wounds,
                Leadership = unit.Leadership,
                Save = unit.Save,
                HasInvulnSave = unit.HasInvulnSave,
                InvulnSave = unit.InvulnSave,
                ModelCount  = unit.ModelCount,
                MeleeWeapons = unit.MeleeWeapons,
                RangedWeapons = unit.RangedWeapons
            }
        }

    }
}
