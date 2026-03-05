using SimHammer.Core.Persistance.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimHammer.Core.Persistance.Interfaces
{
    public interface IFactionRepository
    {
        Task<IEnumerable<FactionDocument>> GetAllFactionsAsync();
         Task AddFactionAsync(FactionDocument faction, CancellationToken ct);
         Task DeleteFactionAsync(string id, string factionPartitionKey, CancellationToken ct);
    }
}
