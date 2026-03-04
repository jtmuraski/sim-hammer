using SimHammer.Core.Persistance.Documents;
using SimHammer.Core.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace SimHammer.Core.Persistance.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        // --- Properties ---
        private readonly ILogger<UnitRepository> _logger;
        private readonly Container _container;


        // --- Constructors ----
        public UnitRepository(CosmosClient cosmosClient, ILogger<UnitRepository> logger)
        {
            _logger = logger;
            _container = cosmosClient.GetContainer("SimHammerDB", "faction-units");
        }

        // --- Methods ---
        public async Task<UnitDocument> AddUnitAsync(UnitDocument unit, string factionPartitionKey, CancellationToken ct = default)
        {
            unit.CreatedTime = DateTimeOffset.UtcNow;
            unit.LastUpdatedTime = DateTimeOffset.UtcNow;
            unit.Id = Guid.NewGuid().ToString();
            unit.PartitionKey = factionPartitionKey;
            unit.Type = "unit";

            var response = await _container.CreateItemAsync(unit, new PartitionKey(unit.PartitionKey), cancellationToken: ct);
            _logger.LogInformation($"Unit for {unit.Faction} added: {unit.Name}");

            return response.Resource;

        }

        public async Task DeleteUnitByIdAsync(string id, string factionPartitionKey, CancellationToken ct = default)
        {
            try
            {
                var response = await _container.DeleteItemAsync<UnitDocument>(id.ToString(), new PartitionKey(factionPartitionKey), cancellationToken: ct);
                return;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Unit with id {id} not found for deletion.");
                throw new KeyNotFoundException($"Unit with id {id} not found for deletion.");
            }

        }
        public async Task<IEnumerable<UnitDocument>> GetAllUnitsAsync()
        {
            var query = new QueryDefinition("SELECT * FROM container WHERE type = @type")
                                           .WithParameter("@type", "unit");

            List<UnitDocument> results = new List<UnitDocument>();

            var pageFeeder = _container.GetItemQueryIterator<UnitDocument>(query);

            while (pageFeeder.HasMoreResults)
            {
                var page = await pageFeeder.ReadNextAsync();
                results.AddRange(page);
            }

            return results;

        }

        public async Task<UnitDocument> GetUnitByIdAsync(string id, string factionPartitionKey, CancellationToken ct = default)
        {
            var response = await _container.ReadItemAsync<UnitDocument>(id, new PartitionKey(factionPartitionKey));

            return response.Resource;
        }

        public async Task<IEnumerable<UnitDocument>> GetUnitsByFactionAsync(string factionId)
        {
            string partitionKey = $"faction-{factionId}";

            var query = new QueryDefinition("SELECT * FROM container WHERE container.partitionKey = @partitionKey AND container.type = @type")
                                           .WithParameter("@partitionKey", partitionKey)
                                           .WithParameter("@type", "unit");

            List<UnitDocument> results = new List<UnitDocument>();

            var pageFeeder = _container.GetItemQueryIterator<UnitDocument>(query,
                                                                           requestOptions: new QueryRequestOptions
                                                                           { PartitionKey = new PartitionKey(partitionKey) });


            while (pageFeeder.HasMoreResults)
            {
                var page = await pageFeeder.ReadNextAsync();
                results.AddRange(page);
            }

            return results;
        }

        public async Task UpdateUnitAsync(UnitDocument unit, CancellationToken ct = default)
        {
            try
            {
                unit.LastUpdatedTime = DateTimeOffset.UtcNow;
                var response = await _container.ReplaceItemAsync(unit, unit.Id, new PartitionKey(unit.PartitionKey), cancellationToken: ct);
                return;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Unit with id {unit.Id} not found for update.");
                throw new KeyNotFoundException($"Unit with id {unit.Id} not found for update.");
            }
        }
    }
}
