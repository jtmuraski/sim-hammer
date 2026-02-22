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
    public class MeleeCombatServiceTest
    {
        // ---Propeties---
        private NullLogger<ICommonCombatRolls> _commonCombatRollsLogger;
        private NullLogger<IMeleeCombatService> _meleeLogger;
        private Mock<IDiceRoller> _mockCombatRolls;
        private Mock<IDiceRoller> _mockRangedRoller;
        private CommonCombatRolls? _commonCombatRolls;

        public MeleeCombatServiceTest()
        {
            _commonCombatRollsLogger = new NullLogger<ICommonCombatRolls>();
            _meleeLogger = new NullLogger<IMeleeCombatService>();
            _mockCombatRolls = new Mock<IDiceRoller>();
            _mockRangedRoller = new Mock<IDiceRoller>();
        }

        #region Full SimulateRangedCombat Integration Testing
        [Fact]
        public void SimulateMeleeCombatRoundTest_FullIntegration_1Weapon2Attacks()
        {
            // Arrange
            _mockCombatRolls.SetupSequence(x => x.RollD6())
                // Hit Rolls (2 attacks - 1 hit, 1miss
                .Returns(4) // Hit
                .Returns(2) // Miss
                            // Wound Rolls (1 hit - 1 wound)
                .Returns(5) // Wound roll success
                            // Save Rolls (1 wound - 1 failed save)
                .Returns(2); // Save fails, damage occurs
            _commonCombatRolls = new CommonCombatRolls(_commonCombatRollsLogger, _mockCombatRolls.Object);

            var service = new MeleeCombatService(_meleeLogger, _commonCombatRolls);

            var attacker = new Unit() { Name = "Attacker", Toughness = 5, Save = 4, InvulnSave = 4 };
            attacker.MeleeWeapons.Add(new MeleeWeapon("Chainsword", 12, 2, 3, 4, 0, 1, 1));
            var defender = new Unit() { Name = "Defender", Toughness = 5, Save = 4, HasInvulnSave = true, InvulnSave = 4, Wounds = 1, ModelCount = 5 };

            // Act
            CombatRound roundResult = service.SimulateMeleeCombatRound(attacker, defender, 1);

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
