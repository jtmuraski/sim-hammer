using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using SimHammer.Core;
using SimHammer.Core.Models.Units;

namespace SimHammer.Core.Persistance.Documents
{
    public class UnitDocument
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("faction")]
        public string? Faction { get; set; }

        [JsonPropertyName("subFaction")]
        public string? Subfaction { get; set; }

        [JsonPropertyName("movement")]
        public int Movement { get; set; } // in inches

        [JsonPropertyName("toughness")]
        public int Toughness { get; set; }

        [JsonPropertyName("wounds")]
        public int Wounds { get; set; }

        [JsonPropertyName("leadership")]
        public int Leadership { get; set; }

        [JsonPropertyName("save")]
        public int Save { get; set; } // Armor Save

        [JsonPropertyName("hasInvulnSave")]
        public bool HasInvulnSave { get; set; }

        [JsonPropertyName("invulnSave")]
        public int InvulnSave { get; set; }

        [JsonPropertyName("modelCount")]
        public int ModelCount { get; set; } = 1; // Default to 1 model, can be adjusted for units with multiple models

        [JsonPropertyName("meleeWeapons")]
        public List<MeleeWeapon> MeleeWeapons { get; set; } = new List<MeleeWeapon>();

        [JsonPropertyName("rangedWeapons")]
        public List<RangedWeapon> RangedWeapons { get; set; } = new List<RangedWeapon>();

        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion { get; set; } = 1;

        [JsonPropertyName("createdTime")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("lastUpdatedTIme")]
        public DateTime LastUsedTime
        {
            get; set;
        }
}
