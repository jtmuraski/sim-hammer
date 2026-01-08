using System;

namespace SimHammer.Core.Services.Simulation;

public static class DiceRoller
{
    private static readonly Random _random = new Random();

    public static int RollD6()
    {
        return RollDice(6);
    }

    public static int RollD6WithModifier(int modifier)
    {
        return RollD6() + modifier;
    }

    private static int RollDice(int sides)
    {
        if (sides < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(sides), "Number of sides must be at least 1.");
        }

        lock (_random)
        {
            return _random.Next(1, sides + 1);
        }
        
    }
}
