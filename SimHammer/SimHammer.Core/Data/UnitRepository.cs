using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using SimHammer.Core.Models.Units;

namespace SimHammer.Core.Data
{
    public class UnitRepository
    {
        private readonly DatabaseContext _dbContext;

        public UnitRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Saves a unit to the database. Creates a new record if the unit doesn't exist,
        /// or updates an existing record if it does.
        /// </summary>
        /// <param name="unit">The unit to save.</param>
        /// <returns>The ID of the saved unit.</returns>
        public long SaveUnit(Unit unit)
        {
            // Check if unit exists by name (you might want a more robust way to identify units)
            var existingId = GetUnitIdByName(unit.Name);
            
            if (existingId.HasValue)
            {
                // Update existing unit
                UpdateUnit(existingId.Value, unit);
                return existingId.Value;
            }
            else
            {
                // Insert new unit
                return InsertUnit(unit);
            }
        }

        /// <summary>
        /// Gets a unit by its ID, including its weapons.
        /// </summary>
        /// <param name="id">The ID of the unit to retrieve.</param>
        /// <returns>The unit, or null if not found.</returns>
        public Unit GetUnitById(long id)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = "SELECT * FROM Units WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var unit = MapUnitFromReader(reader);
                
                // Load weapons
                unit.MeleeWeapons = GetMeleeWeaponsForUnit(id);
                unit.RangedWeapons = GetRangedWeaponsForUnit(id);
                
                return unit;
            }
            
            return null;
        }

        /// <summary>
        /// Gets all units from the database.
        /// </summary>
        /// <returns>A list of units.</returns>
        public List<Unit> GetAllUnits()
        {
            var units = new List<Unit>();
            
            using var command = _dbContext.CreateCommand();
            command.CommandText = "SELECT * FROM Units";
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var unit = MapUnitFromReader(reader);
                
                // Load weapons
                long unitId = reader.GetInt64(reader.GetOrdinal("Id"));
                unit.MeleeWeapons = GetMeleeWeaponsForUnit(unitId);
                unit.RangedWeapons = GetRangedWeaponsForUnit(unitId);
                
                units.Add(unit);
            }
            
            return units;
        }

        /// <summary>
        /// Deletes a unit from the database.
        /// </summary>
        /// <param name="id">The ID of the unit to delete.</param>
        /// <returns>True if the unit was deleted, false otherwise.</returns>
        public bool DeleteUnit(long id)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = "DELETE FROM Units WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            return command.ExecuteNonQuery() > 0;
        }

        #region Private Helper Methods

        private long? GetUnitIdByName(string name)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = "SELECT Id FROM Units WHERE Name = @Name";
            command.Parameters.AddWithValue("@Name", name);
            
            var result = command.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt64(result);
            }
            
            return null;
        }

        private long InsertUnit(Unit unit)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = @"
                INSERT INTO Units (
                    Name, Movement, Strength, Toughness, Wounds, 
                    Attacks, Leadership, Save, InvulnSave, ModelCount,
                    WeaponSkill, BallisticSkill
                ) VALUES (
                    @Name, @Movement, @Strength, @Toughness, @Wounds, 
                    @Attacks, @Leadership, @Save, @InvulnSave, @ModelCount,
                    @WeaponSkill, @BallisticSkill
                );
                SELECT last_insert_rowid();";
            
            SetUnitParameters(command, unit);
            
            long unitId = Convert.ToInt64(command.ExecuteScalar());
            
            // Save weapons
            SaveMeleeWeapons(unitId, unit.MeleeWeapons);
            SaveRangedWeapons(unitId, unit.RangedWeapons);
            
            return unitId;
        }

        private void UpdateUnit(long id, Unit unit)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = @"
                UPDATE Units SET 
                    Name = @Name, 
                    Movement = @Movement, 
                    Strength = @Strength, 
                    Toughness = @Toughness, 
                    Wounds = @Wounds, 
                    Attacks = @Attacks, 
                    Leadership = @Leadership, 
                    Save = @Save, 
                    InvulnSave = @InvulnSave, 
                    ModelCount = @ModelCount,
                    WeaponSkill = @WeaponSkill,
                    BallisticSkill = @BallisticSkill
                WHERE Id = @Id";
            
            command.Parameters.AddWithValue("@Id", id);
            SetUnitParameters(command, unit);
            
            command.ExecuteNonQuery();
            
            // Update weapons - first delete existing ones
            DeleteMeleeWeaponsForUnit(id);
            DeleteRangedWeaponsForUnit(id);
            
            // Then save new ones
            SaveMeleeWeapons(id, unit.MeleeWeapons);
            SaveRangedWeapons(id, unit.RangedWeapons);
        }

        private void SetUnitParameters(SqliteCommand command, Unit unit)
        {
            command.Parameters.AddWithValue("@Name", unit.Name);
            command.Parameters.AddWithValue("@Movement", unit.Movement);
            command.Parameters.AddWithValue("@Strength", unit.Strength);
            command.Parameters.AddWithValue("@Toughness", unit.Toughness);
            command.Parameters.AddWithValue("@Wounds", unit.Wounds);
            command.Parameters.AddWithValue("@Attacks", unit.Attacks);
            command.Parameters.AddWithValue("@Leadership", unit.Leadership);
            command.Parameters.AddWithValue("@Save", unit.Save);
            command.Parameters.AddWithValue("@InvulnSave", unit.InvulnSave);
            command.Parameters.AddWithValue("@ModelCount", unit.ModelCount);
            
            // Note: These properties are expected to be available in the Unit class
            // If they're not, you'll need to modify the Unit class or this code
            var weaponSkill = 0;
            var ballisticSkill = 0;
            
            // Use reflection to check if these properties exist
            var type = typeof(Unit);
            var weaponSkillProp = type.GetProperty("WeaponSkill");
            var ballisticSkillProp = type.GetProperty("BallisticSkill");
            
            if (weaponSkillProp != null)
                weaponSkill = (int)weaponSkillProp.GetValue(unit);
            
            if (ballisticSkillProp != null)
                ballisticSkill = (int)ballisticSkillProp.GetValue(unit);
            
            command.Parameters.AddWithValue("@WeaponSkill", weaponSkill);
            command.Parameters.AddWithValue("@BallisticSkill", ballisticSkill);
        }

        private Unit MapUnitFromReader(SqliteDataReader reader)
        {
            // Retrieve the WeaponSkill and BallisticSkill values
            int weaponSkill = 0;
            int ballisticSkill = 0;
            
            if (!reader.IsDBNull(reader.GetOrdinal("WeaponSkill")))
                weaponSkill = reader.GetInt32(reader.GetOrdinal("WeaponSkill"));
            
            if (!reader.IsDBNull(reader.GetOrdinal("BallisticSkill")))
                ballisticSkill = reader.GetInt32(reader.GetOrdinal("BallisticSkill"));
            
            // Create a new Unit with the retrieved values
            var unit = new Unit(
                name: reader.GetString(reader.GetOrdinal("Name")),
                movement: reader.GetInt32(reader.GetOrdinal("Movement")),
                weaponSkill: weaponSkill,
                ballisticSkill: ballisticSkill,
                strength: reader.GetInt32(reader.GetOrdinal("Strength")),
                toughness: reader.GetInt32(reader.GetOrdinal("Toughness")),
                wounds: reader.GetInt32(reader.GetOrdinal("Wounds")),
                attacks: reader.GetInt32(reader.GetOrdinal("Attacks")),
                leadership: reader.GetInt32(reader.GetOrdinal("Leadership")),
                save: reader.GetInt32(reader.GetOrdinal("Save")),
                invuln: reader.GetInt32(reader.GetOrdinal("InvulnSave")),
                modelCount: reader.GetInt32(reader.GetOrdinal("ModelCount"))
            );

            return unit;
        }

        #region Melee Weapon Methods        private void SaveMeleeWeapons(long unitId, List<MeleeWeapon> weapons)
        {
            if (weapons == null || weapons.Count == 0)
                return;
                
            using var command = _dbContext.CreateCommand();
            
            foreach (var weapon in weapons)
            {
                command.CommandText = @"
                    INSERT INTO MeleeWeapons (Name, UnitId, Strength, AP, Damage, Range) 
                    VALUES (@Name, @UnitId, @Strength, @AP, @Damage, @Range)";
                
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", weapon.Name);
                command.Parameters.AddWithValue("@UnitId", unitId);
                command.Parameters.AddWithValue("@Strength", weapon.Strength);
                command.Parameters.AddWithValue("@AP", weapon.ArmorPiercing);
                command.Parameters.AddWithValue("@Damage", weapon.Damage);
                command.Parameters.AddWithValue("@Range", weapon.Range);
                
                command.ExecuteNonQuery();
            }
        }

        private void DeleteMeleeWeaponsForUnit(long unitId)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = "DELETE FROM MeleeWeapons WHERE UnitId = @UnitId";
            command.Parameters.AddWithValue("@UnitId", unitId);
            
            command.ExecuteNonQuery();
        }

        private List<MeleeWeapon> GetMeleeWeaponsForUnit(long unitId)
        {
            var weapons = new List<MeleeWeapon>();
            
            using var command = _dbContext.CreateCommand();
            command.CommandText = "SELECT * FROM MeleeWeapons WHERE UnitId = @UnitId";
            command.Parameters.AddWithValue("@UnitId", unitId);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                weapons.Add(new MeleeWeapon
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Strength = reader.GetInt32(reader.GetOrdinal("Strength")),
                    AP = reader.GetInt32(reader.GetOrdinal("AP")),
                    Damage = reader.GetInt32(reader.GetOrdinal("Damage"))
                });
            }
            
            return weapons;
        }

        #endregion

        #region Ranged Weapon Methods

        private void SaveRangedWeapons(long unitId, List<RangedWeapon> weapons)
        {
            if (weapons == null || weapons.Count == 0)
                return;
                
            using var command = _dbContext.CreateCommand();
            
            foreach (var weapon in weapons)
            {
                command.CommandText = @"
                    INSERT INTO RangedWeapons (Name, UnitId, Strength, AP, Damage, Range) 
                    VALUES (@Name, @UnitId, @Strength, @AP, @Damage, @Range)";
                
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", weapon.Name);
                command.Parameters.AddWithValue("@UnitId", unitId);
                command.Parameters.AddWithValue("@Strength", weapon.Strength);
                command.Parameters.AddWithValue("@AP", weapon.AP);
                command.Parameters.AddWithValue("@Damage", weapon.Damage);
                command.Parameters.AddWithValue("@Range", weapon.Range);
                
                command.ExecuteNonQuery();
            }
        }

        private void DeleteRangedWeaponsForUnit(long unitId)
        {
            using var command = _dbContext.CreateCommand();
            command.CommandText = "DELETE FROM RangedWeapons WHERE UnitId = @UnitId";
            command.Parameters.AddWithValue("@UnitId", unitId);
            
            command.ExecuteNonQuery();
        }

        private List<RangedWeapon> GetRangedWeaponsForUnit(long unitId)
        {
            var weapons = new List<RangedWeapon>();
            
            using var command = _dbContext.CreateCommand();
            command.CommandText = "SELECT * FROM RangedWeapons WHERE UnitId = @UnitId";
            command.Parameters.AddWithValue("@UnitId", unitId);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                weapons.Add(new RangedWeapon
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Strength = reader.GetInt32(reader.GetOrdinal("Strength")),
                    AP = reader.GetInt32(reader.GetOrdinal("AP")),
                    Damage = reader.GetInt32(reader.GetOrdinal("Damage")),
                    Range = reader.GetInt32(reader.GetOrdinal("Range"))
                });
            }
            
            return weapons;
        }

        #endregion

        #endregion
    }
}
