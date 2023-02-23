using System;
using UnityEngine;

public class Move
{
    public readonly string Name;
    public readonly string Description;
    public readonly PocketMonster.Element Type;
    public readonly float Damage;
    public readonly float Accuracy;

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

    private Effect m_effect;

    public Move(string name, string description, PocketMonster.Element type, Effect effect, float damage, float accuracy)
    {
        Name = name;
        Description = description;
        Type = type;
        m_effect = effect;
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
