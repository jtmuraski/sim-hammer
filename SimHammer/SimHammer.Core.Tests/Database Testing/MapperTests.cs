using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Persistance.Documents;
using SimHammer.Core.Persistance.Mappers;

namespace SimHammer.Core.Tests
{
    public class MapperTests
    {
        [Fact]
        public void MapFactionToDocumentTest()
        {
            // Arrange
            Faction newFaction = new Faction("Space Marines", "Elite warriors of the Imperium");

            // Act
            FactionDocument factionDocument = FactionMapper.ToDocument(newFaction);
            // Assert
            Assert.Equal(factionDocument.Id, newFaction.Name.ToLower().Replace(' ', '-'));
            Assert.Equal(factionDocument.PartitionKey, "faction-space-marines");
            Assert.Equal(factionDocument.Type, "faction");
            Assert.Equal(factionDocument.SchemaVersion, 1);
        }

        [Fact]
        public void MaptFactionDocumentToDomainTest()
        {
            // Arrange
            FactionDocument document = new FactionDocument
            {
                Id = "space-marines",
                Type = "faction",
                PartitionKey = "faction-space-marines",
                Name = "Space Marines",
                Description = "Elite warriors of the Imperium"
            };
            // Act
            Faction faction = FactionMapper.ToDomain(document);

            // Assert
            Assert.Equal(faction.Id, document.Id);
            Assert.Equal(faction.Name, document.Name);
            Assert.Equal(faction.Description, document.Description);
        }

        [Fact]
        public void MapUnitDomainToUnitDocumentTest()
        {
            // Arrange
            Unit unit = new Unit()
            {
                Id = "tactical-marine",
                Name = "Tactical Marine",
                Faction = "Space Marines",
                Subfaction = "Ultramarines",
                Movement = 6,
                Toughness = 4,
                Wounds = 1,
                Leadership = 7,
                Save = 3,
                HasInvulnSave = false,
                InvulnSave = 0,
                ModelCount = 5,
                MeleeWeapons = new List<MeleeWeapon>
                {
                    new MeleeWeapon()
                    {
                        Name = "Chainsword",
                        Range = 1,
                        Attacks = 1,
                        Strength = 4,
                        ArmorPiercing = -1,
                        Damage = 1
                    }
                },
                RangedWeapons = new List<RangedWeapon>
                {
                    new RangedWeapon()
                    {
                        Name = "Boltgun",
                        Range = 24,
                        Attacks = 2,
                        Strength = 4,
                        ArmourPiercing = -1,
                        Damage = 1
                    }
                }
            };

            // Act
            UnitDocument document = UnitMapper.ToDocument(unit);

            // Assert
            Assert.Equal(document.Type, "unit");
            Assert.Equal(document.PartitionKey, "faction-space-marines");
            Assert.Equal(document.SchemaVersion, 1);
        }

        [Fact]
        public void UnitDocumentToUnitDomainTest()
        {
            // Arrange
            UnitDocument document = new UnitDocument()
            {
                Id = "tactical-marine",
                Name = "Tactical Marine",
                Faction = "Space Marines",
                Subfaction = "Ultramarines",
                Type = "faction",
                PartitionKey = "faction-space-marines",
                Movement = 6,
                Toughness = 4,
                Wounds = 1,
                Leadership = 7,
                Save = 3,
                HasInvulnSave = false,
                InvulnSave = 0,
                ModelCount = 5,
                MeleeWeapons = new List<MeleeWeapon>
                {
                    new MeleeWeapon()
                    {
                        Name = "Chainsword",
                        Range = 1,
                        Attacks = 1,
                        Strength = 4,
                        ArmorPiercing = -1,
                        Damage = 1
                    }
                },
                RangedWeapons = new List<RangedWeapon>
                {
                    new RangedWeapon()
                    {
                        Name = "Boltgun",
                        Range = 24,
                        Attacks = 2,
                        Strength = 4,
                        ArmourPiercing = -1,
                        Damage = 1
                    }
                }
            };

            // Act
            Unit unit = UnitMapper.ToDomain(document);

            // Assert
            Assert.Equal(unit.Id, document.Id);
            Assert.Equal(unit.Name, document.Name);
            Assert.Equal(unit.Faction, document.Faction);
            Assert.Equal(unit.Subfaction, document.Subfaction);
            Assert.Equal(unit.Movement, document.Movement);
            Assert.Equal(unit.Toughness, document.Toughness);
            Assert.Equal(unit.Wounds, document.Wounds);
            Assert.Equal(unit.Leadership, document.Leadership);
            Assert.Equal(unit.Save, document.Save);
            Assert.Equal(unit.HasInvulnSave, document.HasInvulnSave);
            Assert.Equal(unit.InvulnSave, document.InvulnSave);
            Assert.Equal(unit.ModelCount, document.ModelCount);
            Assert.Equal(unit.MeleeWeapons.Count, 1);
            Assert.Equal(unit.RangedWeapons.Count, 1);
        }
    }
}
