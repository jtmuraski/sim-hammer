using SimHammer.Core.Persistance.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Persistance.Tools;

namespace SimHammer.Core.Persistance.Mappers
{
    public static class FactionMapper
    {
        public static FactionDocument ToDocument(this Models.Units.Faction faction)
        {
            string partitionKey = PersistanceTools.NormalizeFactionPartitionKey(faction.Name);

            return new FactionDocument
            {
                Id = faction.Id,
                Name = faction.Name,
                PartitionKey = partitionKey,
                Type = "faction",
                Description = faction.Description,
                CreationDate = DateTimeOffset.UtcNow
            };
        }

        public static Faction ToDomain(this FactionDocument document)
        {
            return new Faction()
            {
                Id = document.Id,
                Name = document.Name,
                Description = document.Description
            };
        }
    }
}
