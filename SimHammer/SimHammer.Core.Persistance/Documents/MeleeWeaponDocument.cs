using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SimHammer.Core.Persistance.Documents
{
    public class MeleeWeaponDocument
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("range")]
        public int Range { get; set; } // in inches

        [JsonPropertyName("weaponSkill")]
        public int WeaponSkill { get; set; } // Weapon Skill of the unit using the weapon

        [JsonPropertyName("attacks")]
        public int Attacks { get; set; }

        [JsonPropertyName("strength")]
        public int Strength { get; set; }

        [JsonPropertyName("armorPiercing")]
        public int ArmorPiercing { get; set; } // Armor Penetration

        [JsonPropertyName("damage")]
        public int Damage { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion { get; set; } = 1;

        [JsonPropertyName("createdTime")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("lastUpdatedTIme")]
        public DateTime LastUsedTime { get; set; }
    }
}
