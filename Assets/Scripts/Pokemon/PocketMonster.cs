using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PocketMonster
{
    public enum Element
    {
        Bug,
        Dark,
        Dragon,
        Electric,
        Fire,
        Fighting,
        Flying,
        Ghost,
        Grass,
        Ground,
        Ice,
        Normal,
        Poison,
        Psychic,
        Rock,
        Steel,
        Water
    }

    public Element Type { get; }

    public class Stats
    {
        public Stats(float hp, float attack, float defense, float speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
        }

        public float HP;
        public float Attack;
        public float Defense;
        public float Speed;

        public void Print()
        {
            Debug.Log($"HP: {HP}\tATK: {Attack}\tDEF: {Defense}\tSPD: {Speed}");
        }
    }

    public string Name { get; }

    //private Ability m_ability;

    private Stats m_stats;

    private Move[] m_moves;

    private GameObject m_model;

    public PocketMonster(string name, Element type, /*Ability ability, */ Stats stats, Move[] moves)
    {
        Name = name;
        Type = type;
        //m_ability = ability;
        m_stats = stats;
        m_moves = moves;
    }

    public void SetMesh(GameObject model)
    {
        m_model = model;
    }

    public static Element StringToType(string type)
    {
        return type switch
        {
            "Bug" => Element.Bug,
            "Dark" => Element.Dark,
            "Dragon" => Element.Dragon,
            "Electric" => Element.Electric,
            "Fighting" => Element.Fighting,
            "Fire" => Element.Fire,
            "Flying" => Element.Flying,
            "Ghost" => Element.Ghost,
            "Grass" => Element.Grass,
            "Ground" => Element.Ground,
            "Ice" => Element.Ice,
            "Typeless" or "Normal" => Element.Normal,
            "Poison" => Element.Poison,
            "Psychic" => Element.Psychic,
            "Rock" => Element.Rock,
            "Steel" => Element.Steel,
            "Water" => Element.Water,
            _ => throw new ArgumentOutOfRangeException(type, "Make sure the type is supported!")
        };
    }

    public static string TypeToString(Element type)
    {
        return type switch
        {
            Element.Bug => "Bug",
            Element.Dark => "Dark",
            Element.Dragon => "Dragon",
            Element.Electric => "Electric",
            Element.Fire => "Fire",
            Element.Fighting => "Fighting",
            Element.Flying => "Flying",
            Element.Ghost => "Ghost",
            Element.Grass => "Grass",
            Element.Ground => "Ground",
            Element.Ice => "Ice",
            Element.Normal => "Normal",
            Element.Poison => "Poison",
            Element.Psychic => "Psychic",
            Element.Rock => "Rock",
            Element.Steel => "Steel",
            Element.Water => "Water",
            _ => throw new ArgumentOutOfRangeException(type.ToString(), "Make sure the type is supported!")
        };
    }

    public void Print()
    {
        Debug.Log($"Name: {Name}");
        Debug.Log($"Type: {TypeToString(Type)}");
        m_stats.Print();
        foreach (Move move in m_moves)
        {
            move.Print();
        }
    }

    public float GetHP()
    {
        return m_stats.HP;
    }

    public float GetAttackStat()
    {
        return m_stats.Attack;
    }

    public float GetDefenseStat()
    {
        return m_stats.Defense;
    }

    public float GetSpeedStat()
    {
        return m_stats.Speed;
    }

    public Move[] GetMoves()
    {
        return m_moves;
    }
}
