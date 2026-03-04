using SimHammer.Core.Persistance.Documents;
using SimHammer.Core.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Linq;
using SimHammer.Core.Persistance.Tools;

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
            _container = cosmosClient.GetContainer("SimHammerDB", "faction-units");
        }

        // --- Methods ---
        public async Task AddFactionAsync(FactionDocument faction, CancellationToken ct = default)
        {
            var newFaction = new FactionDocument
            {
                Id = faction.Name.ToLower().Replace(' ', '-'),
                Name = faction.Name,
                Description = faction.Description,
                CreationDate = DateTime.UtcNow
            };
            newFaction.PartitionKey = PersistanceTools.NormalizeFactionPartitionKey(faction.Name);

            try
            {   
                var response = await _container.CreateItemAsync(newFaction, new PartitionKey(newFaction.PartitionKey));
                _logger.LogInformation($"Faction {newFaction.Name} has been created.");
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw new Exception($"The Faction - {newFaction.Name} - already exists!");
            }

            return;
        }

        public async Task DeleteFactionAsync(string id, string factionPartitionKey, CancellationToken ct = default)
        {
            _logger.LogInformation($"Deleting faction {id}");
           var response = await _container.DeleteItemAsync<FactionDocument>(id, new PartitionKey(factionPartitionKey));

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
