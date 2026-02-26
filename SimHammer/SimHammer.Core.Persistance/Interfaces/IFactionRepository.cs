using SimHammer.Core.Persistance.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimHammer.Core.Persistance.Interfaces
{
    public interface IFactionRepository
    {
        Task<IEnumerable<FactionDocument>> GetAllFactionsAsync();
         Task AddFactionAsync(string factionName, string? factionDescription);
         Task UpdateFactionAsync(FactionDocument faction);
         Task DeleteFactionAsync(string id);
    }
}
