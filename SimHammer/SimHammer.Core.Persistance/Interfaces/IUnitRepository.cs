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
        Task<UnitDocument> AddUnitAsync(UnitDocument unit, CancellationToken ct);
        Task DeleteUnitByIdAsync(int id, string faction, CancellationToken ct);
        Task<UnitDocument> UpdateUnitASync(UnitDocument unit, CancellationToken ct);
    }
}
