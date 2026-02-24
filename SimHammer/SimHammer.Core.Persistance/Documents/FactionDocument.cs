using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SimHammer.Core.Persistance.Documents
{
    public class FactionDocument
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")] 
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("creationDate")]
        public DateTimeOffset CreationDate { get; set; }

        [JsonPropertyName("schemaVerion")]
        public int SchemaVersion { get; set; } = 1;
    }
}
