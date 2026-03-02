using SimHammer.Core.Persistance.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimHammer.Core.Persistance.Interfaces
{
    public interface IUnitRepository
    {
        Task<IEnumerable<UnitDocument>> GetAllUnitsAsync();
        Task<IEnumerable<UnitDocument>> GetUnitsByFactionAsync(string faction);
        Task<UnitDocument>  GetUnitByIdAsync(string id);
        Task<UnitDocument> AddUnitAsync(UnitDocument unit, string factionPartitionKey, CancellationToken ct);
        Task DeleteUnitByIdAsync(int id, string factionPartitionKey, CancellationToken ct);
        Task UpdateUnitAsync(UnitDocument unit, CancellationToken ct);
    }
}
