using SimHammer.Core.Persistance.Documents;
using SimHammer.Core.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace SimHammer.Core.Persistance.Repositories
{
    public class FactionRepository : IFactionRepository
    {

        private readonly ILogger<FactionRepository> _logger;
        private Container _container;

        // --- Constructor --- 
        public FactionRepository(CosmosClient cosmosClient, ILogger<FactionRepository> logger) 
        {
            _logger = logger;
            _container = cosmosClient.GetContainer("SimHammerDB", "Factions");
        }

        // --- Methods ---
        public async Task AddFactionAsync(string factionName, string? factionDescription)
        {
            var newFaction = new FactionDocument
            {
                Id = factionName,
                Name = factionName,
                Description = factionDescription,
                CreationDate = DateTime.UtcNow
            };

            try
            {
                var response = await _container.CreateItemAsync(newFaction, new PartitionKey(newFaction.Id));
                _logger.LogInformation($"Faction {newFaction.Name} has been created.");
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"The Faction - {newFaction.Name} - already exists!");
            }

            return;
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
