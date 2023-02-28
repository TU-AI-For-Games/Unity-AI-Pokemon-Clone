using System;
using UnityEngine;

public class Move
{
    public readonly string Name;
    public readonly string Description;
    public readonly PocketMonster.Element Type;
    public readonly int Damage;
    public readonly int Accuracy;

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
        Status
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

    public readonly Effect MoveEffect;

    public Move(string name, string description, PocketMonster.Element type, Effect moveEffect, int damage, int accuracy)
    {
        Name = name;
        Description = description;
        Type = type;
        MoveEffect = moveEffect;
        Damage = damage;
        Accuracy = accuracy;
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
