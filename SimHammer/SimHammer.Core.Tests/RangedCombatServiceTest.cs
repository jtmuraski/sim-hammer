using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SimHammer.Core;
using SimHammer.Core.Services.Simulation;
using Microsoft.Extensions.Logging.Abstractions;

namespace SimHammer.Core.Tests
{
    public class RangedCombatServiceTest
    {
        // ***** Calculate Roll To Wound *****
        [Fact]
        public void CalculateRollToWound_Return2()
        {
            // Arrange
            int weaponStrength = 11;
            int defenderToughness = 5;
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger);

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
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger);

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
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger);

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
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger);

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
            var logger = new NullLogger<RangedCombatService>();
            var service = new RangedCombatService(logger);

            // Act
            int result = service.CalculateRollToWound(weaponStrength, defenderToughness);

            // Assert
            Assert.Equal(6, result);
        }
    }
}
