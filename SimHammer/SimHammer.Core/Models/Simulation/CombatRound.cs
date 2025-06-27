using System;

namespace SimHammer.Core.Models.Simulation;

public class CombatRound
{
    public int Id {get; set;}
    public int SimNumber {get; set;} // Which sim round this represents (such as 146 out of 1000)
    public int AttacksMade {get; set;}
    public int Hits {get; set;}
    public int WoundsInflicted {get; set;}
    public int SavesMade {get; set;}
    public int InvulnSavesMade {get; set;}
    public int ModelsKilled {get; set;}
    public double MoraleSuccessChance {get; set;} // Percentage chance of passing morale test

    // Constructors
    public CombatRound() 
    { 
        Id = 0;
        SimNumber = 0;
        AttacksMade = 0;
        Hits = 0;
        WoundsInflicted = 0;
        SavesMade = 0;
        InvulnSavesMade = 0;
        ModelsKilled = 0;
        MoraleSuccessChance = 0.0;
    }

    public CombatRound(int id, int simNumber, int attacksMade, int hits, int woundsInflicted, int savesMade, int invulnSavesMade, int modelsKilled, double moraleSuccessChance)
    {
        Id = id;
        SimNumber = simNumber;
        AttacksMade = attacksMade;
        Hits = hits;
        WoundsInflicted = woundsInflicted;
        SavesMade = savesMade;
        InvulnSavesMade = invulnSavesMade;
        ModelsKilled = modelsKilled;
        MoraleSuccessChance = moraleSuccessChance;
    }

    // Methods
    public double HitPercentage() => AttacksMade > 0 ? (double)Hits / AttacksMade * 100 : 0.0;

    public int Misses() => AttacksMade - Hits;


}
