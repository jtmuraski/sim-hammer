using SimHammer.Core.Persistance.Documents;
using SimHammer.Core.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;

namespace SimHammer.Core.Persistance.Repositories
{
    public class FactionRepository : IFactionRepository
    {
        public Task AddFactionAsync(FactionDocument faction)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFactionAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FactionDocument>> GetAllFactionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateFactionAsync(FactionDocument faction)
        {
            throw new NotImplementedException();
        }
    }
}
