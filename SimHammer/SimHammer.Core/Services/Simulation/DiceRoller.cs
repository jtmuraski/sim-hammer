using System;

namespace SimHammer.Core.Services.Simulation;

public static class DiceRoller
{
    public static int RollD6()
    {
        return RollDice(6);
    }

    public static int RollD6WithModifier(int modifier)
    {
        int roll = RollDice(6);
        return roll + modifier;
    }

    private static int RollDice(int sides)
    {
        if (sides < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(sides), "Number of sides must be at least 1.");
        }

        Random random = new Random();
        return random.Next(1, sides + 1);
    }
}
