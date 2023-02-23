using System;
using UnityEngine;

public class Move
{
    private string m_name;
    private string m_description;
    private PocketMonster.Element m_type;
    private float m_damage;
    private float m_accuracy;

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
        m_name = name;
        m_description = description;
        m_type = type;
        m_effect = effect;
        m_damage = damage;
        m_accuracy = accuracy;
    }

    public override string ToString()
    {
        return m_name;
    }

    public void Print()
    {
        Debug.Log(m_name);
    }
}
