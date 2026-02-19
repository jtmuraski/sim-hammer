using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Interfaces;
using SimHammer.Core.Services.Simulation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimHammer.Core.Tests
{
    public class CommonCombatRollsTest
    {
        [Theory]
        [InlineData(6, 1, 1, 4)] // Hit - Auto
        [InlineData(1, 0, 1, 4)] // Miss - Auto
        [InlineData(4, 1, 1, 4)] // Hit - Roll Equal to Ballistic Skill Level
        [InlineData(5, 1, 1, 3)] // Hit - Roll greater than Ballistic Skill Level
        [InlineData(2, 0, 1, 3)] // Miss - Roll less than Ballistic Skill Level
        public void RollForHitTest_1Attack_VariousResults(int diceRoll, int hitsExpected, int numAttacks, int ballisticSkillLevel)
        {
            // Arrnage
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(diceRoll);

            var logger = new NullLogger<ICommonCombatRolls>();
            var service = new CommonCombatRolls(logger, mockRoller.Object);


            // Act
            int hits = service.RollForHits(numAttacks, ballisticSkillLevel);

            // Assert
            Assert.Equal(hitsExpected, hits);
        }

        // Test condictions with multiple attacks
        [Theory]
        [InlineData(new int[] { 1, 2, 3, 4, 5, 6 }, 3)] // 3 hits (4,5,6)
        [InlineData(new int[] { 1, 1, 1 }, 0)] // All auto misses
        [InlineData(new int[] { 6, 6, 6, 6 }, 4)] // All auto hits)
        public void RollForHitsTest_MultipleAttacks_VariousHits(int[] rolls, int expectedHits)
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            var sequence = mockRoller.SetupSequence(x => x.RollD6());
            foreach (var roll in rolls)
            {
                sequence.Returns(roll);
            }

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int hits = service.RollForHits(rolls.Length, 4);

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
            var logger = new NullLogger<IRangedCombatService>();
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
            var logger = new NullLogger<IRangedCombatService>();
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
            var logger = new NullLogger<IRangedCombatService>();
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
            var logger = new NullLogger<IRangedCombatService>();
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
            var logger = new NullLogger<IRangedCombatService>();
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
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 5, InvulnSave = 3 }; // Since the Invuln save is better(lower) than the normal save, it should be used instead

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
            mockRoller.Setup(x => x.RollD6()).Returns(2);

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);
            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 5, InvulnSave = 3 };

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
            var attacker = new RangedWeapon("Bolter", 24, 2, 3, 4, -3, 1, 10);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 2, InvulnSave = 0 };

            // Act
            int saves = service.RollForSaves(1, attacker, defender);

            // Assert
            Assert.Equal(0, saves);
        }

        // Test Damage Calculation
        [Fact]
        public void CalculateDamagetest()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);
            var rangedWeapon = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);

            // Act
            int damage = service.CalculateDamage(rangedWeapon, 4);

            // Assert
            Assert.Equal(4, damage);
        }

        #endregion


        #region Test CalculateRollToWound
        [Theory]
        [InlineData(5, 2, 2)]
        [InlineData(5, 4, 3)]
        [InlineData(4, 4, 4)]
        [InlineData(2, 6, 6)]
        [InlineData(3, 4, 5)]
        public void CalculateRollToWoundTest_VariousStrengths(int weaponStrength, int defenderToughness, int expectedRollToWound)
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<IRangedCombatService>();

            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int result = service.CalculateRollToWound(weaponStrength, defenderToughness);

            // Assert
            Assert.Equal(expectedRollToWound, result);
        }

        #endregion

        #region Test RollForWounds

        [Theory]
        [InlineData(1, 3, new int[] { 6 }, 1)] // Auto wound on 6
        [InlineData(1, 3, new int[] { 5 }, 1)] // 1 Wound caused on roll greater than needed
        [InlineData(1, 3, new int[] { 3 }, 1)] // 1 Wound caused on roll equal to needed roll
        [InlineData(1, 3, new int[] { 1 }, 0)] // No wound casued on roll less than needed
        [InlineData(2, 3, new int[] { 5, 2 }, 1)] // 1 wound caused on 2 hits, 1 success and 1 fail
        public void RollForWoundsTest_VariousRollNeeded(int numHits, int rollNeeded, int[] diceRolls, int woundsExpected)
        {
            // Act
            var mockRoller = new Mock<IDiceRoller>();
            var sequence = mockRoller.SetupSequence(x => x.RollD6());
            for (int i = 0; i < diceRolls.Length; i++)
            {
                sequence.Returns(diceRolls[i]);
            }

            var logger = new NullLogger<IRangedCombatService>();
            var service = new RangedCombatService(logger, mockRoller.Object);

            // Act
            int wounds = service.RollForWounds(numHits, rollNeeded);

            // Arrange
            Assert.Equal(woundsExpected, wounds);
        }

        #endregion
    }
}
