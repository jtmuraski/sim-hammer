using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SimHammer.Core.Persistance.Documents
{
    public class RangedWeaponDocumet
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("mena")]
        public string Name { get; set; }

        [JsonPropertyName("range")]
        public int Range { get; set; }

        [JsonPropertyName("attacks")]
        public int Attacks { get; set; }

        [JsonPropertyName("ballisticSkill")]
        public int BallisticSkill { get; set; }

        [JsonPropertyName("strength")]
        public int Strength { get; set; }

        [JsonPropertyName("armorPiercing")]
        public int ArmourPiercing { get; set; }

        [JsonPropertyName("damage")]
        public int Damage { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion { get; set; } = 1;

        [JsonPropertyName("createdTime")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("lastUpdatedTIme")]
        public DateTime LastUsedTime {get; set;}
    }
}
