using System;
using UnityEngine;

public class Move
{
    public enum Effect
    {
        Damage,
        Heal,
        IncreaseAttack,
        DecreaseAttack,
        IncreaseAccuracy,
        DecreaseAccuracy,
        IncreaseDefense,
        DecreaseDefense,
        IncreaseSpeed,
        DecreaseSpeed,
        RaiseAllStats,
        Status
    }

    public enum StatChangeAffected
    {
        None,
        User,
        Target
    }

    public enum StatusEffect
    {
        None,
        Burn,
        Confuse,
        Flinch,
        Freeze,
        Paralyze,
        Poison,
        Sleep,
        // The move TriAttack has a chance to Burn, Freeze or Paralyze - since the StatusEffect enum is being used on the Pokemon-side and the Move-side, I am making it an entry here - Tom :)
        TriAttack
    }

    public enum Effectiveness
    {
        Immune,
        NotVeryEffective,
        Neutral,
        SuperEffective
    }

    public enum Outcome
    {
        Miss,
        Hit,
        CriticalHit
    }

    public readonly string Name;
    public readonly string Description;

    public readonly PocketMonster.Element Type;

    public readonly int Damage;
    public readonly int Accuracy;

    public readonly Effect MoveEffect;
    public readonly StatusEffect Status;
    public readonly int StatusEffectChance;

    public readonly StatChangeAffected AffectedStatChange;
    public readonly int StatChangeChance;

    public Move(string name, string description, PocketMonster.Element type, Effect moveEffect, StatusEffect statusEffect, int statusEffectChance, int damage, int accuracy, StatChangeAffected affectedStatChange, int statChangeChance)
    {
        Name = name;
        Description = description;

        Type = type;

        Damage = damage;
        Accuracy = accuracy;

        MoveEffect = moveEffect;
        Status = statusEffect;
        StatusEffectChance = statusEffectChance;

        AffectedStatChange = affectedStatChange;
        StatChangeChance = statChangeChance;
    }

    public override string ToString()
    {
        return Name;
    }

    public void Print()
    {
        Debug.Log(Name);
    }
}
