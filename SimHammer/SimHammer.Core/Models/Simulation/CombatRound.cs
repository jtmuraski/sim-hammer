using System;

namespace SimHammer.Core.Models.Simulation;

public class CombatRound
{
    public int Id {get; set;}
    public int SimNumber {get; set;} // Which sim round this represents (such as 146 out of 1000)
    public int TotalAttacksMade => WeaponResults.Sum(wr => wr.AttacksMade); // Total attacks made across all weapon results
    public int TotalHits => WeaponResults.Sum(wr => wr.Hits);
    public int TotalWoundsInflicted => WeaponResults.Sum(wr => wr.WoundsInflicted); // Total wounds inflicted from WeaponResults
    public int TotalSavesMade => WeaponResults.Sum(wr => wr.SavesMade);
    public int TotalInvulnSavesMade => WeaponResults.Sum(wr => wr.InvulnSavesMade);
    public int TotalDamageDealt => WeaponResults.Sum(wr => wr.DamageDealt);
    public int ModelsKilled {get; set;}
    public double MoraleSuccessChance {get; set;} // Percentage chance of passing morale test
    public List<WeaponResult> WeaponResults { get; set; } = new List<WeaponResult>();

    // Constructors
    public CombatRound()
    {
        Id = 0;
        SimNumber = 0;
        ModelsKilled = 0;
        MoraleSuccessChance = 0.0;
    }

    // Methods
  


}
