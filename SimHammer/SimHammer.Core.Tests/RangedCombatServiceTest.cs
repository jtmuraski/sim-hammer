using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SimHammer.Core;
using SimHammer.Core.Services.Simulation;
using Microsoft.Extensions.Logging.Abstractions;
using SimHammer.Core.Services.Interfaces;
using Moq;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Models.Simulation;

namespace SimHammer.Core.Tests
{
    public class RangedCombatServiceTest
    {
        #region Full SimulateRangedCombat Integration Testing
        [Fact]
        public void SimulateRangedCombatRoundTest_FullIntegration_1Weapon2Attacks()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(x => x.RollD6())
                // Hit Rolls (2 attacks - 1 hit, 1miss
                .Returns(4) // Hit
                .Returns(2) // Miss
                // Wound Rolls (1 hit - 1 wound)
                .Returns(5) // Wound roll success
                // Save Rolls (1 wound - 1 failed save)
                .Returns(2); // Save fails, damge occurs

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            var attacker = new Unit() { Name = "Attacker", Toughness = 5, Save = 4, InvulnSave = 4 };
            attacker.RangedWeapons.Add(new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 1));
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 4, InvulnSave = 4, Wounds = 1, ModelCount = 5 };

            // Act
            CombatRound roundResult = service.SimulateRangedCombatRound(attacker, defender, 1);

            // Assert

            // Check the combat round results
            Assert.Equal(1, roundResult.SimNumber);

            var weaponResult = roundResult.WeaponResults[0];
            Assert.Equal(1, weaponResult.DamageDealt);
            Assert.Equal(0, weaponResult.SavesMade);
            Assert.Equal(1, weaponResult.WoundsInflicted);
            Assert.Equal(1, weaponResult.Hits);
            Assert.Equal(1, weaponResult.Misses());
        }

        #endregion
    }
}
