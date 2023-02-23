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
            m_hp = hp;
            m_attack = attack;
            m_defense = defense;
            m_speed = speed;
        }

        public float m_hp;
        public float m_attack;
        public float m_defense;
        public float m_speed;

        public void Print()
        {
            Debug.Log($"HP: {m_hp}\tATK: {m_attack}\tDEF: {m_defense}\tSPD: {m_speed}");
        }
    }

    public string Name { get; }

    //private Ability m_ability;

    [SerializeField] private Stats m_stats;

    [SerializeField] private Move[] m_moves;

    public PocketMonster(string name, Element type, /*Ability ability, */ Stats stats, Move[] moves)
    {
        Name = name;
        Type = type;
        //m_ability = ability;
        m_stats = stats;
        m_moves = moves;
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
}
