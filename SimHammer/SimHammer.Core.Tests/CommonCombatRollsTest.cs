using Castle.Core.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SimHammer.Core.Models.Units;
using SimHammer.Core.Services.Interfaces;
using SimHammer.Core.Services.Simulation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SimHammer.Core.Tests
{
    public class CommonCombatRollsTest
    {
        // ---Properties---
        private Mock<IDiceRoller> _mockRoller;
        private NullLogger<ICommonCombatRolls> _logger;

        // ---Constructors---
        public CommonCombatRollsTest()
        {
            _mockRoller = new Mock<IDiceRoller>();
            _logger = new NullLogger<ICommonCombatRolls>();
        }

        // ---Tests---

        [Theory]
        [InlineData(6, 1, 1, 4)] // Hit - Auto
        [InlineData(1, 0, 1, 4)] // Miss - Auto
        [InlineData(4, 1, 1, 4)] // Hit - Roll Equal to Ballistic Skill Level
        [InlineData(5, 1, 1, 3)] // Hit - Roll greater than Ballistic Skill Level
        [InlineData(2, 0, 1, 3)] // Miss - Roll less than Ballistic Skill Level
        public void RollForHitTest_1Attack_VariousResults(int diceRoll, int hitsExpected, int numAttacks, int ballisticSkillLevel)
        {
            // Arrange
            var service = new CommonCombatRolls(_logger, _mockRoller.Object);
            _mockRoller.Setup(x => x.RollD6()).Returns(diceRoll);
            // Act
            int hits = service.RollForHits(numAttacks, ballisticSkillLevel);

            // Assert
            Assert.Equal(hitsExpected, hits);
        }

        // Test condictions with multiple attacks, based on a Ballistic SKill of 4. 4 was chosen as a good median value
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

            var logger = new NullLogger<ICommonCombatRolls>();
            var service = new CommonCombatRolls(logger, mockRoller.Object);

            // Act
            int hits = service.RollForHits(rolls.Length, 4);

            // Assert
            Assert.Equal(expectedHits, hits);
        }

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
            var logger = new NullLogger<ICommonCombatRolls>();

            var service = new CommonCombatRolls(logger, mockRoller.Object);

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

            var logger = new NullLogger<ICommonCombatRolls>();
            var service = new CommonCombatRolls(logger, mockRoller.Object);

            // Act
            int wounds = service.RollForWounds(numHits, rollNeeded);

            // Arrange
            Assert.Equal(woundsExpected, wounds);
        }

        #endregion

        #region Roll For Saves Test

        [Theory]
        [InlineData(1, -1, 0, false, 1, 0, 1)] // Auto fail save on a roll of 1
        [InlineData(1, 0, 3, false, 1, 1, 4)] // Successful save on a roll greater than Save value
        [InlineData(1, 0, 5, false, 5, 0, 3)] // Failed save - roll less than save value
        [InlineData(1, -2, 5, true, 2, 1, 5)] // Successful save. Use Invuln Save due to weapon AP
        [InlineData(1, 0, 5, true, 3, 0, 2 )] // Fail save using Invuln Save
        public void RollForSavesTest(int woundsInflicted, int weaponApValue, int saveValue, bool hasInvuln, int invulnSaveValue, int expectedNbrSaves, int diceRoll)
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(diceRoll);

            var logger = new NullLogger<ICommonCombatRolls>();
            var service = new CommonCombatRolls(logger, mockRoller.Object);
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = saveValue, HasInvulnSave = hasInvuln, InvulnSave = invulnSaveValue };

            // Act
            int saves = service.RollForSaves(woundsInflicted, weaponApValue, defender);

            // Assert
            Assert.Equal(expectedNbrSaves, saves);
        }

        // Test Damage Calculation
        [Fact]
        public void CalculateDamageTest()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            var logger = new NullLogger<ICommonCombatRolls>();
            var service = new CommonCombatRolls(logger, mockRoller.Object);
            var rangedWeapon = new RangedWeapon("Bolter", 24, 2, 3, 4, -1, 1, 10);

            // Act
            int damage = service.CalculateDamage(1, 4);

            // Assert
            Assert.Equal(4, damage);
        }

        #endregion


       
    }
}
