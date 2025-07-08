using System;

namespace SimHammer.Core.Models.Simulation;

public class WeaponResult
{
    public int Id { get; set; }
    public int SimNumber { get; set; } // Which sim round this represents (such as 146 out of 1000)
    public int AttacksMade { get; set; }
    public int Hits { get; set; }
    public int WoundsInflicted { get; set; }
    public int SavesMade { get; set; }
    public int InvulnSavesMade { get; set; }
    public int DamageDealt { get; set; } // Total damage dealt by this weapon result
    
    // Constructors
    public WeaponResult()
    {
        Id = 0;
        SimNumber = 0;
        AttacksMade = 0;
        Hits = 0;
        WoundsInflicted = 0;
        SavesMade = 0;
        InvulnSavesMade = 0;
        DamageDealt = 0;
    }

    public WeaponResult(int id, int simNumber, int attacksMade, int hits, int woundsInflicted, int savesMade, int invulnSavesMade, int modelsKilled, double moraleSuccessChance)
    {
        Id = id;
        SimNumber = simNumber;
        AttacksMade = attacksMade;
        Hits = hits;
        WoundsInflicted = woundsInflicted;
        SavesMade = savesMade;
        InvulnSavesMade = invulnSavesMade;
    }

    // Methods
    public double HitPercentage() => AttacksMade > 0 ? (double)Hits / AttacksMade * 100 : 0.0;

    public int Misses() => AttacksMade - Hits;
}
