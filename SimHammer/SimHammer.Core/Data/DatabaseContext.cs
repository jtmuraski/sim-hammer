using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace SimHammer.Core.Data
{
    public class DatabaseContext : IDisposable
    {
        private readonly SqliteConnection _connection;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the DatabaseContext class with the specified database file path.
        /// </summary>
        /// <param name="dbPath">The path to the SQLite database file. If null, a default path will be used.</param>
        public DatabaseContext(string dbPath = null)
        {
            // If no path is specified, create a default path in the user's AppData folder
            if (string.IsNullOrEmpty(dbPath))
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SimHammer");
                
                // Ensure the directory exists
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }
                
                dbPath = Path.Combine(appDataPath, "simhammer.db");
            }

            // Create the connection string
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath,
                Mode = SqliteOpenMode.ReadWriteCreate
            };

            // Initialize the connection
            _connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            _connection.Open();
            
            // Initialize the database schema if needed
            InitializeDatabase();
        }

        /// <summary>
        /// Creates the database schema if it doesn't exist.
        /// </summary>
        private void InitializeDatabase()
        {
            using var command = _connection.CreateCommand();
            
            // Create Units table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Units (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Movement INTEGER,
                    Strength INTEGER,
                    Toughness INTEGER,
                    Wounds INTEGER,
                    Attacks INTEGER,
                    Leadership INTEGER,
                    Save INTEGER,
                    InvulnSave INTEGER,
                    ModelCount INTEGER,
                    WeaponSkill INTEGER,
                    BallisticSkill INTEGER
                );
                
                CREATE TABLE IF NOT EXISTS MeleeWeapons (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    UnitId INTEGER,
                    Strength INTEGER,
                    AP INTEGER,
                    Damage INTEGER,
                    FOREIGN KEY (UnitId) REFERENCES Units (Id) ON DELETE CASCADE
                );
                
                CREATE TABLE IF NOT EXISTS RangedWeapons (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    UnitId INTEGER,
                    Strength INTEGER,
                    AP INTEGER,
                    Damage INTEGER,
                    Range INTEGER,
                    FOREIGN KEY (UnitId) REFERENCES Units (Id) ON DELETE CASCADE
                );
                
                CREATE TABLE IF NOT EXISTS SimulationResults (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    AttackerId INTEGER,
                    DefenderId INTEGER,
                    RoundsSimulated INTEGER,
                    AttackerVictories INTEGER,
                    DefenderVictories INTEGER,
                    Draws INTEGER,
                    AverageAttackerWoundsRemaining REAL,
                    AverageDefenderWoundsRemaining REAL,
                    SimulationDate TEXT,
                    FOREIGN KEY (AttackerId) REFERENCES Units (Id),
                    FOREIGN KEY (DefenderId) REFERENCES Units (Id)
                );";
            
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the SQLite connection.
        /// </summary>
        public SqliteConnection Connection => _connection;

        /// <summary>
        /// Creates a new SQLite command.
        /// </summary>
        public SqliteCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }

        /// <summary>
        /// Executes a non-query SQL command.
        /// </summary>
        /// <param name="commandText">The SQL command text to execute.</param>
        /// <param name="parameters">Optional parameters for the SQL command.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = commandText;
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes a SQL query and returns a single value.
        /// </summary>
        /// <param name="commandText">The SQL command text to execute.</param>
        /// <param name="parameters">Optional parameters for the SQL command.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        public object ExecuteScalar(string commandText, Dictionary<string, object> parameters = null)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = commandText;
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            
            return command.ExecuteScalar();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
