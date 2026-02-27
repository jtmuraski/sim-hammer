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
        public Container _container;


        // --- Constructors ----
        public UnitRepository(CosmosClient cosmosClient, ILogger<UnitRepository> logger)
        {
            _logger = logger;
            _container = cosmosClient.GetContainer("SimHammerDB", "Units");
        }

        // --- Methods ---
        public async Task<UnitDocument> AddUnitAsync(UnitDocument unit, CancellationToken ct = default)
        {
            unit.CreatedTime = DateTimeOffset.UtcNow;
            unit.LastUpdatedTime = DateTimeOffset.UtcNow;
            unit.Id = Guid.NewGuid().ToString();

            var response = await _container.CreateItemAsync(unit, new PartitionKey(unit.Faction), cancellationToken: ct);
            _logger.LogInformation($"Unit for {unit.Faction} added: {unit.Name}");

            return response.Resource;

        }

        public async Task DeleteUnitByIdAsync(int id, string faction, CancellationToken ct = default)
        {
            try
            {
                var response = _container.DeleteItemAsync<UnitDocument>(id.ToString(), new PartitionKey(faction), cancellationToken: ct);
                return;
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Unit with id {id} not found for deletion.");
                return;
            }

        }
        public Task<IEnumerable<UnitDocument>> GetAllUnitsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UnitDocument> GetUnitByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnitDocument>> GetUnitsByFactionAsync(string faction)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUnitASync(UnitDocument unit, CancellationToken ct = default)
        {
            try
            {
                unit.LastUpdatedTime = DateTimeOffset.UtcNow;
                var response = _container.ReplaceItemAsync(unit, unit.Id, new PartitionKey(unit.Faction), cancellationToken: ct);
                return response;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Unit with id {unit.Id} not found for update.");
                return Task.CompletedTask;
            }
    }
}
