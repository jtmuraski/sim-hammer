using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SimHammer.Core;
using SimHammer.Core.Services.Simulation;
using Microsoft.Extensions.Logging.Abstractions;
using SimHammer.Core.Services.Interfaces;
using Moq;

namespace SimHammer.Core.Tests
{
    public class RangedCombatServiceTest
    {
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

        #region Roll For Hits Tests
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

        [Fact]
        public void RollForHitsTest_1Attack_HitOnRollEqualToSkill()
        {
            // Arrange
            var mockRoller = new Mock<IDiceRoller>();
            mockRoller.Setup(x => x.RollD6()).Returns(4);
            
        }

        #endregion
    }
}
