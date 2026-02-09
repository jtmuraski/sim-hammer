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

namespace SimHammer.Core.Tests
{
    public class RangedCombatServiceTest
    {
        #region Roll For Hits Tests

        // Test the auto hit/miss
        [Fact]
        public void RollForHitTest_1Attack_AutoHitOn6()
        {
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(6);

            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);


            // Act
            int hits = service.RollForHits(1, 4);

            // Assert
            Assert.Equal(1, hits);
        }

        [Fact]
        public void RollForHitTest_1Attack_AutoMissOn1()
        {
            //Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(1);

            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int hits = service.RollForHits(1, 4);

            // Assert
            Assert.Equal(0, hits);
        }

        // Test for hits
        [Fact]
        public void RollForHitsTest_1Attack_HitOnRollEqualToSkill()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(4);

            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int hits = service.RollForHits(1, 4);

            // Assert
            Assert.Equal(1, hits);

        }

        [Fact]
        public void RollForHitsTest_1Attack_HitOnGreaterThanSkill()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(5);

            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int hits = service.RollForHits(1, 3);

            // Assert
            Assert.Equal(1, hits);
        }

        // Test for Misses
        [Fact]
        public void RollForHitsTest_1Attack_MissOnLessThanSkill()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(2);

            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int hits = service.RollForHits(1, 5);

            // Arrange
            Assert.Equal(0, hits);
        }

        // Test condictions with multiple attacks
        [Theory]
        [InlineData(1,0)]
        [InlineData(2,0)]
        [InlineData(3,1)]
        [InlineData(4,1)]
        [InlineData(5,1)]
        [InlineData(6,1)]
        public void RollForHitsTest_6Attacks_3Hits(int diceRoll, int expectedHits)
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.SetupSequence(x => x.RollD6()).Returns(diceRoll);

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService (logger, mockRoller.Object);

            // Act
            int hits = service.RollForHits(1, 3);

            // Assert
            Assert.Equal(expectedHits, hits);
        }

        #endregion

        #region Calculate Roll To Wound Tests
        [Fact]
        public void CalculateRollToWound_Return2()
        {
            // Arrange
            int weaponStrength = 11;
            int defenderToughness = 5;
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int result = service.CalculateRollToWound(weaponStrength, defenderToughness);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void CalculateRollToWound_Return3()
        {
            // Arrange
            int weaponStrength = 7;
            int defenderToughness = 5;
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int result = service.CalculateRollToWound(weaponStrength, defenderToughness);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void CalculateRollToWound_Return4()
        {
            // Arrange
            int weaponStrength = 5;
            int defenderToughness = 5;
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int result = service.CalculateRollToWound(weaponStrength, defenderToughness);

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void CalculateRollToWound_Return5()
        {
            // Arrange
            int weaponStrength = 4;
            int defenderToughness = 5;
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int result = service.CalculateRollToWound(weaponStrength, defenderToughness);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void CalculateRollToWound_Return6()
        {
            // Arrange
            int weaponStrength = 2;
            int defenderToughness = 5;
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int result = service.CalculateRollToWound(weaponStrength, defenderToughness);

            // Assert
            Assert.Equal(6, result);
        }

        #endregion

        #region Roll For Saves Test

        [Fact]
        public void RollForSavesTest_AutoFailOn1()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(1);

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 5, InvulnSave = 4 };

            // Act
            int saves = service.RollForSaves(1, attacker, defender);

            // Assert
            Assert.Equal(0, saves);
        }

        [Fact]
        public void RollForSavesTest_SuccessfulSave()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(5);

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);
            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 4, InvulnSave = 4 };

            // Act
            int saves = service.RollForSaves(1, attacker, defender);
            // Assert
            Assert.Equal(1, saves);
        }

        [Fact]
        public void RollForSavesTest_FailedSave()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(3);

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);
            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 4, InvulnSave = 4 };

            // Act
            int saves = service.RollForSaves(1, attacker, defender);

            // Assert
            Assert.Equal(0, saves);
        }

        [Fact]
        public void RollForSavesTest_UseInvulnSave_SaveSuccessful()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(4);

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);
            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 5, InvulnSave = 4 };

            // Act
            int saves = service.RollForSaves(1, attacker, defender);

            // Assert
            Assert.Equal(1, saves);
        }

        [Fact]
        public void RollForSavesTest_UseInvulnSave_SaveFailed()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(3)

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);
            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 5, InvulnSave = 4 };

            // Act
            int saves = service.RollForSaves(1, attacker, defender);

            // Assert
            Assert.Equal(0, saves);
        }

        [Fact]
        public void RollForSavesTest_HighAp_SaveFailed()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(4);

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);
            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -5, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 4, InvulnSave = 4 };

            // Act
            int saves = service.RollForSaves(1, attacker, defender);

            // Assert
            Assert.Equal(0, saves);
        }

        #endregion
    }
}
