using SimHammer.Core.Persistance.Documents;
using SimHammer.Core.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Linq;

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

        public async Task DeleteFactionAsync(string id)
        {
            _logger.LogInformation($"Deleting faction {id}";
           var response = _container.DeleteItemAsync<FactionDocument>(id, new PartitionKey(id));

            return;
        }

        public async Task<IEnumerable<FactionDocument>> GetAllFactionsAsync()
        {
            _logger.LogInformation("Getting all factions from CosmosDB");
            var results = new List<FactionDocument>();
            var query = _container.GetItemLinqQueryable<FactionDocument>().ToFeedIterator();

            while(query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
            
        }
    }
}
