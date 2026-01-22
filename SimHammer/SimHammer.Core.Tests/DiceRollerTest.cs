using System;
using System.Collections.Generic;
using System.Text;
using SimHammer.Core;
using Xunit;
using SimHammer.Core.Services.Simulation;

namespace SimHammer.Core.Tests
{
    public class DiceRollerTest
    {
        [Fact]
        public void DiceRoller_RollD6_ReturnsValueBetween1And6()
        {
            // Arrange
            // This is a static class, so no instantiation is needed

            // Act
            int roll = DiceRoller.RollD6();

            // Assert
            Assert.InRange(roll, 1, 6);
        }
    }
}
