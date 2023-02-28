using System;
using UnityEngine;

public class Move
{
    public readonly string Name;
    public readonly string Description;
    public readonly PocketMonster.Element Type;
    public readonly int Damage;
    public readonly int Accuracy;

    public enum MoveEffect
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

    public readonly MoveEffect Effect;

    public Move(string name, string description, PocketMonster.Element type, MoveEffect moveEffect, int damage, int accuracy)
    {
        Name = name;
        Description = description;
        Type = type;
        Effect = moveEffect;
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
