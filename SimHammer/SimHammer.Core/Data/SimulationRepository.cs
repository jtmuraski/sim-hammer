using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SimHammer.Core.Models.Simulation;
using SimHammer.Core.Models.Units;

namespace SimHammer.Core.Data
{
    public class SimulationRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly UnitRepository _unitRepository;

        public SimulationRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _unitRepository = new UnitRepository(dbContext);
        }

        /// <summary>
        /// Saves a simulation result to the database.
        /// </summary>
        /// <param name="result">The simulation result to save.</param>
        /// <returns>The ID of the saved simulation result.</returns>
        public long SaveSimulationResult(SimulationResult result)
        {
            // Make sure units are saved first
            long attackerId = _unitRepository.SaveUnit(result.Attacker);
            long defenderId = _unitRepository.SaveUnit(result.Defender);
            
            using var command = _dbContext.CreateCommand();
            command.CommandText = @"
                INSERT INTO SimulationResults (
                    AttackerId, DefenderId, RoundsSimulated, 
                    AttackerVictories, DefenderVictories, Draws,
                    AverageAttackerWoundsRemaining, AverageDefenderWoundsRemaining,
                    SimulationDate
                ) VALUES (
                    @AttackerId, @DefenderId, @RoundsSimulated, 
                    @AttackerVictories, @DefenderVictories, @Draws,
                    @AverageAttackerWoundsRemaining, @AverageDefenderWoundsRemaining,
                    @SimulationDate
                );
                SELECT last_insert_rowid();";
            
            command.Parameters.AddWithValue("@AttackerId", attackerId);
            command.Parameters.AddWithValue("@DefenderId", defenderId);
            command.Parameters.AddWithValue("@RoundsSimulated", result.RoundsSimulated);
            command.Parameters.AddWithValue("@AttackerVictories", result.AttackerVictories);
            command.Parameters.AddWithValue("@DefenderVictories", result.DefenderVictories);
            command.Parameters.AddWithValue("@Draws", result.Draws);
            command.Parameters.AddWithValue("@AverageAttackerWoundsRemaining", result.AverageAttackerWoundsRemaining);
            command.Parameters.AddWithValue("@AverageDefenderWoundsRemaining", result.AverageDefenderWoundsRemaining);
            command.Parameters.AddWithValue("@SimulationDate", DateTime.UtcNow.ToString("o")); // ISO 8601 format
            
            return Convert.ToInt64(command.ExecuteScalar());
        }

        /// <summary>
        /// Gets a simulation result by its ID.
        /// </summary>
        /// <param name="id">The ID of the simulation result to retrieve.</param>
        /// <returns>The simulation result, or null if not found.</returns>
        public SimulationResult GetSimulationResultById(long id)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = "SELECT * FROM SimulationResults WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapSimulationResultFromReader(reader);
            }
            
            return null;
        }

        /// <summary>
        /// Gets all simulation results from the database.
        /// </summary>
        /// <returns>A list of simulation results.</returns>
        public List<SimulationResult> GetAllSimulationResults()
        {
            var results = new List<SimulationResult>();
            
            using var command = _dbContext.CreateCommand();
            command.CommandText = "SELECT * FROM SimulationResults ORDER BY SimulationDate DESC";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(MapSimulationResultFromReader(reader));
            }
            
            return results;
        }

        /// <summary>
        /// Gets simulation results for a specific unit (either as attacker or defender).
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>A list of simulation results involving the specified unit.</returns>
        public List<SimulationResult> GetSimulationResultsForUnit(long unitId)
        {
            var results = new List<SimulationResult>();
            
            using var command = _dbContext.CreateCommand();
            command.CommandText = @"
                SELECT * FROM SimulationResults 
                WHERE AttackerId = @UnitId OR DefenderId = @UnitId
                ORDER BY SimulationDate DESC";
            
            command.Parameters.AddWithValue("@UnitId", unitId);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(MapSimulationResultFromReader(reader));
            }
            
            return results;
        }

        /// <summary>
        /// Deletes a simulation result from the database.
        /// </summary>
        /// <param name="id">The ID of the simulation result to delete.</param>
        /// <returns>True if the simulation result was deleted, false otherwise.</returns>
        public bool DeleteSimulationResult(long id)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = "DELETE FROM SimulationResults WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            return command.ExecuteNonQuery() > 0;
        }

        private SimulationResult MapSimulationResultFromReader(SqliteDataReader reader)
        {
            long attackerId = reader.GetInt64(reader.GetOrdinal("AttackerId"));
            long defenderId = reader.GetInt64(reader.GetOrdinal("DefenderId"));
            
            // Load the units from the database
            Unit attacker = _unitRepository.GetUnitById(attackerId);
            Unit defender = _unitRepository.GetUnitById(defenderId);
            
            if (attacker == null || defender == null)
            {
                throw new InvalidOperationException($"Could not find attacker (ID {attackerId}) or defender (ID {defenderId}) for simulation result.");
            }
            
            return new SimulationResult
            {
                Attacker = attacker,
                Defender = defender,
                RoundsSimulated = reader.GetInt32(reader.GetOrdinal("RoundsSimulated")),
                AttackerVictories = reader.GetInt32(reader.GetOrdinal("AttackerVictories")),
                DefenderVictories = reader.GetInt32(reader.GetOrdinal("DefenderVictories")),
                Draws = reader.GetInt32(reader.GetOrdinal("Draws")),
                AverageAttackerWoundsRemaining = reader.GetDouble(reader.GetOrdinal("AverageAttackerWoundsRemaining")),
                AverageDefenderWoundsRemaining = reader.GetDouble(reader.GetOrdinal("AverageDefenderWoundsRemaining"))
            };
        }
    }
}
